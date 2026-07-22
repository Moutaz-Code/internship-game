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
        [SerializeField, Min(0.5f)] private float transientMessageSeconds = 2.4f;
        [SerializeField, Min(0f)] private float controlsHintSeconds = 12f;

        private readonly List<IPrototypeStatusProvider> statusProviders = new();
        private FuelRodDepositRack depositRack;
        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;
        private GUIStyle missionStyle;
        private GUIStyle missionDetailStyle;
        private GUIStyle controlsStyle;
        private bool operatorVisible;
        private bool rackEventsBound;
        private string transientMessage = string.Empty;
        private Color transientMessageColor = Color.white;
        private float transientMessageEndsAt;
        private float controlsHintEndsAt;

        public void Configure(
            PrototypeRunController controller,
            TMP_Text display,
            PlayerModeController modeController,
            params MonoBehaviour[] providers)
        {
            if (Application.isPlaying)
            {
                BindRackEvents(false);
            }

            runController = controller;
            objectiveDisplay = display;
            playerMode = modeController;
            statusProviderBehaviours = providers ?? Array.Empty<MonoBehaviour>();
            ResolveStatusProviders();
            if (Application.isPlaying && isActiveAndEnabled)
            {
                BindRackEvents(true);
            }
        }

        public void ConfigureObjectiveItems(FuelRodItem[] items)
        {
            fuelRods = items ?? Array.Empty<FuelRodItem>();
        }

        private void Awake()
        {
            ResolveStatusProviders();
        }

        private void OnEnable()
        {
            if (runController != null)
            {
                runController.StateChanged += OnStateChanged;
                runController.ProgressChanged += OnProgressChanged;
            }

            BindRackEvents(true);
            controlsHintEndsAt = Time.unscaledTime + controlsHintSeconds;
        }

        private void Start()
        {
            controlsHintEndsAt = Time.unscaledTime + controlsHintSeconds;
            RefreshObjectiveDisplay();
        }

        private void OnDisable()
        {
            if (runController != null)
            {
                runController.StateChanged -= OnStateChanged;
                runController.ProgressChanged -= OnProgressChanged;
            }

            BindRackEvents(false);
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
            transientMessage = string.Empty;
            transientMessageEndsAt = 0f;
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
                    $"LIVE RODS SECURED  {progress.Deposited} / {progress.RequiredDeposits}\n" +
                    "FIND  |  HOLD 3s TO SCAN  |  RETURN LIVE",
                PrototypeRunState.ExtractionCycling =>
                    $"DOOR CYCLING\n{runController.ExtractionCycleRemaining:0.0}s - HOLD OUT",
                PrototypeRunState.EscapeReady => "EXIT OPEN\nCROSS THE CYAN GATE",
                PrototypeRunState.Extracted => "MISSION COMPLETE\nEXTRACTED\nPRESS R TO RESTART",
                PrototypeRunState.Failed => $"MISSION FAILED\n{runController.LastFailureReason}\nPRESS R TO RESTART",
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
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white },
            };
            missionDetailStyle ??= new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.82f, 0.9f, 0.94f) },
            };
            controlsStyle ??= new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.88f, 0.92f, 0.95f) },
            };

            DrawPlayerMissionPanel();
            DrawControlsHint();
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
            GUI.Label(new Rect(32f, 108f, 610f, 30f), $"PROTOTYPE | OPERATOR | v{Application.version}", titleStyle);
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
                : "WASD/Mouse | Shift sprint | Space jump | E grab | Esc pause | F11 fullscreen | F2/F3 A/B";
            GUI.Label(new Rect(32f, 98f + height - 25f, 610f, 22f), controls, bodyStyle);
        }

        private void DrawPlayerMissionPanel()
        {
            var progress = runController.Progress;
            var heldRod = FindHeldRod();
            var contextLine = heldRod == null
                ? "FOLLOW GEIGER CLICKS  |  [E] PICK UP  |  HOLD 3s TO SCAN"
                : heldRod.AssayState switch
                {
                    FuelRodAssayState.Assaying => $"SCANNING {heldRod.AssayProgress * 100f:0}%  |  KEEP HOLDING",
                    FuelRodAssayState.ConfirmedLive => "LIVE ROD  |  WARM AMBER = CONFIRMED  |  RETURN TO CYAN RACK",
                    FuelRodAssayState.ConfirmedInert => "INERT ROD  |  BLUE = NOT NEEDED  |  [E] DROP",
                    _ => "KEEP HOLDING THIS ROD FOR 3 SECONDS TO SCAN",
                };
            var contextColor = heldRod?.AssayState switch
            {
                FuelRodAssayState.ConfirmedLive => new Color(1f, 0.64f, 0.22f),
                FuelRodAssayState.ConfirmedInert => new Color(0.32f, 0.74f, 1f),
                FuelRodAssayState.Assaying => new Color(0.96f, 0.96f, 0.86f),
                _ => new Color(0.82f, 0.9f, 0.94f),
            };
            if (Time.unscaledTime < transientMessageEndsAt && !string.IsNullOrEmpty(transientMessage))
            {
                contextLine = transientMessage;
                contextColor = transientMessageColor;
            }

            var stateLine = runController.CurrentState switch
            {
                PrototypeRunState.ExtractionCycling => $"HOLD OUT WHILE THE EXIT OPENS  |  {runController.ExtractionCycleRemaining:0.0}s",
                PrototypeRunState.EscapeReady => "EXIT OPEN - CROSS THE CYAN GATE",
                PrototypeRunState.Extracted => "MISSION COMPLETE - PRESS R TO REPLAY",
                PrototypeRunState.Failed => $"MISSION FAILED - {runController.LastFailureReason}",
                _ => $"LIVE RODS SECURED  {progress.Deposited} / {progress.RequiredDeposits}",
            };

            var width = Mathf.Min(700f, Screen.width - 32f);
            var rect = new Rect((Screen.width - width) * 0.5f, 18f, width, 72f);
            GUI.Box(rect, GUIContent.none);
            missionStyle.normal.textColor = runController.CurrentState is PrototypeRunState.Failed
                ? new Color(1f, 0.36f, 0.3f)
                : new Color(0.5f, 0.95f, 1f);
            missionDetailStyle.normal.textColor = contextColor;
            GUI.Label(new Rect(rect.x + 12f, rect.y + 5f, width - 24f, 29f), stateLine, missionStyle);
            GUI.Label(new Rect(rect.x + 12f, rect.y + 36f, width - 24f, 27f), contextLine, missionDetailStyle);
        }

        private void DrawControlsHint()
        {
            if (Time.unscaledTime >= controlsHintEndsAt ||
                playerMode != null && playerMode.ActiveMode == PlayerControlMode.VR)
            {
                return;
            }

            const float width = 900f;
            var rect = new Rect(18f, Screen.height - 54f, Mathf.Min(width, Screen.width - 36f), 34f);
            GUI.Box(rect, GUIContent.none);
            GUI.Label(
                new Rect(rect.x + 10f, rect.y + 5f, rect.width - 20f, 24f),
                "WASD MOVE  |  SHIFT RUN  |  SPACE JUMP  |  E CARRY / DROP  |  LMB THROW  |  ESC PAUSE  |  F11 FULLSCREEN",
                controlsStyle);
        }

        private void ResolveStatusProviders()
        {
            statusProviders.Clear();
            depositRack = null;
            foreach (var behaviour in statusProviderBehaviours)
            {
                if (behaviour is IPrototypeStatusProvider provider)
                {
                    statusProviders.Add(provider);
                }

                depositRack ??= behaviour as FuelRodDepositRack;
            }
        }

        private void BindRackEvents(bool bind)
        {
            if (bind)
            {
                if (rackEventsBound || depositRack == null)
                {
                    return;
                }

                depositRack.DepositResolved += OnDepositResolved;
                rackEventsBound = true;
                return;
            }

            if (!rackEventsBound || depositRack == null)
            {
                rackEventsBound = false;
                return;
            }

            depositRack.DepositResolved -= OnDepositResolved;
            rackEventsBound = false;
        }

        private void OnDepositResolved(FuelRodItem _, DepositResult result)
        {
            var progress = runController != null ? runController.Progress : RunProgressSnapshot.Empty();
            (transientMessage, transientMessageColor) = result switch
            {
                DepositResult.Accepted => (
                    $"LIVE ROD SECURED  |  {progress.Deposited} / {progress.RequiredDeposits}",
                    new Color(0.3f, 1f, 0.55f)),
                DepositResult.Unassayed => (
                    "SCAN REQUIRED  |  PICK UP AND HOLD THE ROD FOR 3 SECONDS",
                    new Color(0.96f, 0.96f, 0.86f)),
                DepositResult.Inert => (
                    "INERT ROD REJECTED  |  THE NOISE MAY ATTRACT THE OWL",
                    new Color(1f, 0.32f, 0.24f)),
                DepositResult.Duplicate => ("THIS ROD IS ALREADY SECURED", new Color(0.85f, 0.9f, 0.94f)),
                DepositResult.RackFull or DepositResult.ObjectiveComplete => (
                    "ALL RACK SLOTS ARE FILLED",
                    new Color(0.5f, 0.95f, 1f)),
                DepositResult.RunInactive => ("THE EXTRACTION RACK IS OFFLINE", new Color(0.85f, 0.9f, 0.94f)),
                _ => ("THIS ITEM DOES NOT BELONG IN THE EXTRACTION RACK", new Color(1f, 0.72f, 0.3f)),
            };
            transientMessageEndsAt = Time.unscaledTime + transientMessageSeconds;
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
