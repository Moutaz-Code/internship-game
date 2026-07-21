using UnityEngine;

namespace InternshipGame.Prototype
{
    /// <summary>
    /// Controller-independent locomotion telemetry for threats, feedback, and analytics.
    /// Implementations may be desktop, XR, or automated test drivers.
    /// </summary>
    public interface IPlayerMotionSource
    {
        bool IsGrounded { get; }
        Vector3 Velocity { get; }
        bool IsSprinting { get; }
        bool JumpedThisFrame { get; }
        bool LandedThisFrame { get; }
        float LandingImpactSpeed { get; }
    }
}
