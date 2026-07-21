using System;

namespace InternshipGame.Prototype
{
    public enum PrototypeRunState
    {
        Ready,
        RetrieveFuelRods,
        ExtractionCycling,
        EscapeReady,
        Extracted,
        Failed,

        // Transitional states retained until the original one-rod scene is rebuilt.
        [Obsolete("Use RetrieveFuelRods.")]
        FindFuelRod,
        [Obsolete("Use RetrieveFuelRods or ExtractionCycling.")]
        ReturnToExtraction,
    }

    public sealed class PrototypeRunStateMachine
    {
        public const float DefaultExtractionCycleSeconds = 20f;

        private float extractionCycleDurationSeconds;

        public PrototypeRunStateMachine(float extractionCycleDurationSeconds = DefaultExtractionCycleSeconds)
        {
            SetExtractionCycleDuration(extractionCycleDurationSeconds);
        }

        public PrototypeRunState State { get; private set; } = PrototypeRunState.Ready;
        public float ExtractionCycleElapsed { get; private set; }
        public float ExtractionCycleDuration => extractionCycleDurationSeconds;
        public float ExtractionCycleRemaining => Math.Max(0f, extractionCycleDurationSeconds - ExtractionCycleElapsed);

        /// <summary>Starts the transitional one-rod loop used by the existing scene.</summary>
        public void StartRun()
        {
#pragma warning disable CS0618
            State = PrototypeRunState.FindFuelRod;
#pragma warning restore CS0618
            ExtractionCycleElapsed = 0f;
        }

        public void StartMultiRodRun()
        {
            State = PrototypeRunState.RetrieveFuelRods;
            ExtractionCycleElapsed = 0f;
        }

        public void SetExtractionCycleDuration(float seconds)
        {
            if (seconds <= 0f || float.IsNaN(seconds) || float.IsInfinity(seconds))
            {
                throw new ArgumentOutOfRangeException(nameof(seconds));
            }

            extractionCycleDurationSeconds = seconds;
            ExtractionCycleElapsed = Math.Min(ExtractionCycleElapsed, seconds);
        }

        public bool BeginExtractionCycle()
        {
            if (State != PrototypeRunState.RetrieveFuelRods)
            {
                return false;
            }

            ExtractionCycleElapsed = 0f;
            State = PrototypeRunState.ExtractionCycling;
            return true;
        }

        public bool AdvanceExtractionCycle(float deltaTime)
        {
            if (State != PrototypeRunState.ExtractionCycling)
            {
                return false;
            }

            ExtractionCycleElapsed = Math.Min(
                extractionCycleDurationSeconds,
                ExtractionCycleElapsed + Math.Max(0f, deltaTime));
            if (ExtractionCycleElapsed < extractionCycleDurationSeconds)
            {
                return false;
            }

            State = PrototypeRunState.EscapeReady;
            return true;
        }

        public bool Escape()
        {
            if (State != PrototypeRunState.EscapeReady)
            {
                return false;
            }

            State = PrototypeRunState.Extracted;
            return true;
        }

        // Backward-compatible single-objective operations.
        public bool CollectFuelRod()
        {
#pragma warning disable CS0618
            if (State != PrototypeRunState.FindFuelRod)
            {
                return false;
            }

            State = PrototypeRunState.ReturnToExtraction;
#pragma warning restore CS0618
            return true;
        }

        public bool Extract(bool hasFuelRod)
        {
#pragma warning disable CS0618
            if (!hasFuelRod || State != PrototypeRunState.ReturnToExtraction)
            {
                return false;
            }
#pragma warning restore CS0618

            State = PrototypeRunState.Extracted;
            return true;
        }

        public bool Fail()
        {
            if (State is PrototypeRunState.Extracted or PrototypeRunState.Failed)
            {
                return false;
            }

            State = PrototypeRunState.Failed;
            return true;
        }
    }
}
