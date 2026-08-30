using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class CRTRenderFeature : ScriptableRendererFeature
    {
        private CRTPass crtPass;

        public override void Create()
        {
            crtPass = new CRTPass(RenderPassEvent.BeforeRenderingPostProcessing);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (crtPass != null)
            {
                renderer.EnqueuePass(crtPass);
            }
        }

        protected override void Dispose(bool disposing)
        {
            crtPass?.Dispose();
        }
    }

    public sealed class CRTPass : PSXRenderGraphPass<Crt>
    {
        private static readonly int ScanLinesWeight = Shader.PropertyToID("_ScanlinesWeight");
        private static readonly int NoiseWeight = Shader.PropertyToID("_NoiseWeight");
        private static readonly int ScreenBendX = Shader.PropertyToID("_ScreenBendX");
        private static readonly int ScreenBendY = Shader.PropertyToID("_ScreenBendY");
        private static readonly int VignetteAmount = Shader.PropertyToID("_VignetteAmount");
        private static readonly int VignetteSize = Shader.PropertyToID("_VignetteSize");
        private static readonly int VignetteRounding = Shader.PropertyToID("_VignetteRounding");
        private static readonly int VignetteSmoothing = Shader.PropertyToID("_VignetteSmoothing");
        private static readonly int ScanLinesDensity = Shader.PropertyToID("_ScanLinesDensity");
        private static readonly int ScanLinesSpeed = Shader.PropertyToID("_ScanLinesSpeed");
        private static readonly int NoiseAmount = Shader.PropertyToID("_NoiseAmount");
        private static readonly int ChromaticRed = Shader.PropertyToID("_ChromaticRed");
        private static readonly int ChromaticGreen = Shader.PropertyToID("_ChromaticGreen");
        private static readonly int ChromaticBlue = Shader.PropertyToID("_ChromaticBlue");
        private static readonly int GrilleOpacity = Shader.PropertyToID("_GrilleOpacity");
        private static readonly int GrilleCounterOpacity = Shader.PropertyToID("_GrilleCounterOpacity");
        private static readonly int GrilleResolution = Shader.PropertyToID("_GrilleResolution");
        private static readonly int GrilleCounterResolution = Shader.PropertyToID("_GrilleCounterResolution");
        private static readonly int GrilleBrightness = Shader.PropertyToID("_GrilleBrightness");
        private static readonly int GrilleUvRotation = Shader.PropertyToID("_GrilleUvRotation");
        private static readonly int GrilleUvMidPoint = Shader.PropertyToID("_GrilleUvMidPoint");
        private static readonly int GrilleShift = Shader.PropertyToID("_GrilleShift");

        public CRTPass(RenderPassEvent passEvent)
            : base(passEvent, "PostEffect/CRTShader", "Render CRT Effects", requiresDepth: true)
        {
        }

        protected override void UpdateMaterial(Material material, Crt settings)
        {
            material.SetFloat(ScanLinesWeight, settings.scanlinesWeight.value);
            material.SetFloat(NoiseWeight, settings.noiseWeight.value);
            material.SetFloat(ScreenBendX, Mathf.Max(0.0001f, settings.screenBendX.value));
            material.SetFloat(ScreenBendY, Mathf.Max(0.0001f, settings.screenBendY.value));
            material.SetFloat(VignetteAmount, settings.vignetteAmount.value);
            material.SetFloat(VignetteSize, settings.vignetteSize.value);
            material.SetFloat(VignetteRounding, settings.vignetteRounding.value);
            material.SetFloat(VignetteSmoothing, settings.vignetteSmoothing.value);
            material.SetFloat(ScanLinesDensity, settings.scanlinesDensity.value);
            material.SetFloat(ScanLinesSpeed, settings.scanlinesSpeed.value);
            material.SetFloat(NoiseAmount, settings.noiseAmount.value);
            material.SetVector(ChromaticRed, settings.chromaticRed.value);
            material.SetVector(ChromaticGreen, settings.chromaticGreen.value);
            material.SetVector(ChromaticBlue, settings.chromaticBlue.value);
            material.SetFloat(GrilleOpacity, settings.grilleOpacity.value);
            material.SetFloat(GrilleCounterOpacity, settings.grilleCounterOpacity.value);
            material.SetFloat(GrilleResolution, settings.grilleResolution.value);
            material.SetFloat(GrilleCounterResolution, settings.grilleCounterResolution.value);
            material.SetFloat(GrilleBrightness, settings.grilleBrightness.value);
            material.SetFloat(GrilleUvRotation, settings.grilleUvRotation.value);
            material.SetFloat(GrilleUvMidPoint, settings.grilleUvMidPoint.value);
            material.SetVector(GrilleShift, settings.grilleShift.value);
        }
    }
}
