using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace InternshipGame.Prototype.Editor
{
    internal static class FacilityKitAssetBuilder
    {
        private const string UrpLitShader = "Universal Render Pipeline/Lit";
        private const string PaletteRootName = "_Generated FacilityKit Palette";

        [MenuItem("Prototype/Facility Kit/Rebuild Kit Prefabs and Palette")]
        public static void RebuildKitAndPalette()
        {
            var materials = BuildKitAssets();
            BuildPaletteScene(materials);
            Debug.Log($"FACILITY_KIT_REBUILT|prefabs={FacilityKitCatalog.Entries.Length}|palette={FacilityKitCatalog.PaletteScenePath}");
        }

        public static FacilityKitMaterials BuildKitAssets()
        {
            EnsureFolders();
            var materials = CreateMaterials();
            var glassLayer = LayerMask.NameToLayer("PerceptionTransparent");
            if (glassLayer < 0)
            {
                throw new InvalidOperationException(
                    "Required PerceptionTransparent layer is missing. Add it through Unity MCP before rebuilding the kit.");
            }

            foreach (var entry in FacilityKitCatalog.Entries)
            {
                BuildPrefab(entry, materials, glassLayer);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return materials;
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets/_Project/Prefabs", "Environment");
            EnsureFolder("Assets/_Project/Prefabs/Environment", "FacilityKit");
            foreach (var category in Enum.GetNames(typeof(FacilityKitCategory)))
            {
                EnsureFolder(FacilityKitCatalog.RootFolder, category);
            }

            EnsureFolder("Assets/_Project/Materials", "FacilityKit");
        }

        private static void EnsureFolder(string parent, string child)
        {
            var path = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }

        private static FacilityKitMaterials CreateMaterials()
        {
            return new FacilityKitMaterials
            {
                Wall = CreateMaterial("M_FK_WallConcrete", new Color(0.28f, 0.27f, 0.25f), 0.08f),
                Shield = CreateMaterial("M_FK_Shielding", new Color(0.18f, 0.17f, 0.16f), 0.12f),
                Glass = CreateMaterial("M_FK_Glass", new Color(0.12f, 0.72f, 0.82f, 0.30f), 0.82f, true),
                Door = CreateMaterial("M_FK_Door", new Color(0.24f, 0.28f, 0.30f), 0.34f),
                Floor = CreateMaterial("M_FK_Floor", new Color(0.10f, 0.105f, 0.105f), 0.18f),
                HotZone = CreateMaterial("M_FK_HotZone", new Color(0.72f, 0.18f, 0.06f), 0.22f),
                EditorReference = CreateMaterial("M_FK_EditorReference", new Color(0.14f, 0.62f, 0.92f, 0.18f), 0.35f, true),
                Metal = CreateMaterial("M_FK_Metal", new Color(0.29f, 0.31f, 0.31f), 0.48f),
                DarkTrim = CreateMaterial("M_FK_DarkTrim", new Color(0.035f, 0.04f, 0.04f), 0.22f),
                Decon = CreateMaterial("M_FK_DeconBlueGrey", new Color(0.31f, 0.42f, 0.49f), 0.31f),
                Clinical = CreateMaterial("M_FK_ClinicalGreen", new Color(0.35f, 0.57f, 0.49f), 0.34f),
                Amber = CreateMaterial("M_FK_InstrumentAmber", new Color(0.94f, 0.47f, 0.08f), 0.42f, false, true),
                Coolant = CreateMaterial("M_FK_CoolantCyan", new Color(0.10f, 0.54f, 0.67f), 0.46f),
                Chemical = CreateMaterial("M_FK_ChemicalGreen", new Color(0.28f, 0.47f, 0.33f), 0.32f),
                Fuel = CreateMaterial("M_FK_FuelOchre", new Color(0.60f, 0.40f, 0.13f), 0.28f),
            };
        }

        private static Material CreateMaterial(
            string name,
            Color color,
            float smoothness,
            bool transparent = false,
            bool emission = false)
        {
            var path = $"{FacilityKitCatalog.MaterialFolder}/{name}.mat";
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
            material.SetFloat("_Smoothness", smoothness);
            material.SetFloat("_Metallic", name.Contains("Metal", StringComparison.Ordinal) ? 0.65f : 0.05f);
            if (transparent)
            {
                material.SetFloat("_Surface", 1f);
                material.SetFloat("_Blend", 0f);
                material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                material.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                material.SetFloat("_ZWrite", 0f);
                material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                material.renderQueue = (int)RenderQueue.Transparent;
            }
            else
            {
                material.SetFloat("_Surface", 0f);
                material.SetFloat("_ZWrite", 1f);
                material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
                material.renderQueue = -1;
            }

            if (emission)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * 1.7f);
            }
            else
            {
                material.DisableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", Color.black);
            }

            EditorUtility.SetDirty(material);
            return material;
        }

        private static void BuildPrefab(
            FacilityKitCatalogEntry entry,
            FacilityKitMaterials materials,
            int glassLayer)
        {
            var root = new GameObject(Path.GetFileNameWithoutExtension(entry.PrefabPath));
            var metadata = root.AddComponent<FacilityModuleMetadata>();
            var bounds = BuildModuleGeometry(entry.StableId, root.transform, materials, glassLayer);
            metadata.Configure(
                entry.StableId,
                entry.Category,
                bounds,
                GetCoverClass(entry.StableId),
                !entry.StableId.StartsWith("FK-DOOR", StringComparison.Ordinal) &&
                !entry.StableId.StartsWith("FK-ONEWAY", StringComparison.Ordinal),
                entry.StableId == "FK-CEILING-2X2");

            foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
            {
                renderer.shadowCastingMode = entry.StableId.Contains("WINDOW", StringComparison.Ordinal)
                    ? ShadowCastingMode.Off
                    : ShadowCastingMode.On;
            }

            PrefabUtility.SaveAsPrefabAsset(root, entry.PrefabPath);
            UnityEngine.Object.DestroyImmediate(root);
        }

        private static FacilityCoverClass GetCoverClass(string id)
        {
            if (id.Contains("WINDOW", StringComparison.Ordinal)) return FacilityCoverClass.PerceptionTransparent;
            if (id.Contains("SHIELD", StringComparison.Ordinal) || id == "FK-BLOCKED-MASS") return FacilityCoverClass.FullCover;
            if (id.Contains("CASK", StringComparison.Ordinal) || id.Contains("PUMP", StringComparison.Ordinal)) return FacilityCoverClass.Partial;
            return FacilityCoverClass.None;
        }

        private static Vector3 BuildModuleGeometry(
            string id,
            Transform root,
            FacilityKitMaterials m,
            int glassLayer)
        {
            switch (id)
            {
                case "FK-WALL-1M": return BuildStraightWall(root, 1f, m.Wall);
                case "FK-WALL-2M": return BuildStraightWall(root, 2f, m.Wall);
                case "FK-WALL-4M": return BuildStraightWall(root, 4f, m.Wall);
                case "FK-CORNER-INSIDE":
                    AddBox("Leg X", root, new Vector3(1f, 1.8f, 0f), new Vector3(2f, 3.6f, 0.3f), m.Wall);
                    AddBox("Leg Z", root, new Vector3(0f, 1.8f, 1f), new Vector3(0.3f, 3.6f, 2f), m.Wall);
                    return new Vector3(2f, 3.6f, 2f);
                case "FK-CORNER-OUTSIDE":
                    AddBox("Outer Leg X", root, new Vector3(-1f, 1.8f, 0f), new Vector3(2f, 3.6f, 0.3f), m.Wall);
                    AddBox("Outer Leg Z", root, new Vector3(0f, 1.8f, -1f), new Vector3(0.3f, 3.6f, 2f), m.Wall);
                    return new Vector3(2f, 3.6f, 2f);
                case "FK-WALL-ANGLED-45":
                {
                    var wall = AddBox("45 Degree Wall", root, new Vector3(0f, 1.8f, 0f), new Vector3(2f, 3.6f, 0.3f), m.Wall);
                    wall.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
                    return new Vector3(1.63f, 3.6f, 1.63f);
                }
                case "FK-FLOOR-2X2": AddBox("Floor", root, new Vector3(0f, -0.1f, 0f), new Vector3(2f, 0.2f, 2f), m.Floor); return new Vector3(2f, 0.2f, 2f);
                case "FK-CEILING-2X2": AddBox("Ceiling", root, new Vector3(0f, 3.7f, 0f), new Vector3(2f, 0.2f, 2f), m.Wall); return new Vector3(2f, 0.2f, 2f);
                case "FK-END-CAP": AddBox("End Cap", root, new Vector3(0f, 1.8f, 0f), new Vector3(0.3f, 3.6f, 0.5f), m.DarkTrim); return new Vector3(0.3f, 3.6f, 0.5f);
                case "FK-COLUMN": AddBox("Column", root, new Vector3(0f, 1.8f, 0f), new Vector3(0.62f, 3.6f, 0.62f), m.Shield); return new Vector3(0.62f, 3.6f, 0.62f);
                case "FK-WALL-TRIM": AddBox("Trim", root, new Vector3(0f, 0.18f, -0.18f), new Vector3(2f, 0.36f, 0.12f), m.DarkTrim); return new Vector3(2f, 0.36f, 0.12f);
                case "FK-PORTAL-FRAME": return BuildPortalFrame(root, m, false);
                case "FK-REACTOR-SEGMENT": AddBox("Arc Chord", root, new Vector3(0f, 1.8f, 0f), new Vector3(1.54f, 3.6f, 0.45f), m.Shield); return new Vector3(1.54f, 3.6f, 0.45f);
                case "FK-BLOCKED-MASS": AddBox("Blocked Mass", root, new Vector3(0f, 1.8f, 0f), new Vector3(2f, 3.6f, 2f), m.Shield); return new Vector3(2f, 3.6f, 2f);

                case "FK-WINDOW-OBSERVATION": return BuildWindow(root, m, glassLayer, 3.2f, 1.5f, 0.18f);
                case "FK-WINDOW-SERVICE": return BuildWindow(root, m, glassLayer, 1.4f, 1.15f, 0.16f);
                // Four 12-degree arc modules are omitted at the facility-facing side.
                // A 6.2 m span covers the resulting ~6.0 m chord as one continuous,
                // collision-bearing lead-glass boundary.
                case "FK-WINDOW-REACTOR-LEADGLASS": return BuildWindow(root, m, glassLayer, 6.2f, 2.1f, 0.28f);
                case "FK-DOOR-SLIDING": return BuildDoorPresentation(root, m, DoorPresentation.Sliding);
                case "FK-DOOR-HINGED": return BuildDoorPresentation(root, m, DoorPresentation.Hinged);
                case "FK-DOOR-LOCKED": return BuildDoorPresentation(root, m, DoorPresentation.Locked);
                case "FK-ONEWAY-BULKHEAD": return BuildOneWayDoor(root, m, OneWayPresentation.PressureBulkhead);
                case "FK-ONEWAY-CRASHBAR": return BuildOneWayDoor(root, m, OneWayPresentation.CrashBar);
                case "FK-ONEWAY-VALVE": return BuildOneWayDoor(root, m, OneWayPresentation.MaintenanceValve);

                case "FK-SHIELD-STRAIGHT": AddBox("Shield", root, new Vector3(0f, 1.35f, 0f), new Vector3(4.6f, 2.7f, 0.9f), m.Shield); return new Vector3(4.6f, 2.7f, 0.9f);
                case "FK-SHIELD-CORNER":
                    AddBox("Shield X", root, new Vector3(1.2f, 1.35f, 0f), new Vector3(3.2f, 2.7f, 0.9f), m.Shield);
                    AddBox("Shield Z", root, new Vector3(0f, 1.35f, 1.2f), new Vector3(0.9f, 2.7f, 3.2f), m.Shield);
                    return new Vector3(3.2f, 2.7f, 3.2f);

                case "FK-CONTROL-CONSOLE-STRAIGHT": return BuildControlConsole(root, m, false);
                case "FK-CONTROL-CONSOLE-CORNER": return BuildControlConsole(root, m, true);
                case "FK-ASSAY-BENCH": return BuildAssayBench(root, m);
                case "FK-ISOLATION-CABINET": return BuildIsolationCabinet(root, m, glassLayer);
                case "FK-COOLANT-BASIN": return BuildCoolantBasin(root, m);
                case "FK-PUMP-A": return BuildPump(root, m, 0);
                case "FK-PUMP-B": return BuildPump(root, m, 1);
                case "FK-PUMP-C": return BuildPump(root, m, 2);
                case "FK-PIPE-A": return BuildPipeCluster(root, m, 0);
                case "FK-PIPE-B": return BuildPipeCluster(root, m, 1);
                case "FK-PIPE-C": return BuildPipeCluster(root, m, 2);
                case "FK-SCRUBBER-VESSEL": return BuildScrubber(root, m);
                case "FK-DUCT-BANK": return BuildDuctBank(root, m);
                case "FK-CASK-INDIVIDUAL": return BuildCask(root, m, 1);
                case "FK-CASK-ROW": return BuildCask(root, m, 4);
                case "FK-CASK-END": AddBox("Cask Row End", root, new Vector3(0f, 1f, 0f), new Vector3(0.35f, 2f, 2f), m.Fuel); return new Vector3(0.35f, 2f, 2f);
                case "FK-OVERHEAD-CRANE": return BuildCrane(root, m);
                case "FK-LOCKERS": return BuildLockers(root, m);
                case "FK-TOOL-CAGE": return BuildToolCage(root, m);
                case "FK-WORKBENCH": return BuildWorkbench(root, m);
                case "FK-PARTS-CART": return BuildPartsCart(root, m);
                case "FK-PORTABLE-STORAGE": return BuildPortableStorage(root, m);
                case "FK-DECON-FRAME": return BuildDeconFrame(root, m);
                case "FK-INTAKE-STATION": return BuildIntake(root, m);
                case "FK-EXTRACTION-VEHICLE": return BuildExtraction(root, m, 0);
                case "FK-EXTRACTION-LIFT": return BuildExtraction(root, m, 1);
                case "FK-EXTRACTION-EGRESS": return BuildExtraction(root, m, 2);
                default: throw new ArgumentOutOfRangeException(nameof(id), id, "Unknown FacilityKit catalog id.");
            }
        }

        private static Vector3 BuildStraightWall(Transform root, float length, Material material)
        {
            AddBox("Wall", root, new Vector3(0f, 1.8f, 0f), new Vector3(length, 3.6f, 0.3f), material);
            return new Vector3(length, 3.6f, 0.3f);
        }

        private static Vector3 BuildPortalFrame(Transform root, FacilityKitMaterials m, bool includeThreshold)
        {
            var half = ApprovedFacilityLayout.PrimaryPortalClearWidth * 0.5f;
            AddBox("Left Jamb", root, new Vector3(-half - 0.15f, 1.26f, 0f), new Vector3(0.3f, 2.52f, 0.42f), m.DarkTrim);
            AddBox("Right Jamb", root, new Vector3(half + 0.15f, 1.26f, 0f), new Vector3(0.3f, 2.52f, 0.42f), m.DarkTrim);
            AddBox("Header", root, new Vector3(0f, 2.7f, 0f), new Vector3(3.36f, 0.36f, 0.42f), m.DarkTrim);
            if (includeThreshold) AddBox("Threshold", root, new Vector3(0f, 0.04f, 0f), new Vector3(2.76f, 0.08f, 0.55f), m.Metal);
            return new Vector3(3.36f, 2.88f, 0.55f);
        }

        private static Vector3 BuildWindow(Transform root, FacilityKitMaterials m, int glassLayer, float width, float height, float depth)
        {
            var frame = 0.22f;
            AddBox("Left Frame", root, new Vector3(-width * 0.5f - frame * 0.5f, height * 0.5f + 0.75f, 0f), new Vector3(frame, height + frame * 2f, depth + 0.12f), m.DarkTrim);
            AddBox("Right Frame", root, new Vector3(width * 0.5f + frame * 0.5f, height * 0.5f + 0.75f, 0f), new Vector3(frame, height + frame * 2f, depth + 0.12f), m.DarkTrim);
            AddBox("Top Frame", root, new Vector3(0f, height + 0.75f + frame * 0.5f, 0f), new Vector3(width, frame, depth + 0.12f), m.DarkTrim);
            AddBox("Bottom Frame", root, new Vector3(0f, 0.75f - frame * 0.5f, 0f), new Vector3(width, frame, depth + 0.12f), m.DarkTrim);
            var glass = AddBox("Perception Transparent Glass", root, new Vector3(0f, height * 0.5f + 0.75f, 0f), new Vector3(width, height, depth), m.Glass);
            glass.layer = glassLayer;
            return new Vector3(width + frame * 2f, height + frame * 2f, depth + 0.12f);
        }

        private enum DoorPresentation { Sliding, Hinged, Locked }
        private enum OneWayPresentation { PressureBulkhead, CrashBar, MaintenanceValve }

        private static Vector3 BuildDoorPresentation(Transform root, FacilityKitMaterials m, DoorPresentation presentation)
        {
            BuildPortalFrame(root, m, true);
            var leaf = AddBox("Door Leaf", root, new Vector3(0f, 1.25f, 0f), new Vector3(2.7f, 2.5f, 0.20f), presentation == DoorPresentation.Locked ? m.HotZone : m.Door);
            if (presentation == DoorPresentation.Hinged)
            {
                leaf.transform.localPosition = new Vector3(-1.33f, 1.25f, 0f);
                leaf.transform.localScale = new Vector3(0.05f, 2.5f, 2.7f);
                leaf.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                AddBox("Lever", root, new Vector3(-1.15f, 1.15f, -0.22f), new Vector3(0.35f, 0.08f, 0.08f), m.Amber, false);
            }
            else
            {
                AddBox(presentation == DoorPresentation.Locked ? "Red Lock Bar" : "Slide Rail", root, new Vector3(0f, 1.25f, -0.16f), new Vector3(2.1f, 0.14f, 0.08f), presentation == DoorPresentation.Locked ? m.HotZone : m.Amber, false);
            }
            return new Vector3(3.36f, 2.88f, 0.55f);
        }

        private static Vector3 BuildOneWayDoor(Transform root, FacilityKitMaterials m, OneWayPresentation presentation)
        {
            BuildPortalFrame(root, m, true);
            var leaf = AddBox("Moving Door Leaf", root, new Vector3(0f, 1.25f, 0f), new Vector3(2.7f, 2.5f, 0.20f), m.Door);
            var blocker = leaf.GetComponent<Collider>();
            if (presentation == OneWayPresentation.PressureBulkhead)
            {
                AddBox("Pressure Dogs", root, new Vector3(0f, 1.25f, -0.16f), new Vector3(1.8f, 0.12f, 0.08f), m.Amber, false);
                AddCylinder("Wheel", root, new Vector3(0f, 1.25f, -0.25f), new Vector3(0.34f, 0.07f, 0.34f), Quaternion.Euler(90f, 0f, 0f), m.Metal, false);
            }
            else if (presentation == OneWayPresentation.CrashBar)
            {
                AddBox("Crash Bar", root, new Vector3(0f, 1.1f, -0.2f), new Vector3(1.9f, 0.13f, 0.10f), m.HotZone, false);
            }
            else
            {
                AddCylinder("Valve Wheel", root, new Vector3(0f, 1.25f, -0.26f), new Vector3(0.52f, 0.08f, 0.52f), Quaternion.Euler(90f, 0f, 0f), m.Chemical, false);
                AddBox("Valve Stem", root, new Vector3(0f, 1.25f, -0.34f), new Vector3(0.10f, 0.10f, 0.22f), m.Metal, false);
            }

            var clearanceObject = new GameObject("Safe Obstruction Volume");
            clearanceObject.transform.SetParent(root, false);
            clearanceObject.transform.localPosition = new Vector3(0f, 1.25f, 0f);
            var clearance = clearanceObject.AddComponent<BoxCollider>();
            clearance.isTrigger = true;
            clearance.size = new Vector3(3.05f, 2.5f, 1.4f);

            var sensor = root.gameObject.AddComponent<BoxCollider>();
            sensor.isTrigger = true;
            sensor.center = new Vector3(0f, 1.25f, 0f);
            sensor.size = new Vector3(3.1f, 2.5f, 2.2f);
            var lightObject = new GameObject("Direction State Light");
            lightObject.transform.SetParent(root, false);
            lightObject.transform.localPosition = new Vector3(1.55f, 2.25f, -0.28f);
            var stateLight = lightObject.AddComponent<Light>();
            stateLight.type = LightType.Point;
            stateLight.range = 2.5f;
            stateLight.intensity = 2.2f;

            var controller = root.gameObject.AddComponent<OneWayTraversalController>();
            controller.Configure(
                leaf.transform,
                blocker,
                clearance,
                Vector3.forward,
                true,
                presentation switch
                {
                    OneWayPresentation.PressureBulkhead => OneWayOwlPolicy.AlternateRouteOnly,
                    OneWayPresentation.CrashBar => OneWayOwlPolicy.FollowPlayerRule,
                    _ => OneWayOwlPolicy.AllowBothDirections,
                },
                TraversalDoorState.Closed,
                true,
                presentation == OneWayPresentation.MaintenanceValve ? 2.2f : 1.2f,
                new Vector3(2.85f, 0f, 0f));
            var stateLightField = new SerializedObject(controller).FindProperty("stateLight");
            stateLightField.objectReferenceValue = stateLight;
            stateLightField.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return new Vector3(3.36f, 2.88f, 2.2f);
        }

        private static Vector3 BuildControlConsole(Transform root, FacilityKitMaterials m, bool corner)
        {
            AddBox("Console Base", root, new Vector3(0f, 0.55f, 0f), new Vector3(2.4f, 1.1f, 0.8f), m.Metal);
            var panel = AddBox("Analog Panel", root, new Vector3(0f, 1.15f, 0.22f), new Vector3(2.3f, 0.65f, 0.16f), m.Door);
            panel.transform.localRotation = Quaternion.Euler(-18f, 0f, 0f);
            for (var i = -2; i <= 2; i++) AddBox($"Amber Instrument {i + 3}", root, new Vector3(i * 0.38f, 1.22f, 0.12f), new Vector3(0.18f, 0.12f, 0.04f), m.Amber, false);
            if (corner) AddBox("Corner Return", root, new Vector3(1.55f, 0.55f, 0.75f), new Vector3(0.8f, 1.1f, 2.3f), m.Metal);
            return corner ? new Vector3(2.8f, 1.5f, 2.3f) : new Vector3(2.4f, 1.5f, 0.9f);
        }

        private static Vector3 BuildAssayBench(Transform root, FacilityKitMaterials m)
        {
            AddBox("Stainless Worktop", root, new Vector3(0f, 0.92f, 0f), new Vector3(2.4f, 0.16f, 0.85f), m.Metal);
            AddBox("Left Cabinet", root, new Vector3(-0.8f, 0.45f, 0f), new Vector3(0.65f, 0.9f, 0.7f), m.Clinical);
            AddBox("Right Cabinet", root, new Vector3(0.8f, 0.45f, 0f), new Vector3(0.65f, 0.9f, 0.7f), m.Clinical);
            return new Vector3(2.4f, 1f, 0.85f);
        }

        private static Vector3 BuildIsolationCabinet(Transform root, FacilityKitMaterials m, int glassLayer)
        {
            AddBox("Cabinet", root, new Vector3(0f, 1.1f, 0f), new Vector3(1.4f, 2.2f, 0.75f), m.Clinical);
            var glass = AddBox("Isolation Glass", root, new Vector3(0f, 1.35f, -0.39f), new Vector3(1.05f, 0.8f, 0.04f), m.Glass);
            glass.layer = glassLayer;
            return new Vector3(1.4f, 2.2f, 0.8f);
        }

        private static Vector3 BuildCoolantBasin(Transform root, FacilityKitMaterials m)
        {
            AddCylinder("Basin", root, new Vector3(0f, 0.45f, 0f), new Vector3(3.2f, 0.45f, 3.2f), Quaternion.identity, m.Coolant);
            AddCylinder("Dry Center", root, new Vector3(0f, 0.92f, 0f), new Vector3(1.35f, 0.48f, 1.35f), Quaternion.identity, m.DarkTrim);
            return new Vector3(6.4f, 1.4f, 6.4f);
        }

        private static Vector3 BuildPump(Transform root, FacilityKitMaterials m, int variant)
        {
            if (variant == 0)
            {
                AddCylinder("Horizontal Pump", root, new Vector3(0f, 0.95f, 0f), new Vector3(0.75f, 1.5f, 0.75f), Quaternion.Euler(0f, 0f, 90f), m.Metal);
                AddBox("Drive", root, new Vector3(1.25f, 0.75f, 0f), new Vector3(1.1f, 1.1f, 1.1f), m.Coolant);
            }
            else if (variant == 1)
            {
                AddCylinder("Twin A", root, new Vector3(-0.75f, 1.0f, 0f), new Vector3(0.65f, 1f, 0.65f), Quaternion.identity, m.Metal);
                AddCylinder("Twin B", root, new Vector3(0.75f, 1.0f, 0f), new Vector3(0.65f, 1f, 0.65f), Quaternion.identity, m.Metal);
                AddBox("Manifold", root, new Vector3(0f, 1.65f, 0f), new Vector3(2.4f, 0.35f, 0.65f), m.Coolant);
            }
            else
            {
                AddCylinder("Vertical Pump", root, new Vector3(0f, 1.25f, 0f), new Vector3(0.85f, 1.25f, 0.85f), Quaternion.identity, m.Metal);
                AddCylinder("Top Motor", root, new Vector3(0f, 2.65f, 0f), new Vector3(0.58f, 0.55f, 0.58f), Quaternion.identity, m.Coolant);
            }
            return variant == 2 ? new Vector3(1.7f, 3.2f, 1.7f) : new Vector3(3f, 2.2f, 1.5f);
        }

        private static Vector3 BuildPipeCluster(Transform root, FacilityKitMaterials m, int variant)
        {
            var count = variant + 2;
            for (var i = 0; i < count; i++)
            {
                var rotation = variant == 1 && i == count - 1 ? Quaternion.Euler(0f, 0f, 90f) : Quaternion.identity;
                AddCylinder($"Pipe {i + 1}", root, new Vector3((i - (count - 1) * 0.5f) * 0.65f, 1.25f, (i % 2) * 0.35f), new Vector3(0.22f, 1.25f, 0.22f), rotation, i % 2 == 0 ? m.Metal : m.Coolant);
            }
            return new Vector3(Mathf.Max(1.4f, count * 0.65f), 2.5f, 0.8f);
        }

        private static Vector3 BuildScrubber(Transform root, FacilityKitMaterials m)
        {
            for (var i = -1; i <= 1; i++) AddCylinder($"Filter Vessel {i + 2}", root, new Vector3(i * 0.75f, 1.35f, 0f), new Vector3(0.48f, 1.35f, 0.48f), Quaternion.identity, m.Chemical);
            AddBox("Header Duct", root, new Vector3(0f, 2.65f, 0f), new Vector3(2.6f, 0.35f, 0.65f), m.Metal);
            return new Vector3(2.6f, 2.85f, 1f);
        }

        private static Vector3 BuildDuctBank(Transform root, FacilityKitMaterials m)
        {
            for (var i = 0; i < 3; i++) AddBox($"Duct {i + 1}", root, new Vector3(0f, 0.5f + i * 0.75f, 0f), new Vector3(2.4f, 0.55f, 0.8f), i == 1 ? m.Chemical : m.Metal);
            return new Vector3(2.4f, 2.3f, 0.8f);
        }

        private static Vector3 BuildCask(Transform root, FacilityKitMaterials m, int count)
        {
            for (var i = 0; i < count; i++)
            {
                AddCylinder($"Shielded Cask {i + 1}", root, new Vector3((i - (count - 1) * 0.5f) * 1.25f, 1f, 0f), new Vector3(0.55f, 1f, 0.55f), Quaternion.identity, m.Fuel);
                AddCylinder($"Cask Cap {i + 1}", root, new Vector3((i - (count - 1) * 0.5f) * 1.25f, 2.05f, 0f), new Vector3(0.62f, 0.08f, 0.62f), Quaternion.identity, m.DarkTrim);
            }
            return new Vector3(Mathf.Max(1.1f, count * 1.25f), 2.2f, 1.1f);
        }

        private static Vector3 BuildCrane(Transform root, FacilityKitMaterials m)
        {
            AddBox("Overhead Beam", root, new Vector3(0f, 3.25f, 0f), new Vector3(6f, 0.35f, 0.45f), m.Fuel);
            AddBox("Left Support", root, new Vector3(-2.7f, 1.65f, 0f), new Vector3(0.32f, 3.3f, 0.45f), m.Metal);
            AddBox("Right Support", root, new Vector3(2.7f, 1.65f, 0f), new Vector3(0.32f, 3.3f, 0.45f), m.Metal);
            AddBox("Hoist", root, new Vector3(0.8f, 2.7f, 0f), new Vector3(0.55f, 0.65f, 0.65f), m.DarkTrim, false);
            return new Vector3(6f, 3.45f, 0.7f);
        }

        private static Vector3 BuildLockers(Transform root, FacilityKitMaterials m)
        {
            for (var i = -1; i <= 1; i++) AddBox($"Locker {i + 2}", root, new Vector3(i * 0.7f, 1f, 0f), new Vector3(0.62f, 2f, 0.62f), m.Decon);
            return new Vector3(2.1f, 2f, 0.62f);
        }

        private static Vector3 BuildToolCage(Transform root, FacilityKitMaterials m)
        {
            AddBox("Cage Back", root, new Vector3(0f, 1.2f, 0.75f), new Vector3(2.8f, 2.4f, 0.10f), m.Metal);
            AddBox("Cage Left", root, new Vector3(-1.35f, 1.2f, 0f), new Vector3(0.10f, 2.4f, 1.5f), m.Metal);
            AddBox("Cage Right", root, new Vector3(1.35f, 1.2f, 0f), new Vector3(0.10f, 2.4f, 1.5f), m.Metal);
            AddBox("Cage Header", root, new Vector3(0f, 2.35f, 0f), new Vector3(2.8f, 0.10f, 1.5f), m.Fuel);
            return new Vector3(2.8f, 2.4f, 1.5f);
        }

        private static Vector3 BuildWorkbench(Transform root, FacilityKitMaterials m)
        {
            AddBox("Bench Top", root, new Vector3(0f, 0.95f, 0f), new Vector3(2.2f, 0.15f, 0.75f), m.Metal);
            AddBox("Backboard", root, new Vector3(0f, 1.55f, 0.32f), new Vector3(2.2f, 1.1f, 0.10f), m.Decon);
            return new Vector3(2.2f, 2.1f, 0.75f);
        }

        private static Vector3 BuildPartsCart(Transform root, FacilityKitMaterials m)
        {
            AddBox("Cart", root, new Vector3(0f, 0.65f, 0f), new Vector3(1.5f, 1.3f, 0.8f), m.Decon);
            for (var x = -1; x <= 1; x += 2) for (var z = -1; z <= 1; z += 2) AddCylinder("Wheel", root, new Vector3(x * 0.55f, 0.18f, z * 0.32f), new Vector3(0.16f, 0.08f, 0.16f), Quaternion.Euler(90f, 0f, 0f), m.DarkTrim);
            return new Vector3(1.5f, 1.3f, 0.8f);
        }

        private static Vector3 BuildPortableStorage(Transform root, FacilityKitMaterials m)
        {
            AddBox("Storage Rack", root, new Vector3(0f, 0.9f, 0f), new Vector3(1.8f, 1.8f, 0.65f), m.Decon);
            for (var i = -1; i <= 1; i++) AddCylinder($"Loose Metal {i + 2}", root, new Vector3(i * 0.45f, 1.15f, -0.4f), new Vector3(0.18f, 0.32f, 0.18f), Quaternion.Euler(0f, 0f, 90f), m.Metal, false);
            return new Vector3(1.8f, 1.8f, 0.8f);
        }

        private static Vector3 BuildDeconFrame(Transform root, FacilityKitMaterials m)
        {
            AddBox("Left Frame", root, new Vector3(-1.35f, 1.4f, 0f), new Vector3(0.22f, 2.8f, 0.65f), m.Decon);
            AddBox("Right Frame", root, new Vector3(1.35f, 1.4f, 0f), new Vector3(0.22f, 2.8f, 0.65f), m.Decon);
            AddBox("Top Frame", root, new Vector3(0f, 2.7f, 0f), new Vector3(2.9f, 0.22f, 0.65f), m.Decon);
            return new Vector3(2.9f, 2.8f, 0.65f);
        }

        private static Vector3 BuildIntake(Transform root, FacilityKitMaterials m)
        {
            AddBox("Intake Desk", root, new Vector3(0f, 0.55f, 0f), new Vector3(2.4f, 1.1f, 0.85f), m.Decon);
            AddBox("Checklist Panel", root, new Vector3(0.65f, 1.25f, 0.32f), new Vector3(0.75f, 0.85f, 0.08f), m.Amber, false);
            return new Vector3(2.4f, 1.7f, 0.85f);
        }

        private static Vector3 BuildExtraction(Transform root, FacilityKitMaterials m, int variant)
        {
            if (variant == 0)
            {
                AddBox("Vehicle Airlock Header", root, new Vector3(0f, 3.1f, 0f), new Vector3(5.5f, 0.4f, 0.55f), m.Decon);
                AddBox("Vehicle Airlock Left", root, new Vector3(-2.5f, 1.55f, 0f), new Vector3(0.5f, 3.1f, 0.55f), m.Decon);
                AddBox("Vehicle Airlock Right", root, new Vector3(2.5f, 1.55f, 0f), new Vector3(0.5f, 3.1f, 0.55f), m.Decon);
                return new Vector3(5.5f, 3.3f, 0.55f);
            }
            if (variant == 1)
            {
                AddBox("Lift Recess", root, new Vector3(0f, 1.35f, 0.2f), new Vector3(2.4f, 2.7f, 0.45f), m.DarkTrim);
                AddBox("Lift Door", root, new Vector3(0f, 1.25f, -0.08f), new Vector3(2.0f, 2.5f, 0.18f), m.Door);
                AddBox("Lift Indicator", root, new Vector3(0f, 2.45f, -0.2f), new Vector3(0.7f, 0.18f, 0.08f), m.Amber, false);
                return new Vector3(2.4f, 2.7f, 0.65f);
            }
            AddBox("Emergency Egress Frame", root, new Vector3(0f, 1.35f, 0f), new Vector3(2.4f, 2.7f, 0.45f), m.DarkTrim);
            AddBox("Emergency Door", root, new Vector3(0f, 1.25f, -0.25f), new Vector3(2.0f, 2.5f, 0.18f), m.Door);
            AddBox("Emergency Control", root, new Vector3(0.75f, 1.2f, -0.38f), new Vector3(0.28f, 0.55f, 0.10f), m.HotZone, false);
            return new Vector3(2.4f, 2.7f, 0.65f);
        }

        private static GameObject AddBox(string name, Transform parent, Vector3 position, Vector3 scale, Material material, bool collider = true)
        {
            return AddPrimitive(name, PrimitiveType.Cube, parent, position, scale, Quaternion.identity, material, collider);
        }

        private static GameObject AddCylinder(string name, Transform parent, Vector3 position, Vector3 scale, Quaternion rotation, Material material, bool collider = true)
        {
            return AddPrimitive(name, PrimitiveType.Cylinder, parent, position, scale, rotation, material, collider);
        }

        private static GameObject AddPrimitive(
            string name,
            PrimitiveType type,
            Transform parent,
            Vector3 position,
            Vector3 scale,
            Quaternion rotation,
            Material material,
            bool collider)
        {
            var value = GameObject.CreatePrimitive(type);
            value.name = name;
            value.transform.SetParent(parent, false);
            value.transform.localPosition = position;
            value.transform.localRotation = rotation;
            value.transform.localScale = scale;
            value.GetComponent<Renderer>().sharedMaterial = material;
            value.isStatic = collider;
            if (!collider)
            {
                UnityEngine.Object.DestroyImmediate(value.GetComponent<Collider>());
            }
            return value;
        }

        private static void BuildPaletteScene(FacilityKitMaterials materials)
        {
            EditorSceneManager.SaveOpenScenes();
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(FacilityKitCatalog.PaletteScenePath);
            var scene = sceneAsset != null
                ? EditorSceneManager.OpenScene(FacilityKitCatalog.PaletteScenePath, OpenSceneMode.Single)
                : EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            foreach (var existing in scene.GetRootGameObjects())
            {
                if (existing.name == PaletteRootName)
                {
                    UnityEngine.Object.DestroyImmediate(existing);
                }
            }

            var root = new GameObject(PaletteRootName);
            SceneManager.MoveGameObjectToScene(root, scene);
            var source = root.AddComponent<ApprovedFacilitySourceReference>();
            source.ConfigureFromAuthority();

            const int columns = 6;
            const float columnSpacing = 6.5f;
            const float rowSpacing = 5.5f;
            var categoryCounts = new Dictionary<FacilityKitCategory, int>();
            foreach (FacilityKitCategory category in Enum.GetValues(typeof(FacilityKitCategory))) categoryCounts[category] = 0;
            foreach (var entry in FacilityKitCatalog.Entries) categoryCounts[entry.Category]++;

            var categoryStartRows = new Dictionary<FacilityKitCategory, int>();
            var totalRows = 0;
            foreach (FacilityKitCategory category in Enum.GetValues(typeof(FacilityKitCategory)))
            {
                categoryStartRows[category] = totalRows;
                totalRows += Mathf.CeilToInt(categoryCounts[category] / (float)columns) + 1;
            }

            var floorLength = totalRows * rowSpacing + 10f;
            var floor = AddBox("Palette Floor", root.transform, new Vector3(0f, -0.12f, -floorLength * 0.5f + 3f), new Vector3(48f, 0.24f, floorLength), materials.Floor);
            floor.isStatic = true;

            var byCategory = new Dictionary<FacilityKitCategory, int>();
            foreach (FacilityKitCategory category in Enum.GetValues(typeof(FacilityKitCategory)))
            {
                byCategory[category] = 0;
                var categoryLabel = new GameObject($"CATEGORY {category}");
                categoryLabel.transform.SetParent(root.transform, false);
                categoryLabel.transform.position = new Vector3(-21f, 0f, -categoryStartRows[category] * rowSpacing);
                CreatePaletteLabel(categoryLabel.transform, category.ToString().ToUpperInvariant(), materials.Coolant);
            }
            foreach (var entry in FacilityKitCatalog.Entries)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(entry.PrefabPath);
                if (prefab == null) throw new InvalidOperationException($"FacilityKit prefab missing after generation: {entry.PrefabPath}");
                var index = byCategory[entry.Category]++;
                var instance = PrefabUtility.InstantiatePrefab(prefab, scene) as GameObject;
                if (instance == null) throw new InvalidOperationException($"Could not instantiate {entry.PrefabPath}");
                instance.transform.SetParent(root.transform, true);
                var row = categoryStartRows[entry.Category] + index / columns;
                var column = index % columns;
                instance.transform.position = new Vector3((column - 2.5f) * columnSpacing, 0f, -row * rowSpacing);
                instance.name = entry.StableId;
                CreatePaletteLabel(instance.transform, entry.StableId, materials.Amber);
            }

            var cameraObject = new GameObject("Palette Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.transform.SetParent(root.transform, false);
            var paletteCenter = new Vector3(0f, 0f, -floorLength * 0.5f + 3f);
            cameraObject.transform.position = paletteCenter + new Vector3(42f, 52f, 45f);
            cameraObject.transform.LookAt(paletteCenter);
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.GetComponent<Camera>();
            camera.fieldOfView = 48f;
            camera.farClipPlane = 250f;
            var lightObject = new GameObject("Palette Directional Light", typeof(Light));
            lightObject.transform.SetParent(root.transform, false);
            lightObject.transform.rotation = Quaternion.Euler(52f, -32f, 0f);
            var light = lightObject.GetComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, FacilityKitCatalog.PaletteScenePath);
            AssetDatabase.SaveAssets();
            Selection.activeGameObject = root;
        }

        private static void CreatePaletteLabel(Transform parent, string text, Material accent)
        {
            var backing = AddBox("Palette Label Backing", parent, new Vector3(0f, 0.8f, -1.3f), new Vector3(3.8f, 0.7f, 0.08f), accent, false);
            backing.transform.localRotation = Quaternion.identity;
            var labelObject = new GameObject("Palette Label", typeof(RectTransform), typeof(TextMeshPro));
            labelObject.transform.SetParent(parent, false);
            labelObject.transform.localPosition = new Vector3(0f, 0.82f, -1.35f);
            var rect = (RectTransform)labelObject.transform;
            rect.sizeDelta = new Vector2(3.5f, 0.55f);
            var label = labelObject.GetComponent<TextMeshPro>();
            label.text = text;
            label.fontSize = 0.44f;
            label.enableAutoSizing = true;
            label.fontSizeMin = 0.22f;
            label.fontSizeMax = 0.44f;
            label.fontStyle = FontStyles.Bold;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.black;
        }

        internal sealed class FacilityKitMaterials
        {
            public Material Wall;
            public Material Shield;
            public Material Glass;
            public Material Door;
            public Material Floor;
            public Material HotZone;
            public Material EditorReference;
            public Material Metal;
            public Material DarkTrim;
            public Material Decon;
            public Material Clinical;
            public Material Amber;
            public Material Coolant;
            public Material Chemical;
            public Material Fuel;
        }
    }
}
