using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace InternshipGame.Prototype.Tests
{
    public sealed class PrototypePlayModeTests
    {
        private const string PrototypeScenePath = "Assets/_Project/Scenes/P01_CoreLoop.unity";

        [UnityTest]
        public IEnumerator DesktopGrab_FollowsRealLateUpdateAndRestoresState()
        {
            var player = new GameObject("PlayMode Test Player");
            var originalParent = new GameObject("PlayMode Test Item Parent");
            var item = new GameObject("PlayMode Test Item");
            DesktopGrabInteractor interactor = null;
            GrabbableObject grabbable = null;

            try
            {
                var playerCollider = player.AddComponent<BoxCollider>();
                var preIgnoredPlayerCollider = player.AddComponent<SphereCollider>();
                var cameraObject = new GameObject("PlayMode Test Camera");
                cameraObject.transform.SetParent(player.transform, false);
                var camera = cameraObject.AddComponent<Camera>();
                camera.enabled = false;
                var holdPoint = new GameObject("PlayMode Test Hold Anchor").transform;
                holdPoint.SetParent(cameraObject.transform, false);

                interactor = player.AddComponent<DesktopGrabInteractor>();
                interactor.Configure(camera, holdPoint, null);

                originalParent.transform.SetPositionAndRotation(
                    new Vector3(-2f, 0.5f, 1f),
                    Quaternion.Euler(0f, 22f, 0f));
                item.transform.SetParent(originalParent.transform, false);
                item.transform.localPosition = new Vector3(0.4f, 0.8f, -0.2f);
                var itemCollider = item.AddComponent<BoxCollider>();
                Physics.IgnoreCollision(preIgnoredPlayerCollider, itemCollider, true);
                var body = item.AddComponent<Rigidbody>();
                body.isKinematic = false;
                body.useGravity = true;
                body.detectCollisions = true;
                body.interpolation = RigidbodyInterpolation.Interpolate;
                body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                // Use a non-rotational constraint so Unity does not project angular
                // velocity differently after the deliberately changed held rotation.
                body.constraints = RigidbodyConstraints.FreezePositionY;
                var originalLinearVelocity = new Vector3(0.35f, -0.15f, 0.2f);
                var originalAngularVelocity = new Vector3(-0.1f, 0.25f, 0.4f);

                grabbable = item.AddComponent<GrabbableObject>();
                grabbable.ConfigureDesktopGripPose(
                    new Vector3(0.12f, -0.08f, 0.27f),
                    new Vector3(14f, 31f, -9f));
                var xrInteractable = item.GetComponent<XRGrabInteractable>();
                Assert.That(xrInteractable, Is.Not.Null);
                Assert.That(xrInteractable.enabled, Is.True);
                body.linearVelocity = originalLinearVelocity;
                body.angularVelocity = originalAngularVelocity;
                var capturedLinearVelocity = body.linearVelocity;
                var capturedAngularVelocity = body.angularVelocity;

                Assert.That(interactor.TryGrab(grabbable), Is.True);
                Assert.That(interactor.HeldObject, Is.SameAs(grabbable));
                Assert.That(item.transform.parent, Is.EqualTo(holdPoint));
                Assert.That(body.isKinematic, Is.True);
                Assert.That(body.useGravity, Is.False);
                Assert.That(body.detectCollisions, Is.False);
                Assert.That(body.interpolation, Is.EqualTo(RigidbodyInterpolation.None));
                Assert.That(body.collisionDetectionMode, Is.EqualTo(CollisionDetectionMode.Discrete));
                Assert.That(xrInteractable.enabled, Is.False);
                Assert.That(Physics.GetIgnoreCollision(playerCollider, itemCollider), Is.True);
                Assert.That(Physics.GetIgnoreCollision(preIgnoredPlayerCollider, itemCollider), Is.True);

                holdPoint.SetPositionAndRotation(
                    new Vector3(5.4f, 2.25f, -3.8f),
                    Quaternion.Euler(28f, 137f, -17f));

                // Do not call the internal pose-refresh helper: this frame exercises the
                // component's real LateUpdate ordering and transform propagation.
                yield return null;

                var expectedPosition = holdPoint.TransformPoint(grabbable.DesktopGripPositionOffset);
                var expectedRotation = holdPoint.rotation * grabbable.DesktopGripRotationOffset;
                Assert.That(
                    Vector3.Distance(item.transform.position, expectedPosition),
                    Is.LessThan(0.01f),
                    "Held object exceeded the 1 cm follow-error gate.");
                Assert.That(
                    Quaternion.Angle(item.transform.rotation, expectedRotation),
                    Is.LessThan(0.5f),
                    "Held object exceeded the 0.5 degree follow-error gate.");

                interactor.ReleaseHeldObject(false);

                Assert.That(interactor.HeldObject, Is.Null);
                Assert.That(item.transform.parent, Is.EqualTo(originalParent.transform));
                Assert.That(body.isKinematic, Is.False);
                Assert.That(body.useGravity, Is.True);
                Assert.That(body.detectCollisions, Is.True);
                Assert.That(body.interpolation, Is.EqualTo(RigidbodyInterpolation.Interpolate));
                Assert.That(body.collisionDetectionMode, Is.EqualTo(CollisionDetectionMode.ContinuousDynamic));
                Assert.That(body.constraints, Is.EqualTo(RigidbodyConstraints.FreezePositionY));
                Assert.That(
                    body.linearVelocity,
                    Is.EqualTo(capturedLinearVelocity).Using(Vector3ComparerWithEqualsOperator.Instance));
                Assert.That(
                    body.angularVelocity,
                    Is.EqualTo(capturedAngularVelocity).Using(Vector3ComparerWithEqualsOperator.Instance));
                Assert.That(xrInteractable.enabled, Is.True);
                Assert.That(Physics.GetIgnoreCollision(playerCollider, itemCollider), Is.False);
                Assert.That(Physics.GetIgnoreCollision(preIgnoredPlayerCollider, itemCollider), Is.True);
            }
            finally
            {
                if (interactor != null && grabbable != null)
                {
                    interactor.ForceRelease(grabbable);
                }

                Object.Destroy(item);
                Object.Destroy(originalParent);
                Object.Destroy(player);
            }
        }

        [UnityTest]
        public IEnumerator IntegratedPrototype_HasExpectedCountsAndCompleteNavMeshRoutes()
        {
            var loadOperation = SceneManager.LoadSceneAsync(PrototypeScenePath, LoadSceneMode.Single);
            Assert.That(loadOperation, Is.Not.Null, $"Could not load {PrototypeScenePath}.");
            while (!loadOperation.isDone)
            {
                yield return null;
            }

            // Allow Awake/Start and the baked NavMesh agent registration to settle.
            yield return null;
            yield return null;

            Assert.That(SceneManager.GetActiveScene().path, Is.EqualTo(PrototypeScenePath));

            var maps = Object.FindObjectsByType<PrototypeMapDefinition>(FindObjectsInactive.Include);
            var sockets = Object.FindObjectsByType<RodSocketAnchor>(FindObjectsInactive.Include);
            var rods = Object.FindObjectsByType<FuelRodItem>(FindObjectsInactive.Include);
            var objectiveSets = Object.FindObjectsByType<FuelRodObjectiveSet>(FindObjectsInactive.Include);
            var racks = Object.FindObjectsByType<FuelRodDepositRack>(FindObjectsInactive.Include);
            var owls = Object.FindObjectsByType<PursuitThreatController>(FindObjectsInactive.Include);
            var navigators = Object.FindObjectsByType<NavMeshThreatNavigator>(FindObjectsInactive.Include);

            Assert.That(maps, Has.Length.EqualTo(1));
            Assert.That(sockets, Has.Length.EqualTo(8));
            Assert.That(maps[0].RodSockets.Count, Is.EqualTo(8));
            Assert.That(rods, Has.Length.EqualTo(5));
            Assert.That(objectiveSets, Has.Length.EqualTo(1));
            Assert.That(objectiveSets[0].Items.Count, Is.EqualTo(5));
            Assert.That(objectiveSets[0].RequiredLiveRods, Is.EqualTo(3));
            Assert.That(racks, Has.Length.EqualTo(1));
            Assert.That(
                racks[0].StatusLabel,
                Does.Contain("/3"),
                "The rack's public status must report three configured slots.");
            Assert.That(owls, Has.Length.EqualTo(1));
            Assert.That(navigators, Has.Length.EqualTo(1));
            Assert.That(navigators[0].gameObject, Is.SameAs(owls[0].gameObject));

            var xrJump = System.Array.Find(
                Object.FindObjectsByType<Transform>(FindObjectsInactive.Include),
                candidate => candidate.name == "Jump" && candidate.parent != null && candidate.parent.name == "Locomotion");
            Assert.That(xrJump, Is.Not.Null, "XR rig jump object was not found.");
            Assert.That(xrJump.gameObject.activeSelf, Is.False, "XR artificial jump must remain comfort-gated.");

            var map = maps[0];
            var owl = owls[0];
            var navigator = navigators[0];
            var agent = navigator.GetComponent<NavMeshAgent>();
            Assert.That(agent, Is.Not.Null);
            Assert.That(map.OwlSpawnAnchor, Is.Not.Null);
            Assert.That(map.ExtractionRackAnchor, Is.Not.Null);
            var owlPresentation = owl.transform.Find("Owl Presentation");
            Assert.That(owlPresentation, Is.Not.Null);
            Assert.That(map.OwlSpawnAnchor.position.y, Is.LessThan(0.1f));
            Assert.That(owlPresentation.localPosition.y, Is.EqualTo(0.75f).Within(0.001f));
            Assert.That(agent.baseOffset, Is.EqualTo(0f).Within(0.001f));

            var originalRouteProfile = map.ActiveRouteProfileId;
            var originalLightingProfile = map.ActiveLightingProfileId;
            var routeProfiles = new[]
            {
                PrototypeVariationIds.MaintenanceObstruction,
                PrototypeVariationIds.LaboratoryQuarantine,
                PrototypeVariationIds.CentralPowerOutage,
            };

            try
            {
                foreach (var routeProfile in routeProfiles)
                {
                    navigator.Stop();
                    map.ApplyAuthoredVariation(routeProfile, PrototypeVariationIds.LightingStable);
                    owl.SetRouteProfile(routeProfile);

                    // Carving is asynchronous and stationary obstacles use a debounce.
                    yield return new WaitForSeconds(0.65f);

                    Assert.That(
                        NavMesh.SamplePosition(
                            map.OwlSpawnAnchor.position,
                            out var startHit,
                            3f,
                            NavMesh.AllAreas),
                        Is.True,
                        $"Owl spawn is off NavMesh for profile '{routeProfile}'.");
                    Assert.That(
                        NavMesh.SamplePosition(
                            map.ExtractionRackAnchor.position,
                            out var destinationHit,
                            3f,
                            NavMesh.AllAreas),
                        Is.True,
                        $"Extraction rack is off NavMesh for profile '{routeProfile}'.");

                    Assert.That(
                        NavMesh.SamplePosition(map.PlayerSpawnPosition, out var playerStartHit, 3f, NavMesh.AllAreas),
                        Is.True,
                        $"Player spawn is off NavMesh for profile '{routeProfile}'.");
                    foreach (var socket in map.RodSockets)
                    {
                        Assert.That(
                            NavMesh.SamplePosition(socket.SpawnPoint.position, out var socketHit, 3f, NavMesh.AllAreas),
                            Is.True,
                            $"Socket '{socket.SocketId}' is off NavMesh for profile '{routeProfile}'.");
                        var socketPath = new NavMeshPath();
                        Assert.That(
                            NavMesh.CalculatePath(playerStartHit.position, socketHit.position, NavMesh.AllAreas, socketPath),
                            Is.True,
                            $"Player route to socket '{socket.SocketId}' failed for '{routeProfile}'.");
                        Assert.That(
                            socketPath.status,
                            Is.EqualTo(NavMeshPathStatus.PathComplete),
                            $"Player route to socket '{socket.SocketId}' is incomplete for '{routeProfile}'.");
                    }

                    var calculatedPath = new NavMeshPath();
                    Assert.That(
                        NavMesh.CalculatePath(
                            startHit.position,
                            destinationHit.position,
                            NavMesh.AllAreas,
                            calculatedPath),
                        Is.True,
                        $"NavMesh failed to calculate profile '{routeProfile}'.");
                    Assert.That(
                        calculatedPath.status,
                        Is.EqualTo(NavMeshPathStatus.PathComplete),
                        $"North-to-extraction path is incomplete for profile '{routeProfile}'.");

                    navigator.Warp(startHit.position, map.OwlSpawnAnchor.rotation);
                    navigator.SetDestination(destinationHit.position, 1f);

                    // Give the agent one update to enqueue the request before polling.
                    yield return null;
                    for (var frame = 0; frame < 20 && agent.pathPending; frame++)
                    {
                        yield return null;
                    }

                    Assert.That(navigator.UsingNavMesh, Is.True, $"Wrapper fell back for '{routeProfile}'.");
                    Assert.That(
                        owl.transform.position.y,
                        Is.LessThan(0.15f),
                        "Owl root must remain on the navigation floor; presentation owns the visual height.");
                    Assert.That(agent.hasPath, Is.True, $"Wrapper has no path for '{routeProfile}'.");
                    Assert.That(
                        agent.pathStatus,
                        Is.EqualTo(NavMeshPathStatus.PathComplete),
                        $"Wrapper path is incomplete for '{routeProfile}'.");
                }
            }
            finally
            {
                navigator.Stop();
                map.ApplyAuthoredVariation(originalRouteProfile, originalLightingProfile);
                owl.SetRouteProfile(originalRouteProfile);
            }
        }

        [UnityTest]
        public IEnumerator IntegratedPrototype_CompletesAssayDepositHoldoutEscapeTimeoutAndSeedReset()
        {
            var loadOperation = SceneManager.LoadSceneAsync(PrototypeScenePath, LoadSceneMode.Single);
            Assert.That(loadOperation, Is.Not.Null);
            while (!loadOperation.isDone)
            {
                yield return null;
            }

            yield return null;
            yield return null;

            var run = Object.FindFirstObjectByType<PrototypeRunController>(FindObjectsInactive.Include);
            var rack = Object.FindFirstObjectByType<FuelRodDepositRack>(FindObjectsInactive.Include);
            var rods = Object.FindObjectsByType<FuelRodItem>(FindObjectsInactive.Include);
            Assert.That(run, Is.Not.Null);
            Assert.That(rack, Is.Not.Null);
            Assert.That(run.CurrentState, Is.EqualTo(PrototypeRunState.RetrieveFuelRods));

            var liveRods = new List<FuelRodItem>();
            FuelRodItem inertRod = null;
            foreach (var rod in rods)
            {
                if (rod.Classification == FuelRodClassification.Live)
                {
                    liveRods.Add(rod);
                }
                else
                {
                    inertRod ??= rod;
                }
            }

            Assert.That(liveRods, Has.Count.EqualTo(3));
            Assert.That(inertRod, Is.Not.Null);
            Assert.That(rack.TryDeposit(liveRods[0]), Is.EqualTo(DepositResult.Unassayed));
            inertRod.AdvanceAssay(3f, true);
            Assert.That(rack.TryDeposit(inertRod), Is.EqualTo(DepositResult.Inert));
            Assert.That(run.Progress.Deposited, Is.Zero);

            run.ConfigureTiming(PrototypeRunController.DefaultRunDurationSeconds, 0.05f);
            foreach (var rod in liveRods)
            {
                Assert.That(rod.AdvanceAssay(3f, true), Is.True);
                Assert.That(rack.TryDeposit(rod), Is.EqualTo(DepositResult.Accepted));
            }

            Assert.That(run.Progress.Deposited, Is.EqualTo(3));
            Assert.That(run.CurrentState, Is.EqualTo(PrototypeRunState.ExtractionCycling));
            for (var frame = 0; frame < 30 && run.CurrentState == PrototypeRunState.ExtractionCycling; frame++)
            {
                yield return null;
            }

            Assert.That(run.CurrentState, Is.EqualTo(PrototypeRunState.EscapeReady));
            Assert.That(run.TryEscape(), Is.True);
            Assert.That(run.CurrentState, Is.EqualTo(PrototypeRunState.Extracted));

            run.ResetRun();
            Assert.That(run.CurrentState, Is.EqualTo(PrototypeRunState.RetrieveFuelRods));
            Assert.That(run.Progress.Deposited, Is.Zero);
            foreach (var rod in rods)
            {
                Assert.That(rod.IsDeposited, Is.False);
                Assert.That(rod.IsIdentified, Is.False);
            }

            var signatures = new HashSet<string>();
            foreach (var seed in new[] { 1337, 2027, 9001 })
            {
                run.ConfigureSeed(seed);
                run.ResetRun();
                var configuration = run.CurrentConfiguration;
                Assert.That(configuration.Seed, Is.EqualTo(seed));
                Assert.That(configuration.LiveSocketIds, Has.Count.EqualTo(3));
                Assert.That(configuration.InertSocketIds, Has.Count.EqualTo(2));
                signatures.Add(
                    $"{configuration.RouteProfile}|{configuration.OwlProfile}|{configuration.LightingProfile}|" +
                    string.Join(",", configuration.LiveSocketIds) + "|" +
                    string.Join(",", configuration.InertSocketIds));
            }

            Assert.That(signatures, Has.Count.EqualTo(3));
            var repeated = run.CurrentConfiguration;
            run.ResetRun();
            Assert.That(run.CurrentConfiguration.Seed, Is.EqualTo(repeated.Seed));
            Assert.That(
                string.Join(",", run.CurrentConfiguration.LiveSocketIds),
                Is.EqualTo(string.Join(",", repeated.LiveSocketIds)));

            run.ConfigureTiming(0.05f, 0.05f);
            run.ResetRun();
            for (var frame = 0; frame < 30 && run.CurrentState != PrototypeRunState.Failed; frame++)
            {
                yield return null;
            }

            Assert.That(run.CurrentState, Is.EqualTo(PrototypeRunState.Failed));
            Assert.That(run.LastFailureReason, Does.Contain("lockdown"));
        }
    }
}
