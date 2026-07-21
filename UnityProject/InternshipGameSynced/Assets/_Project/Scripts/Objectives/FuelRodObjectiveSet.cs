using System;
using System.Collections.Generic;
using UnityEngine;

namespace InternshipGame.Prototype
{
    public sealed class FuelRodObjectiveSet : MonoBehaviour, IRunObjective
    {
        [SerializeField] private FuelRodItem[] items = Array.Empty<FuelRodItem>();
        [SerializeField, Min(1)] private int requiredLiveRods = 3;
        [SerializeField] private MonoBehaviour progressSinkBehaviour;

        private readonly Dictionary<string, FuelRodItem> itemsById = new(StringComparer.Ordinal);
        private readonly HashSet<string> depositedIds = new(StringComparer.Ordinal);
        private IRunProgressSink progressSink;
        private bool collectionNotificationSent;
        private FuelRodItem activeCarriedItem;
        private bool hasPublishedProgress;
        private RunProgressSnapshot lastPublishedProgress;

        public event Action<RunProgressSnapshot> ProgressChanged;
        public event Action<FuelRodItem> ItemGrabbed;
        public event Action<FuelRodItem, DepositResult> DepositAttempted;
        public event Action AllRequiredDeposited;

        public string ObjectiveId => "fuel-rod-set";
        public bool IsCollected => Progress.IsComplete;
        public bool IsComplete => Progress.IsComplete;
        public int RequiredLiveRods => requiredLiveRods;
        public IReadOnlyCollection<FuelRodItem> Items => itemsById.Values;
        public RunProgressSnapshot Progress => BuildProgress();

        public void Configure(FuelRodItem[] configuredItems, int requiredDeposits = 3, MonoBehaviour sinkBehaviour = null)
        {
            if (requiredDeposits <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(requiredDeposits));
            }

            items = configuredItems ?? Array.Empty<FuelRodItem>();
            requiredLiveRods = requiredDeposits;
            progressSinkBehaviour = sinkBehaviour;
            progressSink = sinkBehaviour as IRunProgressSink;
            RebuildRegistry(throwOnDuplicate: true);
        }

        private void Awake()
        {
            progressSink = progressSinkBehaviour as IRunProgressSink;
            RebuildRegistry(throwOnDuplicate: false);
        }

        private void OnDestroy()
        {
            UnsubscribeFromItems();
        }

        public bool Contains(FuelRodItem item)
        {
            return item != null
                && itemsById.TryGetValue(item.UniqueId, out var registered)
                && ReferenceEquals(item, registered);
        }

        public DepositResult TryDeposit(FuelRodItem item, Transform slot = null)
        {
            DepositResult result;
            if (!Contains(item))
            {
                result = DepositResult.Unregistered;
            }
            else if (depositedIds.Contains(item.UniqueId) || item.IsDeposited)
            {
                result = DepositResult.Duplicate;
            }
            else if (IsCollected)
            {
                result = DepositResult.ObjectiveComplete;
            }
            else if (!item.IsIdentified)
            {
                result = DepositResult.Unassayed;
            }
            else if (item.Classification == FuelRodClassification.Inert)
            {
                result = DepositResult.Inert;
            }
            else if (!item.TryMarkDeposited(slot))
            {
                result = DepositResult.Unassayed;
            }
            else
            {
                depositedIds.Add(item.UniqueId);
                result = DepositResult.Accepted;
            }

            DepositAttempted?.Invoke(item, result);
            RaiseProgressChanged();
            if (result == DepositResult.Accepted && IsCollected)
            {
                AllRequiredDeposited?.Invoke();
            }

            return result;
        }

        public void ResetObjective()
        {
            activeCarriedItem = null;
            hasPublishedProgress = false;
            depositedIds.Clear();
            collectionNotificationSent = false;
            foreach (var item in itemsById.Values)
            {
                item.ResetObjective();
            }

            RaiseProgressChanged();
        }

        private void RebuildRegistry(bool throwOnDuplicate)
        {
            UnsubscribeFromItems();
            itemsById.Clear();
            depositedIds.Clear();
            activeCarriedItem = null;
            hasPublishedProgress = false;

            foreach (var item in items ?? Array.Empty<FuelRodItem>())
            {
                if (item == null)
                {
                    continue;
                }

                if (itemsById.ContainsKey(item.UniqueId))
                {
                    var message = $"Fuel rod ID '{item.UniqueId}' is registered more than once.";
                    if (throwOnDuplicate)
                    {
                        throw new ArgumentException(message, nameof(items));
                    }

                    Debug.LogError(message, this);
                    continue;
                }

                itemsById.Add(item.UniqueId, item);
                item.GrabStarted += OnItemGrabbed;
                item.GrabEnded += OnItemReleased;
                item.StateChanged += OnItemStateChanged;
            }
        }

        private void UnsubscribeFromItems()
        {
            foreach (var item in itemsById.Values)
            {
                if (item == null)
                {
                    continue;
                }

                item.GrabStarted -= OnItemGrabbed;
                item.GrabEnded -= OnItemReleased;
                item.StateChanged -= OnItemStateChanged;
            }
        }

        private void OnItemGrabbed(FuelRodItem item)
        {
            var previousItem = activeCarriedItem;
            activeCarriedItem = item;
            if (previousItem != null && !ReferenceEquals(previousItem, item) && previousItem.IsHeld)
            {
                previousItem.ForceReleaseForObjective();
            }

            if (!collectionNotificationSent)
            {
                collectionNotificationSent = true;
                progressSink?.NotifyObjectiveCollected(this);
            }

            ItemGrabbed?.Invoke(item);
        }

        private void OnItemReleased(FuelRodItem item)
        {
            if (ReferenceEquals(activeCarriedItem, item))
            {
                activeCarriedItem = null;
            }

        }

        private void OnItemStateChanged(FuelRodItem _)
        {
            RaiseProgressChanged();
        }

        private RunProgressSnapshot BuildProgress()
        {
            var identifiedLive = 0;
            var identifiedInert = 0;
            var carried = 0;

            foreach (var item in itemsById.Values)
            {
                if (item.IsHeld)
                {
                    carried++;
                }

                if (!item.IsIdentified)
                {
                    continue;
                }

                if (item.Classification == FuelRodClassification.Live)
                {
                    identifiedLive++;
                }
                else
                {
                    identifiedInert++;
                }
            }

            return new RunProgressSnapshot(
                itemsById.Count,
                requiredLiveRods,
                identifiedLive,
                identifiedInert,
                carried,
                depositedIds.Count);
        }

        private void RaiseProgressChanged()
        {
            var snapshot = BuildProgress();
            if (hasPublishedProgress && AreEqual(lastPublishedProgress, snapshot))
            {
                return;
            }

            hasPublishedProgress = true;
            lastPublishedProgress = snapshot;
            ProgressChanged?.Invoke(snapshot);
        }

        private static bool AreEqual(RunProgressSnapshot left, RunProgressSnapshot right)
        {
            return left.TotalRegistered == right.TotalRegistered &&
                   left.RequiredDeposits == right.RequiredDeposits &&
                   left.IdentifiedLive == right.IdentifiedLive &&
                   left.IdentifiedInert == right.IdentifiedInert &&
                   left.Carried == right.Carried &&
                   left.Deposited == right.Deposited;
        }
    }
}
