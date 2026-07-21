using UnityEngine;

namespace InternshipGame.Prototype
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class ExtractionExitTrigger : MonoBehaviour, IRunStateListener, IPrototypeStatusProvider
    {
        [SerializeField] private PrototypeRunController runController;
        [SerializeField] private GameObject closedDoor;

        public bool IsOpen { get; private set; }
        public string StatusLabel => IsOpen ? "Exit: OPEN" : "Exit: sealed";

        public void Configure(PrototypeRunController controller, GameObject door)
        {
            runController = controller;
            closedDoor = door;
            var trigger = GetComponent<BoxCollider>();
            trigger.isTrigger = true;
            SetOpen(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            TryEscape(other);
        }

        public void OnRunReset()
        {
            SetOpen(false);
        }

        public void OnRunStateChanged(PrototypeRunState previous, PrototypeRunState current)
        {
            SetOpen(current is PrototypeRunState.EscapeReady or PrototypeRunState.Extracted);
        }

        private void TryEscape(Collider other)
        {
            if (!IsOpen || runController == null || other == null)
            {
                return;
            }

            if (FacilityZoneTrigger.BelongsToPlayer(other))
            {
                runController.TryEscape();
            }
        }

        private void SetOpen(bool open)
        {
            IsOpen = open;
            if (closedDoor != null)
            {
                closedDoor.SetActive(!open);
            }
        }
    }
}
