using System;
using UnityEngine;

namespace PerspectiveCamera
{
    internal class PerspectiveCameraMouse : MonoBehaviour
    {
        public KeyCode MouseOrbitButton;


        public bool ScreenEdgeMoveBreaksFollow;
        public int ScreenEdgeBorderWidth;
        public float MoveSpeed;

        public bool AllowPan;
        public bool PanBreaksFollow;
        public float PanSpeedVar;
        public float PanSpeed;

        public bool AllowRotate;

        public bool AllowTilt;
        public float TiltSpeedVar;
        public float TiltSpeed;

        public bool AllowZoom;

        public string RotateInputAxis = "Mouse X";
        public string TiltInputAxis = "Mouse Y";
        public string ZoomInputAxis = "Mouse ScrollWheel";
        public KeyCode PanKey1 = KeyCode.LeftShift;
        public KeyCode PanKey2 = KeyCode.RightShift;

        //

        private PerspectiveCamera _camera;

        //

        public void Reset()
        {
            MouseOrbitButton = KeyCode.Mouse2;


            ScreenEdgeMoveBreaksFollow = true;
            ScreenEdgeBorderWidth = 4;
            MoveSpeed = 10f;

            AllowPan = true;
            PanBreaksFollow = true;
            PanSpeedVar = 50f;
            PanKey1 = KeyCode.LeftShift;
            PanKey2 = KeyCode.RightShift;

            AllowRotate = true;

            AllowTilt = true;
            TiltSpeedVar = 200f;

            AllowZoom = true;
            

            RotateInputAxis = "Mouse X";
            TiltInputAxis = "Mouse Y";
            ZoomInputAxis = "Mouse ScrollWheel";
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
                float distanceMultiplier = _camera.Distance * 5 / _camera.MaxDistance + 0.1f;
                float distanceModifier = Mathf.Sqrt(_camera.Distance / _camera.MaxDistance);


                MoveSpeed = 20f;
                var rotateSpeed = PerspectiveCameraSettings.Instance.RotateSpeed;
                var zoomSpeed = PerspectiveCameraSettings.Instance.ZoomSpeed;
                TiltSpeed = TiltSpeedVar;
                PanSpeed = PanSpeedVar;


                if (_camera == null)
                    return;

                if (AllowZoom && !UIUtility.isMouseOverUIElement())
                {
                    var scroll = -Input.GetAxisRaw(ZoomInputAxis);
                    _camera.Distance -= scroll * zoomSpeed * num * distanceMultiplier;
                }

                if (Input.GetKey(MouseOrbitButton))
                {
                    if (AllowPan && (Input.GetKey(PanKey1) || Input.GetKey(PanKey2)))
                    {
                        var panX = -1 * Input.GetAxisRaw("Mouse X") * PanSpeed * num;
                        var panZ = -1 * Input.GetAxisRaw("Mouse Y") * PanSpeed * num;

                        _camera.AddToPosition(panX, 0, panZ);

                        if (PanBreaksFollow && (Mathf.Abs(panX) > 0.001f || Mathf.Abs(panZ) > 0.001f))
                        {
                            _camera.EndFollow();
                        }
                    }
                    else
                    {
                        if (AllowTilt)
                        {
                            var tilt = Input.GetAxisRaw(TiltInputAxis);
                            _camera.Tilt -= tilt * TiltSpeed * num;
                        }

                        if (AllowRotate)
                        {
                            var rot = Input.GetAxisRaw(RotateInputAxis);
                            _camera.Rotation += rot * rotateSpeed * num;
                        }
                    }
                }

                if (Settings.Instance.controlsEdgeScrolling && (!_camera.IsFollowing || ScreenEdgeMoveBreaksFollow))
                {
                    var hasMovement = false;

                    if (Input.mousePosition.y > (Screen.height - ScreenEdgeBorderWidth))
                    {
                        hasMovement = true;
                        _camera.AddToPosition(0, 0, MoveSpeed * num * distanceModifier);
                    }
                    else if (Input.mousePosition.y < ScreenEdgeBorderWidth)
                    {
                        hasMovement = true;
                        _camera.AddToPosition(0, 0, -1 * MoveSpeed * num * distanceModifier);
                    }

                    if (Input.mousePosition.x > (Screen.width - ScreenEdgeBorderWidth))
                    {
                        hasMovement = true;
                        _camera.AddToPosition(MoveSpeed * num * distanceModifier, 0, 0);
                    }
                    else if (Input.mousePosition.x < ScreenEdgeBorderWidth)
                    {
                        hasMovement = true;
                        _camera.AddToPosition(-1 * MoveSpeed * num * distanceModifier, 0, 0);
                    }

                    if (hasMovement && _camera.IsFollowing && ScreenEdgeMoveBreaksFollow)
                    {
                        _camera.EndFollow();
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