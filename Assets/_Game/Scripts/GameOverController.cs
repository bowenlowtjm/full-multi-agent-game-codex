using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pully.Game
{
    public class GameOverController : MonoBehaviour
    {
        private void OnGUI()
        {
            UISkin.DrawBackground();
            var card = UISkin.DrawCard(top: 90f, minHeight: 320f);

            GUI.Label(new Rect(0, 34, Screen.width, 58), "Game Over", UISkin.TitleStyle);
            GUI.Label(new Rect(0, card.y + 56, Screen.width, 42), $"Score: {SessionData.LastScore}", UISkin.SubtitleStyle);
            GUI.Label(new Rect(0, card.y + 102, Screen.width, 36), $"Best: {PlayerPrefs.GetInt("pully.highscore", 0)}", UISkin.ChipStyle);

            int cx = Screen.width / 2 - 130;
            if (GUI.Button(new Rect(cx, (int)card.y + 168, 260, 56), "Retry", UISkin.ButtonStyle))
                SceneManager.LoadScene(SceneNames.Game);

            if (GUI.Button(new Rect(cx, (int)card.y + 236, 260, 56), "Menu", UISkin.ButtonStyle))
                SceneManager.LoadScene(SceneNames.Menu);
        }
    }
}
