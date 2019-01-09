using System;
using UnityEngine;

namespace PerspectiveCamera
{
    public class PerspectiveCameraSettings : ScriptableSingleton<PerspectiveCameraSettings>
    {
        public bool Smoothing = true;
        public float Smoothness = 7f;
        public float MoveSpeed = 15f;
        public float RotateSpeed = 360f;
        public float ZoomSpeed = 5f;

        public void Reset()
        {
            Smoothing = true;
            Smoothness = 7f;
            MoveSpeed = 15f;
            RotateSpeed = 360f;
            ZoomSpeed = 5f;
        }

        public void Save()
        {
            PlayerPrefs.SetInt("Smooth", Convert.ToInt32(Smoothing));
            PlayerPrefs.SetFloat("Smoothness", Smoothness);
            PlayerPrefs.SetFloat("MoveSpeed", MoveSpeed);
            PlayerPrefs.SetFloat("RotateSpeed", RotateSpeed);
            PlayerPrefs.SetFloat("ZoomSpeed", ZoomSpeed);
        }

        public void Load()
        {
            if (PlayerPrefs.HasKey("Smooth") && PlayerPrefs.HasKey("MoveSpeed"))
            {
                Smoothing = Convert.ToBoolean(PlayerPrefs.GetInt("Smooth"));
                Smoothness = PlayerPrefs.GetFloat("Smoothness");
                MoveSpeed = PlayerPrefs.GetFloat("MoveSpeed");
                RotateSpeed = PlayerPrefs.GetFloat("RotateSpeed");
                ZoomSpeed = PlayerPrefs.GetFloat("ZoomSpeed");
            }
        }
    }
}