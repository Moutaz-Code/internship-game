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
            var clip = GetOrCreate(cue, () => CreateFootstep(band));
            var volume = band switch
            {
                MovementNoiseBand.Crouch => 0.18f,
                MovementNoiseBand.Sprint => 0.72f,
                _ => 0.38f,
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

        private static AudioClip CreateFootstep(MovementNoiseBand band)
        {
            var duration = band == MovementNoiseBand.Sprint ? 0.15f : band == MovementNoiseBand.Crouch ? 0.08f : 0.11f;
            var samples = new float[Mathf.CeilToInt(SampleRate * duration)];
            var baseFrequency = band == MovementNoiseBand.Sprint ? 92f : band == MovementNoiseBand.Crouch ? 145f : 118f;
            for (var i = 0; i < samples.Length; i++)
            {
                var t = i / (float)SampleRate;
                var normalized = i / (float)samples.Length;
                var envelope = Mathf.Pow(1f - normalized, 2.4f);
                var hash = (i * 1103515245 + 12345) & 0x7fffffff;
                var noise = hash / (float)int.MaxValue * 2f - 1f;
                var tone = Mathf.Sin(2f * Mathf.PI * baseFrequency * t);
                samples[i] = (tone * 0.68f + noise * 0.32f) * envelope * 0.32f;
            }

            return CreateClip($"{band} Footstep", samples);
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
