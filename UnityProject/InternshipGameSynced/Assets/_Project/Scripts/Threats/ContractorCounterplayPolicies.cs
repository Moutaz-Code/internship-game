using System;

namespace InternshipGame.Prototype
{
    public sealed class ThrowableImpactGate
    {
        public bool IsArmed { get; private set; }
        public float NextAllowedTime { get; private set; }
        public int AcceptedImpactCount { get; private set; }

        public void OnGrabbed()
        {
            IsArmed = false;
        }

        public void OnReleased()
        {
            IsArmed = true;
        }

        public bool TryAccept(float now, float impactSpeed, float minimumImpactSpeed, float cooldownSeconds)
        {
            ValidateTime(now, nameof(now));
            ValidateTime(impactSpeed, nameof(impactSpeed));
            if (minimumImpactSpeed <= 0f || cooldownSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (!IsArmed || now < NextAllowedTime || impactSpeed < minimumImpactSpeed)
            {
                return false;
            }

            IsArmed = false;
            NextAllowedTime = now + cooldownSeconds;
            AcceptedImpactCount++;
            return true;
        }

        public void Reset()
        {
            IsArmed = false;
            NextAllowedTime = 0f;
            AcceptedImpactCount = 0;
        }

        private static void ValidateTime(float value, string name)
        {
            if (value < 0f || float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentOutOfRangeException(name);
            }
        }
    }

    public sealed class MachineryPanelState
    {
        public MachineryPanelState(float activeSeconds, float cooldownSeconds)
        {
            if (activeSeconds <= 0f || cooldownSeconds < activeSeconds ||
                float.IsNaN(activeSeconds) || float.IsInfinity(activeSeconds) ||
                float.IsNaN(cooldownSeconds) || float.IsInfinity(cooldownSeconds))
            {
                throw new ArgumentOutOfRangeException();
            }

            ActiveSeconds = activeSeconds;
            CooldownSeconds = cooldownSeconds;
        }

        public float ActiveSeconds { get; }
        public float CooldownSeconds { get; }
        public float ActiveUntil { get; private set; }
        public float CooldownUntil { get; private set; }
        public int ActivationCount { get; private set; }

        public bool IsActive(float now) => now < ActiveUntil;
        public bool IsReady(float now) => now >= CooldownUntil;
        public float CooldownRemaining(float now) => Math.Max(0f, CooldownUntil - now);

        public bool TryActivate(float now)
        {
            if (now < 0f || float.IsNaN(now) || float.IsInfinity(now))
            {
                throw new ArgumentOutOfRangeException(nameof(now));
            }

            if (!IsReady(now))
            {
                return false;
            }

            ActiveUntil = now + ActiveSeconds;
            CooldownUntil = now + CooldownSeconds;
            ActivationCount++;
            return true;
        }

        public void Reset()
        {
            ActiveUntil = 0f;
            CooldownUntil = 0f;
            ActivationCount = 0;
        }
    }
}
