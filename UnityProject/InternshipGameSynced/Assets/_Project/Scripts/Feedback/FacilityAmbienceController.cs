using UnityEngine;

namespace InternshipGame.Prototype
{
    /// <summary>
    /// Audible half of the seeded lighting/audio profile. It is intentionally a
    /// presentation adapter: run configuration stays immutable and gameplay logic
    /// never depends on the generated ambience.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public sealed class FacilityAmbienceController : MonoBehaviour, IPrototypeStatusProvider
    {
        [SerializeField] private PrototypeRunController runController;
        [SerializeField] private AudioSource audioSource;

        private readonly AudioClip[] clips = new AudioClip[3];

        public LightingAudioProfile ActiveProfile { get; private set; } = LightingAudioProfile.StableCool;
        public string StatusLabel => $"Ambience: {ActiveProfile}";

        public void Configure(PrototypeRunController controller, AudioSource source)
        {
            runController = controller;
            audioSource = source;
        }

        private void Awake()
        {
            audioSource ??= GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            if (runController != null)
            {
                runController.ConfigurationChanged += OnConfigurationChanged;
            }
        }

        private void Start()
        {
            Apply(runController?.CurrentConfiguration?.LightingProfile ?? LightingAudioProfile.StableCool);
        }

        private void OnDisable()
        {
            if (runController != null)
            {
                runController.ConfigurationChanged -= OnConfigurationChanged;
            }

            audioSource?.Stop();
        }

        private void OnConfigurationChanged(RunConfiguration configuration)
        {
            if (configuration != null)
            {
                Apply(configuration.LightingProfile);
            }
        }

        private void Apply(LightingAudioProfile profile)
        {
            ActiveProfile = profile;
            if (audioSource == null)
            {
                return;
            }

            var index = (int)profile;
            clips[index] ??= CreateLoop(profile);
            audioSource.Stop();
            audioSource.clip = clips[index];
            audioSource.loop = true;
            audioSource.spatialBlend = 0f;
            audioSource.volume = profile switch
            {
                LightingAudioProfile.StableCool => 0.09f,
                LightingAudioProfile.WestWingFlicker => 0.11f,
                _ => 0.13f,
            };
            audioSource.Play();
        }

        private static AudioClip CreateLoop(LightingAudioProfile profile)
        {
            const int sampleRate = 22050;
            const int durationSeconds = 4;
            var samples = new float[sampleRate * durationSeconds];
            for (var i = 0; i < samples.Length; i++)
            {
                var time = i / (float)sampleRate;
                var envelope = 1f;
                var frequency = 54f;
                if (profile == LightingAudioProfile.WestWingFlicker)
                {
                    var cycle = time % 1.35f;
                    envelope = cycle is > 0.82f and < 0.94f ? 0.08f : 0.78f;
                    frequency = 61f;
                }
                else if (profile == LightingAudioProfile.EmergencyPulse)
                {
                    envelope = 0.35f + 0.65f * Mathf.Pow(0.5f + 0.5f * Mathf.Sin(time * Mathf.PI), 4f);
                    frequency = 74f;
                }

                var fundamental = Mathf.Sin(2f * Mathf.PI * frequency * time);
                var harmonic = Mathf.Sin(2f * Mathf.PI * frequency * 2.01f * time) * 0.28f;
                samples[i] = (fundamental + harmonic) * envelope * 0.12f;
            }

            var clip = AudioClip.Create($"Facility Ambience - {profile}", samples.Length, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private void OnDestroy()
        {
            foreach (var clip in clips)
            {
                if (clip != null)
                {
                    Destroy(clip);
                }
            }
        }
    }
}
