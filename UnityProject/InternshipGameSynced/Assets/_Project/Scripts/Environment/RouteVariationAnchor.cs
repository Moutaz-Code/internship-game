using System;
using UnityEngine;

namespace InternshipGame.Prototype
{
    public sealed class RouteVariationAnchor : MonoBehaviour
    {
        [SerializeField] private string profileId = string.Empty;
        [SerializeField] private GameObject variationRoot;

        public string ProfileId => profileId;
        public bool IsApplied { get; private set; }

        public void Configure(string id, GameObject root)
        {
            profileId = id ?? string.Empty;
            variationRoot = root != null ? root : gameObject;
        }

        public void Apply(string selectedProfileId)
        {
            IsApplied = string.Equals(profileId, selectedProfileId, StringComparison.Ordinal);
            var root = variationRoot != null ? variationRoot : gameObject;
            if (root != gameObject || gameObject.activeSelf != IsApplied)
            {
                root.SetActive(IsApplied);
            }
        }
    }
}
