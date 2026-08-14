using UnityEngine;

namespace InternshipGame.Prototype
{
    public sealed class FacilitySemanticAnchor : MonoBehaviour
    {
        [SerializeField] private string stableId = string.Empty;
        [SerializeField] private FacilitySemanticKind kind;
        [SerializeField] private bool gameplayActive;
        [SerializeField] private float clearanceRadius = 1.2f;

        public string StableId => stableId;
        public FacilitySemanticKind Kind => kind;
        public bool GameplayActive => gameplayActive;
        public float ClearanceRadius => clearanceRadius;

        public void Configure(string id, FacilitySemanticKind semanticKind, bool isGameplayActive, float radius = 1.2f)
        {
            stableId = id ?? string.Empty;
            kind = semanticKind;
            gameplayActive = isGameplayActive;
            clearanceRadius = Mathf.Max(0.1f, radius);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = gameplayActive
                ? new Color(0.1f, 1f, 0.35f, 0.82f)
                : new Color(1f, 0.75f, 0.1f, 0.58f);
            Gizmos.DrawWireSphere(transform.position, clearanceRadius);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * clearanceRadius);
        }
    }
}
