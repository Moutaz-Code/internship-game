using System;
using UnityEngine;

namespace InternshipGame.Prototype
{
    internal static class PlayerPostureMath
    {
        public static float StepCrouchAmount(
            float current,
            bool crouchRequested,
            bool canStand,
            float transitionSpeed,
            float deltaTime)
        {
            if (transitionSpeed < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(transitionSpeed));
            }

            if (deltaTime < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaTime));
            }

            var target = crouchRequested || !canStand ? 1f : 0f;
            return Mathf.MoveTowards(Mathf.Clamp01(current), target, transitionSpeed * deltaTime);
        }

        public static Vector3 CapsuleCenter(float height)
        {
            return Vector3.up * Mathf.Max(0f, height) * 0.5f;
        }

        public static float AllowedLeanDistance(float requestedDistance, float blockingDistance, float clearance)
        {
            if (requestedDistance <= 0f)
            {
                return 0f;
            }

            if (float.IsPositiveInfinity(blockingDistance))
            {
                return requestedDistance;
            }

            return Mathf.Clamp(blockingDistance - Mathf.Max(0f, clearance), 0f, requestedDistance);
        }
    }
}
