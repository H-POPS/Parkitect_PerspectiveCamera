using System;
using System.Data;
using System.Reflection;
using Parkitect.UI;
using PostFX;
using UnityEngine;

namespace PerspectiveCamera
{
    public class PerspectiveCamera : CameraController, IUpdateEvent
    {
        public bool ShowSettings = true;
        public Vector3 LookAt;
        public float Distance;
        public float Rotation;
        public float Tilt;
        
        public float MoveDampening;
        public float ZoomDampening;
        public float RotationDampening;
        public float TiltDampening;

        public Vector3 MinBounds;
        public Vector3 MaxBounds;

        public float MinDistance;
        public float MaxDistance;

        public float MinTilt;
        public float MaxTilt;

        public bool TerrainHeightViaPhysics;
        public LayerMask TerrainPhysicsLayerMask;

        public float LookAtHeightOffset;

        public bool TargetVisbilityViaPhysics;
        public float CameraRadius;
        public LayerMask TargetVisibilityIgnoreLayerMask;

        public bool FollowBehind;
        public float FollowRotationOffset;

        public Action<Transform> OnBeginFollow;
        public Action<Transform> OnEndFollow;

        public bool ShowDebugCameraTarget;


        private Vector3 _initialLookAt;
        private float _initialDistance;
        private float _initialRotation;
        private float _initialTilt;

        private float _currDistance;
        private float _currRotation;
        private float _currTilt;

        private Vector3 _moveVector;

        private GameObject _target;
        private MeshRenderer _targetRenderer;

        private Transform _followTarget;

        private bool _lastDebugCamera;
        private PerspectiveCameraKeys _keysScript;
        private PerspectiveCameraMouse _mouseScript;
        public Camera Camera;
        private TiltShift _tiltShift;

        private FieldInfo animatePanToCoroutineField;
        private FieldInfo cameraFocusPointField;

        public void Reset()
        {
            _lastDebugCamera = true;
            LookAtHeightOffset = .2f;
            TerrainPhysicsLayerMask = 1 << 12;

            TargetVisbilityViaPhysics = false;
            CameraRadius = 1f;
            TargetVisibilityIgnoreLayerMask = 0;

            LookAt = new Vector3(10, 0, 22.5f);
            MoveDampening = 5f;
            MinBounds = new Vector3(.6f, 3, .6f);

            Distance = 10f;
            MinDistance = .2f;
            MaxDistance = 42f;
            ZoomDampening = 5f;

            Rotation = -90f;
            RotationDampening = 5f;

            Tilt = 45f;
            MinTilt = 2f;
            MaxTilt = 85f;
            TiltDampening = 5f;

            FollowBehind = false;
            FollowRotationOffset = 0;
        }

        protected void Start()
        {
            ObjectEventManager.remove(this);
            ObjectEventManager.add(this);
            _tiltShift = Camera.main.GetComponent<TiltShift>();
            _keysScript = GetComponent<PerspectiveCameraKeys>();
            _keysScript = GetComponent<PerspectiveCameraKeys>();
            _mouseScript = GetComponent<PerspectiveCameraMouse>();
            Camera = GetComponent<Camera>();


            var prop = this.GetType().BaseType?.GetField("controlledCamera", BindingFlags.NonPublic
                                                                             | BindingFlags.Instance);
            if (prop != null)
                prop.SetValue(this, Camera);

            Reset();
            _keysScript.Reset();
            _mouseScript.Reset();

            if (GetComponent<Rigidbody>())
                GetComponent<Rigidbody>().freezeRotation = true;


            CreateTarget();
            Park park = GameController.Instance.park;
            MaxBounds = new Vector3(park.xSize - .6f, park.ySize, park.zSize - .6f);

            CullingGroupManager.Instance.setTargetCamera(Camera);


            EventManager.Instance.OnNightModeChanged += Instance_OnNightModeChanged;

            if (park.parkEntrances.Count > 0)
            {
                LookAt = park.parkEntrances[0].transform.position;
            }

            _initialLookAt = LookAt;
            _initialDistance = Distance;
            _initialRotation = Rotation;
            _initialTilt = Tilt;

            _currDistance = Distance;
            _currRotation = Rotation;
            _currTilt = Tilt;

            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            animatePanToCoroutineField = typeof(CameraController).GetField("animatePanToCoroutine", flags);
            cameraFocusPointField = typeof(CameraController).GetField("cameraFocusPoint", flags);
        }

        private void Instance_OnNightModeChanged(bool isNightMode)
        {
            if (isNightMode)
            {
                Camera.clearFlags = CameraClearFlags.SolidColor;
                Camera.backgroundColor = Color.black;
            }
            else
            {
                Camera.clearFlags = CameraClearFlags.Skybox;
            }
        }

        public new void eventUpdate()
        {
            // Retrieve the value of the field, and cast as necessary
            var animatePanToCoroutine = (Coroutine) animatePanToCoroutineField.GetValue(this);
            if (animatePanToCoroutine != null)
            {
                StopCoroutine(animatePanToCoroutine);
                LookAt = (Vector3) cameraFocusPointField.GetValue(this);
                animatePanToCoroutineField.SetValue(this, null);
            }

            _followTarget = lockedOnto?.transform;

            var park = GameController.Instance.park;
            MaxBounds = new Vector3(park.xSize - .6f, park.ySize, park.zSize - .6f);
            if (Input.GetKeyUp(Settings.Instance.getKeyMapping("H-POPS@PerspectiveCamera/setting")))
            {
                ShowSettings = !ShowSettings;
            }

            if (Input.GetMouseButton(2))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            var smoothness = PerspectiveCameraSettings.Instance.Smoothness;
            MoveDampening = smoothness;
            ZoomDampening = smoothness;
            RotationDampening = smoothness;
            TiltDampening = smoothness;



            if (_lastDebugCamera != ShowDebugCameraTarget)
            {
                if (_targetRenderer != null)
                {
                    _targetRenderer.enabled = ShowDebugCameraTarget;
                    _lastDebugCamera = ShowDebugCameraTarget;
                }
            }

            Camera.nearClipPlane = .1f;


            if (PerspectiveCameraSettings.Instance.TargetFrameRate <= 0)
            {
                Camera.farClipPlane = PerspectiveCameraSettings.Instance.RenderDistance;
            }
            else if (Time.timeScale != 0)
            {
                AdaptFarClipPaneToFps();
            }

            LUpdate();
        }


        private void AdaptFarClipPaneToFps()
        {
            var fps = 1.0f / Time.deltaTime;
            if (fps < PerspectiveCameraSettings.Instance.TargetFrameRate - 5) Camera.farClipPlane = Math.Max(60, Camera.farClipPlane - 30f * Time.deltaTime);
            if (fps > PerspectiveCameraSettings.Instance.TargetFrameRate) Camera.farClipPlane = Math.Min(260, Camera.farClipPlane + 20f * Time.deltaTime);
        }
        protected void LUpdate()
        {
            //camera.farClipPlane = Distance * 1.5f + 100;
            float num = 0.02f;
            if (IsFollowing)
            {
                LookAt = _followTarget.position;
            }
            else
            {
                _moveVector.y = 0;
                LookAt += Quaternion.Euler(0, Rotation, 0) * _moveVector;

                var tool = GameController.Instance.getActiveMouseTool();

                if (tool == null || tool.GetType() != typeof(Terraformer) || !Input.GetMouseButton(0))
                    LookAt.y = GetHeightAt(LookAt.x, LookAt.z);
                else
                    LookAt.y -= LookAtHeightOffset; 
            }

            LookAt.y += LookAtHeightOffset;


            Tilt = Mathf.Clamp(Tilt, MinTilt, MaxTilt);
            Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
            LookAt = new Vector3(Mathf.Clamp(LookAt.x, MinBounds.x, MaxBounds.x),
                Mathf.Clamp(LookAt.y, MinBounds.y, MaxBounds.y), Mathf.Clamp(LookAt.z, MinBounds.z, MaxBounds.z));

            if (PerspectiveCameraSettings.Instance.Smoothing)
            {
                _currRotation = Mathf.LerpAngle(_currRotation, Rotation, num * RotationDampening);
                _currDistance = Mathf.Lerp(_currDistance, Distance, num * ZoomDampening);
                _currTilt = Mathf.LerpAngle(_currTilt, Tilt, num * TiltDampening);
                _target.transform.position = Vector3.Lerp(_target.transform.position, LookAt, num * MoveDampening);
            }
            else
            {
                _currRotation = Rotation;
                _currDistance = Distance;
                _currTilt = Tilt;
                _target.transform.position = LookAt;
            }

            _moveVector = Vector3.zero;

            if (IsFollowing && FollowBehind)
            {
                ForceFollowBehind();
            }

            if (IsFollowing && TargetVisbilityViaPhysics && DistanceToTargetIsLessThan(1f))
            {
                EnsureTargetIsVisible();
            }

            // setZoomPercentage(Mathf.InverseLerp(MinDistance, MaxDistance, Distance));

            UpdateCamera();
            UpdateZoom();
            if (_dynMethod != null) _dynMethod.Invoke(this, new object[] { });
        }


        public Transform CameraTarget
        {
            get { return _target.transform; }
        }


        public bool IsFollowing
        {
            get { return FollowTarget != null; }
        }


        public Transform FollowTarget
        {
            get { return _followTarget; }
        }


        public void ResetToInitialValues(bool includePosition, bool snap)
        {
            if (includePosition)
                LookAt = _initialLookAt;

            Distance = _initialDistance;
            Rotation = _initialRotation;
            Tilt = _initialTilt;

            if (snap)
            {
                _currDistance = Distance;
                _currRotation = Rotation;
                _currTilt = Tilt;
                _target.transform.position = LookAt;
            }
        }


        public void JumpTo(Vector3 toPosition, bool snap)
        {
            EndFollow();

            LookAt = toPosition;

            if (snap)
            {
                _target.transform.position = toPosition;
            }
        }


        public void JumpTo(Transform toTransform, bool snap)
        {
            JumpTo(toTransform.position, snap);
        }


        public void JumpTo(GameObject toGameObject, bool snap)
        {
            JumpTo(toGameObject.transform.position, snap);
        }

        public new float GetZoomPercentage()
        {
            return Mathf.InverseLerp(this.MinDistance, this.MaxDistance, this.Distance);
        }

        private new void UpdateZoom()
        {
            UpdateAudioMix();
            UpdateTiltShift();
        }

        public new void UpdateAudioMix()
        {
            if (AudioController.Instance != null)
            {
                AudioController.Instance.setZoomLevel(GetZoomPercentage());
            }
        }

        public new void UpdateTiltShift()
        {
            if (_tiltShift != null)
            {
                float num = 1f - GetZoomPercentage();
                float graphicsTiltShiftIntensity = Settings.Instance.graphicsTiltShiftIntensity;
                float num2 = Mathf.Lerp(graphicsTiltShiftIntensity * 0.3f, graphicsTiltShiftIntensity, num * num * num);
                _tiltShift.enabled = (num2 > 0f);
                _tiltShift.Area = num2;
            }
        }

        
        public void Follow(Transform followTarget, bool snap)
        {
            if (_followTarget != null)
            {
                OnEndFollow?.Invoke(_followTarget);
            }

            _followTarget = followTarget;

            if (_followTarget == null) return;
            if (snap)
            {
                LookAt = _followTarget.position;
            }

            OnBeginFollow?.Invoke(_followTarget);
        }

        private MethodInfo _dynMethod;


        public void Follow(GameObject followTarget, bool snap)
        {
            Follow(followTarget.transform, snap);
        }


        public void EndFollow()
        {
            base.lockOnto(null);
            Follow((Transform) null, false);
        }


        public void AddToPosition(float dx, float dy, float dz)
        {
            _moveVector += new Vector3(dx, dy, dz);
        }


        private float GetHeightAt(float x, float z)
        {
            if (PerspectiveCameraSettings.Instance.FollowTerrain)
            {
                var y = MaxBounds.y;
                var maxDist = MaxBounds.y - MinBounds.y + 1f;

                RaycastHit hitInfo;
                if (Physics.Raycast(new Vector3(x, y, z), new Vector3(0, -1, 0), out hitInfo, maxDist,
                    TerrainPhysicsLayerMask))
                {
                    return hitInfo.point.y;
                }

                return 0;
            }


            return 0;
        }


        private void UpdateCamera()
        {
            var rotation = Quaternion.Euler(_currTilt, _currRotation, 0);
            var v = new Vector3(0.0f, 0.0f, -_currDistance);
            var position = rotation * v + _target.transform.position;

            if (Camera.orthographic)
            {
                Camera.orthographicSize = _currDistance;
            }


            transform.rotation = rotation;
            transform.position = position;
        }


        private void CreateTarget()
        {
            _target = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _target.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            _target.GetComponent<Renderer>().material.color = Color.green;

            var targetCollider = _target.GetComponent<Collider>();
            if (targetCollider != null)
            {
                targetCollider.enabled = false;
            }

            _targetRenderer = _target.GetComponent<MeshRenderer>();
            _targetRenderer.enabled = false;

            _target.name = "CameraTarget";
            _target.transform.position = LookAt;
        }

        private bool DistanceToTargetIsLessThan(float sqrDistance)
        {
            if (!IsFollowing)
                return true;

            var p1 = _target.transform.position;
            var p2 = _followTarget.position;
            p1.y = p2.y = 0;
            var v = p1 - p2;
            var vd = v.sqrMagnitude;

            return vd < sqrDistance;
        }

        private void EnsureTargetIsVisible()
        {
            var direction = (transform.position - _target.transform.position);
            direction.Normalize();

            var distance = Distance;

            RaycastHit hitInfo;


            if (Physics.SphereCast(_target.transform.position, CameraRadius, direction, out hitInfo, distance,
                ~TargetVisibilityIgnoreLayerMask))
            {
                if (hitInfo.transform != _target)
                {
                    _currDistance = hitInfo.distance - 0.1f;
                }
            }
        }

        private void ForceFollowBehind()
        {
            var v = _followTarget.transform.forward * -1;
            var angle = Vector3.Angle(Vector3.forward, v);
            var sign = (Vector3.Dot(v, Vector3.right) > 0.0f) ? 1.0f : -1.0f;
            _currRotation = Rotation = 180f + (sign * angle) + FollowRotationOffset;
        }
    }
}