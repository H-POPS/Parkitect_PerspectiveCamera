using System;
using UnityEngine;

namespace PerspectiveCamera
{
    public static class PerspectiveCameraSettingsUI
    {
        public static void DrawGUI()
        {
            var settings = PerspectiveCameraSettings.Instance;
            settings.Smoothing = GUILayout.Toggle(settings.Smoothing, "Smooth");

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Smoothness:");
            settings.Smoothness = GUILayout.HorizontalSlider(settings.Smoothness, 10.0F, 1f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Move Speed:");
            settings.MoveSpeed = GUILayout.HorizontalSlider(settings.MoveSpeed, 1.0F, 40.0f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Rotate Speed:");
            settings.RotateSpeed = GUILayout.HorizontalSlider(settings.RotateSpeed, 150F, 570F);
            //_mouseScript.RotateSpeedVar = GUILayout.HorizontalSlider(_mouseScript.RotateSpeedVar, 150F, 570F);
            //_keysScript.RotateSpeedVar = _mouseScript.RotateSpeedVar - 100f;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Zoom Speed:");
            settings.ZoomSpeed = GUILayout.HorizontalSlider(settings.ZoomSpeed, 1f, 30f);
//            _mouseScript.ZoomSpeedVar = GUILayout.HorizontalSlider(_mouseScript.ZoomSpeedVar, 1f, 30f);
//            _keysScript.ZoomSpeedVar = _mouseScript.ZoomSpeedVar;
            GUILayout.EndHorizontal();


            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Default Settings"))
            {
                settings.Reset();
            }
            GUILayout.EndVertical();
        }


    }
}