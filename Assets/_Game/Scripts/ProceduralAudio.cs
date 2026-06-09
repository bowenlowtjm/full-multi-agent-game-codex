using UnityEngine;

namespace Pully.Game
{
    /// <summary>
    /// Generates procedural audio clips at runtime for SFX.
    /// Used as fallback when no audio files are assigned.
    /// </summary>
    public static class ProceduralAudio
    {
        private const int SampleRate = 44100;

        /// <summary>
        /// Generate a short "pop" sound for hit feedback.
        /// </summary>
        public static AudioClip GenerateHitClip()
        {
            float duration = 0.1f;
            int samples = Mathf.RoundToInt(SampleRate * duration);
            float[] data = new float[samples];

            float frequency = 880f; // A5 note
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float envelope = 1f - (i / (float)samples); // Linear decay
                data[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * 0.5f;
            }

            AudioClip clip = AudioClip.Create("Hit", samples, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        /// <summary>
        /// Generate a lower "thud" sound for miss feedback.
        /// </summary>
        public static AudioClip GenerateMissClip()
        {
            float duration = 0.2f;
            int samples = Mathf.RoundToInt(SampleRate * duration);
            float[] data = new float[samples];

            float frequency = 220f; // A3 note
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float envelope = 1f - (i / (float)samples);
                // Add some noise for texture
                float noise = Random.Range(-0.3f, 0.3f);
                float tone = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope;
                data[i] = (tone * 0.7f + noise * 0.3f) * 0.5f;
            }

            AudioClip clip = AudioClip.Create("Miss", samples, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        /// <summary>
        /// Generate an ascending tone for combo feedback.
        /// </summary>
        public static AudioClip GenerateComboClip()
        {
            float duration = 0.15f;
            int samples = Mathf.RoundToInt(SampleRate * duration);
            float[] data = new float[samples];

            float startFreq = 440f;
            float endFreq = 880f;

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float progress = i / (float)samples;
                float envelope = 1f - progress;
                float frequency = Mathf.Lerp(startFreq, endFreq, progress);
                data[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * 0.4f;
            }

            AudioClip clip = AudioClip.Create("Combo", samples, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        /// <n        /// Generate a sad descending tone for game over.
        /// </summary>
        public static AudioClip GenerateGameOverClip()
        {
            float duration = 0.5f;
            int samples = Mathf.RoundToInt(SampleRate * duration);
            float[] data = new float[samples];

            float startFreq = 440f;
            float endFreq = 220f;

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float progress = i / (float)samples;
                float envelope = 1f - progress;
                float frequency = Mathf.Lerp(startFreq, endFreq, progress);
                data[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * 0.5f;
            }

            AudioClip clip = AudioClip.Create("GameOver", samples, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        /// <summary>
        /// Generate a short blip for UI clicks.
        /// </summary>
        public static AudioClip GenerateUIClickClip()
        {
            float duration = 0.05f;
            int samples = Mathf.RoundToInt(SampleRate * duration);
            float[] data = new float[samples];

            float frequency = 1760f; // A6 note
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float envelope = 1f - (i / (float)samples);
                data[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * 0.3f;
            }

            AudioClip clip = AudioClip.Create("UIClick", samples, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        /// <summary>
        /// Generate a celebratory chord for high score.
        /// </summary>
        public static AudioClip GenerateHighScoreClip()
        {
            float duration = 0.3f;
            int samples = Mathf.RoundToInt(SampleRate * duration);
            float[] data = new float[samples];

            float freq1 = 523.25f; // C5
            float freq2 = 659.25f; // E5
            float freq3 = 783.99f; // G5

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float envelope = 1f - (i / (float)samples);
                float wave1 = Mathf.Sin(2f * Mathf.PI * freq1 * t);
                float wave2 = Mathf.Sin(2f * Mathf.PI * freq2 * t);
                float wave3 = Mathf.Sin(2f * Mathf.PI * freq3 * t);
                data[i] = (wave1 + wave2 + wave3) / 3f * envelope * 0.4f;
            }

            AudioClip clip = AudioClip.Create("HighScore", samples, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        /// <summary>
        /// Generate a simple music loop.
        /// </summary>
        public static AudioClip GenerateMusicLoop()
        {
            float duration = 4f; // 4 second loop
            int samples = Mathf.RoundToInt(SampleRate * duration);
            float[] data = new float[samples];

            // Simple bassline pattern
            float[] notes = { 220f, 220f, 261.63f, 196f }; // A3, A3, C4, G3
            float noteDuration = duration / notes.Length;

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                int noteIndex = Mathf.FloorToInt(t / noteDuration);
                noteIndex = Mathf.Clamp(noteIndex, 0, notes.Length - 1);
                float frequency = notes[noteIndex];

                float wave = Mathf.Sin(2f * Mathf.PI * frequency * t);
                // Add octave for richness
                wave += Mathf.Sin(2f * Mathf.PI * frequency * 2f * t) * 0.5f;
                
                data[i] = wave * 0.15f; // Keep music quiet
            }

            AudioClip clip = AudioClip.Create("MusicLoop", samples, 1, SampleRate, true);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
