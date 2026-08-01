using UnityEngine;

namespace InternshipGame.Prototype
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class MachineryDistractionPanel : MonoBehaviour, IPlayerInteractable, IRunStateListener, IPrototypeStatusProvider
    {
        public const string Prompt = "TEST MACHINERY";
        public const float DefaultIntensity = 0.75f;
        public const float DefaultRadius = 18f;
        public const float DefaultActiveSeconds = 2.5f;
        public const float DefaultCooldownSeconds = 25f;

        [SerializeField] private MonoBehaviour stimulusSinkBehaviour;
        [SerializeField] private Renderer indicatorRenderer;
        [SerializeField] private Material readyMaterial;
        [SerializeField] private Material activeMaterial;
        [SerializeField] private ProceduralSpatialAudio machineryAudio;
        [SerializeField, Range(0f, 1f)] private float intensity = DefaultIntensity;
        [SerializeField, Min(0f)] private float radius = DefaultRadius;
        [SerializeField, Min(0.1f)] private float activeSeconds = DefaultActiveSeconds;
        [SerializeField, Min(0.1f)] private float cooldownSeconds = DefaultCooldownSeconds;

        private IThreatStimulusSink stimulusSink;
        private MachineryPanelState state;
        private bool interactionEnabled = true;
        private bool displayedActive;

        public string InteractionPrompt => Prompt;
        public float Intensity => intensity;
        public float Radius => radius;
        public float ActiveSeconds => activeSeconds;
        public float CooldownSeconds => cooldownSeconds;
        public int ActivationCount => State.ActivationCount;
        public bool IsActive => State.IsActive(Time.time);
        public bool IsReady => interactionEnabled && State.IsReady(Time.time);
        public string StatusLabel => IsReady
            ? $"{name}: READY"
            : IsActive
                ? $"{name}: ACTIVE {Mathf.Max(0f, State.ActiveUntil - Time.time):0.0}s"
                : $"{name}: cooldown {State.CooldownRemaining(Time.time):0.0}s";

        private MachineryPanelState State => state ??= new MachineryPanelState(
            Mathf.Max(0.1f, activeSeconds),
            Mathf.Max(Mathf.Max(0.1f, activeSeconds), cooldownSeconds));

        public void Configure(
            MonoBehaviour sink,
            Renderer indicator,
            Material ready,
            Material active,
            ProceduralSpatialAudio audio)
        {
            stimulusSinkBehaviour = sink;
            stimulusSink = sink as IThreatStimulusSink;
            indicatorRenderer = indicator;
            readyMaterial = ready;
            activeMaterial = active;
            machineryAudio = audio;
            ApplyIndicator(false);
        }

        private void Awake()
        {
            stimulusSink = stimulusSinkBehaviour as IThreatStimulusSink;
            _ = State;
            ApplyIndicator(false);
        }

        private void Update()
        {
            var active = State.IsActive(Time.time);
            if (active != displayedActive)
            {
                ApplyIndicator(active);
            }
        }

        public bool CanInteract(in PlayerInteractionContext context)
        {
            return IsReady;
        }

        public void Interact(in PlayerInteractionContext context)
        {
            if (stimulusSink == null || !interactionEnabled || !State.TryActivate(Time.time))
            {
                return;
            }

            stimulusSink.ReportStimulus(new ThreatStimulus(
                ThreatStimulusKind.Machinery,
                transform.position,
                intensity,
                radius,
                Mathf.Max(0f, Time.time)));
            machineryAudio?.PlayMachinery();
            ApplyIndicator(true);
        }

        public void OnRunReset()
        {
            interactionEnabled = true;
            State.Reset();
            machineryAudio?.Stop();
            ApplyIndicator(false);
        }

        public void OnRunStateChanged(PrototypeRunState previous, PrototypeRunState current)
        {
            interactionEnabled = current is not PrototypeRunState.Extracted and not PrototypeRunState.Failed;
            if (!interactionEnabled)
            {
                machineryAudio?.Stop();
                ApplyIndicator(false);
            }
        }

        private void ApplyIndicator(bool active)
        {
            displayedActive = active;
            if (indicatorRenderer != null)
            {
                indicatorRenderer.sharedMaterial = active ? activeMaterial : readyMaterial;
            }
        }
    }
}
