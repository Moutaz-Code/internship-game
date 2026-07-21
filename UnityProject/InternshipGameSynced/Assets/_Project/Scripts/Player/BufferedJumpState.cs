using UnityEngine;

namespace InternshipGame.Prototype
{
    /// <summary>
    /// Pure timing state shared by controller adapters and deterministic tests.
    /// </summary>
    internal sealed class BufferedJumpState
    {
        private readonly float coyoteDuration;
        private readonly float bufferDuration;

        private float coyoteRemaining;
        private float bufferRemaining;
        private bool jumpAvailable;
        private bool wasGrounded;

        public BufferedJumpState(float coyoteDuration, float bufferDuration)
        {
            this.coyoteDuration = Mathf.Max(0f, coyoteDuration);
            this.bufferDuration = Mathf.Max(0f, bufferDuration);
        }

        internal float CoyoteRemaining => coyoteRemaining;
        internal float BufferRemaining => bufferRemaining;
        internal bool JumpAvailable => jumpAvailable;

        public void Reset(bool grounded)
        {
            coyoteRemaining = grounded ? coyoteDuration : 0f;
            bufferRemaining = 0f;
            jumpAvailable = grounded;
            wasGrounded = grounded;
        }

        public bool Tick(bool grounded, bool jumpPressed, float deltaTime)
        {
            var safeDeltaTime = Mathf.Max(0f, deltaTime);

            if (grounded)
            {
                coyoteRemaining = coyoteDuration;
                if (!wasGrounded)
                {
                    jumpAvailable = true;
                }
            }
            else
            {
                coyoteRemaining = Mathf.Max(0f, coyoteRemaining - safeDeltaTime);
            }

            if (jumpPressed)
            {
                bufferRemaining = bufferDuration;
            }
            else
            {
                bufferRemaining = Mathf.Max(0f, bufferRemaining - safeDeltaTime);
            }

            var hasBufferedPress = jumpPressed || bufferRemaining > 0f;
            var isWithinGroundWindow = grounded || coyoteRemaining > 0f;
            var shouldJump = jumpAvailable && hasBufferedPress && isWithinGroundWindow;
            if (shouldJump)
            {
                jumpAvailable = false;
                coyoteRemaining = 0f;
                bufferRemaining = 0f;
            }

            wasGrounded = grounded;
            return shouldJump;
        }
    }
}
