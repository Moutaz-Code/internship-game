using System;
using System.Collections.Generic;
using UnityEngine;

namespace InternshipGame.Prototype
{
    public enum FacilityShellSurface
    {
        OrdinaryWall,
        Shielding,
        BlockedMass,
    }

    public enum FacilitySemanticKind
    {
        Room,
        Portal,
        BlockedZone,
        Reactor,
        ObjectivePlaceholder,
        ExtractionPlaceholder,
        GameplayAnchor,
    }

    [Serializable]
    public readonly struct FacilityWallPathSpec
    {
        public FacilityWallPathSpec(string id, FacilityShellSurface surface, params Vector2[] imagePoints)
        {
            Id = id;
            Surface = surface;
            ImagePoints = imagePoints ?? Array.Empty<Vector2>();
        }

        public string Id { get; }
        public FacilityShellSurface Surface { get; }
        public Vector2[] ImagePoints { get; }
    }

    [Serializable]
    public readonly struct FacilityPolygonSpec
    {
        public FacilityPolygonSpec(string id, string displayName, params Vector2[] imagePoints)
        {
            Id = id;
            DisplayName = displayName;
            ImagePoints = imagePoints ?? Array.Empty<Vector2>();
        }

        public string Id { get; }
        public string DisplayName { get; }
        public Vector2[] ImagePoints { get; }
    }

    [Serializable]
    public readonly struct FacilityPortalSpec
    {
        public FacilityPortalSpec(
            string id,
            string fromRoomId,
            string toRoomId,
            Vector2 imageCenter,
            float imageYawDegrees,
            float sourceGapPixels,
            float clearWidthMeters = ApprovedFacilityLayout.PrimaryPortalClearWidth)
        {
            Id = id;
            FromRoomId = fromRoomId;
            ToRoomId = toRoomId;
            ImageCenter = imageCenter;
            ImageYawDegrees = imageYawDegrees;
            SourceGapPixels = sourceGapPixels;
            ClearWidthMeters = clearWidthMeters;
        }

        public string Id { get; }
        public string FromRoomId { get; }
        public string ToRoomId { get; }
        public Vector2 ImageCenter { get; }
        public float ImageYawDegrees { get; }
        public float SourceGapPixels { get; }
        public float ClearWidthMeters { get; }
    }

    [Serializable]
    public readonly struct FacilityAnchorSpec
    {
        public FacilityAnchorSpec(
            string id,
            FacilitySemanticKind kind,
            Vector2 imagePosition,
            float imageYawDegrees = 0f,
            bool gameplayActive = false)
        {
            Id = id;
            Kind = kind;
            ImagePosition = imagePosition;
            ImageYawDegrees = imageYawDegrees;
            GameplayActive = gameplayActive;
        }

        public string Id { get; }
        public FacilitySemanticKind Kind { get; }
        public Vector2 ImagePosition { get; }
        public float ImageYawDegrees { get; }
        public bool GameplayActive { get; }
    }

    [Serializable]
    public readonly struct FacilityHeroPlacementSpec
    {
        public FacilityHeroPlacementSpec(string id, string prefabId, Vector2 imagePosition, float imageYawDegrees)
        {
            Id = id;
            PrefabId = prefabId;
            ImagePosition = imagePosition;
            ImageYawDegrees = imageYawDegrees;
        }

        public string Id { get; }
        public string PrefabId { get; }
        public Vector2 ImagePosition { get; }
        public float ImageYawDegrees { get; }
    }

    /// <summary>
    /// Durable image-space authority for the approved 2048 x 2048 facility drawing.
    /// The drawing is never stretched: X and Z use the same scalar and one explicit
    /// handedness conversion maps image-down to Unity north/up-plan.
    /// </summary>
    public static class ApprovedFacilityLayout
    {
        public const string LayoutId = "approved-facility-shell-2026-08-11";
        public const string SourceSha256 = "857275437965E59390FBE8939B8D0844FE404FB4DB77F94042F7A21BA1DC7969";
        public const int SourceWidthPixels = 2048;
        public const int SourceHeightPixels = 2048;
        public const float PixelsToMeters = 0.025f;
        public const float WorldYawDegrees = 0f;
        public const float WallHeight = 3.60f;
        public const float WallThickness = 0.30f;
        public const float PrimaryPortalClearWidth = 2.76f;
        public const float PrimaryPortalClearHeight = 2.52f;
        public const float MinimumCarriedClearance = 1.80f;
        public const float ReactorRadiusPixels = 294f;
        public const float ReactorWallSegmentDegrees = 12f;
        public const float ReactorWindowStartDegrees = -19f;
        public const float ReactorWindowEndDegrees = 19f;

        public static readonly Vector2 ImageOrigin = new(1024f, 1024f);
        public static readonly Vector3 WorldOrigin = Vector3.zero;
        public static readonly Vector2 ReactorImageCenter = new(586f, 906f);

        // Room envelopes are semantic/floor authoring polygons. Their walls remain
        // authoritative in WallPaths; envelopes may overlap at shared circulation.
        public static readonly FacilityPolygonSpec[] Rooms =
        {
            new("ROOM-ENTRANCE", "Contractor Decontamination and Dispatch",
                new(1000, 1475), new(1290, 1445), new(1410, 1510), new(1410, 1660),
                new(1380, 1690), new(1295, 1690), new(1295, 1900), new(990, 1900),
                new(990, 1690), new(1000, 1690)),
            new("ROOM-ASSAY-A", "Radiological Assay Laboratory",
                new(745, 1475), new(1015, 1475), new(1015, 1710), new(745, 1710)),
            new("ROOM-CONTROL-C", "Reactor Control and Instrumentation Gallery",
                new(780, 35), new(1295, 35), new(1385, 220), new(1285, 365), new(940, 330), new(780, 205)),
            new("ROOM-CONTAINMENT", "Containment Shield Gallery",
                new(665, 525), new(1055, 450), new(1195, 450), new(1320, 680), new(1455, 830),
                new(1455, 1125), new(1270, 1215), new(1120, 1160), new(1000, 1240), new(690, 1050)),
            new("ROOM-COOLANT", "Primary Coolant Pump Hall",
                new(1280, 255), new(1815, 255), new(1840, 430), new(1620, 575), new(1450, 560), new(1280, 360)),
            new("ROOM-SCRUBBER", "Air Filtration and Scrubber Service",
                new(1510, 660), new(1715, 660), new(1715, 1055), new(1510, 1055)),
            new("ROOM-FUEL-B", "Spent Fuel Transfer and Waste Handling",
                new(1435, 1160), new(1735, 1215), new(1735, 1515), new(1450, 1460)),
            new("ROOM-TOOL-CRIB", "Tool Crib and Maintenance Stores",
                new(1180, 1465), new(1405, 1465), new(1405, 1665), new(1180, 1665)),
            new("ROOM-EXTRACTION-A", "Vehicle Airlock",
                new(1540, 1620), new(1775, 1620), new(1775, 1845), new(1540, 1845)),
            new("ROOM-EXTRACTION-B", "Maintenance Lift",
                new(1580, 590), new(1800, 590), new(1800, 790), new(1580, 790)),
            new("ROOM-EXTRACTION-C", "Emergency Control Egress",
                new(775, 320), new(1015, 320), new(1015, 565), new(775, 565)),
        };

        public static readonly FacilityPolygonSpec[] BlockedZones =
        {
            new("BLOCKED-NORTH-SHIELD", "North control shielding",
                new(1018, 212), new(1215, 184), new(1286, 248), new(1285, 300), new(1235, 362), new(1075, 362), new(1018, 301)),
            new("BLOCKED-REACTOR-NECK", "Reactor service neck",
                new(675, 582), new(719, 618), new(810, 676), new(762, 711), new(706, 671)),
            new("BLOCKED-EAST-SERVICE", "East service mass",
                new(1372, 827), new(1512, 827), new(1512, 1014), new(1595, 1058), new(1576, 1103), new(1463, 1066), new(1372, 1019)),
            new("BLOCKED-SOUTH-WEST", "Southwest shield mass",
                new(875, 1152), new(1000, 1172), new(1112, 1126), new(1124, 1283), new(1070, 1372), new(900, 1372), new(850, 1292)),
            new("BLOCKED-SOUTH-EAST", "Southeast shield mass",
                new(1190, 1172), new(1370, 1150), new(1386, 1290), new(1460, 1372), new(1290, 1372), new(1210, 1285)),
        };

        // Each path is split at every approved opening. No builder-side gap inference
        // is permitted; the list is the reproducible wall-centerline authority.
        public static readonly FacilityWallPathSpec[] WallPaths =
        {
            new("W-CONTROL-NW", FacilityShellSurface.OrdinaryWall, new(786, 51), new(881, 53)),
            new("W-CONTROL-W", FacilityShellSurface.OrdinaryWall, new(786, 51), new(780, 204), new(929, 208)),
            new("W-CONTROL-SW", FacilityShellSurface.OrdinaryWall, new(1008, 207), new(1125, 206), new(1149, 183), new(1215, 184)),
            new("W-CONTROL-N", FacilityShellSurface.OrdinaryWall, new(957, 36), new(1214, 38)),
            new("W-CONTROL-NE", FacilityShellSurface.OrdinaryWall, new(1214, 38), new(1212, 128)),
            new("W-CONTROL-E", FacilityShellSurface.OrdinaryWall, new(1216, 61), new(1294, 63), new(1295, 156), new(1318, 157), new(1382, 220), new(1382, 300)),
            new("W-EXTC-NW", FacilityShellSurface.OrdinaryWall, new(934, 327), new(776, 474), new(842, 548), new(897, 548)),
            new("W-EXTC-SE", FacilityShellSurface.OrdinaryWall, new(897, 548), new(1014, 420), new(934, 327)),
            new("W-CONTROL-INNER-W", FacilityShellSurface.OrdinaryWall, new(934, 207), new(934, 326)),
            new("W-NORTH-BLOCK", FacilityShellSurface.Shielding, new(1018, 212), new(1018, 301), new(1075, 362), new(1235, 362), new(1285, 300), new(1286, 248), new(1215, 184)),

            new("W-COOLANT-NW", FacilityShellSurface.OrdinaryWall, new(1382, 255), new(1586, 258), new(1648, 326)),
            new("W-COOLANT-N", FacilityShellSurface.OrdinaryWall, new(1648, 326), new(1695, 275), new(1798, 276), new(1840, 324)),
            new("W-COOLANT-E", FacilityShellSurface.OrdinaryWall, new(1840, 324), new(1840, 430), new(1798, 480), new(1708, 479)),
            new("W-COOLANT-SE", FacilityShellSurface.OrdinaryWall, new(1654, 480), new(1615, 571), new(1540, 571)),
            new("W-COOLANT-SW", FacilityShellSurface.OrdinaryWall, new(1510, 570), new(1450, 570), new(1335, 564), new(1240, 360)),
            new("W-COOLANT-W", FacilityShellSurface.OrdinaryWall, new(1240, 360), new(1236, 302)),
            new("W-COOLANT-POD", FacilityShellSurface.OrdinaryWall, new(1708, 479), new(1760, 480), new(1800, 435)),

            new("W-GALLERY-NW", FacilityShellSurface.OrdinaryWall, new(830, 641), new(1056, 453), new(1190, 453), new(1314, 682)),
            new("W-GALLERY-E-NORTH", FacilityShellSurface.OrdinaryWall, new(1314, 682), new(1312, 826), new(1453, 826)),
            new("W-GALLERY-E-SOUTH", FacilityShellSurface.OrdinaryWall, new(1453, 878), new(1453, 1124), new(1360, 1210), new(1248, 1154)),
            new("W-GALLERY-SOUTH", FacilityShellSurface.OrdinaryWall, new(1121, 1157), new(1000, 1235), new(930, 1198)),
            new("W-GALLERY-SW", FacilityShellSurface.OrdinaryWall, new(884, 1165), new(819, 1120), new(656, 1015)),
            new("W-GALLERY-REACTOR-RETURN", FacilityShellSurface.OrdinaryWall, new(667, 838), new(704, 858)),

            new("W-SCRUBBER-N", FacilityShellSurface.OrdinaryWall, new(1510, 570), new(1510, 645), new(1540, 645)),
            new("W-SCRUBBER-W", FacilityShellSurface.OrdinaryWall, new(1453, 572), new(1453, 640), new(1510, 640)),
            new("W-SCRUBBER-E", FacilityShellSurface.OrdinaryWall, new(1615, 571), new(1615, 646), new(1580, 646)),
            new("W-EXTRACTION-B", FacilityShellSurface.OrdinaryWall, new(1615, 590), new(1750, 590), new(1795, 640), new(1795, 770), new(1765, 770)),
            new("W-SERVICE-SPINE-W", FacilityShellSurface.OrdinaryWall, new(1510, 645), new(1510, 825), new(1453, 825)),
            new("W-SERVICE-SPINE-E", FacilityShellSurface.OrdinaryWall, new(1580, 645), new(1580, 825), new(1600, 825)),
            new("W-SERVICE-MID", FacilityShellSurface.OrdinaryWall, new(1510, 820), new(1510, 1015)),
            new("W-SERVICE-SOUTH", FacilityShellSurface.OrdinaryWall, new(1578, 1104), new(1688, 1104), new(1688, 970)),
            new("W-SERVICE-E", FacilityShellSurface.OrdinaryWall, new(1688, 970), new(1688, 825), new(1600, 825)),
            new("W-EAST-BLOCK", FacilityShellSurface.Shielding, new(1372, 827), new(1512, 827), new(1512, 1014), new(1595, 1058), new(1576, 1103), new(1463, 1066), new(1372, 1019), new(1372, 827)),

            new("W-FUEL-N", FacilityShellSurface.OrdinaryWall, new(1576, 1104), new(1700, 1104), new(1700, 1210)),
            new("W-FUEL-E", FacilityShellSurface.OrdinaryWall, new(1700, 1210), new(1700, 1420), new(1650, 1470)),
            new("W-FUEL-S", FacilityShellSurface.OrdinaryWall, new(1650, 1470), new(1500, 1430), new(1430, 1395)),
            new("W-FUEL-W", FacilityShellSurface.OrdinaryWall, new(1430, 1395), new(1385, 1290)),
            new("W-FUEL-OBJB", FacilityShellSurface.OrdinaryWall, new(1600, 1160), new(1750, 1215), new(1700, 1395), new(1460, 1350)),

            new("W-SOUTH-BLOCK-W", FacilityShellSurface.Shielding, new(875, 1152), new(1000, 1172), new(1112, 1126), new(1124, 1283), new(1070, 1372), new(900, 1372), new(850, 1292), new(875, 1152)),
            new("W-SOUTH-BLOCK-E", FacilityShellSurface.Shielding, new(1190, 1172), new(1370, 1150), new(1386, 1290), new(1460, 1372), new(1290, 1372), new(1210, 1285), new(1190, 1172)),
            new("W-ASSAY-NW", FacilityShellSurface.OrdinaryWall, new(830, 1395), new(830, 1475), new(900, 1475)),
            new("W-ASSAY-N", FacilityShellSurface.OrdinaryWall, new(900, 1475), new(1000, 1475)),
            new("W-ASSAY-W", FacilityShellSurface.OrdinaryWall, new(745, 1510), new(745, 1650), new(790, 1695)),
            new("W-ASSAY-S", FacilityShellSurface.OrdinaryWall, new(790, 1695), new(975, 1695), new(1015, 1650)),
            new("W-ASSAY-E", FacilityShellSurface.OrdinaryWall, new(1015, 1650), new(1015, 1510)),

            new("W-DECON-NW", FacilityShellSurface.OrdinaryWall, new(1000, 1475), new(1080, 1475)),
            new("W-DECON-N", FacilityShellSurface.OrdinaryWall, new(1160, 1475), new(1260, 1475), new(1290, 1445), new(1410, 1510)),
            new("W-DECON-E", FacilityShellSurface.OrdinaryWall, new(1410, 1510), new(1410, 1660), new(1380, 1690)),
            new("W-DECON-S", FacilityShellSurface.OrdinaryWall, new(1380, 1690), new(1185, 1690)),
            new("W-DECON-INTERNAL", FacilityShellSurface.OrdinaryWall, new(1185, 1520), new(1185, 1655), new(1260, 1655)),
            new("W-ENTRANCE-W", FacilityShellSurface.OrdinaryWall, new(990, 1690), new(990, 1885), new(1070, 1885)),
            new("W-ENTRANCE-E", FacilityShellSurface.OrdinaryWall, new(1205, 1885), new(1295, 1885), new(1295, 1690)),
            new("W-ENTRANCE-N", FacilityShellSurface.OrdinaryWall, new(1015, 1690), new(1080, 1690)),
            new("W-ENTRANCE-N2", FacilityShellSurface.OrdinaryWall, new(1160, 1690), new(1295, 1690)),

            new("W-EXTRACTION-A-N", FacilityShellSurface.OrdinaryWall, new(1410, 1510), new(1510, 1570), new(1540, 1570)),
            new("W-EXTRACTION-A-W", FacilityShellSurface.OrdinaryWall, new(1540, 1570), new(1540, 1805)),
            new("W-EXTRACTION-A-E", FacilityShellSurface.OrdinaryWall, new(1775, 1620), new(1775, 1835), new(1540, 1835)),
            new("W-EXTRACTION-A-N2", FacilityShellSurface.OrdinaryWall, new(1635, 1620), new(1740, 1620), new(1775, 1655)),
        };

        public static readonly FacilityPortalSpec[] Portals =
        {
            new("PORTAL-ENTRY-EXTERIOR", "EXTERIOR", "ROOM-ENTRANCE", new(1138, 1886), 0f, 135f, 3.38f),
            new("PORTAL-ENTRANCE-DECON", "ROOM-ENTRANCE", "ROOM-TOOL-CRIB", new(1120, 1690), 0f, 80f),
            new("PORTAL-ASSAY-DECON", "ROOM-ASSAY-A", "ROOM-ENTRANCE", new(1015, 1485), 90f, 70f),
            new("PORTAL-DECON-TOOL", "ROOM-ENTRANCE", "ROOM-TOOL-CRIB", new(1185, 1485), 90f, 80f),
            new("PORTAL-DECON-EXTRACTION-A", "ROOM-ENTRANCE", "ROOM-EXTRACTION-A", new(1540, 1600), 90f, 92f),
            new("PORTAL-ASSAY-CONTAINMENT", "ROOM-ASSAY-A", "ROOM-CONTAINMENT", new(830, 1435), 90f, 72f),
            new("PORTAL-DECON-CONTAINMENT", "ROOM-ENTRANCE", "ROOM-CONTAINMENT", new(1120, 1450), 0f, 82f),
            new("PORTAL-FUEL-OBJECTIVE-B", "ROOM-FUEL-B", "OBJECTIVE-B", new(1510, 1420), 0f, 86f),
            new("PORTAL-CONTAINMENT-FUEL", "ROOM-CONTAINMENT", "ROOM-FUEL-B", new(1418, 1135), 90f, 92f),
            new("PORTAL-FUEL-SCRUBBER", "ROOM-FUEL-B", "ROOM-SCRUBBER", new(1580, 1085), 0f, 88f),
            new("PORTAL-SCRUBBER-EXTRACTION-B", "ROOM-SCRUBBER", "ROOM-EXTRACTION-B", new(1580, 610), 0f, 80f),
            new("PORTAL-SCRUBBER-COOLANT", "ROOM-SCRUBBER", "ROOM-COOLANT", new(1545, 570), 90f, 76f),
            new("PORTAL-CONTAINMENT-COOLANT", "ROOM-CONTAINMENT", "ROOM-COOLANT", new(1428, 570), 90f, 90f),
            new("PORTAL-CONTROL-COOLANT", "ROOM-CONTROL-C", "ROOM-COOLANT", new(1310, 300), 90f, 78f),
            new("PORTAL-CONTROL-CONTAINMENT", "ROOM-CONTROL-C", "ROOM-CONTAINMENT", new(970, 208), 0f, 78f),
            new("PORTAL-CONTROL-EXTRACTION-C", "ROOM-CONTROL-C", "ROOM-EXTRACTION-C", new(934, 327), 90f, 78f),
            new("PORTAL-EXTRACTION-C-CONTAINMENT", "ROOM-EXTRACTION-C", "ROOM-CONTAINMENT", new(815, 520), 45f, 82f),
        };

        public static readonly FacilityAnchorSpec[] SemanticAnchors =
        {
            new("ANCHOR-OBJECTIVE-A", FacilitySemanticKind.ObjectivePlaceholder, new(880, 1590)),
            new("ANCHOR-OBJECTIVE-B", FacilitySemanticKind.ObjectivePlaceholder, new(1585, 1370)),
            new("ANCHOR-OBJECTIVE-C", FacilitySemanticKind.ObjectivePlaceholder, new(1050, 122)),
            new("ANCHOR-EXTRACTION-A", FacilitySemanticKind.ExtractionPlaceholder, new(1660, 1730), 0f),
            new("ANCHOR-EXTRACTION-B", FacilitySemanticKind.ExtractionPlaceholder, new(1700, 685), 90f),
            new("ANCHOR-EXTRACTION-C", FacilitySemanticKind.ExtractionPlaceholder, new(885, 435), -45f),
            new("ANCHOR-PLAYER-SPAWN", FacilitySemanticKind.GameplayAnchor, new(1138, 1835), 0f, true),
            new("ANCHOR-OWL-SPAWN", FacilitySemanticKind.GameplayAnchor, new(1340, 880), 180f, true),
        };

        public static readonly FacilityHeroPlacementSpec[] HeroPlacements =
        {
            new("HERO-DECON", "FK-DECON-FRAME", new(1115, 1770), 0f),
            new("HERO-ASSAY-BENCH", "FK-ASSAY-BENCH", new(850, 1550), 0f),
            new("HERO-ASSAY-ISOLATION", "FK-ISOLATION-CABINET", new(940, 1550), 0f),
            new("HERO-CONTROL-BANK", "FK-CONTROL-CONSOLE-STRAIGHT", new(1110, 92), 0f),
            new("HERO-CONTAINMENT-SHIELD-A", "FK-SHIELD-STRAIGHT", new(1015, 725), -16f),
            new("HERO-CONTAINMENT-SHIELD-B", "FK-SHIELD-STRAIGHT", new(1185, 830), 90f),
            new("HERO-CONTAINMENT-SHIELD-C", "FK-SHIELD-STRAIGHT", new(1060, 1015), 10f),
            new("HERO-CONTAINMENT-SHIELD-D", "FK-SHIELD-CORNER", new(1285, 980), 0f),
            new("HERO-COOLANT-BASIN", "FK-COOLANT-BASIN", new(1500, 390), 0f),
            new("HERO-COOLANT-PUMP-A", "FK-PUMP-A", new(1380, 345), -12f),
            new("HERO-COOLANT-PUMP-B", "FK-PUMP-B", new(1735, 370), 8f),
            new("HERO-COOLANT-PUMP-C", "FK-PUMP-C", new(1390, 490), 12f),
            new("HERO-SCRUBBER", "FK-SCRUBBER-VESSEL", new(1615, 875), 0f),
            new("HERO-DUCT-BANK", "FK-DUCT-BANK", new(1660, 930), 90f),
            new("HERO-FUEL-CASK-ROW-A", "FK-CASK-ROW", new(1575, 1265), 0f),
            new("HERO-FUEL-CASK-ROW-B", "FK-CASK-ROW", new(1600, 1320), 0f),
            new("HERO-FUEL-CRANE", "FK-OVERHEAD-CRANE", new(1570, 1225), 0f),
            new("HERO-TOOL-CAGE", "FK-TOOL-CAGE", new(1365, 1620), 90f),
            new("HERO-PARTS-CART", "FK-PARTS-CART", new(1360, 1515), 90f),
            new("HERO-EXTRACTION-A", "FK-EXTRACTION-VEHICLE", new(1660, 1725), 0f),
            new("HERO-EXTRACTION-B", "FK-EXTRACTION-LIFT", new(1700, 690), 90f),
            new("HERO-EXTRACTION-C", "FK-EXTRACTION-EGRESS", new(880, 430), -45f),
        };

        public static Vector3 ImageToWorld(Vector2 imagePoint, float y = 0f)
        {
            var local = new Vector3(
                (imagePoint.x - ImageOrigin.x) * PixelsToMeters,
                y,
                (ImageOrigin.y - imagePoint.y) * PixelsToMeters);
            return WorldOrigin + Quaternion.Euler(0f, WorldYawDegrees, 0f) * local;
        }

        public static Vector2 WorldToImage(Vector3 worldPoint)
        {
            var local = Quaternion.Euler(0f, -WorldYawDegrees, 0f) * (worldPoint - WorldOrigin);
            return new Vector2(
                local.x / PixelsToMeters + ImageOrigin.x,
                ImageOrigin.y - local.z / PixelsToMeters);
        }

        public static float ImageYawToWorldYaw(float imageYawDegrees)
        {
            return WorldYawDegrees - imageYawDegrees;
        }

        public static IEnumerable<string> EnumerateStableIds()
        {
            foreach (var room in Rooms) yield return room.Id;
            foreach (var blocked in BlockedZones) yield return blocked.Id;
            foreach (var wall in WallPaths) yield return wall.Id;
            foreach (var portal in Portals) yield return portal.Id;
            foreach (var anchor in SemanticAnchors) yield return anchor.Id;
            foreach (var hero in HeroPlacements) yield return hero.Id;
            yield return "REACTOR-CORE";
            yield return "REACTOR-LEAD-GLASS-WINDOW";
        }
    }
}
