using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class DitheringRenderFeature : ScriptableRendererFeature
    {
        private DitheringPass ditheringPass;

        public override void Create()
        {
            ditheringPass = new DitheringPass(RenderPassEvent.BeforeRenderingPostProcessing);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (ditheringPass != null)
            {
                renderer.EnqueuePass(ditheringPass);
            }
        }

        protected override void Dispose(bool disposing)
        {
            ditheringPass?.Dispose();
        }
    }

    public sealed class DitheringPass : PSXRenderGraphPass<Dithering>
    {
        private static readonly int PatternIndex = Shader.PropertyToID("_PatternIndex");
        private static readonly int DitherThreshold = Shader.PropertyToID("_DitherThreshold");
        private static readonly int DitherStrength = Shader.PropertyToID("_DitherStrength");
        private static readonly int DitherScale = Shader.PropertyToID("_DitherScale");

        public DitheringPass(RenderPassEvent passEvent)
            : base(passEvent, "PostEffect/Dithering", "Render Dithering Effects")
        {
        }

        protected override void UpdateMaterial(Material material, Dithering settings)
        {
            material.SetInt(PatternIndex, settings.patternIndex.value);
            material.SetFloat(DitherThreshold, settings.ditherThreshold.value);
            material.SetFloat(DitherStrength, settings.ditherStrength.value);
            material.SetFloat(DitherScale, Mathf.Max(0.0001f, settings.ditherScale.value));
        }
    }
}
