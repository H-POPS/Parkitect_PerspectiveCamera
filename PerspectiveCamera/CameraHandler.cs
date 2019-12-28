using System.Reflection;
using Moona;
using Parkitect.UI;
using UnityEngine;

namespace PerspectiveCamera
{
    internal class CameraHandler : MonoBehaviour
    {
        public GameObject OrigCamera;
        public GameObject PerspectiveCamera;


        public void SetCameraActive(GameObject cameraGo)
        {
            var frames = UIWindowsController.Instance.getWindows();

            foreach (var frame in frames)
            {
                if (frame.windowContent.GetType() == typeof(ParkVisualizersWindow))
                {
                    frame.close();
                }
            }

            OrigCamera.SetActive(false);
            PerspectiveCamera.SetActive(false);
            cameraGo.SetActive(true);
            Settings.Instance.changeGraphicsColorCorrectionFilter(Settings.Instance
                .graphicsColorCorrectionFilterIdentifier);
            Settings.Instance.changeAntiAliasing(Settings.Instance.antiAliasing);
            GameController.Instance.cameraController = cameraGo.GetComponent<CameraController>();
            CullingGroupManager.Instance.setTargetCamera(cameraGo.GetComponent<Camera>());

            var field = typeof(MFlatPanning).GetField("_cam",
                BindingFlags.Static |
                BindingFlags.NonPublic);
            // Normally the first argument to "SetValue" is the instance
            // of the type but since we are mutating a static field we pass "null"
            if (field != null) field.SetValue(null, cameraGo.GetComponent<Camera>());

            if (PerspectiveCamera.activeSelf)
            {
                GameController.Instance.pushCameraControlLock();
            }
            else
            {
                GameController.Instance.popCameraControlLock();
            }
        }

        private void ToggleCamera()
        {
            if (!PerspectiveCamera.activeSelf)
            {
                SetCameraActive(PerspectiveCamera);
            }
            else
            {
                SetCameraActive(OrigCamera);
            }
        }

        private void Update()
        {
            if (Input.GetKeyUp(Settings.Instance.getKeyMapping("H-POPS@PerspectiveCamera/switchMode")))
            {
                ToggleCamera();
            }
        }
    }
}