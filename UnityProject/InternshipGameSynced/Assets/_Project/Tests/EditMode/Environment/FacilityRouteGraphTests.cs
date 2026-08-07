using System;
using NUnit.Framework;
using UnityEngine;

namespace InternshipGame.Prototype.Tests
{
    public sealed class FacilityRouteGraphTests
    {
        [TestCase(PrototypeVariationIds.OpenCircuit)]
        [TestCase(PrototypeVariationIds.CoolantService)]
        [TestCase(PrototypeVariationIds.FuelInspection)]
        public void EveryRouteProfile_PreservesTwoRoutesToEverySocket(string profileId)
        {
            Assert.That(FacilityRouteGraph.PreservesCriticalPaths(profileId), Is.True);
        }

        [TestCase(PrototypeVariationIds.OpenCircuit)]
        [TestCase(PrototypeVariationIds.CoolantService)]
        [TestCase(PrototypeVariationIds.FuelInspection)]
        public void EveryAuthoredNode_RemainsReachableWithoutJumping(string profileId)
        {
            Assert.That(
                FacilityRouteGraph.GetReachableNodes(profileId).Count,
                Is.EqualTo(Enum.GetValues(typeof(FacilityRouteNodeId)).Length));
        }

        [TestCase(PrototypeVariationIds.OpenCircuit)]
        [TestCase(PrototypeVariationIds.CoolantService)]
        [TestCase(PrototypeVariationIds.FuelInspection)]
        public void EveryRodSocket_HasTwoEdgeDisjointRoutesFromP01ContainmentSide(string profileId)
        {
            foreach (var socket in FacilityRouteGraph.AuthoredSocketNodes)
            {
                Assert.That(
                    FacilityRouteGraph.HasAtLeastTwoEdgeDisjointPaths(
                        FacilityRouteNodeId.P01Containment,
                        socket,
                        profileId),
                    Is.True,
                    $"{socket} lacks two graph-distinct routes for '{profileId}'.");
            }
        }

        [Test]
        public void RouteEvidenceCompatibility_MapsOnlyTheThreeConnectorZones()
        {
            Assert.That(FacilityZoneId.NorthPipeGallery.ToThreatWing(), Is.EqualTo(ThreatWing.West));
            Assert.That(FacilityZoneId.UtilitySpine.ToThreatWing(), Is.EqualTo(ThreatWing.Central));
            Assert.That(FacilityZoneId.SouthMaintenanceChannel.ToThreatWing(), Is.EqualTo(ThreatWing.East));
            Assert.That(FacilityZoneId.Containment.ToThreatWing(), Is.EqualTo(ThreatWing.None));
            Assert.That(FacilityZoneId.Coolant.ToThreatWing(), Is.EqualTo(ThreatWing.None));
            Assert.That(FacilityZoneId.FuelHandling.ToThreatWing(), Is.EqualTo(ThreatWing.None));
            Assert.That(FacilityZoneId.Annex.ToThreatWing(), Is.EqualTo(ThreatWing.None));
        }

        [Test]
        public void RoutePointLookup_UsesAuthoredConnectorVolumeRatherThanWorldX()
        {
            var root = new GameObject("Map");
            try
            {
                var map = root.AddComponent<PrototypeMapDefinition>();
                var connector = new GameObject("North Connector");
                connector.transform.SetParent(root.transform);
                connector.transform.position = new Vector3(19f, 1f, 0f);
                var collider = connector.AddComponent<BoxCollider>();
                collider.size = new Vector3(4f, 3f, 8f);
                var trigger = connector.AddComponent<FacilityZoneTrigger>();
                trigger.Configure(FacilityZoneId.NorthPipeGallery, map);
                map.Configure(null, null, null, null, null, null, null, null, new[] { trigger });

                Assert.That(
                    map.GetRouteFamilyAt(new Vector3(19f, 1f, 0f)),
                    Is.EqualTo(FacilityRouteFamily.NorthPipeGallery));
                Assert.That(
                    map.GetRouteFamilyAt(new Vector3(-19f, 1f, 0f)),
                    Is.EqualTo(FacilityRouteFamily.None));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void OwlRouteNodeMetadata_IsExplicitOnlyForConnectorNodes()
        {
            var nodeObject = new GameObject("Node");
            try
            {
                var node = nodeObject.AddComponent<OwlRouteNodeAnchor>();
                node.Configure("NPG-01", OwlRouteNodeKind.Connector, FacilityRouteFamily.NorthPipeGallery);
                Assert.That(node.RouteFamily, Is.EqualTo(FacilityRouteFamily.NorthPipeGallery));

                node.Configure("CON-01", OwlRouteNodeKind.Lobe, FacilityRouteFamily.NorthPipeGallery);
                Assert.That(node.RouteFamily, Is.EqualTo(FacilityRouteFamily.None));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(nodeObject);
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
                UnityEngine.Object.DestroyImmediate(sharedRoot);
            }
        }
    }
}
