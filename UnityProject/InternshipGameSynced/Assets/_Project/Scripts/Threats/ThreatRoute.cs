using System;
using UnityEngine;

namespace InternshipGame.Prototype
{
    public sealed class ThreatRoute : MonoBehaviour
    {
        [SerializeField] private Transform[] waypoints = Array.Empty<Transform>();

        public int Count => waypoints?.Length ?? 0;

        public void Configure(params Transform[] points)
        {
            waypoints = points ?? Array.Empty<Transform>();
        }

        public Vector3 GetNextPointToward(Vector3 currentPosition, Vector3 targetPosition)
        {
            if (waypoints == null || waypoints.Length == 0)
            {
                return targetPosition;
            }

            var currentIndex = FindNearestIndex(currentPosition);
            var targetIndex = FindNearestIndex(targetPosition);
            if (currentIndex == targetIndex)
            {
                return targetPosition;
            }

            var nextIndex = currentIndex + Math.Sign(targetIndex - currentIndex);
            return waypoints[nextIndex] != null ? waypoints[nextIndex].position : targetPosition;
        }

        public int FindNearestIndex(Vector3 position)
        {
            var bestIndex = 0;
            var bestDistance = float.PositiveInfinity;
            for (var i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == null)
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

        private void OnDrawGizmosSelected()
        {
            if (waypoints == null)
            {
                return;
            }

            Gizmos.color = new Color(1f, 0.25f, 0.05f, 0.8f);
            for (var i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == null)
                {
                    continue;
                }

                Gizmos.DrawWireSphere(waypoints[i].position, 0.16f);
                if (i > 0 && waypoints[i - 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i - 1].position, waypoints[i].position);
                }
            }
        }
    }
}
