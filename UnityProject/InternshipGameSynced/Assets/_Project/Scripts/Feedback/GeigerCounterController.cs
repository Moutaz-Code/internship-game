using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InternshipGame.Prototype
{
    public sealed class GeigerCounterController : MonoBehaviour, IRunStateListener, IPrototypeStatusProvider
    {
        [SerializeField] private PrototypeRunController runController;
        [SerializeField] private MonoBehaviour playerContextBehaviour;
        [SerializeField] private FuelRodItem[] rods = Array.Empty<FuelRodItem>();
        [SerializeField] private PursuitThreatController owl;
        [SerializeField] private FeedbackHub feedbackHub;
        [SerializeField] private bool interferenceEnabled;

        private IPlayerContext playerContext;
        private GeigerSignalModel model;
        private GeigerReading reading;
        private float clickAccumulator;
        private GUIStyle titleStyle;
        private GUIStyle valueStyle;
        private bool outputSuppressed;

        public GeigerReading Reading => reading;
        public bool InterferenceEnabled => interferenceEnabled;
        public bool IsOutputSuppressed => outputSuppressed;
        public string StatusLabel => outputSuppressed
            ? "Geiger: stopped (run complete)"
            : $"Geiger: {reading.ReportedClicksPerSecond:0.0} cps (truth {reading.TruthClicksPerSecond:0.0}) " +
              $"interference {(reading.InterferenceActive ? "active" : "off")}";

        public void Configure(
            PrototypeRunController controller,
            MonoBehaviour context,
            FuelRodItem[] configuredRods,
            PursuitThreatController threat,
            FeedbackHub hub,
            bool allowInterference = false)
        {
            runController = controller;
            playerContextBehaviour = context;
            rods = configuredRods ?? Array.Empty<FuelRodItem>();
            owl = threat;
            feedbackHub = hub;
            interferenceEnabled = allowInterference;
            playerContext = context as IPlayerContext;
        }

        private void Awake()
        {
            playerContext = playerContextBehaviour as IPlayerContext;
        }

        private void OnEnable()
        {
            if (runController != null)
            {
                runController.ConfigurationChanged += OnConfigurationChanged;
            }
        }

        private void Start()
        {
            CreateModel(runController?.CurrentConfiguration?.Seed ?? 1337);
        }

        private void OnDisable()
        {
            if (runController != null)
            {
                runController.ConfigurationChanged -= OnConfigurationChanged;
            }
        }

        private void Update()
        {
            if (Keyboard.current?.f2Key.wasPressedThisFrame == true)
            {
                SetInterferenceEnabled(!interferenceEnabled);
            }

            if (outputSuppressed || model == null || playerContext?.ViewTransform == null)
            {
                return;
            }

            var nearestDistance = FindNearestLiveRodDistance(playerContext.ViewTransform.position);
            var owlActive = owl != null && owl.CurrentState is not PursuitThreatState.Dormant and not PursuitThreatState.Complete;
            var owlDistance = owl != null ? owl.PlayerDistance : float.PositiveInfinity;
            reading = model.Tick(Time.deltaTime, nearestDistance, owlActive, owlDistance);

            clickAccumulator += reading.ReportedClicksPerSecond * Time.deltaTime;
            var emitted = 0;
            while (clickAccumulator >= 1f && emitted < 3)
            {
                clickAccumulator -= 1f;
                emitted++;
                feedbackHub?.Send(new FeedbackSignal(
                    PrototypeFeedbackCues.GeigerClick,
                    FeedbackCommand.Pulse,
                    FeedbackChannel.Audio,
                    Mathf.InverseLerp(
                        GeigerSignalModel.MinimumTruthClicksPerSecond,
                        GeigerSignalModel.MaximumTruthClicksPerSecond,
                        reading.ReportedClicksPerSecond),
                    0.03f));
            }
        }

        public void SetInterferenceEnabled(bool enabled)
        {
            interferenceEnabled = enabled;
            if (model != null)
            {
                model.InterferenceEnabled = enabled;
            }
        }

        public void OnRunReset()
        {
            outputSuppressed = false;
            model?.Reset();
            StopOutput();
        }

        public void OnRunStateChanged(PrototypeRunState previous, PrototypeRunState current)
        {
            if (current is PrototypeRunState.Extracted or PrototypeRunState.Failed)
            {
                outputSuppressed = true;
                model?.Reset();
                StopOutput();
                return;
            }

            outputSuppressed = false;
        }

        private void OnConfigurationChanged(RunConfiguration configuration)
        {
            CreateModel(configuration?.Seed ?? 1337);
        }

        private void CreateModel(int seed)
        {
            model = new GeigerSignalModel(seed, interferenceEnabled);
            clickAccumulator = 0f;
            reading = default;
        }

        private void StopOutput()
        {
            clickAccumulator = 0f;
            reading = default;
            feedbackHub?.Send(new FeedbackSignal(
                PrototypeFeedbackCues.GeigerClick,
                FeedbackCommand.Stop,
                FeedbackChannel.Audio));
        }

        private float FindNearestLiveRodDistance(Vector3 position)
        {
            var nearest = float.PositiveInfinity;
            foreach (var rod in rods)
            {
                if (rod == null || rod.Classification != FuelRodClassification.Live || rod.IsDeposited)
                {
                    continue;
                }

                nearest = Mathf.Min(nearest, Vector3.Distance(position, rod.transform.position));
            }

            return nearest;
        }

        private void OnGUI()
        {
            if (outputSuppressed)
            {
                return;
            }

            titleStyle ??= new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.35f, 1f, 0.48f) },
            };
            valueStyle ??= new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleRight,
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white },
            };

            var rect = new Rect(Screen.width - 246f, Screen.height - 104f, 224f, 80f);
            GUI.Box(rect, GUIContent.none);
            GUI.Label(new Rect(rect.x + 12f, rect.y + 8f, 122f, 22f), "FIELD GEIGER", titleStyle);
            GUI.Label(new Rect(rect.x + 124f, rect.y + 6f, 86f, 25f), $"{reading.ReportedClicksPerSecond:0.0}", valueStyle);
            var normalized = Mathf.InverseLerp(
                GeigerSignalModel.MinimumTruthClicksPerSecond,
                GeigerSignalModel.MaximumTruthClicksPerSecond,
                reading.ReportedClicksPerSecond);
            GUI.Box(new Rect(rect.x + 12f, rect.y + 40f, 200f, 18f), GUIContent.none);
            var oldColor = GUI.color;
            GUI.color = Color.Lerp(new Color(0.12f, 0.5f, 0.22f), new Color(0.85f, 1f, 0.2f), normalized);
            GUI.DrawTexture(new Rect(rect.x + 15f, rect.y + 43f, 194f * normalized, 12f), Texture2D.whiteTexture);
            GUI.color = oldColor;
            GUI.Label(new Rect(rect.x + 12f, rect.y + 59f, 200f, 18f), interferenceEnabled ? "F2: interference on" : "F2: baseline mode", titleStyle);
        }
    }
}
