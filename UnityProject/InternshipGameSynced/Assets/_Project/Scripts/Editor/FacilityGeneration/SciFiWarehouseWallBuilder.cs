using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InternshipGame.Prototype.Editor
{
    /// <summary>
    /// Places only wall-family prefab instances over the active FloorPlan.
    /// The exact owned root is disposable; all manual scene content is preserved.
    /// </summary>
    internal static class SciFiWarehouseWallBuilder
    {
        internal const string OwnedRootName = "_Generated SciFi Warehouse Walls (Agent)";
        internal const string PlanObjectName = "FloorPlan";

        private const string WallPrefabPath =
            "Assets/SciFi Warehouse Kit/Prefabs/Structures/Walls/Wall Plain.prefab";
        private const string DoorPrefabPath =
            "Assets/SciFi Warehouse Kit/Prefabs/Structures/Walls/Wall Door.prefab";
        private const string BayDoorPrefabPath =
            "Assets/SciFi Warehouse Kit/Prefabs/Structures/Walls/Wall BayDoor.prefab";
        private const string WindowPrefabPath =
            "Assets/SciFi Warehouse Kit/Prefabs/Structures/Walls/Wall Window.prefab";
        private const string SourcePlanTexturePath = "Assets/InternshipHorrorMap.png";

        private const float ModuleLength = 6f;
        private const float PrefabPivotFloorOffset = 1f;
        private const float WallJoinOverlap = 0.12f;
        private const int ReactorSegmentCount = 30;
        private const int ReactorWindowHalfSegmentCount = 2;

        [MenuItem("Prototype/Approved Facility/Rebuild SciFi Warehouse Walls Only")]
        public static void Rebuild()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                throw new InvalidOperationException("Exit Play Mode before rebuilding warehouse walls.");
            }

            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid() || !scene.isLoaded)
            {
                throw new InvalidOperationException("A loaded scene is required.");
            }

            var plan = FindRoot(scene, PlanObjectName);
            if (plan == null)
            {
                throw new InvalidOperationException(
                    $"The active scene must contain a root named '{PlanObjectName}'.");
            }

            var planRenderer = plan.GetComponent<Renderer>();
            if (planRenderer == null)
            {
                throw new InvalidOperationException("FloorPlan requires a Renderer.");
            }

            ValidatePlan(plan.transform, planRenderer.bounds);
            var wallPrefab = LoadPrefab(WallPrefabPath);
            var doorPrefab = LoadPrefab(DoorPrefabPath);
            var bayDoorPrefab = LoadPrefab(BayDoorPrefabPath);
            var windowPrefab = LoadPrefab(WindowPrefabPath);

            var existing = FindRoot(scene, OwnedRootName);
            if (existing != null)
            {
                UnityEngine.Object.DestroyImmediate(existing);
            }

            var root = new GameObject(OwnedRootName);
            SceneManager.MoveGameObjectToScene(root, scene);
            Undo.RegisterCreatedObjectUndo(root, "Build SciFi warehouse wall shell");

            var wallRoot = CreateGroup("Walls — Wall Plain Prefab Instances", root.transform);
            var doorRoot = CreateGroup("Doors — Open Approved Portal Instances", root.transform);
            var reactorRoot = CreateGroup("Reactor Wall — Wall Family Instances", root.transform);

            var context = new PlanContext(plan.transform);
            using var ink = new PlanInkMap(SourcePlanTexturePath);
            var tracedRuns = TraceWallRuns(ink);
            var wallCount = BuildWallPaths(context, scene, wallRoot, wallPrefab, tracedRuns);
            var doorCount = BuildDoors(
                context,
                scene,
                doorRoot,
                doorPrefab,
                bayDoorPrefab,
                DetectDoorGaps(tracedRuns, ink));
            var reactorCount = BuildReactor(context, scene, reactorRoot, wallPrefab, windowPrefab);

            Selection.activeGameObject = root;
            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene))
            {
                throw new InvalidOperationException($"Failed to save scene '{scene.path}'.");
            }

            Debug.Log(
                $"SCIFI_WAREHOUSE_WALLS_REBUILT|scene={scene.path}" +
                $"|walls={wallCount}|doors={doorCount}|reactorPieces={reactorCount}" +
                $"|ownedRoot={OwnedRootName}|manualContent=preserved");
        }

        [MenuItem("Prototype/Approved Facility/Validate SciFi Warehouse Walls")]
        public static void ValidateGeneratedPlacement()
        {
            var scene = SceneManager.GetActiveScene();
            var root = FindRoot(scene, OwnedRootName);
            var plan = FindRoot(scene, PlanObjectName);
            if (root == null || plan == null)
            {
                throw new InvalidOperationException("The generated wall root and FloorPlan are both required.");
            }

            var context = new PlanContext(plan.transform);
            var walls = root.transform.GetChild(0);
            var doors = root.transform.GetChild(1);
            var wallCount = 0;
            var unsupportedWalls = 0;
            var duplicateNames = 0;
            var seamGaps = 0;
            var doorInkFailures = 0;
            var blockedDoorRays = 0;
            var failureDetails = new System.Text.StringBuilder();
            var siblingNames = new HashSet<string>();
            var wallSeries = new Dictionary<string, List<Transform>>();

            using var ink = new PlanInkMap(SourcePlanTexturePath);
            foreach (var wall in walls.GetComponentsInChildren<Transform>(true))
            {
                if (!PrefabUtility.IsAnyPrefabInstanceRoot(wall.gameObject)) continue;
                wallCount++;
                var start = context.PlanToImage(wall.position);
                var end = context.PlanToImage(wall.position + wall.forward * (ModuleLength * wall.localScale.z));
                var wallInkFraction = ink.InkFractionAlong(start, end, 2f, 4);
                if (wallInkFraction < 0.65f && Vector2.Distance(start, end) >= 20f)
                {
                    unsupportedWalls++;
                    failureDetails.Append($"|wall:{wall.name}:{wallInkFraction:F2}");
                }

                var siblingKey = wall.parent.name + "|" + wall.name;
                if (!siblingNames.Add(siblingKey)) duplicateNames++;

                var separator = wall.name.LastIndexOf(" Wall ", StringComparison.Ordinal);
                var seriesKey = wall.parent.name + "|" +
                                (separator >= 0 ? wall.name.Substring(0, separator) : wall.name);
                if (!wallSeries.TryGetValue(seriesKey, out var series))
                {
                    series = new List<Transform>();
                    wallSeries.Add(seriesKey, series);
                }

                series.Add(wall);
            }

            foreach (var series in wallSeries.Values)
            {
                series.Sort((left, right) => string.CompareOrdinal(left.name, right.name));
                for (var i = 0; i + 1 < series.Count; i++)
                {
                    var end = series[i].position + series[i].forward * (ModuleLength * series[i].localScale.z);
                    if (Vector3.Distance(end, series[i + 1].position) > 0.02f) seamGaps++;
                }
            }

            foreach (Transform door in doors)
            {
                var length = ModuleLength * door.localScale.z;
                var start = context.PlanToImage(door.position);
                var end = context.PlanToImage(door.position + door.forward * length);
                var doorInkFraction = ink.InkFractionAlong(start, end, 5f, 2);
                if (doorInkFraction > 0.35f)
                {
                    doorInkFailures++;
                    failureDetails.Append($"|door:{door.name}:{doorInkFraction:F2}");
                }

                var center = door.position + door.forward * (length * 0.5f) + context.Normal * 1.2f;
                var ray = new Ray(center - door.right * 3f, door.right);
                foreach (var collider in door.GetComponentsInChildren<Collider>(true))
                {
                    if (!collider.enabled || !collider.gameObject.activeInHierarchy) continue;
                    if (!collider.Raycast(ray, out _, 6f)) continue;
                    blockedDoorRays++;
                    break;
                }
            }

            Debug.Log(
                $"SCIFI_WAREHOUSE_WALLS_VALIDATED|walls={wallCount}" +
                $"|unsupportedWallCenterlines={unsupportedWalls}" +
                $"|duplicateSiblingNames={duplicateNames}|moduleSeamGaps={seamGaps}" +
                $"|doors={doors.childCount}|doorInkFailures={doorInkFailures}" +
                $"|blockedDoorCenterRays={blockedDoorRays}|sceneDirty={scene.isDirty}" +
                failureDetails);
        }

        private static int BuildWallPaths(
            PlanContext context,
            Scene scene,
            Transform parent,
            GameObject wallPrefab,
            IReadOnlyList<ImageWallRun> tracedRuns)
        {
            var count = 0;
            var groups = new Dictionary<string, Transform>();
            foreach (var run in tracedRuns)
            {
                if (!groups.TryGetValue(run.SourcePathId, out var pathRoot))
                {
                    pathRoot = CreateGroup(run.SourcePathId, parent);
                    groups.Add(run.SourcePathId, pathRoot);
                }

                for (var i = 0; i + 1 < run.ImagePoints.Length; i++)
                {
                    count += BuildTiledSegment(
                        context,
                        scene,
                        pathRoot,
                        wallPrefab,
                        $"{run.SourcePathId} R{run.RunIndex:00} S{i + 1:00}",
                        context.ImageToPlan(run.ImagePoints[i]),
                        context.ImageToPlan(run.ImagePoints[i + 1]),
                        true);
                }
            }

            return count;
        }

        private static List<ImageWallRun> TraceWallRuns(PlanInkMap ink)
        {
            var result = new List<ImageWallRun>();
            foreach (var path in ApprovedFacilityLayout.WallPaths)
            {
                // This legacy helper had no supporting stroke in the approved drawing and
                // previously produced two visibly orphaned modules inside the reactor.
                if (path.Id == "W-GALLERY-REACTOR-RETURN") continue;

                var runIndex = 0;
                for (var segmentIndex = 0; segmentIndex + 1 < path.ImagePoints.Length; segmentIndex++)
                {
                    var searchRadius = path.Id == "W-SERVICE-E" || path.Id == "W-SCRUBBER-W"
                        ? 90
                        : 60;
                    var segmentRuns = new List<Vector2[]>(TraceSegment(
                        path.ImagePoints[segmentIndex],
                        path.ImagePoints[segmentIndex + 1],
                        searchRadius,
                        ink));
                    if (path.Surface == FacilityShellSurface.Shielding && segmentRuns.Count > 1)
                    {
                        var longest = segmentRuns[0];
                        var longestLength = PolylineLength(longest);
                        for (var candidateIndex = 1; candidateIndex < segmentRuns.Count; candidateIndex++)
                        {
                            var candidateLength = PolylineLength(segmentRuns[candidateIndex]);
                            if (candidateLength <= longestLength) continue;
                            longest = segmentRuns[candidateIndex];
                            longestLength = candidateLength;
                        }

                        segmentRuns.Clear();
                        segmentRuns.Add(longest);
                    }

                    foreach (var points in segmentRuns)
                    {
                        if (IsKnownLabelArtifact(path.Id, points)) continue;
                        if (IsDuplicateRun(points, result)) continue;
                        result.Add(new ImageWallRun(path.Id, ++runIndex, points));
                    }
                }
            }

            return result;
        }

        private static bool IsKnownLabelArtifact(string sourcePathId, IReadOnlyList<Vector2> points)
        {
            if (sourcePathId != "W-FUEL-OBJB") return false;
            var bounds = new Rect(1440f, 1318f, 255f, 48f);
            var contained = 0;
            foreach (var point in points)
            {
                if (bounds.Contains(point)) contained++;
            }

            return contained >= Mathf.CeilToInt(points.Count * 0.8f);
        }

        private static IEnumerable<Vector2[]> TraceSegment(
            Vector2 start,
            Vector2 end,
            int searchRadiusPixels,
            PlanInkMap ink)
        {
            const float sampleSpacing = 5f;
            const int offsetStep = 2;
            const int maximumOffsetChangeSteps = 6;
            const int allowedUnsupportedSamples = 2;
            const float minimumRunLengthPixels = 18f;

            var delta = end - start;
            var length = delta.magnitude;
            if (length < minimumRunLengthPixels) yield break;

            var direction = delta / length;
            var normal = new Vector2(-direction.y, direction.x);
            var sampleCount = Mathf.Max(3, Mathf.CeilToInt(length / sampleSpacing) + 1);
            var offsetCount = searchRadiusPixels * 2 / offsetStep + 1;
            var scores = new float[sampleCount, offsetCount];
            var previous = new short[sampleCount, offsetCount];
            var supported = new bool[sampleCount, offsetCount];

            for (var sampleIndex = 0; sampleIndex < sampleCount; sampleIndex++)
            {
                var t = sampleIndex / (float)(sampleCount - 1);
                var basePoint = Vector2.Lerp(start, end, t);
                for (var offsetIndex = 0; offsetIndex < offsetCount; offsetIndex++)
                {
                    var offset = -searchRadiusPixels + offsetIndex * offsetStep;
                    var candidate = basePoint + normal * offset;
                    var insideReactor = (candidate - ApprovedFacilityLayout.ReactorImageCenter).sqrMagnitude <
                                        Mathf.Pow(ApprovedFacilityLayout.ReactorRadiusPixels - 8f, 2f);
                    supported[sampleIndex, offsetIndex] = !insideReactor && ink.IsInkNear(candidate, 4);
                    var emission = (supported[sampleIndex, offsetIndex] ? 2f : 0f) -
                                   Mathf.Abs(offset) * 0.002f;

                    if (sampleIndex == 0)
                    {
                        scores[sampleIndex, offsetIndex] = emission;
                        continue;
                    }

                    var fromMin = Mathf.Max(0, offsetIndex - maximumOffsetChangeSteps);
                    var fromMax = Mathf.Min(offsetCount - 1, offsetIndex + maximumOffsetChangeSteps);
                    var bestScore = float.NegativeInfinity;
                    var bestPrevious = fromMin;
                    for (var from = fromMin; from <= fromMax; from++)
                    {
                        var transition = Mathf.Abs(from - offsetIndex) * offsetStep * 0.025f;
                        var candidateScore = scores[sampleIndex - 1, from] - transition;
                        if (candidateScore <= bestScore) continue;
                        bestScore = candidateScore;
                        bestPrevious = from;
                    }

                    scores[sampleIndex, offsetIndex] = bestScore + emission;
                    previous[sampleIndex, offsetIndex] = (short)bestPrevious;
                }
            }

            var chosenOffsets = new int[sampleCount];
            var finalOffsetIndex = 0;
            for (var offsetIndex = 1; offsetIndex < offsetCount; offsetIndex++)
            {
                if (scores[sampleCount - 1, offsetIndex] > scores[sampleCount - 1, finalOffsetIndex])
                {
                    finalOffsetIndex = offsetIndex;
                }
            }

            for (var sampleIndex = sampleCount - 1; sampleIndex >= 0; sampleIndex--)
            {
                chosenOffsets[sampleIndex] = finalOffsetIndex;
                if (sampleIndex > 0) finalOffsetIndex = previous[sampleIndex, finalOffsetIndex];
            }

            var tracedPoints = new Vector2[sampleCount];
            var tracedSupport = new bool[sampleCount];
            for (var sampleIndex = 0; sampleIndex < sampleCount; sampleIndex++)
            {
                var t = sampleIndex / (float)(sampleCount - 1);
                var offset = -searchRadiusPixels + chosenOffsets[sampleIndex] * offsetStep;
                var candidate = Vector2.Lerp(start, end, t) + normal * offset;
                tracedSupport[sampleIndex] = supported[sampleIndex, chosenOffsets[sampleIndex]];
                tracedPoints[sampleIndex] = tracedSupport[sampleIndex]
                    ? ink.SnapToInk(candidate, 5)
                    : candidate;
            }

            var runStart = -1;
            var lastSupported = -1;
            var unsupportedCount = 0;
            for (var sampleIndex = 0; sampleIndex <= sampleCount; sampleIndex++)
            {
                var hasSupport = sampleIndex < sampleCount && tracedSupport[sampleIndex];
                if (hasSupport)
                {
                    if (runStart < 0) runStart = sampleIndex;
                    lastSupported = sampleIndex;
                    unsupportedCount = 0;
                    continue;
                }

                if (runStart < 0) continue;
                unsupportedCount++;
                if (sampleIndex < sampleCount && unsupportedCount <= allowedUnsupportedSamples) continue;

                if (lastSupported > runStart &&
                    Vector2.Distance(tracedPoints[runStart], tracedPoints[lastSupported]) >= minimumRunLengthPixels)
                {
                    var supportedSamples = 0;
                    for (var supportIndex = runStart; supportIndex <= lastSupported; supportIndex++)
                    {
                        if (tracedSupport[supportIndex]) supportedSamples++;
                    }

                    var continuity = supportedSamples / (float)(lastSupported - runStart + 1);
                    if (continuity < 0.78f)
                    {
                        runStart = -1;
                        lastSupported = -1;
                        unsupportedCount = 0;
                        continue;
                    }

                    var run = new List<Vector2>();
                    for (var pointIndex = runStart; pointIndex <= lastSupported; pointIndex++)
                    {
                        run.Add(tracedPoints[pointIndex]);
                    }

                    var simplified = SimplifyPolyline(run, 2.5f);
                    if (simplified.Length >= 2) yield return simplified;
                }

                runStart = -1;
                lastSupported = -1;
                unsupportedCount = 0;
            }
        }

        private static float PolylineLength(IReadOnlyList<Vector2> points)
        {
            var length = 0f;
            for (var i = 0; i + 1 < points.Count; i++)
            {
                length += Vector2.Distance(points[i], points[i + 1]);
            }

            return length;
        }

        private static Vector2[] SimplifyPolyline(IReadOnlyList<Vector2> points, float tolerancePixels)
        {
            if (points.Count <= 2) return new[] { points[0], points[points.Count - 1] };
            var keep = new bool[points.Count];
            keep[0] = true;
            keep[points.Count - 1] = true;
            SimplifyPolylineRange(points, 0, points.Count - 1, tolerancePixels, keep);
            var result = new List<Vector2>();
            for (var i = 0; i < points.Count; i++)
            {
                if (keep[i]) result.Add(points[i]);
            }

            return result.ToArray();
        }

        private static void SimplifyPolylineRange(
            IReadOnlyList<Vector2> points,
            int first,
            int last,
            float tolerancePixels,
            bool[] keep)
        {
            if (last <= first + 1) return;
            var maximumDistance = 0f;
            var maximumIndex = -1;
            for (var i = first + 1; i < last; i++)
            {
                var distance = DistanceToSegment(points[i], points[first], points[last]);
                if (distance <= maximumDistance) continue;
                maximumDistance = distance;
                maximumIndex = i;
            }

            if (maximumDistance <= tolerancePixels || maximumIndex < 0) return;
            keep[maximumIndex] = true;
            SimplifyPolylineRange(points, first, maximumIndex, tolerancePixels, keep);
            SimplifyPolylineRange(points, maximumIndex, last, tolerancePixels, keep);
        }

        private static bool IsDuplicateRun(Vector2[] points, IReadOnlyList<ImageWallRun> acceptedRuns)
        {
            if (acceptedRuns.Count == 0) return false;
            var supported = 0;
            foreach (var point in points)
            {
                var alreadyCovered = false;
                foreach (var run in acceptedRuns)
                {
                    for (var i = 0; i + 1 < run.ImagePoints.Length; i++)
                    {
                        if (DistanceToSegment(point, run.ImagePoints[i], run.ImagePoints[i + 1]) > 5f) continue;
                        alreadyCovered = true;
                        break;
                    }

                    if (alreadyCovered) break;
                }

                if (alreadyCovered) supported++;
            }

            return supported >= Mathf.CeilToInt(points.Length * 0.9f);
        }

        private static List<ImageDoorGap> DetectDoorGaps(
            IReadOnlyList<ImageWallRun> wallRuns,
            PlanInkMap ink)
        {
            const float joinedEndpointDistance = 15f;
            const float minimumDoorGap = 22f;
            const float maximumDoorGap = 155f;
            const float minimumFacingDot = 0.92f;
            const float maximumInkFraction = 0.18f;

            var endpoints = new List<ImageWallEndpoint>();
            for (var runIndex = 0; runIndex < wallRuns.Count; runIndex++)
            {
                var points = wallRuns[runIndex].ImagePoints;
                if (points.Length < 2) continue;
                endpoints.Add(new ImageWallEndpoint(
                    runIndex,
                    points[0],
                    (points[0] - points[1]).normalized));
                endpoints.Add(new ImageWallEndpoint(
                    runIndex,
                    points[points.Length - 1],
                    (points[points.Length - 1] - points[points.Length - 2]).normalized));
            }

            var joined = new bool[endpoints.Count];
            for (var endpointIndex = 0; endpointIndex < endpoints.Count; endpointIndex++)
            {
                for (var runIndex = 0; runIndex < wallRuns.Count && !joined[endpointIndex]; runIndex++)
                {
                    if (runIndex == endpoints[endpointIndex].RunIndex) continue;
                    var points = wallRuns[runIndex].ImagePoints;
                    for (var i = 0; i + 1 < points.Length; i++)
                    {
                        if (DistanceToSegment(endpoints[endpointIndex].Position, points[i], points[i + 1]) >
                            joinedEndpointDistance) continue;
                        joined[endpointIndex] = true;
                        break;
                    }
                }
            }

            var candidates = new List<ImageDoorCandidate>();
            for (var aIndex = 0; aIndex < endpoints.Count; aIndex++)
            {
                if (joined[aIndex]) continue;
                for (var bIndex = aIndex + 1; bIndex < endpoints.Count; bIndex++)
                {
                    if (joined[bIndex] || endpoints[aIndex].RunIndex == endpoints[bIndex].RunIndex) continue;
                    var gap = endpoints[bIndex].Position - endpoints[aIndex].Position;
                    var length = gap.magnitude;
                    if (length < minimumDoorGap || length > maximumDoorGap) continue;
                    var direction = gap / length;
                    if (Vector2.Dot(endpoints[aIndex].Outward, direction) < minimumFacingDot ||
                        Vector2.Dot(endpoints[bIndex].Outward, -direction) < minimumFacingDot)
                    {
                        continue;
                    }

                    if (ink.InkFractionAlong(
                            endpoints[aIndex].Position,
                            endpoints[bIndex].Position,
                            6f) > maximumInkFraction)
                    {
                        continue;
                    }

                    var center = (endpoints[aIndex].Position + endpoints[bIndex].Position) * 0.5f;
                    if ((center - ApprovedFacilityLayout.ReactorImageCenter).magnitude <
                        ApprovedFacilityLayout.ReactorRadiusPixels + 8f)
                    {
                        continue;
                    }

                    candidates.Add(new ImageDoorCandidate(aIndex, bIndex, length));
                }
            }

            candidates.Sort((left, right) => left.LengthPixels.CompareTo(right.LengthPixels));
            var used = new bool[endpoints.Count];
            var gaps = new List<ImageDoorGap>();
            foreach (var candidate in candidates)
            {
                if (used[candidate.EndpointA] || used[candidate.EndpointB]) continue;
                used[candidate.EndpointA] = true;
                used[candidate.EndpointB] = true;
                gaps.Add(new ImageDoorGap(
                    string.Empty,
                    endpoints[candidate.EndpointA].Position,
                    endpoints[candidate.EndpointB].Position));
            }

            // These three hand-drawn openings include short jambs at their ends, so the
            // endpoint graph correctly recognizes the jambs as joins. Add the visible gaps
            // explicitly after the graph pass; coordinates are the traced ink endpoints.
            AddDoorGapIfUnique(gaps, new Vector2(881f, 53f), new Vector2(957f, 36f));
            AddDoorGapIfUnique(gaps, new Vector2(929f, 208f), new Vector2(1008f, 207f));
            AddDoorGapIfUnique(gaps, new Vector2(1540f, 645f), new Vector2(1596f, 638f));

            for (var index = gaps.Count - 1; index >= 0; index--)
            {
                if (gaps[index].LengthPixels <= 96f || gaps[index].ImageCenter.y >= 1800f) continue;
                gaps.RemoveAt(index);
            }

            gaps.Sort((left, right) =>
            {
                var y = left.ImageCenter.y.CompareTo(right.ImageCenter.y);
                return y != 0 ? y : left.ImageCenter.x.CompareTo(right.ImageCenter.x);
            });
            for (var index = 0; index < gaps.Count; index++)
            {
                gaps[index] = new ImageDoorGap(
                    $"AUTO-DOOR-{index + 1:00}",
                    gaps[index].ImageStart,
                    gaps[index].ImageEnd);
            }

            return gaps;
        }

        private static void AddDoorGapIfUnique(List<ImageDoorGap> gaps, Vector2 start, Vector2 end)
        {
            var center = (start + end) * 0.5f;
            foreach (var gap in gaps)
            {
                if (Vector2.Distance(center, gap.ImageCenter) < 20f) return;
            }

            gaps.Add(new ImageDoorGap(string.Empty, start, end));
        }

        private static float DistanceToSegment(Vector2 point, Vector2 start, Vector2 end)
        {
            var delta = end - start;
            var denominator = delta.sqrMagnitude;
            if (denominator < 0.0001f) return Vector2.Distance(point, start);
            var t = Mathf.Clamp01(Vector2.Dot(point - start, delta) / denominator);
            return Vector2.Distance(point, start + delta * t);
        }

        private static int BuildTiledSegment(
            PlanContext context,
            Scene scene,
            Transform parent,
            GameObject prefab,
            string stablePrefix,
            Vector3 start,
            Vector3 end,
            bool overlapEnds = false)
        {
            var delta = Vector3.ProjectOnPlane(end - start, context.Normal);
            var length = delta.magnitude;
            if (length < 0.05f) return 0;

            var direction = delta / length;
            if (overlapEnds)
            {
                start -= direction * WallJoinOverlap;
                length += WallJoinOverlap * 2f;
            }

            var rotation = Quaternion.LookRotation(direction, context.Normal);
            var travelled = 0f;
            var index = 0;
            while (travelled < length - 0.01f)
            {
                var pieceLength = Mathf.Min(ModuleLength, length - travelled);
                var instance = InstantiatePrefab(prefab, scene, parent);
                instance.name = $"{stablePrefix} Wall {++index:00}";
                instance.transform.SetPositionAndRotation(
                    start + direction * travelled + context.Normal * PrefabPivotFloorOffset,
                    rotation);
                instance.transform.localScale = new Vector3(1f, 1f, pieceLength / ModuleLength);
                travelled += pieceLength;
            }

            return index;
        }

        private static int BuildDoors(
            PlanContext context,
            Scene scene,
            Transform parent,
            GameObject doorPrefab,
            GameObject bayDoorPrefab,
            IReadOnlyList<ImageDoorGap> doorGaps)
        {
            var count = 0;
            foreach (var gap in doorGaps)
            {
                var broadVehicleOpening = gap.LengthPixels >= 96f;
                var prefab = broadVehicleOpening ? bayDoorPrefab : doorPrefab;
                var start = context.ImageToPlan(gap.ImageStart);
                var end = context.ImageToPlan(gap.ImageEnd);
                var tangent = Vector3.ProjectOnPlane(end - start, context.Normal).normalized;
                var gapLength = Vector3.Distance(start, end);

                var instance = InstantiatePrefab(prefab, scene, parent);
                instance.name = $"{gap.Id} {(broadVehicleOpening ? "Bay Door" : "Open Door")}";
                instance.transform.SetPositionAndRotation(
                    start + context.Normal * PrefabPivotFloorOffset,
                    Quaternion.LookRotation(tangent, context.Normal));
                instance.transform.localScale = new Vector3(1f, 1f, gapLength / ModuleLength);
                if (broadVehicleOpening)
                {
                    ConfigureBayDoorAsOpen(instance);
                }

                count++;
            }

            return count;
        }

        private static void ConfigureBayDoorAsOpen(GameObject instance)
        {
            // The kit ships this prefab with a visible closed leaf and an oversized wall
            // collider spanning the opening. Keep its frame and wall geometry while making
            // these two approved route instances visibly and physically open.
            var closedLeaf = instance.transform.Find("LP_Bay_Door_snaps");
            if (closedLeaf != null)
            {
                closedLeaf.gameObject.SetActive(false);
            }

            Collider openingBlocker = null;
            var largestVolume = 0f;
            foreach (var collider in instance.GetComponentsInChildren<Collider>(true))
            {
                var size = collider.bounds.size;
                var volume = size.x * size.y * size.z;
                if (volume <= largestVolume) continue;
                largestVolume = volume;
                openingBlocker = collider;
            }

            if (openingBlocker != null)
            {
                openingBlocker.enabled = false;
            }
        }

        private static int BuildReactor(
            PlanContext context,
            Scene scene,
            Transform parent,
            GameObject wallPrefab,
            GameObject windowPrefab)
        {
            var center = ApprovedFacilityLayout.ReactorImageCenter;
            var radius = ApprovedFacilityLayout.ReactorRadiusPixels;
            var step = 360f / ReactorSegmentCount;
            var count = 0;

            for (var i = 0; i < ReactorSegmentCount; i++)
            {
                var startDegrees = -180f + step * i;
                var endDegrees = startDegrees + step;
                var centerDegrees = startDegrees + step * 0.5f;
                if (Mathf.Abs(Mathf.DeltaAngle(centerDegrees, 0f)) <= 18.1f)
                {
                    continue;
                }

                var start = context.ImageToPlan(PointOnImageCircle(center, radius, startDegrees));
                var end = context.ImageToPlan(PointOnImageCircle(center, radius, endDegrees));
                count += BuildTiledSegment(
                    context,
                    scene,
                    parent,
                    wallPrefab,
                    $"REACTOR S{i + 1:00}",
                    start,
                    end);
            }

            // One stretched wall-window prefab owns the complete four-segment east chord.
            var halfAngle = step * ReactorWindowHalfSegmentCount;
            var windowStart = context.ImageToPlan(PointOnImageCircle(center, radius, -halfAngle));
            var windowEnd = context.ImageToPlan(PointOnImageCircle(center, radius, halfAngle));
            var delta = Vector3.ProjectOnPlane(windowEnd - windowStart, context.Normal);
            var window = InstantiatePrefab(windowPrefab, scene, parent);
            window.name = "REACTOR Broad Observation Window Wall";
            window.transform.SetPositionAndRotation(
                windowStart + context.Normal * PrefabPivotFloorOffset,
                Quaternion.LookRotation(delta.normalized, context.Normal));
            window.transform.localScale = new Vector3(1f, 1f, delta.magnitude / ModuleLength);
            return count + 1;
        }

        private static Vector2 PointOnImageCircle(Vector2 center, float radius, float degrees)
        {
            var radians = degrees * Mathf.Deg2Rad;
            return center + new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
        }

        private static void ValidatePlan(Transform plan, Bounds rendererBounds)
        {
            var xSpan = plan.TransformVector(Vector3.right).magnitude;
            var ySpan = plan.TransformVector(Vector3.up).magnitude;
            if (Mathf.Abs(xSpan - ySpan) > 0.01f)
            {
                throw new InvalidOperationException(
                    $"FloorPlan must use a uniform image scale. Current spans are {xSpan:F3} and {ySpan:F3} m.");
            }

            var normal = plan.TransformDirection(Vector3.back).normalized;
            if (Vector3.Dot(normal, Vector3.up) < 0.98f || rendererBounds.size.x < 1f || rendererBounds.size.z < 1f)
            {
                throw new InvalidOperationException("FloorPlan must lie horizontally with its visible side facing +Y.");
            }
        }

        private static GameObject LoadPrefab(string path)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                throw new InvalidOperationException($"Required warehouse prefab is missing: {path}");
            }

            return prefab;
        }

        private static GameObject InstantiatePrefab(GameObject prefab, Scene scene, Transform parent)
        {
            var instance = PrefabUtility.InstantiatePrefab(prefab, scene) as GameObject;
            if (instance == null)
            {
                throw new InvalidOperationException($"Failed to instantiate prefab '{prefab.name}'.");
            }

            instance.transform.SetParent(parent, true);
            return instance;
        }

        private static Transform CreateGroup(string name, Transform parent)
        {
            var group = new GameObject(name);
            group.transform.SetParent(parent, false);
            return group.transform;
        }

        private static GameObject FindRoot(Scene scene, string rootName)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == rootName) return root;
            }

            return null;
        }

        private readonly struct ImageWallRun
        {
            public ImageWallRun(string sourcePathId, int runIndex, Vector2[] imagePoints)
            {
                SourcePathId = sourcePathId;
                RunIndex = runIndex;
                ImagePoints = imagePoints;
            }

            public string SourcePathId { get; }
            public int RunIndex { get; }
            public Vector2[] ImagePoints { get; }
        }

        private readonly struct ImageWallEndpoint
        {
            public ImageWallEndpoint(int runIndex, Vector2 position, Vector2 outward)
            {
                RunIndex = runIndex;
                Position = position;
                Outward = outward;
            }

            public int RunIndex { get; }
            public Vector2 Position { get; }
            public Vector2 Outward { get; }
        }

        private readonly struct ImageDoorCandidate
        {
            public ImageDoorCandidate(int endpointA, int endpointB, float lengthPixels)
            {
                EndpointA = endpointA;
                EndpointB = endpointB;
                LengthPixels = lengthPixels;
            }

            public int EndpointA { get; }
            public int EndpointB { get; }
            public float LengthPixels { get; }
        }

        private readonly struct ImageDoorGap
        {
            public ImageDoorGap(string id, Vector2 imageStart, Vector2 imageEnd)
            {
                Id = id;
                ImageStart = imageStart;
                ImageEnd = imageEnd;
            }

            public string Id { get; }
            public Vector2 ImageStart { get; }
            public Vector2 ImageEnd { get; }
            public Vector2 ImageCenter => (ImageStart + ImageEnd) * 0.5f;
            public float LengthPixels => Vector2.Distance(ImageStart, ImageEnd);
        }

        private sealed class PlanInkMap : IDisposable
        {
            private readonly int width;
            private readonly int height;
            private readonly Color32[] pixels;

            public PlanInkMap(string assetPath)
            {
                var source = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                if (source == null)
                {
                    throw new InvalidOperationException($"Required source plan texture is missing: {assetPath}");
                }

                width = source.width;
                height = source.height;
                if (width != ApprovedFacilityLayout.SourceWidthPixels ||
                    height != ApprovedFacilityLayout.SourceHeightPixels)
                {
                    throw new InvalidOperationException(
                        $"Source plan must remain {ApprovedFacilityLayout.SourceWidthPixels} x " +
                        $"{ApprovedFacilityLayout.SourceHeightPixels}; found {width} x {height}.");
                }

                var previous = RenderTexture.active;
                var renderTexture = RenderTexture.GetTemporary(
                    width,
                    height,
                    0,
                    RenderTextureFormat.ARGB32,
                    RenderTextureReadWrite.Linear);
                try
                {
                    Graphics.Blit(source, renderTexture);
                    RenderTexture.active = renderTexture;
                    var readable = new Texture2D(width, height, TextureFormat.RGBA32, false, true);
                    try
                    {
                        readable.ReadPixels(new Rect(0f, 0f, width, height), 0, 0, false);
                        readable.Apply(false, false);
                        pixels = readable.GetPixels32();
                    }
                    finally
                    {
                        UnityEngine.Object.DestroyImmediate(readable);
                    }
                }
                finally
                {
                    RenderTexture.active = previous;
                    RenderTexture.ReleaseTemporary(renderTexture);
                }
            }

            public bool IsInkNear(Vector2 imagePoint, int radius)
            {
                var centerX = Mathf.RoundToInt(imagePoint.x);
                var centerY = Mathf.RoundToInt(imagePoint.y);
                var radiusSquared = radius * radius;
                for (var y = centerY - radius; y <= centerY + radius; y++)
                {
                    for (var x = centerX - radius; x <= centerX + radius; x++)
                    {
                        var deltaX = x - centerX;
                        var deltaY = y - centerY;
                        if (deltaX * deltaX + deltaY * deltaY > radiusSquared || !IsInk(x, y)) continue;
                        return true;
                    }
                }

                return false;
            }

            public Vector2 SnapToInk(Vector2 imagePoint, int radius)
            {
                var centerX = Mathf.RoundToInt(imagePoint.x);
                var centerY = Mathf.RoundToInt(imagePoint.y);
                var best = imagePoint;
                var bestSquaredDistance = float.PositiveInfinity;
                for (var y = centerY - radius; y <= centerY + radius; y++)
                {
                    for (var x = centerX - radius; x <= centerX + radius; x++)
                    {
                        if (!IsInk(x, y)) continue;
                        var candidate = new Vector2(x, y);
                        var squaredDistance = (candidate - imagePoint).sqrMagnitude;
                        if (squaredDistance >= bestSquaredDistance) continue;
                        bestSquaredDistance = squaredDistance;
                        best = candidate;
                    }
                }

                return best;
            }

            public float InkFractionAlong(
                Vector2 start,
                Vector2 end,
                float endpointInsetPixels,
                int inkRadius = 2)
            {
                var delta = end - start;
                var length = delta.magnitude;
                if (length <= endpointInsetPixels * 2f) return 1f;
                var direction = delta / length;
                var insetStart = start + direction * endpointInsetPixels;
                var insetEnd = end - direction * endpointInsetPixels;
                var samples = Mathf.Max(2, Mathf.CeilToInt(Vector2.Distance(insetStart, insetEnd) / 4f));
                var inkSamples = 0;
                for (var sample = 0; sample <= samples; sample++)
                {
                    if (IsInkNear(
                            Vector2.Lerp(insetStart, insetEnd, sample / (float)samples),
                            inkRadius))
                    {
                        inkSamples++;
                    }
                }

                return inkSamples / (float)(samples + 1);
            }

            public void Dispose()
            {
                // Pixel data is managed memory; the temporary Unity objects are released in the constructor.
            }

            private bool IsInk(int imageX, int imageY)
            {
                if (imageX < 0 || imageX >= width || imageY < 0 || imageY >= height) return false;
                var color = pixels[(height - 1 - imageY) * width + imageX];
                return color.a > 16 && color.r < 112 && color.g < 112 && color.b < 112;
            }
        }

        private readonly struct PlanContext
        {
            private readonly Transform plan;

            public PlanContext(Transform floorPlan)
            {
                plan = floorPlan;
                Normal = floorPlan.TransformDirection(Vector3.back).normalized;
                MetersPerPixel = floorPlan.TransformVector(Vector3.right).magnitude /
                                 ApprovedFacilityLayout.SourceWidthPixels;
            }

            public Vector3 Normal { get; }
            public float MetersPerPixel { get; }

            public Vector3 ImageToPlan(Vector2 imagePoint)
            {
                var local = new Vector3(
                    imagePoint.x / ApprovedFacilityLayout.SourceWidthPixels - 0.5f,
                    0.5f - imagePoint.y / ApprovedFacilityLayout.SourceHeightPixels,
                    0f);
                return plan.TransformPoint(local);
            }

            public Vector2 PlanToImage(Vector3 worldPoint)
            {
                var local = plan.InverseTransformPoint(worldPoint);
                return new Vector2(
                    (local.x + 0.5f) * ApprovedFacilityLayout.SourceWidthPixels,
                    (0.5f - local.y) * ApprovedFacilityLayout.SourceHeightPixels);
            }
        }
    }
}
