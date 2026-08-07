using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace InternshipGame.Prototype
{
    public enum RouteStateProfile
    {
        OpenCircuit,
        CoolantService,
        FuelInspection,
    }

    public enum OwlBehaviorProfile
    {
        PatientSearch,
        PatrolPressure,
        NoiseSensitive,
    }

    public enum LightingAudioProfile
    {
        StableCool,
        WestWingFlicker,
        EmergencyPulse,
    }

    /// <summary>
    /// Immutable, deterministic authored-variation selection. Geometry remains authored;
    /// this model only chooses which prepared sockets and profiles are active.
    /// </summary>
    public sealed class RunConfiguration
    {
        public const string C1 = "C1";
        public const string C2 = "C2";
        public const string C3 = "C3";
        public const string K1 = "K1";
        public const string K2 = "K2";
        public const string K3 = "K3";
        public const string F1 = "F1";
        public const string F2 = "F2";

        // Temporary compile bridge for the disposable v0.3 generated builder. These
        // names are removed with that builder in the V040 scene slice.
        public const string WestStorageA = "west-storage-a";
        public const string WestStorageB = "west-storage-b";
        public const string CoreVaultA = "core-vault-a";
        public const string CoreVaultB = "core-vault-b";
        public const string EastReactorA = "east-reactor-a";
        public const string EastReactorB = "east-reactor-b";
        public const string Maintenance = "maintenance";
        public const string Laboratory = "laboratory";

        private static readonly string[][] LobeSockets =
        {
            new[] { C1, C2, C3 },
            new[] { K1, K2, K3 },
            new[] { F1, F2 },
        };

        private readonly HashSet<string> liveLookup;
        private readonly HashSet<string> inertLookup;

        private RunConfiguration(
            int seed,
            string[] liveSocketIds,
            string[] inertSocketIds,
            RouteStateProfile routeProfile,
            OwlBehaviorProfile owlProfile,
            LightingAudioProfile lightingProfile)
        {
            Seed = seed;
            LiveSocketIds = new ReadOnlyCollection<string>(liveSocketIds);
            InertSocketIds = new ReadOnlyCollection<string>(inertSocketIds);
            RouteProfile = routeProfile;
            OwlProfile = owlProfile;
            LightingProfile = lightingProfile;
            liveLookup = new HashSet<string>(liveSocketIds, StringComparer.Ordinal);
            inertLookup = new HashSet<string>(inertSocketIds, StringComparer.Ordinal);
        }

        public int Seed { get; }
        public IReadOnlyList<string> LiveSocketIds { get; }
        public IReadOnlyList<string> InertSocketIds { get; }
        public RouteStateProfile RouteProfile { get; }
        public OwlBehaviorProfile OwlProfile { get; }
        public LightingAudioProfile LightingProfile { get; }

        public bool IsLiveSocket(string socketId)
        {
            return socketId != null && liveLookup.Contains(socketId);
        }

        public bool IsInertSocket(string socketId)
        {
            return socketId != null && inertLookup.Contains(socketId);
        }

        public static RunConfiguration Generate(int seed)
        {
            var random = new DeterministicRandom(seed);
            var live = new string[3];
            var inertCandidates = new List<string>(5);

            for (var index = 0; index < LobeSockets.Length; index++)
            {
                var lobe = LobeSockets[index];
                var liveIndex = random.Next(lobe.Length);
                live[index] = lobe[liveIndex];
                for (var socketIndex = 0; socketIndex < lobe.Length; socketIndex++)
                {
                    if (socketIndex != liveIndex)
                    {
                        inertCandidates.Add(lobe[socketIndex]);
                    }
                }
            }

            var inert = new string[2];
            for (var index = 0; index < inert.Length; index++)
            {
                var selectedIndex = random.Next(inertCandidates.Count);
                inert[index] = inertCandidates[selectedIndex];
                inertCandidates.RemoveAt(selectedIndex);
            }

            return new RunConfiguration(
                seed,
                live,
                inert,
                (RouteStateProfile)random.Next(Enum.GetValues(typeof(RouteStateProfile)).Length),
                (OwlBehaviorProfile)random.Next(Enum.GetValues(typeof(OwlBehaviorProfile)).Length),
                (LightingAudioProfile)random.Next(Enum.GetValues(typeof(LightingAudioProfile)).Length));
        }

        private struct DeterministicRandom
        {
            private uint state;

            public DeterministicRandom(int seed)
            {
                state = unchecked((uint)seed);
                if (state == 0u)
                {
                    state = 0x9E3779B9u;
                }
            }

            public int Next(int exclusiveMaximum)
            {
                if (exclusiveMaximum <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(exclusiveMaximum));
                }

                state ^= state << 13;
                state ^= state >> 17;
                state ^= state << 5;
                return (int)(state % (uint)exclusiveMaximum);
            }
        }
    }
}
