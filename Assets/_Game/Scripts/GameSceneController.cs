using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pully.Game
{
    public class GameSceneController : MonoBehaviour
    {
        private CoreLoopBootstrap _core;
        private GameStateManager _state;
        private ScoreManager _score;

        private void Start()
        {
            _core = FindFirstObjectByType<CoreLoopBootstrap>();
            _state = FindFirstObjectByType<GameStateManager>();
            _score = FindFirstObjectByType<ScoreManager>();

            if (_core == null)
            {
                var go = new GameObject("CoreLoop");
                _core = go.AddComponent<CoreLoopBootstrap>();
            }

            if (FindFirstObjectByType<HUDManager>() == null)
            {
                var hud = new GameObject("HUD");
                hud.AddComponent<HUDManager>();
            }

            _state = FindFirstObjectByType<GameStateManager>();
            _score = FindFirstObjectByType<ScoreManager>();

            if (_state != null)
            {
                _state.OnStateChanged += OnStateChanged;
            }

            if (FindFirstObjectByType<AudioManager>() == null)
            {
                var audioRoot = new GameObject("AudioManager");
                audioRoot.AddComponent<AudioManager>();
            }

            _core.BeginGameplay();
        }

        private void OnDestroy()
        {
            if (_state != null)
            {
                _state.OnStateChanged -= OnStateChanged;
            }
        }

        private void OnStateChanged(GameState next, GameState old)
        {
            if (next != GameState.GAME_OVER) return;

            int score = _score != null ? _score.Score : 0;
            SessionData.LastScore = score;
            int best = PlayerPrefs.GetInt("pully.highscore", 0);
            if (score > best)
            {
                PlayerPrefs.SetInt("pully.highscore", score);
                PlayerPrefs.Save();
            }

            SceneManager.LoadScene(SceneNames.GameOver);
        }

        private void OnGUI()
        {
            if (_state == null || _core == null) return;

            if (_state.CurrentState == GameState.GAMEPLAY)
            {
                if (GUI.Button(new Rect(Screen.width - 140, 20, 120, 40), "Pause"))
                {
                    _core.PauseGameplay();
                }
            }
            else if (_state.CurrentState == GameState.PAUSE)
            {
                if (GUI.Button(new Rect(Screen.width - 140, 20, 120, 40), "Resume"))
                {
                    _core.ResumeGameplay();
                }
                if (GUI.Button(new Rect(Screen.width - 140, 70, 120, 40), "Menu"))
                {
                    SceneManager.LoadScene(SceneNames.Menu);
                }
            }
        }
    }
}
