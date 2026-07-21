using System;
using System.Collections.Generic;
using UnityEngine;

namespace InternshipGame.Prototype
{
    public enum FacilityZoneId
    {
        EntryExtraction,
        Decontamination,
        OperationsHub,
        WestMaintenance,
        CentralTransfer,
        EastLaboratory,
        NorthCrossHall,
        WestRodStorage,
        CoreVault,
        EastReactor,
    }

    public sealed class PrototypeMapDefinition : MonoBehaviour, IMapContext, IPrototypeStatusProvider
    {
        [SerializeField] private Transform playerSpawn;
        [SerializeField] private Transform extractionRackAnchor;
        [SerializeField] private Transform extractionExitAnchor;
        [SerializeField] private Transform owlSpawnAnchor;
        [SerializeField] private RodSocketAnchor[] rodSockets = Array.Empty<RodSocketAnchor>();
        [SerializeField] private Transform[] owlPatrolPoints = Array.Empty<Transform>();
        [SerializeField] private RouteVariationAnchor[] routeVariations = Array.Empty<RouteVariationAnchor>();
        [SerializeField] private LightingVariantAnchor[] lightingVariants = Array.Empty<LightingVariantAnchor>();
        [SerializeField] private FacilityZoneTrigger[] zoneTriggers = Array.Empty<FacilityZoneTrigger>();

        private readonly Dictionary<string, RodSocketAnchor> socketsById = new(StringComparer.Ordinal);

        public event Action<FacilityZoneId> ZoneEntered;

        public Vector3 PlayerSpawnPosition => playerSpawn != null ? playerSpawn.position : Vector3.zero;
        public Quaternion PlayerSpawnRotation => playerSpawn != null ? playerSpawn.rotation : Quaternion.identity;
        public Transform ExtractionRackAnchor => extractionRackAnchor;
        public Transform ExtractionExitAnchor => extractionExitAnchor;
        public Transform OwlSpawnAnchor => owlSpawnAnchor;
        public IReadOnlyList<RodSocketAnchor> RodSockets => rodSockets;
        public IReadOnlyList<Transform> OwlPatrolPoints => owlPatrolPoints;
        public string ActiveRouteProfileId { get; private set; } = string.Empty;
        public string ActiveLightingProfileId { get; private set; } = string.Empty;
        public FacilityZoneId LastEnteredZone { get; private set; } = FacilityZoneId.EntryExtraction;
        public string StatusLabel => $"Map: {rodSockets.Length} sockets | {ActiveRouteProfileId} | {ActiveLightingProfileId}";

        public void Configure(
            Transform spawn,
            Transform rackAnchor,
            Transform exitAnchor,
            Transform owlSpawn,
            RodSocketAnchor[] sockets,
            Transform[] patrolPoints,
            RouteVariationAnchor[] variations,
            LightingVariantAnchor[] lighting,
            FacilityZoneTrigger[] triggers)
        {
            playerSpawn = spawn;
            extractionRackAnchor = rackAnchor;
            extractionExitAnchor = exitAnchor;
            owlSpawnAnchor = owlSpawn;
            rodSockets = sockets ?? Array.Empty<RodSocketAnchor>();
            owlPatrolPoints = patrolPoints ?? Array.Empty<Transform>();
            routeVariations = variations ?? Array.Empty<RouteVariationAnchor>();
            lightingVariants = lighting ?? Array.Empty<LightingVariantAnchor>();
            zoneTriggers = triggers ?? Array.Empty<FacilityZoneTrigger>();
            RebuildSocketIndex();

            foreach (var trigger in zoneTriggers)
            {
                if (trigger != null)
                {
                    trigger.Configure(trigger.Zone, this);
                }
            }
        }

        private void Awake()
        {
            RebuildSocketIndex();
        }

        public bool TryGetRodSocket(string socketId, out RodSocketAnchor socket)
        {
            if (socketsById.Count != rodSockets.Length)
            {
                RebuildSocketIndex();
            }

            return socketsById.TryGetValue(socketId ?? string.Empty, out socket);
        }

        public void ApplyAuthoredVariation(string routeProfileId, string lightingProfileId)
        {
            ActiveRouteProfileId = routeProfileId ?? string.Empty;
            ActiveLightingProfileId = lightingProfileId ?? string.Empty;

            foreach (var variation in routeVariations)
            {
                variation?.Apply(ActiveRouteProfileId);
            }

            foreach (var lighting in lightingVariants)
            {
                lighting?.Apply(ActiveLightingProfileId);
            }
        }

        public void ResetDefinition()
        {
            ApplyAuthoredVariation(string.Empty, string.Empty);
            LastEnteredZone = FacilityZoneId.EntryExtraction;
        }

        internal void NotifyZoneEntered(FacilityZoneId zone)
        {
            if (LastEnteredZone == zone)
            {
                return;
            }

            LastEnteredZone = zone;
            ZoneEntered?.Invoke(zone);
        }

        private void RebuildSocketIndex()
        {
            socketsById.Clear();
            foreach (var socket in rodSockets)
            {
                if (socket == null || string.IsNullOrWhiteSpace(socket.SocketId))
                {
                    continue;
                }

                if (!socketsById.TryAdd(socket.SocketId, socket))
                {
                    Debug.LogError($"Duplicate rod socket id '{socket.SocketId}' in map definition.", socket);
                }
            }
        }
    }
}
