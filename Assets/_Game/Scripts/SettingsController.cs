using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pully.Game
{
    public class SettingsController : MonoBehaviour
    {
        private float _music = 0.75f;
        private float _sfx = 0.85f;
        private bool _mute;
        private bool _haptics = true;
        private bool _colorblind;

        private void Start()
        {
            _music = PlayerPrefs.GetFloat("pully.music", 0.75f);
            _sfx = PlayerPrefs.GetFloat("pully.sfx", 0.85f);
            _mute = PlayerPrefs.GetInt("pully.mute", 0) == 1;
            _haptics = PlayerPrefs.GetInt("pully.haptics", 1) == 1;
            _colorblind = PlayerPrefs.GetInt("pully.colorblind", 0) == 1;
        }

        private void OnGUI()
        {
            GUIStyle h = new GUIStyle(GUI.skin.label) { fontSize = 30, alignment = TextAnchor.MiddleCenter };
            GUI.Label(new Rect(0, 30, Screen.width, 50), "Settings", h);

            GUILayout.BeginArea(new Rect(30, 110, Screen.width - 60, Screen.height - 180));
            GUILayout.Label($"Music: {_music:0.00}");
            _music = GUILayout.HorizontalSlider(_music, 0f, 1f);
            GUILayout.Label($"SFX: {_sfx:0.00}");
            _sfx = GUILayout.HorizontalSlider(_sfx, 0f, 1f);
            _mute = GUILayout.Toggle(_mute, "Mute");
            _haptics = GUILayout.Toggle(_haptics, "Haptics");
            _colorblind = GUILayout.Toggle(_colorblind, "Colorblind mode");

            if (GUILayout.Button("Save")) Save();
            if (GUILayout.Button("Replay tutorial")) SceneManager.LoadScene(SceneNames.Tutorial);
            if (GUILayout.Button("Back")) SceneManager.LoadScene(SceneNames.Menu);
            GUILayout.EndArea();
        }

        private void Save()
        {
            PlayerPrefs.SetFloat("pully.music", _music);
            PlayerPrefs.SetFloat("pully.sfx", _sfx);
            PlayerPrefs.SetInt("pully.mute", _mute ? 1 : 0);
            PlayerPrefs.SetInt("pully.haptics", _haptics ? 1 : 0);
            PlayerPrefs.SetInt("pully.colorblind", _colorblind ? 1 : 0);
            PlayerPrefs.Save();
            if (AudioManager.Instance != null) AudioManager.Instance.ApplyPrefs();
        }
    }
}
