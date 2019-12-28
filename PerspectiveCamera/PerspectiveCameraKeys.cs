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

        public bool IncludePositionOnReset;

        public bool MovementBreaksFollow;

        private PerspectiveCamera _camera;

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

            TiltSpeed = 45f;

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
                var num = 0.02f;
                var distanceMultiplier = _camera.Distance * 7 / _camera.MaxDistance + 0.1f;
                var distanceModifier = Mathf.Sqrt(_camera.Distance / _camera.MaxDistance) * 2;

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

                    var h = InputManager.getAxisRaw("CameraMoveLeft", "CameraMoveRight");
                    if (Mathf.Abs(h) > 0.001f)
                    {
                        hasMovement = true;
                        _camera.AddToPosition(h * speed * num * distanceModifier, 0, 0);
                    }

                    var v = InputManager.getAxisRaw("CameraMoveDown", "CameraMoveUp");
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
                    var r = InputManager.getAxisRaw("CameraRotateRight", "CameraRotateLeft");
                    _camera.Rotation += r * rotateSpeed * num;

                }

                if (AllowZoom)
                {
                    var z = InputManager.getAxisRaw("CameraZoomIn", "CameraZoomOut");
                    _camera.Distance += z * zoomSpeed * num * distanceMultiplier;
                }

                if (AllowTilt)
                {
                    var t = InputManager.getAxisRaw("H-POPS@PerspectiveCamera/CameraTiltDown", "H-POPS@PerspectiveCamera/CameraTiltUp");
                    _camera.Tilt += t * TiltSpeed * num;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}