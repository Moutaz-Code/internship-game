using UnityEngine;

namespace InternshipGame.Prototype
{
    [RequireComponent(typeof(Rigidbody), typeof(GrabbableObject))]
    public sealed class ThrowableThreatStimulus : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour stimulusSinkBehaviour;
        [SerializeField, Min(0.1f)] private float minimumImpactSpeed = 1.2f;
        [SerializeField, Min(0.1f)] private float cooldownSeconds = 0.7f;

        private IThreatStimulusSink stimulusSink;
        private GrabbableObject grabbable;
        private bool armed;
        private float nextAllowedTime;

        public void Configure(MonoBehaviour sink)
        {
            stimulusSinkBehaviour = sink;
            stimulusSink = sink as IThreatStimulusSink;
        }

        private void Awake()
        {
            stimulusSink = stimulusSinkBehaviour as IThreatStimulusSink;
            grabbable = GetComponent<GrabbableObject>();
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
            if (!armed || stimulusSink == null || Time.time < nextAllowedTime ||
                collision.relativeVelocity.magnitude < minimumImpactSpeed)
            {
                return;
            }

            var intensity = Mathf.InverseLerp(minimumImpactSpeed, 8f, collision.relativeVelocity.magnitude);
            stimulusSink.ReportStimulus(new ThreatStimulus(
                ThreatStimulusKind.DroppedProp,
                transform.position,
                Mathf.Lerp(0.4f, 0.9f, intensity),
                Mathf.Lerp(10f, 22f, intensity),
                Mathf.Max(0f, Time.time)));
            nextAllowedTime = Time.time + cooldownSeconds;
            armed = false;
        }

        private void OnGrabbed(GrabbableObject _)
        {
            armed = false;
        }

        private void OnReleased(GrabbableObject _)
        {
            armed = true;
        }
    }
}
