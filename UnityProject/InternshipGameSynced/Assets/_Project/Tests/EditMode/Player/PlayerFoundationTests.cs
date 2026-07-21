using NUnit.Framework;
using UnityEngine;

namespace InternshipGame.Prototype.Tests
{
    public sealed class PlayerFoundationTests
    {
        [TestCase(30f)]
        [TestCase(60f)]
        [TestCase(120f)]
        public void VerticalMotion_ReachesConfiguredApexAcrossFrameRates(float framesPerSecond)
        {
            const float gravity = -19.62f;
            const float targetHeight = 0.9f;
            var deltaTime = 1f / framesPerSecond;
            var velocity = PlayerVerticalMotion.CalculateJumpSpeed(gravity, targetHeight);
            var height = 0f;
            var apex = 0f;

            for (var i = 0; i < 300 && height >= 0f; i++)
            {
                height += PlayerVerticalMotion.IntegrateDisplacement(velocity, gravity, deltaTime);
                velocity = PlayerVerticalMotion.IntegrateVelocity(velocity, gravity, deltaTime);
                apex = Mathf.Max(apex, height);
            }

            Assert.That(apex, Is.EqualTo(targetHeight).Within(0.002f));
        }

        [Test]
        public void VerticalMotion_CeilingCollisionCancelsUpwardVelocity()
        {
            var resolved = PlayerVerticalMotion.ResolveVelocityAfterCollision(
                4.5f,
                CollisionFlags.Above,
                false,
                -2f);

            Assert.That(resolved, Is.Zero);
        }

        [Test]
        public void CeilingCollision_DoesNotRearmConsumedJump()
        {
            var jumpState = new BufferedJumpState(0.12f, 0.12f);
            jumpState.Reset(true);

            Assert.That(jumpState.Tick(true, true, 0.01f), Is.True);
            Assert.That(
                PlayerVerticalMotion.ResolveVelocityAfterCollision(
                    4.5f,
                    CollisionFlags.Above,
                    false,
                    -2f),
                Is.Zero);
            Assert.That(jumpState.Tick(false, true, 0.01f), Is.False);
            Assert.That(jumpState.JumpAvailable, Is.False);
        }

        [Test]
        public void VerticalMotion_LandingImpactUsesRequestedFallSpeed()
        {
            Assert.That(
                PlayerVerticalMotion.CalculateLandingImpactSpeed(true, -6.25f),
                Is.EqualTo(6.25f));
            Assert.That(
                PlayerVerticalMotion.CalculateLandingImpactSpeed(false, -6.25f),
                Is.Zero);
        }

        [Test]
        public void BufferedJump_AllowsCoyoteJumpWithinWindow()
        {
            var state = new BufferedJumpState(0.12f, 0.12f);
            state.Reset(true);

            Assert.That(state.Tick(true, false, 0.01f), Is.False);
            Assert.That(state.Tick(false, false, 0.05f), Is.False);
            Assert.That(state.Tick(false, true, 0.05f), Is.True);
            Assert.That(state.CoyoteRemaining, Is.Zero);
        }

        [Test]
        public void BufferedJump_RejectsCoyoteJumpAfterWindow()
        {
            var state = new BufferedJumpState(0.12f, 0.12f);
            state.Reset(true);

            state.Tick(true, false, 0.01f);
            state.Tick(false, false, 0.121f);

            Assert.That(state.Tick(false, true, 0f), Is.False);
        }

        [Test]
        public void BufferedJump_ConsumesBufferedPressOnLanding()
        {
            var state = new BufferedJumpState(0.12f, 0.12f);
            state.Reset(true);

            Assert.That(state.Tick(true, true, 0.01f), Is.True);
            Assert.That(state.Tick(false, false, 0.04f), Is.False);
            Assert.That(state.Tick(false, true, 0.02f), Is.False);
            Assert.That(state.BufferRemaining, Is.GreaterThan(0f));
            Assert.That(state.Tick(true, false, 0.04f), Is.True);
        }

        [Test]
        public void BufferedJump_DoesNotAllowAnAirJump()
        {
            var state = new BufferedJumpState(0.12f, 0.12f);
            state.Reset(true);

            Assert.That(state.Tick(true, true, 0.01f), Is.True);
            Assert.That(state.Tick(false, true, 0.01f), Is.False);
            Assert.That(state.JumpAvailable, Is.False);
        }

        [Test]
        public void GrabbableObject_EnforcesExclusiveOwnership()
        {
            var gameObject = new GameObject("Ownership Test");
            try
            {
                gameObject.AddComponent<Rigidbody>();
                var grabbable = gameObject.AddComponent<GrabbableObject>();
                var firstOwner = new TestGrabOwner();
                var secondOwner = new TestGrabOwner();

                Assert.That(grabbable.TryBeginGrab(firstOwner), Is.True);
                Assert.That(grabbable.TryBeginGrab(secondOwner), Is.False);
                grabbable.EndGrab(secondOwner);
                Assert.That(grabbable.IsOwnedBy(firstOwner), Is.True);

                grabbable.EndGrab(firstOwner);
                Assert.That(grabbable.IsGrabbed, Is.False);
                Assert.That(grabbable.TryBeginGrab(secondOwner), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(gameObject);
            }
        }

        [Test]
        public void DesktopGrab_FollowsGripExactlyAndRestoresState()
        {
            var player = new GameObject("Player");
            var originalParent = new GameObject("Original Parent");
            var item = new GameObject("Item");
            try
            {
                var playerCollider = player.AddComponent<BoxCollider>();
                var cameraObject = new GameObject("Camera");
                cameraObject.transform.SetParent(player.transform, false);
                var camera = cameraObject.AddComponent<Camera>();
                var holdPoint = new GameObject("Hold Point").transform;
                holdPoint.SetParent(cameraObject.transform, false);

                var interactor = player.AddComponent<DesktopGrabInteractor>();
                interactor.Configure(camera, holdPoint, null);

                item.transform.SetParent(originalParent.transform, true);
                var itemCollider = item.AddComponent<BoxCollider>();
                var body = item.AddComponent<Rigidbody>();
                body.isKinematic = false;
                body.useGravity = true;
                body.detectCollisions = true;
                body.interpolation = RigidbodyInterpolation.Interpolate;
                body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                body.constraints = RigidbodyConstraints.FreezeRotationX;
                body.linearVelocity = new Vector3(0.25f, 0.5f, -0.75f);
                body.angularVelocity = new Vector3(1f, 2f, 3f);

                var grabbable = item.AddComponent<GrabbableObject>();
                grabbable.ConfigureDesktopGripPose(
                    new Vector3(0.1f, -0.2f, 0.3f),
                    new Vector3(10f, 20f, 30f));

                Assert.That(interactor.TryGrab(grabbable), Is.True);
                Assert.That(interactor.HeldObject, Is.SameAs(grabbable));
                Assert.That(body.isKinematic, Is.True);
                Assert.That(body.useGravity, Is.False);
                Assert.That(body.detectCollisions, Is.False);
                Assert.That(body.interpolation, Is.EqualTo(RigidbodyInterpolation.None));
                Assert.That(grabbable.IsXrInteractionEnabled, Is.False);
                Assert.That(Physics.GetIgnoreCollision(playerCollider, itemCollider), Is.True);

                holdPoint.SetPositionAndRotation(
                    new Vector3(4f, 2f, -3f),
                    Quaternion.Euler(35f, 125f, -15f));
                interactor.RefreshHeldPose();

                var expectedPosition = holdPoint.TransformPoint(grabbable.DesktopGripPositionOffset);
                var expectedRotation = holdPoint.rotation * grabbable.DesktopGripRotationOffset;
                Assert.That(Vector3.Distance(item.transform.position, expectedPosition), Is.LessThan(0.0001f));
                Assert.That(Quaternion.Angle(item.transform.rotation, expectedRotation), Is.LessThan(0.01f));

                interactor.ReleaseHeldObject(false);

                Assert.That(interactor.HeldObject, Is.Null);
                Assert.That(item.transform.parent, Is.EqualTo(originalParent.transform));
                Assert.That(body.isKinematic, Is.False);
                Assert.That(body.useGravity, Is.True);
                Assert.That(body.detectCollisions, Is.True);
                Assert.That(body.interpolation, Is.EqualTo(RigidbodyInterpolation.Interpolate));
                Assert.That(body.collisionDetectionMode, Is.EqualTo(CollisionDetectionMode.ContinuousDynamic));
                Assert.That(body.constraints, Is.EqualTo(RigidbodyConstraints.FreezeRotationX));
                Assert.That(grabbable.IsXrInteractionEnabled, Is.True);
                Assert.That(Physics.GetIgnoreCollision(playerCollider, itemCollider), Is.False);
            }
            finally
            {
                Object.DestroyImmediate(item);
                Object.DestroyImmediate(originalParent);
                Object.DestroyImmediate(player);
            }
        }

        private sealed class TestGrabOwner : IGrabOwner
        {
            public void ForceRelease(GrabbableObject target)
            {
                target.EndGrab(this);
            }
        }
    }
}
