using UnityEngine;

namespace InternshipGame.Prototype
{
    /// <summary>
    /// Deterministic horizontal acceleration used by desktop locomotion. Keeping the
    /// feel math outside the controller preserves the controller-swap seam and makes
    /// tuning regressions inexpensive to test.
    /// </summary>
    internal static class PlayerPlanarMotion
    {
        public static Vector3 Step(
            Vector3 currentVelocity,
            Vector3 desiredVelocity,
            bool isGrounded,
            float groundAcceleration,
            float groundDeceleration,
            float airAcceleration,
            float airDeceleration,
            float deltaTime)
        {
            currentVelocity.y = 0f;
            desiredVelocity.y = 0f;

            var safeDeltaTime = Mathf.Max(0f, deltaTime);
            if (safeDeltaTime <= 0f)
            {
                return currentVelocity;
            }

            var hasDesiredMotion = desiredVelocity.sqrMagnitude > 0.0001f;
            float acceleration;
            if (isGrounded)
            {
                var reversing = hasDesiredMotion &&
                    currentVelocity.sqrMagnitude > 0.0001f &&
                    Vector3.Dot(currentVelocity, desiredVelocity) < 0f;
                acceleration = !hasDesiredMotion || reversing
                    ? groundDeceleration
                    : groundAcceleration;
            }
            else
            {
                acceleration = hasDesiredMotion ? airAcceleration : airDeceleration;
            }

            return Vector3.MoveTowards(
                currentVelocity,
                desiredVelocity,
                Mathf.Max(0f, acceleration) * safeDeltaTime);
        }
    }
}
