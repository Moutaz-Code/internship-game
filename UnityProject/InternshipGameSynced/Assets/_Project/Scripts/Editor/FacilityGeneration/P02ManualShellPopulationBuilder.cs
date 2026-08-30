using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace InternshipGame.Prototype.Editor
{
    /// <summary>
    /// Adds a reversible first-pass prop population to the user's hand-authored P02 shell.
    /// Only the exact generated population root is replaced. Manual walls, floors, ceilings,
    /// room groups, the floor-plan reference, and gameplay integration are never reparented or
    /// regenerated here.
    /// </summary>
    public static partial class PrototypeSceneBuilder
    {
        internal const string P02PopulationRootName = "_Generated P02 Room Population";

        private const string WarehouseRoot = "Assets/Imports/SciFi Warehouse Kit/Prefabs/";
        private const string FacilityKitRoot = "Assets/_Project/Prefabs/Environment/FacilityKit/";
        private const float ManualFloorHeight = 0.50f;

        private const string DeconFrame = FacilityKitRoot + "RoomIdentity/PF_FK_Decon_Frame.prefab";
        private const string ContractorIntake = FacilityKitRoot + "RoomIdentity/PF_FK_Contractor_Intake.prefab";
        private const string Lockers = FacilityKitRoot + "Machinery/PF_FK_Lockers.prefab";
        private const string AssayBench = FacilityKitRoot + "Machinery/PF_FK_Assay_Bench.prefab";
        private const string IsolationCabinet = FacilityKitRoot + "Machinery/PF_FK_Assay_IsolationCabinet.prefab";
        private const string ConsoleStraight = FacilityKitRoot + "Machinery/PF_FK_ControlConsole_Straight.prefab";
        private const string ConsoleCorner = FacilityKitRoot + "Machinery/PF_FK_ControlConsole_Corner.prefab";
        private const string ShieldStraight = FacilityKitRoot + "Cover/PF_FK_Shield_Straight.prefab";
        private const string ShieldCorner = FacilityKitRoot + "Cover/PF_FK_Shield_Corner.prefab";
        private const string StructuralColumn = FacilityKitRoot + "Architecture/PF_FK_Structural_Column.prefab";
        private const string CoolantBasin = FacilityKitRoot + "Machinery/PF_FK_Coolant_Basin.prefab";
        private const string PumpA = FacilityKitRoot + "Machinery/PF_FK_Pump_A_Horizontal.prefab";
        private const string PumpB = FacilityKitRoot + "Machinery/PF_FK_Pump_B_Twin.prefab";
        private const string PumpC = FacilityKitRoot + "Machinery/PF_FK_Pump_C_Vertical.prefab";
        private const string PipeA = FacilityKitRoot + "Machinery/PF_FK_PipeCluster_A.prefab";
        private const string PipeB = FacilityKitRoot + "Machinery/PF_FK_PipeCluster_B.prefab";
        private const string PipeC = FacilityKitRoot + "Machinery/PF_FK_PipeCluster_C.prefab";
        private const string Scrubber = FacilityKitRoot + "Machinery/PF_FK_Scrubber_Vessel.prefab";
        private const string DuctBank = FacilityKitRoot + "Machinery/PF_FK_Duct_Bank.prefab";
        private const string CaskRow = FacilityKitRoot + "Machinery/PF_FK_Cask_Row.prefab";
        private const string Cask = FacilityKitRoot + "Machinery/PF_FK_Cask_Individual.prefab";
        private const string Crane = FacilityKitRoot + "Machinery/PF_FK_Overhead_Crane.prefab";
        private const string ToolCage = FacilityKitRoot + "Machinery/PF_FK_Tool_Cage.prefab";
        private const string Workbench = FacilityKitRoot + "Machinery/PF_FK_Workbench.prefab";
        private const string PartsCart = FacilityKitRoot + "Machinery/PF_FK_Parts_Cart.prefab";
        private const string VehicleAirlock = FacilityKitRoot + "Extraction/PF_FK_Extraction_VehicleAirlock.prefab";
        private const string MaintenanceLift = FacilityKitRoot + "Extraction/PF_FK_Extraction_MaintenanceLift.prefab";
        private const string EmergencyEgress = FacilityKitRoot + "Extraction/PF_FK_Extraction_EmergencyEgress.prefab";

        private const string WarehouseShelves = WarehouseRoot + "Props/Shelves/Shelves Empty.prefab";
        private const string WarehouseCart = WarehouseRoot + "Props/Misc Props/Cart.prefab";
        private const string WarehouseBarrel = WarehouseRoot + "Props/Crates Barrels Pallets/Barrel.prefab";
        private const string WarehouseCrateLong = WarehouseRoot + "Props/Crates Barrels Pallets/Crate Long.prefab";
        private const string WarehouseCrateShort = WarehouseRoot + "Props/Crates Barrels Pallets/Crate Short.prefab";
        private const string WarehousePallet = WarehouseRoot + "Props/Crates Barrels Pallets/Pallet.prefab";

        private static readonly PopulationPlacement[] P02PopulationPlacements =
        {
            // Contractor Decontamination and Dispatch
            Placement("DECON-FRAME-01", "01 Contractor Decontamination and Dispatch", DeconFrame, 1110, 1765, 0, 1, true),
            Placement("DECON-FRAME-02", "01 Contractor Decontamination and Dispatch", DeconFrame, 1165, 1765, 0, 1, true),
            Placement("DECON-INTAKE-01", "01 Contractor Decontamination and Dispatch", ContractorIntake, 1035, 1810, 0, 1, true),
            Placement("DECON-LOCKERS-W", "01 Contractor Decontamination and Dispatch", Lockers, 1035, 1750, 90, 1, true),
            Placement("DECON-LOCKERS-E", "01 Contractor Decontamination and Dispatch", Lockers, 1240, 1750, -90, 1, true),
            Placement("DECON-CART", "01 Contractor Decontamination and Dispatch", WarehouseCart, 1260, 1815, -20, 1, false, portable: true),
            Placement("DECON-BARREL", "01 Contractor Decontamination and Dispatch", WarehouseBarrel, 1025, 1840, 0, 1, false, portable: true),
            Placement("DECON-CRATE", "01 Contractor Decontamination and Dispatch", WarehouseCrateLong, 1270, 1845, 90, 1, true),

            // Radiological Assay Laboratory
            Placement("ASSAY-BENCH-NW", "02 Radiological Assay Laboratory", AssayBench, 810, 1535, 0, 1, true),
            Placement("ASSAY-BENCH-NE", "02 Radiological Assay Laboratory", AssayBench, 845, 1535, 0, 1, true),
            Placement("ASSAY-BENCH-SW", "02 Radiological Assay Laboratory", AssayBench, 810, 1650, 180, 1, true),
            Placement("ASSAY-BENCH-SE", "02 Radiological Assay Laboratory", AssayBench, 845, 1650, 180, 1, true),
            Placement("ASSAY-ISOLATION-N", "02 Radiological Assay Laboratory", IsolationCabinet, 970, 1540, -90, 1, true),
            Placement("ASSAY-ISOLATION-S", "02 Radiological Assay Laboratory", IsolationCabinet, 970, 1650, -90, 1, true),
            Placement("ASSAY-SHELVES", "02 Radiological Assay Laboratory", WarehouseShelves, 760, 1600, 90, 0.65f, true),
            Placement("ASSAY-CART", "02 Radiological Assay Laboratory", WarehouseCart, 960, 1600, 10, 1, false, portable: true),

            // Reactor Control and Instrumentation Gallery
            Placement("CONTROL-CONSOLE-01", "03 Reactor Control and Instrumentation Gallery", ConsoleStraight, 960, 80, 0, 1, true),
            Placement("CONTROL-CONSOLE-02", "03 Reactor Control and Instrumentation Gallery", ConsoleStraight, 1030, 80, 0, 1, true),
            Placement("CONTROL-CONSOLE-03", "03 Reactor Control and Instrumentation Gallery", ConsoleStraight, 1100, 80, 0, 1, true),
            Placement("CONTROL-CONSOLE-04", "03 Reactor Control and Instrumentation Gallery", ConsoleStraight, 1170, 80, 0, 1, true),
            Placement("CONTROL-CONSOLE-CORNER", "03 Reactor Control and Instrumentation Gallery", ConsoleCorner, 1240, 105, 0, 1, true),
            Placement("CONTROL-CONSOLE-WEST", "03 Reactor Control and Instrumentation Gallery", ConsoleStraight, 820, 160, 90, 1, true),
            Placement("CONTROL-CART", "03 Reactor Control and Instrumentation Gallery", WarehouseCart, 840, 90, 15, 1, false, portable: true),
            Placement("CONTROL-COLUMN-W", "03 Reactor Control and Instrumentation Gallery", StructuralColumn, 900, 155, 0, 1, true),
            Placement("CONTROL-COLUMN-E", "03 Reactor Control and Instrumentation Gallery", StructuralColumn, 1260, 180, 0, 1, true),

            // Containment Shield Gallery
            Placement("CONTAINMENT-SHIELD-A", "04 Containment Shield Gallery", ShieldStraight, 1015, 725, -16, 1, true),
            Placement("CONTAINMENT-SHIELD-B", "04 Containment Shield Gallery", ShieldStraight, 1185, 830, 90, 1, true),
            Placement("CONTAINMENT-SHIELD-C", "04 Containment Shield Gallery", ShieldStraight, 1060, 1015, 10, 1, true),
            Placement("CONTAINMENT-SHIELD-D", "04 Containment Shield Gallery", ShieldCorner, 1285, 980, 0, 1, true),
            Placement("CONTAINMENT-SHIELD-E", "04 Containment Shield Gallery", ShieldStraight, 970, 900, 90, 1, true),
            Placement("CONTAINMENT-SHIELD-F", "04 Containment Shield Gallery", ShieldStraight, 1185, 1060, 0, 1, true),
            Placement("CONTAINMENT-PANEL", "04 Containment Shield Gallery", ConsoleStraight, 1190, 900, 90, 1, true, fixedDistraction: true),
            Placement("CONTAINMENT-BARREL", "04 Containment Shield Gallery", WarehouseBarrel, 1000, 1120, 0, 1, false, portable: true),
            Placement("CONTAINMENT-COLUMN-N", "04 Containment Shield Gallery", StructuralColumn, 1100, 600, 0, 1, true),
            Placement("CONTAINMENT-COLUMN-E", "04 Containment Shield Gallery", StructuralColumn, 1280, 740, 0, 1, true),

            // Primary Coolant Pump Hall
            Placement("COOLANT-BASIN", "05 Primary Coolant Pump Hall", CoolantBasin, 1500, 390, 0, 2, true),
            Placement("COOLANT-PUMP-A", "05 Primary Coolant Pump Hall", PumpA, 1380, 345, -12, 1.4f, true),
            Placement("COOLANT-PUMP-B", "05 Primary Coolant Pump Hall", PumpB, 1735, 370, 8, 1.4f, true),
            Placement("COOLANT-PUMP-C", "05 Primary Coolant Pump Hall", PumpC, 1390, 490, 12, 1.2f, true),
            Placement("COOLANT-PIPE-BANK-W", "05 Primary Coolant Pump Hall", PipeB, 1430, 310, 0, 1.25f, true),
            Placement("COOLANT-PIPE-BANK-E", "05 Primary Coolant Pump Hall", PipeC, 1690, 345, 180, 1.25f, true),
            Placement("COOLANT-PANEL", "05 Primary Coolant Pump Hall", ConsoleStraight, 1430, 440, -90, 1, true, fixedDistraction: true),
            Placement("COOLANT-BARREL", "05 Primary Coolant Pump Hall", WarehouseBarrel, 1620, 420, 0, 1, false, portable: true),
            Placement("COOLANT-PALLET", "05 Primary Coolant Pump Hall", WarehousePallet, 1750, 450, 12, 1, true),

            // Air Filtration and Scrubber Service
            Placement("SCRUBBER-VESSEL-NW", "06 Air Filtration and Scrubber Service", Scrubber, 1540, 730, 90, 1.2f, true),
            Placement("SCRUBBER-VESSEL-NE", "06 Air Filtration and Scrubber Service", Scrubber, 1650, 730, -90, 1.2f, true),
            Placement("SCRUBBER-VESSEL-SW", "06 Air Filtration and Scrubber Service", Scrubber, 1540, 950, 90, 1.2f, true),
            Placement("SCRUBBER-VESSEL-SE", "06 Air Filtration and Scrubber Service", Scrubber, 1650, 950, -90, 1.2f, true),
            Placement("SCRUBBER-DUCT-N", "06 Air Filtration and Scrubber Service", DuctBank, 1580, 800, 90, 1.2f, true),
            Placement("SCRUBBER-DUCT-S", "06 Air Filtration and Scrubber Service", DuctBank, 1630, 900, 0, 1.2f, true),
            Placement("SCRUBBER-PIPE", "06 Air Filtration and Scrubber Service", PipeA, 1550, 840, 0, 1.25f, true),
            Placement("SCRUBBER-CART", "06 Air Filtration and Scrubber Service", WarehouseCart, 1660, 1020, -15, 1, false, portable: true),

            // Spent Fuel Transfer and Waste Handling
            Placement("FUEL-CASK-ROW-01", "07 Spent Fuel Transfer and Waste Handling", CaskRow, 1530, 1250, 0, 1.2f, true),
            Placement("FUEL-CASK-ROW-02", "07 Spent Fuel Transfer and Waste Handling", CaskRow, 1600, 1260, 0, 1.2f, true),
            Placement("FUEL-CASK-ROW-03", "07 Spent Fuel Transfer and Waste Handling", CaskRow, 1500, 1330, 0, 1.2f, true),
            Placement("FUEL-CASK-ROW-04", "07 Spent Fuel Transfer and Waste Handling", CaskRow, 1600, 1360, 0, 1.2f, true),
            Placement("FUEL-CASK-W", "07 Spent Fuel Transfer and Waste Handling", Cask, 1460, 1280, 0, 1.2f, true),
            Placement("FUEL-CASK-E", "07 Spent Fuel Transfer and Waste Handling", Cask, 1660, 1320, 0, 1.2f, true),
            Placement("FUEL-CRANE", "07 Spent Fuel Transfer and Waste Handling", Crane, 1570, 1240, 0, 1.15f, true),
            Placement("FUEL-PANEL", "07 Spent Fuel Transfer and Waste Handling", ConsoleStraight, 1460, 1400, -90, 1, true, fixedDistraction: true),
            Placement("FUEL-PALLET", "07 Spent Fuel Transfer and Waste Handling", WarehousePallet, 1660, 1430, 20, 1, true),
            Placement("FUEL-BARREL", "07 Spent Fuel Transfer and Waste Handling", WarehouseBarrel, 1580, 1435, 0, 1, false, portable: true),

            // Tool Crib and Maintenance Stores
            Placement("TOOL-SHELVES-NW", "08 Tool Crib and Maintenance Stores", WarehouseShelves, 1240, 1540, 0, 0.65f, true),
            Placement("TOOL-SHELVES-NE", "08 Tool Crib and Maintenance Stores", WarehouseShelves, 1310, 1510, 0, 0.65f, true),
            Placement("TOOL-SHELVES-SW", "08 Tool Crib and Maintenance Stores", WarehouseShelves, 1240, 1620, 180, 0.65f, true),
            Placement("TOOL-SHELVES-SE", "08 Tool Crib and Maintenance Stores", WarehouseShelves, 1310, 1620, 180, 0.65f, true),
            Placement("TOOL-CAGE", "08 Tool Crib and Maintenance Stores", ToolCage, 1360, 1610, -90, 1, true),
            Placement("TOOL-WORKBENCH", "08 Tool Crib and Maintenance Stores", Workbench, 1360, 1510, -90, 1, true),
            Placement("TOOL-PARTS-CART", "08 Tool Crib and Maintenance Stores", PartsCart, 1375, 1560, 90, 1, false, portable: true),
            Placement("TOOL-WAREHOUSE-CART", "08 Tool Crib and Maintenance Stores", WarehouseCart, 1250, 1570, 8, 1, false, portable: true),
            Placement("TOOL-CRATE", "08 Tool Crib and Maintenance Stores", WarehouseCrateShort, 1390, 1580, 15, 1, true),

            // Static extraction identities. Contract anchors remain inactive in the gameplay root.
            Placement("EXTRACTION-VEHICLE-AIRLOCK", "09 Extraction Architecture (Contract Anchors Inactive)", VehicleAirlock, 1660, 1815, 0, 1, true),
            Placement("EXTRACTION-MAINTENANCE-LIFT", "09 Extraction Architecture (Contract Anchors Inactive)", MaintenanceLift, 1750, 690, 90, 1, true),
            Placement("EXTRACTION-EMERGENCY-EGRESS", "09 Extraction Architecture (Contract Anchors Inactive)", EmergencyEgress, 830, 440, -45, 1, true),
        };

        [MenuItem("Prototype/Approved Facility/Rebuild Manual Shell Room Population")]
        public static void RebuildP02ManualShellRoomPopulation()
        {
            var scene = RequireManualP02Scene();
            var plan = new ManualPlanSpace(RequireSceneRoot(scene, "FloorPlan").transform);
            var rooms = RequireSceneRoot(scene, "Rooms");
            var hallways = RequireSceneRoot(scene, "Hallways");
            var manualRoomCount = rooms.GetComponentsInChildren<Transform>(true).Length;
            var manualHallwayCount = hallways.GetComponentsInChildren<Transform>(true).Length;

            RemoveExactRoot(scene, P02PopulationRootName);
            var root = new GameObject(P02PopulationRootName);
            SceneManager.MoveGameObjectToScene(root, scene);

            var gameplayRoot = scene.GetRootGameObjects().SingleOrDefault(item => item.name == P02GameplayRootName);
            if (gameplayRoot == null)
            {
                throw new InvalidOperationException(
                    $"'{P02GameplayRootName}' must exist before room population so distraction props can target the active Owl.");
            }

            var owl = gameplayRoot.GetComponentInChildren<PursuitThreatController>(true);
            if (owl == null) throw new InvalidOperationException("The P02 gameplay root has no PursuitThreatController.");

            var readyMaterial = AssetDatabase.LoadAssetAtPath<Material>(
                "Assets/_Project/Materials/FacilityKit/M_FK_InstrumentAmber.mat");
            var activeMaterial = AssetDatabase.LoadAssetAtPath<Material>(
                "Assets/_Project/Materials/M_Danger_Red.mat");
            if (readyMaterial == null || activeMaterial == null)
            {
                throw new InvalidOperationException("Required maintenance-panel materials are missing.");
            }

            var groupLookup = new Dictionary<string, Transform>(StringComparer.Ordinal);
            foreach (var groupName in P02PopulationPlacements.Select(item => item.Group).Distinct())
            {
                var group = new GameObject(groupName);
                group.transform.SetParent(root.transform, false);
                groupLookup.Add(groupName, group.transform);
            }

            var placedRoots = new List<GameObject>(P02PopulationPlacements.Length);
            var warehouseCount = 0;
            var facilityKitCount = 0;
            foreach (var placement in P02PopulationPlacements)
            {
                var instance = InstantiatePopulationPrefab(scene, groupLookup[placement.Group], placement, plan);
                placedRoots.Add(instance);
                if (placement.PrefabPath.StartsWith(WarehouseRoot, StringComparison.Ordinal)) warehouseCount++;
                if (placement.PrefabPath.StartsWith(FacilityKitRoot, StringComparison.Ordinal)) facilityKitCount++;

                if (placement.Portable)
                {
                    ConfigurePortableMetalPopulationProp(instance, owl);
                }
                else if (placement.BlocksNavigation)
                {
                    AddPopulationCarvingObstacle(instance);
                }

                if (placement.FixedDistraction)
                {
                    ConfigureFixedPopulationDistraction(instance, owl, readyMaterial, activeMaterial);
                }
            }

            if (rooms.GetComponentsInChildren<Transform>(true).Length != manualRoomCount ||
                hallways.GetComponentsInChildren<Transform>(true).Length != manualHallwayCount)
            {
                throw new InvalidOperationException("Manual Rooms or Hallways changed during population generation.");
            }

            var report = ValidateP02Population(scene, root, placedRoots, warehouseCount, facilityKitCount);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("P02_ROOM_POPULATION_REBUILT|" + report);
        }

        private static GameObject InstantiatePopulationPrefab(
            Scene scene,
            Transform parent,
            PopulationPlacement placement,
            ManualPlanSpace plan)
        {
            if (!placement.PrefabPath.StartsWith(WarehouseRoot, StringComparison.Ordinal) &&
                !placement.PrefabPath.StartsWith(FacilityKitRoot, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Unapproved population source: " + placement.PrefabPath);
            }

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(placement.PrefabPath);
            if (prefab == null) throw new InvalidOperationException("Population prefab missing: " + placement.PrefabPath);
            var instance = PrefabUtility.InstantiatePrefab(prefab, scene) as GameObject;
            if (instance == null) throw new InvalidOperationException("Could not instantiate: " + placement.PrefabPath);

            instance.name = placement.StableId + "__" + prefab.name;
            instance.transform.SetParent(parent, true);
            instance.transform.localScale = Vector3.one * placement.UniformScale;
            instance.transform.SetPositionAndRotation(
                plan.ImageToWorld(placement.ImagePoint),
                plan.ImageYawToRotation(placement.ImageYaw));

            if (!TryGetRendererBounds(instance, out var bounds))
            {
                throw new InvalidOperationException("Population prefab has no Renderer: " + placement.PrefabPath);
            }

            var target = plan.ImageToWorld(placement.ImagePoint);
            instance.transform.position += new Vector3(
                target.x - bounds.center.x,
                ManualFloorHeight - bounds.min.y,
                target.z - bounds.center.z);
            return instance;
        }

        private static void AddPopulationCarvingObstacle(GameObject instance)
        {
            if (!TryGetRendererBounds(instance, out var worldBounds)) return;
            var localBounds = WorldBoundsToLocal(instance.transform, worldBounds);
            var obstacle = GetOrAddPopulationComponent<NavMeshObstacle>(instance);
            obstacle.shape = NavMeshObstacleShape.Box;
            obstacle.center = localBounds.center;
            obstacle.size = new Vector3(
                Mathf.Max(0.20f, localBounds.size.x),
                Mathf.Max(0.20f, localBounds.size.y),
                Mathf.Max(0.20f, localBounds.size.z));
            obstacle.carving = true;
            obstacle.carveOnlyStationary = true;
            obstacle.carvingMoveThreshold = 0.10f;
            obstacle.carvingTimeToStationary = 0.15f;
        }

        private static void ConfigurePortableMetalPopulationProp(GameObject instance, PursuitThreatController owl)
        {
            foreach (var obstacle in instance.GetComponentsInChildren<NavMeshObstacle>(true))
            {
                UnityEngine.Object.DestroyImmediate(obstacle);
            }

            var body = GetOrAddPopulationComponent<Rigidbody>(instance);
            body.mass = 3.2f;
            body.linearDamping = 0.12f;
            body.angularDamping = 0.24f;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            if (instance.GetComponent<XRGrabInteractable>() == null) instance.AddComponent<XRGrabInteractable>();
            var grabbable = GetOrAddPopulationComponent<GrabbableObject>(instance);
            grabbable.ConfigureDesktopGripPose(new Vector3(0.25f, -0.08f, 0f), Vector3.zero);

            var source = GetOrAddPopulationComponent<AudioSource>(instance);
            ConfigurePopulationSpatialAudioSource(source, 0.6f, 22f);
            var impactAudio = GetOrAddPopulationComponent<ProceduralSpatialAudio>(instance);
            impactAudio.Configure(source);
            var stimulus = GetOrAddPopulationComponent<ThrowableThreatStimulus>(instance);
            stimulus.Configure(owl, impactAudio);
        }

        private static void ConfigureFixedPopulationDistraction(
            GameObject instance,
            PursuitThreatController owl,
            Material readyMaterial,
            Material activeMaterial)
        {
            var indicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
            indicator.name = "Maintenance Test Indicator";
            indicator.transform.SetParent(instance.transform, false);
            indicator.transform.localPosition = new Vector3(0f, 1.36f, -0.46f);
            indicator.transform.localScale = new Vector3(0.55f, 0.12f, 0.04f);
            UnityEngine.Object.DestroyImmediate(indicator.GetComponent<Collider>());
            var indicatorRenderer = indicator.GetComponent<Renderer>();
            indicatorRenderer.sharedMaterial = readyMaterial;

            var source = GetOrAddPopulationComponent<AudioSource>(instance);
            ConfigurePopulationSpatialAudioSource(source, 0.8f, MachineryDistractionPanel.DefaultRadius);
            var panelAudio = GetOrAddPopulationComponent<ProceduralSpatialAudio>(instance);
            panelAudio.Configure(source);
            var panel = GetOrAddPopulationComponent<MachineryDistractionPanel>(instance);
            panel.Configure(owl, indicatorRenderer, readyMaterial, activeMaterial, panelAudio);
        }

        private static T GetOrAddPopulationComponent<T>(GameObject gameObject) where T : Component
        {
            // Unity's native-object null can differ from CLR null for removed prefab
            // overrides, so use the overloaded equality check instead of ?? here.
            var component = gameObject.GetComponent<T>();
            if (component == null) component = gameObject.AddComponent<T>();
            return component;
        }

        private static void ConfigurePopulationSpatialAudioSource(AudioSource source, float minDistance, float maxDistance)
        {
            source.playOnAwake = false;
            source.loop = false;
            source.spatialBlend = 1f;
            source.rolloffMode = AudioRolloffMode.Logarithmic;
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;
        }

        private static int RebindP02PopulationThreatSink(Scene scene, PursuitThreatController owl)
        {
            var roots = scene.GetRootGameObjects().Where(item => item.name == P02PopulationRootName).ToArray();
            if (roots.Length == 0) return 0;
            if (roots.Length != 1)
            {
                throw new InvalidOperationException(
                    $"Expected at most one '{P02PopulationRootName}' while rebinding gameplay, found {roots.Length}.");
            }

            var rebound = 0;
            foreach (var behaviour in roots[0].GetComponentsInChildren<MonoBehaviour>(true)
                         .Where(item => item is ThrowableThreatStimulus or MachineryDistractionPanel))
            {
                var serialized = new SerializedObject(behaviour);
                var sink = serialized.FindProperty("stimulusSinkBehaviour");
                if (sink == null)
                {
                    throw new InvalidOperationException($"{behaviour.name} has no serialized stimulus sink field.");
                }
                sink.objectReferenceValue = owl;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(behaviour);
                rebound++;
            }
            return rebound;
        }

        private static string ValidateP02Population(
            Scene scene,
            GameObject root,
            IReadOnlyCollection<GameObject> placedRoots,
            int warehouseCount,
            int facilityKitCount)
        {
            var roots = scene.GetRootGameObjects();
            var ownedRootCount = roots.Count(item => item.name == P02PopulationRootName);
            var duplicateNames = placedRoots.GroupBy(item => item.name).Count(group => group.Count() > 1);
            var disconnectedPrefabs = placedRoots.Count(item =>
                PrefabUtility.GetCorrespondingObjectFromSource(item) == null ||
                PrefabUtility.GetPrefabInstanceStatus(item) == PrefabInstanceStatus.NotAPrefab);
            var missingColliders = placedRoots.Count(item => item.GetComponentsInChildren<Collider>(true).Length == 0);
            var portableCount = root.GetComponentsInChildren<ThrowableThreatStimulus>(true).Length;
            var panelCount = root.GetComponentsInChildren<MachineryDistractionPanel>(true).Length;
            var obstacleCount = root.GetComponentsInChildren<NavMeshObstacle>(true).Count(item => item.carving);
            Physics.SyncTransforms();
            var wallOverlapCount = CountManualWallOverlaps(scene, placedRoots);
            var propOverlapCount = CountPopulationIntersections(placedRoots);
            var portalClearanceCount = CountPortalClearanceEncroachments(scene, placedRoots);
            var belowFloor = 0;
            foreach (var item in placedRoots)
            {
                if (TryGetRendererBounds(item, out var bounds) && bounds.min.y < ManualFloorHeight - 0.02f) belowFloor++;
            }

            var deprecated = roots.SingleOrDefault(item => item.name == "Deprecated");
            var gameplayRootCount = roots.Count(item => item.name == P02GameplayRootName);
            if (ownedRootCount != 1 || placedRoots.Count != P02PopulationPlacements.Length ||
                duplicateNames != 0 || disconnectedPrefabs != 0 || missingColliders != 0 ||
                portableCount != P02PopulationPlacements.Count(item => item.Portable) ||
                panelCount != P02PopulationPlacements.Count(item => item.FixedDistraction) ||
                wallOverlapCount != 0 || propOverlapCount != 0 || portalClearanceCount != 0 ||
                belowFloor != 0 || gameplayRootCount != 1 || deprecated == null || deprecated.activeSelf)
            {
                throw new InvalidOperationException(
                    $"P02 population validation failed: roots={ownedRootCount}, placed={placedRoots.Count}, " +
                    $"duplicates={duplicateNames}, disconnected={disconnectedPrefabs}, missingColliders={missingColliders}, " +
                    $"portable={portableCount}, panels={panelCount}, belowFloor={belowFloor}, " +
                    $"wallOverlaps={wallOverlapCount}, propOverlaps={propOverlapCount}, portalClearance={portalClearanceCount}, " +
                    $"gameplayRoots={gameplayRootCount}, deprecatedInactive={deprecated != null && !deprecated.activeSelf}.");
            }

            return $"root={P02PopulationRootName}|groups={root.transform.childCount}|placed={placedRoots.Count}" +
                   $"|warehouse={warehouseCount}|facilityKit={facilityKitCount}|prefabLinked={placedRoots.Count - disconnectedPrefabs}" +
                   $"|portableMetal={portableCount}|fixedMachineryPanels={panelCount}|carvingObstacles={obstacleCount}" +
                   $"|missingColliders={missingColliders}|duplicates={duplicateNames}|belowFloor={belowFloor}" +
                   $"|wallOverlaps={wallOverlapCount}|propOverlaps={propOverlapCount}|portalClearanceHits={portalClearanceCount}" +
                   $"|gameplayRootCount={gameplayRootCount}|deprecatedInactive={deprecated != null && !deprecated.activeSelf}";
        }

        private static int CountManualWallOverlaps(Scene scene, IEnumerable<GameObject> placedRoots)
        {
            var rooms = RequireSceneRoot(scene, "Rooms").transform;
            var hallways = RequireSceneRoot(scene, "Hallways").transform;
            var count = 0;
            foreach (var item in placedRoots)
            {
                if (!TryGetRendererBounds(item, out var bounds)) continue;
                var extents = new Vector3(
                    Mathf.Max(0.02f, bounds.extents.x - 0.04f),
                    Mathf.Max(0.02f, bounds.extents.y - 0.04f),
                    Mathf.Max(0.02f, bounds.extents.z - 0.04f));
                if (Physics.OverlapBox(bounds.center, extents, Quaternion.identity, Physics.AllLayers,
                        QueryTriggerInteraction.Ignore)
                    .Any(collider => collider != null &&
                                     (collider.transform.IsChildOf(rooms) || collider.transform.IsChildOf(hallways))))
                {
                    count++;
                }
            }
            return count;
        }

        private static int CountPopulationIntersections(IEnumerable<GameObject> placedRoots)
        {
            var bounds = placedRoots
                .Select(item => TryGetRendererBounds(item, out var itemBounds)
                    ? new KeyValuePair<GameObject, Bounds>(item, itemBounds)
                    : default)
                .Where(pair => pair.Key != null)
                .ToArray();
            var count = 0;
            for (var first = 0; first < bounds.Length; first++)
            {
                for (var second = first + 1; second < bounds.Length; second++)
                {
                    var a = bounds[first].Value;
                    var b = bounds[second].Value;
                    a.Expand(-0.08f);
                    b.Expand(-0.08f);
                    if (a.Intersects(b)) count++;
                }
            }
            return count;
        }

        private static int CountPortalClearanceEncroachments(Scene scene, IEnumerable<GameObject> placedRoots)
        {
            var planReference = RequireSceneRoot(scene, "FloorPlan").transform;
            var bounds = placedRoots
                .Select(item => TryGetRendererBounds(item, out var itemBounds) ? itemBounds : default)
                .ToArray();
            var count = 0;
            foreach (var portal in ApprovedFacilityLayout.Portals)
            {
                var local = new Vector3(
                    portal.ImageCenter.x / ApprovedFacilityLayout.SourceWidthPixels - 0.5f,
                    0.5f - portal.ImageCenter.y / ApprovedFacilityLayout.SourceHeightPixels,
                    0f);
                var position = planReference.TransformPoint(local);
                foreach (var itemBounds in bounds)
                {
                    var deltaX = Mathf.Max(Mathf.Abs(position.x - itemBounds.center.x) - itemBounds.extents.x, 0f);
                    var deltaZ = Mathf.Max(Mathf.Abs(position.z - itemBounds.center.z) - itemBounds.extents.z, 0f);
                    if (Mathf.Sqrt(deltaX * deltaX + deltaZ * deltaZ) < 2f) count++;
                }
            }
            return count;
        }

        private static bool TryGetRendererBounds(GameObject gameObject, out Bounds bounds)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
            {
                bounds = default;
                return false;
            }

            bounds = renderers[0].bounds;
            for (var index = 1; index < renderers.Length; index++) bounds.Encapsulate(renderers[index].bounds);
            return true;
        }

        private static Bounds WorldBoundsToLocal(Transform transform, Bounds worldBounds)
        {
            var extents = worldBounds.extents;
            var center = worldBounds.center;
            var corners = new[]
            {
                center + new Vector3(-extents.x, -extents.y, -extents.z),
                center + new Vector3(-extents.x, -extents.y, extents.z),
                center + new Vector3(-extents.x, extents.y, -extents.z),
                center + new Vector3(-extents.x, extents.y, extents.z),
                center + new Vector3(extents.x, -extents.y, -extents.z),
                center + new Vector3(extents.x, -extents.y, extents.z),
                center + new Vector3(extents.x, extents.y, -extents.z),
                center + new Vector3(extents.x, extents.y, extents.z),
            };
            var local = new Bounds(transform.InverseTransformPoint(corners[0]), Vector3.zero);
            for (var index = 1; index < corners.Length; index++) local.Encapsulate(transform.InverseTransformPoint(corners[index]));
            return local;
        }

        private static PopulationPlacement Placement(
            string stableId,
            string group,
            string prefabPath,
            float imageX,
            float imageY,
            float imageYaw,
            float uniformScale,
            bool blocksNavigation,
            bool portable = false,
            bool fixedDistraction = false)
        {
            return new PopulationPlacement(
                stableId,
                group,
                prefabPath,
                new Vector2(imageX, imageY),
                imageYaw,
                uniformScale,
                blocksNavigation,
                portable,
                fixedDistraction);
        }

        private readonly struct PopulationPlacement
        {
            public PopulationPlacement(
                string stableId,
                string group,
                string prefabPath,
                Vector2 imagePoint,
                float imageYaw,
                float uniformScale,
                bool blocksNavigation,
                bool portable,
                bool fixedDistraction)
            {
                StableId = stableId;
                Group = group;
                PrefabPath = prefabPath;
                ImagePoint = imagePoint;
                ImageYaw = imageYaw;
                UniformScale = uniformScale;
                BlocksNavigation = blocksNavigation;
                Portable = portable;
                FixedDistraction = fixedDistraction;
            }

            public string StableId { get; }
            public string Group { get; }
            public string PrefabPath { get; }
            public Vector2 ImagePoint { get; }
            public float ImageYaw { get; }
            public float UniformScale { get; }
            public bool BlocksNavigation { get; }
            public bool Portable { get; }
            public bool FixedDistraction { get; }
        }
    }
}
