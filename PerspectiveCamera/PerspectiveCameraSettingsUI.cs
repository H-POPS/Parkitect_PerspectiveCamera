using UnityEngine;

namespace PerspectiveCamera
{
    public static class PerspectiveCameraSettingsUi
    {
        public static void DrawGui()
        {
            GUILayout.BeginVertical();
            var settings = PerspectiveCameraSettings.Instance;
            var toggleStyle = new GUIStyle(GUI.skin.toggle);

            var m = 5;
            toggleStyle.margin = new RectOffset(m, m, m, m);
            GUILayout.BeginHorizontal();
            settings.Smoothing = GUILayout.Toggle(settings.Smoothing, " Smooth", toggleStyle);
            GUILayout.EndHorizontal();

            //var labelStyle = new GUIStyle() {margin = new RectOffset(5, 5,5,5), fontSize = GUI.skin.toggle.fontSize };
            var labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.margin = new RectOffset();
            labelStyle.padding = toggleStyle.margin;
            labelStyle.fontSize = GUI.skin.toggle.fontSize;
            HorizontalLine();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Smoothness:", labelStyle);
            settings.Smoothness = GUILayout.HorizontalSlider(settings.Smoothness, 10.0F, 1f);
            GUILayout.EndHorizontal(); 
            
            HorizontalLine();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Move Speed:", labelStyle);
            settings.MoveSpeed = GUILayout.HorizontalSlider(settings.MoveSpeed, 1.0F, 40.0f);
            GUILayout.EndHorizontal();

            HorizontalLine();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Rotate Speed:", labelStyle);
            settings.RotateSpeed = GUILayout.HorizontalSlider(settings.RotateSpeed, 150F, 570F);
            GUILayout.EndHorizontal();

            HorizontalLine();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Zoom Speed:", labelStyle);
            settings.ZoomSpeed = GUILayout.HorizontalSlider(settings.ZoomSpeed, 1f, 30f);
            GUILayout.EndHorizontal();

            HorizontalLine();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Target Framerate (" + settings.TargetFrameRate + "fps): ", labelStyle);
            settings.TargetFrameRate = (int)GUILayout.HorizontalSlider(settings.TargetFrameRate, -1, 60f);
            GUILayout.EndHorizontal();

            HorizontalLine();

            if (settings.TargetFrameRate <= 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Render distance: ", labelStyle);
                settings.RenderDistance = (int) GUILayout.HorizontalSlider(settings.RenderDistance, 50, 260);
                GUILayout.EndHorizontal();

                HorizontalLine();
            }

            GUILayout.BeginHorizontal();
            settings.FollowTerrain = GUILayout.Toggle(settings.FollowTerrain, " Follow terrain", toggleStyle);
            GUILayout.EndHorizontal();

            HorizontalLine();

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Default Settings", new GUIStyle(GUI.skin.button) { margin = toggleStyle.margin }))
            {
                settings.Reset();
            }
            GUILayout.EndVertical();
        }

        
        public static void HorizontalLine(Color? color = null)
        {
            // create your style
            var horizontalLine = new GUIStyle {normal = {background = Texture2D.whiteTexture}, margin = new RectOffset(0, 0, 4, 4), fixedHeight = 1};
            var c = GUI.color;
            var white = Color.white;
            white.a = .3f;
            GUI.color = color ?? white;
            GUILayout.Box(GUIContent.none, horizontalLine);
            GUI.color = c;
        }

    }
}