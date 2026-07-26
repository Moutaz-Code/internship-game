using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace InternshipGame.Prototype
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class DesktopFirstPersonController : MonoBehaviour, IPlayerMotionSource
    {
        private const float GroundedVerticalVelocity = -2f;
        private const float CharacterControllerMinimumMoveDistance = 0f;

        [SerializeField] private Camera viewCamera;
        [SerializeField, Min(0.1f)] private float walkSpeed = 3.2f;
        [SerializeField, Min(1f)] private float sprintMultiplier = 1.65f;
        [SerializeField, Min(0.1f)] private float groundAcceleration = 18f;
        [SerializeField, Min(0.1f)] private float groundDeceleration = 24f;
        [SerializeField, Min(0.1f)] private float airAcceleration = 5.5f;
        [SerializeField, Min(0f)] private float airDeceleration = 1.5f;
        [SerializeField, Min(0.01f)] private float mouseSensitivity = 0.12f;
        [SerializeField] private float gravity = -19.62f;
        [SerializeField, Min(0.1f)] private float jumpHeight = 0.9f;
        [SerializeField, Min(0f)] private float coyoteTime = 0.12f;
        [SerializeField, Min(0f)] private float jumpBufferTime = 0.12f;
        [SerializeField, Range(30f, 89f)] private float maxPitch = 82f;

        [Header("Desktop View Feel")]
        [SerializeField] private bool enableViewMotion = true;
        [SerializeField, Min(0f)] private float walkBobAmplitude = 0.018f;
        [SerializeField, Min(0f)] private float sprintBobAmplitude = 0.028f;
        [SerializeField, Min(0.01f)] private float bobCyclesPerMeter = 0.54f;
        [SerializeField, Range(0f, 4f)] private float maximumStrafeRoll = 1.2f;
        [SerializeField, Min(0f)] private float maximumLandingDip = 0.038f;
        [SerializeField, Min(0.1f)] private float viewMotionSharpness = 12f;
        [SerializeField, Min(0f)] private float sprintFieldOfViewBoost = 3f;

        [Header("Desktop Posture")]
        [SerializeField, Min(1f)] private float standingHeight = 1.75f;
        [SerializeField, Min(0.6f)] private float crouchingHeight = 1.15f;
        [SerializeField, Min(0.4f)] private float crouchingEyeHeight = 1.02f;
        [SerializeField, Range(0.2f, 1f)] private float crouchSpeedMultiplier = 0.64f;
        [SerializeField, Min(0.1f)] private float crouchTransitionSpeed = 4.5f;
        [SerializeField, Range(0f, 0.6f)] private float headLeanDistance = 0.34f;
        [SerializeField, Range(0f, 20f)] private float headLeanRoll = 10f;
        [SerializeField, Min(0.1f)] private float headLeanSharpness = 12f;
        [SerializeField, Range(0.02f, 0.2f)] private float headLeanCollisionRadius = 0.09f;
        [SerializeField, Range(0f, 0.1f)] private float headLeanWallClearance = 0.025f;

        private CharacterController characterController;
        private BufferedJumpState jumpState;
        private Vector3 planarVelocity;
        private Vector2 movementInput;
        private Vector3 viewBaseLocalPosition;
        private Vector3 currentViewOffset;
        private float viewBaseFieldOfView;
        private float currentViewRoll;
        private float landingDip;
        private float bobPhase;
        private bool viewBaseCaptured;
        private float pitch;
        private float verticalVelocity;
        private float crouchAmount;
        private float leanAmount;
        private bool motionTelemetryInitialized;
        private bool wasGrounded;
        private readonly Collider[] postureOverlaps = new Collider[16];
        private readonly RaycastHit[] leanHits = new RaycastHit[16];

        public bool IsGrounded { get; private set; }
        public Vector3 Velocity { get; private set; }
        public bool IsSprinting { get; private set; }
        public bool JumpedThisFrame { get; private set; }
        public bool LandedThisFrame { get; private set; }
        public float LandingImpactSpeed { get; private set; }
        public bool IsCrouching => crouchAmount >= 0.5f;
        public float LeanAmount => leanAmount;

        public void Configure(Camera camera)
        {
            viewCamera = camera;
            CaptureViewBase();
        }

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            // Acceleration starts with sub-millimetre steps at high refresh rates.
            // Unity's legacy 1 mm threshold would discard those steps, after which
            // measured velocity returned to zero and locomotion could never ramp up.
            characterController.minMoveDistance = CharacterControllerMinimumMoveDistance;
            standingHeight = Mathf.Max(standingHeight, crouchingHeight + 0.1f);
            characterController.height = standingHeight;
            characterController.center = PlayerPostureMath.CapsuleCenter(standingHeight);
            jumpState = new BufferedJumpState(coyoteTime, jumpBufferTime);
            CaptureViewBase();
            IsGrounded = characterController.isGrounded;
            wasGrounded = IsGrounded;
            jumpState.Reset(IsGrounded);
        }

        private void OnEnable()
        {
            LockCursor();
        }

        private void OnDisable()
        {
            ClearMotionTelemetry();
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void Update()
        {
            HandleCursor();
            HandleLook();
            HandleMovement();
        }

        private void LateUpdate()
        {
            UpdateViewMotion(Mathf.Max(0f, Time.deltaTime));
        }

        public void Teleport(Vector3 position, Quaternion rotation)
        {
            var wasEnabled = characterController != null && characterController.enabled;
            if (wasEnabled)
            {
                characterController.enabled = false;
            }

            transform.SetPositionAndRotation(position, rotation);
            pitch = 0f;
            verticalVelocity = 0f;
            planarVelocity = Vector3.zero;
            movementInput = Vector2.zero;
            Velocity = Vector3.zero;
            IsSprinting = false;
            JumpedThisFrame = false;
            LandedThisFrame = false;
            LandingImpactSpeed = 0f;
            crouchAmount = 0f;
            leanAmount = 0f;
            if (characterController != null)
            {
                characterController.height = standingHeight;
                characterController.center = PlayerPostureMath.CapsuleCenter(standingHeight);
            }
            motionTelemetryInitialized = false;
            ResetViewMotion();

            if (wasEnabled)
            {
                characterController.enabled = true;
            }

            IsGrounded = characterController != null && characterController.enabled && characterController.isGrounded;
            wasGrounded = IsGrounded;
            EnsureJumpState();
            jumpState.Reset(IsGrounded);
        }

        private void HandleCursor()
        {
            // DesktopPauseController owns Escape so pause/resume cannot race this
            // controller and leave the cursor in an indeterminate state.
            if (Mouse.current?.leftButton.wasPressedThisFrame == true && Cursor.lockState != CursorLockMode.Locked)
            {
                LockCursor();
            }
        }

        private void HandleLook()
        {
            if (viewCamera == null || Mouse.current == null || Cursor.lockState != CursorLockMode.Locked)
            {
                return;
            }

            var delta = Mouse.current.delta.ReadValue() * mouseSensitivity;
            transform.Rotate(0f, delta.x, 0f, Space.Self);
            pitch = Mathf.Clamp(pitch - delta.y, -maxPitch, maxPitch);
        }

        private void HandleMovement()
        {
            if (characterController == null || !characterController.enabled)
            {
                ClearMotionTelemetry();
                return;
            }

            EnsureJumpState();
            JumpedThisFrame = false;
            LandedThisFrame = false;
            LandingImpactSpeed = 0f;

            var keyboard = Keyboard.current;
            var deltaTime = Mathf.Max(0f, Time.deltaTime);
            UpdatePosture(keyboard, deltaTime);
            var horizontal = keyboard == null ? 0f : ReadAxis(keyboard.aKey, keyboard.dKey);
            var vertical = keyboard == null ? 0f : ReadAxis(keyboard.sKey, keyboard.wKey);
            var input = Vector2.ClampMagnitude(new Vector2(horizontal, vertical), 1f);
            movementInput = input;
            IsSprinting = keyboard?.leftShiftKey.isPressed == true && input.sqrMagnitude > 0f && crouchAmount <= 0.01f;
            var speed = walkSpeed *
                        (IsSprinting ? sprintMultiplier : 1f) *
                        Mathf.Lerp(1f, crouchSpeedMultiplier, crouchAmount);
            var desiredPlanarVelocity = (transform.right * input.x + transform.forward * input.y) * speed;
            var groundedBeforeMove = characterController.isGrounded;
            var jumpPressed = keyboard?.spaceKey.wasPressedThisFrame == true;
            planarVelocity = PlayerPlanarMotion.Step(
                planarVelocity,
                desiredPlanarVelocity,
                groundedBeforeMove,
                groundAcceleration,
                groundDeceleration,
                airAcceleration,
                airDeceleration,
                deltaTime);

            if (jumpState.Tick(groundedBeforeMove, jumpPressed, deltaTime))
            {
                verticalVelocity = PlayerVerticalMotion.CalculateJumpSpeed(gravity, jumpHeight);
                JumpedThisFrame = true;
            }
            else if (groundedBeforeMove && verticalVelocity < 0f)
            {
                verticalVelocity = GroundedVerticalVelocity;
            }

            float verticalDisplacement;
            if (!groundedBeforeMove || JumpedThisFrame)
            {
                verticalDisplacement = PlayerVerticalMotion.IntegrateDisplacement(
                    verticalVelocity,
                    gravity,
                    deltaTime);
                verticalVelocity = PlayerVerticalMotion.IntegrateVelocity(
                    verticalVelocity,
                    gravity,
                    deltaTime);
            }
            else
            {
                verticalDisplacement = verticalVelocity * deltaTime;
            }

            var previousPosition = transform.position;
            var collisionFlags = characterController.Move(
                planarVelocity * deltaTime + Vector3.up * verticalDisplacement);
            var safeDeltaTime = Mathf.Max(deltaTime, 0.00001f);
            Velocity = (transform.position - previousPosition) / safeDeltaTime;
            planarVelocity = Vector3.ProjectOnPlane(Velocity, Vector3.up);

            IsGrounded = characterController.isGrounded || (collisionFlags & CollisionFlags.Below) != 0;
            LandedThisFrame = motionTelemetryInitialized && !wasGrounded && IsGrounded && verticalVelocity <= 0f;
            LandingImpactSpeed = PlayerVerticalMotion.CalculateLandingImpactSpeed(
                LandedThisFrame,
                verticalVelocity);
            verticalVelocity = PlayerVerticalMotion.ResolveVelocityAfterCollision(
                verticalVelocity,
                collisionFlags,
                IsGrounded,
                GroundedVerticalVelocity);

            wasGrounded = IsGrounded;
            motionTelemetryInitialized = true;
        }

        private void UpdateViewMotion(float deltaTime)
        {
            if (viewCamera == null)
            {
                return;
            }

            CaptureViewBase();
            UpdateLeanInput(deltaTime);
            var postureOffset = Vector3.up * Mathf.Lerp(
                0f,
                crouchingEyeHeight - viewBaseLocalPosition.y,
                crouchAmount);
            var baseViewPosition = viewBaseLocalPosition + postureOffset;
            if (!enableViewMotion)
            {
                viewCamera.transform.localPosition = baseViewPosition + ResolveLeanOffset(baseViewPosition);
                viewCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, -leanAmount * headLeanRoll);
                viewCamera.fieldOfView = viewBaseFieldOfView;
                return;
            }

            var planarSpeed = Vector3.ProjectOnPlane(Velocity, Vector3.up).magnitude;
            var movingOnGround = IsGrounded && planarSpeed > 0.08f;
            var targetOffset = Vector3.zero;
            if (movingOnGround)
            {
                bobPhase += planarSpeed * bobCyclesPerMeter * Mathf.PI * 2f * deltaTime;
                var amplitude = IsSprinting ? sprintBobAmplitude : walkBobAmplitude;
                targetOffset = new Vector3(
                    Mathf.Cos(bobPhase * 0.5f) * amplitude * 0.45f,
                    Mathf.Sin(bobPhase) * amplitude,
                    0f);
            }

            if (LandedThisFrame)
            {
                landingDip = -Mathf.Min(
                    maximumLandingDip,
                    LandingImpactSpeed * maximumLandingDip / 8f);
            }

            var blend = 1f - Mathf.Exp(-viewMotionSharpness * deltaTime);
            currentViewOffset = Vector3.Lerp(currentViewOffset, targetOffset, blend);
            landingDip = Mathf.Lerp(landingDip, 0f, blend);
            var speedRatio = walkSpeed <= 0f ? 0f : Mathf.Clamp01(planarSpeed / walkSpeed);
            var targetRoll = -movementInput.x * maximumStrafeRoll * speedRatio;
            currentViewRoll = Mathf.Lerp(currentViewRoll, targetRoll, blend);

            baseViewPosition += currentViewOffset + Vector3.up * landingDip;
            viewCamera.transform.localPosition = baseViewPosition + ResolveLeanOffset(baseViewPosition);
            viewCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, currentViewRoll - leanAmount * headLeanRoll);
            var targetFieldOfView = viewBaseFieldOfView + (IsSprinting ? sprintFieldOfViewBoost : 0f);
            viewCamera.fieldOfView = Mathf.Lerp(viewCamera.fieldOfView, targetFieldOfView, blend);
        }

        private void CaptureViewBase()
        {
            if (viewBaseCaptured || viewCamera == null)
            {
                return;
            }

            viewBaseLocalPosition = viewCamera.transform.localPosition;
            viewBaseFieldOfView = viewCamera.fieldOfView;
            viewBaseCaptured = true;
        }

        private void ResetViewMotion()
        {
            currentViewOffset = Vector3.zero;
            currentViewRoll = 0f;
            landingDip = 0f;
            bobPhase = 0f;
            if (viewCamera == null || !viewBaseCaptured)
            {
                return;
            }

            viewCamera.transform.localPosition = viewBaseLocalPosition;
            viewCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            viewCamera.fieldOfView = viewBaseFieldOfView;
        }

        private void UpdatePosture(Keyboard keyboard, float deltaTime)
        {
            var crouchRequested = keyboard?.leftCtrlKey.isPressed == true || keyboard?.cKey.isPressed == true;
            var canStand = crouchRequested || crouchAmount <= 0f || HasStandingClearance();
            crouchAmount = PlayerPostureMath.StepCrouchAmount(
                crouchAmount,
                crouchRequested,
                canStand,
                crouchTransitionSpeed,
                deltaTime);

            var height = Mathf.Lerp(standingHeight, crouchingHeight, crouchAmount);
            characterController.height = height;
            characterController.center = PlayerPostureMath.CapsuleCenter(height);
        }

        private bool HasStandingClearance()
        {
            var radius = Mathf.Max(0.02f, characterController.radius - characterController.skinWidth);
            var center = transform.position + Vector3.up * standingHeight * 0.5f;
            var halfCylinder = Mathf.Max(0f, standingHeight * 0.5f - radius);
            var bottom = center - Vector3.up * halfCylinder;
            var top = center + Vector3.up * halfCylinder;
            var hitCount = Physics.OverlapCapsuleNonAlloc(
                bottom,
                top,
                radius,
                postureOverlaps,
                ~0,
                QueryTriggerInteraction.Ignore);
            for (var i = 0; i < hitCount; i++)
            {
                var hit = postureOverlaps[i];
                if (hit != null && hit.transform.root != transform.root)
                {
                    return false;
                }
            }

            return true;
        }

        private void UpdateLeanInput(float deltaTime)
        {
            var keyboard = Keyboard.current;
            var target = 0f;
            if (keyboard?.zKey.isPressed == true)
            {
                target -= 1f;
            }

            if (keyboard?.xKey.isPressed == true)
            {
                target += 1f;
            }

            var blend = 1f - Mathf.Exp(-headLeanSharpness * deltaTime);
            leanAmount = Mathf.Lerp(leanAmount, Mathf.Clamp(target, -1f, 1f), blend);
        }

        private Vector3 ResolveLeanOffset(Vector3 baseLocalPosition)
        {
            var requestedDistance = Mathf.Abs(leanAmount) * headLeanDistance;
            if (requestedDistance <= 0.0001f)
            {
                return Vector3.zero;
            }

            var direction = transform.right * Mathf.Sign(leanAmount);
            var origin = transform.TransformPoint(baseLocalPosition);
            var hitCount = Physics.SphereCastNonAlloc(
                origin,
                headLeanCollisionRadius,
                direction,
                leanHits,
                requestedDistance,
                ~0,
                QueryTriggerInteraction.Ignore);
            var blockingDistance = float.PositiveInfinity;
            for (var i = 0; i < hitCount; i++)
            {
                var hit = leanHits[i];
                if (hit.collider == null || hit.collider.transform.root == transform.root)
                {
                    continue;
                }

                blockingDistance = Mathf.Min(blockingDistance, hit.distance);
            }

            var allowedDistance = PlayerPostureMath.AllowedLeanDistance(
                requestedDistance,
                blockingDistance,
                headLeanWallClearance);
            return Vector3.right * Mathf.Sign(leanAmount) * allowedDistance;
        }

        private void EnsureJumpState()
        {
            jumpState ??= new BufferedJumpState(coyoteTime, jumpBufferTime);
        }

        private void ClearMotionTelemetry()
        {
            IsGrounded = false;
            Velocity = Vector3.zero;
            planarVelocity = Vector3.zero;
            movementInput = Vector2.zero;
            IsSprinting = false;
            JumpedThisFrame = false;
            LandedThisFrame = false;
            LandingImpactSpeed = 0f;
            motionTelemetryInitialized = false;
            wasGrounded = false;
            EnsureJumpState();
            jumpState.Reset(false);
            ResetViewMotion();
        }

        private static float ReadAxis(KeyControl negative, KeyControl positive)
        {
            return (positive.isPressed ? 1f : 0f) - (negative.isPressed ? 1f : 0f);
        }

        private static void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
