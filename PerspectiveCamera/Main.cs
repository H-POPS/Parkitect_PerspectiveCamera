using UnityEngine;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace BetterPerspective
{
    public class Main : IMod
    {
        CameraHandler cameraHandler;

        public Main()
        {
            SetupKeyBinding();
        }

        public void onEnabled()
        {
            EventManager.Instance.OnStartPlayingPark += InstanceOnOnStartPlayingPark;
        }

        private void InstanceOnOnStartPlayingPark()
        {
            GameObject cameraHandlerGO = new GameObject("CameraHandler");
            cameraHandler = cameraHandlerGO.AddComponent<CameraHandler>();
            

            cameraHandler.origCamera = Camera.main.gameObject;
            cameraHandler.perspectiveCamera = Object.Instantiate(cameraHandler.origCamera);
            cameraHandler.perspectiveCamera.name = "Perspective Camera";


            cameraHandler.origCamera.SetActive(false);

            GameObject go = new GameObject();
            Camera cam2 = go.AddComponent<Camera>();

            Camera cam = cameraHandler.perspectiveCamera.GetComponent<Camera>();


            /**
            * Ugliest code of all time, but hey, it works!
            */
            cam.fieldOfView = cam2.fieldOfView;

            Object.DestroyImmediate(cam.gameObject.GetComponent<CameraController>());
            Camera.main.gameObject.AddComponent<BetterPerspectiveCamera>();
            Camera.main.gameObject.AddComponent<BetterPerspectiveCameraKeys>();
            Camera.main.gameObject.AddComponent<BetterPerspectiveCameraMouse>();
            Object.Destroy(go);
        }

        public void onDisabled()
        {
            cameraHandler.SetCameraActive(cameraHandler.origCamera);
            Object.DestroyImmediate(cameraHandler.perspectiveCamera);
            Object.DestroyImmediate(cameraHandler.gameObject);
        }

        private void SetupKeyBinding()
        {
            KeyGroup group = new KeyGroup(Identifier);
            group.keyGroupName = Name;

            InputManager.Instance.registerKeyGroup(group);

            //Options
            RegisterKey("setting", KeyCode.P, "Open camera Settings",
                "Use this key to open the camera settings panel (only when perspective camera is active)");
            RegisterKey("switchMode", KeyCode.F10, "Switch camera",
                "Use this key to quicly switch between the default game camera & the perspective camera");
        }

        private KeyMapping RegisterKey(string identifier, KeyCode keyCode, string Name, string Description = "")
        {
            KeyMapping key = new KeyMapping(Identifier + "/" + identifier, keyCode, KeyCode.None);
            key.keyGroupIdentifier = Identifier;
            key.keyName = Name;
            key.keyDescription = Description;
            InputManager.Instance.registerKeyMapping(key);
            return key;
        }

        public string Name => name;
        public string Description => description;
        public string Identifier => identifier;

        private static string name, description, identifier;


        static Main()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var meta =
                assembly.GetCustomAttributes(typeof(AssemblyMetadataAttribute), false)
                    .Cast<AssemblyMetadataAttribute>()
                    .Single(a => a.Key == "Identifier");
            identifier = meta.Value;

            T GetAssemblyAttribute<T>() where T : System.Attribute => (T) assembly.GetCustomAttribute(typeof(T));

            name = GetAssemblyAttribute<AssemblyTitleAttribute>().Title;
            description = GetAssemblyAttribute<AssemblyDescriptionAttribute>().Description;
            
        }
    }
}