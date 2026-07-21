using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;

namespace InternshipGame.Prototype.Tests
{
    public sealed class ThreatNavigationPolicyTests
    {
        [TestCase(true, NavMeshPathStatus.PathComplete, true)]
        [TestCase(false, NavMeshPathStatus.PathComplete, false)]
        [TestCase(true, NavMeshPathStatus.PathPartial, false)]
        [TestCase(true, NavMeshPathStatus.PathInvalid, false)]
        public void NavMeshPrimaryAcceptsOnlySuccessfullyCalculatedCompletePaths(
            bool calculationSucceeded,
            NavMeshPathStatus status,
            bool expected)
        {
            Assert.That(
                NavMeshThreatNavigator.IsCompletePathResult(calculationSucceeded, status),
                Is.EqualTo(expected));
        }

        [Test]
        public void InvestigatingAndSearchingUseLastKnownPosition()
        {
            var currentPlayer = new Vector3(9f, 0f, 9f);
            var lastKnown = new Vector3(2f, 0f, 3f);
            var patrol = new Vector3(-5f, 0f, 1f);

            Assert.That(
                ThreatTargetPolicy.SelectMovementTarget(
                    PursuitThreatState.Investigating,
                    currentPlayer,
                    lastKnown,
                    patrol),
                Is.EqualTo(lastKnown));
            Assert.That(
                ThreatTargetPolicy.SelectMovementTarget(
                    PursuitThreatState.Searching,
                    currentPlayer,
                    lastKnown,
                    patrol),
                Is.EqualTo(lastKnown));
        }

        [Test]
        public void HuntUsesCurrentPlayerAndPatrolUsesPatrolTarget()
        {
            var currentPlayer = new Vector3(9f, 0f, 9f);
            var lastKnown = new Vector3(2f, 0f, 3f);
            var patrol = new Vector3(-5f, 0f, 1f);

            Assert.That(
                ThreatTargetPolicy.SelectMovementTarget(
                    PursuitThreatState.Hunting,
                    currentPlayer,
                    lastKnown,
                    patrol),
                Is.EqualTo(currentPlayer));
            Assert.That(
                ThreatTargetPolicy.SelectMovementTarget(
                    PursuitThreatState.Patrolling,
                    currentPlayer,
                    lastKnown,
                    patrol),
                Is.EqualTo(patrol));
        }

        [Test]
        public void WaypointFallback_FailsClosedAcrossAnUnbridgedWall()
        {
            var navigatorObject = new GameObject("Fallback Navigator");
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            try
            {
                navigatorObject.transform.position = new Vector3(-2f, 1.5f, 0f);
                wall.transform.SetPositionAndRotation(new Vector3(0f, 1.5f, 0f), Quaternion.identity);
                wall.transform.localScale = new Vector3(0.25f, 3f, 3f);
                Physics.SyncTransforms();

                var navigator = navigatorObject.AddComponent<WaypointGraphThreatNavigator>();
                navigator.Configure(System.Array.Empty<Transform>(), System.Array.Empty<ThreatWaypointConnection>());
                navigator.SetDestination(new Vector3(2f, 1.5f, 0f), 1f);

                Assert.That(navigator.HasPath, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(wall);
                Object.DestroyImmediate(navigatorObject);
            }
        }
    }
}
