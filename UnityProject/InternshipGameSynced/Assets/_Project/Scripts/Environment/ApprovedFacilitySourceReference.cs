using UnityEngine;

namespace InternshipGame.Prototype
{
    public sealed class ApprovedFacilitySourceReference : MonoBehaviour
    {
        [SerializeField] private string layoutId = ApprovedFacilityLayout.LayoutId;
        [SerializeField] private string sourceSha256 = ApprovedFacilityLayout.SourceSha256;
        [SerializeField] private Vector2Int sourceCanvas = new(
            ApprovedFacilityLayout.SourceWidthPixels,
            ApprovedFacilityLayout.SourceHeightPixels);
        [SerializeField] private Vector2 imageOrigin = default;
        [SerializeField] private Vector3 worldOrigin = default;
        [SerializeField] private float pixelsToMeters = ApprovedFacilityLayout.PixelsToMeters;
        [SerializeField] private float worldYawDegrees = ApprovedFacilityLayout.WorldYawDegrees;

        public string LayoutId => layoutId;
        public string SourceSha256 => sourceSha256;
        public Vector2Int SourceCanvas => sourceCanvas;
        public Vector2 ImageOrigin => imageOrigin;
        public Vector3 WorldOrigin => worldOrigin;
        public float PixelsToMeters => pixelsToMeters;
        public float WorldYawDegrees => worldYawDegrees;

        public void ConfigureFromAuthority()
        {
            layoutId = ApprovedFacilityLayout.LayoutId;
            sourceSha256 = ApprovedFacilityLayout.SourceSha256;
            sourceCanvas = new Vector2Int(
                ApprovedFacilityLayout.SourceWidthPixels,
                ApprovedFacilityLayout.SourceHeightPixels);
            imageOrigin = ApprovedFacilityLayout.ImageOrigin;
            worldOrigin = ApprovedFacilityLayout.WorldOrigin;
            pixelsToMeters = ApprovedFacilityLayout.PixelsToMeters;
            worldYawDegrees = ApprovedFacilityLayout.WorldYawDegrees;
        }
    }
}
