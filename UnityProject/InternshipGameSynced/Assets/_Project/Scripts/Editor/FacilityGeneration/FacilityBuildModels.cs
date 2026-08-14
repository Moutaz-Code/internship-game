using UnityEngine;

namespace InternshipGame.Prototype.Editor
{
    internal sealed class FacilityBuildMaterials
    {
        public Material BaseFloor;
        public Material Wall;
        public Material Metal;
        public Material DarkTrim;
        public Material Panel;
        public Material Cyan;
        public Material Amber;
        public Material RadiationGreen;
        public Material DangerRed;
        public Material NeutralLight;
    }

    internal sealed class FacilityBuildResult
    {
        public GameObject Root;
        public PrototypeMapDefinition MapDefinition;
        public Transform MissionDisplayAnchor;
    }
}
