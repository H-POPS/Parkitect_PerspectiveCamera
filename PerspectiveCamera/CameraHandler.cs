using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Moona;
using Parkitect.UI;
using UnityEngine;

namespace BetterPerspective
{
    class CameraHandler : MonoBehaviour
    {
        public GameObject origCamera;
        public GameObject perspectiveCamera;


        public void SetCameraActive(GameObject cameraGO)
        {
            origCamera.SetActive(false);
            perspectiveCamera.SetActive(false);
            cameraGO.SetActive(true);
            Settings.Instance.changeGraphicsColorCorrectionFilter(Settings.Instance
                .graphicsColorCorrectionFilterIdentifier);
            Settings.Instance.changeAntiAliasing(Settings.Instance.antiAliasing);
            GameController.Instance.cameraController = cameraGO.GetComponent<CameraController>();
            CullingGroupManager.Instance.setTargetCamera(cameraGO.GetComponent<Camera>());

            var field = typeof(MFlatPanning).GetField("_cam",
                BindingFlags.Static |
                BindingFlags.NonPublic);
            // Normally the first argument to "SetValue" is the instance
            // of the type but since we are mutating a static field we pass "null"
            if (field != null) field.SetValue(null, cameraGO.GetComponent<Camera>());

            if (perspectiveCamera.activeSelf)
            {
                GameController.Instance.pushCameraControlLock();
            }
            else
            {
                GameController.Instance.popCameraControlLock();
            }
        }

        void ToggleCamera()
        {
            if (!perspectiveCamera.activeSelf)
            {
                SetCameraActive(perspectiveCamera);
            }
            else
            {
                SetCameraActive(origCamera);
            }
        }

        void Update()
        {
            if (Input.GetKeyUp(Settings.Instance.getKeyMapping("H-POPS@PerspectiveCamera/switchMode")))
            {
                ToggleCamera();
            }
        }
    }
}