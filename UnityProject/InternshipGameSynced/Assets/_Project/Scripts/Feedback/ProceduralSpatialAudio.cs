using System.Collections.Generic;
using UnityEngine;

namespace InternshipGame.Prototype
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class ProceduralSpatialAudio : MonoBehaviour
    {
        private const int SampleRate = 44100;

        [SerializeField] private AudioSource audioSource;

        private readonly Dictionary<string, AudioClip> clips = new();
        private int footstepVariant;

        public int PlayCount { get; private set; }
        public string LastCue { get; private set; } = string.Empty;

        public void Configure(AudioSource source)
        {
            audioSource = source;
        }

        private void Awake()
        {
            audioSource ??= GetComponent<AudioSource>();
        }

        public void PlayFootstep(MovementNoiseBand band)
        {
            if (band == MovementNoiseBand.Still)
            {
                return;
            }

            var cue = band switch
            {
                MovementNoiseBand.Crouch => "footstep.crouch",
                MovementNoiseBand.Sprint => "footstep.sprint",
                _ => "footstep.walk",
            };
            var variant = footstepVariant++ & 1;
            var clip = GetOrCreate($"{cue}.{variant}", () => CreateFootstep(band, variant));
            var volume = band switch
            {
                MovementNoiseBand.Crouch => 0.10f,
                MovementNoiseBand.Sprint => 0.36f,
                _ => 0.22f,
            };
            PlayOneShot(cue, clip, volume);
        }

        public void PlayMetalImpact(float intensity)
        {
            const string cue = "metal.impact";
            PlayOneShot(cue, GetOrCreate(cue, CreateMetalImpact), Mathf.Lerp(0.35f, 0.9f, Mathf.Clamp01(intensity)));
        }

        public void PlayMachinery()
        {
            const string cue = "machinery.test";
            PlayOneShot(cue, GetOrCreate(cue, CreateMachineryAlarm), 0.72f);
        }

        public void Stop()
        {
            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }

        private void PlayOneShot(string cue, AudioClip clip, float volume)
        {
            if (audioSource == null || clip == null)
            {
                return;
            }

            LastCue = cue;
            PlayCount++;
            audioSource.PlayOneShot(clip, Mathf.Clamp01(volume));
        }

        private AudioClip GetOrCreate(string key, System.Func<AudioClip> factory)
        {
            if (!clips.TryGetValue(key, out var clip) || clip == null)
            {
                clip = factory();
                clips[key] = clip;
            }

            return clip;
        }

        private static AudioClip CreateFootstep(MovementNoiseBand band, int variant)
        {
            return CreateClip($"{band} Footstep {variant + 1}", CreateFootstepSamples(band, variant));
        }

        internal static float[] CreateFootstepSamples(MovementNoiseBand band, int variant = 0)
        {
            var duration = band == MovementNoiseBand.Sprint ? 0.19f : band == MovementNoiseBand.Crouch ? 0.13f : 0.17f;
            var samples = new float[Mathf.CeilToInt(SampleRate * duration)];
            var heelStartFrequency = band == MovementNoiseBand.Sprint ? 78f : band == MovementNoiseBand.Crouch ? 58f : 68f;
            heelStartFrequency += (variant & 1) == 0 ? -2.5f : 2.5f;
            var heelEndFrequency = heelStartFrequency * 0.64f;
            var toeDelay = band == MovementNoiseBand.Sprint ? 0.052f : band == MovementNoiseBand.Crouch ? 0.036f : 0.045f;
            var toeFrequency = heelStartFrequency * 1.42f;
            var noiseState = 0x9e3779b9u ^ ((uint)band * 0x85ebca6bu) ^ ((uint)(variant + 1) * 0xc2b2ae35u);
            var filteredNoise = 0f;

            for (var i = 0; i < samples.Length; i++)
            {
                var t = i / (float)SampleRate;
                var normalized = i / (float)samples.Length;
                var attack = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / 0.006f));
                var heelEnvelope = attack * Mathf.Exp(-t / (duration * 0.27f));
                var heelPhase = 2f * Mathf.PI *
                    (heelStartFrequency * t + 0.5f * (heelEndFrequency - heelStartFrequency) * t * t / duration);

                noiseState ^= noiseState << 13;
                noiseState ^= noiseState >> 17;
                noiseState ^= noiseState << 5;
                var rawNoise = noiseState / (float)uint.MaxValue * 2f - 1f;
                filteredNoise += (rawNoise - filteredNoise) * 0.055f;

                var heel = Mathf.Sin(heelPhase) * heelEnvelope * 0.30f;
                var texture = filteredNoise * heelEnvelope * 0.13f;
                var toeTime = t - toeDelay;
                var toe = 0f;
                if (toeTime > 0f)
                {
                    var toeAttack = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(toeTime / 0.004f));
                    var toeEnvelope = toeAttack * Mathf.Exp(-toeTime / (duration * 0.18f));
                    toe = (Mathf.Sin(2f * Mathf.PI * toeFrequency * toeTime) * 0.12f + filteredNoise * 0.045f) * toeEnvelope;
                }

                var tail = Mathf.SmoothStep(1f, 0f, Mathf.InverseLerp(0.72f, 1f, normalized));
                samples[i] = (heel + texture + toe) * tail;
            }

            return samples;
        }

        private static AudioClip CreateMetalImpact()
        {
            const float duration = 0.28f;
            var samples = new float[Mathf.CeilToInt(SampleRate * duration)];
            for (var i = 0; i < samples.Length; i++)
            {
                var t = i / (float)SampleRate;
                var normalized = i / (float)samples.Length;
                var envelope = Mathf.Exp(-normalized * 8f);
                var strike = Mathf.Sin(2f * Mathf.PI * 760f * t) + Mathf.Sin(2f * Mathf.PI * 1130f * t) * 0.55f;
                samples[i] = strike * envelope * 0.22f;
            }

            return CreateClip("Accepted Metal Impact", samples);
        }

        private static AudioClip CreateMachineryAlarm()
        {
            const float duration = 2.5f;
            var samples = new float[Mathf.CeilToInt(SampleRate * duration)];
            for (var i = 0; i < samples.Length; i++)
            {
                var t = i / (float)SampleRate;
                var normalized = i / (float)samples.Length;
                var envelope = Mathf.Min(1f, normalized * 12f) * Mathf.Min(1f, (1f - normalized) * 10f);
                var motor = Mathf.Sign(Mathf.Sin(2f * Mathf.PI * 74f * t)) * 0.08f;
                var alarm = Mathf.Sin(2f * Mathf.PI * (230f + 32f * Mathf.Sin(t * Mathf.PI * 2f)) * t) * 0.13f;
                samples[i] = (motor + alarm) * envelope;
            }

            return CreateClip("Maintenance Test Machinery", samples);
        }

        private static AudioClip CreateClip(string name, float[] samples)
        {
            var clip = AudioClip.Create(name, samples.Length, 1, SampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private void OnDestroy()
        {
            foreach (var clip in clips.Values)
            {
                if (clip != null)
                {
                    Destroy(clip);
                }
            }

            clips.Clear();
        }
    }
}
