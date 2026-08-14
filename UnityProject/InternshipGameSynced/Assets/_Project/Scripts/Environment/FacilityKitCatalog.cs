using System;
using System.Collections.Generic;

namespace InternshipGame.Prototype
{
    [Serializable]
    public readonly struct FacilityKitCatalogEntry
    {
        public FacilityKitCatalogEntry(string stableId, FacilityKitCategory category, string prefabPath)
        {
            StableId = stableId;
            Category = category;
            PrefabPath = prefabPath;
        }

        public string StableId { get; }
        public FacilityKitCategory Category { get; }
        public string PrefabPath { get; }
    }

    public static class FacilityKitCatalog
    {
        public const string RootFolder = "Assets/_Project/Prefabs/Environment/FacilityKit";
        public const string MaterialFolder = "Assets/_Project/Materials/FacilityKit";
        public const string PaletteScenePath = "Assets/_Project/Scenes/FacilityKit_Palette.unity";

        public static readonly FacilityKitCatalogEntry[] Entries =
        {
            Entry("FK-WALL-1M", FacilityKitCategory.Architecture, "Architecture/PF_FK_Wall_Straight_1m.prefab"),
            Entry("FK-WALL-2M", FacilityKitCategory.Architecture, "Architecture/PF_FK_Wall_Straight_2m.prefab"),
            Entry("FK-WALL-4M", FacilityKitCategory.Architecture, "Architecture/PF_FK_Wall_Straight_4m.prefab"),
            Entry("FK-CORNER-INSIDE", FacilityKitCategory.Architecture, "Architecture/PF_FK_Wall_Corner_Inside.prefab"),
            Entry("FK-CORNER-OUTSIDE", FacilityKitCategory.Architecture, "Architecture/PF_FK_Wall_Corner_Outside.prefab"),
            Entry("FK-WALL-ANGLED-45", FacilityKitCategory.Architecture, "Architecture/PF_FK_Wall_Angled_45.prefab"),
            Entry("FK-FLOOR-2X2", FacilityKitCategory.Architecture, "Architecture/PF_FK_Floor_2x2.prefab"),
            Entry("FK-CEILING-2X2", FacilityKitCategory.Architecture, "Architecture/PF_FK_Ceiling_2x2.prefab"),
            Entry("FK-END-CAP", FacilityKitCategory.Architecture, "Architecture/PF_FK_Wall_EndCap.prefab"),
            Entry("FK-COLUMN", FacilityKitCategory.Architecture, "Architecture/PF_FK_Structural_Column.prefab"),
            Entry("FK-WALL-TRIM", FacilityKitCategory.Architecture, "Architecture/PF_FK_Wall_Trim_2m.prefab"),
            Entry("FK-PORTAL-FRAME", FacilityKitCategory.Architecture, "Architecture/PF_FK_PortalFrame_2p76x2p52.prefab"),
            Entry("FK-REACTOR-SEGMENT", FacilityKitCategory.Architecture, "Architecture/PF_FK_ReactorWall_12deg.prefab"),
            Entry("FK-BLOCKED-MASS", FacilityKitCategory.Architecture, "Architecture/PF_FK_BlockedMass_2x2.prefab"),

            Entry("FK-WINDOW-OBSERVATION", FacilityKitCategory.DoorsAndWindows, "DoorsAndWindows/PF_FK_Window_Observation_Reinforced.prefab"),
            Entry("FK-WINDOW-SERVICE", FacilityKitCategory.DoorsAndWindows, "DoorsAndWindows/PF_FK_Window_Service_Narrow.prefab"),
            Entry("FK-WINDOW-REACTOR-LEADGLASS", FacilityKitCategory.DoorsAndWindows, "DoorsAndWindows/PF_FK_Window_Reactor_LeadGlass.prefab"),
            Entry("FK-DOOR-SLIDING", FacilityKitCategory.DoorsAndWindows, "DoorsAndWindows/PF_FK_Door_Bulkhead_Sliding.prefab"),
            Entry("FK-DOOR-HINGED", FacilityKitCategory.DoorsAndWindows, "DoorsAndWindows/PF_FK_Door_Service_Hinged.prefab"),
            Entry("FK-DOOR-LOCKED", FacilityKitCategory.DoorsAndWindows, "DoorsAndWindows/PF_FK_Door_Locked_Inactive.prefab"),
            Entry("FK-ONEWAY-BULKHEAD", FacilityKitCategory.DoorsAndWindows, "DoorsAndWindows/PF_FK_OneWay_PressureBulkhead.prefab"),
            Entry("FK-ONEWAY-CRASHBAR", FacilityKitCategory.DoorsAndWindows, "DoorsAndWindows/PF_FK_OneWay_CrashBar.prefab"),
            Entry("FK-ONEWAY-VALVE", FacilityKitCategory.DoorsAndWindows, "DoorsAndWindows/PF_FK_OneWay_MaintenanceValve.prefab"),

            Entry("FK-SHIELD-STRAIGHT", FacilityKitCategory.Cover, "Cover/PF_FK_Shield_Straight.prefab"),
            Entry("FK-SHIELD-CORNER", FacilityKitCategory.Cover, "Cover/PF_FK_Shield_Corner.prefab"),

            Entry("FK-CONTROL-CONSOLE-STRAIGHT", FacilityKitCategory.Machinery, "Machinery/PF_FK_ControlConsole_Straight.prefab"),
            Entry("FK-CONTROL-CONSOLE-CORNER", FacilityKitCategory.Machinery, "Machinery/PF_FK_ControlConsole_Corner.prefab"),
            Entry("FK-ASSAY-BENCH", FacilityKitCategory.Machinery, "Machinery/PF_FK_Assay_Bench.prefab"),
            Entry("FK-ISOLATION-CABINET", FacilityKitCategory.Machinery, "Machinery/PF_FK_Assay_IsolationCabinet.prefab"),
            Entry("FK-COOLANT-BASIN", FacilityKitCategory.Machinery, "Machinery/PF_FK_Coolant_Basin.prefab"),
            Entry("FK-PUMP-A", FacilityKitCategory.Machinery, "Machinery/PF_FK_Pump_A_Horizontal.prefab"),
            Entry("FK-PUMP-B", FacilityKitCategory.Machinery, "Machinery/PF_FK_Pump_B_Twin.prefab"),
            Entry("FK-PUMP-C", FacilityKitCategory.Machinery, "Machinery/PF_FK_Pump_C_Vertical.prefab"),
            Entry("FK-PIPE-A", FacilityKitCategory.Machinery, "Machinery/PF_FK_PipeCluster_A.prefab"),
            Entry("FK-PIPE-B", FacilityKitCategory.Machinery, "Machinery/PF_FK_PipeCluster_B.prefab"),
            Entry("FK-PIPE-C", FacilityKitCategory.Machinery, "Machinery/PF_FK_PipeCluster_C.prefab"),
            Entry("FK-SCRUBBER-VESSEL", FacilityKitCategory.Machinery, "Machinery/PF_FK_Scrubber_Vessel.prefab"),
            Entry("FK-DUCT-BANK", FacilityKitCategory.Machinery, "Machinery/PF_FK_Duct_Bank.prefab"),
            Entry("FK-CASK-INDIVIDUAL", FacilityKitCategory.Machinery, "Machinery/PF_FK_Cask_Individual.prefab"),
            Entry("FK-CASK-ROW", FacilityKitCategory.Machinery, "Machinery/PF_FK_Cask_Row.prefab"),
            Entry("FK-CASK-END", FacilityKitCategory.Machinery, "Machinery/PF_FK_Cask_Row_EndCap.prefab"),
            Entry("FK-OVERHEAD-CRANE", FacilityKitCategory.Machinery, "Machinery/PF_FK_Overhead_Crane.prefab"),
            Entry("FK-LOCKERS", FacilityKitCategory.Machinery, "Machinery/PF_FK_Lockers.prefab"),
            Entry("FK-TOOL-CAGE", FacilityKitCategory.Machinery, "Machinery/PF_FK_Tool_Cage.prefab"),
            Entry("FK-WORKBENCH", FacilityKitCategory.Machinery, "Machinery/PF_FK_Workbench.prefab"),
            Entry("FK-PARTS-CART", FacilityKitCategory.Machinery, "Machinery/PF_FK_Parts_Cart.prefab"),
            Entry("FK-PORTABLE-STORAGE", FacilityKitCategory.Machinery, "Machinery/PF_FK_PortableMetal_Storage.prefab"),

            Entry("FK-DECON-FRAME", FacilityKitCategory.RoomIdentity, "RoomIdentity/PF_FK_Decon_Frame.prefab"),
            Entry("FK-INTAKE-STATION", FacilityKitCategory.RoomIdentity, "RoomIdentity/PF_FK_Contractor_Intake.prefab"),

            Entry("FK-EXTRACTION-VEHICLE", FacilityKitCategory.Extraction, "Extraction/PF_FK_Extraction_VehicleAirlock.prefab"),
            Entry("FK-EXTRACTION-LIFT", FacilityKitCategory.Extraction, "Extraction/PF_FK_Extraction_MaintenanceLift.prefab"),
            Entry("FK-EXTRACTION-EGRESS", FacilityKitCategory.Extraction, "Extraction/PF_FK_Extraction_EmergencyEgress.prefab"),
        };

        private static readonly Dictionary<string, FacilityKitCatalogEntry> ById = BuildLookup();

        public static bool TryGet(string stableId, out FacilityKitCatalogEntry entry)
        {
            return ById.TryGetValue(stableId ?? string.Empty, out entry);
        }

        private static FacilityKitCatalogEntry Entry(string id, FacilityKitCategory category, string relativePath)
        {
            return new FacilityKitCatalogEntry(id, category, $"{RootFolder}/{relativePath}");
        }

        private static Dictionary<string, FacilityKitCatalogEntry> BuildLookup()
        {
            var result = new Dictionary<string, FacilityKitCatalogEntry>(StringComparer.Ordinal);
            foreach (var entry in Entries)
            {
                result.Add(entry.StableId, entry);
            }

            return result;
        }
    }
}
