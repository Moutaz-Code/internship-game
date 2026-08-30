using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InternshipGame.Prototype.Editor
{
    /// <summary>
    /// Adds disposable collider children to the manually placed P02 wall shell.
    /// Imported prefabs and existing user-authored colliders are never modified.
    /// </summary>
    internal static class P02WallColliderGenerator
    {
        private const string ScenePath = "Assets/_Project/Scenes/P02_ApprovedFacilityShell.unity";
        private const string OwnedPrefix = "__P02_GeneratedWallCollider_";

        private static readonly string[] ScopeRootNames = { "Rooms", "Hallways" };

        [MenuItem("Prototype/Approved Facility/Rebuild Manual Wall Colliders")]
        public static void Rebuild()
        {
            var scene = RequireActiveP02Scene();
            var undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Rebuild P02 manual wall colliders");

            var report = new BuildReport();
            foreach (var wallRoot in EnumeratePlacedRoots(scene))
            {
                report.PlacedRoots++;
                report.RemovedOwnedObjects += RemoveOwnedColliderChildren(wallRoot);

                var prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(wallRoot.gameObject);
                if (ShouldExcludeRoot(wallRoot, prefabPath))
                {
                    report.ExcludedRoots++;
                    continue;
                }

                if (HasExistingSolidCollider(wallRoot))
                {
                    report.ExistingColliderRoots++;
                    continue;
                }

                var meshFilters = wallRoot.GetComponentsInChildren<MeshFilter>(true)
                    .Where(IsEligibleMesh)
                    .ToArray();

                if (meshFilters.Length == 0)
                {
                    report.MeshlessRoots++;
                    continue;
                }

                var useVisualMesh = RequiresVisualMeshCollider(wallRoot, prefabPath);
                for (var index = 0; index < meshFilters.Length; index++)
                {
                    var colliderObject = CreateOwnedColliderObject(meshFilters[index], index);
                    if (useVisualMesh)
                    {
                        var collider = Undo.AddComponent<MeshCollider>(colliderObject);
                        collider.sharedMesh = meshFilters[index].sharedMesh;
                        collider.convex = false;
                        ConfigureNavMeshBlocking(colliderObject);
                        report.MeshColliders++;
                    }
                    else
                    {
                        // A number of the hand-placed wall prefabs are mirrored. Unity rejects a
                        // BoxCollider whose effective (lossy) scale contains a negative axis, so
                        // counter-mirror only the disposable collider child. Adjusting its center
                        // at the same time keeps it aligned with an off-centre source mesh.
                        var scaleSign = GetScaleSign(meshFilters[index].transform.lossyScale);
                        colliderObject.transform.localScale = scaleSign;
                        var collider = Undo.AddComponent<BoxCollider>(colliderObject);
                        collider.center = Vector3.Scale(meshFilters[index].sharedMesh.bounds.center, scaleSign);
                        collider.size = meshFilters[index].sharedMesh.bounds.size;
                        ConfigureNavMeshBlocking(colliderObject);
                        report.BoxColliders++;
                    }
                }

                report.GeneratedRoots++;
            }

            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene))
            {
                throw new InvalidOperationException($"Failed to save '{ScenePath}'.");
            }

            Undo.CollapseUndoOperations(undoGroup);
            var validation = ValidateScene(scene, throwOnFailure: true);
            Debug.Log($"P02_WALL_COLLIDERS_REBUILT|{report}|{validation}");
        }

        [MenuItem("Prototype/Approved Facility/Validate Manual Wall Colliders")]
        public static void Validate()
        {
            var scene = RequireActiveP02Scene();
            Debug.Log($"P02_WALL_COLLIDERS_VALID|{ValidateScene(scene, throwOnFailure: true)}");
        }

        private static Scene RequireActiveP02Scene()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                throw new InvalidOperationException("Exit Play Mode before rebuilding P02 wall colliders.");
            }

            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid() || !scene.isLoaded || !string.Equals(scene.path, ScenePath, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"The active scene must be '{ScenePath}'.");
            }

            return scene;
        }

        private static IEnumerable<Transform> EnumeratePlacedRoots(Scene scene)
        {
            foreach (var scopeName in ScopeRootNames)
            {
                var scope = scene.GetRootGameObjects().FirstOrDefault(root => root.name == scopeName);
                if (scope == null)
                {
                    throw new InvalidOperationException($"P02 is missing the '{scopeName}' root.");
                }

                foreach (Transform category in scope.transform)
                {
                    foreach (Transform placedRoot in category)
                    {
                        yield return placedRoot;
                    }
                }
            }
        }

        private static int RemoveOwnedColliderChildren(Transform wallRoot)
        {
            var owned = wallRoot.GetComponentsInChildren<Transform>(true)
                .Where(transform => transform != wallRoot && transform.name.StartsWith(OwnedPrefix, StringComparison.Ordinal))
                .Select(transform => transform.gameObject)
                .ToArray();

            foreach (var ownedObject in owned)
            {
                Undo.DestroyObjectImmediate(ownedObject);
            }

            return owned.Length;
        }

        private static bool ShouldExcludeRoot(Transform wallRoot, string prefabPath)
        {
            var name = wallRoot.name;
            return ContainsIgnoreCase(prefabPath, "/Doors/") ||
                   ContainsIgnoreCase(prefabPath, "/Stuff/") ||
                   name.StartsWith("Door", StringComparison.OrdinalIgnoreCase) ||
                   name.StartsWith("PREFAB__Door", StringComparison.OrdinalIgnoreCase);
        }

        private static bool HasExistingSolidCollider(Transform wallRoot)
        {
            return wallRoot.GetComponentsInChildren<Collider>(true)
                .Any(collider => !collider.isTrigger &&
                                 !collider.name.StartsWith(OwnedPrefix, StringComparison.Ordinal));
        }

        private static bool IsEligibleMesh(MeshFilter meshFilter)
        {
            if (meshFilter == null || meshFilter.sharedMesh == null) return false;
            if (meshFilter.transform.name.StartsWith(OwnedPrefix, StringComparison.Ordinal)) return false;

            var size = meshFilter.sharedMesh.bounds.size;
            return size.x > 0.05f && size.y > 0.05f && size.z > 0.05f;
        }

        private static bool RequiresVisualMeshCollider(Transform wallRoot, string prefabPath)
        {
            return ContainsIgnoreCase(prefabPath, "eighth_column") ||
                   ContainsIgnoreCase(prefabPath, "TurnLarge_90") ||
                   ContainsIgnoreCase(prefabPath, "TurnShort_90") ||
                   ContainsIgnoreCase(wallRoot.name, "eighth_column") ||
                   ContainsIgnoreCase(wallRoot.name, "TurnLarge_90") ||
                   ContainsIgnoreCase(wallRoot.name, "TurnShort_90");
        }

        private static GameObject CreateOwnedColliderObject(MeshFilter source, int index)
        {
            var colliderObject = new GameObject(OwnedPrefix + index.ToString("D2"));
            Undo.RegisterCreatedObjectUndo(colliderObject, "Create P02 wall collider");
            colliderObject.layer = source.gameObject.layer;
            colliderObject.transform.SetParent(source.transform, false);
            colliderObject.transform.localPosition = Vector3.zero;
            colliderObject.transform.localRotation = Quaternion.identity;
            colliderObject.transform.localScale = Vector3.one;
            GameObjectUtility.SetStaticEditorFlags(
                colliderObject,
                GameObjectUtility.GetStaticEditorFlags(source.gameObject));
            return colliderObject;
        }

        private static Vector3 GetScaleSign(Vector3 lossyScale)
        {
            return new Vector3(
                lossyScale.x < 0f ? -1f : 1f,
                lossyScale.y < 0f ? -1f : 1f,
                lossyScale.z < 0f ? -1f : 1f);
        }

        private static void ConfigureNavMeshBlocking(GameObject colliderObject)
        {
            var modifier = Undo.AddComponent<NavMeshModifier>(colliderObject);
            modifier.overrideArea = true;
            modifier.area = UnityEngine.AI.NavMesh.GetAreaFromName("Not Walkable");
        }

        private static ValidationReport ValidateScene(Scene scene, bool throwOnFailure)
        {
            var report = new ValidationReport();
            foreach (var wallRoot in EnumeratePlacedRoots(scene))
            {
                report.PlacedRoots++;
                var prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(wallRoot.gameObject);
                var excluded = ShouldExcludeRoot(wallRoot, prefabPath);
                var ownedObjects = wallRoot.GetComponentsInChildren<Transform>(true)
                    .Where(transform => transform.name.StartsWith(OwnedPrefix, StringComparison.Ordinal))
                    .ToArray();

                if (excluded)
                {
                    report.ExcludedRoots++;
                    if (ownedObjects.Length > 0) report.ExcludedRootsWithGeneratedColliders++;
                    continue;
                }

                var eligibleMeshes = wallRoot.GetComponentsInChildren<MeshFilter>(true).Count(IsEligibleMesh);
                if (eligibleMeshes == 0)
                {
                    report.MeshlessRoots++;
                    continue;
                }

                var solidColliders = wallRoot.GetComponentsInChildren<Collider>(true)
                    .Where(collider => !collider.isTrigger)
                    .ToArray();
                if (solidColliders.Length == 0) report.UncoveredRoots++;

                foreach (var ownedObject in ownedObjects)
                {
                    var colliders = ownedObject.GetComponents<Collider>();
                    if (colliders.Length != 1 || colliders[0].isTrigger) report.InvalidOwnedObjects++;
                    if (ownedObject.GetComponent<Renderer>() != null) report.InvalidOwnedObjects++;
                    var modifier = ownedObject.GetComponent<NavMeshModifier>();
                    if (modifier == null || !modifier.overrideArea ||
                        modifier.area != UnityEngine.AI.NavMesh.GetAreaFromName("Not Walkable"))
                    {
                        report.InvalidNavMeshModifiers++;
                    }
                    if (colliders.Length == 1 && colliders[0] is BoxCollider)
                    {
                        var scale = ownedObject.transform.lossyScale;
                        if (scale.x < 0f || scale.y < 0f || scale.z < 0f)
                        {
                            report.NegativeScaleBoxColliders++;
                        }
                    }
                    report.OwnedColliderObjects++;
                }

                if (ownedObjects.GroupBy(item => item.parent).Any(group =>
                        group.GroupBy(item => item.name).Any(names => names.Count() > 1)))
                {
                    report.DuplicateOwnedObjects++;
                }
            }

            if (throwOnFailure && !report.IsValid)
            {
                throw new InvalidOperationException($"P02 wall collider validation failed: {report}");
            }

            return report;
        }

        private static bool ContainsIgnoreCase(string value, string fragment)
        {
            return !string.IsNullOrEmpty(value) &&
                   value.IndexOf(fragment, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private sealed class BuildReport
        {
            public int PlacedRoots;
            public int GeneratedRoots;
            public int ExistingColliderRoots;
            public int ExcludedRoots;
            public int MeshlessRoots;
            public int RemovedOwnedObjects;
            public int BoxColliders;
            public int MeshColliders;

            public override string ToString()
            {
                return $"placedRoots={PlacedRoots}|generatedRoots={GeneratedRoots}" +
                       $"|existingColliderRoots={ExistingColliderRoots}|excludedRoots={ExcludedRoots}" +
                       $"|meshlessRoots={MeshlessRoots}|removedOwnedObjects={RemovedOwnedObjects}" +
                       $"|boxColliders={BoxColliders}|meshColliders={MeshColliders}";
            }
        }

        private sealed class ValidationReport
        {
            public int PlacedRoots;
            public int ExcludedRoots;
            public int MeshlessRoots;
            public int OwnedColliderObjects;
            public int UncoveredRoots;
            public int InvalidOwnedObjects;
            public int NegativeScaleBoxColliders;
            public int InvalidNavMeshModifiers;
            public int DuplicateOwnedObjects;
            public int ExcludedRootsWithGeneratedColliders;

            public bool IsValid => UncoveredRoots == 0 &&
                                   InvalidOwnedObjects == 0 &&
                                   NegativeScaleBoxColliders == 0 &&
                                   InvalidNavMeshModifiers == 0 &&
                                   DuplicateOwnedObjects == 0 &&
                                   ExcludedRootsWithGeneratedColliders == 0;

            public override string ToString()
            {
                return $"placedRoots={PlacedRoots}|ownedColliderObjects={OwnedColliderObjects}" +
                       $"|excludedRoots={ExcludedRoots}|meshlessRoots={MeshlessRoots}" +
                       $"|uncoveredRoots={UncoveredRoots}|invalidOwnedObjects={InvalidOwnedObjects}" +
                       $"|negativeScaleBoxColliders={NegativeScaleBoxColliders}" +
                       $"|invalidNavMeshModifiers={InvalidNavMeshModifiers}" +
                       $"|duplicateOwnedObjects={DuplicateOwnedObjects}" +
                       $"|excludedRootsWithGeneratedColliders={ExcludedRootsWithGeneratedColliders}";
            }
        }
    }
}
