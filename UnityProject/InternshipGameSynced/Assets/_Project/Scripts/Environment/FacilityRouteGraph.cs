using System;
using System.Collections.Generic;

namespace InternshipGame.Prototype
{
    public static class PrototypeVariationIds
    {
        public const string OpenCircuit = "open-circuit";
        public const string CoolantService = "coolant-service";
        public const string FuelInspection = "fuel-inspection";

        // Kept until the generated v0.3 facility is replaced in the V040 scene slice.
        public const string MaintenanceObstruction = "maintenance-obstruction";
        public const string LaboratoryQuarantine = "laboratory-quarantine";
        public const string CentralPowerOutage = "central-power-outage";

        public const string LightingStable = "lighting-stable";
        public const string LightingEmergency = "lighting-emergency";
        public const string LightingPartialOutage = "lighting-partial-outage";
    }

    public enum FacilityRouteNodeId
    {
        P01Containment,
        ContainmentNorth,
        NorthPipeGallery,
        CoolantNorth,
        CoolantSouth,
        UtilitySpine,
        FuelNorth,
        FuelSouth,
        SouthMaintenanceChannel,
        ContainmentSouth,
        CoolantServiceBranch,
        FuelInspectionBranch,
        C1,
        C2,
        C3,
        K1,
        K2,
        K3,
        F1,
        F2,
    }

    public readonly struct FacilityRouteEdge
    {
        public FacilityRouteEdge(
            FacilityRouteNodeId from,
            FacilityRouteNodeId to,
            string blockedByProfileId = "")
        {
            From = from;
            To = to;
            BlockedByProfileId = blockedByProfileId ?? string.Empty;
        }

        public FacilityRouteNodeId From { get; }
        public FacilityRouteNodeId To { get; }
        public string BlockedByProfileId { get; }
    }

    /// <summary>
    /// Testable topology for the authored 7C circuit. Socket nodes are pass-through
    /// pockets with two lobe connections; the Annex spur is deliberately outside this
    /// graph because P01's Containment side is the route-comparison origin.
    /// </summary>
    public static class FacilityRouteGraph
    {
        private static readonly FacilityRouteEdge[] Edges =
        {
            // Macro circuit around the inaccessible shielding/service void.
            new(FacilityRouteNodeId.P01Containment, FacilityRouteNodeId.ContainmentNorth),
            new(FacilityRouteNodeId.ContainmentNorth, FacilityRouteNodeId.NorthPipeGallery),
            new(FacilityRouteNodeId.NorthPipeGallery, FacilityRouteNodeId.CoolantNorth),
            new(FacilityRouteNodeId.CoolantNorth, FacilityRouteNodeId.CoolantSouth),
            new(FacilityRouteNodeId.CoolantSouth, FacilityRouteNodeId.UtilitySpine),
            new(FacilityRouteNodeId.UtilitySpine, FacilityRouteNodeId.FuelNorth),
            new(FacilityRouteNodeId.FuelNorth, FacilityRouteNodeId.FuelSouth),
            new(FacilityRouteNodeId.FuelSouth, FacilityRouteNodeId.SouthMaintenanceChannel),
            new(FacilityRouteNodeId.SouthMaintenanceChannel, FacilityRouteNodeId.ContainmentSouth),
            new(FacilityRouteNodeId.ContainmentSouth, FacilityRouteNodeId.P01Containment),

            // Local secondary branches. Profiles close one local edge, never a connector.
            new(FacilityRouteNodeId.CoolantNorth, FacilityRouteNodeId.CoolantServiceBranch,
                PrototypeVariationIds.CoolantService),
            new(FacilityRouteNodeId.CoolantServiceBranch, FacilityRouteNodeId.CoolantSouth),
            new(FacilityRouteNodeId.FuelNorth, FacilityRouteNodeId.FuelInspectionBranch,
                PrototypeVariationIds.FuelInspection),
            new(FacilityRouteNodeId.FuelInspectionBranch, FacilityRouteNodeId.FuelSouth),

            // Pass-through objective pockets: each socket has two distinct lobe edges.
            new(FacilityRouteNodeId.C1, FacilityRouteNodeId.P01Containment),
            new(FacilityRouteNodeId.C1, FacilityRouteNodeId.ContainmentNorth),
            new(FacilityRouteNodeId.C2, FacilityRouteNodeId.ContainmentNorth),
            new(FacilityRouteNodeId.C2, FacilityRouteNodeId.ContainmentSouth),
            new(FacilityRouteNodeId.C3, FacilityRouteNodeId.ContainmentSouth),
            new(FacilityRouteNodeId.C3, FacilityRouteNodeId.P01Containment),

            new(FacilityRouteNodeId.K1, FacilityRouteNodeId.CoolantNorth),
            new(FacilityRouteNodeId.K1, FacilityRouteNodeId.CoolantSouth),
            new(FacilityRouteNodeId.K2, FacilityRouteNodeId.CoolantNorth),
            new(FacilityRouteNodeId.K2, FacilityRouteNodeId.CoolantSouth),
            new(FacilityRouteNodeId.K3, FacilityRouteNodeId.CoolantNorth),
            new(FacilityRouteNodeId.K3, FacilityRouteNodeId.CoolantSouth),

            new(FacilityRouteNodeId.F1, FacilityRouteNodeId.FuelNorth),
            new(FacilityRouteNodeId.F1, FacilityRouteNodeId.FuelSouth),
            new(FacilityRouteNodeId.F2, FacilityRouteNodeId.FuelNorth),
            new(FacilityRouteNodeId.F2, FacilityRouteNodeId.FuelSouth),
        };

        private static readonly FacilityRouteNodeId[] SocketNodes =
        {
            FacilityRouteNodeId.C1,
            FacilityRouteNodeId.C2,
            FacilityRouteNodeId.C3,
            FacilityRouteNodeId.K1,
            FacilityRouteNodeId.K2,
            FacilityRouteNodeId.K3,
            FacilityRouteNodeId.F1,
            FacilityRouteNodeId.F2,
        };

        public static IReadOnlyList<FacilityRouteEdge> AuthoredEdges => Edges;
        public static IReadOnlyList<FacilityRouteNodeId> AuthoredSocketNodes => SocketNodes;

        public static bool PreservesCriticalPaths(string routeProfileId)
        {
            foreach (var socket in SocketNodes)
            {
                if (!HasAtLeastTwoEdgeDisjointPaths(
                        FacilityRouteNodeId.P01Containment,
                        socket,
                        routeProfileId))
                {
                    return false;
                }
            }

            return true;
        }

        public static HashSet<FacilityRouteNodeId> GetReachableNodes(string routeProfileId)
        {
            var reachable = new HashSet<FacilityRouteNodeId> { FacilityRouteNodeId.P01Containment };
            var frontier = new Queue<FacilityRouteNodeId>();
            frontier.Enqueue(FacilityRouteNodeId.P01Containment);

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();
                for (var edgeIndex = 0; edgeIndex < Edges.Length; edgeIndex++)
                {
                    var edge = Edges[edgeIndex];
                    if (IsBlocked(edge, routeProfileId))
                    {
                        continue;
                    }

                    if (!TryGetNeighbour(edge, current, out var neighbour) || !reachable.Add(neighbour))
                    {
                        continue;
                    }

                    frontier.Enqueue(neighbour);
                }
            }

            return reachable;
        }

        public static bool HasAtLeastTwoEdgeDisjointPaths(
            FacilityRouteNodeId from,
            FacilityRouteNodeId to,
            string routeProfileId)
        {
            var paths = new List<HashSet<int>>();
            FindSimplePaths(
                from,
                to,
                routeProfileId ?? string.Empty,
                new HashSet<FacilityRouteNodeId> { from },
                new HashSet<int>(),
                paths);

            for (var first = 0; first < paths.Count; first++)
            {
                for (var second = first + 1; second < paths.Count; second++)
                {
                    if (!paths[first].Overlaps(paths[second]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static void FindSimplePaths(
            FacilityRouteNodeId current,
            FacilityRouteNodeId target,
            string routeProfileId,
            HashSet<FacilityRouteNodeId> visited,
            HashSet<int> usedEdges,
            List<HashSet<int>> results)
        {
            if (current == target)
            {
                results.Add(new HashSet<int>(usedEdges));
                return;
            }

            for (var edgeIndex = 0; edgeIndex < Edges.Length; edgeIndex++)
            {
                var edge = Edges[edgeIndex];
                if (IsBlocked(edge, routeProfileId) || !TryGetNeighbour(edge, current, out var neighbour) ||
                    !visited.Add(neighbour))
                {
                    continue;
                }

                usedEdges.Add(edgeIndex);
                FindSimplePaths(neighbour, target, routeProfileId, visited, usedEdges, results);
                usedEdges.Remove(edgeIndex);
                visited.Remove(neighbour);
            }
        }

        private static bool IsBlocked(FacilityRouteEdge edge, string routeProfileId)
        {
            return !string.IsNullOrEmpty(edge.BlockedByProfileId) &&
                   string.Equals(edge.BlockedByProfileId, routeProfileId, StringComparison.Ordinal);
        }

        private static bool TryGetNeighbour(
            FacilityRouteEdge edge,
            FacilityRouteNodeId current,
            out FacilityRouteNodeId neighbour)
        {
            if (edge.From == current)
            {
                neighbour = edge.To;
                return true;
            }

            if (edge.To == current)
            {
                neighbour = edge.From;
                return true;
            }

            neighbour = default;
            return false;
        }
    }
}
