using UnityEngine;

namespace InternshipGame.Prototype
{
    public sealed class RodSocketAnchor : MonoBehaviour
    {
        [SerializeField] private string socketId = string.Empty;
        [SerializeField] private FacilityZoneId zone;
        [SerializeField] private Transform spawnPoint;

        public string SocketId => socketId;
        public FacilityZoneId Zone => zone;
        public Transform SpawnPoint => spawnPoint != null ? spawnPoint : transform;

        public void Configure(string id, FacilityZoneId facilityZone, Transform point = null)
        {
            socketId = id ?? string.Empty;
            zone = facilityZone;
            spawnPoint = point != null ? point : transform;
        }
    }
}
