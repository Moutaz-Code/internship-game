using System;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace InternshipGame.Prototype.Editor
{
    /// <summary>
    /// Durable generator for the approved 2026-08-11 shell. It consumes only
    /// ApprovedFacilityLayout image-space data and FacilityKit source prefabs.
    /// </summary>
    internal static class ApprovedFacilityBuilder
    {
        public const string FacilityRootName = "Generated Approved Facility Shell";
        public const string OwnedRootName = "_Generated P02 Approved Facility";

        private static readonly FacilityPolygonSpec[] ContainmentFloorAreas =
        {
            new("FLOOR-CONTAINMENT-EAST", "Containment east gallery",
                new(884, 545), new(1055, 452), new(1195, 452), new(1320, 680), new(1455, 830),
                new(1455, 1125), new(1270, 1215), new(1120, 1160), new(1000, 1240), new(885, 1160)),
            new("FLOOR-REACTOR-NORTH", "Reactor north approach",
                new(690, 525), new(1010, 330), new(1060, 452), new(884, 640), new(884, 700), new(690, 610)),
            new("FLOOR-REACTOR-SOUTH", "Reactor south approach",
                new(884, 1040), new(1000, 1240), new(900, 1390), new(750, 1360), new(650, 1140), new(665, 1020)),
            new("FLOOR-SOUTH-CENTRAL-APPROACH", "South central approved passage",
                new(1098, 1100), new(1232, 1100), new(1232, 1475), new(1098, 1475)),
        };

        public static ApprovedFacilityShellResult Build(Transform parent)
        {
            var materials = FacilityKitAssetBuilder.BuildKitAssets();
            var root = new GameObject(FacilityRootName);
            root.transform.SetParent(parent, false);
            var source = root.AddComponent<ApprovedFacilitySourceReference>();
            source.ConfigureFromAuthority();

            BuildFloorAndCeiling(root.transform, materials);
            BuildWalls(root.transform, materials);
            BuildBlockedZones(root.transform, materials);
            BuildPortals(root.transform);
            ClearDeclaredPortalVolumes(root.transform);
            BuildReactor(root.transform, materials);
            BuildRoomIdentity(root.transform);
            BuildWindows(root.transform);
            BuildSemanticAnchors(root.transform, out var playerSpawn, out var owlSpawn);
            BuildLighting(root.transform, materials);
            var patrol = BuildOwlPatrolAnchors(root.transform);

            var missionDisplay = new GameObject("Mission Display Anchor").transform;
            missionDisplay.SetParent(root.transform, false);
            missionDisplay.position = ApprovedFacilityLayout.ImageToWorld(new Vector2(1160, 1740), 2.65f);
            missionDisplay.rotation = Quaternion.Euler(0f, 180f, 0f);

            return new ApprovedFacilityShellResult
            {
                Root = root,
                PlayerSpawn = playerSpawn,
                OwlSpawn = owlSpawn,
                MissionDisplayAnchor = missionDisplay,
                OwlPatrolPoints = patrol,
            };
        }

        private static void BuildFloorAndCeiling(
            Transform parent,
            FacilityKitAssetBuilder.FacilityKitMaterials materials)
        {
            var floors = new GameObject("Generated Floor Polygons");
            floors.transform.SetParent(parent, false);
            foreach (var room in ApprovedFacilityLayout.Rooms)
            {
                if (room.Id == "ROOM-CONTAINMENT") continue;
                CreatePolygonSurface($"{room.Id} Floor", floors.transform, room.ImagePoints, -0.10f, materials.Floor, true);
            }
            foreach (var area in ContainmentFloorAreas)
            {
                CreatePolygonSurface($"{area.Id} Floor", floors.transform, area.ImagePoints, -0.10f, materials.Floor, true);
            }

            // The entrance uses the drag-and-drop floor module directly; the remaining
            // irregular floors use deterministic polygon meshes generated from source data.
            var entranceFloor = InstantiateModule("FK-FLOOR-2X2", floors.transform);
            entranceFloor.name = "FacilityKit Floor Module Example In Actual Map";
            entranceFloor.transform.position = ApprovedFacilityLayout.ImageToWorld(new Vector2(1138, 1800));
            // The irregular entrance polygon owns collision/navigation. Keep this
            // source-prefab instance as the visible modular-use example without
            // creating a second, vertically offset NavMesh island at the spawn.
            foreach (var moduleCollider in entranceFloor.GetComponentsInChildren<Collider>(true))
            {
                UnityEngine.Object.DestroyImmediate(moduleCollider);
            }
            foreach (var teleportationArea in entranceFloor.GetComponentsInChildren<TeleportationArea>(true))
            {
                UnityEngine.Object.DestroyImmediate(teleportationArea);
            }

            // Semantic room polygons intentionally stop at their wall centerlines.
            // These deterministic pads bridge only the approved portal gaps so the
            // baked navigation surface agrees with the declared topology.
            foreach (var portal in ApprovedFacilityLayout.Portals)
            {
                var bridge = GameObject.CreatePrimitive(PrimitiveType.Cube);
                bridge.name = $"{portal.Id} Approved Portal Floor Bridge";
                bridge.transform.SetParent(floors.transform, false);
                // Cube top is coplanar with polygon floors at y=-0.10; offset
                // surfaces create separate NavMesh layers even across an overlap.
                bridge.transform.position = ApprovedFacilityLayout.ImageToWorld(portal.ImageCenter, -0.16f);
                bridge.transform.rotation = Quaternion.Euler(
                    0f,
                    ApprovedFacilityLayout.ImageYawToWorldYaw(portal.ImageYawDegrees),
                    0f);
                bridge.transform.localScale = new Vector3(portal.ClearWidthMeters, 0.12f, 4.0f);
                bridge.GetComponent<Renderer>().sharedMaterial = materials.Floor;
                bridge.isStatic = true;
                bridge.AddComponent<TeleportationArea>();
            }

            var ceilings = new GameObject("Ceilings (Toggleable For Overhead Inspection)");
            ceilings.transform.SetParent(parent, false);
            foreach (var room in ApprovedFacilityLayout.Rooms)
            {
                if (room.Id == "ROOM-CONTAINMENT") continue;
                CreatePolygonSurface($"{room.Id} Ceiling", ceilings.transform, room.ImagePoints, ApprovedFacilityLayout.WallHeight + 0.12f, materials.Wall, true);
            }
            foreach (var area in ContainmentFloorAreas)
            {
                CreatePolygonSurface($"{area.Id} Ceiling", ceilings.transform, area.ImagePoints, ApprovedFacilityLayout.WallHeight + 0.12f, materials.Wall, true);
            }
        }

        private static void BuildWalls(
            Transform parent,
            FacilityKitAssetBuilder.FacilityKitMaterials materials)
        {
            var root = new GameObject("Modular Wall Centerlines");
            root.transform.SetParent(parent, false);
            foreach (var path in ApprovedFacilityLayout.WallPaths)
            {
                var pathRoot = new GameObject(path.Id);
                pathRoot.transform.SetParent(root.transform, false);
                for (var i = 0; i < path.ImagePoints.Length - 1; i++)
                {
                    TileWallSegment(
                        $"{path.Id}-{i + 1:00}",
                        pathRoot.transform,
                        ApprovedFacilityLayout.ImageToWorld(path.ImagePoints[i]),
                        ApprovedFacilityLayout.ImageToWorld(path.ImagePoints[i + 1]),
                        path.Surface == FacilityShellSurface.OrdinaryWall ? null : materials.Shield);
                }

                for (var i = 1; i < path.ImagePoints.Length - 1; i++)
                {
                    var column = InstantiateModule("FK-COLUMN", pathRoot.transform);
                    column.name = $"{path.Id} Modular Corner {i:00}";
                    column.transform.position = ApprovedFacilityLayout.ImageToWorld(path.ImagePoints[i]);
                    if (path.Surface != FacilityShellSurface.OrdinaryWall)
                    {
                        OverrideMaterial(column, materials.Shield);
                    }
                }
            }
        }

        private static void TileWallSegment(
            string id,
            Transform parent,
            Vector3 start,
            Vector3 end,
            Material overrideMaterial)
        {
            var delta = end - start;
            var length = delta.magnitude;
            if (length < 0.08f) return;
            var direction = delta / length;
            var yaw = -Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            var travelled = 0f;
            var moduleIndex = 0;
            while (travelled < length - 0.01f)
            {
                var remaining = length - travelled;
                // One-metre pieces keep portal-volume trimming bounded to the
                // smallest architecture module instead of deleting a 4 m span.
                const float nominal = 1f;
                var pieceLength = Mathf.Min(nominal, remaining);
                if (remaining < 0.5f && moduleIndex > 0) break;
                const string moduleId = "FK-WALL-1M";
                var piece = InstantiateModule(moduleId, parent);
                piece.name = $"{id} Modular Wall {++moduleIndex:00}";
                piece.transform.position = start + direction * (travelled + pieceLength * 0.5f);
                piece.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
                piece.transform.localScale = new Vector3(pieceLength / nominal, 1f, 1f);
                if (overrideMaterial != null) OverrideMaterial(piece, overrideMaterial);
                travelled += pieceLength;
            }
        }

        private static void BuildBlockedZones(
            Transform parent,
            FacilityKitAssetBuilder.FacilityKitMaterials materials)
        {
            var root = new GameObject("Locked Blocked Zones (Non Playable)");
            root.transform.SetParent(parent, false);
            foreach (var blocked in ApprovedFacilityLayout.BlockedZones)
            {
                var mass = CreateExtrudedPolygon(
                    blocked.Id,
                    root.transform,
                    blocked.ImagePoints,
                    ApprovedFacilityLayout.WallHeight,
                    materials.Shield);
                var marker = mass.AddComponent<FacilitySemanticAnchor>();
                marker.Configure(blocked.Id, FacilitySemanticKind.BlockedZone, false, 1.5f);
            }
        }

        private static void BuildPortals(Transform parent)
        {
            var root = new GameObject("Approved Portal Frames");
            root.transform.SetParent(parent, false);
            foreach (var portal in ApprovedFacilityLayout.Portals)
            {
                var instance = InstantiateModule("FK-PORTAL-FRAME", root.transform);
                instance.name = portal.Id;
                instance.transform.position = ApprovedFacilityLayout.ImageToWorld(portal.ImageCenter);
                instance.transform.rotation = Quaternion.Euler(0f, ApprovedFacilityLayout.ImageYawToWorldYaw(portal.ImageYawDegrees), 0f);
                var marker = instance.AddComponent<FacilitySemanticAnchor>();
                marker.Configure(portal.Id, FacilitySemanticKind.Portal, true, portal.ClearWidthMeters * 0.5f);
                var navigationLink = instance.AddComponent<NavMeshLink>();
                navigationLink.startPoint = Vector3.back * 1.8f;
                navigationLink.endPoint = Vector3.forward * 1.8f;
                navigationLink.width = Mathf.Max(1.2f, portal.ClearWidthMeters - 0.5f);
                navigationLink.bidirectional = true;
                navigationLink.autoUpdate = true;
            }
        }

        private static void ClearDeclaredPortalVolumes(Transform facilityRoot)
        {
            var wallRoot = facilityRoot.Find("Modular Wall Centerlines");
            var portalRoot = facilityRoot.Find("Approved Portal Frames");
            if (wallRoot == null || portalRoot == null) return;

            Physics.SyncTransforms();
            var wallModulesToRemove = new HashSet<GameObject>();
            foreach (var portal in ApprovedFacilityLayout.Portals)
            {
                var portalInstance = portalRoot.Find(portal.Id);
                var center = ApprovedFacilityLayout.ImageToWorld(
                    portal.ImageCenter,
                    ApprovedFacilityLayout.PrimaryPortalClearHeight * 0.5f);
                var rotation = Quaternion.Euler(
                    0f,
                    ApprovedFacilityLayout.ImageYawToWorldYaw(portal.ImageYawDegrees),
                    0f);
                var halfExtents = new Vector3(
                    Mathf.Max(0.9f, portal.ClearWidthMeters * 0.5f - 0.13f),
                    ApprovedFacilityLayout.PrimaryPortalClearHeight * 0.5f - 0.06f,
                    0.72f);
                foreach (var hit in Physics.OverlapBox(
                             center,
                             halfExtents,
                             rotation,
                             ~0,
                             QueryTriggerInteraction.Ignore))
                {
                    if (hit == null) continue;
                    if (hit.transform.IsChildOf(wallRoot))
                    {
                        var module = FindDirectWallModule(hit.transform, wallRoot);
                        if (module != null) wallModulesToRemove.Add(module.gameObject);
                        continue;
                    }

                    // Some compact service intersections place two approved frames
                    // within one clearance volume. Keep both presentations, but let
                    // the currently inspected opening own collision in its throat.
                    if (hit.transform.IsChildOf(portalRoot) &&
                        (portalInstance == null || !hit.transform.IsChildOf(portalInstance)))
                    {
                        hit.enabled = false;
                    }
                }
            }

            foreach (var module in wallModulesToRemove)
            {
                if (module != null) UnityEngine.Object.DestroyImmediate(module);
            }
        }

        private static Transform FindDirectWallModule(Transform candidate, Transform wallRoot)
        {
            var current = candidate;
            while (current != null && current.parent != null)
            {
                if (current.parent.parent == wallRoot) return current;
                current = current.parent;
            }
            return null;
        }

        private static void BuildReactor(
            Transform parent,
            FacilityKitAssetBuilder.FacilityKitMaterials materials)
        {
            var root = new GameObject("REACTOR-CORE Inaccessible Continuous Boundary");
            root.transform.SetParent(parent, false);
            var center = ApprovedFacilityLayout.ImageToWorld(ApprovedFacilityLayout.ReactorImageCenter);
            var radius = ApprovedFacilityLayout.ReactorRadiusPixels * ApprovedFacilityLayout.PixelsToMeters;
            const int segmentCount = 30;
            const float step = 360f / segmentCount;
            for (var i = 0; i < segmentCount; i++)
            {
                var angle = -180f + step * (i + 0.5f);
                if (Mathf.Abs(Mathf.DeltaAngle(angle, 0f)) <= 18.1f) continue;
                var radians = angle * Mathf.Deg2Rad;
                var radial = new Vector3(Mathf.Cos(radians), 0f, -Mathf.Sin(radians));
                var segment = InstantiateModule("FK-REACTOR-SEGMENT", root.transform);
                segment.name = $"REACTOR-WALL-{i + 1:00}";
                segment.transform.position = center + radial * radius;
                segment.transform.rotation = Quaternion.LookRotation(radial, Vector3.up);
            }

            // Exactly one broad window object fills the one omitted facility-facing arc.
            var window = InstantiateModule("FK-WINDOW-REACTOR-LEADGLASS", root.transform);
            window.name = "REACTOR-LEAD-GLASS-WINDOW (Exactly One)";
            window.transform.position = center + Vector3.right * radius;
            window.transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
            var windowMarker = window.AddComponent<FacilitySemanticAnchor>();
            windowMarker.Configure("REACTOR-LEAD-GLASS-WINDOW", FacilitySemanticKind.Reactor, false, 2f);

            var interior = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            interior.name = "REACTOR-CORE No Floor No Objective No Navigation";
            interior.transform.SetParent(root.transform, false);
            interior.transform.position = center + Vector3.up * 1.25f;
            interior.transform.localScale = new Vector3(radius * 0.94f, 1.25f, radius * 0.94f);
            interior.GetComponent<Renderer>().sharedMaterial = materials.DarkTrim;
            var modifier = interior.AddComponent<NavMeshModifier>();
            modifier.overrideArea = true;
            modifier.area = NavMesh.GetAreaFromName("Not Walkable");
            var marker = interior.AddComponent<FacilitySemanticAnchor>();
            marker.Configure("REACTOR-CORE", FacilitySemanticKind.Reactor, false, radius);
        }

        private static void BuildRoomIdentity(Transform parent)
        {
            var root = new GameObject("Room Identity Hero Modules");
            root.transform.SetParent(parent, false);
            foreach (var hero in ApprovedFacilityLayout.HeroPlacements)
            {
                var instance = InstantiateModule(hero.PrefabId, root.transform);
                instance.name = hero.Id;
                instance.transform.position = ApprovedFacilityLayout.ImageToWorld(hero.ImagePosition);
                instance.transform.rotation = Quaternion.Euler(0f, ApprovedFacilityLayout.ImageYawToWorldYaw(hero.ImageYawDegrees), 0f);
            }
        }

        private static void BuildWindows(Transform parent)
        {
            var root = new GameObject("Ordinary Reinforced Windows");
            root.transform.SetParent(parent, false);
            var assay = InstantiateModule("FK-WINDOW-OBSERVATION", root.transform);
            assay.name = "Assay Reinforced Observation Window";
            assay.transform.position = ApprovedFacilityLayout.ImageToWorld(new Vector2(1015, 1585));
            assay.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
            var service = InstantiateModule("FK-WINDOW-SERVICE", root.transform);
            service.name = "Scrubber Narrow Service Window";
            service.transform.position = ApprovedFacilityLayout.ImageToWorld(new Vector2(1688, 905));
            service.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }

        private static void BuildSemanticAnchors(
            Transform parent,
            out Transform playerSpawn,
            out Transform owlSpawn)
        {
            var root = new GameObject("Semantic Anchors (Placeholders Explicitly Inactive)");
            root.transform.SetParent(parent, false);
            playerSpawn = null;
            owlSpawn = null;
            foreach (var spec in ApprovedFacilityLayout.SemanticAnchors)
            {
                var anchor = new GameObject(spec.Id);
                anchor.transform.SetParent(root.transform, false);
                anchor.transform.position = ApprovedFacilityLayout.ImageToWorld(spec.ImagePosition, 0.04f);
                anchor.transform.rotation = Quaternion.Euler(0f, ApprovedFacilityLayout.ImageYawToWorldYaw(spec.ImageYawDegrees), 0f);
                var marker = anchor.AddComponent<FacilitySemanticAnchor>();
                marker.Configure(spec.Id, spec.Kind, spec.GameplayActive, 1.35f);
                if (spec.Id == "ANCHOR-PLAYER-SPAWN") playerSpawn = anchor.transform;
                if (spec.Id == "ANCHOR-OWL-SPAWN") owlSpawn = anchor.transform;
                if (!spec.GameplayActive) anchor.SetActive(false);
            }

            if (playerSpawn == null || owlSpawn == null)
            {
                throw new InvalidOperationException("Approved facility player/Owl anchors are incomplete.");
            }
        }

        private static Transform[] BuildOwlPatrolAnchors(Transform parent)
        {
            var root = new GameObject("P02 Owl Route Nodes");
            root.transform.SetParent(parent, false);
            var specs = new[]
            {
                ("P02-ENT-01", new Vector2(1138, 1760)),
                ("P02-ASSAY-01", new Vector2(860, 1570)),
                ("P02-SOUTH-01", new Vector2(940, 1420)),
                ("P02-GALLERY-01", new Vector2(970, 1080)),
                ("P02-GALLERY-02", new Vector2(1220, 950)),
                ("P02-GALLERY-03", new Vector2(1120, 650)),
                ("P02-CONTROL-01", new Vector2(1060, 155)),
                ("P02-EXTC-01", new Vector2(880, 430)),
                ("P02-COOLANT-01", new Vector2(1390, 360)),
                ("P02-COOLANT-02", new Vector2(1580, 390)),
                ("P02-COOLANT-03", new Vector2(1730, 420)),
                ("P02-SERVICE-01", new Vector2(1560, 720)),
                ("P02-SERVICE-02", new Vector2(1630, 980)),
                ("P02-FUEL-01", new Vector2(1510, 1210)),
                ("P02-FUEL-02", new Vector2(1550, 1400)),
                ("P02-TOOL-01", new Vector2(1290, 1560)),
                ("P02-SEARCH-01", new Vector2(900, 760)),
                ("P02-SEARCH-02", new Vector2(1390, 1080)),
            };
            var result = new Transform[specs.Length];
            for (var i = 0; i < specs.Length; i++)
            {
                var node = new GameObject(specs[i].Item1);
                node.transform.SetParent(root.transform, false);
                node.transform.position = ApprovedFacilityLayout.ImageToWorld(specs[i].Item2, 0.01f);
                var metadata = node.AddComponent<OwlRouteNodeAnchor>();
                metadata.Configure(
                    specs[i].Item1,
                    i >= 16 ? OwlRouteNodeKind.SearchProbe : OwlRouteNodeKind.Lobe,
                    FacilityRouteFamily.None);
                result[i] = node.transform;
            }
            return result;
        }

        private static void BuildLighting(
            Transform parent,
            FacilityKitAssetBuilder.FacilityKitMaterials materials)
        {
            var root = new GameObject("P02 Greybox Lighting");
            root.transform.SetParent(parent, false);
            for (var i = 0; i < ApprovedFacilityLayout.Rooms.Length; i++)
            {
                var room = ApprovedFacilityLayout.Rooms[i];
                var center = CalculateCentroid(room.ImagePoints);
                var lightObject = new GameObject($"{room.Id} Work Light");
                lightObject.transform.SetParent(root.transform, false);
                lightObject.transform.position = ApprovedFacilityLayout.ImageToWorld(center, 2.8f);
                var light = lightObject.AddComponent<Light>();
                light.type = LightType.Point;
                light.range = 10f;
                light.intensity = 2.1f;
                light.shadows = LightShadows.Soft;
                light.color = room.Id switch
                {
                    "ROOM-COOLANT" => materials.Coolant.color,
                    "ROOM-SCRUBBER" => materials.Chemical.color,
                    "ROOM-FUEL-B" => materials.Fuel.color,
                    "ROOM-CONTROL-C" => materials.Amber.color,
                    "ROOM-ASSAY-A" => materials.Clinical.color,
                    _ => new Color(0.72f, 0.78f, 0.82f),
                };
            }
        }

        private static GameObject InstantiateModule(string stableId, Transform parent)
        {
            if (!FacilityKitCatalog.TryGet(stableId, out var entry))
            {
                throw new InvalidOperationException($"FacilityKit id is not cataloged: {stableId}");
            }
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(entry.PrefabPath);
            if (prefab == null)
            {
                throw new InvalidOperationException($"FacilityKit prefab is missing: {entry.PrefabPath}");
            }
            var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (instance == null) throw new InvalidOperationException($"Could not instantiate {entry.PrefabPath}");
            instance.transform.SetParent(parent, true);
            return instance;
        }

        private static void OverrideMaterial(GameObject root, Material material)
        {
            foreach (var renderer in root.GetComponentsInChildren<Renderer>(true)) renderer.sharedMaterial = material;
        }

        private static GameObject CreatePolygonSurface(
            string name,
            Transform parent,
            Vector2[] imagePoints,
            float y,
            Material material,
            bool collider)
        {
            var root = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));
            root.transform.SetParent(parent, false);
            var mesh = new Mesh { name = $"{name} Mesh" };
            var vertices = new Vector3[imagePoints.Length];
            var points = new Vector2[imagePoints.Length];
            for (var i = 0; i < imagePoints.Length; i++)
            {
                vertices[i] = ApprovedFacilityLayout.ImageToWorld(imagePoints[i], y);
                points[i] = new Vector2(vertices[i].x, vertices[i].z);
            }
            mesh.vertices = vertices;
            var triangles = Triangulate(points);
            // In X/Z plan space, a mathematically counter-clockwise triangle has a
            // downward Unity normal. Floors must face up for NavMesh collection;
            // ceilings intentionally retain the down-facing winding.
            if (y < 0f)
            {
                for (var i = 0; i < triangles.Length; i += 3)
                {
                    (triangles[i + 1], triangles[i + 2]) = (triangles[i + 2], triangles[i + 1]);
                }
            }
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            root.GetComponent<MeshFilter>().sharedMesh = mesh;
            root.GetComponent<MeshRenderer>().sharedMaterial = material;
            if (collider)
            {
                var meshCollider = root.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = mesh;
                root.isStatic = true;
                if (y < 0f) root.AddComponent<TeleportationArea>();
            }
            return root;
        }

        private static GameObject CreateExtrudedPolygon(
            string name,
            Transform parent,
            Vector2[] imagePoints,
            float height,
            Material material)
        {
            var root = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
            root.transform.SetParent(parent, false);
            var count = imagePoints.Length;
            var vertices = new Vector3[count * 2];
            var flat = new Vector2[count];
            for (var i = 0; i < count; i++)
            {
                var bottom = ApprovedFacilityLayout.ImageToWorld(imagePoints[i]);
                vertices[i] = bottom;
                vertices[i + count] = bottom + Vector3.up * height;
                flat[i] = new Vector2(bottom.x, bottom.z);
            }
            var top = Triangulate(flat);
            var triangles = new List<int>(top.Length * 2 + count * 6);
            for (var i = 0; i < top.Length; i += 3)
            {
                triangles.Add(top[i + 2]); triangles.Add(top[i + 1]); triangles.Add(top[i]);
                triangles.Add(top[i] + count); triangles.Add(top[i + 1] + count); triangles.Add(top[i + 2] + count);
            }
            for (var i = 0; i < count; i++)
            {
                var next = (i + 1) % count;
                triangles.Add(i); triangles.Add(next); triangles.Add(i + count);
                triangles.Add(next); triangles.Add(next + count); triangles.Add(i + count);
            }
            var mesh = new Mesh { name = $"{name} Mesh", vertices = vertices, triangles = triangles.ToArray() };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            root.GetComponent<MeshFilter>().sharedMesh = mesh;
            root.GetComponent<MeshRenderer>().sharedMaterial = material;
            root.GetComponent<MeshCollider>().sharedMesh = mesh;
            root.isStatic = true;
            return root;
        }

        private static int[] Triangulate(Vector2[] points)
        {
            if (points == null || points.Length < 3) return Array.Empty<int>();
            var indices = new List<int>();
            for (var i = 0; i < points.Length; i++) indices.Add(i);
            if (SignedArea(points) < 0f) indices.Reverse();
            var triangles = new List<int>((points.Length - 2) * 3);
            var guard = points.Length * points.Length;
            while (indices.Count > 2 && guard-- > 0)
            {
                var clipped = false;
                for (var i = 0; i < indices.Count; i++)
                {
                    var previous = indices[(i - 1 + indices.Count) % indices.Count];
                    var current = indices[i];
                    var next = indices[(i + 1) % indices.Count];
                    if (!IsConvex(points[previous], points[current], points[next])) continue;
                    var contains = false;
                    for (var j = 0; j < indices.Count; j++)
                    {
                        var test = indices[j];
                        if (test == previous || test == current || test == next) continue;
                        if (PointInTriangle(points[test], points[previous], points[current], points[next])) { contains = true; break; }
                    }
                    if (contains) continue;
                    triangles.Add(previous); triangles.Add(current); triangles.Add(next);
                    indices.RemoveAt(i);
                    clipped = true;
                    break;
                }
                if (!clipped) break;
            }
            return triangles.ToArray();
        }

        private static float SignedArea(Vector2[] points)
        {
            var area = 0f;
            for (var i = 0; i < points.Length; i++)
            {
                var next = (i + 1) % points.Length;
                area += points[i].x * points[next].y - points[next].x * points[i].y;
            }
            return area * 0.5f;
        }

        private static bool IsConvex(Vector2 a, Vector2 b, Vector2 c) =>
            (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x) > 0.00001f;

        private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            var ab = Cross(b - a, p - a);
            var bc = Cross(c - b, p - b);
            var ca = Cross(a - c, p - c);
            return ab >= -0.0001f && bc >= -0.0001f && ca >= -0.0001f;
        }

        private static float Cross(Vector2 a, Vector2 b) => a.x * b.y - a.y * b.x;

        private static Vector2 CalculateCentroid(Vector2[] points)
        {
            var sum = Vector2.zero;
            foreach (var point in points) sum += point;
            return points.Length > 0 ? sum / points.Length : ApprovedFacilityLayout.ImageOrigin;
        }
    }

    internal sealed class ApprovedFacilityShellResult
    {
        public GameObject Root;
        public Transform PlayerSpawn;
        public Transform OwlSpawn;
        public Transform MissionDisplayAnchor;
        public Transform[] OwlPatrolPoints;
    }
}
