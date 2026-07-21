using System;

namespace InternshipGame.Prototype
{
    public enum PursuitThreatState
    {
        Dormant,
        Patrolling,
        Investigating,
        Hunting,
        Searching,
        Caught,
        Complete,
    }

    public sealed class PursuitThreatStateMachine
    {
        private readonly float investigationSeconds;
        private readonly float loseSightGraceSeconds;
        private readonly float searchSeconds;
        private readonly float captureDistance;

        public PursuitThreatStateMachine(
            float investigationSeconds,
            float loseSightGraceSeconds,
            float searchSeconds,
            float captureDistance)
        {
            if (investigationSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(investigationSeconds));
            }

            if (loseSightGraceSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(loseSightGraceSeconds));
            }

            if (searchSeconds <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(searchSeconds));
            }

            if (captureDistance <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(captureDistance));
            }

            this.investigationSeconds = investigationSeconds;
            this.loseSightGraceSeconds = loseSightGraceSeconds;
            this.searchSeconds = searchSeconds;
            this.captureDistance = captureDistance;
        }

        public PursuitThreatState State { get; private set; } = PursuitThreatState.Dormant;
        public float StateElapsed { get; private set; }
        public float LostSightElapsed { get; private set; }

        public bool Activate()
        {
            if (State != PursuitThreatState.Dormant)
            {
                return false;
            }

            Enter(PursuitThreatState.Investigating);
            return true;
        }

        public void Tick(float deltaSeconds, bool hasLineOfSight, float playerDistance)
        {
            if (deltaSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaSeconds));
            }

            if (playerDistance < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(playerDistance));
            }

            if (State is PursuitThreatState.Dormant or PursuitThreatState.Caught or PursuitThreatState.Complete)
            {
                return;
            }

            StateElapsed += deltaSeconds;
            switch (State)
            {
                case PursuitThreatState.Patrolling:
                    if (hasLineOfSight)
                    {
                        Enter(PursuitThreatState.Investigating);
                    }
                    break;

                case PursuitThreatState.Investigating:
                    if (StateElapsed >= investigationSeconds)
                    {
                        Enter(hasLineOfSight ? PursuitThreatState.Hunting : PursuitThreatState.Searching);
                    }
                    break;

                case PursuitThreatState.Hunting:
                    if (hasLineOfSight && playerDistance <= captureDistance)
                    {
                        Enter(PursuitThreatState.Caught);
                        return;
                    }

                    LostSightElapsed = hasLineOfSight ? 0f : LostSightElapsed + deltaSeconds;
                    if (LostSightElapsed >= loseSightGraceSeconds)
                    {
                        Enter(PursuitThreatState.Searching);
                    }
                    break;

                case PursuitThreatState.Searching:
                    if (hasLineOfSight && playerDistance <= captureDistance)
                    {
                        Enter(PursuitThreatState.Caught);
                    }
                    else if (hasLineOfSight)
                    {
                        Enter(PursuitThreatState.Hunting);
                    }
                    else if (StateElapsed >= searchSeconds)
                    {
                        Enter(PursuitThreatState.Patrolling);
                    }
                    break;
            }
        }

        public bool Investigate()
        {
            if (State != PursuitThreatState.Patrolling)
            {
                return false;
            }

            Enter(PursuitThreatState.Investigating);
            return true;
        }

        public bool Deactivate()
        {
            if (State is PursuitThreatState.Dormant or PursuitThreatState.Complete)
            {
                return false;
            }

            Enter(PursuitThreatState.Complete);
            return true;
        }

        public void Reset()
        {
            State = PursuitThreatState.Dormant;
            StateElapsed = 0f;
            LostSightElapsed = 0f;
        }

        private void Enter(PursuitThreatState state)
        {
            State = state;
            StateElapsed = 0f;
            LostSightElapsed = 0f;
        }
    }
}
