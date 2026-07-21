using NUnit.Framework;
using UnityEngine;

namespace InternshipGame.Prototype.Tests
{
    public sealed class FacilityRouteGraphTests
    {
        [TestCase(PrototypeVariationIds.MaintenanceObstruction)]
        [TestCase(PrototypeVariationIds.LaboratoryQuarantine)]
        [TestCase(PrototypeVariationIds.CentralPowerOutage)]
        public void EveryRouteProfile_PreservesAllCriticalPathsWithoutJumping(string profileId)
        {
            Assert.That(FacilityRouteGraph.PreservesCriticalPaths(profileId), Is.True);
        }

        [Test]
        public void BaselineGraph_ContainsAllTenAuthoredSpaces()
        {
            Assert.That(FacilityRouteGraph.GetReachableZones(string.Empty).Count, Is.EqualTo(10));
        }

        [TestCase(PrototypeVariationIds.MaintenanceObstruction)]
        [TestCase(PrototypeVariationIds.LaboratoryQuarantine)]
        [TestCase(PrototypeVariationIds.CentralPowerOutage)]
        public void EveryRodDestination_HasTwoEdgeDisjointNonJumpRoutes(string profileId)
        {
            var destinations = new[]
            {
                FacilityZoneId.WestMaintenance,
                FacilityZoneId.CentralTransfer,
                FacilityZoneId.EastLaboratory,
                FacilityZoneId.WestRodStorage,
                FacilityZoneId.CoreVault,
                FacilityZoneId.EastReactor,
            };

            foreach (var destination in destinations)
            {
                Assert.That(
                    FacilityRouteGraph.HasAtLeastTwoEdgeDisjointPaths(
                        FacilityZoneId.OperationsHub,
                        destination,
                        profileId),
                    Is.True,
                    $"{destination} lacks two non-jump route options for '{profileId}'.");
            }
        }

        [Test]
        public void ZoneTracking_OnlyAcceptsCollidersUnderTaggedPlayerAncestor()
        {
            var sharedRoot = new GameObject("Shared Generated Root");
            var player = new GameObject("Player");
            var prop = new GameObject("Prop");
            try
            {
                player.tag = "Player";
                player.transform.SetParent(sharedRoot.transform);
                prop.transform.SetParent(sharedRoot.transform);
                var playerChild = new GameObject("Player Collider");
                playerChild.transform.SetParent(player.transform);
                var playerCollider = playerChild.AddComponent<BoxCollider>();
                var propCollider = prop.AddComponent<BoxCollider>();

                Assert.That(FacilityZoneTrigger.BelongsToPlayer(playerCollider), Is.True);
                Assert.That(FacilityZoneTrigger.BelongsToPlayer(propCollider), Is.False);
            }
            finally
            {
                Object.DestroyImmediate(sharedRoot);
            }
        }
    }
}
