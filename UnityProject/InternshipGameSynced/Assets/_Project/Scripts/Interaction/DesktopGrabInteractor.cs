using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InternshipGame.Prototype
{
    public sealed class DesktopGrabInteractor : MonoBehaviour, IGrabOwner
    {
        [SerializeField] private Camera viewCamera;
        [SerializeField] private Transform holdPoint;
        [SerializeField] private MonoBehaviour playerContextBehaviour;
        [SerializeField, Min(0.25f)] private float interactionDistance = 3f;
        [SerializeField] private LayerMask interactionLayers = ~0;
        [SerializeField, Min(0f)] private float dropForwardSpeed = 0.5f;
        [SerializeField, Min(0f)] private float throwForwardSpeed = 6.25f;
        [SerializeField, Min(0f)] private float throwUpwardSpeed = 0.45f;
        [SerializeField, Min(0f)] private float throwSpinSpeed = 5.5f;

        private readonly List<CollisionPairState> ignoredCollisionPairs = new();
        private GrabbableObject heldObject;
        private HeldObjectState heldState;
        private Collider[] playerColliders;
        private GUIStyle promptStyle;
        private IPlayerContext playerContext;
        private IPlayerMotionSource playerMotion;

        public string CurrentPrompt { get; private set; } = string.Empty;
        public GrabbableObject HeldObject => heldObject;

        public void Configure(Camera camera, Transform objectHoldPoint, MonoBehaviour contextBehaviour)
        {
            viewCamera = camera;
            holdPoint = objectHoldPoint;
            playerContextBehaviour = contextBehaviour;
            playerContext = contextBehaviour as IPlayerContext;
            CachePlayerDependencies();
        }

        private void Awake()
        {
            playerContext = playerContextBehaviour as IPlayerContext;
            CachePlayerDependencies();
        }

        private void Update()
        {
            UpdatePrompt();
            var dropPressed = Keyboard.current?.eKey.wasPressedThisFrame == true;
            var primaryPressed = Mouse.current?.leftButton.wasPressedThisFrame == true &&
                Cursor.lockState == CursorLockMode.Locked;
            if (heldObject != null)
            {
                if (primaryPressed)
                {
                    ThrowHeldObject();
                }
                else if (dropPressed)
                {
                    ReleaseHeldObject(true);
                }

                return;
            }

            if (dropPressed)
            {
                TryGrabTarget();
            }
            else if (primaryPressed)
            {
                TryInteract();
            }
        }

        private void LateUpdate()
        {
            RefreshHeldPose();
        }

        private void OnDisable()
        {
            ReleaseHeldObject(false);
            CurrentPrompt = string.Empty;
        }

        public void ForceRelease(GrabbableObject target)
        {
            if (heldObject == target)
            {
                ReleaseHeldObject(false);
            }
        }

        internal bool ThrowHeldObject()
        {
            if (heldObject == null)
            {
                return false;
            }

            ReleaseHeldObject(GrabReleaseKind.Throw);
            return true;
        }

        private void TryGrabTarget()
        {
            if (!TryRaycast(out var hit))
            {
                return;
            }

            TryGrab(hit.collider.GetComponentInParent<GrabbableObject>());
        }

        public bool TryGrab(GrabbableObject target)
        {
            if (target == null || heldObject != null || holdPoint == null)
            {
                return false;
            }

            var body = target.Body;
            if (body == null)
            {
                return false;
            }

            var capturedState = HeldObjectState.Capture(target);
            if (!target.TryBeginGrab(this))
            {
                return false;
            }

            heldObject = target;
            heldState = capturedState;
            target.SetXrInteractionEnabled(false);

            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.detectCollisions = false;
            body.useGravity = false;
            body.interpolation = RigidbodyInterpolation.None;
            body.collisionDetectionMode = CollisionDetectionMode.Discrete;
            body.isKinematic = true;

            CachePlayerDependencies();
            SetPlayerCollisionIgnored(true);
            target.transform.SetParent(holdPoint, false);
            RefreshHeldPose();
            return true;
        }

        public void ReleaseHeldObject(bool addDropVelocity)
        {
            ReleaseHeldObject(addDropVelocity ? GrabReleaseKind.Drop : GrabReleaseKind.Restore);
        }

        private void ReleaseHeldObject(GrabReleaseKind releaseKind)
        {
            if (heldObject == null)
            {
                return;
            }

            var target = heldObject;
            heldObject = null;
            target.transform.SetParent(heldState.Parent, true);
            SetPlayerCollisionIgnored(false);

            // Re-enable interaction ownership before restoring physics: XRGrabInteractable
            // may adjust its Rigidbody during OnEnable, so the captured body state must be
            // the final authority on a desktop release.
            target.EndGrab(this);
            target.SetXrInteractionEnabled(heldState.XrInteractionEnabled);
            var body = target.Body;
            heldState.RestoreBody(body);
            if (!body.isKinematic && releaseKind is GrabReleaseKind.Drop or GrabReleaseKind.Throw && viewCamera != null)
            {
                var inheritedVelocity = playerMotion?.Velocity ?? Vector3.zero;
                if (releaseKind == GrabReleaseKind.Throw)
                {
                    body.linearVelocity = inheritedVelocity +
                        viewCamera.transform.forward * throwForwardSpeed +
                        Vector3.up * throwUpwardSpeed;
                    body.angularVelocity = viewCamera.transform.right * throwSpinSpeed;
                }
                else
                {
                    body.linearVelocity = inheritedVelocity + viewCamera.transform.forward * dropForwardSpeed;
                }

                body.WakeUp();
            }
            else if (heldState.WasSleeping)
            {
                body.Sleep();
            }

            heldState = default;
        }

        internal void RefreshHeldPose()
        {
            if (heldObject == null || holdPoint == null)
            {
                return;
            }

            heldObject.transform.SetPositionAndRotation(
                holdPoint.TransformPoint(heldObject.DesktopGripPositionOffset),
                holdPoint.rotation * heldObject.DesktopGripRotationOffset);
        }

        private void CachePlayerDependencies()
        {
            playerColliders = GetComponentsInChildren<Collider>(true);
            playerMotion = GetComponent<IPlayerMotionSource>();
        }

        private void SetPlayerCollisionIgnored(bool ignored)
        {
            if (!ignored)
            {
                foreach (var pair in ignoredCollisionPairs)
                {
                    if (pair.PlayerCollider != null && pair.HeldCollider != null)
                    {
                        Physics.IgnoreCollision(pair.PlayerCollider, pair.HeldCollider, pair.WasIgnored);
                    }
                }

                ignoredCollisionPairs.Clear();
                return;
            }

            ignoredCollisionPairs.Clear();
            if (playerColliders == null || heldObject == null)
            {
                return;
            }

            var heldColliders = heldObject.GetComponentsInChildren<Collider>(true);
            foreach (var playerCollider in playerColliders)
            {
                if (playerCollider == null)
                {
                    continue;
                }

                foreach (var heldCollider in heldColliders)
                {
                    if (heldCollider == null || heldCollider == playerCollider)
                    {
                        continue;
                    }

                    var wasIgnored = Physics.GetIgnoreCollision(playerCollider, heldCollider);
                    ignoredCollisionPairs.Add(new CollisionPairState(playerCollider, heldCollider, wasIgnored));
                    Physics.IgnoreCollision(playerCollider, heldCollider, true);
                }
            }
        }

        private void TryInteract()
        {
            if (!TryRaycast(out var hit))
            {
                return;
            }

            var behaviours = hit.collider.GetComponentsInParent<MonoBehaviour>(true);
            var context = new PlayerInteractionContext(playerContext);
            foreach (var behaviour in behaviours)
            {
                if (behaviour is IPlayerInteractable interactable && interactable.CanInteract(context))
                {
                    interactable.Interact(context);
                    return;
                }
            }
        }

        private void UpdatePrompt()
        {
            if (heldObject != null)
            {
                CurrentPrompt = "[E] DROP    [LMB] THROW";
                return;
            }

            if (!TryRaycast(out var hit))
            {
                CurrentPrompt = string.Empty;
                return;
            }

            if (hit.collider.GetComponentInParent<GrabbableObject>() != null)
            {
                CurrentPrompt = "[E] PICK UP";
                return;
            }

            var context = new PlayerInteractionContext(playerContext);
            foreach (var behaviour in hit.collider.GetComponentsInParent<MonoBehaviour>(true))
            {
                if (behaviour is IPlayerInteractable interactable && interactable.CanInteract(context))
                {
                    CurrentPrompt = $"[LMB] {interactable.InteractionPrompt}";
                    return;
                }
            }

            CurrentPrompt = string.Empty;
        }

        private bool TryRaycast(out RaycastHit hit)
        {
            hit = default;
            return viewCamera != null && Physics.Raycast(
                viewCamera.transform.position,
                viewCamera.transform.forward,
                out hit,
                interactionDistance,
                interactionLayers,
                QueryTriggerInteraction.Ignore);
        }

        private void OnGUI()
        {
            promptStyle ??= new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16,
                normal = { textColor = Color.white },
            };

            var centerX = Screen.width * 0.5f;
            var centerY = Screen.height * 0.5f;
            GUI.Label(new Rect(centerX - 12f, centerY - 14f, 24f, 24f), "+", promptStyle);
            if (!string.IsNullOrEmpty(CurrentPrompt))
            {
                GUI.Box(new Rect(centerX - 135f, centerY + 28f, 270f, 30f), GUIContent.none);
                GUI.Label(new Rect(centerX - 135f, centerY + 28f, 270f, 30f), CurrentPrompt, promptStyle);
            }
        }

        private enum GrabReleaseKind
        {
            Restore,
            Drop,
            Throw,
        }

        private readonly struct CollisionPairState
        {
            public CollisionPairState(Collider playerCollider, Collider heldCollider, bool wasIgnored)
            {
                PlayerCollider = playerCollider;
                HeldCollider = heldCollider;
                WasIgnored = wasIgnored;
            }

            public Collider PlayerCollider { get; }
            public Collider HeldCollider { get; }
            public bool WasIgnored { get; }
        }

        private readonly struct HeldObjectState
        {
            private HeldObjectState(
                Transform parent,
                bool isKinematic,
                bool useGravity,
                bool detectCollisions,
                RigidbodyInterpolation interpolation,
                CollisionDetectionMode collisionDetectionMode,
                RigidbodyConstraints constraints,
                Vector3 linearVelocity,
                Vector3 angularVelocity,
                bool wasSleeping,
                bool xrInteractionEnabled)
            {
                Parent = parent;
                IsKinematic = isKinematic;
                UseGravity = useGravity;
                DetectCollisions = detectCollisions;
                Interpolation = interpolation;
                CollisionDetectionMode = collisionDetectionMode;
                Constraints = constraints;
                LinearVelocity = linearVelocity;
                AngularVelocity = angularVelocity;
                WasSleeping = wasSleeping;
                XrInteractionEnabled = xrInteractionEnabled;
            }

            public Transform Parent { get; }
            public bool IsKinematic { get; }
            public bool UseGravity { get; }
            public bool DetectCollisions { get; }
            public RigidbodyInterpolation Interpolation { get; }
            public CollisionDetectionMode CollisionDetectionMode { get; }
            public RigidbodyConstraints Constraints { get; }
            public Vector3 LinearVelocity { get; }
            public Vector3 AngularVelocity { get; }
            public bool WasSleeping { get; }
            public bool XrInteractionEnabled { get; }

            public static HeldObjectState Capture(GrabbableObject target)
            {
                var body = target.Body;
                return new HeldObjectState(
                    target.transform.parent,
                    body.isKinematic,
                    body.useGravity,
                    body.detectCollisions,
                    body.interpolation,
                    body.collisionDetectionMode,
                    body.constraints,
                    body.linearVelocity,
                    body.angularVelocity,
                    body.IsSleeping(),
                    target.IsXrInteractionEnabled);
            }

            public void RestoreBody(Rigidbody body)
            {
                body.isKinematic = IsKinematic;
                body.useGravity = UseGravity;
                body.detectCollisions = DetectCollisions;
                body.interpolation = Interpolation;
                body.collisionDetectionMode = CollisionDetectionMode;
                body.constraints = Constraints;

                if (!body.isKinematic)
                {
                    body.linearVelocity = LinearVelocity;
                    body.angularVelocity = AngularVelocity;
                }
            }
        }
    }
}
