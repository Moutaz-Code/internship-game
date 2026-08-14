using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace InternshipGame.Prototype
{
    /// <summary>
    /// Reusable traversal behavior. Presentation is supplied by a moving leaf and
    /// optional indicator/audio hooks; no mesh, animation clip, or door prefab is
    /// hard-coded here. The allowed direction is local +Z and is visible as a gizmo.
    /// </summary>
    public sealed class OneWayTraversalController : MonoBehaviour, IRunStateListener
    {
        private const float SideDeadZone = 0.08f;

        [Header("Direction and permissions")]
        [SerializeField] private Vector3 allowedLocalDirection = Vector3.forward;
        [SerializeField] private bool playerPermission = true;
        [SerializeField] private OneWayOwlPolicy owlPolicy = OneWayOwlPolicy.AlternateRouteOnly;

        [Header("State")]
        [SerializeField] private TraversalDoorState initialState = TraversalDoorState.Closed;
        [SerializeField] private TraversalDoorState currentState = TraversalDoorState.Closed;
        [SerializeField] private bool closeAfterPass = true;
        [SerializeField, Min(0f)] private float closeDelaySeconds = 1.2f;

        [Header("Presentation and collision")]
        [SerializeField] private Transform movingLeaf;
        [SerializeField] private Vector3 openLocalOffset = new(1.5f, 0f, 0f);
        [SerializeField, Min(0.01f)] private float movementSpeed = 3.2f;
        [SerializeField] private Collider blockingCollider;
        [SerializeField] private BoxCollider clearanceVolume;
        [SerializeField] private AudioSource denialAudio;
        [SerializeField] private Light stateLight;
        [SerializeField] private LayerMask obstructionLayers = ~0;

        [Header("Hooks")]
        [SerializeField] private UnityEvent allowed = new();
        [SerializeField] private UnityEvent denied = new();
        [SerializeField] private UnityEvent opened = new();
        [SerializeField] private UnityEvent closed = new();

        private readonly Collider[] obstructionHits = new Collider[32];
        private Vector3 closedLocalPosition;
        private float scheduledCloseTime = float.PositiveInfinity;
        private bool initialized;

        public Vector3 AllowedWorldDirection => transform.TransformDirection(
            allowedLocalDirection.sqrMagnitude > 0.001f ? allowedLocalDirection.normalized : Vector3.forward);
        public bool PlayerPermission => playerPermission;
        public OneWayOwlPolicy OwlPolicy => owlPolicy;
        public TraversalDoorState InitialState => initialState;
        public TraversalDoorState CurrentState => currentState;
        public bool CloseAfterPass => closeAfterPass;
        public float CloseDelaySeconds => closeDelaySeconds;
        public UnityEvent Allowed => allowed;
        public UnityEvent Denied => denied;
        public UnityEvent Opened => opened;
        public UnityEvent Closed => closed;

        public void Configure(
            Transform leaf,
            Collider blocker,
            BoxCollider clearance,
            Vector3 localAllowedDirection,
            bool allowPlayer,
            OneWayOwlPolicy configuredOwlPolicy,
            TraversalDoorState configuredInitialState,
            bool shouldCloseAfterPass,
            float configuredCloseDelaySeconds,
            Vector3 configuredOpenOffset)
        {
            movingLeaf = leaf;
            blockingCollider = blocker;
            clearanceVolume = clearance;
            allowedLocalDirection = localAllowedDirection.sqrMagnitude > 0.001f
                ? localAllowedDirection.normalized
                : Vector3.forward;
            playerPermission = allowPlayer;
            owlPolicy = configuredOwlPolicy;
            initialState = configuredInitialState;
            closeAfterPass = shouldCloseAfterPass;
            closeDelaySeconds = Mathf.Max(0f, configuredCloseDelaySeconds);
            openLocalOffset = configuredOpenOffset;
            InitializeIfNeeded();
            ResetTraversal();
        }

        public bool TryRequestPass(Vector3 actorWorldPosition, TraversalActorKind actorKind)
        {
            InitializeIfNeeded();
            if (currentState == TraversalDoorState.Locked || !HasActorPermission(actorKind) ||
                !IsApproachingFromAllowedSide(actorWorldPosition, actorKind))
            {
                EmitDeniedFeedback();
                return false;
            }

            allowed.Invoke();
            SetState(TraversalDoorState.Open);
            return true;
        }

        public bool IsApproachingFromAllowedSide(Vector3 actorWorldPosition, TraversalActorKind actorKind)
        {
            if (actorKind == TraversalActorKind.Owl && owlPolicy == OneWayOwlPolicy.AllowBothDirections)
            {
                return true;
            }

            var signedSide = Vector3.Dot(actorWorldPosition - transform.position, AllowedWorldDirection);
            return signedSide <= SideDeadZone;
        }

        public bool HasActorPermission(TraversalActorKind actorKind)
        {
            return actorKind switch
            {
                TraversalActorKind.Player => playerPermission,
                TraversalActorKind.Owl => owlPolicy != OneWayOwlPolicy.AlternateRouteOnly,
                _ => false,
            };
        }

        public void NotifyPassComplete()
        {
            if (closeAfterPass && currentState == TraversalDoorState.Open)
            {
                scheduledCloseTime = Time.time + closeDelaySeconds;
            }
        }

        public bool TryCloseNow()
        {
            if (currentState != TraversalDoorState.Open || IsObstructed())
            {
                return false;
            }

            SetState(TraversalDoorState.Closed);
            return true;
        }

        public bool IsObstructed()
        {
            if (clearanceVolume == null)
            {
                return false;
            }

            var center = clearanceVolume.transform.TransformPoint(clearanceVolume.center);
            var scale = clearanceVolume.transform.lossyScale;
            var halfExtents = Vector3.Scale(clearanceVolume.size * 0.5f, new Vector3(
                Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z)));
            var count = Physics.OverlapBoxNonAlloc(
                center,
                halfExtents,
                obstructionHits,
                clearanceVolume.transform.rotation,
                obstructionLayers,
                QueryTriggerInteraction.Ignore);
            for (var i = 0; i < count; i++)
            {
                var hit = obstructionHits[i];
                if (hit == null || hit == blockingCollider || hit == clearanceVolume ||
                    hit.transform == transform || hit.transform.IsChildOf(transform))
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        public void SetState(TraversalDoorState state)
        {
            InitializeIfNeeded();
            currentState = state;
            scheduledCloseTime = float.PositiveInfinity;
            var open = state == TraversalDoorState.Open;
            if (blockingCollider != null)
            {
                blockingCollider.enabled = !open;
            }

            if (movingLeaf != null && !Application.isPlaying)
            {
                movingLeaf.localPosition = closedLocalPosition + (open ? openLocalOffset : Vector3.zero);
            }

            if (stateLight != null)
            {
                stateLight.color = state switch
                {
                    TraversalDoorState.Open => new Color(0.12f, 1f, 0.36f),
                    TraversalDoorState.Locked => new Color(1f, 0.08f, 0.04f),
                    _ => new Color(1f, 0.62f, 0.08f),
                };
                stateLight.enabled = true;
            }

            if (open) opened.Invoke(); else closed.Invoke();
        }

        public void ResetTraversal()
        {
            InitializeIfNeeded();
            SetState(initialState);
        }

        public void OnRunReset()
        {
            ResetTraversal();
        }

        public void OnRunStateChanged(PrototypeRunState previous, PrototypeRunState current)
        {
            if (current == PrototypeRunState.Ready)
            {
                ResetTraversal();
            }
        }

        private void Awake()
        {
            InitializeIfNeeded();
            ResetTraversal();
        }

        private void Update()
        {
            InitializeIfNeeded();
            if (movingLeaf != null)
            {
                var target = closedLocalPosition +
                             (currentState == TraversalDoorState.Open ? openLocalOffset : Vector3.zero);
                movingLeaf.localPosition = Vector3.MoveTowards(
                    movingLeaf.localPosition,
                    target,
                    movementSpeed * Time.deltaTime);
            }

            if (currentState == TraversalDoorState.Open && closeAfterPass &&
                Time.time >= scheduledCloseTime && !IsObstructed())
            {
                SetState(TraversalDoorState.Closed);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == null)
            {
                return;
            }

            if (BelongsToTaggedPlayer(other))
            {
                TryRequestPass(other.transform.position, TraversalActorKind.Player);
                return;
            }

            if (other.GetComponentInParent<NavMeshAgent>() != null)
            {
                TryRequestPass(other.transform.position, TraversalActorKind.Owl);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other != null && (BelongsToTaggedPlayer(other) || other.GetComponentInParent<NavMeshAgent>() != null))
            {
                NotifyPassComplete();
            }
        }

        private void InitializeIfNeeded()
        {
            if (initialized)
            {
                return;
            }

            closedLocalPosition = movingLeaf != null ? movingLeaf.localPosition : Vector3.zero;
            initialized = true;
        }

        private void EmitDeniedFeedback()
        {
            denied.Invoke();
            if (denialAudio != null && denialAudio.enabled && denialAudio.gameObject.activeInHierarchy)
            {
                denialAudio.Play();
            }

            if (stateLight != null)
            {
                stateLight.color = new Color(1f, 0.08f, 0.04f);
                stateLight.enabled = true;
            }
        }

        private static bool BelongsToTaggedPlayer(Collider other)
        {
            for (var current = other.transform; current != null; current = current.parent)
            {
                if (current.CompareTag("Player"))
                {
                    return true;
                }
            }

            return false;
        }

        private void OnDrawGizmos()
        {
            var direction = AllowedWorldDirection;
            Gizmos.color = new Color(0.15f, 1f, 0.42f, 0.9f);
            Gizmos.DrawLine(transform.position - direction * 1.2f, transform.position + direction * 1.4f);
            var tip = transform.position + direction * 1.4f;
            Gizmos.DrawLine(tip, tip - (direction + transform.right) * 0.32f);
            Gizmos.DrawLine(tip, tip - (direction - transform.right) * 0.32f);

            if (clearanceVolume != null)
            {
                var oldMatrix = Gizmos.matrix;
                Gizmos.matrix = clearanceVolume.transform.localToWorldMatrix;
                Gizmos.color = new Color(1f, 0.68f, 0.1f, 0.55f);
                Gizmos.DrawWireCube(clearanceVolume.center, clearanceVolume.size);
                Gizmos.matrix = oldMatrix;
            }
        }
    }
}
