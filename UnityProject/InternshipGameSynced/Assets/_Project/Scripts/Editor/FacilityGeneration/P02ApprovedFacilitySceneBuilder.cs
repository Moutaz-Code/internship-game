using System;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace InternshipGame.Prototype.Editor
{
    public static partial class PrototypeSceneBuilder
    {
        internal const string ApprovedFacilityScenePath = "Assets/_Project/Scenes/P02_ApprovedFacilityShell.unity";
        internal const string ApprovedFacilityGeneratedRootName = "_Generated P02 Approved Facility";

        [MenuItem("Prototype/Approved Facility/Rebuild P02 Traversal Shell")]
        public static void BuildApprovedFacilityTraversalShell()
        {
            EditorSceneManager.SaveOpenScenes();
            var xrRigPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(XrRigPrefabPath);
            if (xrRigPrefab == null)
            {
                Debug.LogError($"Reusable XR rig prefab was not found at {XrRigPrefabPath}.");
                return;
            }

            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(ApprovedFacilityScenePath);
            var scene = sceneAsset != null
                ? EditorSceneManager.OpenScene(ApprovedFacilityScenePath, OpenSceneMode.Single)
                : EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            RemoveOwnedApprovedFacilityRoot(scene);
            var generatedRoot = new GameObject(ApprovedFacilityGeneratedRootName);
            SceneManager.MoveGameObjectToScene(generatedRoot, scene);

            var materials = CreateMaterials();
            BuildBaseLighting(generatedRoot.transform);
            var shell = ApprovedFacilityBuilder.Build(generatedRoot.transform);
            var map = BuildApprovedFacilityMapDefinition(shell);
            BuildApprovedFacilityNavigationSurface(shell.Root);
            var missionDisplay = BuildMissionDisplay(shell.MissionDisplayAnchor, materials);
            missionDisplay.text = "P02 APPROVED FACILITY SHELL\nCONTRACT MARKERS A / B / C INACTIVE PENDING APPROVAL";

            var xrRig = PrefabUtility.InstantiatePrefab(xrRigPrefab, scene) as GameObject;
            if (xrRig == null)
            {
                throw new InvalidOperationException("Reusable XR rig prefab could not be instantiated.");
            }

            xrRig.name = "XR Player Rig (Simulator Ready)";
            xrRig.transform.SetParent(generatedRoot.transform, true);
            xrRig.tag = "Player";
            ConfigureControllerFirstRig(xrRig, map.PlayerSpawnPosition);

            var desktopRig = BuildDesktopRig(generatedRoot.transform, map.PlayerSpawnPosition);
            var systems = new GameObject("P02 Traversal Runtime Systems");
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
            var desktopEnvironment = systems.AddComponent<DesktopEnvironmentalFeedbackOutput>();
            feedbackOutputs.Add(desktopEnvironment);
            AddMissionAudioOutputs(systems.transform, feedbackOutputs);

            var runController = systems.AddComponent<PrototypeRunController>();
            runController.ConfigureTiming(PrototypeRunController.DefaultRunDurationSeconds, 20f);
            runController.ConfigureSeed(1337);
            var ambienceSource = systems.AddComponent<AudioSource>();
            ambienceSource.playOnAwake = false;
            var ambience = systems.AddComponent<FacilityAmbienceController>();
            ambience.Configure(runController, ambienceSource);

            var owl = BuildOwl(
                generatedRoot.transform,
                materials,
                playerMode,
                runController,
                map,
                feedbackHub,
                feedbackOutputs,
                CreateApprovedFacilityThreatConnections());

            // Glass blocks bodies, hands, and carried objects, but does not become
            // opaque Owl cover. P01 retains its all-layers default.
            var transparentLayer = LayerMask.NameToLayer("PerceptionTransparent");
            if (transparentLayer >= 0)
            {
                var owlProperties = new SerializedObject(owl);
                owlProperties.FindProperty("sightBlockingLayers").intValue = ~(1 << transparentLayer);
                // The P02 traversal harness has no approved rods to wake the Owl;
                // activate it promptly for navigation/stealth inspection only.
                owlProperties.FindProperty("automaticActivationSeconds").floatValue = 1f;
                owlProperties.ApplyModifiedPropertiesWithoutUndo();
            }

            foreach (var hapticPlayer in xrRig.GetComponentsInChildren<HapticImpulsePlayer>(true))
            {
                var output = systems.AddComponent<XRHapticFeedbackOutput>();
                output.Configure(hapticPlayer);
                feedbackOutputs.Add(output);
            }

            AddExternalFeedbackStubs(systems.transform, feedbackOutputs);
            feedbackHub.Configure(feedbackOutputs.ToArray());

            // This empty objective set deliberately starts the protected run/Owl loop
            // without selecting or relocating any objective or extraction marker.
            var pendingContract = new GameObject("INACTIVE Contract Distribution Adapter");
            pendingContract.transform.SetParent(generatedRoot.transform, false);
            var objectiveSet = pendingContract.AddComponent<FuelRodObjectiveSet>();
            objectiveSet.Configure(Array.Empty<FuelRodItem>(), 3, runController);

            var geiger = systems.AddComponent<GeigerCounterController>();
            geiger.Configure(runController, playerMode, Array.Empty<FuelRodItem>(), owl, feedbackHub, false);
            var motionStimuli = systems.AddComponent<PlayerThreatStimulusEmitter>();
            var footstepSource = desktopRig.root.AddComponent<AudioSource>();
            ConfigureSpatialSource(footstepSource, 0.8f, 15f);
            var footstepAudio = desktopRig.root.AddComponent<ProceduralSpatialAudio>();
            footstepAudio.Configure(footstepSource);
            motionStimuli.Configure(playerMode, desktopRig.controller, owl, footstepAudio);

            var throwableProps = BuildApprovedFacilityThrowableProps(generatedRoot.transform, materials.metal, owl);
            var machineryPanels = BuildApprovedFacilityMachineryPanels(
                generatedRoot.transform,
                materials.panel,
                materials.amber,
                owl);
            BuildTraversalClearanceRod(generatedRoot.transform, materials.rodCasing, owl);

            var pause = systems.AddComponent<DesktopPauseController>();
            pause.Configure(playerMode);
            systems.AddComponent<DesktopDisplayController>();

            var runStateListeners = new List<MonoBehaviour>
            {
                owl,
                geiger,
                motionStimuli,
                feedbackHub,
            };
            runStateListeners.AddRange(throwableProps);
            runStateListeners.AddRange(machineryPanels);
            runController.Configure(objectiveSet, runStateListeners.ToArray());

            var runInput = systems.AddComponent<PrototypeRunInput>();
            runInput.Configure(runController);

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogColor = new Color(0.04f, 0.045f, 0.048f);
            RenderSettings.fogDensity = 0.0055f;
            RenderSettings.ambientIntensity = 0.78f;

            MarkGeneratedContentDirty(generatedRoot);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ApprovedFacilityScenePath);
            AssetDatabase.SaveAssets();

            Selection.activeGameObject = systems;
            Debug.Log(
                "P02_APPROVED_SHELL_REBUILT|rooms=" + ApprovedFacilityLayout.Rooms.Length +
                "|portals=" + ApprovedFacilityLayout.Portals.Length +
                "|blocked=" + ApprovedFacilityLayout.BlockedZones.Length +
                "|owlNodes=" + map.OwlPatrolPoints.Count +
                "|objectiveMarkers=inactive|extractionMarkers=inactive|buildSettings=unchanged");
        }

        private static PrototypeMapDefinition BuildApprovedFacilityMapDefinition(ApprovedFacilityShellResult shell)
        {
            var mapObject = new GameObject("P02 Approved Facility Map Definition");
            mapObject.transform.SetParent(shell.Root.transform, false);
            var map = mapObject.AddComponent<PrototypeMapDefinition>();
            var anchorRoot = shell.Root.transform.Find("Semantic Anchors (Placeholders Explicitly Inactive)");
            var extractionReference = anchorRoot != null ? anchorRoot.Find("ANCHOR-EXTRACTION-A") : null;
            if (extractionReference == null)
            {
                throw new InvalidOperationException("P02 inactive Extraction A reference is missing.");
            }

            map.Configure(
                shell.PlayerSpawn,
                extractionReference,
                extractionReference,
                shell.OwlSpawn,
                Array.Empty<RodSocketAnchor>(),
                shell.OwlPatrolPoints,
                Array.Empty<RouteVariationAnchor>(),
                Array.Empty<LightingVariantAnchor>(),
                Array.Empty<FacilityZoneTrigger>());
            return map;
        }

        private static void BuildApprovedFacilityNavigationSurface(GameObject facilityRoot)
        {
            var notWalkableArea = NavMesh.GetAreaFromName("Not Walkable");
            foreach (var collider in facilityRoot.GetComponentsInChildren<Collider>(true))
            {
                if (collider == null || collider.isTrigger || collider.GetComponent<NavMeshObstacle>() != null)
                {
                    continue;
                }

                var isFloor = collider.GetComponent<TMPro.TextMeshPro>() == null &&
                              collider.transform.parent != null &&
                              collider.transform.parent.name == "Generated Floor Polygons";
                if (isFloor || collider.gameObject.name == "FacilityKit Floor Module Example In Actual Map")
                {
                    continue;
                }

                var modifier = collider.GetComponent<NavMeshModifier>() ??
                               collider.gameObject.AddComponent<NavMeshModifier>();
                modifier.overrideArea = true;
                modifier.area = notWalkableArea;
            }

            var surface = facilityRoot.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.Children;
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surface.layerMask = ~0;
            surface.ignoreNavMeshAgent = true;
            surface.ignoreNavMeshObstacle = true;
            surface.BuildNavMesh();
            foreach (var link in facilityRoot.GetComponentsInChildren<NavMeshLink>(true))
            {
                var center = link.transform.position;
                var forward = link.transform.forward;
                if (NavMesh.SamplePosition(center - forward * 2.2f, out var start, 1.4f, NavMesh.AllAreas))
                {
                    link.startPoint = link.transform.InverseTransformPoint(start.position);
                }
                if (NavMesh.SamplePosition(center + forward * 2.2f, out var end, 1.4f, NavMesh.AllAreas))
                {
                    link.endPoint = link.transform.InverseTransformPoint(end.position);
                }
                link.UpdateLink();
            }
        }

        private static ThreatWaypointConnection[] CreateApprovedFacilityThreatConnections()
        {
            return new[]
            {
                new ThreatWaypointConnection(0, 1),
                new ThreatWaypointConnection(0, 15),
                new ThreatWaypointConnection(1, 2),
                new ThreatWaypointConnection(2, 3),
                new ThreatWaypointConnection(2, 15),
                new ThreatWaypointConnection(3, 4),
                new ThreatWaypointConnection(4, 5),
                new ThreatWaypointConnection(4, 12),
                new ThreatWaypointConnection(5, 6),
                new ThreatWaypointConnection(5, 8),
                new ThreatWaypointConnection(6, 7),
                new ThreatWaypointConnection(8, 9),
                new ThreatWaypointConnection(9, 10),
                new ThreatWaypointConnection(10, 11),
                new ThreatWaypointConnection(11, 12),
                new ThreatWaypointConnection(12, 13),
                new ThreatWaypointConnection(13, 14),
                new ThreatWaypointConnection(14, 15),
                new ThreatWaypointConnection(16, 3),
                new ThreatWaypointConnection(16, 4),
                new ThreatWaypointConnection(17, 4),
                new ThreatWaypointConnection(17, 12),
            };
        }

        private static ThrowableThreatStimulus[] BuildApprovedFacilityThrowableProps(
            Transform parent,
            Material material,
            PursuitThreatController owl)
        {
            var positions = new[]
            {
                ApprovedFacilityLayout.ImageToWorld(new Vector2(1130, 1770), 0.45f),
                ApprovedFacilityLayout.ImageToWorld(new Vector2(850, 1580), 0.45f),
                ApprovedFacilityLayout.ImageToWorld(new Vector2(1000, 1130), 0.45f),
                ApprovedFacilityLayout.ImageToWorld(new Vector2(1260, 920), 0.45f),
                ApprovedFacilityLayout.ImageToWorld(new Vector2(1460, 410), 0.45f),
                ApprovedFacilityLayout.ImageToWorld(new Vector2(1590, 760), 0.45f),
                ApprovedFacilityLayout.ImageToWorld(new Vector2(1480, 1280), 0.45f),
                ApprovedFacilityLayout.ImageToWorld(new Vector2(1290, 1540), 0.45f),
            };
            var root = new GameObject("P02 Portable Metal Distractions");
            root.transform.SetParent(parent, false);
            var result = new ThrowableThreatStimulus[positions.Length];
            for (var i = 0; i < positions.Length; i++)
            {
                var prop = GameObject.CreatePrimitive(i % 2 == 0 ? PrimitiveType.Cylinder : PrimitiveType.Cube);
                prop.name = $"P02 Metal Distraction {i + 1}";
                prop.transform.SetParent(root.transform);
                prop.transform.position = positions[i];
                prop.transform.localScale = i % 2 == 0
                    ? new Vector3(0.24f, 0.35f, 0.24f)
                    : new Vector3(0.4f, 0.4f, 0.4f);
                prop.GetComponent<Renderer>().sharedMaterial = material;
                var body = prop.AddComponent<Rigidbody>();
                body.mass = 1.4f;
                body.linearDamping = 0.1f;
                body.angularDamping = 0.22f;
                body.interpolation = RigidbodyInterpolation.Interpolate;
                body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                prop.AddComponent<XRGrabInteractable>();
                var grabbable = prop.AddComponent<GrabbableObject>();
                grabbable.ConfigureDesktopGripPose(new Vector3(0.25f, -0.08f, 0f), Vector3.zero);
                var source = prop.AddComponent<AudioSource>();
                ConfigureSpatialSource(source, 0.6f, 22f);
                var impactAudio = prop.AddComponent<ProceduralSpatialAudio>();
                impactAudio.Configure(source);
                var stimulus = prop.AddComponent<ThrowableThreatStimulus>();
                stimulus.Configure(owl, impactAudio);
                result[i] = stimulus;
            }
            return result;
        }

        private static MachineryDistractionPanel[] BuildApprovedFacilityMachineryPanels(
            Transform parent,
            Material readyMaterial,
            Material activeMaterial,
            PursuitThreatController owl)
        {
            var placements = new[]
            {
                ("Containment", ApprovedFacilityLayout.ImageToWorld(new Vector2(1190, 900), 1.15f), Quaternion.Euler(0f, 90f, 0f)),
                ("Coolant", ApprovedFacilityLayout.ImageToWorld(new Vector2(1430, 440), 1.15f), Quaternion.Euler(0f, -90f, 0f)),
                ("Fuel Handling", ApprovedFacilityLayout.ImageToWorld(new Vector2(1500, 1290), 1.15f), Quaternion.Euler(0f, -90f, 0f)),
            };
            var root = new GameObject("P02 Fixed Machinery Distractions");
            root.transform.SetParent(parent, false);
            var result = new MachineryDistractionPanel[placements.Length];
            for (var i = 0; i < placements.Length; i++)
            {
                var placement = placements[i];
                var panelRoot = new GameObject($"{placement.Item1} Maintenance Test Panel");
                panelRoot.transform.SetParent(root.transform, false);
                panelRoot.transform.SetPositionAndRotation(placement.Item2, placement.Item3);
                CreatePrimitiveLocal("Panel Housing", PrimitiveType.Cube, panelRoot.transform, Vector3.zero,
                    new Vector3(0.95f, 1.25f, 0.18f), readyMaterial, false);
                var indicator = CreatePrimitiveLocal("Local Amber Active Indicator", PrimitiveType.Cube,
                    panelRoot.transform, new Vector3(0f, 0.38f, -0.11f), new Vector3(0.58f, 0.14f, 0.035f),
                    readyMaterial, false).GetComponent<Renderer>();
                CreateFittedTextLocal("Maintenance Test Label", panelRoot.transform,
                    new Vector3(0f, -0.08f, -0.115f), Quaternion.identity, new Vector2(0.78f, 0.48f),
                    "MAINTENANCE\nTEST", 0.32f, Color.white, true);
                var source = panelRoot.AddComponent<AudioSource>();
                ConfigureSpatialSource(source, 0.8f, MachineryDistractionPanel.DefaultRadius);
                var panelAudio = panelRoot.AddComponent<ProceduralSpatialAudio>();
                panelAudio.Configure(source);
                var panel = panelRoot.AddComponent<MachineryDistractionPanel>();
                panel.Configure(owl, indicator, readyMaterial, activeMaterial, panelAudio);
                result[i] = panel;
            }
            return result;
        }

        private static void BuildTraversalClearanceRod(
            Transform parent,
            Material material,
            PursuitThreatController owl)
        {
            var rod = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            rod.name = "Traversal Clearance Rod (Not An Objective)";
            rod.transform.SetParent(parent, false);
            rod.transform.position = ApprovedFacilityLayout.ImageToWorld(new Vector2(1160, 1760), 0.62f);
            rod.transform.localScale = new Vector3(0.17f, 0.56f, 0.17f);
            rod.GetComponent<Renderer>().sharedMaterial = material;
            var body = rod.AddComponent<Rigidbody>();
            body.mass = 2.2f;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rod.AddComponent<XRGrabInteractable>();
            var grabbable = rod.AddComponent<GrabbableObject>();
            grabbable.ConfigureDesktopGripPose(new Vector3(0.28f, -0.12f, 0f), Vector3.zero);
            var source = rod.AddComponent<AudioSource>();
            ConfigureSpatialSource(source, 0.6f, 22f);
            var impactAudio = rod.AddComponent<ProceduralSpatialAudio>();
            impactAudio.Configure(source);
            var stimulus = rod.AddComponent<ThrowableThreatStimulus>();
            stimulus.Configure(owl, impactAudio);
        }

        private static void RemoveOwnedApprovedFacilityRoot(Scene scene)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == ApprovedFacilityGeneratedRootName)
                {
                    UnityEngine.Object.DestroyImmediate(root);
                }
            }
        }
    }
}
