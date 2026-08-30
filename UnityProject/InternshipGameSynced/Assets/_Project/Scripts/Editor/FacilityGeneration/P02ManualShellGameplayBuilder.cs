using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

namespace InternshipGame.Prototype.Editor
{
    /// <summary>
    /// Adds only gameplay adapters to the user's hand-authored P02 shell. The wall,
    /// floor, ceiling, reference image, and deprecated generated shell are not owned
    /// by this builder and are never moved or regenerated here.
    /// </summary>
    public static partial class PrototypeSceneBuilder
    {
        internal const string P02GameplayRootName = "_Generated P02 Gameplay Integration";
        private const string ManualFloorPlanName = "FloorPlan";
        private const string ManualDesktopPlayerName = "Desktop Player_";

        private static readonly FacilityPolygonSpec[] ManualContainmentFloorAreas =
        {
            new FacilityPolygonSpec("FLOOR-CONTAINMENT-EAST", "Containment east gallery",
                new Vector2(884, 545), new Vector2(1055, 452), new Vector2(1195, 452),
                new Vector2(1320, 680), new Vector2(1455, 830), new Vector2(1455, 1125),
                new Vector2(1270, 1215), new Vector2(1120, 1160), new Vector2(1000, 1240),
                new Vector2(885, 1160)),
            new FacilityPolygonSpec("FLOOR-REACTOR-NORTH", "Reactor north approach",
                new Vector2(690, 525), new Vector2(1010, 330), new Vector2(1060, 452),
                new Vector2(884, 640), new Vector2(884, 700), new Vector2(690, 610)),
            new FacilityPolygonSpec("FLOOR-REACTOR-SOUTH", "Reactor south approach",
                new Vector2(884, 1040), new Vector2(1000, 1240), new Vector2(900, 1390),
                new Vector2(750, 1360), new Vector2(650, 1140), new Vector2(665, 1020)),
            new FacilityPolygonSpec("FLOOR-SOUTH-CENTRAL-APPROACH", "South central approved passage",
                new Vector2(1098, 1100), new Vector2(1232, 1100), new Vector2(1232, 1475),
                new Vector2(1098, 1475)),
        };

        private static readonly (string id, Vector2 imagePoint, OwlRouteNodeKind kind)[] ManualPatrolSpecs =
        {
            ("P02-ENT-01", new Vector2(1138, 1760), OwlRouteNodeKind.Lobe),
            ("P02-ASSAY-01", new Vector2(860, 1570), OwlRouteNodeKind.Lobe),
            ("P02-SOUTH-01", new Vector2(940, 1420), OwlRouteNodeKind.Lobe),
            ("P02-GALLERY-01", new Vector2(970, 1080), OwlRouteNodeKind.Lobe),
            ("P02-GALLERY-02", new Vector2(1220, 950), OwlRouteNodeKind.Lobe),
            ("P02-GALLERY-03", new Vector2(1120, 650), OwlRouteNodeKind.Lobe),
            ("P02-CONTROL-01", new Vector2(1060, 155), OwlRouteNodeKind.Lobe),
            ("P02-EXTC-01", new Vector2(880, 430), OwlRouteNodeKind.Lobe),
            ("P02-COOLANT-01", new Vector2(1390, 360), OwlRouteNodeKind.Lobe),
            ("P02-COOLANT-02", new Vector2(1580, 390), OwlRouteNodeKind.Lobe),
            ("P02-COOLANT-03", new Vector2(1730, 420), OwlRouteNodeKind.Lobe),
            ("P02-SERVICE-01", new Vector2(1560, 720), OwlRouteNodeKind.Lobe),
            ("P02-SERVICE-02", new Vector2(1630, 980), OwlRouteNodeKind.Lobe),
            ("P02-FUEL-01", new Vector2(1510, 1210), OwlRouteNodeKind.Lobe),
            ("P02-FUEL-02", new Vector2(1550, 1400), OwlRouteNodeKind.Lobe),
            ("P02-TOOL-01", new Vector2(1290, 1560), OwlRouteNodeKind.Lobe),
            ("P02-SEARCH-01", new Vector2(900, 760), OwlRouteNodeKind.SearchProbe),
            ("P02-SEARCH-02", new Vector2(1390, 1080), OwlRouteNodeKind.SearchProbe),
        };

        [MenuItem("Prototype/Approved Facility/Rebuild Manual Shell Gameplay Integration")]
        public static void RebuildP02ManualShellGameplayIntegration()
        {
            var scene = RequireManualP02Scene();
            var plan = new ManualPlanSpace(RequireSceneRoot(scene, ManualFloorPlanName).transform);
            RemoveExactRoot(scene, P02GameplayRootName);

            var generatedRoot = new GameObject(P02GameplayRootName);
            SceneManager.MoveGameObjectToScene(generatedRoot, scene);

            var anchors = BuildManualSemanticAnchors(generatedRoot.transform, plan);
            var patrolPoints = BuildManualPatrolAnchors(generatedRoot.transform, plan);
            var map = BuildManualMapDefinition(generatedRoot.transform, anchors, patrolPoints);
            var navigation = BuildManualNavigation(generatedRoot.transform, plan, anchors, patrolPoints);

            var xrRigPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(XrRigPrefabPath);
            if (xrRigPrefab == null)
            {
                throw new InvalidOperationException($"Reusable XR rig prefab was not found at {XrRigPrefabPath}.");
            }

            var xrRig = PrefabUtility.InstantiatePrefab(xrRigPrefab, scene) as GameObject;
            if (xrRig == null)
            {
                throw new InvalidOperationException("Reusable XR rig prefab could not be instantiated.");
            }
            xrRig.name = "XR Player Rig (P02 Manual Shell)";
            xrRig.transform.SetParent(generatedRoot.transform, true);
            xrRig.tag = "Player";
            ConfigureControllerFirstRig(xrRig, map.PlayerSpawnPosition);

            var desktopRig = ResolveManualDesktopRig(scene, generatedRoot.transform, map.PlayerSpawnPosition);
            var systems = new GameObject("P02 Manual Shell Runtime Systems");
            systems.transform.SetParent(generatedRoot.transform, false);

            var xrView = xrRig.transform.Find("Camera Offset/Main Camera");
            if (xrView == null)
            {
                throw new InvalidOperationException("The XR rig Main Camera was not found.");
            }
            var xrInteractionOrigin = xrRig.transform.Find("Camera Offset/Right Controller") ?? xrView;
            var playerMode = systems.AddComponent<PlayerModeController>();
            playerMode.Configure(
                PlayerControlMode.Desktop,
                desktopRig.root,
                desktopRig.camera.transform,
                desktopRig.camera.transform,
                xrRig,
                xrView,
                xrInteractionOrigin,
                map);
            desktopRig.interactor.Configure(desktopRig.camera, desktopRig.holdPoint, playerMode);
            desktopRig.root.SetActive(true);
            xrRig.SetActive(false);

            var feedbackHub = systems.AddComponent<FeedbackHub>();
            var feedbackOutputs = new List<MonoBehaviour>();
            feedbackOutputs.Add(systems.AddComponent<DesktopEnvironmentalFeedbackOutput>());
            AddMissionAudioOutputs(systems.transform, feedbackOutputs);

            var runController = systems.AddComponent<PrototypeRunController>();
            runController.ConfigureTiming(PrototypeRunController.DefaultRunDurationSeconds, 20f);
            runController.ConfigureSeed(1337);
            var ambienceSource = systems.AddComponent<AudioSource>();
            ambienceSource.playOnAwake = false;
            var ambience = systems.AddComponent<FacilityAmbienceController>();
            ambience.Configure(runController, ambienceSource);

            var materials = CreateMaterials();
            var owl = BuildOwl(
                generatedRoot.transform,
                materials,
                playerMode,
                runController,
                map,
                feedbackHub,
                feedbackOutputs,
                CreateApprovedFacilityThreatConnections());
            ConfigureManualOwlHarness(owl);

            foreach (var hapticPlayer in xrRig.GetComponentsInChildren<HapticImpulsePlayer>(true))
            {
                var output = systems.AddComponent<XRHapticFeedbackOutput>();
                output.Configure(hapticPlayer);
                feedbackOutputs.Add(output);
            }
            AddExternalFeedbackStubs(systems.transform, feedbackOutputs);
            feedbackHub.Configure(feedbackOutputs.ToArray());

            // Contract locations A/B/C remain explicit, inactive placeholders. This
            // traversal harness starts the protected run loop without choosing a rod
            // distribution or an extraction exit on the user's behalf.
            var pendingContract = new GameObject("INACTIVE Contract Distribution Adapter");
            pendingContract.transform.SetParent(generatedRoot.transform, false);
            var objectiveSet = pendingContract.AddComponent<FuelRodObjectiveSet>();
            objectiveSet.Configure(Array.Empty<FuelRodItem>(), 3, runController);

            var geiger = systems.AddComponent<GeigerCounterController>();
            geiger.Configure(runController, playerMode, Array.Empty<FuelRodItem>(), owl, feedbackHub, false);
            var motionStimuli = systems.AddComponent<PlayerThreatStimulusEmitter>();
            var footstepSource = systems.AddComponent<AudioSource>();
            ConfigureSpatialSource(footstepSource, 0.8f, 22f);
            var footstepAudio = systems.AddComponent<ProceduralSpatialAudio>();
            footstepAudio.Configure(footstepSource);
            motionStimuli.Configure(playerMode, desktopRig.controller, owl, footstepAudio);

            var pause = systems.AddComponent<DesktopPauseController>();
            pause.Configure(playerMode);
            systems.AddComponent<DesktopDisplayController>();

            runController.Configure(objectiveSet, owl, geiger, motionStimuli, feedbackHub);
            var runInput = systems.AddComponent<PrototypeRunInput>();
            runInput.Configure(runController);

            // Population is independently disposable, but its distraction adapters
            // target the generated Owl. Rebind them whenever gameplay is regenerated
            // so either menu can be run safely without leaving stale scene references.
            var reboundPopulationAdapters = RebindP02PopulationThreatSink(scene, owl);

            MarkGeneratedContentDirty(generatedRoot);
            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene, ApprovedFacilityScenePath))
            {
                throw new InvalidOperationException($"Failed to save '{ApprovedFacilityScenePath}'.");
            }
            AssetDatabase.SaveAssets();

            var validation = ValidateP02ManualShellGameplay(scene, true);
            Selection.activeGameObject = systems;
            Debug.Log(
                "P02_MANUAL_GAMEPLAY_REBUILT|root=" + P02GameplayRootName +
                "|desktop=existing-or-owned|xr=inactive-ready|objectives=inactive|extractions=inactive|" +
                navigation + "|populationAdaptersRebound=" + reboundPopulationAdapters + "|" + validation);
        }

        [MenuItem("Prototype/Approved Facility/Validate Manual Shell Gameplay Integration")]
        public static void ValidateP02ManualShellGameplayIntegration()
        {
            var scene = RequireManualP02Scene();
            Debug.Log("P02_MANUAL_GAMEPLAY_VALID|" + ValidateP02ManualShellGameplay(scene, true));
        }

        private static Scene RequireManualP02Scene()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                throw new InvalidOperationException("Exit Play Mode before rebuilding P02 gameplay integration.");
            }
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid() || !scene.isLoaded ||
                !string.Equals(scene.path, ApprovedFacilityScenePath, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"The active scene must be '{ApprovedFacilityScenePath}'.");
            }
            RequireSceneRoot(scene, "Rooms");
            RequireSceneRoot(scene, "Hallways");
            return scene;
        }

        private static GameObject RequireSceneRoot(Scene scene, string name)
        {
            var matches = scene.GetRootGameObjects().Where(root => root.name == name).ToArray();
            if (matches.Length != 1)
            {
                throw new InvalidOperationException($"P02 requires exactly one root named '{name}', found {matches.Length}.");
            }
            return matches[0];
        }

        private static void RemoveExactRoot(Scene scene, string rootName)
        {
            foreach (var root in scene.GetRootGameObjects().Where(root => root.name == rootName).ToArray())
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static ManualGameplayAnchors BuildManualSemanticAnchors(Transform parent, ManualPlanSpace plan)
        {
            var activeRoot = new GameObject("Active Gameplay Anchors");
            activeRoot.transform.SetParent(parent, false);
            var inactiveRoot = new GameObject("Semantic Anchors (Placeholders Explicitly Inactive)");
            inactiveRoot.transform.SetParent(parent, false);
            var result = new ManualGameplayAnchors();

            foreach (var spec in ApprovedFacilityLayout.SemanticAnchors)
            {
                var anchor = new GameObject(spec.Id);
                anchor.transform.SetParent(spec.GameplayActive ? activeRoot.transform : inactiveRoot.transform, false);
                anchor.transform.SetPositionAndRotation(
                    plan.ImageToWorld(spec.ImagePosition, 0.02f),
                    plan.ImageYawToRotation(spec.ImageYawDegrees));
                var metadata = anchor.AddComponent<FacilitySemanticAnchor>();
                metadata.Configure(spec.Id, spec.Kind, spec.GameplayActive, 1.35f);
                if (spec.Id == "ANCHOR-PLAYER-SPAWN") result.PlayerSpawn = anchor.transform;
                if (spec.Id == "ANCHOR-OWL-SPAWN") result.OwlSpawn = anchor.transform;
                if (spec.Id == "ANCHOR-EXTRACTION-A") result.ExtractionReference = anchor.transform;
                if (!spec.GameplayActive) anchor.SetActive(false);
            }

            if (result.PlayerSpawn == null || result.OwlSpawn == null || result.ExtractionReference == null)
            {
                throw new InvalidOperationException("P02 manual gameplay anchors are incomplete.");
            }
            return result;
        }

        private static Transform[] BuildManualPatrolAnchors(Transform parent, ManualPlanSpace plan)
        {
            var root = new GameObject("P02 Manual Owl Route Nodes");
            root.transform.SetParent(parent, false);
            var result = new Transform[ManualPatrolSpecs.Length];
            for (var index = 0; index < ManualPatrolSpecs.Length; index++)
            {
                var spec = ManualPatrolSpecs[index];
                var node = new GameObject(spec.id);
                node.transform.SetParent(root.transform, false);
                node.transform.position = plan.ImageToWorld(spec.imagePoint, 0.02f);
                var metadata = node.AddComponent<OwlRouteNodeAnchor>();
                metadata.Configure(spec.id, spec.kind, FacilityRouteFamily.None);
                result[index] = node.transform;
            }
            return result;
        }

        private static PrototypeMapDefinition BuildManualMapDefinition(
            Transform parent,
            ManualGameplayAnchors anchors,
            Transform[] patrolPoints)
        {
            var mapObject = new GameObject("P02 Manual Shell Map Definition");
            mapObject.transform.SetParent(parent, false);
            var map = mapObject.AddComponent<PrototypeMapDefinition>();
            map.Configure(
                anchors.PlayerSpawn,
                anchors.ExtractionReference,
                anchors.ExtractionReference,
                anchors.OwlSpawn,
                Array.Empty<RodSocketAnchor>(),
                patrolPoints,
                Array.Empty<RouteVariationAnchor>(),
                Array.Empty<LightingVariantAnchor>(),
                Array.Empty<FacilityZoneTrigger>());
            return map;
        }

        private static string BuildManualNavigation(
            Transform parent,
            ManualPlanSpace plan,
            ManualGameplayAnchors anchors,
            Transform[] patrolPoints)
        {
            var navigationRoot = new GameObject("P02 Manual Shell Navigation");
            navigationRoot.transform.SetParent(parent, false);
            var sources = new GameObject("Disposable Bake Sources (Disabled After Bake)");
            sources.transform.SetParent(navigationRoot.transform, false);

            foreach (var room in ApprovedFacilityLayout.Rooms)
            {
                if (room.Id == "ROOM-CONTAINMENT") continue;
                CreateManualNavPolygon(room.Id, sources.transform, room.ImagePoints, plan);
            }
            foreach (var area in ManualContainmentFloorAreas)
            {
                CreateManualNavPolygon(area.Id, sources.transform, area.ImagePoints, plan);
            }
            foreach (var portal in ApprovedFacilityLayout.Portals)
            {
                CreateManualPortalBridge(portal, sources.transform, plan);
            }

            var surface = navigationRoot.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.Children;
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surface.layerMask = ~0;
            surface.ignoreNavMeshAgent = true;
            surface.ignoreNavMeshObstacle = true;
            surface.BuildNavMesh();

            var snapped = 0;
            var failedToSnap = 0;
            var snapTargets = new List<Transform> { anchors.PlayerSpawn, anchors.OwlSpawn };
            snapTargets.AddRange(patrolPoints);
            foreach (var target in snapTargets)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(target.position, out hit, 12f, NavMesh.AllAreas))
                {
                    target.position = hit.position + Vector3.up * 0.02f;
                    snapped++;
                }
                else
                {
                    failedToSnap++;
                }
            }

            foreach (var collider in sources.GetComponentsInChildren<Collider>(true))
            {
                collider.enabled = false;
            }

            var triangulation = NavMesh.CalculateTriangulation();
            if (surface.navMeshData == null || triangulation.vertices.Length == 0 || failedToSnap > 0)
            {
                throw new InvalidOperationException(
                    $"P02 NavMesh bake failed: data={(surface.navMeshData != null)} vertices={triangulation.vertices.Length} failedToSnap={failedToSnap}.");
            }

            return $"navVertices={triangulation.vertices.Length}|navTriangles={triangulation.indices.Length / 3}" +
                   $"|snappedAnchors={snapped}|failedToSnap={failedToSnap}";
        }

        private static void CreateManualNavPolygon(
            string id,
            Transform parent,
            Vector2[] imagePoints,
            ManualPlanSpace plan)
        {
            var gameObject = new GameObject(id + " NavMesh Floor", typeof(MeshFilter), typeof(MeshCollider));
            gameObject.transform.SetParent(parent, false);
            var vertices = imagePoints.Select(point => plan.ImageToWorld(point)).ToArray();
            var flat = vertices.Select(vertex => new Vector2(vertex.x, vertex.z)).ToArray();
            var triangles = TriangulateManualPolygon(flat);
            var mesh = new Mesh { name = id + " Manual NavMesh" };
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            if (mesh.normals.Length > 0 && mesh.normals[0].y < 0f)
            {
                for (var index = 0; index < triangles.Length; index += 3)
                {
                    var swap = triangles[index + 1];
                    triangles[index + 1] = triangles[index + 2];
                    triangles[index + 2] = swap;
                }
                mesh.triangles = triangles;
                mesh.RecalculateNormals();
            }
            mesh.RecalculateBounds();
            gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;
            gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
        }

        private static void CreateManualPortalBridge(
            FacilityPortalSpec portal,
            Transform parent,
            ManualPlanSpace plan)
        {
            var bridge = new GameObject(portal.Id + " NavMesh Bridge", typeof(BoxCollider));
            bridge.transform.SetParent(parent, false);
            var thickness = 0.12f;
            bridge.transform.SetPositionAndRotation(
                plan.ImageToWorld(portal.ImageCenter, -thickness * 0.5f),
                plan.ImageYawToRotation(portal.ImageYawDegrees));
            // The drawing leaves a longer approved connector between decon and the
            // vehicle airlock than the standard doorway throat. Keep the adjustment
            // navigation-only and limited to that declared portal.
            var bridgeDepth = portal.Id == "PORTAL-DECON-EXTRACTION-A" ? 8f : 4f;
            bridge.transform.localScale = new Vector3(
                Mathf.Max(portal.SourceGapPixels * plan.MetersPerPixel, 2f),
                thickness,
                bridgeDepth * plan.ApprovedToManualScale);
        }

        private static int[] TriangulateManualPolygon(Vector2[] points)
        {
            if (points == null || points.Length < 3) return Array.Empty<int>();
            var indices = Enumerable.Range(0, points.Length).ToList();
            if (ManualSignedArea(points) < 0f) indices.Reverse();
            var triangles = new List<int>((points.Length - 2) * 3);
            var guard = points.Length * points.Length;
            while (indices.Count > 2 && guard-- > 0)
            {
                var clipped = false;
                for (var index = 0; index < indices.Count; index++)
                {
                    var previous = indices[(index - 1 + indices.Count) % indices.Count];
                    var current = indices[index];
                    var next = indices[(index + 1) % indices.Count];
                    if (ManualCross(points[current] - points[previous], points[next] - points[current]) <= 0.00001f)
                    {
                        continue;
                    }
                    var contains = false;
                    for (var testIndex = 0; testIndex < indices.Count; testIndex++)
                    {
                        var test = indices[testIndex];
                        if (test == previous || test == current || test == next) continue;
                        if (ManualPointInTriangle(points[test], points[previous], points[current], points[next]))
                        {
                            contains = true;
                            break;
                        }
                    }
                    if (contains) continue;
                    triangles.Add(previous);
                    triangles.Add(current);
                    triangles.Add(next);
                    indices.RemoveAt(index);
                    clipped = true;
                    break;
                }
                if (!clipped) break;
            }
            if (triangles.Count != (points.Length - 2) * 3)
            {
                throw new InvalidOperationException("Could not triangulate a P02 navigation polygon.");
            }
            return triangles.ToArray();
        }

        private static float ManualSignedArea(Vector2[] points)
        {
            var area = 0f;
            for (var index = 0; index < points.Length; index++)
            {
                var next = (index + 1) % points.Length;
                area += points[index].x * points[next].y - points[next].x * points[index].y;
            }
            return area * 0.5f;
        }

        private static float ManualCross(Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }

        private static bool ManualPointInTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
        {
            var ab = ManualCross(b - a, point - a);
            var bc = ManualCross(c - b, point - b);
            var ca = ManualCross(a - c, point - c);
            return ab >= -0.0001f && bc >= -0.0001f && ca >= -0.0001f;
        }

        private static DesktopRig ResolveManualDesktopRig(Scene scene, Transform generatedParent, Vector3 spawn)
        {
            var existing = scene.GetRootGameObjects().SingleOrDefault(root => root.name == ManualDesktopPlayerName);
            if (existing == null)
            {
                return BuildDesktopRig(generatedParent, spawn);
            }

            var camera = existing.GetComponentInChildren<Camera>(true);
            var controller = existing.GetComponent<DesktopFirstPersonController>();
            var interactor = existing.GetComponent<DesktopGrabInteractor>();
            var holdPoint = camera != null ? camera.transform.Find("Desktop Hold Point") : null;
            if (camera == null || controller == null || interactor == null || holdPoint == null)
            {
                throw new InvalidOperationException(
                    $"Existing '{ManualDesktopPlayerName}' is missing its camera, controller, grab interactor, or hold point.");
            }
            existing.tag = "Player";
            controller.Configure(camera);
            return new DesktopRig
            {
                root = existing,
                camera = camera,
                holdPoint = holdPoint,
                interactor = interactor,
                controller = controller,
            };
        }

        private static void ConfigureManualOwlHarness(PursuitThreatController owl)
        {
            var serialized = new SerializedObject(owl);
            var transparentLayer = LayerMask.NameToLayer("PerceptionTransparent");
            if (transparentLayer >= 0)
            {
                serialized.FindProperty("sightBlockingLayers").intValue = ~(1 << transparentLayer);
            }
            serialized.FindProperty("automaticActivationSeconds").floatValue = 1f;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static string ValidateP02ManualShellGameplay(Scene scene, bool throwOnFailure)
        {
            var report = new ManualGameplayValidationReport();
            var roots = scene.GetRootGameObjects().Where(root => root.name == P02GameplayRootName).ToArray();
            report.RootCount = roots.Length;
            if (roots.Length == 1)
            {
                var root = roots[0];
                report.PlayerModeCount = root.GetComponentsInChildren<PlayerModeController>(true).Length;
                report.OwlCount = root.GetComponentsInChildren<PursuitThreatController>(true).Length;
                report.SurfaceCount = root.GetComponentsInChildren<NavMeshSurface>(true).Length;
                report.MapCount = root.GetComponentsInChildren<PrototypeMapDefinition>(true).Length;
                report.MissingScripts = root.GetComponentsInChildren<Transform>(true)
                    .SelectMany(transform => transform.GetComponents<Component>())
                    .Count(component => component == null);
                var map = root.GetComponentInChildren<PrototypeMapDefinition>(true);
                report.PatrolCount = map != null ? map.OwlPatrolPoints.Count : 0;
                report.RodSocketCount = map != null ? map.RodSockets.Count : -1;
                report.InactiveContractAnchors = root.GetComponentsInChildren<FacilitySemanticAnchor>(true)
                    .Count(anchor =>
                        (anchor.Kind == FacilitySemanticKind.ObjectivePlaceholder ||
                         anchor.Kind == FacilitySemanticKind.ExtractionPlaceholder) &&
                        !anchor.GameplayActive && !anchor.gameObject.activeSelf);
                report.EnabledBakeSourceColliders = root.GetComponentsInChildren<Collider>(true)
                    .Count(collider => collider.transform.IsChildOf(
                        root.transform.Find("P02 Manual Shell Navigation/Disposable Bake Sources (Disabled After Bake)")) &&
                        collider.enabled);
                var surface = root.GetComponentInChildren<NavMeshSurface>(true);
                report.HasNavMeshData = surface != null && surface.navMeshData != null;
                report.NavVertices = NavMesh.CalculateTriangulation().vertices.Length;
            }

            var deprecated = scene.GetRootGameObjects().SingleOrDefault(root => root.name == "Deprecated");
            report.DeprecatedRootRemainsInactive = deprecated == null || !deprecated.activeSelf;
            report.SceneIsClean = !scene.isDirty;
            if (throwOnFailure && !report.IsValid)
            {
                throw new InvalidOperationException("P02 manual gameplay validation failed: " + report);
            }
            return report.ToString();
        }

        private sealed class ManualGameplayAnchors
        {
            public Transform PlayerSpawn;
            public Transform OwlSpawn;
            public Transform ExtractionReference;
        }

        private sealed class ManualPlanSpace
        {
            private readonly Transform reference;
            private readonly Vector3 imageUpWorld;

            public ManualPlanSpace(Transform referenceTransform)
            {
                reference = referenceTransform;
                var imageUp = Vector3.ProjectOnPlane(reference.TransformDirection(Vector3.up), Vector3.up);
                imageUpWorld = imageUp.sqrMagnitude > 0.0001f ? imageUp.normalized : Vector3.forward;
                MetersPerPixel =
                    (Mathf.Abs(reference.lossyScale.x) + Mathf.Abs(reference.lossyScale.y)) * 0.5f /
                    ApprovedFacilityLayout.SourceWidthPixels;
                ApprovedToManualScale = MetersPerPixel / ApprovedFacilityLayout.PixelsToMeters;
            }

            public float MetersPerPixel { get; }
            public float ApprovedToManualScale { get; }

            public Vector3 ImageToWorld(Vector2 imagePoint, float yOffset = 0f)
            {
                var local = new Vector3(
                    imagePoint.x / ApprovedFacilityLayout.SourceWidthPixels - 0.5f,
                    0.5f - imagePoint.y / ApprovedFacilityLayout.SourceHeightPixels,
                    0f);
                var world = reference.TransformPoint(local);
                world.y = reference.position.y + yOffset;
                return world;
            }

            public Quaternion ImageYawToRotation(float imageYawDegrees)
            {
                return Quaternion.AngleAxis(-imageYawDegrees, Vector3.up) * Quaternion.LookRotation(imageUpWorld);
            }
        }

        private sealed class ManualGameplayValidationReport
        {
            public int RootCount;
            public int PlayerModeCount;
            public int OwlCount;
            public int SurfaceCount;
            public int MapCount;
            public int MissingScripts;
            public int PatrolCount;
            public int RodSocketCount;
            public int InactiveContractAnchors;
            public int EnabledBakeSourceColliders;
            public bool HasNavMeshData;
            public int NavVertices;
            public bool DeprecatedRootRemainsInactive;
            public bool SceneIsClean;

            public bool IsValid =>
                RootCount == 1 && PlayerModeCount == 1 && OwlCount == 1 && SurfaceCount == 1 &&
                MapCount == 1 && MissingScripts == 0 && PatrolCount == ManualPatrolSpecs.Length &&
                RodSocketCount == 0 && InactiveContractAnchors == 6 && EnabledBakeSourceColliders == 0 &&
                HasNavMeshData && NavVertices > 0 && DeprecatedRootRemainsInactive && SceneIsClean;

            public override string ToString()
            {
                return $"rootCount={RootCount}|playerMode={PlayerModeCount}|owl={OwlCount}|surface={SurfaceCount}" +
                       $"|map={MapCount}|missingScripts={MissingScripts}|patrol={PatrolCount}|rodSockets={RodSocketCount}" +
                       $"|inactiveContractAnchors={InactiveContractAnchors}|enabledBakeColliders={EnabledBakeSourceColliders}" +
                       $"|hasNavMeshData={HasNavMeshData}|navVertices={NavVertices}" +
                       $"|deprecatedInactive={DeprecatedRootRemainsInactive}|sceneClean={SceneIsClean}";
            }
        }
    }
}
