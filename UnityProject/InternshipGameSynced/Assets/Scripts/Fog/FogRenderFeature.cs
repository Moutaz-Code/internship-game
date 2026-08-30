using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class FogRenderFeature : ScriptableRendererFeature
    {
        private FogPass fogPass;

        public override void Create()
        {
            fogPass = new FogPass(RenderPassEvent.BeforeRenderingPostProcessing);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (fogPass != null)
            {
                renderer.EnqueuePass(fogPass);
            }
        }

        protected override void Dispose(bool disposing)
        {
            fogPass?.Dispose();
        }
    }

    public sealed class FogPass : PSXRenderGraphPass<Fog>
    {
        private static readonly int FogDensity = Shader.PropertyToID("_FogDensity");
        private static readonly int FogDistance = Shader.PropertyToID("_FogDistance");
        private static readonly int FogColor = Shader.PropertyToID("_FogColor");
        private static readonly int AmbientColor = Shader.PropertyToID("_AmbientColor");
        private static readonly int FogNear = Shader.PropertyToID("_FogNear");
        private static readonly int FogFar = Shader.PropertyToID("_FogFar");
        private static readonly int FogAltScale = Shader.PropertyToID("_FogAltScale");
        private static readonly int FogThinning = Shader.PropertyToID("_FogThinning");
        private static readonly int NoiseScale = Shader.PropertyToID("_NoiseScale");
        private static readonly int NoiseStrength = Shader.PropertyToID("_NoiseStrength");

        public FogPass(RenderPassEvent passEvent)
            : base(passEvent, "PostEffect/Fog", "Render Fog Effects", requiresDepth: true)
        {
        }

        protected override void UpdateMaterial(Material material, Fog settings)
        {
            material.SetFloat(FogDensity, settings.fogDensity.value);
            material.SetFloat(FogDistance, settings.fogDistance.value);
            material.SetColor(FogColor, settings.fogColor.value);
            material.SetColor(AmbientColor, settings.ambientColor.value);
            material.SetFloat(FogNear, settings.fogNear.value);
            material.SetFloat(FogFar, settings.fogFar.value);
            material.SetFloat(FogAltScale, settings.fogAltScale.value);
            material.SetFloat(FogThinning, settings.fogThinning.value);
            material.SetFloat(NoiseScale, Mathf.Max(0.0001f, settings.noiseScale.value));
            material.SetFloat(NoiseStrength, settings.noiseStrength.value);
        }
    }
}
