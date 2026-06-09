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
            GUIStyle s = new GUIStyle(GUI.skin.label) { fontSize = 42, alignment = TextAnchor.MiddleCenter };
            GUI.Label(new Rect(0, Screen.height / 2 - 60, Screen.width, 60), "PULLY", s);
            GUIStyle t = new GUIStyle(GUI.skin.label) { fontSize = 18, alignment = TextAnchor.MiddleCenter };
            GUI.Label(new Rect(0, Screen.height / 2 + 10, Screen.width, 40), "loading...", t);
        }
    }
}
