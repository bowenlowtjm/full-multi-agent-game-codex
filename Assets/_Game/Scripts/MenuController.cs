using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pully.Game
{
    public class MenuController : MonoBehaviour
    {
        private void OnGUI()
        {
            GUIStyle h = new GUIStyle(GUI.skin.label) { fontSize = 32, alignment = TextAnchor.MiddleCenter };
            GUI.Label(new Rect(0, 40, Screen.width, 60), "Pully", h);

            int centerX = Screen.width / 2 - 110;
            int y = 160;
            if (GUI.Button(new Rect(centerX, y, 220, 50), "Play")) SceneManager.LoadScene(SceneNames.Game);
            y += 65;
            if (GUI.Button(new Rect(centerX, y, 220, 50), "Settings")) SceneManager.LoadScene(SceneNames.Settings);
            y += 65;
            if (GUI.Button(new Rect(centerX, y, 220, 50), "How To Play")) SceneManager.LoadScene(SceneNames.Tutorial);

            GUIStyle s = new GUIStyle(GUI.skin.label) { fontSize = 20, alignment = TextAnchor.MiddleCenter };
            int best = PlayerPrefs.GetInt("pully.highscore", 0);
            GUI.Label(new Rect(0, Screen.height - 70, Screen.width, 40), $"Best: {best}", s);
        }
    }
}
