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
        public int TargetFrameRate = 60;
        public int RenderDistance = 180;
        public bool FollowTerrain = true;

        public void Reset()
        {
            Smoothing = true;
            Smoothness = 7f;
            MoveSpeed = 15f;
            RotateSpeed = 360f;
            ZoomSpeed = 5f;
            TargetFrameRate = 60;
            RenderDistance = 180;
            FollowTerrain = true;
        }

        public void Save()
        {
            PlayerPrefs.SetInt("Smooth", Convert.ToInt32(Smoothing));
            PlayerPrefs.SetFloat("Smoothness", Smoothness);
            PlayerPrefs.SetFloat("MoveSpeed", MoveSpeed);
            PlayerPrefs.SetFloat("RotateSpeed", RotateSpeed);
            PlayerPrefs.SetFloat("ZoomSpeed", ZoomSpeed);
            PlayerPrefs.SetInt("TargetFrameRate", TargetFrameRate);
            PlayerPrefs.SetInt("RenderDistance", RenderDistance);
            PlayerPrefs.SetInt("FollowTerrain", Convert.ToInt32(FollowTerrain));
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
            if (PlayerPrefs.HasKey("FollowTerrain"))
            {
                FollowTerrain = Convert.ToBoolean(PlayerPrefs.GetInt("FollowTerrain"));
            }
            if (PlayerPrefs.HasKey("TargetFrameRate"))
            {
                TargetFrameRate = PlayerPrefs.GetInt("TargetFrameRate");
            }
            if (PlayerPrefs.HasKey("RenderDistance"))
            {
                RenderDistance = PlayerPrefs.GetInt("RenderDistance");
            }
        }
    }
}