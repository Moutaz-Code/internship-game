using UnityEngine;

namespace InternshipGame.Prototype
{
    [RequireComponent(typeof(Rigidbody), typeof(GrabbableObject))]
    public sealed class ThrowableThreatStimulus : MonoBehaviour, IRunStateListener
    {
        [SerializeField] private MonoBehaviour stimulusSinkBehaviour;
        [SerializeField] private ProceduralSpatialAudio impactAudio;
        [SerializeField, Min(0.1f)] private float minimumImpactSpeed = 1.2f;
        [SerializeField, Min(0.1f)] private float cooldownSeconds = 0.7f;

        private IThreatStimulusSink stimulusSink;
        private GrabbableObject grabbable;
        private Rigidbody body;
        private Vector3 spawnPosition;
        private Quaternion spawnRotation;
        private readonly ThrowableImpactGate impactGate = new();

        public int AcceptedImpactCount => impactGate.AcceptedImpactCount;
        public bool IsArmed => impactGate.IsArmed;

        public void Configure(MonoBehaviour sink, ProceduralSpatialAudio audio = null)
        {
            stimulusSinkBehaviour = sink;
            stimulusSink = sink as IThreatStimulusSink;
            impactAudio = audio;
        }

        private void Awake()
        {
            stimulusSink = stimulusSinkBehaviour as IThreatStimulusSink;
            grabbable = GetComponent<GrabbableObject>();
            body = GetComponent<Rigidbody>();
            spawnPosition = transform.position;
            spawnRotation = transform.rotation;
        }

        private void OnEnable()
        {
            grabbable ??= GetComponent<GrabbableObject>();
            grabbable.Grabbed += OnGrabbed;
            grabbable.Released += OnReleased;
        }

        private void OnDisable()
        {
            if (grabbable != null)
            {
                grabbable.Grabbed -= OnGrabbed;
                grabbable.Released -= OnReleased;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            var speed = collision.relativeVelocity.magnitude;
            if (stimulusSink == null || !impactGate.TryAccept(
                    Mathf.Max(0f, Time.time),
                    speed,
                    minimumImpactSpeed,
                    cooldownSeconds))
            {
                return;
            }

            var intensity = Mathf.InverseLerp(minimumImpactSpeed, 8f, speed);
            stimulusSink.ReportStimulus(new ThreatStimulus(
                ThreatStimulusKind.DroppedProp,
                transform.position,
                Mathf.Lerp(0.4f, 0.9f, intensity),
                Mathf.Lerp(10f, 22f, intensity),
                Mathf.Max(0f, Time.time)));
            impactAudio?.PlayMetalImpact(intensity);
        }

        private void OnGrabbed(GrabbableObject _)
        {
            impactGate.OnGrabbed();
        }

        private void OnReleased(GrabbableObject _)
        {
            impactGate.OnReleased();
        }

        public void OnRunReset()
        {
            grabbable?.ForceRelease();
            impactGate.Reset();
            impactAudio?.Stop();
            if (body != null)
            {
                body.position = spawnPosition;
                body.rotation = spawnRotation;
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                body.Sleep();
            }
            else
            {
                transform.SetPositionAndRotation(spawnPosition, spawnRotation);
            }
        }

        public void OnRunStateChanged(PrototypeRunState previous, PrototypeRunState current)
        {
            if (current is PrototypeRunState.Extracted or PrototypeRunState.Failed)
            {
                impactGate.OnGrabbed();
                impactAudio?.Stop();
            }
        }
    }
}
