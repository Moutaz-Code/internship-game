using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace InternshipGame.Prototype
{
    public sealed class PlayerModeController : MonoBehaviour, IPlayerContext, IPrototypeStatusProvider
    {
        [SerializeField] private PlayerControlMode configuredMode = PlayerControlMode.Desktop;
        [SerializeField] private GameObject desktopRoot;
        [SerializeField] private GameObject xrRoot;
        [SerializeField] private Transform desktopView;
        [SerializeField] private Transform xrView;
        [SerializeField] private Transform desktopInteractionOrigin;
        [SerializeField] private Transform xrInteractionOrigin;
        [SerializeField] private MonoBehaviour mapContextBehaviour;

        private IMapContext mapContext;

        public PlayerControlMode ActiveMode { get; private set; } = PlayerControlMode.Desktop;
        public Transform PlayerRoot => (ActiveMode == PlayerControlMode.VR ? xrRoot : desktopRoot)?.transform;
        public Transform ViewTransform => ActiveMode == PlayerControlMode.VR ? xrView : desktopView;
        public Transform InteractionOrigin => ActiveMode == PlayerControlMode.VR ? xrInteractionOrigin : desktopInteractionOrigin;
        public string StatusLabel => $"Player: {ActiveMode} (configured {configuredMode})";

        public void Configure(
            PlayerControlMode mode,
            GameObject desktopPlayerRoot,
            Transform desktopViewTransform,
            Transform desktopInteractionTransform,
            GameObject xrPlayerRoot,
            Transform xrViewTransform,
            Transform xrInteractionTransform,
            MonoBehaviour sceneContext)
        {
            configuredMode = mode;
            desktopRoot = desktopPlayerRoot;
            desktopView = desktopViewTransform;
            desktopInteractionOrigin = desktopInteractionTransform;
            xrRoot = xrPlayerRoot;
            xrView = xrViewTransform;
            xrInteractionOrigin = xrInteractionTransform;
            mapContextBehaviour = sceneContext;
            mapContext = sceneContext as IMapContext;
        }

        private void Awake()
        {
            mapContext = mapContextBehaviour as IMapContext;
            ApplyMode(PlayerControlModeResolver.Resolve(configuredMode, IsXrRuntimeAvailable()));
        }

        private void Start()
        {
            MoveActiveRigToSpawn();
        }

        public void SetMode(PlayerControlMode mode)
        {
            configuredMode = mode;
            ApplyMode(PlayerControlModeResolver.Resolve(mode, IsXrRuntimeAvailable()));
            MoveActiveRigToSpawn();
        }

        private void ApplyMode(PlayerControlMode requestedMode)
        {
            var resolvedMode = requestedMode;
            if (resolvedMode == PlayerControlMode.VR && xrRoot == null)
            {
                Debug.LogWarning("VR player mode was requested, but no XR rig is configured. Falling back to Desktop.", this);
                resolvedMode = PlayerControlMode.Desktop;
            }

            if (resolvedMode == PlayerControlMode.Desktop && desktopRoot == null && xrRoot != null)
            {
                Debug.LogWarning("Desktop player mode was requested, but no desktop rig is configured. Falling back to VR.", this);
                resolvedMode = PlayerControlMode.VR;
            }

            ActiveMode = resolvedMode;
            if (desktopRoot != null)
            {
                desktopRoot.SetActive(ActiveMode == PlayerControlMode.Desktop);
            }

            if (xrRoot != null)
            {
                xrRoot.SetActive(ActiveMode == PlayerControlMode.VR);
            }
        }

        private void MoveActiveRigToSpawn()
        {
            if (mapContext == null)
            {
                return;
            }

            var activeRoot = ActiveMode == PlayerControlMode.VR ? xrRoot : desktopRoot;
            if (activeRoot == null)
            {
                return;
            }

            var desktopController = activeRoot.GetComponent<DesktopFirstPersonController>();
            if (desktopController != null)
            {
                desktopController.Teleport(mapContext.PlayerSpawnPosition, mapContext.PlayerSpawnRotation);
                return;
            }

            activeRoot.transform.SetPositionAndRotation(mapContext.PlayerSpawnPosition, mapContext.PlayerSpawnRotation);
        }

        private static bool IsXrRuntimeAvailable()
        {
            var subsystems = new List<XRInputSubsystem>();
            SubsystemManager.GetSubsystems(subsystems);
            foreach (var subsystem in subsystems)
            {
                if (subsystem != null && subsystem.running)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
