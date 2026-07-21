using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace InternshipGame.Prototype
{
    [RequireComponent(typeof(Rigidbody), typeof(XRGrabInteractable), typeof(GrabbableObject))]
    public class FuelRodItem : MonoBehaviour, IRunObjective
    {
        [SerializeField] private string uniqueId = "fuel-rod";
        [SerializeField] private FuelRodClassification classification = FuelRodClassification.Live;
        [SerializeField, Min(0.1f)] private float assayDurationSeconds = FuelRodAssayStateModel.DefaultDurationSeconds;
        [SerializeField] private MonoBehaviour progressSinkBehaviour;

        private readonly FuelRodAssayStateModel assay = new();
        private IRunProgressSink progressSink;
        private XRGrabInteractable xrGrabInteractable;
        private GrabbableObject grabbable;
        private Rigidbody body;
        private Transform startParent;
        private Vector3 startPosition;
        private Quaternion startRotation;
        private bool startIsKinematic;
        private bool startUseGravity;
        private bool startDetectCollisions;
        private RigidbodyInterpolation startInterpolation;
        private CollisionDetectionMode startCollisionDetectionMode;
        private RigidbodyConstraints startConstraints;
        private bool grabEventsSubscribed;

        public event Action<FuelRodItem> GrabStarted;
        public event Action<FuelRodItem> GrabEnded;
        public event Action<FuelRodItem> AssayCompleted;
        public event Action<FuelRodItem> StateChanged;

        public string ObjectiveId => uniqueId;
        public string UniqueId => uniqueId;
        public FuelRodClassification Classification => classification;
        public FuelRodAssayState AssayState => assay.State;
        public float AssayProgress => assay.NormalizedProgress;
        public bool IsIdentified => assay.WasIdentified;
        public bool IsDeposited => assay.State == FuelRodAssayState.Deposited;
        public bool IsCollected { get; private set; }
        public bool IsComplete => IsDeposited;
        public RunProgressSnapshot Progress => new(
            1,
            1,
            IsIdentified && classification == FuelRodClassification.Live ? 1 : 0,
            IsIdentified && classification == FuelRodClassification.Inert ? 1 : 0,
            IsHeld ? 1 : 0,
            IsDeposited ? 1 : 0);
        public bool IsHeld => Grabbable != null && Grabbable.IsGrabbed;
        public GrabbableObject Grabbable
        {
            get
            {
                grabbable ??= GetComponent<GrabbableObject>();
                EnsureGrabSubscriptions();
                return grabbable;
            }
        }

        public void Configure(MonoBehaviour sinkBehaviour)
        {
            progressSinkBehaviour = sinkBehaviour;
            progressSink = sinkBehaviour as IRunProgressSink;
        }

        public void ConfigureIdentity(string id, FuelRodClassification rodClassification)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Every fuel rod requires a non-empty unique ID.", nameof(id));
            }

            if (IsDeposited)
            {
                throw new InvalidOperationException("A deposited fuel rod cannot be reconfigured before reset.");
            }

            uniqueId = id.Trim();
            classification = rodClassification;
            StateChanged?.Invoke(this);
        }

        public void ConfigureAssayDuration(float seconds)
        {
            assayDurationSeconds = seconds;
            assay.SetDuration(seconds);
        }

        public void ConfigureSpawn(Transform spawn)
        {
            if (spawn == null)
            {
                throw new ArgumentNullException(nameof(spawn));
            }

            xrGrabInteractable ??= GetComponent<XRGrabInteractable>();
            grabbable ??= GetComponent<GrabbableObject>();
            body ??= GetComponent<Rigidbody>();
            ForceReleaseForObjective();

            startParent = spawn;
            startPosition = spawn.position;
            startRotation = spawn.rotation;
            transform.SetParent(startParent, false);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            if (body != null)
            {
                body.isKinematic = startIsKinematic;
                body.useGravity = startUseGravity;
                body.detectCollisions = startDetectCollisions;
                body.interpolation = startInterpolation;
                body.collisionDetectionMode = startCollisionDetectionMode;
                body.constraints = startConstraints;
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                body.Sleep();
            }
        }

        protected virtual void Awake()
        {
            progressSink = progressSinkBehaviour as IRunProgressSink;
            xrGrabInteractable = GetComponent<XRGrabInteractable>();
            grabbable = GetComponent<GrabbableObject>();
            body = GetComponent<Rigidbody>();
            assay.SetDuration(Mathf.Max(0.1f, assayDurationSeconds));
            CaptureStartingState();
        }

        protected virtual void OnEnable()
        {
            grabbable ??= GetComponent<GrabbableObject>();
            EnsureGrabSubscriptions();
        }

        protected virtual void OnDisable()
        {
            if (!grabEventsSubscribed || grabbable == null)
            {
                return;
            }

            grabbable.Grabbed -= OnGrabbed;
            grabbable.Released -= OnReleased;
            grabEventsSubscribed = false;
        }

        private void EnsureGrabSubscriptions()
        {
            if (grabEventsSubscribed || grabbable == null || !isActiveAndEnabled)
            {
                return;
            }

            grabbable.Grabbed += OnGrabbed;
            grabbable.Released += OnReleased;
            grabEventsSubscribed = true;
        }

        private void Update()
        {
            AdvanceAssay(Time.deltaTime, IsHeld);
        }

        public bool AdvanceAssay(float deltaTime, bool isHeld)
        {
            var previousState = assay.State;
            var previousProgress = assay.NormalizedProgress;
            var completed = assay.Tick(deltaTime, isHeld, classification);
            if (previousState != assay.State || !Mathf.Approximately(previousProgress, assay.NormalizedProgress))
            {
                StateChanged?.Invoke(this);
            }

            if (completed)
            {
                AssayCompleted?.Invoke(this);
            }

            return completed;
        }

        private void OnGrabbed(GrabbableObject _)
        {
            if (IsDeposited)
            {
                ForceReleaseForObjective();
                return;
            }

            var firstCollection = !IsCollected;
            IsCollected = true;
            assay.Begin();
            GrabStarted?.Invoke(this);
            StateChanged?.Invoke(this);

            if (firstCollection)
            {
                progressSink?.NotifyObjectiveCollected(this);
            }
        }

        private void OnReleased(GrabbableObject _)
        {
            assay.CancelUnfinished();
            GrabEnded?.Invoke(this);
            StateChanged?.Invoke(this);
        }

        public bool TryMarkDeposited(Transform slot)
        {
            if (!assay.MarkDeposited())
            {
                return false;
            }

            ForceReleaseForObjective();
            if (grabbable != null)
            {
                grabbable.enabled = false;
            }

            if (xrGrabInteractable != null)
            {
                xrGrabInteractable.enabled = false;
            }

            if (slot != null)
            {
                transform.SetParent(slot, false);
                transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }

            if (body != null)
            {
                body.isKinematic = true;
                body.useGravity = false;
                body.detectCollisions = false;
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }

            StateChanged?.Invoke(this);
            return true;
        }

        /// <summary>
        /// Ends whichever grab path currently owns this rod. Desktop owners receive their
        /// normal release callback, while XR selections are explicitly exited through the
        /// interaction manager before the aggregate hands ownership to another rod.
        /// </summary>
        public void ForceReleaseForObjective()
        {
            xrGrabInteractable ??= GetComponent<XRGrabInteractable>();
            grabbable ??= GetComponent<GrabbableObject>();

            if (xrGrabInteractable != null && xrGrabInteractable.isSelected)
            {
                var interactionManager = xrGrabInteractable.interactionManager;
                if (interactionManager != null)
                {
                    var selectors = new List<IXRSelectInteractor>(xrGrabInteractable.interactorsSelecting);
                    for (var index = selectors.Count - 1; index >= 0; index--)
                    {
                        interactionManager.SelectExit(selectors[index], xrGrabInteractable);
                    }
                }
            }

            grabbable?.ForceRelease();
        }

        public virtual void ResetObjective()
        {
            assay.Reset();
            IsCollected = false;

            ForceReleaseForObjective();

            if (xrGrabInteractable != null)
            {
                xrGrabInteractable.enabled = false;
            }

            transform.SetParent(startParent, true);
            transform.SetPositionAndRotation(startPosition, startRotation);

            if (body != null)
            {
                body.isKinematic = startIsKinematic;
                body.useGravity = startUseGravity;
                body.detectCollisions = startDetectCollisions;
                body.interpolation = startInterpolation;
                body.collisionDetectionMode = startCollisionDetectionMode;
                body.constraints = startConstraints;
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                body.Sleep();
            }

            if (grabbable != null)
            {
                grabbable.enabled = true;
            }

            if (xrGrabInteractable != null)
            {
                xrGrabInteractable.enabled = true;
            }

            StateChanged?.Invoke(this);
        }

        private void CaptureStartingState()
        {
            startParent = transform.parent;
            startPosition = transform.position;
            startRotation = transform.rotation;
            if (body == null)
            {
                return;
            }

            startIsKinematic = body.isKinematic;
            startUseGravity = body.useGravity;
            startDetectCollisions = body.detectCollisions;
            startInterpolation = body.interpolation;
            startCollisionDetectionMode = body.collisionDetectionMode;
            startConstraints = body.constraints;
        }
    }
}
