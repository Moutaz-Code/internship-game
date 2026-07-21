using UnityEngine;

namespace InternshipGame.Prototype
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class FacilityZoneTrigger : MonoBehaviour
    {
        [SerializeField] private FacilityZoneId zone;
        [SerializeField] private PrototypeMapDefinition mapDefinition;

        public FacilityZoneId Zone => zone;

        public void Configure(FacilityZoneId facilityZone, PrototypeMapDefinition definition)
        {
            zone = facilityZone;
            mapDefinition = definition;
            var trigger = GetComponent<BoxCollider>();
            trigger.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (mapDefinition == null || other == null)
            {
                return;
            }

            if (BelongsToPlayer(other))
            {
                mapDefinition.NotifyZoneEntered(zone);
            }
        }

        internal static bool BelongsToPlayer(Collider other)
        {
            if (other == null)
            {
                return false;
            }

            // The generated prototype shares one scene root, so searching that root's
            // children would classify rods, props, and the Owl as the player. Only the
            // collider's own ancestor chain is relevant for desktop and XR rigs.
            for (var current = other.transform; current != null; current = current.parent)
            {
                if (current.CompareTag("Player"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
