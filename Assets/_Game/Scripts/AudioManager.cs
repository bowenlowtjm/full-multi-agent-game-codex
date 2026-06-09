using UnityEngine;

namespace Pully.Game
{
    /// <summary>
    /// Central audio controller for music and SFX.
    /// Supports volume control, mute, and persistence via SettingsManager.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource secondarySfxSource; // For overlapping sounds

        [Header("Music")]
        [SerializeField] private AudioClip musicLoop;

        [Header("SFX Clips")]
        [SerializeField] private AudioClip hitSfx;
        [SerializeField] private AudioClip missSfx;
        [SerializeField] private AudioClip comboSfx;
        [SerializeField] private AudioClip gameOverSfx;
        [SerializeField] private AudioClip uiClickSfx;
        [SerializeField] private AudioClip highScoreSfx;

        [Header("Volume Settings")]
        [Range(0f, 1f)] public float musicVolume = 0.7f;
        [Range(0f, 1f)] public float sfxVolume = 0.8f;
        public bool musicMuted = false;
        public bool sfxMuted = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            EnsureAudioSources();
            LoadSettings();
        }

        private void EnsureAudioSources()
        {
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }
            if (secondarySfxSource == null)
            {
                secondarySfxSource = gameObject.AddComponent<AudioSource>();
                secondarySfxSource.loop = false;
                secondarySfxSource.playOnAwake = false;
            }
        }

        private void Start()
        {
            GenerateProceduralClipsIfNeeded();
            PlayMusic();
        }

        private void GenerateProceduralClipsIfNeeded()
        {
            // Generate procedural clips if no audio files assigned
            if (hitSfx == null) hitSfx = ProceduralAudio.GenerateHitClip();
            if (missSfx == null) missSfx = ProceduralAudio.GenerateMissClip();
            if (comboSfx == null) comboSfx = ProceduralAudio.GenerateComboClip();
            if (gameOverSfx == null) gameOverSfx = ProceduralAudio.GenerateGameOverClip();
            if (uiClickSfx == null) uiClickSfx = ProceduralAudio.GenerateUIClickClip();
            if (highScoreSfx == null) highScoreSfx = ProceduralAudio.GenerateHighScoreClip();
            if (musicLoop == null) musicLoop = ProceduralAudio.GenerateMusicLoop();
        }

        private void LoadSettings()
        {
            musicVolume = PlayerPrefs.GetFloat("Pully_MusicVolume", 0.7f);
            sfxVolume = PlayerPrefs.GetFloat("Pully_SfxVolume", 0.8f);
            musicMuted = PlayerPrefs.GetInt("Pully_MusicMuted", 0) == 1;
            sfxMuted = PlayerPrefs.GetInt("Pully_SfxMuted", 0) == 1;
            ApplyVolumes();
        }

        public void SaveSettings()
        {
            PlayerPrefs.SetFloat("Pully_MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("Pully_SfxVolume", sfxVolume);
            PlayerPrefs.SetInt("Pully_MusicMuted", musicMuted ? 1 : 0);
            PlayerPrefs.SetInt("Pully_SfxMuted", sfxMuted ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void ApplyVolumes()
        {
            musicSource.volume = musicMuted ? 0f : musicVolume;
            sfxSource.volume = sfxMuted ? 0f : sfxVolume;
            secondarySfxSource.volume = sfxMuted ? 0f : sfxVolume;
        }

        public void ApplyPrefs()
        {
            LoadSettings();
            ApplyVolumes();
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            ApplyVolumes();
            SaveSettings();
        }

        public void SetSfxVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            ApplyVolumes();
            SaveSettings();
        }

        public void ToggleMusicMute()
        {
            musicMuted = !musicMuted;
            ApplyVolumes();
            SaveSettings();
        }

        public void ToggleSfxMute()
        {
            sfxMuted = !sfxMuted;
            ApplyVolumes();
            SaveSettings();
        }

        public void PlayMusic()
        {
            if (musicLoop == null) return;
            if (musicSource.isPlaying) return;
            
            musicSource.clip = musicLoop;
            musicSource.Play();
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }

        public void PauseMusic()
        {
            musicSource.Pause();
        }

        public void ResumeMusic()
        {
            musicSource.UnPause();
        }

        // SFX Playback methods
        public void PlayHit() => PlaySfx(hitSfx);
        public void PlayMiss() => PlaySfx(missSfx);
        public void PlayCombo() => PlaySfx(comboSfx);
        public void PlayGameOver() => PlaySfx(gameOverSfx);
        public void PlayUiClick() => PlaySfx(uiClickSfx);
        public void PlayHighScore() => PlaySfx(highScoreSfx);

        private void PlaySfx(AudioClip clip)
        {
            if (clip == null) return;
            if (sfxMuted) return;

            // Use secondary source if primary is busy
            if (sfxSource.isPlaying)
            {
                secondarySfxSource.PlayOneShot(clip, sfxVolume);
            }
            else
            {
                sfxSource.PlayOneShot(clip, sfxVolume);
            }
        }

        // Procedural fallback SFX (used when no clips assigned)
        public void GenerateProceduralHit()
        {
            if (sfxMuted) return;
            // Could generate simple beep here
            Debug.Log("[AudioManager] Hit SFX (procedural)");
        }

        public void GenerateProceduralMiss()
        {
            if (sfxMuted) return;
            Debug.Log("[AudioManager] Miss SFX (procedural)");
        }
    }
}
