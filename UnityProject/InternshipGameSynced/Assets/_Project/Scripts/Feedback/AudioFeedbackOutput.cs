using UnityEngine;

namespace InternshipGame.Prototype
{
    public enum GeneratedAudioProfile
    {
        ThreatPulse,
        WatcherStinger,
        OwlWarning,
        OwlInvestigation,
        RodLive,
        RodInert,
        AssayRequired,
        RodRejected,
        DepositAccepted,
        ExtractionCycle,
        ExtractionReady,
        GeigerClick,
    }

    [RequireComponent(typeof(AudioSource))]
    public sealed class AudioFeedbackOutput : MonoBehaviour, IFeedbackOutput
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private string acceptedCueId = PrototypeFeedbackCues.ThreatAmbient;
        [SerializeField] private GeneratedAudioProfile generatedProfile;

        private AudioClip generatedClip;
        private float baseVolume = 1f;

        public string DeviceId => $"audio:{name}:{acceptedCueId}";
        public FeedbackChannel Channels => FeedbackChannel.Audio;
        public bool IsAvailable => audioSource != null;

        public void Configure(AudioSource source, string cueId, GeneratedAudioProfile profile)
        {
            audioSource = source;
            acceptedCueId = cueId;
            generatedProfile = profile;
        }

        private void Awake()
        {
            audioSource ??= GetComponent<AudioSource>();
            baseVolume = audioSource != null ? audioSource.volume : 1f;
        }

        public bool CanHandle(in FeedbackSignal signal)
        {
            return string.IsNullOrEmpty(acceptedCueId) || signal.CueId == acceptedCueId;
        }

        public void Apply(in FeedbackSignal signal)
        {
            if (audioSource == null)
            {
                return;
            }

            if (signal.Command is FeedbackCommand.Stop or FeedbackCommand.Reset)
            {
                ResetDevice();
                return;
            }

            EnsureClip();
            audioSource.volume = baseVolume * Mathf.Lerp(0.25f, 1f, signal.Intensity);
            if (signal.Command == FeedbackCommand.Pulse)
            {
                audioSource.PlayOneShot(generatedClip);
                return;
            }

            audioSource.clip = generatedClip;
            audioSource.loop = true;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }

        public void ResetDevice()
        {
            if (audioSource != null)
            {
                audioSource.Stop();
                audioSource.volume = baseVolume;
            }
        }

        private void EnsureClip()
        {
            if (generatedClip != null)
            {
                return;
            }

            generatedClip = generatedProfile switch
            {
                GeneratedAudioProfile.WatcherStinger => CreateWatcherStinger(),
                GeneratedAudioProfile.OwlWarning => CreateOwlWarning(),
                GeneratedAudioProfile.OwlInvestigation => CreateOwlInvestigation(),
                GeneratedAudioProfile.RodLive => CreateToneSequence("Live Rod Confirmed", 0.72f, 310f, 620f, false),
                GeneratedAudioProfile.RodInert => CreateToneSequence("Inert Rod Confirmed", 0.62f, 440f, 190f, false),
                GeneratedAudioProfile.AssayRequired => CreateToneSequence("Assay Required", 0.38f, 245f, 245f, false),
                GeneratedAudioProfile.RodRejected => CreateToneSequence("Rod Rejected", 0.48f, 165f, 105f, true),
                GeneratedAudioProfile.DepositAccepted => CreateToneSequence("Rod Deposited", 0.55f, 290f, 510f, false),
                GeneratedAudioProfile.ExtractionCycle => CreateToneSequence("Extraction Cycle", 1.1f, 72f, 92f, true),
                GeneratedAudioProfile.ExtractionReady => CreateToneSequence("Extraction Ready", 0.85f, 360f, 760f, false),
                GeneratedAudioProfile.GeigerClick => CreateGeigerClick(),
                _ => CreateThreatPulse(),
            };
        }

        private static AudioClip CreateToneSequence(
            string clipName,
            float durationSeconds,
            float startFrequency,
            float endFrequency,
            bool rough)
        {
            const int sampleRate = 44100;
            var samples = new float[Mathf.CeilToInt(sampleRate * durationSeconds)];
            var phase = 0f;
            for (var i = 0; i < samples.Length; i++)
            {
                var normalizedTime = i / (float)samples.Length;
                var frequency = Mathf.Lerp(startFrequency, endFrequency, normalizedTime);
                phase += 2f * Mathf.PI * frequency / sampleRate;
                var envelope = Mathf.Sin(normalizedTime * Mathf.PI);
                var carrier = Mathf.Sin(phase);
                if (rough)
                {
                    carrier = Mathf.Sign(carrier) * 0.65f + carrier * 0.35f;
                }

                samples[i] = carrier * envelope * 0.16f;
            }

            var clip = AudioClip.Create(clipName, samples.Length, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private static AudioClip CreateGeigerClick()
        {
            const int sampleRate = 44100;
            const float durationSeconds = 0.025f;
            var samples = new float[Mathf.CeilToInt(sampleRate * durationSeconds)];
            for (var i = 0; i < samples.Length; i++)
            {
                var normalizedTime = i / (float)samples.Length;
                var hash = (i * 1103515245 + 12345) & 0x7fffffff;
                var noise = hash / (float)int.MaxValue * 2f - 1f;
                samples[i] = noise * (1f - normalizedTime) * 0.26f;
            }

            var clip = AudioClip.Create("Geiger Click", samples.Length, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private static AudioClip CreateThreatPulse()
        {
            const int sampleRate = 44100;
            const int durationSeconds = 2;
            var samples = new float[sampleRate * durationSeconds];
            for (var i = 0; i < samples.Length; i++)
            {
                var time = i / (float)sampleRate;
                var slowPulse = 0.35f + 0.65f * Mathf.Pow(0.5f + 0.5f * Mathf.Sin(time * Mathf.PI), 3f);
                samples[i] = Mathf.Sin(2f * Mathf.PI * 48f * time) * slowPulse * 0.16f;
            }

            var clip = AudioClip.Create("Prototype Threat Pulse", samples.Length, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private static AudioClip CreateWatcherStinger()
        {
            const int sampleRate = 44100;
            const float durationSeconds = 0.7f;
            var samples = new float[Mathf.CeilToInt(sampleRate * durationSeconds)];
            var phase = 0f;
            for (var i = 0; i < samples.Length; i++)
            {
                var normalizedTime = i / (float)samples.Length;
                var frequency = Mathf.Lerp(820f, 180f, normalizedTime);
                phase += 2f * Mathf.PI * frequency / sampleRate;
                var envelope = Mathf.Sin(normalizedTime * Mathf.PI) * (1f - normalizedTime * 0.35f);
                samples[i] = Mathf.Sin(phase) * envelope * 0.2f;
            }

            var clip = AudioClip.Create("Watcher Reveal", samples.Length, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private static AudioClip CreateOwlWarning()
        {
            const int sampleRate = 44100;
            const float durationSeconds = 1.15f;
            var samples = new float[Mathf.CeilToInt(sampleRate * durationSeconds)];
            var phaseA = 0f;
            var phaseB = 0f;
            for (var i = 0; i < samples.Length; i++)
            {
                var time = i / (float)sampleRate;
                var normalizedTime = i / (float)samples.Length;
                var frequencyA = Mathf.Lerp(680f, 190f, normalizedTime);
                var frequencyB = Mathf.Lerp(410f, 125f, normalizedTime);
                phaseA += 2f * Mathf.PI * frequencyA / sampleRate;
                phaseB += 2f * Mathf.PI * frequencyB / sampleRate;
                var envelope = Mathf.Sin(normalizedTime * Mathf.PI) * (1f - normalizedTime * 0.2f);
                var tremolo = 0.7f + 0.3f * Mathf.Sin(time * Mathf.PI * 17f);
                samples[i] = (Mathf.Sin(phaseA) * 0.13f + Mathf.Sin(phaseB) * 0.09f) * envelope * tremolo;
            }

            var clip = AudioClip.Create("Owl Hunt Warning", samples.Length, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private static AudioClip CreateOwlInvestigation()
        {
            const int sampleRate = 44100;
            const float durationSeconds = 0.42f;
            var samples = new float[Mathf.CeilToInt(sampleRate * durationSeconds)];
            for (var i = 0; i < samples.Length; i++)
            {
                var normalized = i / (float)samples.Length;
                var time = i / (float)sampleRate;
                var envelope = Mathf.Sin(normalized * Mathf.PI) * (1f - normalized * 0.45f);
                var tone = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(260f, 185f, normalized) * time);
                samples[i] = tone * envelope * 0.105f;
            }

            var clip = AudioClip.Create("Owl Investigation Cue", samples.Length, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private void OnDestroy()
        {
            if (generatedClip != null)
            {
                Destroy(generatedClip);
            }
        }
    }
}
