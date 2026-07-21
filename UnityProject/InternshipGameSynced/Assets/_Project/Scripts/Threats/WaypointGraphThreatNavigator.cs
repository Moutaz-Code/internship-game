using System;
using System.Collections.Generic;
using UnityEngine;

namespace InternshipGame.Prototype
{
    [Serializable]
    public struct ThreatWaypointConnection
    {
        [SerializeField] private int fromIndex;
        [SerializeField] private int toIndex;
        [SerializeField] private string blockedByProfileId;

        public ThreatWaypointConnection(int fromIndex, int toIndex, string blockedByProfileId = "")
        {
            this.fromIndex = fromIndex;
            this.toIndex = toIndex;
            this.blockedByProfileId = blockedByProfileId ?? string.Empty;
        }

        public int FromIndex => fromIndex;
        public int ToIndex => toIndex;
        public string BlockedByProfileId => blockedByProfileId;
    }

    /// <summary>
    /// Offline-safe navigation adapter. It follows authored connections and replans when
    /// the destination or active carved-blocker profile changes, so threat decisions do
    /// not depend on a particular navigation package.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WaypointGraphThreatNavigator : MonoBehaviour, IThreatNavigator, IThreatRouteProfileReceiver
    {
        [SerializeField] private Transform[] waypoints = Array.Empty<Transform>();
        [SerializeField] private ThreatWaypointConnection[] connections = Array.Empty<ThreatWaypointConnection>();
        [SerializeField, Min(0.01f)] private float stoppingDistance = 0.1f;
        [SerializeField, Min(1f)] private float turnSpeedDegrees = 360f;
        [SerializeField, Min(0.05f)] private float endpointColliderAllowance = 0.55f;

        private readonly List<Vector3> activePath = new();
        private int pathIndex;
        private Vector3 destination;
        private Vector3 plannedDestination;
        private float movementSpeed;
        private string activeRouteProfileId = string.Empty;

        public Vector3 CurrentPosition => transform.position;
        public bool HasPath { get; private set; }
        public float RemainingDistance => CalculateRemainingDistance();
        public string ActiveRouteProfileId => activeRouteProfileId;

        public void Configure(Transform[] graphWaypoints, ThreatWaypointConnection[] graphConnections)
        {
            waypoints = graphWaypoints ?? Array.Empty<Transform>();
            connections = graphConnections ?? Array.Empty<ThreatWaypointConnection>();
            Stop();
        }

        public void SetRouteProfile(string profileId)
        {
            var normalized = profileId ?? string.Empty;
            if (string.Equals(activeRouteProfileId, normalized, StringComparison.Ordinal))
            {
                return;
            }

            activeRouteProfileId = normalized;
            if (HasPath)
            {
                BuildPath(transform.position, destination);
            }
        }

        public void SetDestination(Vector3 newDestination, float speed)
        {
            destination = newDestination;
            movementSpeed = Mathf.Max(0f, speed);
            if (movementSpeed <= 0f)
            {
                Stop();
                return;
            }

            if (!HasPath || Vector3.Distance(plannedDestination, destination) > 0.65f)
            {
                BuildPath(transform.position, destination);
            }
        }

        public void Stop()
        {
            HasPath = false;
            movementSpeed = 0f;
            activePath.Clear();
            pathIndex = 0;
        }

        public void Warp(Vector3 position, Quaternion rotation)
        {
            transform.SetPositionAndRotation(position, rotation);
            destination = position;
            plannedDestination = position;
            Stop();
        }

        public void TickNavigation(float deltaSeconds)
        {
            if (!HasPath || deltaSeconds <= 0f || pathIndex >= activePath.Count)
            {
                return;
            }

            var target = activePath[pathIndex];
            target.y = transform.position.y;
            var previousPosition = transform.position;
            transform.position = Vector3.MoveTowards(previousPosition, target, movementSpeed * deltaSeconds);
            var movement = transform.position - previousPosition;
            movement.y = 0f;
            if (movement.sqrMagnitude > 0.0001f)
            {
                var targetRotation = Quaternion.LookRotation(movement.normalized, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeedDegrees * deltaSeconds);
            }

            if (Vector3.Distance(transform.position, target) > stoppingDistance)
            {
                return;
            }

            pathIndex++;
            if (pathIndex >= activePath.Count)
            {
                transform.position = target;
                HasPath = false;
            }
        }

        private void Update()
        {
            TickNavigation(Time.deltaTime);
        }

        private void BuildPath(Vector3 startPosition, Vector3 targetPosition)
        {
            activePath.Clear();
            pathIndex = 0;
            plannedDestination = targetPosition;

            if (waypoints.Length == 0)
            {
                HasPath = IsSegmentClear(startPosition, targetPosition);
                if (HasPath)
                {
                    activePath.Add(targetPosition);
                }

                return;
            }

            var startIndex = FindNearestReachableWaypoint(startPosition);
            var targetIndex = FindNearestReachableWaypoint(targetPosition);
            if (startIndex < 0 || targetIndex < 0)
            {
                HasPath = false;
                return;
            }

            var parents = FindShortestPathParents(startIndex, targetIndex);
            if (startIndex != targetIndex && parents[targetIndex] < 0)
            {
                HasPath = false;
                return;
            }

            var reverseIndices = new List<int>();
            var cursor = targetIndex;
            while (cursor >= 0 && cursor != startIndex)
            {
                reverseIndices.Add(cursor);
                cursor = parents[cursor];
            }

            if (Vector3.Distance(startPosition, waypoints[startIndex].position) > stoppingDistance)
            {
                activePath.Add(waypoints[startIndex].position);
            }

            reverseIndices.Reverse();
            foreach (var waypointIndex in reverseIndices)
            {
                if (waypoints[waypointIndex] != null)
                {
                    activePath.Add(waypoints[waypointIndex].position);
                }
            }

            var finalStart = activePath.Count > 0 ? activePath[^1] : startPosition;
            if (!IsSegmentClear(finalStart, targetPosition))
            {
                activePath.Clear();
                HasPath = false;
                return;
            }

            activePath.Add(targetPosition);
            HasPath = activePath.Count > 0;
        }

        private int[] FindShortestPathParents(int startIndex, int targetIndex)
        {
            var count = waypoints.Length;
            var parents = new int[count];
            var costs = new float[count];
            var visited = new bool[count];
            for (var i = 0; i < count; i++)
            {
                parents[i] = -1;
                costs[i] = float.PositiveInfinity;
            }

            costs[startIndex] = 0f;
            for (var iteration = 0; iteration < count; iteration++)
            {
                var current = -1;
                var currentCost = float.PositiveInfinity;
                for (var i = 0; i < count; i++)
                {
                    if (!visited[i] && costs[i] < currentCost)
                    {
                        current = i;
                        currentCost = costs[i];
                    }
                }

                if (current < 0 || current == targetIndex)
                {
                    break;
                }

                visited[current] = true;
                foreach (var connection in connections)
                {
                    if (IsConnectionBlocked(connection))
                    {
                        continue;
                    }

                    var neighbour = connection.FromIndex == current
                        ? connection.ToIndex
                        : connection.ToIndex == current
                            ? connection.FromIndex
                            : -1;
                    if (neighbour < 0 || neighbour >= count || visited[neighbour] ||
                        waypoints[current] == null || waypoints[neighbour] == null)
                    {
                        continue;
                    }

                    if (!IsSegmentClear(waypoints[current].position, waypoints[neighbour].position))
                    {
                        continue;
                    }

                    var candidateCost = currentCost + Vector3.Distance(
                        waypoints[current].position,
                        waypoints[neighbour].position);
                    if (candidateCost >= costs[neighbour])
                    {
                        continue;
                    }

                    costs[neighbour] = candidateCost;
                    parents[neighbour] = current;
                }
            }

            return parents;
        }

        private bool IsConnectionBlocked(ThreatWaypointConnection connection)
        {
            return !string.IsNullOrEmpty(connection.BlockedByProfileId) &&
                   string.Equals(connection.BlockedByProfileId, activeRouteProfileId, StringComparison.Ordinal);
        }

        private int FindNearestReachableWaypoint(Vector3 position)
        {
            var bestIndex = -1;
            var bestDistance = float.PositiveInfinity;
            for (var i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == null)
                {
                    continue;
                }

                if (!IsSegmentClear(position, waypoints[i].position))
                {
                    continue;
                }

                var distance = (waypoints[i].position - position).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        private bool IsSegmentClear(Vector3 start, Vector3 end)
        {
            var displacement = end - start;
            var distance = displacement.magnitude;
            if (distance <= stoppingDistance)
            {
                return true;
            }

            foreach (var hit in Physics.RaycastAll(
                         start,
                         displacement / distance,
                         distance,
                         ~0,
                         QueryTriggerInteraction.Ignore))
            {
                if (hit.collider == null || hit.collider.transform.IsChildOf(transform) ||
                    hit.distance <= stoppingDistance ||
                    distance - hit.distance <= endpointColliderAllowance)
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        private float CalculateRemainingDistance()
        {
            if (!HasPath || pathIndex >= activePath.Count)
            {
                return 0f;
            }

            var distance = Vector3.Distance(transform.position, activePath[pathIndex]);
            for (var i = pathIndex + 1; i < activePath.Count; i++)
            {
                distance += Vector3.Distance(activePath[i - 1], activePath[i]);
            }

            return distance;
        }
    }
}
