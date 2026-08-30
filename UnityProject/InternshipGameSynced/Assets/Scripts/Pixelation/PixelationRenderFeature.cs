using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    /// <summary>
    /// Unity 6 / URP 17 compatibility pass for the original URP-PSX fullscreen effects.
    /// It binds the camera color through URP's XR-aware _BlitTexture contract and
    /// renders a procedural fullscreen triangle for Unity 6 RenderGraph.
    /// </summary>
    public abstract class PSXRenderGraphPass<TVolume> : ScriptableRenderPass
        where TVolume : VolumeComponent, IPostProcessComponent
    {
        private readonly string effectPassName;
        private readonly Material effectMaterial;

        protected PSXRenderGraphPass(
            RenderPassEvent passEvent,
            string shaderPath,
            string passName,
            bool requiresDepth = false)
        {
            renderPassEvent = passEvent;
            effectPassName = passName;

            // Reading camera color requires URP to provide an intermediate texture rather
            // than the back buffer. RenderGraph then owns the temporary destination.
            requiresIntermediateTexture = true;
            if (requiresDepth)
            {
                ConfigureInput(ScriptableRenderPassInput.Depth);
            }

            var shader = Shader.Find(shaderPath);
            if (shader == null)
            {
                Debug.LogError($"URP-PSX shader not found: {shaderPath}");
                return;
            }

            effectMaterial = CoreUtils.CreateEngineMaterial(shader);
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (effectMaterial == null)
            {
                return;
            }

            var cameraData = frameData.Get<UniversalCameraData>();
            if (!cameraData.postProcessEnabled)
            {
                return;
            }

            var settings = VolumeManager.instance.stack.GetComponent<TVolume>();
            if (settings == null || !settings.IsActive())
            {
                return;
            }

            var resourceData = frameData.Get<UniversalResourceData>();
            if (resourceData.isActiveTargetBackBuffer)
            {
                return;
            }

            var source = resourceData.activeColorTexture;
            if (!source.IsValid())
            {
                return;
            }

            UpdateMaterial(effectMaterial, settings);

            var destinationDescriptor = renderGraph.GetTextureDesc(source);
            destinationDescriptor.name = $"CameraColor-{effectPassName}";
            destinationDescriptor.clearBuffer = false;
            destinationDescriptor.filterMode = FilterMode.Point;
            var destination = renderGraph.CreateTexture(destinationDescriptor);

            // URP 17's default path binds _BlitTexture and draws an XR-aware
            // procedural full-screen triangle, matching the migrated shaders.
            var blitParameters = new RenderGraphUtils.BlitMaterialParameters(
                source,
                destination,
                effectMaterial,
                0);
            renderGraph.AddBlitPass(blitParameters, effectPassName);

            // Feed this result directly into the next feature/post-processing pass.
            resourceData.cameraColor = destination;
        }

        public void Dispose()
        {
            CoreUtils.Destroy(effectMaterial);
        }

        protected abstract void UpdateMaterial(Material material, TVolume settings);
    }

    public class PixelationRenderFeature : ScriptableRendererFeature
    {
        private PixelationPass pixelationPass;

        public override void Create()
        {
            pixelationPass = new PixelationPass(RenderPassEvent.BeforeRenderingPostProcessing);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (pixelationPass != null)
            {
                renderer.EnqueuePass(pixelationPass);
            }
        }

        protected override void Dispose(bool disposing)
        {
            pixelationPass?.Dispose();
        }
    }

    public sealed class PixelationPass : PSXRenderGraphPass<Pixelation>
    {
        private static readonly int WidthPixelation = Shader.PropertyToID("_WidthPixelation");
        private static readonly int HeightPixelation = Shader.PropertyToID("_HeightPixelation");
        private static readonly int ColorPrecision = Shader.PropertyToID("_ColorPrecision");

        public PixelationPass(RenderPassEvent passEvent)
            : base(passEvent, "PostEffect/Pixelation", "Render Pixelation Effects")
        {
        }

        protected override void UpdateMaterial(Material material, Pixelation settings)
        {
            material.SetFloat(WidthPixelation, Mathf.Max(1f, settings.widthPixelation.value));
            material.SetFloat(HeightPixelation, Mathf.Max(1f, settings.heightPixelation.value));
            material.SetFloat(ColorPrecision, Mathf.Max(1f, settings.colorPrecision.value));
        }
    }
}
