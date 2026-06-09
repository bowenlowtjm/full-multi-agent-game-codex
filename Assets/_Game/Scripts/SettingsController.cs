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
            UISkin.DrawBackground();
            var card = UISkin.DrawCard();
            GUI.Label(new Rect(0, 24, Screen.width, 58), "Settings", UISkin.TitleStyle);

            GUILayout.BeginArea(new Rect(card.x + 24, card.y + 30, card.width - 48, card.height - 60));
            GUILayout.Label($"Music: {_music:0.00}", UISkin.BodyStyle);
            _music = GUILayout.HorizontalSlider(_music, 0f, 1f);
            GUILayout.Label($"SFX: {_sfx:0.00}", UISkin.BodyStyle);
            _sfx = GUILayout.HorizontalSlider(_sfx, 0f, 1f);

            _mute = GUILayout.Toggle(_mute, "Mute");
            _haptics = GUILayout.Toggle(_haptics, "Haptics");
            _colorblind = GUILayout.Toggle(_colorblind, "Colorblind mode");

            GUILayout.Space(16);
            if (GUILayout.Button("Save", UISkin.ButtonStyle)) Save();
            if (GUILayout.Button("Replay tutorial", UISkin.ButtonStyle)) SceneManager.LoadScene(SceneNames.Tutorial);
            if (GUILayout.Button("Back", UISkin.ButtonStyle)) SceneManager.LoadScene(SceneNames.Menu);
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
            if (HapticsManager.Instance != null) HapticsManager.Instance.SetEnabled(_haptics);
        }
    }
}
