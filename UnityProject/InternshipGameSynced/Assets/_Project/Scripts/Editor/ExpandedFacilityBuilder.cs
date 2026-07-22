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
        public Material DarkTrim;
        public Material Panel;
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
            new("West Route Portal", new Vector3(-5.5f, 0f, -6.5f), 90f, SurfaceTheme.Maintenance),
            new("Central Route Portal", new Vector3(0f, 0f, -5.7f), 0f, SurfaceTheme.Neutral),
            new("East Route Portal", new Vector3(5.5f, 0f, -6.5f), 90f, SurfaceTheme.Radiation),
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
            new("Operations Orientation Beacon", new Vector3(-3.5f, 1.2f, -9.6f), new Vector3(0.65f, 2.4f, 0.65f), SurfaceTheme.Extraction),
            new("Maintenance Hoist", new Vector3(-13.6f, 1.2f, -3.1f), new Vector3(1.1f, 2.4f, 1.5f), SurfaceTheme.Maintenance),
            new("Transfer Column", new Vector3(0f, 1.2f, 4.8f), new Vector3(1f, 2.4f, 1f), SurfaceTheme.Neutral),
            new("Laboratory Containment", new Vector3(8.3f, 1.05f, 2.2f), new Vector3(1.5f, 2.1f, 1.1f), SurfaceTheme.Radiation),
            new("North Hall Junction", new Vector3(-2.6f, 1f, 11.5f), new Vector3(1.2f, 2f, 1.2f), SurfaceTheme.Extraction),
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
            // Suspend the primary mission board directly over the intake rack. The old
            // west-wall placement sat behind mandatory sight-breaking cover from spawn.
            var missionDisplayAnchor = CreateAnchor("Mission Display Anchor", anchorsRoot.transform, new Vector3(0f, 2.7f, -22.15f));
            missionDisplayAnchor.rotation = Quaternion.identity;

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

            // Route colour is an orientation cue, not a replacement floor. Keeping the
            // inlay narrow exposes the common facility floor and avoids the old stack of
            // full-room coplanar plates that made the map read as ten coloured cubes.
            var inlaySize = ResolveRoomInlaySize(spec.Size);
            CreateBox(
                $"{spec.Name} Route Inlay",
                root.transform,
                new Vector3(spec.Center.x, 0.009f, spec.Center.y),
                new Vector3(inlaySize.x, 0.018f, inlaySize.y),
                material,
                false);

            BuildRoomIdentitySign(spec, root.transform, material, materials);
            BuildRoomProfile(spec, root.transform, material, materials);
        }

        private static void BuildCorridor(CorridorSpec spec, Transform parent, ExpandedFacilityMaterials materials)
        {
            var material = ResolveTheme(spec.Theme, materials);
            CreateBox(
                spec.Name,
                parent,
                new Vector3(spec.Center.x, 0.018f, spec.Center.y),
                new Vector3(
                    Mathf.Max(0.2f, spec.Size.x - 0.14f),
                    0.012f,
                    Mathf.Max(0.2f, spec.Size.y - 0.14f)),
                material,
                false);
        }

        private static void BuildPortal(PortalSpec spec, Transform parent, ExpandedFacilityMaterials materials)
        {
            var root = new GameObject(spec.Name);
            root.transform.SetParent(parent, false);
            root.transform.SetPositionAndRotation(spec.Position, Quaternion.Euler(0f, spec.Yaw, 0f));
            var material = ResolveTheme(spec.Theme, materials);
            const float clearWidth = 2.76f;
            const float postWidth = 0.22f;
            const float frameDepth = 0.32f;
            const float clearHeight = 2.52f;
            var postX = clearWidth * 0.5f + postWidth * 0.5f;
            var frameWidth = clearWidth + postWidth * 2f;

            CreateBoxLocal("Left Fitted Jamb", root.transform, new Vector3(-postX, clearHeight * 0.5f, 0f), new Vector3(postWidth, clearHeight, frameDepth), materials.DarkTrim, true);
            CreateBoxLocal("Right Fitted Jamb", root.transform, new Vector3(postX, clearHeight * 0.5f, 0f), new Vector3(postWidth, clearHeight, frameDepth), materials.DarkTrim, true);
            CreateBoxLocal("Fitted Header", root.transform, new Vector3(0f, clearHeight + 0.12f, 0f), new Vector3(frameWidth, 0.24f, frameDepth), materials.DarkTrim, true);

            CreateBoxLocal("Left Route Edge", root.transform, new Vector3(-clearWidth * 0.5f + 0.035f, 1.24f, -0.175f), new Vector3(0.07f, 2.35f, 0.035f), material, false);
            CreateBoxLocal("Right Route Edge", root.transform, new Vector3(clearWidth * 0.5f - 0.035f, 1.24f, -0.175f), new Vector3(0.07f, 2.35f, 0.035f), material, false);
            CreateBoxLocal("Route Header Edge", root.transform, new Vector3(0f, clearHeight - 0.035f, -0.175f), new Vector3(clearWidth, 0.07f, 0.035f), material, false);
            CreateBoxLocal("Fitted Threshold", root.transform, new Vector3(0f, 0.012f, 0f), new Vector3(clearWidth, 0.024f, 0.52f), material, false);

            var portalLabel = spec.Name
                .Replace(" Portal", string.Empty)
                .Replace(" Route", string.Empty)
                .ToUpperInvariant();
            CreatePanelTextLocal(
                $"{spec.Name} Front Label",
                root.transform,
                new Vector3(0f, clearHeight + 0.12f, -0.175f),
                Quaternion.identity,
                portalLabel,
                Color.white,
                new Vector2(2.55f, 0.28f),
                0.5f,
                false);
            CreatePanelTextLocal(
                $"{spec.Name} Rear Label",
                root.transform,
                new Vector3(0f, clearHeight + 0.12f, 0.175f),
                Quaternion.Euler(0f, 180f, 0f),
                portalLabel,
                Color.white,
                new Vector2(2.55f, 0.28f),
                0.5f,
                false);
        }

        private static void BuildCover(Transform parent, ExpandedFacilityMaterials materials)
        {
            for (var i = 0; i < CoverPositions.Length; i++)
            {
                var scale = i % 3 == 0 ? new Vector3(1.2f, 2.25f, 0.8f) : new Vector3(0.8f, 2.25f, 1.2f);
                var root = new GameObject($"Sight Break {i + 1:00}");
                root.transform.SetParent(parent, false);
                root.transform.position = CoverPositions[i];

                // The core keeps the proven footprint used by navigation and threat
                // tests. Caps, rails and alternating faces make the repeated cover read
                // as a modular equipment family instead of cloned cubes.
                CreateBoxLocal("Collision Core", root.transform, Vector3.up * (scale.y * 0.5f), scale, materials.Metal, true);
                CreateBoxLocal("Dark Base", root.transform, new Vector3(0f, 0.08f, 0f), new Vector3(scale.x + 0.12f, 0.16f, scale.z + 0.12f), materials.DarkTrim, false);
                CreateBoxLocal("Top Cap", root.transform, new Vector3(0f, scale.y + 0.055f, 0f), new Vector3(scale.x + 0.1f, 0.11f, scale.z + 0.1f), materials.DarkTrim, false);
                var accent = i % 4 is 0 or 3 ? materials.Amber : i % 4 == 1 ? materials.Cyan : materials.NeutralLight;
                var railX = i % 2 == 0 ? scale.x * 0.5f + 0.012f : 0f;
                var railZ = i % 2 == 0 ? 0f : scale.z * 0.5f + 0.012f;
                var railScale = i % 2 == 0
                    ? new Vector3(0.025f, scale.y * 0.68f, scale.z * 0.54f)
                    : new Vector3(scale.x * 0.54f, scale.y * 0.68f, 0.025f);
                CreateBoxLocal("Equipment Identity Rail", root.transform, new Vector3(railX, scale.y * 0.55f, railZ), railScale, accent, false);
            }
        }

        private static Vector2 ResolveRoomInlaySize(Vector2 roomSize)
        {
            if (roomSize.x > roomSize.y * 1.7f)
            {
                return new Vector2(Mathf.Max(2f, roomSize.x - 0.5f), 0.72f);
            }

            if (roomSize.y > roomSize.x * 1.35f)
            {
                return new Vector2(Mathf.Min(2.15f, roomSize.x - 0.5f), Mathf.Max(2f, roomSize.y - 0.5f));
            }

            return new Vector2(Mathf.Min(3.4f, roomSize.x - 0.5f), Mathf.Min(3.4f, roomSize.y - 0.5f));
        }

        private static void BuildRoomIdentitySign(
            SpaceSpec spec,
            Transform parent,
            Material accent,
            ExpandedFacilityMaterials materials)
        {
            Vector3 position;
            Quaternion rotation;
            switch (spec.Zone)
            {
                case FacilityZoneId.EntryExtraction:
                    position = new Vector3(-5.72f, 2.7f, -27.4f);
                    rotation = Quaternion.Euler(0f, -90f, 0f);
                    break;
                case FacilityZoneId.WestMaintenance:
                    position = new Vector3(-15.78f, 2.35f, 0.5f);
                    rotation = Quaternion.Euler(0f, -90f, 0f);
                    break;
                case FacilityZoneId.CentralTransfer:
                    position = new Vector3(2.78f, 2.35f, 1f);
                    rotation = Quaternion.Euler(0f, 90f, 0f);
                    break;
                case FacilityZoneId.EastLaboratory:
                    position = new Vector3(15.78f, 2.35f, -0.4f);
                    rotation = Quaternion.Euler(0f, 90f, 0f);
                    break;
                default:
                    position = new Vector3(spec.Center.x, 2.72f, spec.Center.y + spec.Size.y * 0.5f - 0.18f);
                    rotation = Quaternion.identity;
                    break;
            }

            CreatePanelSign(
                $"{spec.Name} Identity Sign",
                parent,
                position,
                rotation,
                $"{spec.Name.ToUpperInvariant()}\n{ResolveZoneCode(spec.Zone)}",
                accent,
                materials,
                3.5f,
                0.72f);
        }

        private static string ResolveZoneCode(FacilityZoneId zone)
        {
            return zone switch
            {
                FacilityZoneId.EntryExtraction => "E-01  /  EXTRACTION",
                FacilityZoneId.Decontamination => "D-02  /  TRANSIT",
                FacilityZoneId.OperationsHub => "H-03  /  THREE ROUTES",
                FacilityZoneId.WestMaintenance => "W-04  /  AMBER ROUTE",
                FacilityZoneId.CentralTransfer => "C-05  /  TRANSFER ROUTE",
                FacilityZoneId.EastLaboratory => "L-06  /  GREEN ROUTE",
                FacilityZoneId.NorthCrossHall => "N-07  /  CROSS-CONNECTION",
                FacilityZoneId.WestRodStorage => "W-08  /  ROD STORAGE",
                FacilityZoneId.CoreVault => "C-09  /  CORE ACCESS",
                FacilityZoneId.EastReactor => "E-10  /  REACTOR ACCESS",
                _ => "FACILITY ZONE",
            };
        }

        private static void BuildRoomProfile(
            SpaceSpec spec,
            Transform parent,
            Material accent,
            ExpandedFacilityMaterials materials)
        {
            var root = new GameObject("Scrappable Architectural Profile");
            root.transform.SetParent(parent, false);

            switch (spec.Zone)
            {
                case FacilityZoneId.EntryExtraction:
                    BuildEntryProfile(root.transform, accent, materials);
                    break;
                case FacilityZoneId.Decontamination:
                    BuildDecontaminationProfile(root.transform, accent, materials);
                    break;
                case FacilityZoneId.OperationsHub:
                    BuildOperationsProfile(root.transform, accent, materials);
                    break;
                case FacilityZoneId.WestMaintenance:
                    BuildMaintenanceProfile(root.transform, accent, materials);
                    break;
                case FacilityZoneId.CentralTransfer:
                    BuildTransferProfile(root.transform, accent, materials);
                    break;
                case FacilityZoneId.EastLaboratory:
                    BuildLaboratoryProfile(root.transform, accent, materials);
                    break;
                case FacilityZoneId.NorthCrossHall:
                    BuildCrossHallProfile(root.transform, accent, materials);
                    break;
                case FacilityZoneId.WestRodStorage:
                    BuildStorageProfile(root.transform, accent, materials);
                    break;
                case FacilityZoneId.CoreVault:
                    BuildCoreProfile(root.transform, accent, materials);
                    break;
                case FacilityZoneId.EastReactor:
                    BuildReactorProfile(root.transform, accent, materials);
                    break;
            }
        }

        private static void BuildEntryProfile(Transform parent, Material accent, ExpandedFacilityMaterials materials)
        {
            CreateBox("Offset Security Booth", parent, new Vector3(-4.92f, 1f, -27.25f), new Vector3(1.55f, 2f, 2.25f), materials.Metal, true);
            CreateBox("Security Booth Face", parent, new Vector3(-4.1f, 1.22f, -27.25f), new Vector3(0.08f, 0.72f, 1.65f), materials.Panel, false);
            CreateBox("Security Booth Accent", parent, new Vector3(-4.04f, 1.62f, -27.25f), new Vector3(0.04f, 0.08f, 1.55f), accent, false);
            CreateBox("Extraction Canopy", parent, new Vector3(2.45f, 3.03f, -23.9f), new Vector3(5.7f, 0.28f, 0.65f), materials.DarkTrim, false);
            CreateBox("Extraction Canopy Edge", parent, new Vector3(2.45f, 2.86f, -23.58f), new Vector3(5.35f, 0.06f, 0.05f), accent, false);
        }

        private static void BuildDecontaminationProfile(Transform parent, Material accent, ExpandedFacilityMaterials materials)
        {
            foreach (var x in new[] { -2.67f, 2.67f })
            {
                CreateBox("Decon Side Filter", parent, new Vector3(x, 1.15f, -18.35f), new Vector3(0.52f, 2.3f, 1.35f), materials.Metal, true);
                CreateBox("Decon Filter Face", parent, new Vector3(x - Mathf.Sign(x) * 0.275f, 1.2f, -18.35f), new Vector3(0.04f, 1.55f, 0.95f), materials.Panel, false);
            }

            foreach (var z in new[] { -19.65f, -17.1f })
            {
                CreateBox("Decon Ceiling Rib", parent, new Vector3(0f, 3.16f, z), new Vector3(5.5f, 0.34f, 0.28f), materials.DarkTrim, false);
                CreateBox("Decon Status Strip", parent, new Vector3(0f, 2.96f, z - 0.15f), new Vector3(4.8f, 0.05f, 0.04f), accent, false);
            }
        }

        private static void BuildOperationsProfile(Transform parent, Material accent, ExpandedFacilityMaterials materials)
        {
            CreateBox("Offset Command Dais", parent, new Vector3(5.25f, 0.54f, -12.25f), new Vector3(2.65f, 1.08f, 1.25f), materials.Metal, true);
            CreateBox("Command Dais Face", parent, new Vector3(5.25f, 0.88f, -11.59f), new Vector3(2.25f, 0.28f, 0.06f), materials.Panel, false);
            CreateBox("Command Dais Readout", parent, new Vector3(5.25f, 0.88f, -11.55f), new Vector3(1.7f, 0.08f, 0.025f), accent, false);
            CreateBox("Asymmetric Hub Gantry", parent, new Vector3(-1.65f, 3.08f, -8f), new Vector3(9.5f, 0.28f, 0.42f), materials.DarkTrim, false);
            CreateBox("Hub Gantry Cyan Edge", parent, new Vector3(-1.65f, 2.9f, -7.78f), new Vector3(8.8f, 0.06f, 0.04f), accent, false);
        }

        private static void BuildMaintenanceProfile(Transform parent, Material accent, ExpandedFacilityMaterials materials)
        {
            var upperPipe = CreatePrimitiveLocal("West Overhead Main", PrimitiveType.Cylinder, parent, new Vector3(-15.3f, 3.02f, 0.7f), new Vector3(0.18f, 4.6f, 0.18f), materials.DarkTrim, false);
            upperPipe.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            var servicePipe = CreatePrimitiveLocal("West Overhead Service", PrimitiveType.Cylinder, parent, new Vector3(-14.75f, 2.72f, -1.2f), new Vector3(0.1f, 3.2f, 0.1f), accent, false);
            servicePipe.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            foreach (var z in new[] { -5.4f, 1.2f, 6.5f })
            {
                CreateBox("Maintenance Pipe Hanger", parent, new Vector3(-15.3f, 2.83f, z), new Vector3(0.75f, 0.58f, 0.14f), materials.Metal, false);
            }
        }

        private static void BuildTransferProfile(Transform parent, Material accent, ExpandedFacilityMaterials materials)
        {
            var positions = new[]
            {
                new Vector3(-2.72f, 1.3f, -3.2f),
                new Vector3(2.72f, 1.3f, 1.5f),
                new Vector3(-2.72f, 1.3f, 7f),
            };
            for (var i = 0; i < positions.Length; i++)
            {
                CreateBox($"Transfer Offset Buttress {i + 1}", parent, positions[i], new Vector3(0.48f, 2.6f, 1.05f), materials.Metal, true);
                CreateBox($"Transfer Buttress Edge {i + 1}", parent, positions[i] + new Vector3(i % 2 == 0 ? 0.26f : -0.26f, 0f, 0f), new Vector3(0.04f, 1.9f, 0.76f), accent, false);
            }
        }

        private static void BuildLaboratoryProfile(Transform parent, Material accent, ExpandedFacilityMaterials materials)
        {
            CreateBox("Laboratory Offset Bench", parent, new Vector3(14.75f, 0.58f, -0.8f), new Vector3(1.45f, 1.16f, 2.7f), materials.Metal, true);
            CreateBox("Laboratory Bench Readout", parent, new Vector3(13.99f, 0.9f, -0.8f), new Vector3(0.05f, 0.38f, 1.85f), accent, false);
            foreach (var z in new[] { 2.25f, 5.25f })
            {
                var pod = CreatePrimitiveLocal("Laboratory Wall Vessel", PrimitiveType.Cylinder, parent, new Vector3(15.05f, 1.25f, z), new Vector3(0.44f, 1.15f, 0.44f), materials.DarkTrim, false);
                CreateBox("Laboratory Vessel Band", parent, pod.transform.position + new Vector3(-0.48f, 0f, 0f), new Vector3(0.05f, 0.14f, 0.62f), accent, false);
            }
        }

        private static void BuildCrossHallProfile(Transform parent, Material accent, ExpandedFacilityMaterials materials)
        {
            foreach (var x in new[] { -11.5f, -5f, 3.5f, 10.5f })
            {
                CreateBox("North Hall Offset Truss", parent, new Vector3(x, 3.1f, 11.5f), new Vector3(0.26f, 0.38f, 4.35f), materials.DarkTrim, false);
            }
            CreateBox("North Hall Route Spine", parent, new Vector3(2f, 2.87f, 13.72f), new Vector3(18f, 0.06f, 0.05f), accent, false);
        }

        private static void BuildStorageProfile(Transform parent, Material accent, ExpandedFacilityMaterials materials)
        {
            CreateBox("West Storage Shelving", parent, new Vector3(-15.05f, 1.25f, 21f), new Vector3(0.9f, 2.5f, 5.4f), materials.Metal, true);
            foreach (var z in new[] { 19f, 21f, 23f })
            {
                CreateBox("Storage Shelf Edge", parent, new Vector3(-14.56f, 1.25f, z), new Vector3(0.06f, 1.85f, 0.12f), accent, false);
            }
            CreateBox("Storage Overhead Crane", parent, new Vector3(-9.2f, 3.04f, 17.2f), new Vector3(5.8f, 0.3f, 0.45f), materials.DarkTrim, false);
        }

        private static void BuildCoreProfile(Transform parent, Material accent, ExpandedFacilityMaterials materials)
        {
            var southBrace = CreateBox("Core South-West Diagonal", parent, new Vector3(-3.25f, 1.45f, 16.25f), new Vector3(0.55f, 2.9f, 1.9f), materials.DarkTrim, false);
            southBrace.transform.rotation = Quaternion.Euler(0f, 34f, 0f);
            var northBrace = CreateBox("Core North-East Diagonal", parent, new Vector3(3.25f, 1.45f, 23.75f), new Vector3(0.55f, 2.9f, 1.9f), materials.DarkTrim, false);
            northBrace.transform.rotation = Quaternion.Euler(0f, 34f, 0f);
            CreateBox("Core Overhead Bridge", parent, new Vector3(0.7f, 3.05f, 20f), new Vector3(6.2f, 0.3f, 0.55f), materials.Metal, false);
            CreateBox("Core Overhead Status", parent, new Vector3(0.7f, 2.86f, 19.7f), new Vector3(5.4f, 0.06f, 0.04f), accent, false);
        }

        private static void BuildReactorProfile(Transform parent, Material accent, ExpandedFacilityMaterials materials)
        {
            foreach (var position in new[] { new Vector3(15f, 1.35f, 17.35f), new Vector3(7.15f, 1.35f, 23.45f) })
            {
                var stack = CreatePrimitiveLocal("Reactor Service Stack", PrimitiveType.Cylinder, parent, position, new Vector3(0.56f, 1.35f, 0.56f), materials.Metal, true);
                CreateBox("Reactor Stack Band", parent, stack.transform.position + new Vector3(0f, 0.25f, -0.59f), new Vector3(0.72f, 0.13f, 0.05f), accent, false);
            }
            CreateBox("Reactor Offset Bus", parent, new Vector3(12.1f, 3.05f, 24.2f), new Vector3(6.4f, 0.32f, 0.48f), materials.DarkTrim, false);
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

            CreatePanelSign(
                "Maintenance Shortcut Sign",
                root.transform,
                new Vector3(-11.5f, 1.55f, -0.02f),
                Quaternion.identity,
                "OPTIONAL JUMP SHORTCUT\nRAMP BYPASS REMAINS OPEN",
                materials.Amber,
                materials,
                3.5f,
                0.64f);
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

                CreateFloorStencil(
                    $"Socket {spec.Label} Stencil",
                    root.transform,
                    spec.Position + new Vector3(0f, 0.046f, -0.72f),
                    spec.Label,
                    materials.RadiationGreen.color);
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
                var labelRotation = spec.Scale.x < spec.Scale.z
                    ? Quaternion.Euler(0f, 90f, 0f)
                    : Quaternion.identity;
                CreatePanelSign(
                    $"{spec.Id} Warning Sign",
                    variationRoot.transform,
                    spec.Position + Vector3.up * 1.45f,
                    labelRotation,
                    $"ROUTE CLOSED\n{spec.Label}",
                    materials.DangerRed,
                    materials,
                    2.85f,
                    0.62f,
                    false);
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

        private static GameObject CreatePrimitiveLocal(
            string name,
            PrimitiveType primitiveType,
            Transform parent,
            Vector3 localPosition,
            Vector3 localScale,
            Material material,
            bool collider)
        {
            var gameObject = GameObject.CreatePrimitive(primitiveType);
            gameObject.name = name;
            gameObject.transform.SetParent(parent, false);
            gameObject.transform.localPosition = localPosition;
            gameObject.transform.localRotation = Quaternion.identity;
            gameObject.transform.localScale = localScale;
            gameObject.GetComponent<Renderer>().sharedMaterial = material;
            gameObject.isStatic = collider;
            if (!collider)
            {
                UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<Collider>());
            }

            return gameObject;
        }

        private static void CreatePanelSign(
            string name,
            Transform parent,
            Vector3 position,
            Quaternion rotation,
            string text,
            Material accentMaterial,
            ExpandedFacilityMaterials materials,
            float width,
            float height,
            bool mountToCeiling = true)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent, false);
            root.transform.SetPositionAndRotation(position, rotation);

            CreateBoxLocal("Recessed Backing", root.transform, Vector3.zero, new Vector3(width, height, 0.08f), materials.Panel, false);
            CreateBoxLocal("Top Frame", root.transform, new Vector3(0f, height * 0.5f, -0.006f), new Vector3(width + 0.08f, 0.055f, 0.1f), materials.DarkTrim, false);
            CreateBoxLocal("Bottom Frame", root.transform, new Vector3(0f, -height * 0.5f, -0.006f), new Vector3(width + 0.08f, 0.055f, 0.1f), materials.DarkTrim, false);
            CreateBoxLocal("Left Frame", root.transform, new Vector3(-width * 0.5f, 0f, -0.006f), new Vector3(0.055f, height, 0.1f), materials.DarkTrim, false);
            CreateBoxLocal("Right Frame", root.transform, new Vector3(width * 0.5f, 0f, -0.006f), new Vector3(0.055f, height, 0.1f), materials.DarkTrim, false);

            CreateBoxLocal("Meaningful Accent", root.transform, new Vector3(0f, -height * 0.5f + 0.065f, -0.058f), new Vector3(width - 0.2f, 0.055f, 0.025f), accentMaterial, false);
            CreatePanelTextLocal(
                "Fitted Text",
                root.transform,
                new Vector3(0f, 0.025f, -0.057f),
                Quaternion.identity,
                text,
                Color.white,
                new Vector2(width - 0.26f, height - 0.17f),
                Mathf.Min(0.95f, height * 1.35f),
                true);

            if (!mountToCeiling)
            {
                return;
            }

            var supportLength = WallHeight - position.y - height * 0.5f;
            if (supportLength <= 0.08f)
            {
                return;
            }

            foreach (var x in new[] { -width * 0.34f, width * 0.34f })
            {
                CreateBoxLocal(
                    "Ceiling Mount",
                    root.transform,
                    new Vector3(x, height * 0.5f + supportLength * 0.5f, 0.04f),
                    new Vector3(0.055f, supportLength, 0.055f),
                    materials.DarkTrim,
                    false);
            }
        }

        private static TMP_Text CreatePanelTextLocal(
            string name,
            Transform parent,
            Vector3 localPosition,
            Quaternion localRotation,
            string text,
            Color color,
            Vector2 size,
            float fontSize,
            bool allowWrapping)
        {
            var labelObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshPro));
            labelObject.transform.SetParent(parent, false);
            labelObject.transform.localPosition = localPosition;
            labelObject.transform.localRotation = localRotation;
            labelObject.transform.localScale = Vector3.one;
            var rect = (RectTransform)labelObject.transform;
            rect.sizeDelta = size;
            var label = labelObject.GetComponent<TextMeshPro>();
            label.text = text;
            label.fontSize = fontSize;
            label.enableAutoSizing = true;
            label.fontSizeMin = Mathf.Max(0.3f, fontSize * 0.62f);
            label.fontSizeMax = fontSize;
            label.fontStyle = FontStyles.Bold;
            label.alignment = TextAlignmentOptions.Center;
            label.color = color;
            label.textWrappingMode = allowWrapping ? TextWrappingModes.Normal : TextWrappingModes.NoWrap;
            label.overflowMode = allowWrapping ? TextOverflowModes.Overflow : TextOverflowModes.Truncate;
            label.margin = new Vector4(0.05f, 0.02f, 0.05f, 0.02f);
            return label;
        }

        private static TMP_Text CreateFloorStencil(
            string name,
            Transform parent,
            Vector3 position,
            string text,
            Color color)
        {
            var labelObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshPro));
            labelObject.transform.SetParent(parent, false);
            labelObject.transform.SetPositionAndRotation(position, Quaternion.Euler(90f, 0f, 0f));
            labelObject.transform.localScale = Vector3.one;
            var rect = (RectTransform)labelObject.transform;
            rect.sizeDelta = new Vector2(1.25f, 0.38f);
            var label = labelObject.GetComponent<TextMeshPro>();
            label.text = text.ToUpperInvariant();
            label.fontSize = 0.72f;
            label.fontStyle = FontStyles.Bold;
            label.alignment = TextAlignmentOptions.Center;
            label.color = color;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.overflowMode = TextOverflowModes.Truncate;
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
