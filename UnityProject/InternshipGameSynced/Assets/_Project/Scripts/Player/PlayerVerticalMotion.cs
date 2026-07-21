using UnityEngine;

namespace InternshipGame.Prototype
{
    /// <summary>
    /// Deterministic vertical-motion math kept separate from input and collision polling.
    /// Position uses analytic constant-acceleration integration so jump height does not
    /// drift with frame rate.
    /// </summary>
    internal static class PlayerVerticalMotion
    {
        public static float CalculateJumpSpeed(float gravity, float jumpHeight)
        {
            return Mathf.Sqrt(2f * Mathf.Abs(gravity) * Mathf.Max(0f, jumpHeight));
        }

        public static float IntegrateDisplacement(float velocity, float gravity, float deltaTime)
        {
            var safeDeltaTime = Mathf.Max(0f, deltaTime);
            return velocity * safeDeltaTime + 0.5f * gravity * safeDeltaTime * safeDeltaTime;
        }

        public static float IntegrateVelocity(float velocity, float gravity, float deltaTime)
        {
            return velocity + gravity * Mathf.Max(0f, deltaTime);
        }

        public static float ResolveVelocityAfterCollision(
            float velocity,
            CollisionFlags collisionFlags,
            bool isGrounded,
            float groundedVelocity)
        {
            if ((collisionFlags & CollisionFlags.Above) != 0 && velocity > 0f)
            {
                velocity = 0f;
            }

            if (isGrounded && velocity < 0f)
            {
                velocity = groundedVelocity;
            }

            return velocity;
        }

        public static float CalculateLandingImpactSpeed(bool landedThisFrame, float verticalVelocity)
        {
            return landedThisFrame ? Mathf.Max(0f, -verticalVelocity) : 0f;
        }
    }
}
