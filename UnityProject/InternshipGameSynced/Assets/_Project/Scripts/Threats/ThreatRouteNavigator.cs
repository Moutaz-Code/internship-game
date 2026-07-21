using UnityEngine;

namespace InternshipGame.Prototype
{
    [DisallowMultipleComponent]
    public sealed class ThreatRouteNavigator : MonoBehaviour, IThreatNavigator
    {
        [SerializeField] private ThreatRoute route;
        [SerializeField, Min(0.01f)] private float stoppingDistance = 0.08f;
        [SerializeField, Min(1f)] private float turnSpeedDegrees = 360f;

        private Vector3 destination;
        private float movementSpeed;

        public Vector3 CurrentPosition => transform.position;
        public bool HasPath { get; private set; }
        public float RemainingDistance => HasPath
            ? Vector3.Distance(transform.position, destination)
            : 0f;

        public void Configure(ThreatRoute threatRoute)
        {
            route = threatRoute;
        }

        public void SetDestination(Vector3 newDestination, float speed)
        {
            destination = newDestination;
            movementSpeed = Mathf.Max(0f, speed);
            HasPath = movementSpeed > 0f &&
                      Vector3.Distance(transform.position, destination) > stoppingDistance;
        }

        public void Stop()
        {
            HasPath = false;
            movementSpeed = 0f;
        }

        public void Warp(Vector3 position, Quaternion rotation)
        {
            transform.SetPositionAndRotation(position, rotation);
            destination = position;
            Stop();
        }

        public void TickNavigation(float deltaSeconds)
        {
            if (!HasPath || deltaSeconds <= 0f)
            {
                return;
            }

            var targetPoint = route != null
                ? route.GetNextPointToward(transform.position, destination)
                : destination;
            var previousPosition = transform.position;
            transform.position = Vector3.MoveTowards(
                previousPosition,
                targetPoint,
                movementSpeed * deltaSeconds);

            var movement = transform.position - previousPosition;
            movement.y = 0f;
            if (movement.sqrMagnitude > 0.0001f)
            {
                var targetRotation = Quaternion.LookRotation(movement.normalized, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    turnSpeedDegrees * deltaSeconds);
            }

            if (Vector3.Distance(transform.position, destination) <= stoppingDistance)
            {
                transform.position = destination;
                Stop();
            }
        }

        private void Update()
        {
            TickNavigation(Time.deltaTime);
        }
    }
}
