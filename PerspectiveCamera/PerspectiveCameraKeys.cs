using System;
using UnityEngine;

namespace PerspectiveCamera
{
    public class PerspectiveCameraKeys : MonoBehaviour
    {
        public bool AllowMove;
        public bool AllowFastMove;
        public float FastMoveSpeed;
        public KeyCode FastMoveKeyCode1;
        public KeyCode FastMoveKeyCode2;

        public bool AllowRotate;

        public bool AllowZoom;

        public bool AllowTilt;
        public float TiltSpeed;

        public KeyCode ResetKey;
        public bool IncludePositionOnReset;

        public bool MovementBreaksFollow;

        public string HorizontalInputAxis = "Horizontal";
        public string VerticalInputAxis = "Vertical";

        public bool RotateUsesInputAxis = false;
        public string RotateInputAxis = "KbCameraRotate";
        public KeyCode RotateLeftKey = KeyCode.Q;
        public KeyCode RotateRightKey = KeyCode.E;

        public bool ZoomUsesInputAxis = false;
        public string ZoomInputAxis = "KbCameraZoom";
        public KeyCode ZoomOutKey = KeyCode.Z;
        public KeyCode ZoomInKey = KeyCode.X;

        public bool TiltUsesInputAxis = false;
        public string TiltInputAxis = "KbCameraTilt";
        public KeyCode TiltUpKey = KeyCode.R;
        public KeyCode TiltDownKey = KeyCode.F;

        //

        private PerspectiveCamera _camera;

        //

        public void Reset()
        {
            AllowMove = true;

            AllowFastMove = true;
            FastMoveSpeed = 40f;
            FastMoveKeyCode1 = KeyCode.LeftShift;
            FastMoveKeyCode2 = KeyCode.RightShift;

            AllowRotate = true;

            AllowZoom = true;

            AllowTilt = true;
            TiltSpeed = 90f;

            ResetKey = KeyCode.Home;
            IncludePositionOnReset = true;

            MovementBreaksFollow = true;
        }

        protected void Start()
        {
            _camera = gameObject.GetComponent<PerspectiveCamera>();
        }

        protected void Update()
        {
            try
            {
                float num = 0.02f;
                float distanceMultiplier = _camera.Distance * 7 / _camera.MaxDistance + 0.1f;
                float distanceModifier = Mathf.Sqrt(_camera.Distance / _camera.MaxDistance) * 2;

                var moveSpeed = PerspectiveCameraSettings.Instance.MoveSpeed;
                FastMoveSpeed = moveSpeed * 2;
                var rotateSpeed = PerspectiveCameraSettings.Instance.RotateSpeed - 100f;
                var zoomSpeed = PerspectiveCameraSettings.Instance.ZoomSpeed;
                TiltSpeed = 90f;

                if (_camera == null || UIUtility.isInputFieldFocused())
                    return; // no camera!... or is he typing? who cares, bail!

                if (AllowMove && (!_camera.IsFollowing || MovementBreaksFollow))
                {
                    var hasMovement = false;

                    var speed = moveSpeed;
                    if (AllowFastMove && (Input.GetKey(FastMoveKeyCode1) || Input.GetKey(FastMoveKeyCode2)))
                    {
                        speed = FastMoveSpeed;
                    }

                    var h = Input.GetAxisRaw(HorizontalInputAxis);
                    if (Mathf.Abs(h) > 0.001f)
                    {
                        hasMovement = true;
                        _camera.AddToPosition(h * speed * num * distanceModifier, 0, 0);
                    }

                    var v = Input.GetAxisRaw(VerticalInputAxis);
                    if (Mathf.Abs(v) > 0.001f)
                    {
                        hasMovement = true;
                        _camera.AddToPosition(0, 0, v * speed * num * distanceModifier);
                    }

                    if (hasMovement && _camera.IsFollowing && MovementBreaksFollow)
                        _camera.EndFollow();
                }


                if (AllowRotate)
                {
                    if (RotateUsesInputAxis)
                    {
                        var rot = Input.GetAxisRaw(RotateInputAxis);
                        if (Mathf.Abs(rot) > 0.001f)
                        {
                            _camera.Rotation += rot * rotateSpeed * num;
                        }
                    }
                    else
                    {
                        if (Input.GetKey(RotateLeftKey))
                        {
                            _camera.Rotation += rotateSpeed * num;
                        }

                        if (Input.GetKey(RotateRightKey))
                        {
                            _camera.Rotation -= rotateSpeed * num;
                        }
                    }
                }

                if (AllowZoom)
                {
                    if (ZoomUsesInputAxis)
                    {
                        var zoom = Input.GetAxisRaw(ZoomInputAxis);
                        if (Mathf.Abs(zoom) > 0.001f)
                        {
                            _camera.Distance += zoom * zoomSpeed * num * distanceMultiplier;
                        }
                    }
                    else
                    {
                        if (Input.GetKey(ZoomOutKey))
                        {
                            _camera.Distance += zoomSpeed * num * distanceMultiplier;
                        }

                        if (Input.GetKey(ZoomInKey))
                        {
                            _camera.Distance -= zoomSpeed * num * distanceMultiplier;
                        }
                    }
                }

                if (AllowTilt)
                {
                    if (TiltUsesInputAxis)
                    {
                        var tilt = Input.GetAxisRaw(TiltInputAxis);
                        if (Mathf.Abs(tilt) > 0.001f)
                        {
                            _camera.Tilt += tilt * TiltSpeed * num;
                        }
                    }
                    else
                    {
                        if (Input.GetKey(TiltUpKey))
                        {
                            _camera.Tilt += TiltSpeed * num;
                        }

                        if (Input.GetKey(TiltDownKey))
                        {
                            _camera.Tilt -= TiltSpeed * num;
                        }
                    }
                }

                if (ResetKey != KeyCode.None)
                {
                    if (Input.GetKeyDown(ResetKey))
                    {
                        _camera.ResetToInitialValues(IncludePositionOnReset, false);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}