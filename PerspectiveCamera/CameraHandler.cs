using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        }

        void ToggleCamera()
        {
            if (!perspectiveCamera.activeSelf)
            {
                SetCameraActive(perspectiveCamera);
                GameController.Instance.pushCameraControlLock();
            }
            else
            {
                GameController.Instance.popCameraControlLock();
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