using System;

namespace InternshipGame.Prototype
{
    public enum WatcherEncounterState
    {
        Dormant,
        Waiting,
        Visible,
        Complete,
    }

    public sealed class WatcherEncounterSequence
    {
        private readonly float revealDelaySeconds;
        private readonly float visibleDurationSeconds;
        private readonly float dismissDistance;

        public WatcherEncounterSequence(
            float revealDelaySeconds,
            float visibleDurationSeconds,
            float dismissDistance)
        {
            if (revealDelaySeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(revealDelaySeconds));
            }

            if (visibleDurationSeconds <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(visibleDurationSeconds));
            }

            if (dismissDistance < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(dismissDistance));
            }

            this.revealDelaySeconds = revealDelaySeconds;
            this.visibleDurationSeconds = visibleDurationSeconds;
            this.dismissDistance = dismissDistance;
        }

        public WatcherEncounterState State { get; private set; } = WatcherEncounterState.Dormant;

        public float Elapsed { get; private set; }

        public bool Arm()
        {
            if (State != WatcherEncounterState.Dormant)
            {
                return false;
            }

            State = WatcherEncounterState.Waiting;
            Elapsed = 0f;
            return true;
        }

        public void Tick(float deltaSeconds, float viewerDistance)
        {
            if (deltaSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaSeconds));
            }

            if (State is WatcherEncounterState.Dormant or WatcherEncounterState.Complete)
            {
                return;
            }

            Elapsed += deltaSeconds;
            if (State == WatcherEncounterState.Waiting && Elapsed >= revealDelaySeconds)
            {
                State = WatcherEncounterState.Visible;
                Elapsed = 0f;
                return;
            }

            if (State == WatcherEncounterState.Visible &&
                (Elapsed >= visibleDurationSeconds || viewerDistance <= dismissDistance))
            {
                State = WatcherEncounterState.Complete;
            }
        }

        public bool Dismiss()
        {
            if (State is WatcherEncounterState.Dormant or WatcherEncounterState.Complete)
            {
                return false;
            }

            State = WatcherEncounterState.Complete;
            return true;
        }

        public void Reset()
        {
            State = WatcherEncounterState.Dormant;
            Elapsed = 0f;
        }
    }
}
