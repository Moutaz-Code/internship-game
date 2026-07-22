using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace InternshipGame.Prototype
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class DesktopFirstPersonController : MonoBehaviour, IPlayerMotionSource
    {
        private const float GroundedVerticalVelocity = -2f;

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
        private bool motionTelemetryInitialized;
        private bool wasGrounded;

        public bool IsGrounded { get; private set; }
        public Vector3 Velocity { get; private set; }
        public bool IsSprinting { get; private set; }
        public bool JumpedThisFrame { get; private set; }
        public bool LandedThisFrame { get; private set; }
        public float LandingImpactSpeed { get; private set; }

        public void Configure(Camera camera)
        {
            viewCamera = camera;
            CaptureViewBase();
        }

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
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
            var horizontal = keyboard == null ? 0f : ReadAxis(keyboard.aKey, keyboard.dKey);
            var vertical = keyboard == null ? 0f : ReadAxis(keyboard.sKey, keyboard.wKey);
            var input = Vector2.ClampMagnitude(new Vector2(horizontal, vertical), 1f);
            movementInput = input;
            IsSprinting = keyboard?.leftShiftKey.isPressed == true && input.sqrMagnitude > 0f;
            var speed = walkSpeed * (IsSprinting ? sprintMultiplier : 1f);
            var desiredPlanarVelocity = (transform.right * input.x + transform.forward * input.y) * speed;
            var groundedBeforeMove = characterController.isGrounded;
            var jumpPressed = keyboard?.spaceKey.wasPressedThisFrame == true;
            var deltaTime = Mathf.Max(0f, Time.deltaTime);
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
            if (!enableViewMotion)
            {
                viewCamera.transform.localPosition = viewBaseLocalPosition;
                viewCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
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

            viewCamera.transform.localPosition =
                viewBaseLocalPosition + currentViewOffset + Vector3.up * landingDip;
            viewCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, currentViewRoll);
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
