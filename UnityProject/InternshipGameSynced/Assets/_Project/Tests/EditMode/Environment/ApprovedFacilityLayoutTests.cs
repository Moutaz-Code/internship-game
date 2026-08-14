using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace InternshipGame.Prototype.Tests
{
    public sealed class ApprovedFacilityLayoutTests
    {
        [Test]
        public void SourceTransform_IsUniformRoundTrippableAndKeepsCanvasAspect()
        {
            Assert.That(ApprovedFacilityLayout.SourceWidthPixels, Is.EqualTo(2048));
            Assert.That(ApprovedFacilityLayout.SourceHeightPixels, Is.EqualTo(2048));
            Assert.That(ApprovedFacilityLayout.PixelsToMeters, Is.GreaterThan(0f));
            Assert.That(ApprovedFacilityLayout.SourceSha256, Has.Length.EqualTo(64));

            var origin = ApprovedFacilityLayout.ImageToWorld(ApprovedFacilityLayout.ImageOrigin);
            Assert.That(origin, Is.EqualTo(ApprovedFacilityLayout.WorldOrigin));

            var image = new Vector2(1733.25f, 411.75f);
            var roundTrip = ApprovedFacilityLayout.WorldToImage(ApprovedFacilityLayout.ImageToWorld(image));
            Assert.That(roundTrip.x, Is.EqualTo(image.x).Within(0.001f));
            Assert.That(roundTrip.y, Is.EqualTo(image.y).Within(0.001f));

            var xMeter = Vector3.Distance(
                ApprovedFacilityLayout.ImageToWorld(Vector2.zero),
                ApprovedFacilityLayout.ImageToWorld(Vector2.right));
            var zMeter = Vector3.Distance(
                ApprovedFacilityLayout.ImageToWorld(Vector2.zero),
                ApprovedFacilityLayout.ImageToWorld(Vector2.up));
            Assert.That(xMeter, Is.EqualTo(zMeter).Within(0.000001f));
            Assert.That(xMeter, Is.EqualTo(ApprovedFacilityLayout.PixelsToMeters).Within(0.000001f));
        }

        [Test]
        public void Layout_ContainsExpectedRoomsPortalsBlockedZonesAndReactorIds()
        {
            var rooms = new HashSet<string>(StringComparer.Ordinal);
            foreach (var room in ApprovedFacilityLayout.Rooms) rooms.Add(room.Id);
            CollectionAssert.IsSubsetOf(new[]
            {
                "ROOM-ENTRANCE", "ROOM-ASSAY-A", "ROOM-CONTROL-C", "ROOM-CONTAINMENT",
                "ROOM-COOLANT", "ROOM-SCRUBBER", "ROOM-FUEL-B", "ROOM-TOOL-CRIB",
                "ROOM-EXTRACTION-A", "ROOM-EXTRACTION-B", "ROOM-EXTRACTION-C",
            }, rooms);
            Assert.That(ApprovedFacilityLayout.Portals.Length, Is.EqualTo(17));
            Assert.That(ApprovedFacilityLayout.BlockedZones.Length, Is.EqualTo(5));
            Assert.That(ApprovedFacilityLayout.WallPaths.Length, Is.GreaterThanOrEqualTo(50));

            var ids = new HashSet<string>(StringComparer.Ordinal);
            foreach (var id in ApprovedFacilityLayout.EnumerateStableIds())
            {
                Assert.That(ids.Add(id), Is.True, $"Duplicate stable layout id: {id}");
            }
            Assert.That(ids.Contains("REACTOR-CORE"), Is.True);
            Assert.That(ids.Contains("REACTOR-LEAD-GLASS-WINDOW"), Is.True);
        }

        [Test]
        public void Reactor_HasNoPortalOrGameplayAnchorInsideItsBoundary()
        {
            var center = ApprovedFacilityLayout.ReactorImageCenter;
            foreach (var portal in ApprovedFacilityLayout.Portals)
            {
                Assert.That(
                    Vector2.Distance(portal.ImageCenter, center),
                    Is.GreaterThan(ApprovedFacilityLayout.ReactorRadiusPixels),
                    $"{portal.Id} invents access through the locked reactor boundary.");
            }

            foreach (var anchor in ApprovedFacilityLayout.SemanticAnchors)
            {
                Assert.That(
                    Vector2.Distance(anchor.ImagePosition, center),
                    Is.GreaterThan(ApprovedFacilityLayout.ReactorRadiusPixels),
                    $"{anchor.Id} is not permitted inside the reactor core.");
            }
        }

        [Test]
        public void DrawingPlaceholders_AreExplicitlyInactive()
        {
            var placeholderCount = 0;
            foreach (var anchor in ApprovedFacilityLayout.SemanticAnchors)
            {
                if (anchor.Kind is not (FacilitySemanticKind.ObjectivePlaceholder or FacilitySemanticKind.ExtractionPlaceholder))
                {
                    continue;
                }
                placeholderCount++;
                Assert.That(anchor.GameplayActive, Is.False, $"{anchor.Id} was activated without relocation approval.");
            }
            Assert.That(placeholderCount, Is.EqualTo(6));
        }

        [Test]
        public void FacilityKitCatalog_IsCompleteUniqueAndBackedByPrefabs()
        {
            Assert.That(FacilityKitCatalog.Entries.Length, Is.EqualTo(52));
            var ids = new HashSet<string>(StringComparer.Ordinal);
            var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var categoryCounts = new Dictionary<FacilityKitCategory, int>();
            foreach (FacilityKitCategory category in Enum.GetValues(typeof(FacilityKitCategory))) categoryCounts[category] = 0;

            foreach (var entry in FacilityKitCatalog.Entries)
            {
                Assert.That(ids.Add(entry.StableId), Is.True, $"Duplicate FacilityKit id {entry.StableId}");
                Assert.That(paths.Add(entry.PrefabPath), Is.True, $"Duplicate FacilityKit path {entry.PrefabPath}");
                categoryCounts[entry.Category]++;
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(entry.PrefabPath);
                Assert.That(prefab, Is.Not.Null, entry.PrefabPath);
                var metadata = prefab.GetComponent<FacilityModuleMetadata>();
                Assert.That(metadata, Is.Not.Null, entry.PrefabPath);
                Assert.That(metadata.StableId, Is.EqualTo(entry.StableId));
                Assert.That(metadata.Category, Is.EqualTo(entry.Category));
            }

            foreach (var pair in categoryCounts) Assert.That(pair.Value, Is.GreaterThan(0), pair.Key.ToString());
            Assert.That(AssetDatabase.LoadAssetAtPath<SceneAsset>(FacilityKitCatalog.PaletteScenePath), Is.Not.Null);
        }

        [Test]
        public void RequiredKitFamilies_HaveCollidersAndExpectedBehavior()
        {
            foreach (var id in new[]
            {
                "FK-WALL-1M", "FK-WALL-2M", "FK-WALL-4M", "FK-PORTAL-FRAME",
                "FK-WINDOW-OBSERVATION", "FK-WINDOW-SERVICE", "FK-WINDOW-REACTOR-LEADGLASS",
                "FK-DOOR-SLIDING", "FK-DOOR-HINGED", "FK-DOOR-LOCKED",
                "FK-SHIELD-STRAIGHT", "FK-SHIELD-CORNER", "FK-COOLANT-BASIN",
                "FK-PUMP-A", "FK-SCRUBBER-VESSEL", "FK-CASK-ROW",
            })
            {
                Assert.That(FacilityKitCatalog.TryGet(id, out var entry), Is.True, id);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(entry.PrefabPath);
                Assert.That(prefab.GetComponentsInChildren<Collider>(true), Is.Not.Empty, id);
            }

            var transparentLayer = LayerMask.NameToLayer("PerceptionTransparent");
            Assert.That(transparentLayer, Is.GreaterThanOrEqualTo(0));
            foreach (var id in new[] { "FK-WINDOW-OBSERVATION", "FK-WINDOW-SERVICE", "FK-WINDOW-REACTOR-LEADGLASS" })
            {
                FacilityKitCatalog.TryGet(id, out var entry);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(entry.PrefabPath);
                var hasTransparentCollider = false;
                foreach (var collider in prefab.GetComponentsInChildren<Collider>(true))
                {
                    if (collider.gameObject.layer == transparentLayer) hasTransparentCollider = true;
                }
                Assert.That(hasTransparentCollider, Is.True, id);
            }
        }

        [Test]
        public void OneWayPrefabs_ReuseOneControllerAndExposeDistinctOwlPolicies()
        {
            var policies = new HashSet<OneWayOwlPolicy>();
            foreach (var id in new[] { "FK-ONEWAY-BULKHEAD", "FK-ONEWAY-CRASHBAR", "FK-ONEWAY-VALVE" })
            {
                FacilityKitCatalog.TryGet(id, out var entry);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(entry.PrefabPath);
                var controllers = prefab.GetComponentsInChildren<OneWayTraversalController>(true);
                Assert.That(controllers, Has.Length.EqualTo(1), id);
                policies.Add(controllers[0].OwlPolicy);
                Assert.That(controllers[0].PlayerPermission, Is.True);
                Assert.That(controllers[0].CloseAfterPass, Is.True);
            }
            Assert.That(policies.Count, Is.EqualTo(3));
        }

        [Test]
        public void OneWayTraversal_DeniesWrongSideHandlesObstructionAndResets()
        {
            var root = new GameObject("One Way Test");
            var leaf = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leaf.transform.SetParent(root.transform, false);
            var clearanceObject = new GameObject("Clearance");
            clearanceObject.transform.SetParent(root.transform, false);
            var clearance = clearanceObject.AddComponent<BoxCollider>();
            clearance.isTrigger = true;
            clearance.size = new Vector3(2f, 2f, 2f);
            var controller = root.AddComponent<OneWayTraversalController>();
            controller.Configure(
                leaf.transform,
                leaf.GetComponent<Collider>(),
                clearance,
                Vector3.forward,
                true,
                OneWayOwlPolicy.FollowPlayerRule,
                TraversalDoorState.Closed,
                true,
                0.5f,
                Vector3.right * 2f);

            var denialCount = 0;
            controller.Denied.AddListener(() => denialCount++);
            Assert.That(controller.TryRequestPass(root.transform.position + Vector3.forward, TraversalActorKind.Player), Is.False);
            Assert.That(denialCount, Is.EqualTo(1));
            Assert.That(controller.CurrentState, Is.EqualTo(TraversalDoorState.Closed));
            Assert.That(controller.TryRequestPass(root.transform.position - Vector3.forward, TraversalActorKind.Player), Is.True);
            Assert.That(controller.CurrentState, Is.EqualTo(TraversalDoorState.Open));

            var carriedRodStandIn = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            carriedRodStandIn.name = "Carried Rod Stand-In";
            carriedRodStandIn.transform.position = root.transform.position + Vector3.up;
            Physics.SyncTransforms();
            Assert.That(controller.IsObstructed(), Is.True);
            Assert.That(controller.TryCloseNow(), Is.False);

            UnityEngine.Object.DestroyImmediate(carriedRodStandIn);
            Physics.SyncTransforms();
            Assert.That(controller.TryCloseNow(), Is.True);
            controller.SetState(TraversalDoorState.Open);
            controller.OnRunReset();
            Assert.That(controller.CurrentState, Is.EqualTo(TraversalDoorState.Closed));
            UnityEngine.Object.DestroyImmediate(root);
        }
    }
}
