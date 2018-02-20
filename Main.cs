using UnityEngine;
using System.Collections;

namespace BetterPerspective
{
    public class Main : IMod
    {
        public void onEnabled()
        {

            GameObject go = new GameObject();
            Camera cam2 = go.AddComponent<Camera>();

            Camera cam = Camera.main;
            
            /**
            * Ugliest code of all time, but hey, it works!
            */
            cam.aspect = cam2.aspect;
            cam.backgroundColor = cam2.backgroundColor;
            cam.clearFlags = cam2.clearFlags;
            cam.clearStencilAfterLightingPass = cam2.clearStencilAfterLightingPass;
            //cam.cullingMask = cam2.cullingMask;
            cam.depth = cam2.depth;
            cam.depthTextureMode = cam2.depthTextureMode;
            //cam.farClipPlane = cam2.farClipPlane;
            cam.fieldOfView = cam2.fieldOfView;
            cam.allowHDR = cam2.allowHDR;
            cam.layerCullDistances = cam2.layerCullDistances;
            cam.layerCullSpherical = cam2.layerCullSpherical;
            cam.pixelRect = cam2.pixelRect;
            cam.projectionMatrix = cam2.projectionMatrix;
            cam.rect = cam2.rect;
            cam.renderingPath = cam2.renderingPath;
            cam.stereoConvergence = cam2.stereoConvergence;
            cam.stereoMirrorMode = cam2.stereoMirrorMode;
            cam.stereoSeparation = cam2.stereoSeparation;
            cam.targetDisplay = cam2.targetDisplay;
            cam.targetTexture = cam2.targetTexture;
            cam.transparencySortMode = cam2.transparencySortMode;
            cam.useOcclusionCulling = cam2.useOcclusionCulling;
            


            Object.DestroyImmediate(cam.gameObject.GetComponent<CameraController>());
            Camera.main.gameObject.AddComponent<BetterPerspectiveCamera>();
            Camera.main.gameObject.AddComponent<BetterPerspectiveCameraKeys>();
            Camera.main.gameObject.AddComponent<BetterPerspectiveCameraMouse>();
            Object.Destroy(go);

            Settings.Instance.changeGraphicsBloom(false);
        }

        public void onDisabled()
        {
            Object.DestroyImmediate(Camera.main.gameObject.GetComponent<BetterPerspectiveCamera>());
            Object.DestroyImmediate(Camera.main.gameObject.GetComponent<BetterPerspectiveCameraKeys>());
            Object.DestroyImmediate(Camera.main.gameObject.GetComponent<BetterPerspectiveCameraMouse>());
            Camera.main.gameObject.AddComponent<CameraController>();
        }

        public string Name { get { return "Perspective camera"; } }
        public string Description { get { return "Perspective camera for your game"; } }
        public string Identifier { get { return "H-POPS@PerspectiveCamera"; }}

    }
    
}
