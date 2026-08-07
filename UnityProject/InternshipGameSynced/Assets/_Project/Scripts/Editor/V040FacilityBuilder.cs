using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace InternshipGame.Prototype.Editor
{
    /// <summary>
    /// Durable authoring source for the v0.4.0.0 7C facility. Only the generated
    /// facility root is disposable; all coordinates and topology live here.
    /// </summary>
    internal static class V040FacilityBuilder
    {
        private const float WallHeight = 3.6f;
        private const float WallThickness = 0.22f;
        private const float PortalClearWidth = 2.76f;
        private const float PortalClearHeight = 2.52f;
        private const float PortalPostWidth = 0.22f;

        private static readonly FloorPadSpec[] FloorPads =
        {
            new("Extraction Annex", new Vector2(-26.5f, -1f), new Vector2(7.2f, 8f), SurfaceTheme.Extraction),
            new("Containment & Control", new Vector2(-14f, 4f), new Vector2(18.2f, 24f), SurfaceTheme.Containment),
            new("North Pipe Gallery", new Vector2(1f, 17f), new Vector2(18f, 3.2f), SurfaceTheme.North),
            new("North P02 Vestibule", new Vector2(-7.1f, 15.35f), new Vector2(3.2f, 3.7f), SurfaceTheme.North),
            new("Coolant & Maintenance", new Vector2(17.5f, 12.5f), new Vector2(19f, 19f), SurfaceTheme.Coolant),
            new("Utility Spine", new Vector2(18f, 0.25f), new Vector2(3.4f, 9f), SurfaceTheme.Utility),
            new("Fuel Handling & Waste", new Vector2(17f, -11f), new Vector2(20f, 14f), SurfaceTheme.Fuel),
            new("South Maintenance Vertical", new Vector2(-4.2f, -8.5f), new Vector2(3.6f, 7f), SurfaceTheme.South),
            new("South Maintenance Channel", new Vector2(1f, -12f), new Vector2(14f, 3.6f), SurfaceTheme.South),
            new("South P06 Vestibule", new Vector2(6.5f, -12f), new Vector2(3.6f, 3.6f), SurfaceTheme.South),
        };

        private static readonly PortalSpec[] Portals =
        {
            new("P01", "ANNEX / CONTAINMENT", new Vector3(-23f, 0f, -2f), 90f, SurfaceTheme.Extraction),
            new("P02", "CONTAINMENT / NORTH PIPE", new Vector3(-8f, 0f, 14.5f), 90f, SurfaceTheme.North),
            new("P03", "NORTH PIPE / COOLANT", new Vector3(8.5f, 0f, 18f), 90f, SurfaceTheme.North),
            new("P04", "COOLANT / UTILITY", new Vector3(18f, 0f, 4f), 0f, SurfaceTheme.Utility),
            new("P05", "UTILITY / FUEL", new Vector3(18f, 0f, -4f), 0f, SurfaceTheme.Utility),
            new("P06", "FUEL / SOUTH CHANNEL", new Vector3(6f, 0f, -12f), 90f, SurfaceTheme.South),
            new("P07", "SOUTH CHANNEL / CONTAINMENT", new Vector3(-5f, 0f, -5f), 90f, SurfaceTheme.South),
        };

        private static readonly SocketSpec[] Sockets =
        {
            new(RunConfiguration.C1, FacilityZoneId.Containment, new Vector3(-9f, 0f, 11.5f)),
            new(RunConfiguration.C2, FacilityZoneId.Containment, new Vector3(-19f, 0f, 14f)),
            new(RunConfiguration.C3, FacilityZoneId.Containment, new Vector3(-20f, 0f, 0f)),
            new(RunConfiguration.K1, FacilityZoneId.Coolant, new Vector3(22f, 0f, 16f)),
            new(RunConfiguration.K2, FacilityZoneId.Coolant, new Vector3(21f, 0f, 9f)),
            new(RunConfiguration.K3, FacilityZoneId.Coolant, new Vector3(11f, 0f, 14f)),
            new(RunConfiguration.F1, FacilityZoneId.FuelHandling, new Vector3(21f, 0f, -8f)),
            new(RunConfiguration.F2, FacilityZoneId.FuelHandling, new Vector3(21f, 0f, -13f)),
        };

        private static readonly PatrolSpec[] PatrolNodes =
        {
            new("ANN-01", new Vector3(-24.2f, 0.01f, 0.3f), OwlRouteNodeKind.Annex, FacilityRouteFamily.None),
            new("CON-01", new Vector3(-20.5f, 0.01f, -2.5f), OwlRouteNodeKind.Lobe, FacilityRouteFamily.None),
            new("CON-02", new Vector3(-20.5f, 0.01f, 10.5f), OwlRouteNodeKind.Lobe, FacilityRouteFamily.None),
            new("CON-03", new Vector3(-8.2f, 0.01f, 12.2f), OwlRouteNodeKind.Lobe, FacilityRouteFamily.None),
            new("NPG-01", new Vector3(-5.5f, 0.01f, 17f), OwlRouteNodeKind.Connector, FacilityRouteFamily.NorthPipeGallery),
            new("NPG-02", new Vector3(5.8f, 0.01f, 17f), OwlRouteNodeKind.Connector, FacilityRouteFamily.NorthPipeGallery),
            new("CLT-01", new Vector3(10.5f, 0.01f, 18f), OwlRouteNodeKind.Lobe, FacilityRouteFamily.None),
            new("CLT-02", new Vector3(14.2f, 0.01f, 7.2f), OwlRouteNodeKind.Lobe, FacilityRouteFamily.None),
            new("CLT-03", new Vector3(24f, 0.01f, 15.8f), OwlRouteNodeKind.Lobe, FacilityRouteFamily.None),
            new("UTL-01", new Vector3(18f, 0.01f, 2.8f), OwlRouteNodeKind.Connector, FacilityRouteFamily.UtilitySpine),
            new("UTL-02", new Vector3(18f, 0.01f, -2.5f), OwlRouteNodeKind.Connector, FacilityRouteFamily.UtilitySpine),
            new("FHW-01", new Vector3(23f, 0.01f, -6.2f), OwlRouteNodeKind.Lobe, FacilityRouteFamily.None),
            new("FHW-02", new Vector3(11f, 0.01f, -10.5f), OwlRouteNodeKind.Lobe, FacilityRouteFamily.None),
            new("FHW-03", new Vector3(23f, 0.01f, -15.8f), OwlRouteNodeKind.Lobe, FacilityRouteFamily.None),
            new("SMC-01", new Vector3(6.7f, 0.01f, -12f), OwlRouteNodeKind.Connector, FacilityRouteFamily.SouthMaintenanceChannel),
            new("SMC-02", new Vector3(-4.2f, 0.01f, -10.3f), OwlRouteNodeKind.Connector, FacilityRouteFamily.SouthMaintenanceChannel),
            new("SEARCH-01", new Vector3(-11f, 0.01f, -1.5f), OwlRouteNodeKind.SearchProbe, FacilityRouteFamily.None),
            new("SEARCH-02", new Vector3(12f, 0.01f, 11f), OwlRouteNodeKind.SearchProbe, FacilityRouteFamily.None),
        };

        public static ExpandedFacilityBuild Build(Transform parent, ExpandedFacilityMaterials materials)
        {
            var root = new GameObject("Generated Facility V040 7C");
            root.transform.SetParent(parent, false);

            BuildFloorsAndCeilings(root.transform, materials);
            BuildArchitecture(root.transform, materials);
            BuildLobeGrammars(root.transform, materials);

            var mapObject = new GameObject("Prototype Map Definition V040");
            mapObject.transform.SetParent(root.transform, false);
            var mapDefinition = mapObject.AddComponent<PrototypeMapDefinition>();

            var anchorsRoot = new GameObject("V040 Typed Anchors");
            anchorsRoot.transform.SetParent(root.transform, false);
            var playerSpawn = CreateAnchor("Player Spawn", anchorsRoot.transform, new Vector3(-28f, 0.06f, -2f));
            playerSpawn.rotation = Quaternion.Euler(0f, 90f, 0f);
            var rackAnchor = CreateAnchor("Extraction Rack Anchor", anchorsRoot.transform, new Vector3(-24.5f, 0f, -2f));
            rackAnchor.rotation = Quaternion.Euler(0f, 90f, 0f);
            var exitAnchor = CreateAnchor("Extraction Exit Anchor", anchorsRoot.transform, new Vector3(-30f, 0f, -2f));
            exitAnchor.rotation = Quaternion.Euler(0f, 90f, 0f);
            var owlSpawn = CreateAnchor("Owl Spawn Anchor", anchorsRoot.transform, new Vector3(18f, 0.01f, -2f));
            var missionDisplayAnchor = CreateAnchor("Mission Display Anchor", anchorsRoot.transform, new Vector3(-24.4f, 2.72f, -2f));
            missionDisplayAnchor.rotation = Quaternion.Euler(0f, 90f, 0f);

            var sockets = BuildRodSockets(anchorsRoot.transform, materials);
            var patrolPoints = BuildPatrolPoints(anchorsRoot.transform);
            var variations = BuildRouteVariations(root.transform, materials);
            var lighting = BuildLightingVariants(root.transform, materials);
            var zoneTriggers = BuildZoneTriggers(root.transform, mapDefinition);

            mapDefinition.Configure(
                playerSpawn,
                rackAnchor,
                exitAnchor,
                owlSpawn,
                sockets,
                patrolPoints,
                variations,
                lighting,
                zoneTriggers);
            mapDefinition.ApplyAuthoredVariation(
                PrototypeVariationIds.OpenCircuit,
                PrototypeVariationIds.LightingStable);

            BuildExtractionGate(root.transform, exitAnchor.position, materials);
            return new ExpandedFacilityBuild
            {
                Root = root,
                MapDefinition = mapDefinition,
                MissionDisplayAnchor = missionDisplayAnchor,
            };
        }

        private static void BuildFloorsAndCeilings(Transform parent, ExpandedFacilityMaterials materials)
        {
            var floorRoot = new GameObject("7C Navigable Floor Pads");
            floorRoot.transform.SetParent(parent, false);
            foreach (var pad in FloorPads)
            {
                var floor = CreateBox(
                    "Continuous Navigable Floor",
                    floorRoot.transform,
                    new Vector3(pad.Center.x, -0.12f, pad.Center.y),
                    new Vector3(pad.Size.x, 0.24f, pad.Size.y),
                    materials.BaseFloor,
                    true);
                floor.AddComponent<TeleportationArea>();
                CreateBox(
                    $"{pad.Name} Route Inlay",
                    floorRoot.transform,
                    new Vector3(pad.Center.x, 0.012f, pad.Center.y),
                    new Vector3(Mathf.Max(0.2f, pad.Size.x - 0.25f), 0.018f, Mathf.Min(0.55f, pad.Size.y - 0.2f)),
                    ResolveTheme(pad.Theme, materials),
                    false);
            }

            var ceilingRoot = new GameObject("Asymmetric Ceiling Plates");
            ceilingRoot.transform.SetParent(parent, false);
            foreach (var pad in FloorPads)
            {
                CreateBox(
                    $"{pad.Name} Ceiling",
                    ceilingRoot.transform,
                    new Vector3(pad.Center.x, WallHeight + 0.15f, pad.Center.y),
                    new Vector3(pad.Size.x, 0.2f, pad.Size.y),
                    materials.Wall,
                    true);
            }
        }

        private static void BuildArchitecture(Transform parent, ExpandedFacilityMaterials materials)
        {
            var architecture = new GameObject("7C Shell And Portal Architecture");
            architecture.transform.SetParent(parent, false);

            // Approximate asymmetric outer shell, kept inside the documented footprint.
            Wall("Annex West", architecture.transform, new Vector3(-30.4f, 1.8f, -1f), new Vector3(WallThickness, WallHeight, 8.4f), materials.Wall);
            Wall("Annex North", architecture.transform, new Vector3(-26.5f, 1.8f, 3.1f), new Vector3(7.8f, WallHeight, WallThickness), materials.Wall);
            Wall("Annex South", architecture.transform, new Vector3(-26.5f, 1.8f, -5.1f), new Vector3(7.8f, WallHeight, WallThickness), materials.Wall);
            BuildVerticalWallWithOpening("P01 Partition", architecture.transform, -23f, -5f, 3f, -2f, materials.Wall);

            Wall("Containment West North", architecture.transform, new Vector3(-23.1f, 1.8f, 9.5f), new Vector3(WallThickness, WallHeight, 13f), materials.Wall);
            Wall("Containment South West", architecture.transform, new Vector3(-14.5f, 1.8f, -8.1f), new Vector3(17f, WallHeight, WallThickness), materials.Wall);
            Wall("North Pipe North", architecture.transform, new Vector3(0.15f, 1.8f, 18.72f), new Vector3(16.3f, WallHeight, WallThickness), materials.Wall);
            Wall("North Pipe South", architecture.transform, new Vector3(1f, 1.8f, 15.28f), new Vector3(11.8f, WallHeight, WallThickness), materials.Wall);
            BuildVerticalWallWithOpening("P03 Partition", architecture.transform, 8.5f, 15.3f, 18.7f, 18f, materials.Wall);

            Wall("Coolant North", architecture.transform, new Vector3(17.5f, 1.8f, 22.1f), new Vector3(19f, WallHeight, WallThickness), materials.Wall);
            Wall("Facility East", architecture.transform, new Vector3(27.1f, 1.8f, 2f), new Vector3(WallThickness, WallHeight, 40f), materials.Wall);
            BuildHorizontalWallWithOpening("P04 Partition", architecture.transform, 4f, 16.2f, 23f, 18f, materials.Wall);
            BuildHorizontalWallWithOpening("P05 Partition", architecture.transform, -4f, 16.2f, 23f, 18f, materials.Wall);
            Wall("Fuel South", architecture.transform, new Vector3(17f, 1.8f, -18.1f), new Vector3(20f, WallHeight, WallThickness), materials.Wall);
            BuildVerticalWallWithOpening("P06 Partition", architecture.transform, 6f, -15f, -4f, -12f, materials.Wall);
            Wall("South Channel South", architecture.transform, new Vector3(0.5f, 1.8f, -13.92f), new Vector3(13f, WallHeight, WallThickness), materials.Wall);
            Wall("South Channel West", architecture.transform, new Vector3(-6.12f, 1.8f, -9.5f), new Vector3(WallThickness, WallHeight, 8.8f), materials.Wall);
            BuildVerticalWallWithOpening("P07 Partition", architecture.transform, -5f, -8f, -4f, -5f, materials.Wall);

            BuildShieldingVoid(architecture.transform, materials);
            foreach (var portal in Portals)
            {
                BuildPortal(portal, architecture.transform, materials);
            }
        }

        private static void BuildShieldingVoid(Transform parent, ExpandedFacilityMaterials materials)
        {
            var root = new GameObject("INACCESSIBLE CENTRAL SHIELDING SERVICE VOID");
            root.transform.SetParent(parent, false);
            CreateBox("Primary Shielding Mass", root.transform, new Vector3(1.5f, 1.8f, 5.5f), new Vector3(11.5f, WallHeight, 12.5f), materials.DarkTrim, true);
            CreateBox("South Shielding Spur", root.transform, new Vector3(1f, 1.8f, -2f), new Vector3(10.5f, WallHeight, 3f), materials.DarkTrim, true);
            var angled = CreateBox("Angled Service Mass", root.transform, new Vector3(-1.2f, 1.8f, 12.4f), new Vector3(6.2f, WallHeight, 2.4f), materials.DarkTrim, true);
            angled.transform.rotation = Quaternion.Euler(0f, -12f, 0f);
            CreateSign(root.transform, new Vector3(-4.3f, 2.25f, 5f), Quaternion.Euler(0f, 90f, 0f), "NO ACCESS\nSHIELDING / SERVICE VOID", materials.DangerRed, materials);
        }

        private static void BuildLobeGrammars(Transform parent, ExpandedFacilityMaterials materials)
        {
            var root = new GameObject("Three Lobe Spatial Grammars");
            root.transform.SetParent(parent, false);

            var containment = new GameObject("Containment & Control Grammar");
            containment.transform.SetParent(root.transform, false);
            CreateCylinder("Reactor Drum Landmark", containment.transform, new Vector3(-15f, 1.4f, 6f), new Vector3(10.7f, 1.4f, 10.7f), materials.Metal, true);
            Shield(containment.transform, "Containment Shield A", new Vector3(-21f, 1.4f, 12.2f), new Vector3(4.9f, 2.8f, 0.8f), 20f, materials.DarkTrim);
            Shield(containment.transform, "Containment Shield B", new Vector3(-8.3f, 1.4f, 6.5f), new Vector3(4.2f, 2.8f, 0.8f), 90f, materials.DarkTrim);
            Shield(containment.transform, "Containment Shield C", new Vector3(-16.2f, 1.4f, -4.4f), new Vector3(5.8f, 2.8f, 0.8f), -15f, materials.DarkTrim);
            // The plan intentionally overlaps Shield D's visual footprint with P02.
            // Keep the authored silhouette, but cut its physical/vision collider back
            // to the solid west segment so the exact 2.76 m portal remains traversable.
            ShieldWithPortalCut(containment.transform, materials.DarkTrim);
            CreateSign(containment.transform, new Vector3(-21.8f, 2.55f, 5f), Quaternion.Euler(0f, 90f, 0f), "CONTAINMENT & CONTROL\nREACTOR C-04", materials.Cyan, materials);

            var coolant = new GameObject("Coolant & Maintenance Grammar");
            coolant.transform.SetParent(root.transform, false);
            CreateCylinder("Coolant Basin Landmark", coolant.transform, new Vector3(17f, 0.5f, 12f), new Vector3(6.4f, 0.5f, 6.4f), materials.Cyan, true);
            Shield(coolant.transform, "Pump Cluster West", new Vector3(11.5f, 1.25f, 10.7f), new Vector3(3.6f, 2.5f, 1.5f), -20f, materials.Metal);
            Shield(coolant.transform, "Pump Cluster East", new Vector3(21.5f, 1.25f, 10.1f), new Vector3(3.6f, 2.5f, 1.5f), 18f, materials.Metal);
            Shield(coolant.transform, "Pipe Manifold", new Vector3(13.3f, 1.3f, 18f), new Vector3(4.1f, 2.6f, 1.4f), 0f, materials.DarkTrim);
            CreateSign(coolant.transform, new Vector3(25.7f, 2.55f, 12.5f), Quaternion.Euler(0f, -90f, 0f), "COOLANT & MAINTENANCE\nPUMP DECK K-12", materials.Amber, materials);

            var fuel = new GameObject("Fuel Handling & Waste Grammar");
            fuel.transform.SetParent(root.transform, false);
            Shield(fuel.transform, "Cask Row North", new Vector3(13f, 1.15f, -7.2f), new Vector3(5.8f, 2.3f, 1.7f), 0f, materials.Metal);
            Shield(fuel.transform, "Cask Row Mid", new Vector3(17f, 1.15f, -10.8f), new Vector3(6.7f, 2.3f, 1.7f), 0f, materials.Metal);
            Shield(fuel.transform, "Cask Row South", new Vector3(13f, 1.15f, -14.8f), new Vector3(5.8f, 2.3f, 1.7f), 0f, materials.Metal);
            FuelShieldWithCrosscuts(fuel.transform, materials.DarkTrim);
            Shield(fuel.transform, "Hot Wall Landmark", new Vector3(24.5f, 1.55f, -11f), new Vector3(3f, 3.1f, 10f), 0f, materials.DangerRed);
            CreateSign(fuel.transform, new Vector3(25.8f, 2.55f, -6f), Quaternion.Euler(0f, -90f, 0f), "FUEL HANDLING & WASTE\nHOT CELL F-07", materials.RadiationGreen, materials);
        }

        private static RodSocketAnchor[] BuildRodSockets(Transform parent, ExpandedFacilityMaterials materials)
        {
            var root = new GameObject("Eight Authored Pass-Through Rod Sockets");
            root.transform.SetParent(parent, false);
            var result = new RodSocketAnchor[Sockets.Length];
            for (var i = 0; i < Sockets.Length; i++)
            {
                var spec = Sockets[i];
                CreateCylinder($"{spec.Id} Pedestal", root.transform, spec.Position + Vector3.up * 0.11f, new Vector3(0.56f, 0.11f, 0.56f), materials.RadiationGreen, true);
                var point = CreateAnchor($"{spec.Id} Spawn Point", root.transform, spec.Position + Vector3.up * 1.05f);
                var socket = point.gameObject.AddComponent<RodSocketAnchor>();
                socket.Configure(spec.Id, spec.Zone, point);
                result[i] = socket;
                CreateFloorLabel(root.transform, spec.Position + new Vector3(0f, 0.045f, -0.72f), spec.Id, materials.RadiationGreen.color);
            }

            return result;
        }

        private static Transform[] BuildPatrolPoints(Transform parent)
        {
            var root = new GameObject("V040 Owl Patrol Topology 18 Nodes");
            root.transform.SetParent(parent, false);
            var result = new Transform[PatrolNodes.Length];
            for (var i = 0; i < PatrolNodes.Length; i++)
            {
                var spec = PatrolNodes[i];
                var point = CreateAnchor($"Patrol {i + 1:00} {spec.Id}", root.transform, spec.Position);
                point.gameObject.AddComponent<OwlRouteNodeAnchor>().Configure(spec.Id, spec.Kind, spec.RouteFamily);
                result[i] = point;
            }

            return result;
        }

        private static RouteVariationAnchor[] BuildRouteVariations(Transform parent, ExpandedFacilityMaterials materials)
        {
            var root = new GameObject("V040 Local Route Profiles");
            root.transform.SetParent(parent, false);
            var specs = new[]
            {
                new VariationSpec(PrototypeVariationIds.OpenCircuit, Vector3.zero, Vector3.zero, "OPEN CIRCUIT"),
                new VariationSpec(PrototypeVariationIds.CoolantService, new Vector3(13.4f, 1.05f, 15.2f), new Vector3(2.3f, 2.1f, 0.45f), "COOLANT SERVICE"),
                new VariationSpec(PrototypeVariationIds.FuelInspection, new Vector3(15.5f, 1.05f, -6.2f), new Vector3(0.45f, 2.1f, 2.4f), "FUEL INSPECTION"),
            };
            var result = new RouteVariationAnchor[specs.Length];
            for (var i = 0; i < specs.Length; i++)
            {
                var spec = specs[i];
                var profileRoot = new GameObject(spec.Id);
                profileRoot.transform.SetParent(root.transform, false);
                if (spec.Scale.sqrMagnitude > 0f)
                {
                    var blocker = CreateBox("Local Secondary Branch Blocker", profileRoot.transform, spec.Position, spec.Scale, materials.DangerRed, true);
                    blocker.isStatic = false;
                    var obstacle = blocker.AddComponent<NavMeshObstacle>();
                    obstacle.shape = NavMeshObstacleShape.Box;
                    obstacle.carving = true;
                    obstacle.carveOnlyStationary = true;
                    CreateSign(profileRoot.transform, spec.Position + Vector3.up * 1.45f, Quaternion.identity, $"LOCAL WORK\n{spec.Label}", materials.DangerRed, materials);
                }

                var anchor = profileRoot.AddComponent<RouteVariationAnchor>();
                anchor.Configure(spec.Id, profileRoot);
                result[i] = anchor;
            }

            return result;
        }

        private static LightingVariantAnchor[] BuildLightingVariants(Transform parent, ExpandedFacilityMaterials materials)
        {
            var root = new GameObject("V040 Lighting Profiles");
            root.transform.SetParent(parent, false);
            var ids = new[]
            {
                PrototypeVariationIds.LightingStable,
                PrototypeVariationIds.LightingEmergency,
                PrototypeVariationIds.LightingPartialOutage,
            };
            var lightPositions = new[]
            {
                new Vector3(-26.5f, 2.9f, -1f), new Vector3(-15f, 2.9f, 5f),
                new Vector3(1f, 2.9f, 17f), new Vector3(17.5f, 2.9f, 13f),
                new Vector3(18f, 2.9f, 0f), new Vector3(17f, 2.9f, -11f),
                new Vector3(-3.8f, 2.9f, -10f),
            };
            var result = new LightingVariantAnchor[ids.Length];
            for (var profileIndex = 0; profileIndex < ids.Length; profileIndex++)
            {
                var profileRoot = new GameObject(ids[profileIndex]);
                profileRoot.transform.SetParent(root.transform, false);
                for (var i = 0; i < lightPositions.Length; i++)
                {
                    var lightObject = new GameObject($"7C Light {i + 1:00}");
                    lightObject.transform.SetParent(profileRoot.transform, false);
                    lightObject.transform.position = lightPositions[i];
                    var light = lightObject.AddComponent<Light>();
                    light.type = LightType.Point;
                    light.range = 12f;
                    light.shadows = LightShadows.Soft;
                    light.color = profileIndex == 1 && i % 2 == 0
                        ? materials.DangerRed.color
                        : i is 0 or 2 or 4 ? materials.Cyan.color : materials.NeutralLight.color;
                    light.intensity = profileIndex == 2 && i % 3 == 0 ? 0.65f : 3.1f;
                }

                var anchor = profileRoot.AddComponent<LightingVariantAnchor>();
                anchor.Configure(ids[profileIndex], profileRoot);
                result[profileIndex] = anchor;
            }

            return result;
        }

        private static FacilityZoneTrigger[] BuildZoneTriggers(Transform parent, PrototypeMapDefinition definition)
        {
            var root = new GameObject("V040 Lobe And Connector Zone Tracking");
            root.transform.SetParent(parent, false);
            var specs = new[]
            {
                new ZoneSpec("Annex", FacilityZoneId.Annex, new Vector2(-26.5f, -1f), new Vector2(7f, 8f)),
                new ZoneSpec("Containment", FacilityZoneId.Containment, new Vector2(-14.5f, 4f), new Vector2(16.5f, 22f)),
                new ZoneSpec("North Pipe Gallery", FacilityZoneId.NorthPipeGallery, new Vector2(1f, 17f), new Vector2(18f, 3.2f)),
                new ZoneSpec("Coolant", FacilityZoneId.Coolant, new Vector2(18f, 12.5f), new Vector2(17f, 17f)),
                new ZoneSpec("Utility Spine", FacilityZoneId.UtilitySpine, new Vector2(18f, 0f), new Vector2(3.4f, 8f)),
                new ZoneSpec("Fuel Handling", FacilityZoneId.FuelHandling, new Vector2(17f, -11f), new Vector2(19f, 13f)),
                new ZoneSpec("South Channel Vertical", FacilityZoneId.SouthMaintenanceChannel, new Vector2(-4.2f, -8.5f), new Vector2(3.6f, 7f)),
                new ZoneSpec("South Maintenance Channel", FacilityZoneId.SouthMaintenanceChannel, new Vector2(1f, -12f), new Vector2(14f, 3.6f)),
            };
            var result = new FacilityZoneTrigger[specs.Length];
            for (var i = 0; i < specs.Length; i++)
            {
                var spec = specs[i];
                var triggerObject = new GameObject($"{spec.Name} Zone");
                triggerObject.transform.SetParent(root.transform, false);
                triggerObject.transform.position = new Vector3(spec.Center.x, 1.5f, spec.Center.y);
                var collider = triggerObject.AddComponent<BoxCollider>();
                collider.isTrigger = true;
                collider.size = new Vector3(spec.Size.x, 3f, spec.Size.y);
                var trigger = triggerObject.AddComponent<FacilityZoneTrigger>();
                trigger.Configure(spec.Zone, definition);
                result[i] = trigger;
            }

            return result;
        }

        private static void BuildPortal(PortalSpec spec, Transform parent, ExpandedFacilityMaterials materials)
        {
            var root = new GameObject($"{spec.Id} Exact Portal 2.76x2.52");
            root.transform.SetParent(parent, false);
            root.transform.SetPositionAndRotation(spec.Position, Quaternion.Euler(0f, spec.Yaw, 0f));
            var postX = PortalClearWidth * 0.5f + PortalPostWidth * 0.5f;
            var frameWidth = PortalClearWidth + PortalPostWidth * 2f;
            CreateBoxLocal("Left Jamb", root.transform, new Vector3(-postX, PortalClearHeight * 0.5f, 0f), new Vector3(PortalPostWidth, PortalClearHeight, 0.32f), materials.DarkTrim, true);
            CreateBoxLocal("Right Jamb", root.transform, new Vector3(postX, PortalClearHeight * 0.5f, 0f), new Vector3(PortalPostWidth, PortalClearHeight, 0.32f), materials.DarkTrim, true);
            CreateBoxLocal("Header", root.transform, new Vector3(0f, PortalClearHeight + 0.12f, 0f), new Vector3(frameWidth, 0.24f, 0.32f), materials.DarkTrim, true);
            CreateBoxLocal("Clear Width Threshold", root.transform, new Vector3(0f, 0.012f, 0f), new Vector3(PortalClearWidth, 0.024f, 0.5f), ResolveTheme(spec.Theme, materials), false);
            CreateText(root.transform, new Vector3(0f, PortalClearHeight + 0.13f, -0.18f), Quaternion.identity, $"{spec.Id}  {spec.Label}", new Vector2(2.9f, 0.25f), 0.42f, Color.white);
            CreateText(root.transform, new Vector3(0f, PortalClearHeight + 0.13f, 0.18f), Quaternion.Euler(0f, 180f, 0f), $"{spec.Id}  {spec.Label}", new Vector2(2.9f, 0.25f), 0.42f, Color.white);
        }

        private static void BuildExtractionGate(Transform parent, Vector3 exitPosition, ExpandedFacilityMaterials materials)
        {
            var root = new GameObject("Extraction Gate Presentation");
            root.transform.SetParent(parent, false);
            CreateBox("Gate North", root.transform, exitPosition + new Vector3(0f, 1.4f, 2.1f), new Vector3(0.2f, 2.8f, 0.18f), materials.Cyan, true);
            CreateBox("Gate South", root.transform, exitPosition + new Vector3(0f, 1.4f, -2.1f), new Vector3(0.2f, 2.8f, 0.18f), materials.Cyan, true);
            CreateBox("Gate Header", root.transform, exitPosition + new Vector3(0f, 2.75f, 0f), new Vector3(0.2f, 0.18f, 4.4f), materials.Cyan, true);
        }

        private static void BuildVerticalWallWithOpening(string name, Transform parent, float x, float minZ, float maxZ, float openingZ, Material material)
        {
            var half = PortalClearWidth * 0.5f + PortalPostWidth;
            var lowerLength = openingZ - half - minZ;
            var upperLength = maxZ - (openingZ + half);
            if (lowerLength > 0.05f)
            {
                Wall($"{name} South", parent, new Vector3(x, WallHeight * 0.5f, minZ + lowerLength * 0.5f), new Vector3(WallThickness, WallHeight, lowerLength), material);
            }
            if (upperLength > 0.05f)
            {
                Wall($"{name} North", parent, new Vector3(x, WallHeight * 0.5f, openingZ + half + upperLength * 0.5f), new Vector3(WallThickness, WallHeight, upperLength), material);
            }
        }

        private static void BuildHorizontalWallWithOpening(string name, Transform parent, float z, float minX, float maxX, float openingX, Material material)
        {
            var half = PortalClearWidth * 0.5f + PortalPostWidth;
            var leftLength = openingX - half - minX;
            var rightLength = maxX - (openingX + half);
            if (leftLength > 0.05f)
            {
                Wall($"{name} West", parent, new Vector3(minX + leftLength * 0.5f, WallHeight * 0.5f, z), new Vector3(leftLength, WallHeight, WallThickness), material);
            }
            if (rightLength > 0.05f)
            {
                Wall($"{name} East", parent, new Vector3(openingX + half + rightLength * 0.5f, WallHeight * 0.5f, z), new Vector3(rightLength, WallHeight, WallThickness), material);
            }
        }

        private static void Wall(string name, Transform parent, Vector3 position, Vector3 scale, Material material)
        {
            CreateBox(name, parent, position, scale, material, true);
        }

        private static void Shield(Transform parent, string name, Vector3 position, Vector3 scale, float yaw, Material material)
        {
            var shield = CreateBox(name, parent, position, scale, material, true);
            shield.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        }

        private static void ShieldWithPortalCut(Transform parent, Material material)
        {
            var visual = CreateBox(
                "Containment Shield D",
                parent,
                new Vector3(-9.2f, 1.4f, 14.3f),
                new Vector3(3.9f, 2.8f, 1.5f),
                material,
                false);
            visual.transform.rotation = Quaternion.Euler(0f, -15f, 0f);
            var solid = CreateBox(
                "Containment Shield D Solid West Segment",
                parent,
                new Vector3(-10.65f, 1.4f, 14.55f),
                new Vector3(1.05f, 2.8f, 1.5f),
                material,
                true);
            solid.transform.rotation = Quaternion.Euler(0f, -15f, 0f);
        }

        private static void FuelShieldWithCrosscuts(Transform parent, Material material)
        {
            // Preserve the locked 0.8 x 11.8 m silhouette while authoring two
            // traversable crosscuts through its collision/vision mass. The objective
            // pockets east of the wall therefore remain pass-through, never terminals.
            CreateBox(
                "Fuel Shield Wall",
                parent,
                new Vector3(18.8f, 1.45f, -11f),
                new Vector3(0.8f, 2.9f, 11.8f),
                material,
                false);
            CreateBox("Fuel Shield North Solid", parent, new Vector3(18.8f, 1.45f, -6.2f), new Vector3(0.8f, 2.9f, 2.2f), material, true);
            CreateBox("Fuel Shield Mid Solid", parent, new Vector3(18.8f, 1.45f, -10.5f), new Vector3(0.8f, 2.9f, 3f), material, true);
            CreateBox("Fuel Shield South Solid", parent, new Vector3(18.8f, 1.45f, -15.45f), new Vector3(0.8f, 2.9f, 2.9f), material, true);
        }

        private static void CreateSign(Transform parent, Vector3 position, Quaternion rotation, string label, Material accent, ExpandedFacilityMaterials materials)
        {
            var root = new GameObject($"Sign {label.Replace('\n', ' ')}");
            root.transform.SetParent(parent, false);
            root.transform.SetPositionAndRotation(position, rotation);
            CreateBoxLocal("Backing", root.transform, Vector3.zero, new Vector3(3.2f, 0.72f, 0.08f), materials.Panel, false);
            CreateBoxLocal("Accent", root.transform, new Vector3(0f, -0.31f, -0.05f), new Vector3(2.9f, 0.08f, 0.03f), accent, false);
            CreateText(root.transform, new Vector3(0f, 0.03f, -0.06f), Quaternion.identity, label, new Vector2(2.9f, 0.56f), 0.62f, Color.white);
        }

        private static TMP_Text CreateText(Transform parent, Vector3 localPosition, Quaternion localRotation, string text, Vector2 size, float fontSize, Color color)
        {
            var labelObject = new GameObject("Fitted Text", typeof(RectTransform), typeof(TextMeshPro));
            labelObject.transform.SetParent(parent, false);
            labelObject.transform.localPosition = localPosition;
            labelObject.transform.localRotation = localRotation;
            var rect = (RectTransform)labelObject.transform;
            rect.sizeDelta = size;
            var label = labelObject.GetComponent<TextMeshPro>();
            label.text = text;
            label.fontSize = fontSize;
            label.enableAutoSizing = true;
            label.fontSizeMin = 0.28f;
            label.fontSizeMax = fontSize;
            label.fontStyle = FontStyles.Bold;
            label.alignment = TextAlignmentOptions.Center;
            label.color = color;
            label.textWrappingMode = TextWrappingModes.Normal;
            return label;
        }

        private static void CreateFloorLabel(Transform parent, Vector3 position, string text, Color color)
        {
            var label = CreateText(parent, position, Quaternion.Euler(90f, 0f, 0f), text, new Vector2(1.2f, 0.4f), 0.72f, color);
            label.gameObject.name = $"Socket {text} Floor Stencil";
        }

        private static Transform CreateAnchor(string name, Transform parent, Vector3 position)
        {
            var anchor = new GameObject(name).transform;
            anchor.SetParent(parent);
            anchor.position = position;
            return anchor;
        }

        private static GameObject CreateBox(string name, Transform parent, Vector3 position, Vector3 scale, Material material, bool collider)
        {
            var value = GameObject.CreatePrimitive(PrimitiveType.Cube);
            value.name = name;
            value.transform.SetParent(parent);
            value.transform.SetPositionAndRotation(position, Quaternion.identity);
            value.transform.localScale = scale;
            value.GetComponent<Renderer>().sharedMaterial = material;
            value.isStatic = collider;
            if (!collider)
            {
                UnityEngine.Object.DestroyImmediate(value.GetComponent<Collider>());
            }
            return value;
        }

        private static GameObject CreateBoxLocal(string name, Transform parent, Vector3 position, Vector3 scale, Material material, bool collider)
        {
            var value = CreateBox(name, parent, Vector3.zero, scale, material, collider);
            value.transform.localPosition = position;
            value.transform.localRotation = Quaternion.identity;
            return value;
        }

        private static GameObject CreateCylinder(string name, Transform parent, Vector3 position, Vector3 scale, Material material, bool collider)
        {
            var value = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            value.name = name;
            value.transform.SetParent(parent);
            value.transform.SetPositionAndRotation(position, Quaternion.identity);
            value.transform.localScale = scale;
            value.GetComponent<Renderer>().sharedMaterial = material;
            value.isStatic = collider;
            if (!collider)
            {
                UnityEngine.Object.DestroyImmediate(value.GetComponent<Collider>());
            }
            return value;
        }

        private static Material ResolveTheme(SurfaceTheme theme, ExpandedFacilityMaterials materials)
        {
            return theme switch
            {
                SurfaceTheme.Extraction => materials.Cyan,
                SurfaceTheme.Containment => materials.Cyan,
                SurfaceTheme.North => materials.Amber,
                SurfaceTheme.Coolant => materials.Amber,
                SurfaceTheme.Utility => materials.NeutralLight,
                SurfaceTheme.Fuel => materials.RadiationGreen,
                SurfaceTheme.South => materials.RadiationGreen,
                _ => materials.NeutralLight,
            };
        }

        private enum SurfaceTheme { Extraction, Containment, North, Coolant, Utility, Fuel, South }

        private readonly struct FloorPadSpec
        {
            public FloorPadSpec(string name, Vector2 center, Vector2 size, SurfaceTheme theme)
            {
                Name = name; Center = center; Size = size; Theme = theme;
            }
            public string Name { get; }
            public Vector2 Center { get; }
            public Vector2 Size { get; }
            public SurfaceTheme Theme { get; }
        }

        private readonly struct PortalSpec
        {
            public PortalSpec(string id, string label, Vector3 position, float yaw, SurfaceTheme theme)
            {
                Id = id; Label = label; Position = position; Yaw = yaw; Theme = theme;
            }
            public string Id { get; }
            public string Label { get; }
            public Vector3 Position { get; }
            public float Yaw { get; }
            public SurfaceTheme Theme { get; }
        }

        private readonly struct SocketSpec
        {
            public SocketSpec(string id, FacilityZoneId zone, Vector3 position)
            {
                Id = id; Zone = zone; Position = position;
            }
            public string Id { get; }
            public FacilityZoneId Zone { get; }
            public Vector3 Position { get; }
        }

        private readonly struct PatrolSpec
        {
            public PatrolSpec(string id, Vector3 position, OwlRouteNodeKind kind, FacilityRouteFamily routeFamily)
            {
                Id = id; Position = position; Kind = kind; RouteFamily = routeFamily;
            }
            public string Id { get; }
            public Vector3 Position { get; }
            public OwlRouteNodeKind Kind { get; }
            public FacilityRouteFamily RouteFamily { get; }
        }

        private readonly struct VariationSpec
        {
            public VariationSpec(string id, Vector3 position, Vector3 scale, string label)
            {
                Id = id; Position = position; Scale = scale; Label = label;
            }
            public string Id { get; }
            public Vector3 Position { get; }
            public Vector3 Scale { get; }
            public string Label { get; }
        }

        private readonly struct ZoneSpec
        {
            public ZoneSpec(string name, FacilityZoneId zone, Vector2 center, Vector2 size)
            {
                Name = name; Zone = zone; Center = center; Size = size;
            }
            public string Name { get; }
            public FacilityZoneId Zone { get; }
            public Vector2 Center { get; }
            public Vector2 Size { get; }
        }
    }
}
