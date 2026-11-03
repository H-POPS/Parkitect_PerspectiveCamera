using Parkitect.UI;
using UnityEngine;

namespace PerspectiveCamera
{
    /// <summary>
    /// Keeps the game's floating labels off the screen when what they point at is behind the camera. WorldToScreenPoint
    /// mirrors such a point, so a shop's warning behind the camera would float in the sky in front of it. The game's own
    /// camera doesn't tilt enough to show this, unlike this mod's.
    /// </summary>
    internal class BehindCameraLabels : MonoBehaviour
    {
        private const float OffScreenX = -100000f;

        private void OnEnable()
        {
            Canvas.willRenderCanvases += HideLabelsBehindCamera;
        }

        private void OnDisable()
        {
            Canvas.willRenderCanvases -= HideLabelsBehindCamera;
        }

        // Runs after every label has placed itself for this frame, and each one places itself again in the next, so
        // there is nothing to restore. Their depth keeps the sign of WorldToScreenPoint's, negative behind the camera.
        // Moving them is used because several animate their own CanvasGroup, and it also stops them taking clicks.
        private static void HideLabelsBehindCamera()
        {
            UIWorldOverlayController overlay = UIWorldOverlayController.Instance;
            if (overlay == null)
            {
                return;
            }

            Transform labels = overlay.transform;
            for (int i = 0; i < labels.childCount; i++)
            {
                Transform label = labels.GetChild(i);
                Vector3 position = label.position;
                if (position.z < 0f)
                {
                    position.x = OffScreenX;
                    label.position = position;
                }
            }
        }
    }
}
