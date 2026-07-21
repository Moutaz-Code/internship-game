using UnityEngine;

namespace InternshipGame.Prototype
{
    public static class ThreatTargetPolicy
    {
        public static Vector3 SelectMovementTarget(
            PursuitThreatState state,
            Vector3 currentPlayerPosition,
            Vector3 lastKnownPlayerPosition,
            Vector3 patrolPosition)
        {
            return state switch
            {
                PursuitThreatState.Investigating or PursuitThreatState.Searching => lastKnownPlayerPosition,
                PursuitThreatState.Patrolling => patrolPosition,
                _ => currentPlayerPosition,
            };
        }
    }
}
