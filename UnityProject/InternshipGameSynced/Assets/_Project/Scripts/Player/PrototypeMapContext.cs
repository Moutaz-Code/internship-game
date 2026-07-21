using UnityEngine;

namespace InternshipGame.Prototype
{
    public sealed class PrototypeMapContext : MonoBehaviour, IMapContext
    {
        [SerializeField] private Transform playerSpawn;

        public Vector3 PlayerSpawnPosition => playerSpawn != null ? playerSpawn.position : Vector3.zero;
        public Quaternion PlayerSpawnRotation => playerSpawn != null ? playerSpawn.rotation : Quaternion.identity;

        public void Configure(Transform spawn)
        {
            playerSpawn = spawn;
        }
    }
}
