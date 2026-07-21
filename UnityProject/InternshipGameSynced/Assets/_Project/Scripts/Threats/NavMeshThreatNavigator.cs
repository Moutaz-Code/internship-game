using UnityEngine;
using UnityEngine.AI;

namespace InternshipGame.Prototype
{
    public enum ThreatNavigationMode
    {
        Idle,
        NavMesh,
        WaypointFallback,
    }

    /// <summary>
    /// Primary threat-navigation adapter. Decision code talks only to IThreatNavigator;
    /// this component uses a baked NavMesh when available and falls back to the authored
    /// waypoint graph if the package, bake, or current agent placement is unavailable.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class NavMeshThreatNavigator : MonoBehaviour, IThreatNavigator, IThreatRouteProfileReceiver
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private WaypointGraphThreatNavigator fallbackNavigator;
        [SerializeField, Min(0.1f)] private float sampleRadius = 2.5f;
        [SerializeField, Min(0.05f)] private float destinationRepathDistance = 0.35f;
        [SerializeField, Min(0.1f)] private float primaryRetrySeconds = 0.5f;

        private NavMeshPath candidatePath;
        private Vector3 requestedDestination;
        private Vector3 plannedNavMeshDestination;
        private float requestedSpeed;
        private bool destinationRequested;
        private float nextPrimaryRetryTime;

        public Vector3 CurrentPosition => transform.position;
        public ThreatNavigationMode ActiveMode { get; private set; }
        public bool UsingNavMesh => ActiveMode == ThreatNavigationMode.NavMesh && CanUseNavMesh;
        public bool HasPath => ActiveMode switch
        {
            ThreatNavigationMode.NavMesh => CanUseNavMesh && (agent.pathPending || agent.hasPath),
            ThreatNavigationMode.WaypointFallback => fallbackNavigator != null && fallbackNavigator.HasPath,
            _ => false,
        };
        public float RemainingDistance => ActiveMode switch
        {
            ThreatNavigationMode.NavMesh when CanUseNavMesh => agent.pathPending
                ? float.PositiveInfinity
                : agent.remainingDistance,
            ThreatNavigationMode.WaypointFallback when fallbackNavigator != null => fallbackNavigator.RemainingDistance,
            _ => 0f,
        };

        private bool CanUseNavMesh => agent != null && agent.enabled && agent.isOnNavMesh;

        public void Configure(NavMeshAgent configuredAgent, WaypointGraphThreatNavigator fallback)
        {
            agent = configuredAgent;
            fallbackNavigator = fallback;
            if (fallbackNavigator != null)
            {
                // The wrapper owns ticking so only one navigator can move this transform.
                fallbackNavigator.enabled = false;
            }
        }

        public void SetRouteProfile(string profileId)
        {
            fallbackNavigator?.SetRouteProfile(profileId);
            nextPrimaryRetryTime = 0f;
        }

        public void SetDestination(Vector3 destination, float speed)
        {
            var destinationChanged = !destinationRequested ||
                                     Vector3.Distance(requestedDestination, destination) >= destinationRepathDistance;
            requestedDestination = destination;
            requestedSpeed = Mathf.Max(0f, speed);
            destinationRequested = requestedSpeed > 0f;

            if (!destinationRequested)
            {
                Stop();
                return;
            }

            if (ActiveMode == ThreatNavigationMode.NavMesh &&
                !destinationChanged &&
                IsPrimaryPathHealthy())
            {
                agent.speed = requestedSpeed;
                return;
            }

            if (ActiveMode != ThreatNavigationMode.WaypointFallback ||
                Time.time >= nextPrimaryRetryTime)
            {
                if (TryActivateNavMesh(destination, requestedSpeed))
                {
                    return;
                }

                nextPrimaryRetryTime = Time.time + primaryRetrySeconds;
            }

            ActivateFallback(destination, requestedSpeed);
        }

        public void Stop()
        {
            destinationRequested = false;
            requestedSpeed = 0f;
            if (CanUseNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }

            fallbackNavigator?.Stop();
            ActiveMode = ThreatNavigationMode.Idle;
        }

        public void Warp(Vector3 position, Quaternion rotation)
        {
            transform.rotation = rotation;
            destinationRequested = false;
            ActiveMode = ThreatNavigationMode.Idle;

            if (agent != null && agent.enabled &&
                NavMesh.SamplePosition(position, out var hit, sampleRadius, NavMesh.AllAreas) &&
                agent.Warp(hit.position))
            {
                agent.updatePosition = true;
                agent.updateRotation = true;
                transform.rotation = rotation;
                fallbackNavigator?.Stop();
                return;
            }

            DisableAgentTransformUpdates();
            fallbackNavigator?.Warp(position, rotation);
        }

        private void Awake()
        {
            candidatePath = new NavMeshPath();
            if (agent == null)
            {
                agent = GetComponent<NavMeshAgent>();
            }

            if (fallbackNavigator != null)
            {
                fallbackNavigator.enabled = false;
            }
        }

        private void Update()
        {
            if (!destinationRequested)
            {
                return;
            }

            if (ActiveMode == ThreatNavigationMode.NavMesh)
            {
                if (IsPrimaryPathHealthy() || TryActivateNavMesh(requestedDestination, requestedSpeed))
                {
                    return;
                }

                nextPrimaryRetryTime = Time.time + primaryRetrySeconds;
                ActivateFallback(requestedDestination, requestedSpeed);
            }

            if (ActiveMode != ThreatNavigationMode.WaypointFallback)
            {
                if (Time.time >= nextPrimaryRetryTime)
                {
                    if (TryActivateNavMesh(requestedDestination, requestedSpeed))
                    {
                        return;
                    }

                    nextPrimaryRetryTime = Time.time + primaryRetrySeconds;
                }

                ActivateFallback(requestedDestination, requestedSpeed);
            }

            if (ActiveMode == ThreatNavigationMode.WaypointFallback)
            {
                if (Time.time >= nextPrimaryRetryTime)
                {
                    if (TryActivateNavMesh(requestedDestination, requestedSpeed))
                    {
                        return;
                    }

                    nextPrimaryRetryTime = Time.time + primaryRetrySeconds;
                }

                if (fallbackNavigator != null && !fallbackNavigator.HasPath)
                {
                    fallbackNavigator.SetDestination(requestedDestination, requestedSpeed);
                }

                fallbackNavigator?.TickNavigation(Time.deltaTime);
            }
        }

        public static bool IsCompletePathResult(bool calculationSucceeded, NavMeshPathStatus status)
        {
            return calculationSucceeded && status == NavMeshPathStatus.PathComplete;
        }

        private bool TryActivateNavMesh(Vector3 destination, float speed)
        {
            candidatePath ??= new NavMeshPath();
            if (!PrepareAgentAtCurrentPosition() ||
                !NavMesh.SamplePosition(destination, out var hit, sampleRadius, agent.areaMask))
            {
                return false;
            }

            candidatePath.ClearCorners();
            if (!IsCompletePathResult(agent.CalculatePath(hit.position, candidatePath), candidatePath.status))
            {
                return false;
            }

            agent.speed = speed;
            agent.isStopped = false;
            if (!agent.SetPath(candidatePath))
            {
                return false;
            }

            plannedNavMeshDestination = hit.position;
            fallbackNavigator?.Stop();
            ActiveMode = ThreatNavigationMode.NavMesh;
            return true;
        }

        private bool PrepareAgentAtCurrentPosition()
        {
            if (agent == null || !agent.enabled)
            {
                return false;
            }

            if (ActiveMode == ThreatNavigationMode.WaypointFallback || !agent.isOnNavMesh)
            {
                if (!NavMesh.SamplePosition(transform.position, out var startHit, sampleRadius, agent.areaMask) ||
                    !agent.Warp(startHit.position))
                {
                    return false;
                }
            }

            agent.updatePosition = true;
            agent.updateRotation = true;
            return agent.isOnNavMesh;
        }

        private bool IsPrimaryPathHealthy()
        {
            if (!CanUseNavMesh)
            {
                return false;
            }

            if (agent.pathPending)
            {
                return true;
            }

            if (agent.isPathStale || agent.pathStatus != NavMeshPathStatus.PathComplete)
            {
                return false;
            }

            if (agent.hasPath)
            {
                return true;
            }

            var remaining = plannedNavMeshDestination - transform.position;
            remaining.y = 0f;
            return remaining.magnitude <= Mathf.Max(agent.stoppingDistance, 0.15f);
        }

        private void ActivateFallback(Vector3 destination, float speed)
        {
            if (CanUseNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }

            DisableAgentTransformUpdates();
            if (fallbackNavigator == null)
            {
                ActiveMode = ThreatNavigationMode.Idle;
                return;
            }

            fallbackNavigator.SetDestination(destination, speed);
            ActiveMode = ThreatNavigationMode.WaypointFallback;
        }

        private void DisableAgentTransformUpdates()
        {
            if (agent == null)
            {
                return;
            }

            agent.updatePosition = false;
            agent.updateRotation = false;
        }
    }
}
