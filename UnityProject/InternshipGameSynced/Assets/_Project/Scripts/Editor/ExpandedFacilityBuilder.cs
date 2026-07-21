using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace InternshipGame.Prototype.Editor
{
    internal sealed class ExpandedFacilityMaterials
    {
        public Material BaseFloor;
        public Material Wall;
        public Material Metal;
        public Material Cyan;
        public Material Amber;
        public Material RadiationGreen;
        public Material DangerRed;
        public Material NeutralLight;
    }

    internal sealed class ExpandedFacilityBuild
    {
        public GameObject Root;
        public PrototypeMapDefinition MapDefinition;
        public Transform MissionDisplayAnchor;
    }

    internal static class ExpandedFacilityBuilder
    {
        private const float WallHeight = 3.6f;

        private static readonly SpaceSpec[] Spaces =
        {
            new("Entry and Extraction Bay", FacilityZoneId.EntryExtraction, new Vector2(0f, -25f), new Vector2(12f, 8f), SurfaceTheme.Extraction),
            new("Decontamination", FacilityZoneId.Decontamination, new Vector2(0f, -18f), new Vector2(6f, 6f), SurfaceTheme.Neutral),
            new("Operations Hub", FacilityZoneId.OperationsHub, new Vector2(0f, -10f), new Vector2(14f, 10f), SurfaceTheme.Extraction),
            new("West Maintenance", FacilityZoneId.WestMaintenance, new Vector2(-11f, 0f), new Vector2(10f, 16f), SurfaceTheme.Maintenance),
            new("Central Transfer", FacilityZoneId.CentralTransfer, new Vector2(0f, 1f), new Vector2(6f, 18f), SurfaceTheme.Neutral),
            new("East Laboratory", FacilityZoneId.EastLaboratory, new Vector2(11f, 0f), new Vector2(10f, 16f), SurfaceTheme.Radiation),
            new("North Cross Hall", FacilityZoneId.NorthCrossHall, new Vector2(0f, 11.5f), new Vector2(28f, 5f), SurfaceTheme.Neutral),
            new("West Rod Storage", FacilityZoneId.WestRodStorage, new Vector2(-11f, 20f), new Vector2(10f, 10f), SurfaceTheme.Radiation),
            new("Core Vault", FacilityZoneId.CoreVault, new Vector2(0f, 20f), new Vector2(8f, 10f), SurfaceTheme.Radiation),
            new("East Reactor", FacilityZoneId.EastReactor, new Vector2(11f, 20f), new Vector2(10f, 10f), SurfaceTheme.Radiation),
        };

        private static readonly CorridorSpec[] Corridors =
        {
            new("Entry-Decon", new Vector2(0f, -21.5f), new Vector2(3f, 3f), SurfaceTheme.Extraction),
            new("Decon-Hub", new Vector2(0f, -14.5f), new Vector2(3f, 3f), SurfaceTheme.Extraction),
            new("Hub-West", new Vector2(-7.5f, -6.5f), new Vector2(5f, 3f), SurfaceTheme.Maintenance),
            new("Hub-Central", new Vector2(0f, -5.5f), new Vector2(3f, 3f), SurfaceTheme.Neutral),
            new("Hub-East", new Vector2(7.5f, -6.5f), new Vector2(5f, 3f), SurfaceTheme.Radiation),
            new("West-Central Link", new Vector2(-5.5f, 3.9f), new Vector2(5f, 3f), SurfaceTheme.Maintenance),
            new("Central-East Link", new Vector2(5.5f, 3.9f), new Vector2(5f, 3f), SurfaceTheme.Radiation),
            new("West-North", new Vector2(-11f, 9.5f), new Vector2(3f, 3f), SurfaceTheme.Maintenance),
            new("Central-North", new Vector2(0f, 11f), new Vector2(3f, 3f), SurfaceTheme.Neutral),
            new("East-North", new Vector2(11f, 9.5f), new Vector2(3f, 3f), SurfaceTheme.Radiation),
            new("North-West Storage", new Vector2(-11f, 14.5f), new Vector2(3f, 3f), SurfaceTheme.Radiation),
            new("North-Core", new Vector2(0f, 14.5f), new Vector2(3f, 3f), SurfaceTheme.Radiation),
            new("North-East Reactor", new Vector2(11f, 14.5f), new Vector2(3f, 3f), SurfaceTheme.Radiation),
        };

        private static readonly PortalSpec[] Portals =
        {
            new("Extraction Portal", new Vector3(0f, 0f, -21.3f), 0f, SurfaceTheme.Extraction),
            new("Decon North Portal", new Vector3(0f, 0f, -15.1f), 0f, SurfaceTheme.Extraction),
            new("West Route Portal", new Vector3(-5.6f, 0f, -6.5f), 90f, SurfaceTheme.Maintenance),
            new("Central Route Portal", new Vector3(0f, 0f, -5.1f), 0f, SurfaceTheme.Neutral),
            new("East Route Portal", new Vector3(5.6f, 0f, -6.5f), 90f, SurfaceTheme.Radiation),
            new("West Cross-Link", new Vector3(-5.5f, 0f, 3.9f), 90f, SurfaceTheme.Maintenance),
            new("East Cross-Link", new Vector3(5.5f, 0f, 3.9f), 90f, SurfaceTheme.Radiation),
            new("West Cross Hall", new Vector3(-11f, 0f, 9f), 0f, SurfaceTheme.Maintenance),
            new("Central Cross Hall", new Vector3(0f, 0f, 9f), 0f, SurfaceTheme.Neutral),
            new("East Cross Hall", new Vector3(11f, 0f, 9f), 0f, SurfaceTheme.Radiation),
            new("West Storage Portal", new Vector3(-11f, 0f, 14.2f), 0f, SurfaceTheme.Radiation),
            new("Core Vault Portal", new Vector3(0f, 0f, 14.2f), 0f, SurfaceTheme.Radiation),
            new("East Reactor Portal", new Vector3(11f, 0f, 14.2f), 0f, SurfaceTheme.Radiation),
        };

        private static readonly WallSpec[] InteriorWalls =
        {
            // South partitions make the three Hub exits actual portals. Their gaps are
            // exactly where the seeded blockers live, preventing a one-metre bypass
            // across the otherwise continuous greybox floor.
            new("West Divider Far South", new Vector3(-5.5f, WallHeight * 0.5f, -11.45f), new Vector3(0.22f, WallHeight, 6.7f)),
            new("West Divider South", new Vector3(-5.5f, WallHeight * 0.5f, -1.2f), new Vector3(0.22f, WallHeight, 7.8f)),
            new("West Divider North", new Vector3(-5.5f, WallHeight * 0.5f, 6.3f), new Vector3(0.22f, WallHeight, 2.4f)),
            new("East Divider Far South", new Vector3(5.5f, WallHeight * 0.5f, -11.45f), new Vector3(0.22f, WallHeight, 6.7f)),
            new("East Divider South", new Vector3(5.5f, WallHeight * 0.5f, -1.2f), new Vector3(0.22f, WallHeight, 7.8f)),
            new("East Divider North", new Vector3(5.5f, WallHeight * 0.5f, 6.3f), new Vector3(0.22f, WallHeight, 2.4f)),
            new("Hub North Barrier West", new Vector3(-4.3f, WallHeight * 0.5f, -5.7f), new Vector3(5.6f, WallHeight, 0.22f)),
            new("Hub North Barrier East", new Vector3(4.3f, WallHeight * 0.5f, -5.7f), new Vector3(5.6f, WallHeight, 0.22f)),
            // The continuous greybox floor remains one bakeable surface, but these walls
            // force entry through Decontamination and the Operations Hub instead of
            // allowing a lateral walk around every seeded route state from the south.
            new("South Containment West", new Vector3(-9.75f, WallHeight * 0.5f, -14.8f), new Vector3(16.5f, WallHeight, 0.22f)),
            new("South Containment East", new Vector3(9.75f, WallHeight * 0.5f, -14.8f), new Vector3(16.5f, WallHeight, 0.22f)),
            new("Terminal Divider West A", new Vector3(-15.5f, WallHeight * 0.5f, 14.2f), new Vector3(5f, WallHeight, 0.22f)),
            new("Terminal Divider West B", new Vector3(-6.6f, WallHeight * 0.5f, 14.2f), new Vector3(5.8f, WallHeight, 0.22f)),
            new("Terminal Divider Core A", new Vector3(-2.8f, WallHeight * 0.5f, 14.2f), new Vector3(2.6f, WallHeight, 0.22f)),
            new("Terminal Divider Core B", new Vector3(2.8f, WallHeight * 0.5f, 14.2f), new Vector3(2.6f, WallHeight, 0.22f)),
            new("Terminal Divider East A", new Vector3(6.6f, WallHeight * 0.5f, 14.2f), new Vector3(5.8f, WallHeight, 0.22f)),
            new("Terminal Divider East B", new Vector3(15.5f, WallHeight * 0.5f, 14.2f), new Vector3(5f, WallHeight, 0.22f)),
            new("Extraction Lane Divider", new Vector3(0f, 0.7f, -26.3f), new Vector3(0.3f, 1.4f, 3.1f)),
        };

        private static readonly LandmarkSpec[] Landmarks =
        {
            new("Operations Orientation Beacon", new Vector3(0f, 1.4f, -10f), new Vector3(0.9f, 2.8f, 0.9f), SurfaceTheme.Extraction),
            new("Maintenance Hoist", new Vector3(-13.3f, 1.2f, -2.5f), new Vector3(1.3f, 2.4f, 1.3f), SurfaceTheme.Maintenance),
            new("Transfer Column", new Vector3(0f, 1.2f, 4.8f), new Vector3(1f, 2.4f, 1f), SurfaceTheme.Neutral),
            new("Laboratory Containment", new Vector3(13.3f, 1.2f, -2.5f), new Vector3(1.3f, 2.4f, 1.3f), SurfaceTheme.Radiation),
            new("North Hall Junction", new Vector3(0f, 1f, 11.5f), new Vector3(1.2f, 2f, 1.2f), SurfaceTheme.Extraction),
        };

        private static readonly AnchorSpec[] RodSocketSpecs =
        {
            new(RunConfiguration.WestStorageA, "W1", FacilityZoneId.WestRodStorage, new Vector3(-13.2f, 0f, 20.2f)),
            new(RunConfiguration.WestStorageB, "W2", FacilityZoneId.WestRodStorage, new Vector3(-8.8f, 0f, 22.1f)),
            new(RunConfiguration.CoreVaultA, "C1", FacilityZoneId.CoreVault, new Vector3(-1.55f, 0f, 19.8f)),
            new(RunConfiguration.CoreVaultB, "C2", FacilityZoneId.CoreVault, new Vector3(1.55f, 0f, 22.1f)),
            new(RunConfiguration.EastReactorA, "E1", FacilityZoneId.EastReactor, new Vector3(8.8f, 0f, 22.1f)),
            new(RunConfiguration.EastReactorB, "E2", FacilityZoneId.EastReactor, new Vector3(13.2f, 0f, 20.2f)),
            new(RunConfiguration.Maintenance, "M1", FacilityZoneId.WestMaintenance, new Vector3(-13.2f, 0f, 4.4f)),
            new(RunConfiguration.Laboratory, "L1", FacilityZoneId.EastLaboratory, new Vector3(13.2f, 0f, 4.4f)),
        };

        private static readonly Vector3[] CoverPositions =
        {
            new(-3.9f, 0f, -11.5f), new(3.9f, 0f, -8.2f),
            new(-13.9f, 0f, -5f), new(-8.1f, 0f, 0f), new(-13.7f, 0f, 6.4f),
            new(-1.6f, 0f, -2.8f), new(1.6f, 0f, 3.2f), new(-1.6f, 0f, 8.1f),
            new(13.9f, 0f, -5f), new(8.1f, 0f, 0f), new(13.7f, 0f, 6.4f),
            new(-7f, 0f, 11.5f), new(7f, 0f, 11.5f),
            new(-14f, 0f, 17f), new(-8f, 0f, 23f),
            new(-2.8f, 0f, 17f), new(2.8f, 0f, 23f),
            new(14f, 0f, 17f), new(8f, 0f, 23f),
            new(-3.7f, 0f, -24.5f), new(3.7f, 0f, -27f),
        };

        private static readonly Vector3[] PatrolPositions =
        {
            new(3f, 0.01f, -22f),  // 00 Entry / Decon, clear of rack
            new(2f, 0.01f, -8f),   // 01 Hub, clear of orientation beacon and east cover
            new(-4f, 0.01f, -6.5f), // 02 Hub west portal
            new(-11f, 0.01f, -6f), // 03 West south
            new(-11f, 0.01f, 1f),  // 04 West mid
            new(-11f, 0.01f, 7f),  // 05 West north
            new(-5f, 0.01f, 3.9f), // 06 West cross-link
            new(1.4f, 0.01f, -4f), // 07 Central south
            new(1.4f, 0.01f, 1f),  // 08 Central mid, clear of transfer landmark
            new(1.4f, 0.01f, 8f),  // 09 Central north
            new(5f, 0.01f, 3.9f),  // 10 East cross-link
            new(11f, 0.01f, -6f),  // 11 East south
            new(11f, 0.01f, 1f),   // 12 East mid
            new(11f, 0.01f, 7f),   // 13 East north
            new(-11f, 0.01f, 9.7f),  // 14 Cross hall west, south of cover line
            new(1.4f, 0.01f, 9.7f),  // 15 Cross hall center, clear of landmark
            new(11f, 0.01f, 9.7f),   // 16 Cross hall east, south of cover line
            new(-11f, 0.01f, 20f),   // 17 West storage
            new(0f, 0.01f, 20f),     // 18 Core vault
            new(11f, 0.01f, 20f),    // 19 East reactor
            new(4f, 0.01f, -6.5f),   // 20 Hub east portal
            new(-8.35f, 0.01f, 0.2f), // 21 Maintenance ramp south
            new(-8.35f, 0.01f, 2.8f), // 22 Maintenance ramp north
        };

        public static ExpandedFacilityBuild Build(Transform parent, ExpandedFacilityMaterials materials)
        {
            var root = new GameObject("Generated Facility");
            root.transform.SetParent(parent, false);

            var structure = new GameObject("Structure");
            structure.transform.SetParent(root.transform, false);
            BuildBaseShell(structure.transform, materials);

            var spacesRoot = new GameObject("Authored Spaces");
            spacesRoot.transform.SetParent(root.transform, false);
            foreach (var space in Spaces)
            {
                BuildSpace(space, spacesRoot.transform, materials);
            }

            var corridorsRoot = new GameObject("Route Families");
            corridorsRoot.transform.SetParent(root.transform, false);
            foreach (var corridor in Corridors)
            {
                BuildCorridor(corridor, corridorsRoot.transform, materials);
            }

            foreach (var portal in Portals)
            {
                BuildPortal(portal, corridorsRoot.transform, materials);
            }

            var wallsRoot = new GameObject("Interior Architecture");
            wallsRoot.transform.SetParent(root.transform, false);
            foreach (var wall in InteriorWalls)
            {
                CreateBox(wall.Name, wallsRoot.transform, wall.Position, wall.Scale, materials.Wall, true);
            }

            BuildCover(wallsRoot.transform, materials);
            BuildJumpShortcut(wallsRoot.transform, materials);
            BuildLandmarks(root.transform, materials);

            var mapObject = new GameObject("Prototype Map Definition");
            mapObject.transform.SetParent(root.transform, false);
            var mapDefinition = mapObject.AddComponent<PrototypeMapDefinition>();

            var anchorsRoot = new GameObject("Typed Anchors");
            anchorsRoot.transform.SetParent(root.transform, false);
            var playerSpawn = CreateAnchor("Player Spawn", anchorsRoot.transform, new Vector3(-2.5f, 0.06f, -25.9f));
            playerSpawn.rotation = Quaternion.identity;
            var rackAnchor = CreateAnchor("Extraction Rack Anchor", anchorsRoot.transform, new Vector3(0f, 0f, -22.25f));
            var exitAnchor = CreateAnchor("Extraction Exit Anchor", anchorsRoot.transform, new Vector3(0f, 0f, -28.65f));
            var owlSpawn = CreateAnchor("Owl Spawn Anchor", anchorsRoot.transform, new Vector3(1.4f, 0.01f, 9.7f));
            var missionDisplayAnchor = CreateAnchor("Mission Display Anchor", anchorsRoot.transform, new Vector3(-5.72f, 1.75f, -25f));
            missionDisplayAnchor.rotation = Quaternion.Euler(0f, -90f, 0f);

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
                PrototypeVariationIds.MaintenanceObstruction,
                PrototypeVariationIds.LightingStable);

            BuildExtractionGate(root.transform, exitAnchor.position, materials);

            return new ExpandedFacilityBuild
            {
                Root = root,
                MapDefinition = mapDefinition,
                MissionDisplayAnchor = missionDisplayAnchor,
            };
        }

        private static void BuildBaseShell(Transform parent, ExpandedFacilityMaterials materials)
        {
            var floor = CreateBox("Continuous Navigable Floor", parent, new Vector3(0f, -0.12f, -2f), new Vector3(36f, 0.24f, 56f), materials.BaseFloor, true);
            floor.AddComponent<TeleportationArea>();
            CreateBox("Facility Ceiling", parent, new Vector3(0f, WallHeight + 0.15f, -2f), new Vector3(36f, 0.2f, 56f), materials.Wall, true);
            CreateBox("West Perimeter", parent, new Vector3(-18f, WallHeight * 0.5f, -2f), new Vector3(0.3f, WallHeight, 56f), materials.Wall, true);
            CreateBox("East Perimeter", parent, new Vector3(18f, WallHeight * 0.5f, -2f), new Vector3(0.3f, WallHeight, 56f), materials.Wall, true);
            CreateBox("South Perimeter", parent, new Vector3(0f, WallHeight * 0.5f, -30f), new Vector3(36f, WallHeight, 0.3f), materials.Wall, true);
            CreateBox("North Perimeter", parent, new Vector3(0f, WallHeight * 0.5f, 26f), new Vector3(36f, WallHeight, 0.3f), materials.Wall, true);
        }

        private static void BuildSpace(SpaceSpec spec, Transform parent, ExpandedFacilityMaterials materials)
        {
            var root = new GameObject(spec.Name);
            root.transform.SetParent(parent, false);
            var material = ResolveTheme(spec.Theme, materials);
            CreateBox(
                $"{spec.Name} Floor Identity",
                root.transform,
                new Vector3(spec.Center.x, 0.015f, spec.Center.y),
                new Vector3(spec.Size.x, 0.03f, spec.Size.y),
                material,
                false);
            CreateWorldLabel(spec.Name, root.transform, new Vector3(spec.Center.x, 0.035f, spec.Center.y), Quaternion.Euler(90f, 0f, 0f), material.color);
        }

        private static void BuildCorridor(CorridorSpec spec, Transform parent, ExpandedFacilityMaterials materials)
        {
            var material = ResolveTheme(spec.Theme, materials);
            CreateBox(
                spec.Name,
                parent,
                new Vector3(spec.Center.x, 0.035f, spec.Center.y),
                new Vector3(spec.Size.x, 0.04f, spec.Size.y),
                material,
                false);
        }

        private static void BuildPortal(PortalSpec spec, Transform parent, ExpandedFacilityMaterials materials)
        {
            var root = new GameObject(spec.Name);
            root.transform.SetParent(parent, false);
            root.transform.SetPositionAndRotation(spec.Position, Quaternion.Euler(0f, spec.Yaw, 0f));
            var material = ResolveTheme(spec.Theme, materials);
            CreateBoxLocal("Left Guide", root.transform, new Vector3(-1.75f, 1.25f, 0f), new Vector3(0.14f, 2.5f, 0.14f), material, true);
            CreateBoxLocal("Right Guide", root.transform, new Vector3(1.75f, 1.25f, 0f), new Vector3(0.14f, 2.5f, 0.14f), material, true);
            CreateBoxLocal("Header", root.transform, new Vector3(0f, 2.55f, 0f), new Vector3(3.64f, 0.14f, 0.14f), material, true);
        }

        private static void BuildCover(Transform parent, ExpandedFacilityMaterials materials)
        {
            for (var i = 0; i < CoverPositions.Length; i++)
            {
                var scale = i % 3 == 0 ? new Vector3(1.2f, 2.25f, 0.8f) : new Vector3(0.8f, 2.25f, 1.2f);
                CreateBox($"Sight Break {i + 1:00}", parent, CoverPositions[i] + Vector3.up * (scale.y * 0.5f), scale, materials.Metal, true);
            }
        }

        private static void BuildJumpShortcut(Transform parent, ExpandedFacilityMaterials materials)
        {
            var root = new GameObject("Optional Maintenance Jump Shortcut");
            root.transform.SetParent(parent, false);
            CreateBox(
                "Low Pipe Jump Lane",
                root.transform,
                new Vector3(-14.45f, 0.29f, 1.5f),
                new Vector3(6.5f, 0.58f, 0.6f),
                materials.Amber,
                true);
            CreateBox(
                "Ramped Lane Low Barrier",
                root.transform,
                new Vector3(-8.35f, 0.29f, 1.5f),
                new Vector3(5.3f, 0.58f, 0.6f),
                materials.Metal,
                true);

            var southRamp = CreateBox(
                "Ramped Alternative South",
                root.transform,
                new Vector3(-8.35f, 0.31f, 0.38f),
                new Vector3(4.6f, 0.18f, 2f),
                materials.Metal,
                true);
            southRamp.transform.rotation = Quaternion.Euler(-16.5f, 0f, 0f);
            var northRamp = CreateBox(
                "Ramped Alternative North",
                root.transform,
                new Vector3(-8.35f, 0.31f, 2.62f),
                new Vector3(4.6f, 0.18f, 2f),
                materials.Metal,
                true);
            northRamp.transform.rotation = Quaternion.Euler(16.5f, 0f, 0f);

            CreateWorldLabel(
                "JUMP PIPE  |  RAMP ALTERNATIVE",
                root.transform,
                new Vector3(-11.5f, 0.68f, 0.05f),
                Quaternion.Euler(90f, 0f, 0f),
                materials.Amber.color,
                0.28f);
        }

        private static void BuildLandmarks(Transform parent, ExpandedFacilityMaterials materials)
        {
            var root = new GameObject("Orientation Landmarks");
            root.transform.SetParent(parent, false);
            foreach (var landmark in Landmarks)
            {
                var material = ResolveTheme(landmark.Theme, materials);
                var objectRoot = CreateBox(landmark.Name, root.transform, landmark.Position, landmark.Scale, material, true);
                var lightObject = new GameObject("Landmark Light");
                lightObject.transform.SetParent(objectRoot.transform, false);
                lightObject.transform.localPosition = Vector3.up * 0.65f;
                var light = lightObject.AddComponent<Light>();
                light.type = LightType.Point;
                light.color = material.color;
                light.intensity = 2.2f;
                light.range = 6f;
            }
        }

        private static RodSocketAnchor[] BuildRodSockets(Transform parent, ExpandedFacilityMaterials materials)
        {
            var root = new GameObject("Eight Rod Sockets");
            root.transform.SetParent(parent, false);
            var sockets = new RodSocketAnchor[RodSocketSpecs.Length];
            for (var i = 0; i < RodSocketSpecs.Length; i++)
            {
                var spec = RodSocketSpecs[i];
                var pedestal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                pedestal.name = $"Socket {spec.Id}";
                pedestal.transform.SetParent(root.transform);
                pedestal.transform.position = spec.Position + Vector3.up * 0.11f;
                pedestal.transform.localScale = new Vector3(0.52f, 0.11f, 0.52f);
                pedestal.GetComponent<Renderer>().sharedMaterial = materials.RadiationGreen;
                pedestal.isStatic = true;

                var point = CreateAnchor($"{spec.Id} Spawn Point", pedestal.transform, spec.Position + Vector3.up * 1.05f);
                point.SetParent(root.transform, true);
                var socket = point.gameObject.AddComponent<RodSocketAnchor>();
                socket.Configure(spec.Id, spec.Zone, point);
                sockets[i] = socket;

                CreateWorldLabel(spec.Label, root.transform, spec.Position + new Vector3(0f, 0.04f, -0.75f), Quaternion.Euler(90f, 0f, 0f), materials.RadiationGreen.color, 0.34f);
            }

            return sockets;
        }

        private static Transform[] BuildPatrolPoints(Transform parent)
        {
            var root = new GameObject("Owl Patrol Graph Anchors");
            root.transform.SetParent(parent, false);
            var points = new Transform[PatrolPositions.Length];
            for (var i = 0; i < PatrolPositions.Length; i++)
            {
                points[i] = CreateAnchor($"Patrol {i + 1:00}", root.transform, PatrolPositions[i]);
            }

            return points;
        }

        private static RouteVariationAnchor[] BuildRouteVariations(Transform parent, ExpandedFacilityMaterials materials)
        {
            var root = new GameObject("Seeded Route States");
            root.transform.SetParent(parent, false);
            var specs = new[]
            {
                new VariationSpec(PrototypeVariationIds.MaintenanceObstruction, new Vector3(-5.5f, 1.05f, -6.5f), new Vector3(0.45f, 2.1f, 3.4f), "MAINTENANCE OBSTRUCTION"),
                new VariationSpec(PrototypeVariationIds.LaboratoryQuarantine, new Vector3(5.5f, 1.05f, -6.5f), new Vector3(0.45f, 2.1f, 3.4f), "LAB QUARANTINE"),
                new VariationSpec(PrototypeVariationIds.CentralPowerOutage, new Vector3(0f, 1.05f, -5.7f), new Vector3(3f, 2.1f, 0.45f), "CENTRAL POWER OUTAGE"),
            };
            var anchors = new RouteVariationAnchor[specs.Length];
            for (var i = 0; i < specs.Length; i++)
            {
                var spec = specs[i];
                var variationRoot = new GameObject(spec.Id);
                variationRoot.transform.SetParent(root.transform, false);
                var blocker = CreateBox(
                    "Carved Route Blocker",
                    variationRoot.transform,
                    spec.Position,
                    spec.Scale,
                    materials.DangerRed,
                    true);
                blocker.isStatic = false;
                var obstacle = blocker.AddComponent<NavMeshObstacle>();
                obstacle.shape = NavMeshObstacleShape.Box;
                obstacle.size = Vector3.one;
                obstacle.carving = true;
                obstacle.carveOnlyStationary = true;
                CreateWorldLabel(spec.Label, variationRoot.transform, spec.Position + Vector3.up * 1.35f, Quaternion.identity, materials.DangerRed.color, 0.25f);
                var anchor = variationRoot.AddComponent<RouteVariationAnchor>();
                anchor.Configure(spec.Id, variationRoot);
                anchors[i] = anchor;
            }

            return anchors;
        }

        private static LightingVariantAnchor[] BuildLightingVariants(Transform parent, ExpandedFacilityMaterials materials)
        {
            var root = new GameObject("Seeded Lighting Profiles");
            root.transform.SetParent(parent, false);
            var ids = new[]
            {
                PrototypeVariationIds.LightingStable,
                PrototypeVariationIds.LightingEmergency,
                PrototypeVariationIds.LightingPartialOutage,
            };
            var anchors = new LightingVariantAnchor[ids.Length];
            for (var profileIndex = 0; profileIndex < ids.Length; profileIndex++)
            {
                var profileRoot = new GameObject(ids[profileIndex]);
                profileRoot.transform.SetParent(root.transform, false);
                for (var i = 0; i < Spaces.Length; i++)
                {
                    var space = Spaces[i];
                    var lightObject = new GameObject($"{space.Name} Light");
                    lightObject.transform.SetParent(profileRoot.transform, false);
                    lightObject.transform.position = new Vector3(space.Center.x, 2.8f, space.Center.y);
                    var light = lightObject.AddComponent<Light>();
                    light.type = LightType.Point;
                    light.range = Mathf.Max(space.Size.x, space.Size.y) * 0.72f;
                    light.shadows = LightShadows.Soft;

                    if (profileIndex == 0)
                    {
                        light.color = ResolveTheme(space.Theme, materials).color;
                        light.intensity = 3.2f;
                    }
                    else if (profileIndex == 1)
                    {
                        light.color = i % 2 == 0 ? materials.DangerRed.color : ResolveTheme(space.Theme, materials).color;
                        light.intensity = i % 2 == 0 ? 3.7f : 1.8f;
                    }
                    else
                    {
                        light.color = ResolveTheme(space.Theme, materials).color;
                        light.intensity = i is 0 or 2 or 6 ? 2.4f : 0.65f;
                    }
                }

                var anchor = profileRoot.AddComponent<LightingVariantAnchor>();
                anchor.Configure(ids[profileIndex], profileRoot);
                anchors[profileIndex] = anchor;
            }

            return anchors;
        }

        private static FacilityZoneTrigger[] BuildZoneTriggers(Transform parent, PrototypeMapDefinition definition)
        {
            var root = new GameObject("Zone Tracking");
            root.transform.SetParent(parent, false);
            var triggers = new FacilityZoneTrigger[Spaces.Length];
            for (var i = 0; i < Spaces.Length; i++)
            {
                var space = Spaces[i];
                var triggerObject = new GameObject($"{space.Name} Zone");
                triggerObject.transform.SetParent(root.transform, false);
                triggerObject.transform.position = new Vector3(space.Center.x, 1.5f, space.Center.y);
                var collider = triggerObject.AddComponent<BoxCollider>();
                collider.isTrigger = true;
                collider.size = new Vector3(space.Size.x, 3f, space.Size.y);
                var trigger = triggerObject.AddComponent<FacilityZoneTrigger>();
                trigger.Configure(space.Zone, definition);
                triggers[i] = trigger;
            }

            return triggers;
        }

        private static void BuildExtractionGate(Transform parent, Vector3 exitPosition, ExpandedFacilityMaterials materials)
        {
            var root = new GameObject("Extraction Gate Presentation");
            root.transform.SetParent(parent, false);
            CreateBox("Gate Left", root.transform, exitPosition + new Vector3(-2.2f, 1.4f, 0f), new Vector3(0.18f, 2.8f, 0.24f), materials.Cyan, true);
            CreateBox("Gate Right", root.transform, exitPosition + new Vector3(2.2f, 1.4f, 0f), new Vector3(0.18f, 2.8f, 0.24f), materials.Cyan, true);
            CreateBox("Gate Header", root.transform, exitPosition + new Vector3(0f, 2.75f, 0f), new Vector3(4.6f, 0.18f, 0.24f), materials.Cyan, true);
        }

        private static Transform CreateAnchor(string name, Transform parent, Vector3 position)
        {
            var anchor = new GameObject(name).transform;
            anchor.SetParent(parent);
            anchor.position = position;
            return anchor;
        }

        private static GameObject CreateBox(
            string name,
            Transform parent,
            Vector3 position,
            Vector3 scale,
            Material material,
            bool collider)
        {
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
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

        private static GameObject CreateBoxLocal(
            string name,
            Transform parent,
            Vector3 position,
            Vector3 scale,
            Material material,
            bool collider)
        {
            var gameObject = CreateBox(name, parent, Vector3.zero, scale, material, collider);
            gameObject.transform.localPosition = position;
            gameObject.transform.localRotation = Quaternion.identity;
            return gameObject;
        }

        private static TMP_Text CreateWorldLabel(
            string text,
            Transform parent,
            Vector3 position,
            Quaternion rotation,
            Color color,
            float fontSize = 0.42f)
        {
            var labelObject = new GameObject($"{text} Label", typeof(RectTransform), typeof(TextMeshPro));
            labelObject.transform.SetParent(parent);
            labelObject.transform.SetPositionAndRotation(position, rotation);
            var rect = (RectTransform)labelObject.transform;
            rect.sizeDelta = new Vector2(5.5f, 1.1f);
            var label = labelObject.GetComponent<TextMeshPro>();
            label.text = text.ToUpperInvariant();
            label.fontSize = fontSize;
            label.fontStyle = FontStyles.Bold;
            label.alignment = TextAlignmentOptions.Center;
            label.color = color;
            return label;
        }

        private static Material ResolveTheme(SurfaceTheme theme, ExpandedFacilityMaterials materials)
        {
            return theme switch
            {
                SurfaceTheme.Extraction => materials.Cyan,
                SurfaceTheme.Maintenance => materials.Amber,
                SurfaceTheme.Radiation => materials.RadiationGreen,
                SurfaceTheme.Danger => materials.DangerRed,
                _ => materials.NeutralLight,
            };
        }

        private enum SurfaceTheme
        {
            Neutral,
            Extraction,
            Maintenance,
            Radiation,
            Danger,
        }

        private readonly struct SpaceSpec
        {
            public SpaceSpec(string name, FacilityZoneId zone, Vector2 center, Vector2 size, SurfaceTheme theme)
            {
                Name = name;
                Zone = zone;
                Center = center;
                Size = size;
                Theme = theme;
            }

            public string Name { get; }
            public FacilityZoneId Zone { get; }
            public Vector2 Center { get; }
            public Vector2 Size { get; }
            public SurfaceTheme Theme { get; }
        }

        private readonly struct CorridorSpec
        {
            public CorridorSpec(string name, Vector2 center, Vector2 size, SurfaceTheme theme)
            {
                Name = name;
                Center = center;
                Size = size;
                Theme = theme;
            }

            public string Name { get; }
            public Vector2 Center { get; }
            public Vector2 Size { get; }
            public SurfaceTheme Theme { get; }
        }

        private readonly struct PortalSpec
        {
            public PortalSpec(string name, Vector3 position, float yaw, SurfaceTheme theme)
            {
                Name = name;
                Position = position;
                Yaw = yaw;
                Theme = theme;
            }

            public string Name { get; }
            public Vector3 Position { get; }
            public float Yaw { get; }
            public SurfaceTheme Theme { get; }
        }

        private readonly struct WallSpec
        {
            public WallSpec(string name, Vector3 position, Vector3 scale)
            {
                Name = name;
                Position = position;
                Scale = scale;
            }

            public string Name { get; }
            public Vector3 Position { get; }
            public Vector3 Scale { get; }
        }

        private readonly struct LandmarkSpec
        {
            public LandmarkSpec(string name, Vector3 position, Vector3 scale, SurfaceTheme theme)
            {
                Name = name;
                Position = position;
                Scale = scale;
                Theme = theme;
            }

            public string Name { get; }
            public Vector3 Position { get; }
            public Vector3 Scale { get; }
            public SurfaceTheme Theme { get; }
        }

        private readonly struct AnchorSpec
        {
            public AnchorSpec(string id, string label, FacilityZoneId zone, Vector3 position)
            {
                Id = id;
                Label = label;
                Zone = zone;
                Position = position;
            }

            public string Id { get; }
            public string Label { get; }
            public FacilityZoneId Zone { get; }
            public Vector3 Position { get; }
        }

        private readonly struct VariationSpec
        {
            public VariationSpec(string id, Vector3 position, Vector3 scale, string label)
            {
                Id = id;
                Position = position;
                Scale = scale;
                Label = label;
            }

            public string Id { get; }
            public Vector3 Position { get; }
            public Vector3 Scale { get; }
            public string Label { get; }
        }
    }
}
