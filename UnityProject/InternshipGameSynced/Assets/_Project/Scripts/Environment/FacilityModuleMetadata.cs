using UnityEngine;

namespace InternshipGame.Prototype
{
    public enum FacilityKitCategory
    {
        Architecture,
        DoorsAndWindows,
        Cover,
        Machinery,
        RoomIdentity,
        Extraction,
    }

    public enum FacilityCoverClass
    {
        None,
        Partial,
        FullCover,
        PerceptionTransparent,
    }

    public enum TraversalActorKind
    {
        Player,
        Owl,
    }

    public enum OneWayOwlPolicy
    {
        FollowPlayerRule,
        AllowBothDirections,
        AlternateRouteOnly,
    }

    public enum TraversalDoorState
    {
        Open,
        Closed,
        Locked,
    }

    /// <summary>Stable, discoverable metadata shared by every FacilityKit prefab.</summary>
    public sealed class FacilityModuleMetadata : MonoBehaviour
    {
        [SerializeField] private string stableId = string.Empty;
        [SerializeField] private FacilityKitCategory category;
        [SerializeField] private Vector3 authoredBounds = Vector3.one;
        [SerializeField] private FacilityCoverClass coverClass;
        [SerializeField] private bool navMeshStatic = true;
        [SerializeField] private bool ceilingToggle;

        public string StableId => stableId;
        public FacilityKitCategory Category => category;
        public Vector3 AuthoredBounds => authoredBounds;
        public FacilityCoverClass CoverClass => coverClass;
        public bool NavMeshStatic => navMeshStatic;
        public bool IsCeilingToggle => ceilingToggle;

        public void Configure(
            string id,
            FacilityKitCategory moduleCategory,
            Vector3 bounds,
            FacilityCoverClass moduleCoverClass = FacilityCoverClass.None,
            bool isNavMeshStatic = true,
            bool isCeilingToggle = false)
        {
            stableId = id ?? string.Empty;
            category = moduleCategory;
            authoredBounds = new Vector3(
                Mathf.Max(0.01f, bounds.x),
                Mathf.Max(0.01f, bounds.y),
                Mathf.Max(0.01f, bounds.z));
            coverClass = moduleCoverClass;
            navMeshStatic = isNavMeshStatic;
            ceilingToggle = isCeilingToggle;
        }

        private void OnDrawGizmosSelected()
        {
            var oldMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = coverClass switch
            {
                FacilityCoverClass.FullCover => new Color(1f, 0.42f, 0.08f, 0.72f),
                FacilityCoverClass.PerceptionTransparent => new Color(0.1f, 0.8f, 1f, 0.72f),
                FacilityCoverClass.Partial => new Color(1f, 0.85f, 0.2f, 0.72f),
                _ => new Color(0.65f, 0.78f, 0.88f, 0.58f),
            };
            Gizmos.DrawWireCube(Vector3.zero, authoredBounds);
            Gizmos.matrix = oldMatrix;
        }
    }
}
