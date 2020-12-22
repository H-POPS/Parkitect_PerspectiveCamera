using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PerspectiveCamera
{
    public class Main : AbstractMod, IModSettings
    {
        public override string getName() => "Perspective Camera";
        public override string getDescription() => "Perspective camera for your game";
        public override string getVersionNumber() => "1.0.1 (Multiplayer support)";
        public override string getIdentifier() => "H-POPS@PerspectiveCamera";
        public override bool isMultiplayerModeCompatible() => true;
        public override bool isRequiredByAllPlayersInMultiplayerMode() => false;

        private CameraHandler _cameraHandler;

        public Main()
        {
            SetupKeyBinding();
        }

        public override void onEnabled()
        {
            PerspectiveCameraSettings.Instance.Load();
            EventManager.Instance.OnStartPlayingPark += InstanceOnOnStartPlayingPark;
        }

        private void InstanceOnOnStartPlayingPark()
        {
            GameObject cameraHandlerGo = new GameObject(getIdentifier() + " | Camera Handler");
            _cameraHandler = cameraHandlerGo.AddComponent<CameraHandler>();
            

            _cameraHandler.OrigCamera = Camera.main.gameObject;
            _cameraHandler.PerspectiveCamera = Object.Instantiate(_cameraHandler.OrigCamera);
            _cameraHandler.PerspectiveCamera.name = getIdentifier();
            

            _cameraHandler.OrigCamera.SetActive(false);

            GameObject go = new GameObject();
            Camera cam2 = go.AddComponent<Camera>();

            Camera cam = _cameraHandler.PerspectiveCamera.GetComponent<Camera>();

            cam.fieldOfView = cam2.fieldOfView;

            Object.DestroyImmediate(cam.gameObject.GetComponent<CameraController>());
            Camera.main.gameObject.AddComponent<global::PerspectiveCamera.PerspectiveCamera>();
            Camera.main.gameObject.AddComponent<PerspectiveCameraKeys>();
            Camera.main.gameObject.AddComponent<PerspectiveCameraMouse>();
            Object.Destroy(go);

            _cameraHandler.SetCameraActive(_cameraHandler.PerspectiveCamera);
        }

        

        public void onDisabled()
        {
            _cameraHandler.SetCameraActive(_cameraHandler.OrigCamera);
            Object.DestroyImmediate(_cameraHandler.PerspectiveCamera);
            Object.DestroyImmediate(_cameraHandler.gameObject);
        }

        private void SetupKeyBinding()
        {
            KeyGroup group = new KeyGroup(getIdentifier());
            group.keyGroupName = getName();

            InputManager.Instance.registerKeyGroup(group);

            //Options
            RegisterKey("switchMode", KeyCode.F10, "Switch camera",
                "Use this key to quickly switch between the default game camera & the perspective camera");

            //Options
            RegisterKey("CameraTiltUp", KeyCode.R, "Tilt Camera Up");
            //Options
            RegisterKey("CameraTiltDown", KeyCode.F, "Tilt Camera Up");
        }

        private void RegisterKey(string identifier, KeyCode keyCode, string name, string description = "")
        {
            var key = new KeyMapping(getIdentifier() + "/" + identifier, keyCode, KeyCode.None);
            key.keyGroupIdentifier = getIdentifier();
            key.keyName = name;
            key.keyDescription = description;
            InputManager.Instance.registerKeyMapping(key);
        }

        public void onDrawSettingsUI()
        {
            PerspectiveCameraSettingsUi.DrawGui();
        }

        public void onSettingsOpened()
        {
        }

        public void onSettingsClosed()
        {
            PerspectiveCameraSettings.Instance.Save();
        }

    }
}