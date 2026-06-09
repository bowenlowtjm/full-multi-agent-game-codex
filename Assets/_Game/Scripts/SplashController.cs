using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pully.Game
{
    public class SplashController : MonoBehaviour
    {
        private float _timer;

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer < 1.0f) return;

            bool tutorialSeen = PlayerPrefs.GetInt("pully.tutorial.seen", 0) == 1;
            SceneManager.LoadScene(tutorialSeen ? SceneNames.Menu : SceneNames.Tutorial);
        }

        private void OnGUI()
        {
            UISkin.DrawBackground();
            UISkin.DrawCard(top: Screen.height * 0.28f, minHeight: Screen.height * 0.36f);

            GUI.Label(new Rect(0, Screen.height / 2f - 78f, Screen.width, 60f), "PULLY", UISkin.TitleStyle);
            GUI.Label(new Rect(0, Screen.height / 2f - 18f, Screen.width, 38f), "Gesture arcade", UISkin.SubtitleStyle);
            GUI.Label(new Rect(0, Screen.height / 2f + 28f, Screen.width, 30f), "loading...", UISkin.ChipStyle);
        }
    }
}
