using UnityEngine;

namespace InternshipGame.Prototype
{
    internal static class ThreatPerceptionMath
    {
        public static bool IsWithinViewCone(
            Vector3 eyePosition,
            Vector3 forward,
            Vector3 targetPosition,
            float viewDistance,
            float fieldOfView,
            float nearAwarenessDistance,
            float nearFieldOfView)
        {
            var toTarget = targetPosition - eyePosition;
            var distance = toTarget.magnitude;
            if (distance <= 0.001f || distance > viewDistance)
            {
                return false;
            }

            var flatDirection = Vector3.ProjectOnPlane(toTarget / distance, Vector3.up);
            var flatForward = Vector3.ProjectOnPlane(forward, Vector3.up);
            if (flatDirection.sqrMagnitude <= 0.001f || flatForward.sqrMagnitude <= 0.001f)
            {
                return true;
            }

            var activeFieldOfView = distance <= nearAwarenessDistance
                ? Mathf.Max(fieldOfView, nearFieldOfView)
                : fieldOfView;
            return Vector3.Angle(flatForward, flatDirection) <= activeFieldOfView * 0.5f;
        }

        public static Vector3 PredictObservedPosition(
            Vector3 observedPosition,
            Vector3 observedVelocity,
            float predictionSeconds,
            float maximumLeadDistance)
        {
            var planarVelocity = Vector3.ProjectOnPlane(observedVelocity, Vector3.up);
            var lead = Vector3.ClampMagnitude(
                planarVelocity * Mathf.Max(0f, predictionSeconds),
                Mathf.Max(0f, maximumLeadDistance));
            return observedPosition + lead;
        }

        public static Vector3 SearchOffset(int index, float radius)
        {
            var safeRadius = Mathf.Max(0f, radius);
            return (index & 3) switch
            {
                0 => Vector3.right * safeRadius,
                1 => Vector3.forward * safeRadius,
                2 => Vector3.left * safeRadius,
                _ => Vector3.back * safeRadius,
            };
        }
    }
}
