using UnityEngine;

namespace InternshipGame.Prototype
{
    public sealed class WatcherEncounterController : MonoBehaviour, IRunStateListener, IPrototypeStatusProvider
    {
        [SerializeField] private MonoBehaviour playerContextBehaviour;
        [SerializeField] private FeedbackHub feedbackHub;
        [SerializeField] private GameObject watcherRoot;
        [SerializeField, Min(0f)] private float revealDelaySeconds = 1.5f;
        [SerializeField, Min(0.1f)] private float visibleDurationSeconds = 14f;
        [SerializeField, Min(0f)] private float dismissDistance = 2.2f;

        private WatcherEncounterSequence sequence;
        private IPlayerContext playerContext;
        private bool encounterActive;

        public WatcherEncounterState CurrentState => sequence?.State ?? WatcherEncounterState.Dormant;
        public bool IsVisible => watcherRoot != null && watcherRoot.activeSelf;
        public string StatusLabel => $"Watcher: {CurrentState}";

        public void Configure(MonoBehaviour contextBehaviour, FeedbackHub hub, GameObject visualRoot)
        {
            playerContextBehaviour = contextBehaviour;
            playerContext = contextBehaviour as IPlayerContext;
            feedbackHub = hub;
            watcherRoot = visualRoot;
        }

        private void Awake()
        {
            playerContext = playerContextBehaviour as IPlayerContext;
            sequence = new WatcherEncounterSequence(
                revealDelaySeconds,
                visibleDurationSeconds,
                dismissDistance);
            SetVisible(false);
        }

        private void Update()
        {
            if (!encounterActive || sequence == null)
            {
                return;
            }

            var previousState = sequence.State;
            var viewer = playerContext?.ViewTransform;
            var viewerDistance = viewer != null && watcherRoot != null
                ? Vector3.Distance(viewer.position, watcherRoot.transform.position)
                : float.PositiveInfinity;
            sequence.Tick(Time.deltaTime, viewerDistance);

            if (sequence.State != previousState)
            {
                if (sequence.State == WatcherEncounterState.Visible)
                {
                    Reveal();
                }
                else if (sequence.State == WatcherEncounterState.Complete)
                {
                    SetVisible(false);
                }
            }

            if (sequence.State == WatcherEncounterState.Visible)
            {
                FaceViewer();
            }
        }

        public void OnRunReset()
        {
            ResetEncounter();
        }

        public void OnRunStateChanged(PrototypeRunState previous, PrototypeRunState current)
        {
            if (current == PrototypeRunState.ExtractionCycling)
            {
                encounterActive = true;
                sequence?.Arm();
            }
            else if (current == PrototypeRunState.RetrieveFuelRods)
            {
                ResetEncounter();
            }
            else if (current is PrototypeRunState.EscapeReady
                or PrototypeRunState.Extracted
                or PrototypeRunState.Failed)
            {
                encounterActive = false;
                sequence?.Dismiss();
                SetVisible(false);
            }
        }

        public void ResetEncounter()
        {
            encounterActive = false;
            sequence?.Reset();
            SetVisible(false);
            feedbackHub?.Send(new FeedbackSignal(
                PrototypeFeedbackCues.WatcherReveal,
                FeedbackCommand.Stop,
                FeedbackChannel.Audio | FeedbackChannel.Visual));
        }

        private void Reveal()
        {
            SetVisible(true);
            FaceViewer();
            feedbackHub?.Send(new FeedbackSignal(
                PrototypeFeedbackCues.WatcherReveal,
                FeedbackCommand.Pulse,
                FeedbackChannel.Audio | FeedbackChannel.Visual,
                0.55f,
                0.7f));
        }

        private void FaceViewer()
        {
            var viewer = playerContext?.ViewTransform;
            if (watcherRoot == null || viewer == null)
            {
                return;
            }

            var direction = viewer.position - watcherRoot.transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.001f)
            {
                watcherRoot.transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            }
        }

        private void SetVisible(bool visible)
        {
            if (watcherRoot != null && watcherRoot.activeSelf != visible)
            {
                watcherRoot.SetActive(visible);
            }
        }
    }
}
