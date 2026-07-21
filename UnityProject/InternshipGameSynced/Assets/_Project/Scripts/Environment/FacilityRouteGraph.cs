using System;
using System.Collections.Generic;

namespace InternshipGame.Prototype
{
    public static class PrototypeVariationIds
    {
        public const string MaintenanceObstruction = "maintenance-obstruction";
        public const string LaboratoryQuarantine = "laboratory-quarantine";
        public const string CentralPowerOutage = "central-power-outage";

        public const string LightingStable = "lighting-stable";
        public const string LightingEmergency = "lighting-emergency";
        public const string LightingPartialOutage = "lighting-partial-outage";
    }

    public readonly struct FacilityRouteEdge
    {
        public FacilityRouteEdge(FacilityZoneId from, FacilityZoneId to, string blockedByProfileId = "")
        {
            From = from;
            To = to;
            BlockedByProfileId = blockedByProfileId ?? string.Empty;
        }

        public FacilityZoneId From { get; }
        public FacilityZoneId To { get; }
        public string BlockedByProfileId { get; }
    }

    public static class FacilityRouteGraph
    {
        private static readonly FacilityRouteEdge[] Edges =
        {
            new(FacilityZoneId.EntryExtraction, FacilityZoneId.Decontamination),
            new(FacilityZoneId.Decontamination, FacilityZoneId.OperationsHub),
            new(FacilityZoneId.OperationsHub, FacilityZoneId.WestMaintenance, PrototypeVariationIds.MaintenanceObstruction),
            new(FacilityZoneId.OperationsHub, FacilityZoneId.CentralTransfer, PrototypeVariationIds.CentralPowerOutage),
            new(FacilityZoneId.OperationsHub, FacilityZoneId.EastLaboratory, PrototypeVariationIds.LaboratoryQuarantine),
            new(FacilityZoneId.WestMaintenance, FacilityZoneId.CentralTransfer),
            new(FacilityZoneId.CentralTransfer, FacilityZoneId.EastLaboratory),
            new(FacilityZoneId.WestMaintenance, FacilityZoneId.NorthCrossHall),
            new(FacilityZoneId.CentralTransfer, FacilityZoneId.NorthCrossHall),
            new(FacilityZoneId.EastLaboratory, FacilityZoneId.NorthCrossHall),
            new(FacilityZoneId.WestMaintenance, FacilityZoneId.WestRodStorage),
            new(FacilityZoneId.CentralTransfer, FacilityZoneId.CoreVault),
            new(FacilityZoneId.EastLaboratory, FacilityZoneId.EastReactor),
            new(FacilityZoneId.NorthCrossHall, FacilityZoneId.WestRodStorage),
            new(FacilityZoneId.NorthCrossHall, FacilityZoneId.CoreVault),
            new(FacilityZoneId.NorthCrossHall, FacilityZoneId.EastReactor),
        };

        public static IReadOnlyList<FacilityRouteEdge> AuthoredEdges => Edges;

        public static bool PreservesCriticalPaths(string routeProfileId)
        {
            var reachable = GetReachableZones(routeProfileId);
            return reachable.Contains(FacilityZoneId.WestRodStorage) &&
                   reachable.Contains(FacilityZoneId.CoreVault) &&
                   reachable.Contains(FacilityZoneId.EastReactor) &&
                   reachable.Count == Enum.GetValues(typeof(FacilityZoneId)).Length;
        }

        public static HashSet<FacilityZoneId> GetReachableZones(string routeProfileId)
        {
            var reachable = new HashSet<FacilityZoneId> { FacilityZoneId.EntryExtraction };
            var frontier = new Queue<FacilityZoneId>();
            frontier.Enqueue(FacilityZoneId.EntryExtraction);

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();
                foreach (var edge in Edges)
                {
                    if (!string.IsNullOrEmpty(edge.BlockedByProfileId) &&
                        string.Equals(edge.BlockedByProfileId, routeProfileId, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    FacilityZoneId neighbour;
                    if (edge.From == current)
                    {
                        neighbour = edge.To;
                    }
                    else if (edge.To == current)
                    {
                        neighbour = edge.From;
                    }
                    else
                    {
                        continue;
                    }

                    if (reachable.Add(neighbour))
                    {
                        frontier.Enqueue(neighbour);
                    }
                }
            }

            return reachable;
        }

        public static bool HasAtLeastTwoEdgeDisjointPaths(
            FacilityZoneId from,
            FacilityZoneId to,
            string routeProfileId)
        {
            var paths = new List<HashSet<int>>();
            FindSimplePaths(
                from,
                to,
                routeProfileId ?? string.Empty,
                new HashSet<FacilityZoneId> { from },
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
            FacilityZoneId current,
            FacilityZoneId target,
            string routeProfileId,
            HashSet<FacilityZoneId> visited,
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
                if (!string.IsNullOrEmpty(edge.BlockedByProfileId) &&
                    string.Equals(edge.BlockedByProfileId, routeProfileId, StringComparison.Ordinal))
                {
                    continue;
                }

                FacilityZoneId neighbour;
                if (edge.From == current)
                {
                    neighbour = edge.To;
                }
                else if (edge.To == current)
                {
                    neighbour = edge.From;
                }
                else
                {
                    continue;
                }

                if (!visited.Add(neighbour))
                {
                    continue;
                }

                usedEdges.Add(edgeIndex);
                FindSimplePaths(neighbour, target, routeProfileId, visited, usedEdges, results);
                usedEdges.Remove(edgeIndex);
                visited.Remove(neighbour);
            }
        }
    }
}
