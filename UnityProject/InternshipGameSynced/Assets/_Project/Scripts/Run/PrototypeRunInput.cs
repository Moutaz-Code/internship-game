using UnityEngine;
using UnityEngine.InputSystem;

namespace InternshipGame.Prototype
{
    public sealed class PrototypeRunInput : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour commandTargetBehaviour;

        private IRunCommandTarget commandTarget;

        public void Configure(MonoBehaviour targetBehaviour)
        {
            commandTargetBehaviour = targetBehaviour;
            commandTarget = targetBehaviour as IRunCommandTarget;
        }

        private void Awake()
        {
            commandTarget = commandTargetBehaviour as IRunCommandTarget;
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard?.rKey.wasPressedThisFrame != true)
            {
                return;
            }

            var wantsNewSeed = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;
            if (wantsNewSeed && commandTargetBehaviour is IRunSeedCommandTarget seedTarget)
            {
                seedTarget.NewSeedAndReset();
                return;
            }

            commandTarget?.ResetRun();
        }
    }
}
