using System;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace InternshipGame.Prototype.Editor
{
    public static class PrototypeSceneBuilder
    {
        private const string XrRigPrefabPath = "Assets/_Project/Prefabs/Player/XR_PlayerRig.prefab";
        private const string PrototypeScenePath = "Assets/_Project/Scenes/P01_CoreLoop.unity";
        private const string MaterialFolder = "Assets/_Project/Materials";
        private const string UrpLitShader = "Universal Render Pipeline/Lit";
        private const string GeneratedRootName = "_Generated Polished Prototype";

        private static readonly string[] LegacyGeneratedRootNames =
        {
            "XR Player Rig",
            "Desktop Player",
            "Prototype Systems",
            "Greybox Facility",
            "Lighting",
        };

        [MenuItem("Prototype/Rebuild Polished Multi-Path Prototype")]
        public static void Build()
        {
            EditorSceneManager.SaveOpenScenes();
            var xrRigPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(XrRigPrefabPath);
            if (xrRigPrefab == null)
            {
                Debug.LogError($"Reusable XR rig prefab was not found at {XrRigPrefabPath}.");
                return;
            }

            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(PrototypeScenePath);
            var scene = sceneAsset != null
                ? EditorSceneManager.OpenScene(PrototypeScenePath, OpenSceneMode.Single)
                : EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            RemovePreviousGeneratedContent(scene);
            var generatedRoot = new GameObject(GeneratedRootName);
            SceneManager.MoveGameObjectToScene(generatedRoot, scene);

            var materials = CreateMaterials();
            BuildBaseLighting(generatedRoot.transform);
            var facility = ExpandedFacilityBuilder.Build(
                generatedRoot.transform,
                new ExpandedFacilityMaterials
                {
                    BaseFloor = materials.floor,
                    Wall = materials.wall,
                    Metal = materials.metal,
                    DarkTrim = materials.darkTrim,
                    Panel = materials.panel,
                    Cyan = materials.cyan,
                    Amber = materials.amber,
                    RadiationGreen = materials.radiation,
                    DangerRed = materials.danger,
                    NeutralLight = materials.neutralLight,
                });
            BuildNavigationSurface(facility.Root);
            var missionDisplay = BuildMissionDisplay(facility.MissionDisplayAnchor, materials);

            var xrRig = PrefabUtility.InstantiatePrefab(xrRigPrefab, scene) as GameObject;
            if (xrRig == null)
            {
                throw new InvalidOperationException("Reusable XR rig prefab could not be instantiated.");
            }

            xrRig.name = "XR Player Rig (Simulator Ready)";
            xrRig.transform.SetParent(generatedRoot.transform, true);
            xrRig.tag = "Player";
            ConfigureControllerFirstRig(xrRig, facility.MapDefinition.PlayerSpawnPosition);

            var desktopRig = BuildDesktopRig(generatedRoot.transform, facility.MapDefinition.PlayerSpawnPosition);
            var systems = new GameObject("Prototype Runtime Systems");
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
                facility.MapDefinition);
            desktopRig.interactor.Configure(desktopRig.camera, desktopRig.holdPoint, playerMode);
            desktopRig.root.SetActive(true);
            xrRig.SetActive(false);

            var feedbackHub = systems.AddComponent<FeedbackHub>();
            var feedbackOutputs = new List<MonoBehaviour>();
            var desktopEnvironment = systems.AddComponent<DesktopEnvironmentalFeedbackOutput>();
            feedbackOutputs.Add(desktopEnvironment);
            AddMissionAudioOutputs(systems.transform, feedbackOutputs);
            AddExtractionEmergencyLights(
                generatedRoot.transform,
                facility.MapDefinition.ExtractionRackAnchor.position,
                materials.danger,
                feedbackOutputs);

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
                facility.MapDefinition,
                feedbackHub,
                feedbackOutputs);

            foreach (var hapticPlayer in xrRig.GetComponentsInChildren<HapticImpulsePlayer>(true))
            {
                var output = systems.AddComponent<XRHapticFeedbackOutput>();
                output.Configure(hapticPlayer);
                feedbackOutputs.Add(output);
            }

            AddExternalFeedbackStubs(systems.transform, feedbackOutputs);
            feedbackHub.Configure(feedbackOutputs.ToArray());

            var objectiveRoot = new GameObject("Fuel Rod Objective");
            objectiveRoot.transform.SetParent(generatedRoot.transform, false);
            var rods = BuildFuelRods(objectiveRoot.transform, facility.MapDefinition, materials);
            var objectiveSet = objectiveRoot.AddComponent<FuelRodObjectiveSet>();
            objectiveSet.Configure(rods, 3, runController);

            var rack = BuildDepositRack(
                generatedRoot.transform,
                facility.MapDefinition.ExtractionRackAnchor,
                materials,
                runController,
                objectiveSet,
                feedbackHub,
                owl);
            var exit = BuildExit(
                generatedRoot.transform,
                facility.MapDefinition.ExtractionExitAnchor,
                materials,
                runController);

            var coordinator = systems.AddComponent<FuelRodRunCoordinator>();
            coordinator.Configure(
                runController,
                objectiveSet,
                rods,
                facility.MapDefinition,
                feedbackHub,
                owl);

            var geiger = systems.AddComponent<GeigerCounterController>();
            // Baseline comprehension is the design-document gate; F2 enables the
            // bounded Owl interference profile for advanced/repeated-run testing.
            geiger.Configure(runController, playerMode, rods, owl, feedbackHub, false);

            var motionStimuli = systems.AddComponent<PlayerThreatStimulusEmitter>();
            var footstepSource = desktopRig.root.AddComponent<AudioSource>();
            ConfigureSpatialSource(footstepSource, 0.8f, 15f);
            var footstepAudio = desktopRig.root.AddComponent<ProceduralSpatialAudio>();
            footstepAudio.Configure(footstepSource);
            motionStimuli.Configure(playerMode, desktopRig.controller, owl, footstepAudio);
            var throwableProps = BuildThrowableProps(generatedRoot.transform, materials.metal, owl);
            var machineryPanels = BuildMachineryPanels(
                generatedRoot.transform,
                materials.panel,
                materials.amber,
                owl);

            var pause = systems.AddComponent<DesktopPauseController>();
            pause.Configure(playerMode);
            var display = systems.AddComponent<DesktopDisplayController>();

            var heartRateStub = systems.AddComponent<BluetoothHeartRateSensorStub>();
            heartRateStub.Configure(false);
            var biometricHub = systems.AddComponent<BiometricHub>();
            biometricHub.Configure(heartRateStub);

            var runStateListeners = new List<MonoBehaviour>
            {
                coordinator,
                rack,
                exit,
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

            var hud = systems.AddComponent<PrototypeHudPresenter>();
            var statusProviders = new List<MonoBehaviour>
            {
                playerMode,
                facility.MapDefinition,
                coordinator,
                owl,
                rack,
                exit,
                geiger,
                desktopEnvironment,
                feedbackHub,
                biometricHub,
                pause,
                display,
                ambience,
                motionStimuli,
            };
            statusProviders.AddRange(machineryPanels);
            hud.Configure(runController, missionDisplay, playerMode, statusProviders.ToArray());
            hud.ConfigureObjectiveItems(rods);

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogColor = new Color(0.018f, 0.026f, 0.042f);
            RenderSettings.fogDensity = 0.0095f;
            RenderSettings.ambientIntensity = 0.68f;

            MarkGeneratedContentDirty(generatedRoot);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, PrototypeScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(PrototypeScenePath, true) };
            AssetDatabase.SaveAssets();

            Selection.activeGameObject = systems;
            Debug.Log(
                "Polished multi-path prototype built: 10 spaces, 8 sockets, 5 active rods, " +
                "3 route profiles, 9 metal distractions, 3 machinery panels, " +
                "NavMesh/graph-fallback Owl navigation, and extraction holdout.");
        }

        [MenuItem("Prototype/Rebuild First Prototype (Destructive)")]
        private static void LegacyMenuAlias()
        {
            Build();
        }

        [MenuItem("Prototype/Build Current Version Windows (Strict)")]
        public static void BuildCurrentVersionWindowsStrict()
        {
            var version = PlayerSettings.bundleVersion;
            if (!System.Version.TryParse(version, out var parsedVersion) ||
                parsedVersion.Major < 0 ||
                parsedVersion.Minor < 0 ||
                parsedVersion.Build < 0 ||
                parsedVersion.Revision < 0)
            {
                throw new InvalidOperationException(
                    $"PlayerSettings.bundleVersion must use four numeric parts (x.y.z.w); current value is '{version}'.");
            }

            var versionLabel = $"v{version}";
            var outputDirectory = System.IO.Path.Combine("Builds", versionLabel);
            var outputPath = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(outputDirectory, $"InternshipGame_{versionLabel}.exe"));
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(outputPath));

            try
            {
                var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
                {
                    scenes = new[] { PrototypeScenePath },
                    locationPathName = outputPath,
                    target = BuildTarget.StandaloneWindows64,
                    options = BuildOptions.Development | BuildOptions.StrictMode | BuildOptions.CleanBuildCache,
                });
                var summary = report.summary;
                Debug.Log(
                    $"PROTOTYPE_BUILD_SUMMARY|version={versionLabel}|result={summary.result}|errors={summary.totalErrors}|" +
                    $"warnings={summary.totalWarnings}|bytes={summary.totalSize}|" +
                    $"seconds={summary.totalTime.TotalSeconds:0.00}|path={outputPath}");
            }
            catch (Exception exception)
            {
                Debug.LogError($"PROTOTYPE_BUILD_SUMMARY|version=v{version}|exception={exception}");
                throw;
            }
        }

        private static FacilityMaterials CreateMaterials()
        {
            return new FacilityMaterials
            {
                floor = CreateMaterial("M_Floor", new Color(0.055f, 0.07f, 0.095f)),
                wall = CreateMaterial("M_Wall", new Color(0.105f, 0.115f, 0.14f)),
                metal = CreateMaterial("M_Metal", new Color(0.22f, 0.25f, 0.29f)),
                darkTrim = CreateMaterial("M_DarkTrim", new Color(0.026f, 0.032f, 0.045f)),
                panel = CreateMaterial("M_DisplayPanel", new Color(0.055f, 0.075f, 0.095f)),
                rodCasing = CreateMaterial("M_RodCasing", new Color(0.31f, 0.34f, 0.38f), true),
                cyan = CreateMaterial("M_Route_Cyan", new Color(0.05f, 0.78f, 1f), true),
                amber = CreateMaterial("M_Route_Amber", new Color(1f, 0.46f, 0.05f), true),
                radiation = CreateMaterial("M_Radiation_Green", new Color(0.22f, 1f, 0.34f), true),
                danger = CreateMaterial("M_Danger_Red", new Color(1f, 0.045f, 0.025f), true),
                neutralLight = CreateMaterial("M_Route_Neutral", new Color(0.46f, 0.6f, 0.75f), true),
                owl = CreateMaterial("M_Owl", new Color(0.008f, 0.01f, 0.016f)),
                owlEye = CreateMaterial("M_OwlEye", new Color(1f, 0.035f, 0.012f), true),
            };
        }

        private static void BuildBaseLighting(Transform parent)
        {
            var root = new GameObject("Base Lighting");
            root.transform.SetParent(parent, false);
            var lightObject = new GameObject("Directional Safety Fill");
            lightObject.transform.SetParent(root.transform, false);
            lightObject.transform.rotation = Quaternion.Euler(58f, -32f, 0f);
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(0.36f, 0.43f, 0.55f);
            light.intensity = 0.42f;
            light.shadows = LightShadows.Soft;
        }

        private static TMP_Text BuildMissionDisplay(Transform anchor, FacilityMaterials materials)
        {
            // Keep all children under a scale-one assembly. Parenting the TMP object to
            // the old scaled backing was the source of the tiny, stretched wall text.
            var root = new GameObject("Mission Display Assembly");
            root.transform.SetParent(anchor.parent, false);
            root.transform.SetPositionAndRotation(anchor.position, anchor.rotation);

            CreatePrimitiveLocal(
                "Recessed Mission Display Backing",
                PrimitiveType.Cube,
                root.transform,
                Vector3.zero,
                new Vector3(4.45f, 1.45f, 0.12f),
                materials.panel,
                false);
            CreatePrimitiveLocal("Mission Display Top Frame", PrimitiveType.Cube, root.transform, new Vector3(0f, 0.755f, 0f), new Vector3(4.6f, 0.08f, 0.16f), materials.darkTrim, false);
            CreatePrimitiveLocal("Mission Display Bottom Frame", PrimitiveType.Cube, root.transform, new Vector3(0f, -0.755f, 0f), new Vector3(4.6f, 0.08f, 0.16f), materials.darkTrim, false);
            CreatePrimitiveLocal("Mission Display Left Frame", PrimitiveType.Cube, root.transform, new Vector3(-2.26f, 0f, 0f), new Vector3(0.08f, 1.45f, 0.16f), materials.darkTrim, false);
            CreatePrimitiveLocal("Mission Display Right Frame", PrimitiveType.Cube, root.transform, new Vector3(2.26f, 0f, 0f), new Vector3(0.08f, 1.45f, 0.16f), materials.darkTrim, false);
            CreatePrimitiveLocal("Mission Display Cyan Status Rail", PrimitiveType.Cube, root.transform, new Vector3(0f, -0.62f, -0.075f), new Vector3(4.12f, 0.075f, 0.035f), materials.cyan, false);
            CreatePrimitiveLocal("Mission Display Left Ceiling Mount", PrimitiveType.Cube, root.transform, new Vector3(-1.55f, 0.86f, 0.04f), new Vector3(0.07f, 0.28f, 0.07f), materials.darkTrim, false);
            CreatePrimitiveLocal("Mission Display Right Ceiling Mount", PrimitiveType.Cube, root.transform, new Vector3(1.55f, 0.86f, 0.04f), new Vector3(0.07f, 0.28f, 0.07f), materials.darkTrim, false);

            var displayObject = new GameObject("Diegetic Mission Display", typeof(RectTransform), typeof(TextMeshPro));
            displayObject.transform.SetParent(root.transform, false);
            displayObject.transform.localPosition = new Vector3(0f, 0.05f, -0.075f);
            displayObject.transform.localRotation = Quaternion.identity;
            displayObject.transform.localScale = Vector3.one;
            var rect = (RectTransform)displayObject.transform;
            rect.sizeDelta = new Vector2(4.05f, 1.14f);
            var display = displayObject.GetComponent<TextMeshPro>();
            display.text = "LIVE RODS SECURED  0 / 3\nFIND  |  HOLD 3s TO SCAN  |  RETURN LIVE";
            display.fontSize = 1.05f;
            display.enableAutoSizing = true;
            display.fontSizeMin = 0.58f;
            display.fontSizeMax = 1.05f;
            display.fontStyle = FontStyles.Bold;
            display.alignment = TextAlignmentOptions.Center;
            display.color = new Color(0.4f, 0.95f, 1f);
            display.textWrappingMode = TextWrappingModes.Normal;
            display.overflowMode = TextOverflowModes.Overflow;
            display.margin = new Vector4(0.08f, 0.04f, 0.08f, 0.04f);
            return display;
        }

        private static TMP_Text CreateFittedTextLocal(
            string name,
            Transform parent,
            Vector3 localPosition,
            Quaternion localRotation,
            Vector2 size,
            string text,
            float fontSize,
            Color color,
            bool allowWrapping)
        {
            var textObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshPro));
            textObject.transform.SetParent(parent, false);
            textObject.transform.localPosition = localPosition;
            textObject.transform.localRotation = localRotation;
            textObject.transform.localScale = Vector3.one;
            var rect = (RectTransform)textObject.transform;
            rect.sizeDelta = size;
            var label = textObject.GetComponent<TextMeshPro>();
            label.text = text;
            label.fontSize = fontSize;
            label.enableAutoSizing = true;
            label.fontSizeMin = Mathf.Max(0.28f, fontSize * 0.62f);
            label.fontSizeMax = fontSize;
            label.fontStyle = FontStyles.Bold;
            label.alignment = TextAlignmentOptions.Center;
            label.color = color;
            label.textWrappingMode = allowWrapping ? TextWrappingModes.Normal : TextWrappingModes.NoWrap;
            label.overflowMode = allowWrapping ? TextOverflowModes.Overflow : TextOverflowModes.Truncate;
            label.margin = new Vector4(0.04f, 0.02f, 0.04f, 0.02f);
            return label;
        }

        private static FuelRodItem[] BuildFuelRods(
            Transform parent,
            PrototypeMapDefinition map,
            FacilityMaterials materials)
        {
            var rods = new FuelRodItem[5];
            for (var i = 0; i < rods.Length; i++)
            {
                var socket = map.RodSockets[i];
                var root = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                root.name = $"Fuel Rod {i + 1}";
                root.transform.SetParent(parent);
                root.transform.SetPositionAndRotation(socket.SpawnPoint.position, socket.SpawnPoint.rotation);
                root.transform.localScale = new Vector3(0.17f, 0.56f, 0.17f);
                root.isStatic = false;
                var casingRenderer = root.GetComponent<Renderer>();
                casingRenderer.sharedMaterial = materials.rodCasing;

                var capRenderers = new List<Renderer> { casingRenderer };
                foreach (var y in new[] { -1.02f, 1.02f })
                {
                    var cap = CreatePrimitiveLocal(
                        y < 0f ? "Lower Cap" : "Upper Cap",
                        PrimitiveType.Cylinder,
                        root.transform,
                        new Vector3(0f, y, 0f),
                        new Vector3(1.35f, 0.08f, 1.35f),
                        materials.metal,
                        false);
                    capRenderers.Add(cap.GetComponent<Renderer>());
                }

                var body = root.AddComponent<Rigidbody>();
                body.mass = 2.2f;
                body.linearDamping = 0.08f;
                body.angularDamping = 0.18f;
                body.interpolation = RigidbodyInterpolation.Interpolate;
                body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                root.AddComponent<XRGrabInteractable>();
                var grabbable = root.AddComponent<GrabbableObject>();
                grabbable.ConfigureDesktopGripPose(new Vector3(0.28f, -0.03f, 0f), Vector3.zero);
                var item = root.AddComponent<FuelRodItem>();
                item.ConfigureIdentity($"fuel-rod-{i + 1}", i < 3 ? FuelRodClassification.Live : FuelRodClassification.Inert);
                item.ConfigureAssayDuration(3f);

                var lightObject = new GameObject("Assay Response Light");
                lightObject.transform.SetParent(root.transform, false);
                var light = lightObject.AddComponent<Light>();
                light.type = LightType.Point;
                light.range = 2.8f;
                light.enabled = false;
                var visual = root.AddComponent<FuelRodVisualFeedback>();
                visual.Configure(item, capRenderers.ToArray(), light);
                rods[i] = item;
            }

            return rods;
        }

        private static FuelRodDepositRack BuildDepositRack(
            Transform parent,
            Transform anchor,
            FacilityMaterials materials,
            PrototypeRunController runController,
            FuelRodObjectiveSet objectiveSet,
            FeedbackHub feedbackHub,
            PursuitThreatController owl)
        {
            var root = new GameObject("Three-Slot Deposit Rack");
            root.transform.SetParent(parent, false);
            root.transform.position = anchor.position;

            CreatePrimitiveLocal("Rack Base", PrimitiveType.Cube, root.transform, new Vector3(0f, 0.28f, 0f), new Vector3(3.25f, 0.55f, 1f), materials.metal, true);
            CreatePrimitiveLocal("Rack Front Plinth", PrimitiveType.Cube, root.transform, new Vector3(0f, 0.3f, -0.53f), new Vector3(3.38f, 0.62f, 0.08f), materials.darkTrim, false);
            CreatePrimitiveLocal("Rack Instruction Backing", PrimitiveType.Cube, root.transform, new Vector3(0f, 1.55f, 0.62f), new Vector3(3.35f, 0.7f, 0.1f), materials.panel, false);
            CreatePrimitiveLocal("Rack Sign Left Mount", PrimitiveType.Cube, root.transform, new Vector3(-1.48f, 1.05f, 0.62f), new Vector3(0.08f, 0.95f, 0.08f), materials.darkTrim, false);
            CreatePrimitiveLocal("Rack Sign Right Mount", PrimitiveType.Cube, root.transform, new Vector3(1.48f, 1.05f, 0.62f), new Vector3(0.08f, 0.95f, 0.08f), materials.darkTrim, false);
            CreatePrimitiveLocal("Rack Cyan Status Rail", PrimitiveType.Cube, root.transform, new Vector3(0f, 1.25f, 0.555f), new Vector3(3.02f, 0.055f, 0.025f), materials.cyan, false);
            CreateFittedTextLocal(
                "Rack Instructions",
                root.transform,
                new Vector3(0f, 1.59f, 0.555f),
                Quaternion.identity,
                new Vector2(3.02f, 0.49f),
                "LIVE ROD INTAKE\nSCAN-CONFIRMED RODS ONLY",
                0.78f,
                Color.white,
                true);
            var slots = new Transform[3];
            for (var i = 0; i < slots.Length; i++)
            {
                var x = (i - 1) * 0.9f;
                CreatePrimitiveLocal($"Slot {i + 1} Ring", PrimitiveType.Cylinder, root.transform, new Vector3(x, 0.62f, 0f), new Vector3(0.38f, 0.1f, 0.38f), materials.cyan, true);
                CreateFittedTextLocal(
                    $"Slot {i + 1} Number",
                    root.transform,
                    new Vector3(x, 0.3f, -0.575f),
                    Quaternion.identity,
                    new Vector2(0.58f, 0.32f),
                    $"{i + 1:00}",
                    0.65f,
                    materials.cyan.color,
                    false);
                slots[i] = new GameObject($"Deposit Slot {i + 1}").transform;
                slots[i].SetParent(root.transform, false);
                slots[i].localPosition = new Vector3(x, 1.2f, 0f);
            }

            var triggerObject = new GameObject("Rack Acceptance Field");
            triggerObject.transform.SetParent(root.transform, false);
            triggerObject.transform.localPosition = new Vector3(0f, 1.1f, 0.45f);
            var trigger = triggerObject.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(3.4f, 2.2f, 1.5f);
            var rack = triggerObject.AddComponent<FuelRodDepositRack>();
            rack.Configure(runController, objectiveSet, slots, feedbackHub, owl);
            return rack;
        }

        private static ExtractionExitTrigger BuildExit(
            Transform parent,
            Transform anchor,
            FacilityMaterials materials,
            PrototypeRunController runController)
        {
            var root = new GameObject("Extraction Exit");
            root.transform.SetParent(parent, false);
            root.transform.position = anchor.position;
            var closedDoor = CreatePrimitiveLocal(
                "Cycling Blast Door",
                PrimitiveType.Cube,
                root.transform,
                new Vector3(0f, 1.3f, 0f),
                new Vector3(4.1f, 2.6f, 0.28f),
                materials.danger,
                true);

            var triggerObject = new GameObject("Final Exit Crossing");
            triggerObject.transform.SetParent(root.transform, false);
            // Place the success trigger beyond the blast door so opening the exit does
            // not auto-complete a player waiting safely inside the bay.
            triggerObject.transform.localPosition = new Vector3(0f, 1.2f, -0.75f);
            var trigger = triggerObject.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(4.2f, 2.4f, 0.5f);
            var exit = triggerObject.AddComponent<ExtractionExitTrigger>();
            exit.Configure(runController, closedDoor);
            return exit;
        }

        private static PursuitThreatController BuildOwl(
            Transform parent,
            FacilityMaterials materials,
            PlayerModeController playerMode,
            PrototypeRunController runController,
            PrototypeMapDefinition map,
            FeedbackHub feedbackHub,
            List<MonoBehaviour> feedbackOutputs)
        {
            var threatObject = new GameObject("The Owl");
            threatObject.transform.SetParent(parent, false);
            threatObject.transform.SetPositionAndRotation(map.OwlSpawnAnchor.position, Quaternion.LookRotation(Vector3.back));

            var visualRoot = new GameObject("Owl Presentation");
            visualRoot.transform.SetParent(threatObject.transform, false);
            visualRoot.transform.localPosition = Vector3.up * 0.75f;
            CreatePrimitiveLocal("Body", PrimitiveType.Capsule, visualRoot.transform, new Vector3(0f, -0.18f, 0f), new Vector3(0.4f, 0.52f, 0.38f), materials.owl, false);
            CreatePrimitiveLocal("Head", PrimitiveType.Sphere, visualRoot.transform, new Vector3(0f, 0.42f, 0f), new Vector3(0.72f, 0.56f, 0.58f), materials.owl, false);
            var leftWing = CreatePrimitiveLocal("Left Wing", PrimitiveType.Cube, visualRoot.transform, new Vector3(-0.45f, -0.1f, 0f), new Vector3(0.2f, 0.82f, 0.42f), materials.owl, false);
            leftWing.transform.localRotation = Quaternion.Euler(0f, 0f, 22f);
            var rightWing = CreatePrimitiveLocal("Right Wing", PrimitiveType.Cube, visualRoot.transform, new Vector3(0.45f, -0.1f, 0f), new Vector3(0.2f, 0.82f, 0.42f), materials.owl, false);
            rightWing.transform.localRotation = Quaternion.Euler(0f, 0f, -22f);
            CreatePrimitiveLocal("Left Eye", PrimitiveType.Sphere, visualRoot.transform, new Vector3(-0.19f, 0.46f, 0.29f), Vector3.one * 0.13f, materials.owlEye, false);
            CreatePrimitiveLocal("Right Eye", PrimitiveType.Sphere, visualRoot.transform, new Vector3(0.19f, 0.46f, 0.29f), Vector3.one * 0.13f, materials.owlEye, false);

            var graphNavigator = threatObject.AddComponent<WaypointGraphThreatNavigator>();
            graphNavigator.Configure(ToTransformArray(map.OwlPatrolPoints), CreateThreatConnections());
            graphNavigator.enabled = false;

            var agent = threatObject.AddComponent<NavMeshAgent>();
            agent.radius = 0.42f;
            agent.height = 1.4f;
            agent.baseOffset = 0f;
            agent.speed = 1.45f;
            agent.angularSpeed = 360f;
            agent.acceleration = 8f;
            agent.stoppingDistance = 0.12f;
            agent.autoBraking = true;

            var navigator = threatObject.AddComponent<NavMeshThreatNavigator>();
            navigator.Configure(agent, graphNavigator);
            // NavMeshSurface registers its baked data during scene startup. The adapter
            // enables the agent lazily after it samples a valid point, avoiding a false
            // "no valid NavMesh" warning in standalone desktop builds.
            agent.enabled = false;

            var detectionLight = visualRoot.AddComponent<Light>();
            detectionLight.type = LightType.Point;
            detectionLight.color = materials.danger.color;
            detectionLight.intensity = 5f;
            detectionLight.range = 5f;
            detectionLight.enabled = false;
            var lightOutput = visualRoot.AddComponent<LightFeedbackOutput>();
            lightOutput.Configure(detectionLight, PrototypeFeedbackCues.OwlDetected);
            feedbackOutputs.Add(lightOutput);

            var audioSource = visualRoot.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
            audioSource.minDistance = 1f;
            audioSource.maxDistance = 22f;
            audioSource.volume = 0.78f;
            var audioOutput = visualRoot.AddComponent<AudioFeedbackOutput>();
            audioOutput.Configure(audioSource, PrototypeFeedbackCues.OwlWarning, GeneratedAudioProfile.OwlWarning);
            feedbackOutputs.Add(audioOutput);

            var investigationSource = visualRoot.AddComponent<AudioSource>();
            ConfigureSpatialSource(investigationSource, 1f, 22f);
            investigationSource.volume = 0.5f;
            var investigationOutput = visualRoot.AddComponent<AudioFeedbackOutput>();
            investigationOutput.Configure(
                investigationSource,
                PrototypeFeedbackCues.OwlInvestigation,
                GeneratedAudioProfile.OwlInvestigation);
            feedbackOutputs.Add(investigationOutput);

            var threat = threatObject.AddComponent<PursuitThreatController>();
            threat.Configure(playerMode, runController, null, feedbackHub, visualRoot);
            threat.ConfigureNavigation(navigator, map);
            visualRoot.SetActive(false);
            return threat;
        }

        private static void BuildNavigationSurface(GameObject facilityRoot)
        {
            var notWalkableArea = NavMesh.GetAreaFromName("Not Walkable");
            foreach (var collider in facilityRoot.GetComponentsInChildren<Collider>(true))
            {
                if (collider == null || collider.isTrigger ||
                    collider.gameObject.name == "Continuous Navigable Floor" ||
                    collider.GetComponent<NavMeshObstacle>() != null)
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
        }

        private static ThreatWaypointConnection[] CreateThreatConnections()
        {
            return new[]
            {
                new ThreatWaypointConnection(0, 1),
                new ThreatWaypointConnection(1, 2),
                new ThreatWaypointConnection(1, 7, PrototypeVariationIds.CentralPowerOutage),
                new ThreatWaypointConnection(1, 20),
                new ThreatWaypointConnection(20, 11, PrototypeVariationIds.LaboratoryQuarantine),
                new ThreatWaypointConnection(2, 3, PrototypeVariationIds.MaintenanceObstruction),
                new ThreatWaypointConnection(3, 4),
                new ThreatWaypointConnection(4, 21),
                new ThreatWaypointConnection(21, 22),
                new ThreatWaypointConnection(22, 5),
                new ThreatWaypointConnection(22, 6),
                new ThreatWaypointConnection(6, 8),
                new ThreatWaypointConnection(5, 14),
                new ThreatWaypointConnection(5, 17),
                new ThreatWaypointConnection(7, 8),
                new ThreatWaypointConnection(8, 9),
                new ThreatWaypointConnection(8, 10),
                new ThreatWaypointConnection(10, 12),
                new ThreatWaypointConnection(11, 12),
                new ThreatWaypointConnection(12, 13),
                new ThreatWaypointConnection(13, 16),
                new ThreatWaypointConnection(13, 19),
                new ThreatWaypointConnection(9, 15),
                new ThreatWaypointConnection(14, 15),
                new ThreatWaypointConnection(15, 16),
                new ThreatWaypointConnection(14, 17),
                new ThreatWaypointConnection(15, 18),
                new ThreatWaypointConnection(16, 19),
            };
        }

        private static Transform[] ToTransformArray(IReadOnlyList<Transform> source)
        {
            var result = new Transform[source.Count];
            for (var i = 0; i < source.Count; i++)
            {
                result[i] = source[i];
            }

            return result;
        }

        private static ThrowableThreatStimulus[] BuildThrowableProps(
            Transform parent,
            Material material,
            PursuitThreatController owl)
        {
            var root = new GameObject("Throwable Metal Distractions");
            root.transform.SetParent(parent, false);
            var positions = new[]
            {
                new Vector3(-12.8f, 0.45f, -3.8f),
                new Vector3(-9.2f, 0.45f, 2.2f),
                new Vector3(-12.2f, 0.45f, 6.6f),
                new Vector3(-1.6f, 0.45f, 5.5f),
                new Vector3(1.6f, 0.45f, -1.5f),
                new Vector3(1.4f, 0.45f, 7.2f),
                new Vector3(12.8f, 0.45f, -3.8f),
                new Vector3(9.2f, 0.45f, 2.2f),
                new Vector3(12.2f, 0.45f, 6.6f),
            };
            var result = new ThrowableThreatStimulus[positions.Length];

            for (var i = 0; i < positions.Length; i++)
            {
                var prop = GameObject.CreatePrimitive(i % 2 == 0 ? PrimitiveType.Cylinder : PrimitiveType.Cube);
                prop.name = $"Metal Distraction {i + 1}";
                prop.transform.SetParent(root.transform);
                prop.transform.position = positions[i];
                prop.transform.localScale = i % 2 == 0
                    ? new Vector3(0.24f, 0.35f, 0.24f)
                    : new Vector3(0.4f, 0.4f, 0.4f);
                prop.GetComponent<Renderer>().sharedMaterial = material;
                prop.isStatic = false;
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

        private static MachineryDistractionPanel[] BuildMachineryPanels(
            Transform parent,
            Material readyMaterial,
            Material activeMaterial,
            PursuitThreatController owl)
        {
            var root = new GameObject("Maintenance Test Panels");
            root.transform.SetParent(parent, false);
            var placements = new[]
            {
                ("West", new Vector3(-14.8f, 1.15f, -1.5f), Quaternion.Euler(0f, 90f, 0f)),
                ("Central", new Vector3(-2.4f, 1.15f, 2f), Quaternion.Euler(0f, 90f, 0f)),
                ("East", new Vector3(14.8f, 1.15f, -1.5f), Quaternion.Euler(0f, -90f, 0f)),
            };
            var result = new MachineryDistractionPanel[placements.Length];
            for (var i = 0; i < placements.Length; i++)
            {
                var placement = placements[i];
                var panelRoot = new GameObject($"{placement.Item1} Maintenance Test Panel");
                panelRoot.transform.SetParent(root.transform, false);
                panelRoot.transform.SetPositionAndRotation(placement.Item2, placement.Item3);
                CreatePrimitiveLocal(
                    "Panel Housing",
                    PrimitiveType.Cube,
                    panelRoot.transform,
                    Vector3.zero,
                    new Vector3(0.95f, 1.25f, 0.18f),
                    readyMaterial,
                    false);
                var indicator = CreatePrimitiveLocal(
                    "Local Amber Active Indicator",
                    PrimitiveType.Cube,
                    panelRoot.transform,
                    new Vector3(0f, 0.38f, -0.11f),
                    new Vector3(0.58f, 0.14f, 0.035f),
                    readyMaterial,
                    false).GetComponent<Renderer>();
                CreateFittedTextLocal(
                    "Maintenance Test Label",
                    panelRoot.transform,
                    new Vector3(0f, -0.08f, -0.115f),
                    Quaternion.identity,
                    new Vector2(0.78f, 0.48f),
                    "MAINTENANCE\nTEST",
                    0.32f,
                    Color.white,
                    true);
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

        private static void ConfigureSpatialSource(AudioSource source, float minimumDistance, float maximumDistance)
        {
            source.playOnAwake = false;
            source.spatialBlend = 1f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = minimumDistance;
            source.maxDistance = maximumDistance;
            source.dopplerLevel = 0f;
            source.volume = 1f;
        }

        private static void AddMissionAudioOutputs(Transform parent, List<MonoBehaviour> outputs)
        {
            AddAudioOutput(parent, outputs, PrototypeFeedbackCues.RodLiveConfirmed, GeneratedAudioProfile.RodLive);
            AddAudioOutput(parent, outputs, PrototypeFeedbackCues.RodInertConfirmed, GeneratedAudioProfile.RodInert);
            AddAudioOutput(parent, outputs, PrototypeFeedbackCues.RodAssayRequired, GeneratedAudioProfile.AssayRequired, 0.48f);
            AddAudioOutput(parent, outputs, PrototypeFeedbackCues.RodRejected, GeneratedAudioProfile.RodRejected);
            AddAudioOutput(parent, outputs, PrototypeFeedbackCues.RodDeposited, GeneratedAudioProfile.DepositAccepted);
            AddAudioOutput(parent, outputs, PrototypeFeedbackCues.GeigerClick, GeneratedAudioProfile.GeigerClick, 0.42f);
            AddAudioOutput(parent, outputs, PrototypeFeedbackCues.ExtractionCycling, GeneratedAudioProfile.ExtractionCycle, 0.5f);
            AddAudioOutput(parent, outputs, PrototypeFeedbackCues.ExtractionReady, GeneratedAudioProfile.ExtractionReady, 0.72f);
        }

        private static void AddAudioOutput(
            Transform parent,
            List<MonoBehaviour> outputs,
            string cue,
            GeneratedAudioProfile profile,
            float volume = 0.65f)
        {
            var root = new GameObject($"Audio {cue}");
            root.transform.SetParent(parent, false);
            var source = root.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.volume = volume;
            var output = root.AddComponent<AudioFeedbackOutput>();
            output.Configure(source, cue, profile);
            outputs.Add(output);
        }

        private static void AddExtractionEmergencyLights(
            Transform parent,
            Vector3 rackPosition,
            Material dangerMaterial,
            List<MonoBehaviour> outputs)
        {
            var root = new GameObject("Extraction Cycling Emergency Lights");
            root.transform.SetParent(parent, false);
            var offsets = new[]
            {
                new Vector3(-4.2f, 2.65f, 0.6f),
                new Vector3(4.2f, 2.65f, 0.6f),
                new Vector3(0f, 2.8f, -4.2f),
            };

            for (var i = 0; i < offsets.Length; i++)
            {
                var beacon = new GameObject($"Emergency Beacon {i + 1}");
                beacon.transform.SetParent(root.transform, false);
                beacon.transform.position = rackPosition + offsets[i];
                var light = beacon.AddComponent<Light>();
                light.type = LightType.Point;
                light.color = dangerMaterial.color;
                light.intensity = 4.2f;
                light.range = 8f;
                light.enabled = false;
                var output = beacon.AddComponent<LightFeedbackOutput>();
                output.Configure(light, PrototypeFeedbackCues.ExtractionCycling);
                outputs.Add(output);
            }
        }

        private static void AddExternalFeedbackStubs(Transform parent, List<MonoBehaviour> feedbackOutputs)
        {
            var optionalHardware = new GameObject("Optional Hardware Stubs");
            optionalHardware.transform.SetParent(parent, false);
            var floor = optionalHardware.AddComponent<ExternalFeedbackDeviceStub>();
            floor.Configure("floor-vibration-stub", FeedbackChannel.FloorVibration);
            feedbackOutputs.Add(floor);
            var heat = optionalHardware.AddComponent<ExternalFeedbackDeviceStub>();
            heat.Configure("heat-lamp-stub", FeedbackChannel.Heat);
            feedbackOutputs.Add(heat);
            var wind = optionalHardware.AddComponent<ExternalFeedbackDeviceStub>();
            wind.Configure("wind-blower-stub", FeedbackChannel.Wind);
            feedbackOutputs.Add(wind);
        }

        private static DesktopRig BuildDesktopRig(Transform parent, Vector3 spawnPosition)
        {
            var root = new GameObject("Desktop Player");
            root.transform.SetParent(parent, false);
            root.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
            root.tag = "Player";

            var characterController = root.AddComponent<CharacterController>();
            characterController.height = 1.75f;
            characterController.radius = 0.3f;
            characterController.center = new Vector3(0f, 0.875f, 0f);
            characterController.stepOffset = 0.3f;
            characterController.slopeLimit = 50f;
            characterController.minMoveDistance = 0f;

            var cameraObject = new GameObject("Desktop Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(root.transform, false);
            cameraObject.transform.localPosition = new Vector3(0f, 1.65f, 0f);
            var camera = cameraObject.GetComponent<Camera>();
            camera.nearClipPlane = 0.05f;
            camera.farClipPlane = 120f;
            camera.fieldOfView = 72f;

            var holdPoint = new GameObject("Desktop Hold Point").transform;
            holdPoint.SetParent(cameraObject.transform, false);
            holdPoint.localPosition = new Vector3(0f, -0.18f, 1.15f);

            var controller = root.AddComponent<DesktopFirstPersonController>();
            controller.Configure(camera);
            var interactor = root.AddComponent<DesktopGrabInteractor>();
            return new DesktopRig
            {
                root = root,
                camera = camera,
                holdPoint = holdPoint,
                interactor = interactor,
                controller = controller,
            };
        }

        private static void ConfigureControllerFirstRig(GameObject xrRig, Vector3 spawnPosition)
        {
            xrRig.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
            var modalityManager = xrRig.GetComponent<XRInputModalityManager>();
            if (modalityManager != null)
            {
                modalityManager.enabled = false;
            }

            SetChildActive(xrRig.transform, "Camera Offset/Gaze Interactor", false);
            SetChildActive(xrRig.transform, "Camera Offset/Gaze Stabilized", false);
            SetChildActive(xrRig.transform, "Camera Offset/Left Hand", false);
            SetChildActive(xrRig.transform, "Camera Offset/Right Hand", false);
            SetChildActive(xrRig.transform, "Camera Offset/Hand Visualizer", false);
            // Artificial jump remains comfort-gated for XR; desktop owns the current
            // prototype jump path until headset testing explicitly enables it.
            SetChildActive(xrRig.transform, "Locomotion/Jump", false);
            SetChildLocalPosition(xrRig.transform, "Camera Offset/Left Controller", new Vector3(-0.22f, -0.32f, 0.35f));
            SetChildLocalPosition(xrRig.transform, "Camera Offset/Right Controller", new Vector3(0.22f, -0.32f, 0.35f));
        }

        private static GameObject CreatePrimitive(
            string name,
            PrimitiveType type,
            Transform parent,
            Vector3 position,
            Vector3 scale,
            Material material,
            bool collider)
        {
            var gameObject = GameObject.CreatePrimitive(type);
            gameObject.name = name;
            gameObject.transform.SetParent(parent);
            gameObject.transform.SetPositionAndRotation(position, Quaternion.identity);
            gameObject.transform.localScale = scale;
            gameObject.GetComponent<Renderer>().sharedMaterial = material;
            gameObject.isStatic = collider;
            if (!collider)
            {
                UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<Collider>());
            }

            return gameObject;
        }

        private static GameObject CreatePrimitiveLocal(
            string name,
            PrimitiveType type,
            Transform parent,
            Vector3 localPosition,
            Vector3 localScale,
            Material material,
            bool collider)
        {
            var gameObject = CreatePrimitive(name, type, parent, Vector3.zero, localScale, material, collider);
            gameObject.transform.localPosition = localPosition;
            gameObject.transform.localRotation = Quaternion.identity;
            return gameObject;
        }

        private static Material CreateMaterial(string name, Color color, bool emission = false)
        {
            var path = $"{MaterialFolder}/{name}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                var shader = Shader.Find(UrpLitShader);
                if (shader == null)
                {
                    throw new InvalidOperationException($"Required shader was not found: {UrpLitShader}");
                }

                material = new Material(shader) { name = name };
                AssetDatabase.CreateAsset(material, path);
            }

            material.SetColor("_BaseColor", color);
            material.SetFloat("_Smoothness", emission ? 0.62f : 0.24f);
            if (emission)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * 2.15f);
            }
            else
            {
                material.DisableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", Color.black);
            }

            EditorUtility.SetDirty(material);
            return material;
        }

        private static void RemovePreviousGeneratedContent(Scene scene)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == GeneratedRootName || Array.IndexOf(LegacyGeneratedRootNames, root.name) >= 0)
                {
                    UnityEngine.Object.DestroyImmediate(root);
                }
            }
        }

        private static void MarkGeneratedContentDirty(GameObject root)
        {
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
            {
                EditorUtility.SetDirty(transform.gameObject);
                foreach (var component in transform.GetComponents<Component>())
                {
                    if (component != null)
                    {
                        EditorUtility.SetDirty(component);
                    }
                }
            }
        }

        private static void SetChildActive(Transform root, string path, bool active)
        {
            var child = root.Find(path);
            if (child != null)
            {
                child.gameObject.SetActive(active);
            }
        }

        private static void SetChildLocalPosition(Transform root, string path, Vector3 localPosition)
        {
            var child = root.Find(path);
            if (child != null)
            {
                child.localPosition = localPosition;
            }
        }

        private sealed class FacilityMaterials
        {
            public Material floor;
            public Material wall;
            public Material metal;
            public Material darkTrim;
            public Material panel;
            public Material rodCasing;
            public Material cyan;
            public Material amber;
            public Material radiation;
            public Material danger;
            public Material neutralLight;
            public Material owl;
            public Material owlEye;
        }

        private sealed class DesktopRig
        {
            public GameObject root;
            public Camera camera;
            public Transform holdPoint;
            public DesktopGrabInteractor interactor;
            public DesktopFirstPersonController controller;
        }
    }
}
