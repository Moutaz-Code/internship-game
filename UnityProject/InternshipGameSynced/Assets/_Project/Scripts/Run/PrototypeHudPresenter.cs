using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InternshipGame.Prototype
{
    public sealed class PrototypeHudPresenter : MonoBehaviour
    {
        [SerializeField] private PrototypeRunController runController;
        [SerializeField] private TMP_Text objectiveDisplay;
        [SerializeField] private PlayerModeController playerMode;
        [SerializeField] private FuelRodItem[] fuelRods = Array.Empty<FuelRodItem>();
        [SerializeField] private MonoBehaviour[] statusProviderBehaviours = Array.Empty<MonoBehaviour>();

        private readonly List<IPrototypeStatusProvider> statusProviders = new();
        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;
        private GUIStyle missionStyle;
        private bool operatorVisible;

        public void Configure(
            PrototypeRunController controller,
            TMP_Text display,
            PlayerModeController modeController,
            params MonoBehaviour[] providers)
        {
            runController = controller;
            objectiveDisplay = display;
            playerMode = modeController;
            statusProviderBehaviours = providers ?? Array.Empty<MonoBehaviour>();
        }

        public void ConfigureObjectiveItems(FuelRodItem[] items)
        {
            fuelRods = items ?? Array.Empty<FuelRodItem>();
        }

        private void Awake()
        {
            statusProviders.Clear();
            foreach (var behaviour in statusProviderBehaviours)
            {
                if (behaviour is IPrototypeStatusProvider provider)
                {
                    statusProviders.Add(provider);
                }
            }
        }

        private void OnEnable()
        {
            if (runController != null)
            {
                runController.StateChanged += OnStateChanged;
                runController.ProgressChanged += OnProgressChanged;
            }
        }

        private void Start()
        {
            RefreshObjectiveDisplay();
        }

        private void OnDisable()
        {
            if (runController != null)
            {
                runController.StateChanged -= OnStateChanged;
                runController.ProgressChanged -= OnProgressChanged;
            }
        }

        private void Update()
        {
            if (Keyboard.current?.f1Key.wasPressedThisFrame == true)
            {
                operatorVisible = !operatorVisible;
            }

            if (runController != null && runController.CurrentState == PrototypeRunState.ExtractionCycling)
            {
                RefreshObjectiveDisplay();
            }
        }

        private void OnStateChanged(PrototypeRunState _, PrototypeRunState __)
        {
            RefreshObjectiveDisplay();
        }

        private void OnProgressChanged(RunProgressSnapshot _)
        {
            RefreshObjectiveDisplay();
        }

        private void RefreshObjectiveDisplay()
        {
            if (objectiveDisplay == null || runController == null)
            {
                return;
            }

            var progress = runController.Progress;
            objectiveDisplay.text = runController.CurrentState switch
            {
#pragma warning disable CS0618
                PrototypeRunState.FindFuelRod => "MISSION\nRetrieve the live fuel rod\nand return it here.",
                PrototypeRunState.ReturnToExtraction => "FUEL ROD SECURED\nReturn to extraction.",
#pragma warning restore CS0618
                PrototypeRunState.RetrieveFuelRods =>
                    $"LIVE RODS {progress.Deposited}/{progress.RequiredDeposits}\n" +
                    "SEARCH • HOLD 3s TO ASSAY • RETURN LIVE RODS",
                PrototypeRunState.ExtractionCycling =>
                    $"EXTRACTION CYCLING\n{runController.ExtractionCycleRemaining:0.0}s — SURVIVE",
                PrototypeRunState.EscapeReady => "EXIT OPEN\nCROSS THE CYAN GATE",
                PrototypeRunState.Extracted => "EXTRACTED\nRun complete.\nPress R to restart.",
                PrototypeRunState.Failed => $"LOCKDOWN\n{runController.LastFailureReason}\nPress R to restart.",
                _ => "STANDBY",
            };
        }

        private void OnGUI()
        {
            if (runController == null)
            {
                return;
            }

            titleStyle ??= new GUIStyle(GUI.skin.label)
            {
                fontSize = 22,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.35f, 0.95f, 1f) },
            };
            bodyStyle ??= new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                normal = { textColor = Color.white },
            };
            missionStyle ??= new GUIStyle(GUI.skin.label)
            {
                fontSize = 17,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white },
            };

            DrawPlayerMissionPanel();
            if (!operatorVisible)
            {
                return;
            }

            var statusLines = 0;
            foreach (var provider in statusProviders)
            {
                statusLines += SplitStatusLines(provider.StatusLabel).Length;
            }

            statusLines = Mathf.Max(1, statusLines);
            var height = 136f + statusLines * 21f;
            GUI.Box(new Rect(16f, 98f, 650f, height), GUIContent.none);
            GUI.Label(new Rect(32f, 108f, 610f, 30f), "PROTOTYPE — OPERATOR", titleStyle);
            GUI.Label(new Rect(32f, 140f, 610f, 22f), $"State: {runController.CurrentState}", bodyStyle);
            GUI.Label(new Rect(32f, 162f, 610f, 22f), $"Time remaining: {runController.TimeRemaining:0}s", bodyStyle);

            var y = 184f;
            foreach (var provider in statusProviders)
            {
                foreach (var line in SplitStatusLines(provider.StatusLabel))
                {
                    GUI.Label(new Rect(32f, y, 610f, 22f), line, bodyStyle);
                    y += 21f;
                }
            }

            var controls = playerMode != null && playerMode.ActiveMode == PlayerControlMode.VR
                ? "VR locomotion/grab | R restart | F1 overlay | F2 Geiger A/B | F3 adaptation A/B"
                : "WASD/Mouse | Shift sprint | Space jump | E grab | Esc pause | F2 Geiger A/B | F3 adaptation A/B";
            GUI.Label(new Rect(32f, 98f + height - 25f, 610f, 22f), controls, bodyStyle);
        }

        private void DrawPlayerMissionPanel()
        {
            var progress = runController.Progress;
            var heldRod = FindHeldRod();
            var assayLine = heldRod == null
                ? "GEIGER LOCATES • HEAT CONFIRMS LIVE"
                : heldRod.AssayState switch
                {
                    FuelRodAssayState.Assaying => $"ASSAYING {heldRod.AssayProgress * 100f:0}% — KEEP HOLDING",
                    FuelRodAssayState.ConfirmedLive => "LIVE CONFIRMED — WARMTH / AMBER",
                    FuelRodAssayState.ConfirmedInert => "INERT CONFIRMED — COOL / BLUE",
                    _ => "HOLD ROD FOR 3 SECONDS TO ASSAY",
                };
            var stateLine = runController.CurrentState switch
            {
                PrototypeRunState.ExtractionCycling => $"SURVIVE EXTRACTION CYCLE  {runController.ExtractionCycleRemaining:0.0}s",
                PrototypeRunState.EscapeReady => "EXIT OPEN — CROSS THE CYAN GATE",
                PrototypeRunState.Extracted => "EXTRACTED — PRESS R TO REPLAY",
                PrototypeRunState.Failed => $"MISSION FAILED — {runController.LastFailureReason}",
                _ => $"LIVE RODS DEPOSITED  {progress.Deposited}/{progress.RequiredDeposits}",
            };

            const float width = 620f;
            var rect = new Rect((Screen.width - width) * 0.5f, 18f, width, 66f);
            GUI.Box(rect, GUIContent.none);
            GUI.Label(new Rect(rect.x + 12f, rect.y + 4f, width - 24f, 29f), stateLine, missionStyle);
            GUI.Label(new Rect(rect.x + 12f, rect.y + 32f, width - 24f, 25f), assayLine, missionStyle);
            GUI.Label(new Rect(Screen.width - 168f, 12f, 150f, 22f), "F1: operator info", bodyStyle);
        }

        private FuelRodItem FindHeldRod()
        {
            foreach (var rod in fuelRods)
            {
                if (rod != null && rod.IsHeld)
                {
                    return rod;
                }
            }

            return null;
        }

        private static string[] SplitStatusLines(string status)
        {
            return (status ?? string.Empty).Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        }
    }
}
