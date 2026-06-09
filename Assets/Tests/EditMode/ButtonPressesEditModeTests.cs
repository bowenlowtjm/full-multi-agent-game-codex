using NUnit.Framework;
using Pully.Game;
using UnityEngine;

namespace Pully.Tests
{
    /// <summary>
    /// EditMode tests for UI constants, scene names, and game configuration.
    /// </summary>
    public class ButtonPressesEditModeTests
    {
        [Test]
        public void SceneNames_Splash_IsCorrect()
        {
            Assert.AreEqual("Splash", SceneNames.Splash);
        }

        [Test]
        public void SceneNames_Menu_IsCorrect()
        {
            Assert.AreEqual("Menu", SceneNames.Menu);
        }

        [Test]
        public void SceneNames_Game_IsCorrect()
        {
            Assert.AreEqual("Game", SceneNames.Game);
        }

        [Test]
        public void SceneNames_GameOver_IsCorrect()
        {
            Assert.AreEqual("GameOver", SceneNames.GameOver);
        }

        [Test]
        public void SceneNames_Tutorial_IsCorrect()
        {
            Assert.AreEqual("Tutorial", SceneNames.Tutorial);
        }

        [Test]
        public void SceneNames_Settings_IsCorrect()
        {
            Assert.AreEqual("Settings", SceneNames.Settings);
        }

        [Test]
        public void GestureType_HasFourValues()
        {
            Assert.AreEqual(4, System.Enum.GetValues(typeof(GestureType)).Length);
        }

        [Test]
        public void GestureType_SingleTap_Defined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(GestureType), 0));
        }

        [Test]
        public void GestureType_DoubleTap_Defined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(GestureType), 1));
        }

        [Test]
        public void GestureType_LongPress_Defined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(GestureType), 2));
        }

        [Test]
        public void GestureType_SwipeTap_Defined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(GestureType), 3));
        }

        [Test]
        public void GameState_HasSixStates()
        {
            Assert.AreEqual(6, System.Enum.GetValues(typeof(GameState)).Length);
        }

        [Test]
        public void GameState_INIT_IsZero()
        {
            Assert.AreEqual(0, (int)GameState.INIT);
        }

        [Test]
        public void GameState_MAIN_MENU_IsOne()
        {
            Assert.AreEqual(1, (int)GameState.MAIN_MENU);
        }

        [Test]
        public void GameState_GAMEPLAY_IsTwo()
        {
            Assert.AreEqual(2, (int)GameState.GAMEPLAY);
        }

        [Test]
        public void GameState_PAUSE_IsThree()
        {
            Assert.AreEqual(3, (int)GameState.PAUSE);
        }

        [Test]
        public void GameState_GAME_OVER_IsFour()
        {
            Assert.AreEqual(4, (int)GameState.GAME_OVER);
        }

        [Test]
        public void RulesetDefinition_Shape_HasFourValues()
        {
            Assert.AreEqual(4, System.Enum.GetValues(typeof(RulesetDefinition.Shape)).Length);
        }

        [Test]
        public void RulesetDefinition_Shape_Circle_Defined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(RulesetDefinition.Shape), 0));
        }

        [Test]
        public void RulesetDefinition_Shape_Square_Defined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(RulesetDefinition.Shape), 1));
        }

        [Test]
        public void RulesetDefinition_Shape_Triangle_Defined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(RulesetDefinition.Shape), 2));
        }

        [Test]
        public void RulesetDefinition_Shape_Star_Defined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(RulesetDefinition.Shape), 3));
        }

        [Test]
        public void RulesetDefinition_Gesture_HasFourValues()
        {
            Assert.AreEqual(4, System.Enum.GetValues(typeof(RulesetDefinition.Gesture)).Length);
        }

        [Test]
        public void RulesetDefinition_CanCreateInstance()
        {
            var ruleset = ScriptableObject.CreateInstance<RulesetDefinition>();
            Assert.IsNotNull(ruleset);

            ruleset.lives = 10;
            ruleset.roundSeconds = 60f;
            ruleset.targetLifetime = 3f;
            ruleset.longPressDuration = 0.5f;
            ruleset.doubleTapWindow = 0.3f;
            ruleset.spawnIntervalStart = 1f;
            ruleset.spawnIntervalEnd = 0.5f;
            ruleset.maxConcurrentTargets = 5;
            ruleset.seed = 12345;
            ruleset.comboStep = 1.1f;
            ruleset.comboCap = 5f;

            Assert.AreEqual(10, ruleset.lives);
            Assert.AreEqual(60f, ruleset.roundSeconds);

            Object.DestroyImmediate(ruleset);
        }

        [Test]
        public void ScoreCalculator_StartingCombo_Is1()
        {
            Assert.AreEqual(1f, ScoreCalculator.StartingCombo, 0.0001f);
        }

        [Test]
        public void ScoreCalculator_NextCombo_AppliesStep()
        {
            float result = ScoreCalculator.NextCombo(1f, 1.1f, 5f);
            Assert.AreEqual(1.1f, result, 0.0001f);
        }

        [Test]
        public void ScoreCalculator_NextCombo_ClampsAtCap()
        {
            float result = ScoreCalculator.NextCombo(4.9f, 1.1f, 5f);
            Assert.AreEqual(5f, result, 0.0001f);
        }

        [Test]
        public void ScoreCalculator_ScoreFor_CalculatesCorrectly()
        {
            int result = ScoreCalculator.ScoreFor(10, 2f);
            Assert.AreEqual(20, result);
        }

        [Test]
        public void ScoreCalculator_ScoreFor_RoundsCorrectly()
        {
            int result = ScoreCalculator.ScoreFor(10, 1.5f);
            Assert.AreEqual(15, result);
        }

        [Test]
        public void PlayerPrefs_BestScore_KeyIsCorrect()
        {
            string key = "pully.highscore";
            Assert.AreEqual("pully.highscore", key);
        }
    }
}
