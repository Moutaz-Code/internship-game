using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace InternshipGame.Prototype
{
    public interface IGrabOwner
    {
        void ForceRelease(GrabbableObject target);
    }

    [RequireComponent(typeof(Rigidbody), typeof(XRGrabInteractable))]
    public sealed class GrabbableObject : MonoBehaviour
    {
        [Header("Desktop Grip Pose")]
        [SerializeField] private Vector3 desktopGripPositionOffset;
        [SerializeField] private Vector3 desktopGripRotationOffset;

        private XRGrabInteractable xrGrabInteractable;
        private Rigidbody body;
        private object activeOwner;

        public event Action<GrabbableObject> Grabbed;
        public event Action<GrabbableObject> Released;

        public Rigidbody Body
        {
            get
            {
                EnsureDependencies();
                return body;
            }
        }

        public bool IsGrabbed => activeOwner != null;
        public Vector3 DesktopGripPositionOffset => desktopGripPositionOffset;
        public Quaternion DesktopGripRotationOffset => Quaternion.Euler(desktopGripRotationOffset);

        private void Awake()
        {
            EnsureDependencies();
        }

        private void OnEnable()
        {
            EnsureDependencies();
            xrGrabInteractable.selectEntered.AddListener(OnXrSelected);
            xrGrabInteractable.selectExited.AddListener(OnXrDeselected);
        }

        private void OnDisable()
        {
            if (xrGrabInteractable != null)
            {
                xrGrabInteractable.selectEntered.RemoveListener(OnXrSelected);
                xrGrabInteractable.selectExited.RemoveListener(OnXrDeselected);
            }

            ForceRelease();
        }

        public void ConfigureDesktopGripPose(Vector3 localPosition, Vector3 localEulerAngles)
        {
            desktopGripPositionOffset = localPosition;
            desktopGripRotationOffset = localEulerAngles;
        }

        public bool TryBeginGrab(object owner)
        {
            if (owner == null || (activeOwner != null && !ReferenceEquals(activeOwner, owner)))
            {
                return false;
            }

            if (activeOwner != null)
            {
                return true;
            }

            activeOwner = owner;
            Grabbed?.Invoke(this);
            return ReferenceEquals(activeOwner, owner);
        }

        public void EndGrab(object owner)
        {
            if (!ReferenceEquals(activeOwner, owner))
            {
                return;
            }

            activeOwner = null;
            Released?.Invoke(this);
        }

        public void ForceRelease()
        {
            var owner = activeOwner;
            if (owner is IGrabOwner grabOwner)
            {
                grabOwner.ForceRelease(this);
            }

            if (activeOwner != null)
            {
                EnsureDependencies();
                var interactionManager = xrGrabInteractable.interactionManager;
                if (xrGrabInteractable.isSelected && interactionManager != null)
                {
                    interactionManager.CancelInteractableSelection(
                        (IXRSelectInteractable)xrGrabInteractable);
                }
            }

            if (activeOwner != null)
            {
                activeOwner = null;
                Released?.Invoke(this);
            }
        }

        internal bool IsOwnedBy(object owner)
        {
            return ReferenceEquals(activeOwner, owner);
        }

        internal bool IsXrInteractionEnabled
        {
            get
            {
                EnsureDependencies();
                return xrGrabInteractable.enabled;
            }
        }

        internal void SetXrInteractionEnabled(bool isEnabled)
        {
            EnsureDependencies();
            xrGrabInteractable.enabled = isEnabled;
        }

        private void EnsureDependencies()
        {
            body ??= GetComponent<Rigidbody>();
            xrGrabInteractable ??= GetComponent<XRGrabInteractable>();
        }

        private void OnXrSelected(SelectEnterEventArgs args)
        {
            TryBeginGrab(args.interactorObject);
        }

        private void OnXrDeselected(SelectExitEventArgs args)
        {
            EndGrab(args.interactorObject);
        }
    }
}
