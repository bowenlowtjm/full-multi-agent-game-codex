using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pully.Game
{
    public class TutorialController : MonoBehaviour
    {
        private int _page;

        private readonly Step[] _steps =
        {
            new("Tap green circle", "Single Tap", "Tap once quickly."),
            new("Blue square", "Double Tap", "Tap the SAME square twice quickly (<300ms)."),
            new("Red circle", "Long Press", "Hold for at least 0.5s, then release."),
            new("Yellow triangle", "Swipe / Flick", "Touch and drag/flick through it."),
            new("Purple star", "Two-finger Tap", "Use two fingers on device (simulated as advanced input in editor).")
        };

        private void OnGUI()
        {
            UISkin.DrawBackground();
            var card = UISkin.DrawCard();

            GUI.Label(new Rect(0, 24, Screen.width, 58), "How To Play", UISkin.TitleStyle);

            var step = _steps[_page];
            GUI.Label(new Rect(card.x + 28, card.y + 36, card.width - 56, 48), step.Title, UISkin.SubtitleStyle);
            GUI.Label(new Rect(card.x + 28, card.y + 88, card.width - 56, 36), step.Gesture, UISkin.ChipStyle);
            GUI.Label(new Rect(card.x + 28, card.y + 128, card.width - 56, 130), step.Help, UISkin.BodyStyle);

            GUI.Label(new Rect(0, Screen.height - 142, Screen.width, 30), $"Step {_page + 1}/{_steps.Length}", UISkin.ChipStyle);

            if (GUI.Button(new Rect(30, Screen.height - 90, 170, 50), "Skip", UISkin.ButtonStyle))
                FinishTutorial();

            if (_page > 0)
            {
                if (GUI.Button(new Rect(Screen.width / 2f - 95f, Screen.height - 90, 190, 50), "Previous", UISkin.ButtonStyle))
                    _page--;
            }

            if (GUI.Button(new Rect(Screen.width - 200, Screen.height - 90, 170, 50), _page == _steps.Length - 1 ? "Finish" : "Next", UISkin.ButtonStyle))
            {
                if (_page == _steps.Length - 1) FinishTutorial();
                else _page++;
            }
        }

        private static void FinishTutorial()
        {
            PlayerPrefs.SetInt("pully.tutorial.seen", 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene(SceneNames.Menu);
        }

        private readonly struct Step
        {
            public readonly string Title;
            public readonly string Gesture;
            public readonly string Help;

            public Step(string title, string gesture, string help)
            {
                Title = title;
                Gesture = gesture;
                Help = help;
            }
        }
    }
}
