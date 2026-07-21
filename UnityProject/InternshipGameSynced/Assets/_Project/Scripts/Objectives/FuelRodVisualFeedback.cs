using System;
using UnityEngine;

namespace InternshipGame.Prototype
{
    public sealed class FuelRodVisualFeedback : MonoBehaviour
    {
        [SerializeField] private FuelRodItem item;
        [SerializeField] private Renderer[] renderers = Array.Empty<Renderer>();
        [SerializeField] private Light responseLight;

        private MaterialPropertyBlock propertyBlock;

        public void Configure(FuelRodItem fuelRod, Renderer[] rodRenderers, Light light)
        {
            item = fuelRod;
            renderers = rodRenderers ?? Array.Empty<Renderer>();
            responseLight = light;
        }

        private void OnEnable()
        {
            if (item != null)
            {
                item.StateChanged += OnStateChanged;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (item != null)
            {
                item.StateChanged -= OnStateChanged;
            }
        }

        private void Update()
        {
            if (item != null && item.AssayState == FuelRodAssayState.Assaying)
            {
                Refresh();
            }
        }

        public void ResetVisual()
        {
            SetResponse(new Color(0.46f, 0.52f, 0.58f), 0.18f, false);
        }

        private void OnStateChanged(FuelRodItem _)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (item == null)
            {
                ResetVisual();
                return;
            }

            switch (item.AssayState)
            {
                case FuelRodAssayState.Assaying:
                    var pulse = 0.2f + item.AssayProgress * 0.55f + Mathf.Sin(Time.unscaledTime * 12f) * 0.08f;
                    SetResponse(new Color(0.78f, 0.82f, 0.86f), Mathf.Clamp01(pulse), true);
                    break;
                case FuelRodAssayState.ConfirmedLive:
                    SetResponse(new Color(1f, 0.42f, 0.06f), 1f, true);
                    break;
                case FuelRodAssayState.ConfirmedInert:
                    SetResponse(new Color(0.12f, 0.58f, 1f), 0.78f, true);
                    break;
                case FuelRodAssayState.Deposited:
                    SetResponse(new Color(0.18f, 1f, 0.48f), 0.9f, true);
                    break;
                default:
                    ResetVisual();
                    break;
            }
        }

        private void SetResponse(Color color, float intensity, bool lightEnabled)
        {
            propertyBlock ??= new MaterialPropertyBlock();
            foreach (var targetRenderer in renderers)
            {
                if (targetRenderer == null)
                {
                    continue;
                }

                targetRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor("_BaseColor", Color.Lerp(new Color(0.14f, 0.16f, 0.18f), color, 0.55f));
                propertyBlock.SetColor("_EmissionColor", color * Mathf.Lerp(0.1f, 2.4f, intensity));
                targetRenderer.SetPropertyBlock(propertyBlock);
            }

            if (responseLight != null)
            {
                responseLight.enabled = lightEnabled;
                responseLight.color = color;
                responseLight.intensity = Mathf.Lerp(0.2f, 2.4f, intensity);
            }
        }
    }
}
