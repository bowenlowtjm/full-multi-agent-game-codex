using UnityEngine;

namespace Pully.Game
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        private AudioSource _music;
        private AudioSource _sfx;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _music = gameObject.AddComponent<AudioSource>();
            _music.loop = true;
            _music.playOnAwake = false;

            _sfx = gameObject.AddComponent<AudioSource>();
            _sfx.loop = false;
            _sfx.playOnAwake = false;

            ApplyPrefs();
        }

        public void ApplyPrefs()
        {
            bool mute = PlayerPrefs.GetInt("pully.mute", 0) == 1;
            float music = PlayerPrefs.GetFloat("pully.music", 0.75f);
            float sfx = PlayerPrefs.GetFloat("pully.sfx", 0.85f);

            _music.mute = mute;
            _sfx.mute = mute;
            _music.volume = music;
            _sfx.volume = sfx;
        }

        public void PlayHit()
        {
            if (_sfx == null) return;
            _sfx.pitch = 1.0f;
            _sfx.Play();
        }

        public void PlayMiss()
        {
            if (_sfx == null) return;
            _sfx.pitch = 0.8f;
            _sfx.Play();
        }
    }
}
