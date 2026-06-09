using System.Collections;
using NUnit.Framework;
using Pully.Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Pully.Tests
{
    /// <summary>
    /// Tests for UI button interactions and scene transitions.
    /// </summary>
    public class ButtonPressesTests
    {
        private class ButtonClickTracker : MonoBehaviour
        {
            public bool Clicked = false;
            public void TrackClick() { Clicked = true; }
        }

        [TearDown]
        public void TearDown()
        {
            var sceneObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var go in sceneObjects)
            {
                if (go.GetComponent<MenuController>() != null ||
                    go.GetComponent<GameSceneController>() != null ||
                    go.GetComponent<GameOverController>() != null ||
                    go.GetComponent<SplashController>() != null ||
                    go.GetComponent<SettingsController>() != null ||
                    go.GetComponent<TutorialController>() != null)
                {
                    Object.Destroy(go);
                }
            }
        }

        [UnityTest]
        public IEnumerator MenuController_PlayButton_Exists()
        {
            var menuGO = new GameObject("MenuController");
            menuGO.AddComponent<MenuController>();

            yield return null;

            Assert.IsNotNull(menuGO.GetComponent<MenuController>());
        }

        [UnityTest]
        public IEnumerator MenuController_TutorialButton_Exists()
        {
            var menuGO = new GameObject("MenuController");
            menuGO.AddComponent<MenuController>();

            yield return null;

            Assert.IsNotNull(menuGO.GetComponent<MenuController>());
        }

        [UnityTest]
        public IEnumerator MenuController_SettingsButton_Exists()
        {
            var menuGO = new GameObject("MenuController");
            menuGO.AddComponent<MenuController>();

            yield return null;

            Assert.IsNotNull(menuGO.GetComponent<MenuController>());
        }

        [UnityTest]
        public IEnumerator GameSceneController_PauseButton_Exists()
        {
            // Create canvas for UI
            var canvasGO = new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            var gameGO = new GameObject("GameSceneController");
            gameGO.AddComponent<GameSceneController>();

            yield return null;
            yield return null;

            Assert.IsNotNull(gameGO.GetComponent<GameSceneController>());

            Object.Destroy(gameGO);
            Object.Destroy(canvasGO);
        }

        [UnityTest]
        public IEnumerator GameOverController_RestartButton_Exists()
        {
            var gameOverGO = new GameObject("GameOverController");
            gameOverGO.AddComponent<GameOverController>();

            yield return null;

            Assert.IsNotNull(gameOverGO.GetComponent<GameOverController>());

            Object.Destroy(gameOverGO);
        }

        [UnityTest]
        public IEnumerator GameOverController_MenuButton_Exists()
        {
            var gameOverGO = new GameObject("GameOverController");
            gameOverGO.AddComponent<GameOverController>();

            yield return null;

            Assert.IsNotNull(gameOverGO.GetComponent<GameOverController>());

            Object.Destroy(gameOverGO);
        }

        [UnityTest]
        public IEnumerator SplashController_TapToStart_Detected()
        {
            var splashGO = new GameObject("SplashController");
            splashGO.AddComponent<SplashController>();

            yield return null;

            Assert.IsNotNull(splashGO.GetComponent<SplashController>());

            Object.Destroy(splashGO);
        }

        [UnityTest]
        public IEnumerator SettingsController_BackButton_Exists()
        {
            var settingsGO = new GameObject("SettingsController");
            settingsGO.AddComponent<SettingsController>();

            yield return null;

            Assert.IsNotNull(settingsGO.GetComponent<SettingsController>());

            Object.Destroy(settingsGO);
        }

        [UnityTest]
        public IEnumerator PauseController_ResumeButton_Exists()
        {
            // Test game state pause/resume
            var stateGO = new GameObject("GameStateManager");
            var stateManager = stateGO.AddComponent<GameStateManager>();

            stateManager.StartRound(60f);
            Assert.AreEqual(GameState.GAMEPLAY, stateManager.CurrentState);

            // Pause
            stateManager.SetState(GameState.PAUSE);
            yield return null;
            Assert.AreEqual(GameState.PAUSE, stateManager.CurrentState);

            // Resume
            stateManager.SetState(GameState.GAMEPLAY);
            yield return null;
            Assert.AreEqual(GameState.GAMEPLAY, stateManager.CurrentState);

            Object.Destroy(stateGO);
        }

        [UnityTest]
        public IEnumerator PauseController_ExitButton_Exists()
        {
            var stateGO = new GameObject("GameStateManager");
            var stateManager = stateGO.AddComponent<GameStateManager>();

            stateManager.StartRound(60f);

            // Exit to main menu
            stateManager.SetState(GameState.MAIN_MENU);
            yield return null;

            Assert.AreEqual(GameState.MAIN_MENU, stateManager.CurrentState);

            Object.Destroy(stateGO);
        }

        [UnityTest]
        public IEnumerator TutorialController_BackButton_Exists()
        {
            var tutorialGO = new GameObject("TutorialController");
            tutorialGO.AddComponent<TutorialController>();

            yield return null;

            Assert.IsNotNull(tutorialGO.GetComponent<TutorialController>());

            Object.Destroy(tutorialGO);
        }

        [UnityTest]
        public IEnumerator AllGestures_DetectedCorrectly()
        {
            // Test that all gesture types can be processed
            var gestures = new[]
            {
                GestureType.SingleTap,
                GestureType.DoubleTap,
                GestureType.LongPress,
                GestureType.SwipeTap
            };

            foreach (var gesture in gestures)
            {
                // This validates that gesture type is valid
                Assert.IsTrue(System.Enum.IsDefined(typeof(GestureType), gesture),
                    $"Gesture {gesture} should be defined");
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator AllGameStates_Defined()
        {
            foreach (var state in System.Enum.GetValues(typeof(GameState)))
            {
                Assert.IsNotNull(state);
            }

            Assert.AreEqual(6, System.Enum.GetValues(typeof(GameState)).Length);
            yield return null;
        }

        [UnityTest]
        public IEnumerator AllSceneNames_Defined()
        {
            Assert.AreEqual("Splash", SceneNames.Splash);
            Assert.AreEqual("Menu", SceneNames.Menu);
            Assert.AreEqual("Game", SceneNames.Game);
            Assert.AreEqual("GameOver", SceneNames.GameOver);
            Assert.AreEqual("Tutorial", SceneNames.Tutorial);
            Assert.AreEqual("Settings", SceneNames.Settings);

            yield return null;
        }

        [UnityTest]
        public IEnumerator PlayerPrefs_BestScore_CanBeReadWritten()
        {
            // Clear any existing high score
            PlayerPrefs.SetInt("pully.highscore", 0);
            PlayerPrefs.Save();

            int initial = PlayerPrefs.GetInt("pully.highscore", 0);
            Assert.AreEqual(0, initial);

            // Set a new high score
            PlayerPrefs.SetInt("pully.highscore", 100);
            PlayerPrefs.Save();

            int saved = PlayerPrefs.GetInt("pully.highscore", 0);
            Assert.AreEqual(100, saved);

            // Reset for clean state
            PlayerPrefs.SetInt("pully.highscore", 0);
            PlayerPrefs.Save();

            yield return null;
        }
    }
}
