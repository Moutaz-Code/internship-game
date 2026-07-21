using UnityEngine;
using UnityEngine.InputSystem;

namespace InternshipGame.Prototype
{
    public sealed class DesktopPauseController : MonoBehaviour, IPrototypeStatusProvider
    {
        [SerializeField] private PlayerModeController playerMode;

        private float timeScaleBeforePause = 1f;
        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;

        public bool IsPaused { get; private set; }
        public string StatusLabel => IsPaused ? "Game: paused" : "Game: running";

        public void Configure(PlayerModeController mode)
        {
            playerMode = mode;
        }

        private void Update()
        {
            if (Keyboard.current?.escapeKey.wasPressedThisFrame != true)
            {
                return;
            }

            SetPaused(!IsPaused);
        }

        public void SetPaused(bool paused)
        {
            if (paused == IsPaused)
            {
                return;
            }

            IsPaused = paused;
            if (paused)
            {
                timeScaleBeforePause = Mathf.Max(0.0001f, Time.timeScale);
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Time.timeScale = timeScaleBeforePause;
                if (playerMode == null || playerMode.ActiveMode == PlayerControlMode.Desktop)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }

        private void OnDisable()
        {
            if (IsPaused)
            {
                SetPaused(false);
            }
        }

        private void OnGUI()
        {
            if (!IsPaused)
            {
                return;
            }

            titleStyle ??= new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 34,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.35f, 0.95f, 1f) },
            };
            bodyStyle ??= new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 18,
                normal = { textColor = Color.white },
            };

            var width = 460f;
            var height = 180f;
            var rect = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);
            GUI.Box(rect, GUIContent.none);
            GUI.Label(new Rect(rect.x, rect.y + 22f, width, 50f), "PAUSED", titleStyle);
            GUI.Label(new Rect(rect.x + 20f, rect.y + 84f, width - 40f, 58f), "Escape: resume\nR: restart current run  •  Shift+R: new seed", bodyStyle);
        }
    }
}
