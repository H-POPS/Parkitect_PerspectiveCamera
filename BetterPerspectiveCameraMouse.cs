using UnityEngine;


namespace BetterPerspective
{
    class BetterPerspectiveCameraMouse : MonoBehaviour
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
        public float RotateSpeedVar;
        public float RotateSpeed;

        public bool AllowTilt;
        public float TiltSpeedVar;
        public float TiltSpeed;

        public bool AllowZoom;
        public float ZoomSpeedVar;
        public float ZoomSpeed;

        public string RotateInputAxis = "Mouse X";
        public string TiltInputAxis = "Mouse Y";
        public string ZoomInputAxis = "Mouse ScrollWheel";
        public KeyCode PanKey1 = KeyCode.LeftShift;
        public KeyCode PanKey2 = KeyCode.RightShift;

        //

        private BetterPerspectiveCamera _rtsCamera;

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
            RotateSpeedVar = 360f;

            AllowTilt = true;
            TiltSpeedVar = 200f;

            AllowZoom = true;
            
            ZoomSpeedVar = 10f;

            RotateInputAxis = "Mouse X";
            TiltInputAxis = "Mouse Y";
            ZoomInputAxis = "Mouse ScrollWheel";
        }

        protected void Start()
        {
            Reset();
            _rtsCamera = gameObject.GetComponent<BetterPerspectiveCamera>();
        }

        protected void Update()
        {
            if (Time.timeScale > .2f)
            {
                MoveSpeed = 20f / Time.timeScale;
                RotateSpeed = RotateSpeedVar / Time.timeScale;
                ZoomSpeed = ZoomSpeedVar / Time.timeScale;
                TiltSpeed = TiltSpeedVar / Time.timeScale;
            }

            if (_rtsCamera == null)
                return; 

            if (AllowZoom && !UIUtility.isMouseOverUIElement())
            {

                var scroll = -Input.GetAxisRaw(ZoomInputAxis);
                _rtsCamera.Distance -= scroll * ZoomSpeed * Time.deltaTime;
            }

            if (Input.GetKey(MouseOrbitButton))
            {
                if (AllowPan && (Input.GetKey(PanKey1) || Input.GetKey(PanKey2)))
                {
                    
                    var panX = -1 * Input.GetAxisRaw("Mouse X") * PanSpeed * Time.deltaTime;
                    var panZ = -1 * Input.GetAxisRaw("Mouse Y") * PanSpeed * Time.deltaTime;

                    _rtsCamera.AddToPosition(panX, 0, panZ);

                    if (PanBreaksFollow && (Mathf.Abs(panX) > 0.001f || Mathf.Abs(panZ) > 0.001f))
                    {
                        _rtsCamera.EndFollow();
                    }
                }
                else
                {
                    

                    if (AllowTilt)
                    {
                        var tilt = Input.GetAxisRaw(TiltInputAxis);
                        _rtsCamera.Tilt -= tilt * TiltSpeed * Time.deltaTime;
                    }

                    if (AllowRotate)
                    {
                        var rot = Input.GetAxisRaw(RotateInputAxis);
                        _rtsCamera.Rotation += rot * RotateSpeed * Time.deltaTime;
                    }
                }
            }

            if (Settings.Instance.controlsEdgeScrolling && (!_rtsCamera.IsFollowing || ScreenEdgeMoveBreaksFollow))
            {
                var hasMovement = false;

                if (Input.mousePosition.y > (Screen.height - ScreenEdgeBorderWidth))
                {
                    hasMovement = true;
                    _rtsCamera.AddToPosition(0, 0, MoveSpeed * Time.deltaTime);
                }
                else if (Input.mousePosition.y < ScreenEdgeBorderWidth)
                {
                    hasMovement = true;
                    _rtsCamera.AddToPosition(0, 0, -1 * MoveSpeed * Time.deltaTime);
                }

                if (Input.mousePosition.x > (Screen.width - ScreenEdgeBorderWidth))
                {
                    hasMovement = true;
                    _rtsCamera.AddToPosition(MoveSpeed * Time.deltaTime, 0, 0);
                }
                else if (Input.mousePosition.x < ScreenEdgeBorderWidth)
                {
                    hasMovement = true;
                    _rtsCamera.AddToPosition(-1 * MoveSpeed * Time.deltaTime, 0, 0);
                }

                if (hasMovement && _rtsCamera.IsFollowing && ScreenEdgeMoveBreaksFollow)
                {
                    _rtsCamera.EndFollow();
                }
            }
        }
    }
}
