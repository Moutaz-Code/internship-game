using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace InternshipGame.Prototype.Tests
{
    public sealed class ApprovedFacilitySceneTests
    {
        private const string ScenePath = "Assets/_Project/Scenes/P02_ApprovedFacilityShell.unity";
        private const string P01Path = "Assets/_Project/Scenes/P01_CoreLoop.unity";
        private const string RootName = "_Generated P02 Approved Facility";

        [SetUp]
        public void OpenApprovedScene()
        {
            Assert.That(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath), Is.Not.Null);
            if (SceneManager.GetActiveScene().path != ScenePath)
            {
                EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            }
            Physics.SyncTransforms();
        }

        [Test]
        public void P02_IsSeparateAndDoesNotReplaceBuildSettingsOrP01()
        {
            Assert.That(AssetDatabase.LoadAssetAtPath<SceneAsset>(P01Path), Is.Not.Null);
            Assert.That(EditorBuildSettings.scenes.Any(scene => scene.path == P01Path && scene.enabled), Is.True);
            Assert.That(EditorBuildSettings.scenes.Any(scene => scene.path == ScenePath && scene.enabled), Is.False);
            Assert.That(SceneManager.GetActiveScene().GetRootGameObjects().Count(root => root.name == RootName), Is.EqualTo(1));
        }

        [Test]
        public void GeneratedScene_HasNoMissingScriptsAndRequiredRuntimeAdapters()
        {
            var root = GetOwnedRoot();
            var missing = root.GetComponentsInChildren<Transform>(true)
                .SelectMany(value => value.GetComponents<Component>())
                .Count(component => component == null);
            Assert.That(missing, Is.Zero);
            Assert.That(root.GetComponentsInChildren<PlayerModeController>(true), Has.Length.EqualTo(1));
            Assert.That(root.GetComponentsInChildren<PursuitThreatController>(true), Has.Length.EqualTo(1));
            Assert.That(root.GetComponentsInChildren<NavMeshAgent>(true), Has.Length.EqualTo(1));
            Assert.That(root.GetComponentsInChildren<ThrowableThreatStimulus>(true), Has.Length.EqualTo(9));
            Assert.That(root.GetComponentsInChildren<MachineryDistractionPanel>(true), Has.Length.EqualTo(3));

            var map = root.GetComponentInChildren<PrototypeMapDefinition>(true);
            Assert.That(map, Is.Not.Null);
            Assert.That(map.RodSockets, Is.Empty, "P02 must not silently relocate the active objective distribution.");
            Assert.That(map.OwlPatrolPoints.Count, Is.EqualTo(18));
        }

        [Test]
        public void LockedRoomGraph_IsConnectedAndObjectiveRoomsHaveAtLeastTwoExits()
        {
            var roomIds = ApprovedFacilityLayout.Rooms.Select(room => room.Id).ToHashSet(StringComparer.Ordinal);
            var adjacency = roomIds.ToDictionary(id => id, _ => new HashSet<string>(StringComparer.Ordinal));
            foreach (var portal in ApprovedFacilityLayout.Portals)
            {
                if (!roomIds.Contains(portal.FromRoomId) || !roomIds.Contains(portal.ToRoomId)) continue;
                adjacency[portal.FromRoomId].Add(portal.ToRoomId);
                adjacency[portal.ToRoomId].Add(portal.FromRoomId);
            }

            var visited = new HashSet<string>(StringComparer.Ordinal) { "ROOM-ENTRANCE" };
            var queue = new Queue<string>();
            queue.Enqueue("ROOM-ENTRANCE");
            while (queue.Count > 0)
            {
                foreach (var next in adjacency[queue.Dequeue()])
                {
                    if (visited.Add(next)) queue.Enqueue(next);
                }
            }

            Assert.That(visited, Is.EquivalentTo(roomIds));
            Assert.That(adjacency["ROOM-ASSAY-A"].Count, Is.GreaterThanOrEqualTo(2));
            Assert.That(adjacency["ROOM-FUEL-B"].Count, Is.GreaterThanOrEqualTo(2));
            Assert.That(adjacency["ROOM-CONTROL-C"].Count, Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        public void Reactor_IsInaccessibleHasOneLeadGlassWindowAndNoOneWayPlacement()
        {
            var root = GetOwnedRoot();
            var transforms = root.GetComponentsInChildren<Transform>(true);
            Assert.That(transforms.Count(value => value.name == "REACTOR-LEAD-GLASS-WINDOW (Exactly One)"), Is.EqualTo(1));
            Assert.That(transforms.Any(value => value.name == "REACTOR-CORE No Floor No Objective No Navigation"), Is.True);
            Assert.That(root.GetComponentsInChildren<OneWayTraversalController>(true), Is.Empty,
                "One-way variants remain palette-only until a macro-connectivity placement is approved.");

            var center = ApprovedFacilityLayout.ImageToWorld(ApprovedFacilityLayout.ReactorImageCenter);
            Assert.That(ApprovedFacilityLayout.SemanticAnchors.Any(anchor =>
                Vector3.Distance(ApprovedFacilityLayout.ImageToWorld(anchor.ImagePosition), center) <
                ApprovedFacilityLayout.ReactorRadiusPixels * ApprovedFacilityLayout.PixelsToMeters), Is.False);
        }

        [Test]
        public void ObjectiveAndExtractionDrawingMarkers_ArePresentAndInactive()
        {
            var placeholders = GetOwnedRoot().GetComponentsInChildren<FacilitySemanticAnchor>(true)
                .Where(anchor => anchor.Kind is FacilitySemanticKind.ObjectivePlaceholder or
                    FacilitySemanticKind.ExtractionPlaceholder)
                .ToArray();
            Assert.That(placeholders, Has.Length.EqualTo(6));
            Assert.That(placeholders.All(anchor => !anchor.GameplayActive && !anchor.gameObject.activeSelf), Is.True);
        }

        [Test]
        public void EveryDeclaredPortal_HasAFrameClearWallThroatAndNavigationLink()
        {
            var facility = GameObject.Find("Generated Approved Facility Shell");
            Assert.That(facility, Is.Not.Null);
            var wallRoot = facility.transform.Find("Modular Wall Centerlines");
            var portalRoot = facility.transform.Find("Approved Portal Frames");
            Assert.That(wallRoot, Is.Not.Null);
            Assert.That(portalRoot, Is.Not.Null);

            foreach (var portal in ApprovedFacilityLayout.Portals)
            {
                var instance = portalRoot.Find(portal.Id);
                Assert.That(instance, Is.Not.Null, portal.Id);
                var links = instance.GetComponents<MonoBehaviour>()
                    .Where(component => component != null && component.GetType().Name == "NavMeshLink")
                    .ToArray();
                Assert.That(links, Has.Length.EqualTo(1), portal.Id);

                var center = ApprovedFacilityLayout.ImageToWorld(portal.ImageCenter, 1.2f);
                var rotation = Quaternion.Euler(0f,
                    ApprovedFacilityLayout.ImageYawToWorldYaw(portal.ImageYawDegrees), 0f);
                var wallBlockers = Physics.OverlapBox(center, new Vector3(0.9f, 1.1f, 0.55f), rotation,
                        ~0, QueryTriggerInteraction.Ignore)
                    .Where(hit => hit != null && hit.transform.IsChildOf(wallRoot))
                    .ToArray();
                Assert.That(wallBlockers, Is.Empty, $"{portal.Id} wall throat");
            }
        }

        [Test]
        public void NavigationBakeAndOwlFallbackGraph_ArePresentAndConnected()
        {
            var root = GetOwnedRoot();
            var surface = root.GetComponentsInChildren<MonoBehaviour>(true)
                .SingleOrDefault(component => component != null && component.GetType().Name == "NavMeshSurface");
            Assert.That(surface, Is.Not.Null);
            Assert.That(new SerializedObject(surface).FindProperty("m_NavMeshData").objectReferenceValue, Is.Not.Null);
            Assert.That(NavMesh.CalculateTriangulation().vertices.Length, Is.GreaterThan(100));
            Assert.That(root.GetComponentsInChildren<MonoBehaviour>(true)
                .Count(component => component != null && component.GetType().Name == "NavMeshLink"), Is.EqualTo(17));

            var graph = root.GetComponentInChildren<WaypointGraphThreatNavigator>(true);
            Assert.That(graph, Is.Not.Null);
            var serialized = new SerializedObject(graph);
            var waypoints = serialized.FindProperty("waypoints");
            var connections = serialized.FindProperty("connections");
            Assert.That(waypoints.arraySize, Is.EqualTo(18));
            Assert.That(connections.arraySize, Is.GreaterThanOrEqualTo(17));

            var adjacency = Enumerable.Range(0, waypoints.arraySize)
                .ToDictionary(index => index, _ => new HashSet<int>());
            for (var i = 0; i < connections.arraySize; i++)
            {
                var connection = connections.GetArrayElementAtIndex(i);
                var from = connection.FindPropertyRelative("fromIndex").intValue;
                var to = connection.FindPropertyRelative("toIndex").intValue;
                adjacency[from].Add(to);
                adjacency[to].Add(from);
            }

            var visited = new HashSet<int> { 0 };
            var queue = new Queue<int>();
            queue.Enqueue(0);
            while (queue.Count > 0)
            {
                foreach (var next in adjacency[queue.Dequeue()])
                {
                    if (visited.Add(next)) queue.Enqueue(next);
                }
            }
            Assert.That(visited.Count, Is.EqualTo(waypoints.arraySize));
        }

        [Test]
        public void FullCoverInstances_AreThickAndCollidable()
        {
            var cover = GetOwnedRoot().GetComponentsInChildren<FacilityModuleMetadata>(true)
                .Where(metadata => metadata.CoverClass == FacilityCoverClass.FullCover)
                .ToArray();
            Assert.That(cover.Length, Is.GreaterThanOrEqualTo(4));
            foreach (var module in cover)
            {
                Assert.That(module.AuthoredBounds.y, Is.GreaterThanOrEqualTo(2.2f), module.StableId);
                Assert.That(module.AuthoredBounds.z, Is.GreaterThanOrEqualTo(0.7f), module.StableId);
                Assert.That(module.GetComponentsInChildren<Collider>(true), Is.Not.Empty, module.StableId);
            }
        }

        private static GameObject GetOwnedRoot()
        {
            var matches = SceneManager.GetActiveScene().GetRootGameObjects()
                .Where(root => root.name == RootName)
                .ToArray();
            Assert.That(matches, Has.Length.EqualTo(1));
            return matches[0];
        }
    }
}
