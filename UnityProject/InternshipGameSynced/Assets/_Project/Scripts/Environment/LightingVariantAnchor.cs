using System;
using UnityEngine;

namespace InternshipGame.Prototype
{
    public sealed class LightingVariantAnchor : MonoBehaviour
    {
        [SerializeField] private string profileId = string.Empty;
        [SerializeField] private GameObject lightingRoot;

        public string ProfileId => profileId;
        public bool IsApplied { get; private set; }

        public void Configure(string id, GameObject root)
        {
            profileId = id ?? string.Empty;
            lightingRoot = root != null ? root : gameObject;
        }

        public void Apply(string selectedProfileId)
        {
            IsApplied = string.Equals(profileId, selectedProfileId, StringComparison.Ordinal);
            var root = lightingRoot != null ? lightingRoot : gameObject;
            if (root != gameObject || gameObject.activeSelf != IsApplied)
            {
                root.SetActive(IsApplied);
            }
        }
    }
}
