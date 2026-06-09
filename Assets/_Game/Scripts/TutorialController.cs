using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pully.Game
{
    public class TutorialController : MonoBehaviour
    {
        private int _page;
        private readonly string[] _pages =
        {
            "Tap green circle: Single Tap",
            "Blue square: Double Tap (<300ms)",
            "Red circle: Long Press (>500ms)",
            "Yellow triangle: Swipe/Flick",
            "Purple star: Two-finger tap"
        };

        private void OnGUI()
        {
            GUIStyle h = new GUIStyle(GUI.skin.label) { fontSize = 30, alignment = TextAnchor.MiddleCenter };
            GUIStyle b = new GUIStyle(GUI.skin.label) { fontSize = 20, alignment = TextAnchor.MiddleCenter };
            GUI.Label(new Rect(0, 40, Screen.width, 60), "How To Play", h);
            GUI.Label(new Rect(30, Screen.height / 2 - 30, Screen.width - 60, 100), _pages[_page], b);

            if (GUI.Button(new Rect(30, Screen.height - 90, 150, 45), "Skip")) FinishTutorial();
            if (GUI.Button(new Rect(Screen.width - 180, Screen.height - 90, 150, 45), _page == _pages.Length - 1 ? "Finish" : "Next"))
            {
                if (_page == _pages.Length - 1) FinishTutorial();
                else _page++;
            }
        }

        private static void FinishTutorial()
        {
            PlayerPrefs.SetInt("pully.tutorial.seen", 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene(SceneNames.Menu);
        }
    }
}
