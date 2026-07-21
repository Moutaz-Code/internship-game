using System;
using UnityEngine;

namespace InternshipGame.Prototype
{
    public enum FuelRodClassification
    {
        Live,
        Inert,
    }

    public enum FuelRodAssayState
    {
        Unassayed,
        Assaying,
        ConfirmedLive,
        ConfirmedInert,
        Deposited,
    }

    /// <summary>
    /// Pure timing model for the continuous hold-to-identify interaction.
    /// Dropping a rod before completion deliberately discards partial progress.
    /// </summary>
    public sealed class FuelRodAssayStateModel
    {
        public const float DefaultDurationSeconds = 3f;

        private float durationSeconds;

        public FuelRodAssayStateModel(float durationSeconds = DefaultDurationSeconds)
        {
            SetDuration(durationSeconds);
            Reset();
        }

        public FuelRodAssayState State { get; private set; }
        public float ElapsedSeconds { get; private set; }
        public float DurationSeconds => durationSeconds;
        public float NormalizedProgress => Mathf.Clamp01(ElapsedSeconds / durationSeconds);
        public bool WasIdentified { get; private set; }
        public bool IsComplete => State is FuelRodAssayState.ConfirmedLive or FuelRodAssayState.ConfirmedInert or FuelRodAssayState.Deposited;

        public void SetDuration(float seconds)
        {
            if (seconds <= 0f || float.IsNaN(seconds) || float.IsInfinity(seconds))
            {
                throw new ArgumentOutOfRangeException(nameof(seconds), "Assay duration must be finite and greater than zero.");
            }

            durationSeconds = seconds;
            ElapsedSeconds = Mathf.Min(ElapsedSeconds, durationSeconds);
        }

        public void Begin()
        {
            if (State == FuelRodAssayState.Unassayed)
            {
                State = FuelRodAssayState.Assaying;
            }
        }

        public bool Tick(float deltaTime, bool isHeld, FuelRodClassification classification)
        {
            if (IsComplete)
            {
                return false;
            }

            if (!isHeld)
            {
                CancelUnfinished();
                return false;
            }

            Begin();
            ElapsedSeconds = Mathf.Min(durationSeconds, ElapsedSeconds + Mathf.Max(0f, deltaTime));
            if (ElapsedSeconds < durationSeconds)
            {
                return false;
            }

            State = classification == FuelRodClassification.Live
                ? FuelRodAssayState.ConfirmedLive
                : FuelRodAssayState.ConfirmedInert;
            WasIdentified = true;
            return true;
        }

        public void CancelUnfinished()
        {
            if (State != FuelRodAssayState.Assaying)
            {
                return;
            }

            State = FuelRodAssayState.Unassayed;
            ElapsedSeconds = 0f;
        }

        public bool MarkDeposited()
        {
            if (State != FuelRodAssayState.ConfirmedLive)
            {
                return false;
            }

            State = FuelRodAssayState.Deposited;
            return true;
        }

        public void Reset()
        {
            State = FuelRodAssayState.Unassayed;
            ElapsedSeconds = 0f;
            WasIdentified = false;
        }
    }
}
