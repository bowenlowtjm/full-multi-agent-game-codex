using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pully.Game
{
    public class GameOverController : MonoBehaviour
    {
        private void OnGUI()
        {
            GUIStyle h = new GUIStyle(GUI.skin.label) { fontSize = 34, alignment = TextAnchor.MiddleCenter };
            GUIStyle s = new GUIStyle(GUI.skin.label) { fontSize = 24, alignment = TextAnchor.MiddleCenter };

            GUI.Label(new Rect(0, 40, Screen.width, 60), "Game Over", h);
            GUI.Label(new Rect(0, 120, Screen.width, 50), $"Score: {SessionData.LastScore}", s);
            GUI.Label(new Rect(0, 165, Screen.width, 50), $"Best: {PlayerPrefs.GetInt("pully.highscore", 0)}", s);

            int cx = Screen.width / 2 - 110;
            if (GUI.Button(new Rect(cx, 250, 220, 50), "Retry"))
            {
                SceneManager.LoadScene(SceneNames.Game);
            }
            if (GUI.Button(new Rect(cx, 315, 220, 50), "Menu"))
            {
                SceneManager.LoadScene(SceneNames.Menu);
            }
        }
    }
}
