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
        private const string RootName = "_Generated P02 Gameplay Integration";
        private const string PopulationRootName = "_Generated P02 Room Population";
        private const string GeneratedColliderPrefix = "__P02_GeneratedWallCollider_";

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
        public void ManualScene_HasNoMissingScriptsAndRequiredRuntimeAdapters()
        {
            var root = GetOwnedRoot();
            var missing = root.GetComponentsInChildren<Transform>(true)
                .SelectMany(value => value.GetComponents<Component>())
                .Count(component => component == null);
            Assert.That(missing, Is.Zero);
            Assert.That(root.GetComponentsInChildren<PlayerModeController>(true), Has.Length.EqualTo(1));
            Assert.That(root.GetComponentsInChildren<PursuitThreatController>(true), Has.Length.EqualTo(1));
            Assert.That(root.GetComponentsInChildren<NavMeshAgent>(true), Has.Length.EqualTo(1));

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
            Assert.That(root.GetComponentsInChildren<OneWayTraversalController>(true), Is.Empty,
                "One-way variants remain palette-only until a macro-connectivity placement is approved.");

            var center = ManualImageToWorld(ApprovedFacilityLayout.ReactorImageCenter);
            Assert.That(NavMesh.SamplePosition(center, out _, 1f, NavMesh.AllAreas), Is.False,
                "The circular reactor may not receive a navigable floor.");
            Assert.That(ApprovedFacilityLayout.SemanticAnchors.Any(anchor =>
                Vector3.Distance(ManualImageToWorld(anchor.ImagePosition), center) <
                ApprovedFacilityLayout.ReactorRadiusPixels * ManualMetersPerPixel()), Is.False);
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
        public void EveryDeclaredPortal_HasNavigableManualShellThroat()
        {
            foreach (var portal in ApprovedFacilityLayout.Portals)
            {
                var center = ManualImageToWorld(portal.ImageCenter);
                Assert.That(NavMesh.SamplePosition(center, out _, 12f, NavMesh.AllAreas), Is.True, portal.Id);
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
            Assert.That(NavMesh.CalculateTriangulation().vertices.Length, Is.GreaterThan(0));
            Assert.That(root.transform.Find(
                    "P02 Manual Shell Navigation/Disposable Bake Sources (Disabled After Bake)")
                .GetComponentsInChildren<Collider>(true).All(collider => !collider.enabled), Is.True);

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
        public void ManualWallInstances_HaveGeneratedSolidNavBlockingColliders()
        {
            var scene = SceneManager.GetActiveScene();
            var rooms = scene.GetRootGameObjects().Single(root => root.name == "Rooms");
            var hallways = scene.GetRootGameObjects().Single(root => root.name == "Hallways");
            var generated = rooms.GetComponentsInChildren<Collider>(true)
                .Concat(hallways.GetComponentsInChildren<Collider>(true))
                .Where(collider => collider.name.StartsWith(GeneratedColliderPrefix, StringComparison.Ordinal))
                .ToArray();
            Assert.That(generated.Length, Is.GreaterThan(600));
            foreach (var collider in generated)
            {
                Assert.That(collider.isTrigger, Is.False, collider.name);
                if (collider is BoxCollider)
                {
                    var scale = collider.transform.lossyScale;
                    Assert.That(scale.x, Is.GreaterThanOrEqualTo(0f), collider.name);
                    Assert.That(scale.y, Is.GreaterThanOrEqualTo(0f), collider.name);
                    Assert.That(scale.z, Is.GreaterThanOrEqualTo(0f), collider.name);
                }
                var modifier = collider.GetComponents<MonoBehaviour>()
                    .SingleOrDefault(component => component != null && component.GetType().Name == "NavMeshModifier");
                Assert.That(modifier, Is.Not.Null, collider.name);
                var serialized = new SerializedObject(modifier);
                Assert.That(serialized.FindProperty("m_OverrideArea").boolValue, Is.True, collider.name);
                Assert.That(serialized.FindProperty("m_Area").intValue,
                    Is.EqualTo(NavMesh.GetAreaFromName("Not Walkable")), collider.name);
            }
        }

        [Test]
        public void Population_IsSeparatelyOwnedPrefabLinkedAndUsesWarehouseFirstWhereAvailable()
        {
            var population = GetPopulationRoot();
            Assert.That(population.transform.parent, Is.Null);
            Assert.That(population.transform.childCount, Is.EqualTo(9));
            var items = GetPopulationItems(population);
            Assert.That(items, Has.Length.EqualTo(74));
            Assert.That(items.Select(item => item.name).Distinct(StringComparer.Ordinal).Count(), Is.EqualTo(items.Length));

            var sourcePaths = items.Select(item =>
            {
                Assert.That(PrefabUtility.GetPrefabInstanceStatus(item), Is.Not.EqualTo(PrefabInstanceStatus.NotAPrefab), item.name);
                var source = PrefabUtility.GetCorrespondingObjectFromSource(item);
                Assert.That(source, Is.Not.Null, item.name);
                return AssetDatabase.GetAssetPath(source);
            }).ToArray();
            Assert.That(sourcePaths.Count(path => path.StartsWith(
                "Assets/Imports/SciFi Warehouse Kit/Prefabs/", StringComparison.Ordinal)), Is.EqualTo(18));
            Assert.That(sourcePaths.Count(path => path.StartsWith(
                "Assets/_Project/Prefabs/Environment/FacilityKit/", StringComparison.Ordinal)), Is.EqualTo(56));

            var sceneRoots = SceneManager.GetActiveScene().GetRootGameObjects();
            Assert.That(sceneRoots.Single(root => root.name == "Rooms").transform.IsChildOf(population.transform), Is.False);
            Assert.That(sceneRoots.Single(root => root.name == "Hallways").transform.IsChildOf(population.transform), Is.False);
            Assert.That(sceneRoots.Single(root => root.name == "Basic Floor").transform.IsChildOf(population.transform), Is.False);
        }

        [Test]
        public void Population_HasPhysicalNavigationAndDistractionAdapters()
        {
            var population = GetPopulationRoot();
            var activeOwl = GetOwnedRoot().GetComponentInChildren<PursuitThreatController>(true);
            var items = GetPopulationItems(population);
            Assert.That(items.All(item => item.GetComponentsInChildren<Collider>(true).Length > 0), Is.True);
            Assert.That(items.SelectMany(item => item.GetComponents<Component>()).Count(component => component == null), Is.Zero);

            var obstacles = population.GetComponentsInChildren<NavMeshObstacle>(true);
            Assert.That(obstacles, Has.Length.EqualTo(64));
            Assert.That(obstacles.All(obstacle => obstacle.carving && obstacle.size.x > 0f &&
                                                       obstacle.size.y > 0f && obstacle.size.z > 0f), Is.True);

            var portable = population.GetComponentsInChildren<ThrowableThreatStimulus>(true);
            Assert.That(portable, Has.Length.EqualTo(10));
            foreach (var stimulus in portable)
            {
                Assert.That(stimulus.GetComponent<Rigidbody>(), Is.Not.Null, stimulus.name);
                Assert.That(stimulus.GetComponent<GrabbableObject>(), Is.Not.Null, stimulus.name);
                Assert.That(stimulus.GetComponent("XRGrabInteractable"), Is.Not.Null, stimulus.name);
                Assert.That(new SerializedObject(stimulus).FindProperty("stimulusSinkBehaviour").objectReferenceValue,
                    Is.SameAs(activeOwl), stimulus.name);
            }

            var panels = population.GetComponentsInChildren<MachineryDistractionPanel>(true);
            Assert.That(panels, Has.Length.EqualTo(3));
            foreach (var panel in panels)
            {
                var serialized = new SerializedObject(panel);
                Assert.That(serialized.FindProperty("stimulusSinkBehaviour").objectReferenceValue,
                    Is.SameAs(activeOwl), panel.name);
                Assert.That(serialized.FindProperty("indicatorRenderer").objectReferenceValue, Is.Not.Null, panel.name);
                Assert.That(serialized.FindProperty("readyMaterial").objectReferenceValue, Is.Not.Null, panel.name);
                Assert.That(serialized.FindProperty("activeMaterial").objectReferenceValue, Is.Not.Null, panel.name);
            }
        }

        [Test]
        public void Population_DoesNotOverlapManualWallsOtherPropsOrPortalClearance()
        {
            Physics.SyncTransforms();
            var scene = SceneManager.GetActiveScene();
            var rooms = scene.GetRootGameObjects().Single(root => root.name == "Rooms").transform;
            var hallways = scene.GetRootGameObjects().Single(root => root.name == "Hallways").transform;
            var items = GetPopulationItems(GetPopulationRoot());
            var itemBounds = items.Select(item => (item, bounds: RendererBounds(item))).ToArray();

            foreach (var pair in itemBounds)
            {
                var extents = new Vector3(
                    Mathf.Max(0.02f, pair.bounds.extents.x - 0.04f),
                    Mathf.Max(0.02f, pair.bounds.extents.y - 0.04f),
                    Mathf.Max(0.02f, pair.bounds.extents.z - 0.04f));
                var wallHits = Physics.OverlapBox(pair.bounds.center, extents, Quaternion.identity,
                        Physics.AllLayers, QueryTriggerInteraction.Ignore)
                    .Where(collider => collider != null &&
                                       (collider.transform.IsChildOf(rooms) || collider.transform.IsChildOf(hallways)))
                    .ToArray();
                Assert.That(wallHits, Is.Empty, pair.item.name);
            }

            for (var first = 0; first < itemBounds.Length; first++)
            {
                for (var second = first + 1; second < itemBounds.Length; second++)
                {
                    var a = itemBounds[first].bounds;
                    var b = itemBounds[second].bounds;
                    a.Expand(-0.08f);
                    b.Expand(-0.08f);
                    Assert.That(a.Intersects(b), Is.False,
                        $"{itemBounds[first].item.name} overlaps {itemBounds[second].item.name}");
                }
            }

            foreach (var portal in ApprovedFacilityLayout.Portals)
            {
                var portalPosition = ManualImageToWorld(portal.ImageCenter);
                foreach (var pair in itemBounds)
                {
                    var deltaX = Mathf.Max(Mathf.Abs(portalPosition.x - pair.bounds.center.x) - pair.bounds.extents.x, 0f);
                    var deltaZ = Mathf.Max(Mathf.Abs(portalPosition.z - pair.bounds.center.z) - pair.bounds.extents.z, 0f);
                    Assert.That(Mathf.Sqrt(deltaX * deltaX + deltaZ * deltaZ), Is.GreaterThanOrEqualTo(2f),
                        $"{pair.item.name} encroaches {portal.Id}");
                }
            }
        }

        private static Vector3 ManualImageToWorld(Vector2 imagePoint)
        {
            var plan = SceneManager.GetActiveScene().GetRootGameObjects().Single(root => root.name == "FloorPlan");
            var local = new Vector3(
                imagePoint.x / ApprovedFacilityLayout.SourceWidthPixels - 0.5f,
                0.5f - imagePoint.y / ApprovedFacilityLayout.SourceHeightPixels,
                0f);
            return plan.transform.TransformPoint(local);
        }

        private static float ManualMetersPerPixel()
        {
            var plan = SceneManager.GetActiveScene().GetRootGameObjects().Single(root => root.name == "FloorPlan");
            return (Mathf.Abs(plan.transform.lossyScale.x) + Mathf.Abs(plan.transform.lossyScale.y)) * 0.5f /
                   ApprovedFacilityLayout.SourceWidthPixels;
        }

        private static GameObject GetOwnedRoot()
        {
            var matches = SceneManager.GetActiveScene().GetRootGameObjects()
                .Where(root => root.name == RootName)
                .ToArray();
            Assert.That(matches, Has.Length.EqualTo(1));
            return matches[0];
        }

        private static GameObject GetPopulationRoot()
        {
            var matches = SceneManager.GetActiveScene().GetRootGameObjects()
                .Where(root => root.name == PopulationRootName)
                .ToArray();
            Assert.That(matches, Has.Length.EqualTo(1));
            return matches[0];
        }

        private static GameObject[] GetPopulationItems(GameObject population)
        {
            return population.transform.Cast<Transform>()
                .SelectMany(group => group.Cast<Transform>())
                .Select(item => item.gameObject)
                .ToArray();
        }

        private static Bounds RendererBounds(GameObject gameObject)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>(true);
            Assert.That(renderers, Is.Not.Empty, gameObject.name);
            var bounds = renderers[0].bounds;
            for (var index = 1; index < renderers.Length; index++) bounds.Encapsulate(renderers[index].bounds);
            return bounds;
        }
    }
}
