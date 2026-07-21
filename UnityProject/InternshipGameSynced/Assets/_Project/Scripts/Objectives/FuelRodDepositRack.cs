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
        private float nextAttemptTime;

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
                return LastResult;
            }

            var slotIndex = FindFirstEmptySlot();
            if (slotIndex < 0 || slotIndex >= slots.Length)
            {
                LastResult = DepositResult.RackFull;
                return LastResult;
            }

            var slot = slots[slotIndex];
            LastResult = runController.TryDepositFuelRod(item, slot);
            switch (LastResult)
            {
                case DepositResult.Accepted:
                    if (slotIndex >= 0)
                    {
                        occupants[slotIndex] = item;
                    }

                    feedbackHub?.Send(new FeedbackSignal(
                        PrototypeFeedbackCues.RodDeposited,
                        FeedbackCommand.Pulse,
                        FeedbackChannel.Visual | FeedbackChannel.Audio,
                        0.72f,
                        0.7f));
                    threatStimulusSink?.ReportStimulus(new ThreatStimulus(
                        ThreatStimulusKind.ObjectiveProgress,
                        transform.position,
                        0.45f,
                        14f,
                        Mathf.Max(0f, Time.time)));
                    break;

                case DepositResult.Unassayed:
                    feedbackHub?.Send(new FeedbackSignal(
                        PrototypeFeedbackCues.RodAssayRequired,
                        FeedbackCommand.Pulse,
                        FeedbackChannel.Visual | FeedbackChannel.Audio,
                        0.55f,
                        0.85f));
                    break;

                case DepositResult.Inert:
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
                    Eject(item);
                    break;
            }

            return LastResult;
        }

        public void OnRunReset()
        {
            occupants = new FuelRodItem[slots.Length];
            lastAttemptedItem = null;
            nextAttemptTime = 0f;
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
                if (item == null || !item.IsHeld || !ContainsWorldPoint(acceptanceTrigger, item.transform.position))
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
            body.angularVelocity = UnityEngine.Random.onUnitSphere * 4f;
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
