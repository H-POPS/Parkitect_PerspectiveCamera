using System.Runtime.InteropServices;
using UnityEngine;

namespace PerspectiveCamera
{
    /// <summary>
    /// Keeps the cursor where it is while the orbit button is held. Unity's CursorLockMode.Locked would move it to the
    /// center of the screen, and the game's builders follow the cursor, so whatever is being placed would go along.
    /// </summary>
    internal static class CursorAnchor
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct Point
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point point);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        private static readonly bool IsSupported = Application.platform == RuntimePlatform.WindowsPlayer;

        private static bool _isHolding;
        private static Point _position;

        public static void Update(bool holding)
        {
            if (!holding)
            {
                _isHolding = false;
                Cursor.visible = true;
                // What the game itself applies for its "lock cursor" option.
                Cursor.lockState = Settings.Instance.controlsLockCursor ? CursorLockMode.Confined : CursorLockMode.None;
                return;
            }

            Cursor.visible = false;
            if (!IsSupported)
            {
                Cursor.lockState = CursorLockMode.Locked;
                return;
            }

            if (!_isHolding)
            {
                _isHolding = GetCursorPos(out _position);
            }
            else if (Application.isFocused)
            {
                SetCursorPos(_position.X, _position.Y);
            }
        }
    }
}
