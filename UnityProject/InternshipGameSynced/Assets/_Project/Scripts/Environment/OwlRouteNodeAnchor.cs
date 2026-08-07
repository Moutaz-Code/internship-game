using UnityEngine;

namespace InternshipGame.Prototype
{
    public enum OwlRouteNodeKind
    {
        Lobe,
        Connector,
        Annex,
        SearchProbe,
    }

    /// <summary>
    /// Authored topology metadata. Route families are explicit only on connector
    /// nodes; lobe, Annex, and search-probe nodes deliberately carry None.
    /// </summary>
    public sealed class OwlRouteNodeAnchor : MonoBehaviour
    {
        [SerializeField] private string nodeId = string.Empty;
        [SerializeField] private OwlRouteNodeKind kind;
        [SerializeField] private FacilityRouteFamily routeFamily;

        public string NodeId => nodeId;
        public OwlRouteNodeKind Kind => kind;
        public FacilityRouteFamily RouteFamily => routeFamily;

        public void Configure(string id, OwlRouteNodeKind nodeKind, FacilityRouteFamily family)
        {
            nodeId = id ?? string.Empty;
            kind = nodeKind;
            routeFamily = nodeKind == OwlRouteNodeKind.Connector
                ? family
                : FacilityRouteFamily.None;
        }
    }
}
