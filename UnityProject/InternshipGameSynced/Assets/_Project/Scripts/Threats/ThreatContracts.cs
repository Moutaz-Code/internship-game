using System;
using UnityEngine;

namespace InternshipGame.Prototype
{
    public interface IThreatNavigator
    {
        Vector3 CurrentPosition { get; }
        bool HasPath { get; }
        float RemainingDistance { get; }

        void SetDestination(Vector3 destination, float speed);
        void Stop();
        void Warp(Vector3 position, Quaternion rotation);
    }

    /// <summary>
    /// Optional navigation capability for authored route-state changes. Threat decision
    /// code depends on this narrow seam instead of knowing concrete navigator adapters.
    /// </summary>
    public interface IThreatRouteProfileReceiver
    {
        void SetRouteProfile(string profileId);
    }

    public interface IThreatStimulusSink
    {
        void ReportStimulus(ThreatStimulus stimulus);
    }

    public enum ThreatStimulusKind
    {
        Movement,
        Sprint,
        JumpLanding,
        DroppedProp,
        RodRejected,
        ObjectiveProgress,
    }

    public enum ThreatWing
    {
        None,
        West,
        Central,
        East,
    }

    public readonly struct ThreatStimulus
    {
        public ThreatStimulus(
            ThreatStimulusKind kind,
            Vector3 position,
            float intensity,
            float radius,
            float occurredAtSeconds = 0f)
        {
            if (float.IsNaN(intensity) || float.IsInfinity(intensity) || intensity < 0f || intensity > 1f)
            {
                throw new ArgumentOutOfRangeException(nameof(intensity), "Stimulus intensity must be normalized.");
            }

            if (float.IsNaN(radius) || float.IsInfinity(radius) || radius < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(radius));
            }

            if (float.IsNaN(occurredAtSeconds) || float.IsInfinity(occurredAtSeconds) || occurredAtSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(occurredAtSeconds));
            }

            Kind = kind;
            Position = position;
            Intensity = intensity;
            Radius = radius;
            OccurredAtSeconds = occurredAtSeconds;
        }

        public ThreatStimulusKind Kind { get; }
        public Vector3 Position { get; }
        public float Intensity { get; }
        public float Radius { get; }
        public float OccurredAtSeconds { get; }

        public float EvaluateAt(Vector3 listenerPosition)
        {
            var distance = Vector3.Distance(Position, listenerPosition);
            if (Radius <= Mathf.Epsilon)
            {
                return distance <= Mathf.Epsilon ? Intensity : 0f;
            }

            return Intensity * (1f - Mathf.Clamp01(distance / Radius));
        }
    }
}
