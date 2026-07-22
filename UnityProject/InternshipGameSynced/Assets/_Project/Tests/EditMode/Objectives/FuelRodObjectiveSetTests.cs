using NUnit.Framework;
using UnityEngine;

namespace InternshipGame.Prototype.Tests
{
    public sealed class FuelRodObjectiveSetTests
    {
        private GameObject root;

        [SetUp]
        public void SetUp()
        {
            root = new GameObject("Fuel Rod Objective Test");
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(root);
        }

        [Test]
        public void DepositRejectsUnregisteredUnassayedAndInertRods()
        {
            var live = CreateRod("live", FuelRodClassification.Live);
            var inert = CreateRod("inert", FuelRodClassification.Inert);
            var outsider = CreateRod("outsider", FuelRodClassification.Live);
            var set = CreateSet(live, inert);

            Assert.That(set.TryDeposit(outsider), Is.EqualTo(DepositResult.Unregistered));
            Assert.That(set.TryDeposit(live), Is.EqualTo(DepositResult.Unassayed));

            inert.AdvanceAssay(3f, true);
            Assert.That(set.TryDeposit(inert), Is.EqualTo(DepositResult.Inert));
        }

        [Test]
        public void ThreeUniqueConfirmedLiveRodsCompleteAggregate()
        {
            var first = CreateRod("live-1", FuelRodClassification.Live);
            var second = CreateRod("live-2", FuelRodClassification.Live);
            var third = CreateRod("live-3", FuelRodClassification.Live);
            var set = CreateSet(first, second, third);
            var completionSignals = 0;
            set.AllRequiredDeposited += () => completionSignals++;

            first.AdvanceAssay(3f, true);
            second.AdvanceAssay(3f, true);
            third.AdvanceAssay(3f, true);

            Assert.That(set.TryDeposit(first), Is.EqualTo(DepositResult.Accepted));
            Assert.That(set.TryDeposit(second), Is.EqualTo(DepositResult.Accepted));
            Assert.That(set.TryDeposit(third), Is.EqualTo(DepositResult.Accepted));
            Assert.That(set.Progress.Deposited, Is.EqualTo(3));
            Assert.That(set.IsCollected, Is.True);
            Assert.That(completionSignals, Is.EqualTo(1));
        }

        [Test]
        public void DuplicateRodCannotCountTwice()
        {
            var rod = CreateRod("live", FuelRodClassification.Live);
            var set = CreateSet(rod);
            rod.AdvanceAssay(3f, true);

            Assert.That(set.TryDeposit(rod), Is.EqualTo(DepositResult.Accepted));
            Assert.That(set.TryDeposit(rod), Is.EqualTo(DepositResult.Duplicate));
            Assert.That(set.Progress.Deposited, Is.EqualTo(1));
        }

        [Test]
        public void ResetRestoresAllItemAndAggregateState()
        {
            var rod = CreateRod("live", FuelRodClassification.Live);
            var set = CreateSet(rod);
            rod.AdvanceAssay(3f, true);
            set.TryDeposit(rod);

            set.ResetObjective();

            Assert.That(set.Progress.Deposited, Is.Zero);
            Assert.That(set.IsCollected, Is.False);
            Assert.That(rod.IsDeposited, Is.False);
            Assert.That(rod.IsIdentified, Is.False);
            Assert.That(rod.AssayState, Is.EqualTo(FuelRodAssayState.Unassayed));
        }

        [Test]
        public void DuplicateRegistrationIsRejected()
        {
            var first = CreateRod("same-id", FuelRodClassification.Live);
            var second = CreateRod("same-id", FuelRodClassification.Inert);
            var setObject = new GameObject("Set");
            setObject.transform.SetParent(root.transform);
            var set = setObject.AddComponent<FuelRodObjectiveSet>();

            Assert.Throws<System.ArgumentException>(() => set.Configure(new[] { first, second }, 1));
        }

        [Test]
        public void GrabbingAnotherRegisteredRodForceReleasesThePreviousRod()
        {
            var first = CreateRod("first", FuelRodClassification.Live);
            var second = CreateRod("second", FuelRodClassification.Live);
            var set = CreateSet(first, second);

            Assert.That(first.Grabbable.TryBeginGrab(new object()), Is.True);
            Assert.That(second.Grabbable.TryBeginGrab(new object()), Is.True);

            Assert.That(first.IsHeld, Is.False);
            Assert.That(second.IsHeld, Is.True);
            Assert.That(set.Progress.Carried, Is.EqualTo(1));
        }

        [Test]
        public void RackProximityAcceptsHeldRodWhenRigidbodyCollisionDetectionIsSuppressed()
        {
            var rod = CreateRod("live", FuelRodClassification.Live);
            var set = CreateSet(rod);

            var controllerObject = new GameObject("Run Controller");
            controllerObject.transform.SetParent(root.transform);
            var controller = controllerObject.AddComponent<PrototypeRunController>();
            controller.Configure(set);
            controller.ResetRun();

            var rackObject = new GameObject("Rack");
            rackObject.transform.SetParent(root.transform);
            var trigger = rackObject.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = Vector3.one * 2f;
            var rack = rackObject.AddComponent<FuelRodDepositRack>();
            var slot = new GameObject("Slot").transform;
            slot.SetParent(rackObject.transform);
            rack.Configure(controller, set, new[] { slot }, null, null);

            rod.transform.position = rackObject.transform.position;
            Assert.That(rod.Grabbable.TryBeginGrab(new object()), Is.True);
            rod.AdvanceAssay(3f, true);
            rod.GetComponent<Rigidbody>().detectCollisions = false;

            rack.PollHeldItemsInAcceptanceVolume();

            Assert.That(rack.LastResult, Is.EqualTo(DepositResult.Accepted));
            Assert.That(rod.IsHeld, Is.False);
            Assert.That(rod.IsDeposited, Is.True);
            Assert.That(rod.transform.parent, Is.SameAs(slot));
            Assert.That(controller.CurrentState, Is.EqualTo(PrototypeRunState.ExtractionCycling));
        }

        [Test]
        public void RackOverlapRecognizesRodEndWhenCenterIsOutsideAcceptanceVolume()
        {
            var rod = CreateRod("edge-live", FuelRodClassification.Live);
            var rodCollider = rod.gameObject.AddComponent<CapsuleCollider>();
            rodCollider.direction = 2;
            rodCollider.radius = 0.2f;
            rodCollider.height = 2f;

            var rackObject = new GameObject("Edge Rack");
            rackObject.transform.SetParent(root.transform);
            var volume = rackObject.AddComponent<BoxCollider>();
            volume.isTrigger = true;
            volume.size = Vector3.one * 2f;

            rod.transform.position = new Vector3(0f, 0f, 1.45f);
            Physics.SyncTransforms();

            Assert.That(FuelRodDepositRack.ContainsWorldPoint(volume, rod.transform.position), Is.False);
            Assert.That(FuelRodDepositRack.OverlapsAcceptanceVolume(volume, rod), Is.True);

            rod.transform.position = new Vector3(0f, 0f, 2.25f);
            Physics.SyncTransforms();
            Assert.That(FuelRodDepositRack.OverlapsAcceptanceVolume(volume, rod), Is.False);
        }

        [Test]
        public void RackPublishesClearResultTransitionsForPlayerFeedback()
        {
            var rod = CreateRod("feedback-live", FuelRodClassification.Live);
            var set = CreateSet(rod);
            var controllerObject = new GameObject("Feedback Run Controller");
            controllerObject.transform.SetParent(root.transform);
            var controller = controllerObject.AddComponent<PrototypeRunController>();
            controller.Configure(set);
            controller.ResetRun();

            var rackObject = new GameObject("Feedback Rack");
            rackObject.transform.SetParent(root.transform);
            var rack = rackObject.AddComponent<FuelRodDepositRack>();
            var slot = new GameObject("Feedback Slot").transform;
            slot.SetParent(rackObject.transform);
            rack.Configure(controller, set, new[] { slot }, null, null);

            var results = new System.Collections.Generic.List<DepositResult>();
            rack.DepositResolved += (_, result) => results.Add(result);

            Assert.That(rack.TryDeposit(rod), Is.EqualTo(DepositResult.Unassayed));
            Assert.That(rod.Grabbable.TryBeginGrab(new object()), Is.True);
            rod.AdvanceAssay(3f, true);
            Assert.That(rack.TryDeposit(rod), Is.EqualTo(DepositResult.Accepted));

            Assert.That(results, Is.EqualTo(new[]
            {
                DepositResult.Unassayed,
                DepositResult.Accepted,
            }));
        }

        [Test]
        public void FailedRunRejectsLaterDepositWithoutMutatingProgress()
        {
            var rod = CreateRod("live", FuelRodClassification.Live);
            var set = CreateSet(rod);
            var controllerObject = new GameObject("Run Controller");
            controllerObject.transform.SetParent(root.transform);
            var controller = controllerObject.AddComponent<PrototypeRunController>();
            controller.Configure(set);
            controller.ResetRun();
            rod.AdvanceAssay(3f, true);

            Assert.That(controller.TryFailRun("Captured."), Is.True);
            Assert.That(controller.TryDepositFuelRod(rod), Is.EqualTo(DepositResult.RunInactive));
            Assert.That(set.Progress.Deposited, Is.Zero);
            Assert.That(rod.IsDeposited, Is.False);
        }

        [Test]
        public void RackConfigurationRejectsMissingRequiredSlots()
        {
            var first = CreateRod("first", FuelRodClassification.Live);
            var second = CreateRod("second", FuelRodClassification.Live);
            var set = CreateSet(first, second);
            var rackObject = new GameObject("Rack");
            rackObject.transform.SetParent(root.transform);
            var rack = rackObject.AddComponent<FuelRodDepositRack>();
            var onlySlot = new GameObject("Only Slot").transform;
            onlySlot.SetParent(rackObject.transform);

            Assert.Throws<System.ArgumentException>(() =>
                rack.Configure(null, set, new[] { onlySlot }, null, null));
        }

        private FuelRodObjectiveSet CreateSet(params FuelRodItem[] rods)
        {
            var setObject = new GameObject("Set");
            setObject.transform.SetParent(root.transform);
            var set = setObject.AddComponent<FuelRodObjectiveSet>();
            set.Configure(rods, rods.Length);
            return set;
        }

        private FuelRodItem CreateRod(string id, FuelRodClassification classification)
        {
            var rodObject = new GameObject(id);
            rodObject.transform.SetParent(root.transform);
            var rod = rodObject.AddComponent<FuelRodItem>();
            rod.ConfigureIdentity(id, classification);
            return rod;
        }
    }
}
