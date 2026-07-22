using System;
using UnityEngine;

namespace InternshipGame.Prototype
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class FuelRodDepositRack : MonoBehaviour, IRunStateListener, IPrototypeStatusProvider
    {
        [SerializeField] private PrototypeRunController runController;
        [SerializeField] private FuelRodObjectiveSet objectiveSet;
        [SerializeField] private Transform[] slots = Array.Empty<Transform>();
        [SerializeField] private FeedbackHub feedbackHub;
        [SerializeField] private MonoBehaviour threatStimulusSinkBehaviour;
        [SerializeField, Min(0.1f)] private float retryDelaySeconds = 0.75f;

        private FuelRodItem[] occupants = Array.Empty<FuelRodItem>();
        private IThreatStimulusSink threatStimulusSink;
        private BoxCollider acceptanceTrigger;
        private FuelRodItem lastAttemptedItem;
        private FuelRodItem lastAnnouncedItem;
        private DepositResult lastAnnouncedResult = DepositResult.Unregistered;
        private float nextAttemptTime;
        private float nextAnnouncementTime;

        public event Action<FuelRodItem, DepositResult> DepositResolved;

        public DepositResult LastResult { get; private set; } = DepositResult.Unregistered;
        public string StatusLabel => $"Rack: {CountOccupied()}/{slots.Length} | last {LastResult}";

        public void Configure(
            PrototypeRunController controller,
            FuelRodObjectiveSet set,
            Transform[] rackSlots,
            FeedbackHub hub,
            MonoBehaviour stimulusSink)
        {
            runController = controller;
            objectiveSet = set;
            slots = rackSlots ?? Array.Empty<Transform>();
            if (set != null && slots.Length < set.RequiredLiveRods)
            {
                throw new ArgumentException("The deposit rack requires one non-null slot per required live rod.", nameof(rackSlots));
            }
            for (var i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null)
                {
                    throw new ArgumentException($"Deposit rack slot {i} is null.", nameof(rackSlots));
                }
            }

            feedbackHub = hub;
            threatStimulusSinkBehaviour = stimulusSink;
            threatStimulusSink = stimulusSink as IThreatStimulusSink;
            occupants = new FuelRodItem[slots.Length];
            acceptanceTrigger = GetComponent<BoxCollider>();
            acceptanceTrigger.isTrigger = true;
        }

        private void Awake()
        {
            threatStimulusSink = threatStimulusSinkBehaviour as IThreatStimulusSink;
            occupants = new FuelRodItem[slots.Length];
            acceptanceTrigger = GetComponent<BoxCollider>();
        }

        private void Update()
        {
            PollHeldItemsInAcceptanceVolume();
        }

        private void OnTriggerEnter(Collider other)
        {
            TryHandleCollider(other);
        }

        private void OnTriggerStay(Collider other)
        {
            TryHandleCollider(other);
        }

        public DepositResult TryDeposit(FuelRodItem item)
        {
            if (item == null || runController == null)
            {
                LastResult = DepositResult.Unregistered;
                AnnounceResult(item, LastResult);
                return LastResult;
            }

            var slotIndex = FindFirstEmptySlot();
            if (slotIndex < 0 || slotIndex >= slots.Length)
            {
                LastResult = DepositResult.RackFull;
                AnnounceResult(item, LastResult);
                return LastResult;
            }

            var slot = slots[slotIndex];
            LastResult = runController.TryDepositFuelRod(item, slot);
            var shouldAnnounce = ShouldAnnounce(item, LastResult);
            switch (LastResult)
            {
                case DepositResult.Accepted:
                    if (slotIndex >= 0)
                    {
                        occupants[slotIndex] = item;
                    }

                    if (shouldAnnounce)
                    {
                        feedbackHub?.Send(new FeedbackSignal(
                            PrototypeFeedbackCues.RodDeposited,
                            FeedbackCommand.Pulse,
                            FeedbackChannel.Visual | FeedbackChannel.Audio,
                            0.72f,
                            0.7f));
                    }

                    threatStimulusSink?.ReportStimulus(new ThreatStimulus(
                        ThreatStimulusKind.ObjectiveProgress,
                        transform.position,
                        0.45f,
                        14f,
                        Mathf.Max(0f, Time.time)));
                    break;

                case DepositResult.Unassayed:
                    if (shouldAnnounce)
                    {
                        feedbackHub?.Send(new FeedbackSignal(
                            PrototypeFeedbackCues.RodAssayRequired,
                            FeedbackCommand.Pulse,
                            FeedbackChannel.Visual | FeedbackChannel.Audio,
                            0.55f,
                            0.85f));
                    }

                    break;

                case DepositResult.Inert:
                    if (shouldAnnounce)
                    {
                        feedbackHub?.Send(new FeedbackSignal(
                            PrototypeFeedbackCues.RodRejected,
                            FeedbackCommand.Pulse,
                            FeedbackChannel.Visual | FeedbackChannel.Audio,
                            1f,
                            1.15f));
                        threatStimulusSink?.ReportStimulus(new ThreatStimulus(
                            ThreatStimulusKind.RodRejected,
                            transform.position,
                            1f,
                            24f,
                            Mathf.Max(0f, Time.time)));
                    }

                    Eject(item);
                    break;
            }

            if (shouldAnnounce)
            {
                AnnounceResult(item, LastResult);
            }

            return LastResult;
        }

        public void OnRunReset()
        {
            occupants = new FuelRodItem[slots.Length];
            lastAttemptedItem = null;
            lastAnnouncedItem = null;
            lastAnnouncedResult = DepositResult.Unregistered;
            nextAttemptTime = 0f;
            nextAnnouncementTime = 0f;
            LastResult = DepositResult.Unregistered;
        }

        public void OnRunStateChanged(PrototypeRunState previous, PrototypeRunState current)
        {
        }

        private void TryHandleCollider(Collider other)
        {
            var item = other != null ? other.GetComponentInParent<FuelRodItem>() : null;
            TryHandleItem(item);
        }

        internal void PollHeldItemsInAcceptanceVolume()
        {
            if (objectiveSet == null || acceptanceTrigger == null)
            {
                return;
            }

            foreach (var item in objectiveSet.Items)
            {
                if (item == null || !item.IsHeld || !OverlapsAcceptanceVolume(acceptanceTrigger, item))
                {
                    continue;
                }

                TryHandleItem(item);
                return;
            }
        }

        internal static bool ContainsWorldPoint(BoxCollider box, Vector3 worldPoint)
        {
            if (box == null || !box.enabled || !box.gameObject.activeInHierarchy)
            {
                return false;
            }

            var localPoint = box.transform.InverseTransformPoint(worldPoint) - box.center;
            var halfSize = box.size * 0.5f;
            return Mathf.Abs(localPoint.x) <= halfSize.x &&
                   Mathf.Abs(localPoint.y) <= halfSize.y &&
                   Mathf.Abs(localPoint.z) <= halfSize.z;
        }

        internal static bool OverlapsAcceptanceVolume(BoxCollider volume, FuelRodItem item)
        {
            if (volume == null || item == null || !volume.enabled || !volume.gameObject.activeInHierarchy)
            {
                return false;
            }

            var hasUsableCollider = false;
            foreach (var candidate in item.GetComponentsInChildren<Collider>(true))
            {
                if (candidate == null || candidate == volume || !candidate.enabled ||
                    !candidate.gameObject.activeInHierarchy || candidate.isTrigger)
                {
                    continue;
                }

                hasUsableCollider = true;
                if (!volume.bounds.Intersects(candidate.bounds))
                {
                    continue;
                }

                if (Physics.ComputePenetration(
                    volume,
                    volume.transform.position,
                    volume.transform.rotation,
                    candidate,
                    candidate.transform.position,
                    candidate.transform.rotation,
                    out _,
                    out _))
                {
                    return true;
                }
            }

            return !hasUsableCollider && ContainsWorldPoint(volume, item.transform.position);
        }

        private void TryHandleItem(FuelRodItem item)
        {
            if (item == null || (ReferenceEquals(item, lastAttemptedItem) && Time.time < nextAttemptTime))
            {
                return;
            }

            lastAttemptedItem = item;
            nextAttemptTime = Time.time + retryDelaySeconds;
            TryDeposit(item);
        }

        private void Eject(FuelRodItem item)
        {
            item.ForceReleaseForObjective();
            var body = item.GetComponent<Rigidbody>();
            if (body == null || body.isKinematic)
            {
                return;
            }

            var direction = item.transform.position - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.01f)
            {
                direction = -transform.forward;
            }

            body.linearVelocity = direction.normalized * 3.2f + Vector3.up * 1.6f;
            body.angularVelocity = Vector3.Cross(direction.normalized, Vector3.up) * 4f +
                Vector3.up * 0.8f;
        }

        private bool ShouldAnnounce(FuelRodItem item, DepositResult result)
        {
            return !ReferenceEquals(item, lastAnnouncedItem) ||
                   result != lastAnnouncedResult ||
                   Time.time >= nextAnnouncementTime;
        }

        private void AnnounceResult(FuelRodItem item, DepositResult result)
        {
            lastAnnouncedItem = item;
            lastAnnouncedResult = result;
            nextAnnouncementTime = Time.time + 2.5f;
            DepositResolved?.Invoke(item, result);
        }

        private int FindFirstEmptySlot()
        {
            for (var i = 0; i < occupants.Length; i++)
            {
                if (occupants[i] == null)
                {
                    return i;
                }
            }

            return -1;
        }

        private int CountOccupied()
        {
            var count = 0;
            foreach (var occupant in occupants)
            {
                if (occupant != null)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
