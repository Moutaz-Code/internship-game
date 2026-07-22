using UnityEngine;
using UnityEngine.InputSystem;

namespace InternshipGame.Prototype
{
    public sealed class DesktopDisplayController : MonoBehaviour, IPrototypeStatusProvider
    {
        public const int DefaultWindowedWidth = 1280;
        public const int DefaultWindowedHeight = 720;

        [SerializeField, Min(640)] private int windowedWidth = DefaultWindowedWidth;
        [SerializeField, Min(360)] private int windowedHeight = DefaultWindowedHeight;

        public bool IsFullscreen => IsFullscreenMode(Screen.fullScreenMode);
        public string StatusLabel =>
            $"Display: {(IsFullscreen ? "fullscreen" : "windowed")} {Screen.width}x{Screen.height} | F11 toggle";

        private void Awake()
        {
            CaptureWindowedResolution();
        }

        private void Update()
        {
            if (Keyboard.current?.f11Key.wasPressedThisFrame == true)
            {
                ToggleFullscreen();
            }
        }

        public void ToggleFullscreen()
        {
            var nextMode = GetToggledMode(Screen.fullScreenMode);
            if (nextMode == FullScreenMode.Windowed)
            {
                Screen.SetResolution(
                    Mathf.Max(640, windowedWidth),
                    Mathf.Max(360, windowedHeight),
                    nextMode);
                return;
            }

            CaptureWindowedResolution();
            Screen.fullScreenMode = nextMode;
        }

        public static bool IsFullscreenMode(FullScreenMode mode)
        {
            return mode != FullScreenMode.Windowed;
        }

        public static FullScreenMode GetToggledMode(FullScreenMode currentMode)
        {
            return IsFullscreenMode(currentMode)
                ? FullScreenMode.Windowed
                : FullScreenMode.FullScreenWindow;
        }

        private void CaptureWindowedResolution()
        {
            if (Screen.fullScreenMode != FullScreenMode.Windowed)
            {
                return;
            }

            windowedWidth = Mathf.Max(640, Screen.width);
            windowedHeight = Mathf.Max(360, Screen.height);
        }
    }
}
