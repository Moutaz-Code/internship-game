using System;
using UnityEngine;

namespace InternshipGame.Prototype
{
    public sealed class FuelRodRunCoordinator : MonoBehaviour, IRunStateListener, IPrototypeStatusProvider
    {
        [SerializeField] private PrototypeRunController runController;
        [SerializeField] private FuelRodObjectiveSet objectiveSet;
        [SerializeField] private FuelRodItem[] rods = Array.Empty<FuelRodItem>();
        [SerializeField] private PrototypeMapDefinition mapDefinition;
        [SerializeField] private FeedbackHub feedbackHub;
        [SerializeField] private PursuitThreatController owl;

        private bool firstRodPickupSeen;

        public string StatusLabel
        {
            get
            {
                var configuration = runController != null ? runController.CurrentConfiguration : null;
                return configuration == null
                    ? "Run seed: pending"
                    : $"Seed {configuration.Seed} | {configuration.RouteProfile} | {configuration.OwlProfile} | {configuration.LightingProfile}";
            }
        }

        public void Configure(
            PrototypeRunController controller,
            FuelRodObjectiveSet set,
            FuelRodItem[] configuredRods,
            PrototypeMapDefinition definition,
            FeedbackHub hub,
            PursuitThreatController threat)
        {
            runController = controller;
            objectiveSet = set;
            rods = configuredRods ?? Array.Empty<FuelRodItem>();
            mapDefinition = definition;
            feedbackHub = hub;
            owl = threat;
        }

        private void OnEnable()
        {
            if (runController != null)
            {
                runController.ConfigurationChanged += OnConfigurationChanged;
            }

            if (objectiveSet != null)
            {
                objectiveSet.ItemGrabbed += OnItemGrabbed;
                objectiveSet.ProgressChanged += OnProgressChanged;
            }

            foreach (var rod in rods)
            {
                if (rod != null)
                {
                    rod.AssayCompleted += OnAssayCompleted;
                }
            }
        }

        private void Start()
        {
            ApplyConfiguration(runController != null ? runController.CurrentConfiguration : null);
        }

        private void OnDisable()
        {
            if (runController != null)
            {
                runController.ConfigurationChanged -= OnConfigurationChanged;
            }

            if (objectiveSet != null)
            {
                objectiveSet.ItemGrabbed -= OnItemGrabbed;
                objectiveSet.ProgressChanged -= OnProgressChanged;
            }

            foreach (var rod in rods)
            {
                if (rod != null)
                {
                    rod.AssayCompleted -= OnAssayCompleted;
                }
            }
        }

        public void OnRunReset()
        {
            firstRodPickupSeen = false;
            feedbackHub?.Send(new FeedbackSignal(
                PrototypeFeedbackCues.ExtractionCycling,
                FeedbackCommand.Stop,
                FeedbackChannel.Wind | FeedbackChannel.Visual | FeedbackChannel.Audio));
            foreach (var rod in rods)
            {
                rod?.GetComponent<FuelRodVisualFeedback>()?.ResetVisual();
            }
        }

        public void OnRunStateChanged(PrototypeRunState previous, PrototypeRunState current)
        {
            switch (current)
            {
                case PrototypeRunState.ExtractionCycling:
                    feedbackHub?.Send(new FeedbackSignal(
                        PrototypeFeedbackCues.ExtractionCycling,
                        FeedbackCommand.Set,
                        FeedbackChannel.Wind | FeedbackChannel.Visual | FeedbackChannel.Audio,
                        0.62f,
                        20f));
                    break;
                case PrototypeRunState.EscapeReady:
                    feedbackHub?.Send(new FeedbackSignal(
                        PrototypeFeedbackCues.ExtractionCycling,
                        FeedbackCommand.Stop,
                        FeedbackChannel.Wind | FeedbackChannel.Visual | FeedbackChannel.Audio));
                    feedbackHub?.Send(new FeedbackSignal(
                        PrototypeFeedbackCues.ExtractionReady,
                        FeedbackCommand.Pulse,
                        FeedbackChannel.Visual | FeedbackChannel.Audio,
                        0.9f,
                        1.2f));
                    break;
                case PrototypeRunState.Extracted:
                case PrototypeRunState.Failed:
                    feedbackHub?.Send(new FeedbackSignal(
                        PrototypeFeedbackCues.ExtractionCycling,
                        FeedbackCommand.Stop,
                        FeedbackChannel.Wind | FeedbackChannel.Visual | FeedbackChannel.Audio));
                    break;
            }
        }

        private void OnConfigurationChanged(RunConfiguration configuration)
        {
            ApplyConfiguration(configuration);
        }

        private void ApplyConfiguration(RunConfiguration configuration)
        {
            if (configuration == null || mapDefinition == null || rods.Length < 5)
            {
                return;
            }

            var routeProfileId = configuration.RouteProfile switch
            {
                RouteStateProfile.CoolantService => PrototypeVariationIds.CoolantService,
                RouteStateProfile.FuelInspection => PrototypeVariationIds.FuelInspection,
                _ => PrototypeVariationIds.OpenCircuit,
            };
            var lightingProfileId = configuration.LightingProfile switch
            {
                LightingAudioProfile.StableCool => PrototypeVariationIds.LightingStable,
                LightingAudioProfile.EmergencyPulse => PrototypeVariationIds.LightingEmergency,
                _ => PrototypeVariationIds.LightingPartialOutage,
            };
            mapDefinition.ApplyAuthoredVariation(routeProfileId, lightingProfileId);
            owl?.SetRouteProfile(routeProfileId);
            owl?.ApplyBehaviorProfile(configuration.OwlProfile);

            for (var i = 0; i < 3; i++)
            {
                PlaceRod(rods[i], configuration.LiveSocketIds[i], FuelRodClassification.Live);
            }

            for (var i = 0; i < 2; i++)
            {
                PlaceRod(rods[i + 3], configuration.InertSocketIds[i], FuelRodClassification.Inert);
            }
        }

        private void PlaceRod(FuelRodItem rod, string socketId, FuelRodClassification classification)
        {
            if (rod == null || !mapDefinition.TryGetRodSocket(socketId, out var socket))
            {
                Debug.LogError($"Run configuration referenced missing rod socket '{socketId}'.", this);
                return;
            }

            rod.ConfigureIdentity(rod.UniqueId, classification);
            rod.ConfigureSpawn(socket.SpawnPoint);
        }

        private void OnItemGrabbed(FuelRodItem item)
        {
            if (firstRodPickupSeen)
            {
                return;
            }

            firstRodPickupSeen = true;
            owl?.NotifyFirstRodPickup(item.transform.position);
        }

        private void OnProgressChanged(RunProgressSnapshot snapshot)
        {
            owl?.SetObjectiveProgress(snapshot.RequiredDeposits <= 0
                ? 0f
                : snapshot.Deposited / (float)snapshot.RequiredDeposits);
        }

        private void OnAssayCompleted(FuelRodItem item)
        {
            if (item == null)
            {
                return;
            }

            if (item.Classification == FuelRodClassification.Live)
            {
                feedbackHub?.Send(new FeedbackSignal(
                    PrototypeFeedbackCues.RodLiveConfirmed,
                    FeedbackCommand.Pulse,
                    FeedbackChannel.Heat | FeedbackChannel.Visual | FeedbackChannel.Audio,
                    0.42f,
                    1.2f));
            }
            else
            {
                feedbackHub?.Send(new FeedbackSignal(
                    PrototypeFeedbackCues.RodInertConfirmed,
                    FeedbackCommand.Pulse,
                    FeedbackChannel.Visual | FeedbackChannel.Audio,
                    0.72f,
                    0.9f));
            }
        }
    }
}
