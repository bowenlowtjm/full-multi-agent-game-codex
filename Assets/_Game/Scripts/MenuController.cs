using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pully.Game
{
    public class MenuController : MonoBehaviour
    {
        private void OnGUI()
        {
            UISkin.DrawBackground();
            var card = UISkin.DrawCard();

            GUI.Label(new Rect(0, 24, Screen.width, 58), "PULLY", UISkin.TitleStyle);
            GUI.Label(new Rect(0, 74, Screen.width, 38), "Flappy-style gesture arcade", UISkin.ChipStyle);

            int centerX = Screen.width / 2 - 130;
            int y = (int)card.y + 80;

            if (GUI.Button(new Rect(centerX, y, 260, 56), "Play", UISkin.ButtonStyle))
                SceneManager.LoadScene(SceneNames.Game);
            y += 70;
            if (GUI.Button(new Rect(centerX, y, 260, 56), "How To Play", UISkin.ButtonStyle))
                SceneManager.LoadScene(SceneNames.Tutorial);
            y += 70;
            if (GUI.Button(new Rect(centerX, y, 260, 56), "Settings", UISkin.ButtonStyle))
                SceneManager.LoadScene(SceneNames.Settings);

            int best = PlayerPrefs.GetInt("pully.highscore", 0);
            GUI.Label(new Rect(0, Screen.height - 58, Screen.width, 32), $"Best: {best}", UISkin.ChipStyle);
        }
    }
}
