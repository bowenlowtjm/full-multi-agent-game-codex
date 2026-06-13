using System.Collections;
using System.Reflection;
using NUnit.Framework;
using Pully.Game;
using UnityEngine;
using UnityEngine.TestTools;

namespace Pully.Tests
{
    public class BotSmokePlayModeTests
    {
        private class RoundResult
        {
            public int score;
            public int lives;
            public int misses;
            public BotPlayer bot;
        }

        [UnityTest]
        public IEnumerator BotSmoke_InputMode_UsesRecognizerPath_AndScores()
        {
            var result = new RoundResult();
            yield return RunBotRound(
                seed: 12345,
                accuracy: 1.0f,
                mode: BotPlayer.BotInputMode.Input,
                minActions: 2,
                requireScoreIncrease: true,
                requireMisses: false,
                requireInputEvents: true,
                requireNoDirectResolve: true,
                result: result);

            Assert.Greater(result.score, 0, $"Input mode should score via recognizer path. {result.bot.GetReport()}");
            Assert.AreEqual(0, result.misses, $"All-correct run should have no misses. {result.bot.GetReport()}");
        }

        [UnityTest]
        public IEnumerator BotSmoke_InputMode_WrongGesture_CausesMissPenalty()
        {
            var result = new RoundResult();
            yield return RunBotRound(
                seed: 54321,
                accuracy: 0.0f,
                mode: BotPlayer.BotInputMode.Input,
                minActions: 1,
                requireScoreIncrease: false,
                requireMisses: true,
                requireInputEvents: true,
                requireNoDirectResolve: true,
                result: result);

            Assert.Greater(result.misses, 0, $"Expected at least one miss from wrong injected gestures. {result.bot.GetReport()}");
            Assert.Less(result.lives, 10, $"Expected life penalty from wrong gesture recognizer result. {result.bot.GetReport()}");
            Assert.AreEqual(0, result.bot.DirectResolveCalls, "Input mode must not call direct resolver.");
        }

        [UnityTest]
        public IEnumerator BotSmoke_InputMode_Deterministic_SeededRunsMatch()
        {
            var resultA = new RoundResult();
            yield return RunBotRound(
                seed: 777,
                accuracy: 1.0f,
                mode: BotPlayer.BotInputMode.Input,
                minActions: 4,
                requireScoreIncrease: true,
                requireMisses: false,
                requireInputEvents: true,
                requireNoDirectResolve: true,
                result: resultA);

            var resultB = new RoundResult();
            yield return RunBotRound(
                seed: 777,
                accuracy: 1.0f,
                mode: BotPlayer.BotInputMode.Input,
                minActions: 4,
                requireScoreIncrease: true,
                requireMisses: false,
                requireInputEvents: true,
                requireNoDirectResolve: true,
                result: resultB);

            Debug.Log($"[BotDeterminism] scoreA={resultA.score} scoreB={resultB.score}");
            Assert.AreEqual(resultA.score, resultB.score, $"Seeded input runs diverged. A={resultA.bot.GetReport()} B={resultB.bot.GetReport()}");
        }

        private static IEnumerator RunBotRound(
            int seed,
            float accuracy,
            BotPlayer.BotInputMode mode,
            int minActions,
            bool requireScoreIncrease,
            bool requireMisses,
            bool requireInputEvents,
            bool requireNoDirectResolve,
            RoundResult result)
        {
            result.score = 0;
            result.lives = 0;
            result.misses = 0;
            result.bot = null;

            var root = new GameObject("BotSmokeBootstrapRoot");
            root.AddComponent<MenuBootstrap>();

            yield return null;
            yield return null;

            CoreLoopBootstrap core = null;
            SpawnerManager spawner = null;
            ScoreManager score = null;
            GameStateManager state = null;

            const float bootstrapTimeout = 5f;
            float bootstrapDeadline = Time.time + bootstrapTimeout;
            while (Time.time < bootstrapDeadline)
            {
                core = Object.FindFirstObjectByType<CoreLoopBootstrap>();
                spawner = Object.FindFirstObjectByType<SpawnerManager>();
                score = Object.FindFirstObjectByType<ScoreManager>();
                state = Object.FindFirstObjectByType<GameStateManager>();

                if (core != null && spawner != null && score != null && state != null && state.CurrentState == GameState.GAMEPLAY)
                {
                    break;
                }

                yield return null;
            }

            Assert.IsNotNull(core, "CoreLoopBootstrap missing.");
            Assert.IsNotNull(spawner, "SpawnerManager missing.");
            Assert.IsNotNull(score, "ScoreManager missing.");
            Assert.IsNotNull(state, "GameStateManager missing.");
            Assert.AreEqual(GameState.GAMEPLAY, state.CurrentState, "Game did not reach GAMEPLAY.");

            var botGo = new GameObject($"Bot_{mode}_{seed}");
            result.bot = botGo.AddComponent<BotPlayer>();
            result.bot.Configure(spawner, score, state, seed: seed, mode: mode);

            SetPrivateField(result.bot, "reactionTimeMin", 0.01f);
            SetPrivateField(result.bot, "reactionTimeMax", 0.02f);
            SetPrivateField(result.bot, "accuracyThreshold", accuracy);

            int scoreBefore = score.Score;
            int livesBefore = score.Lives;

            result.bot.StartBotPlay();

            const float runTimeout = 8f;
            float runDeadline = Time.time + runTimeout;
            while (Time.time < runDeadline)
            {
                bool actionsReady = result.bot.TotalActions >= minActions;
                bool scoreReady = !requireScoreIncrease || score.Score > scoreBefore;
                bool missReady = !requireMisses || score.Lives < livesBefore;

                if (actionsReady && scoreReady && missReady)
                {
                    break;
                }

                yield return null;
            }

            result.bot.StopBotPlay();

            result.score = score.Score;
            result.lives = score.Lives;
            result.misses = result.bot.Misses;

            Assert.GreaterOrEqual(result.bot.TotalActions, minActions, $"Bot performed too few actions. {result.bot.GetReport()}");

            if (requireScoreIncrease)
            {
                Assert.Greater(result.score, scoreBefore, $"Expected score increase. {result.bot.GetReport()}");
            }

            if (requireMisses)
            {
                Assert.Less(result.lives, livesBefore, $"Expected miss penalty (life loss). {result.bot.GetReport()}");
                Assert.Greater(result.bot.Misses, 0, $"Expected bot miss count > 0. {result.bot.GetReport()}");
            }

            if (requireInputEvents)
            {
                Assert.Greater(result.bot.InputEventsQueued, 0, $"Expected queued input events in Input mode. {result.bot.GetReport()}");
            }

            if (requireNoDirectResolve)
            {
                Assert.AreEqual(0, result.bot.DirectResolveCalls, $"Input mode must not directly call resolver. {result.bot.GetReport()}");
            }

            Cleanup(root, botGo, core, spawner, score, state);
            yield return null;
        }

        private static void Cleanup(GameObject root, GameObject botGo, CoreLoopBootstrap core, SpawnerManager spawner, ScoreManager score, GameStateManager state)
        {
            if (root != null) Object.Destroy(root);
            if (botGo != null) Object.Destroy(botGo);
            if (core != null) Object.Destroy(core.gameObject);
            if (spawner != null) Object.Destroy(spawner.gameObject);
            if (score != null) Object.Destroy(score.gameObject);
            if (state != null) Object.Destroy(state.gameObject);

            var leftovers = Object.FindObjectsByType<TargetRuntime>(FindObjectsSortMode.None);
            foreach (var target in leftovers)
            {
                if (target != null) Object.Destroy(target.gameObject);
            }
        }

        private static void SetPrivateField(BotPlayer bot, string fieldName, float value)
        {
            FieldInfo field = typeof(BotPlayer).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field, $"Expected private field '{fieldName}' on BotPlayer.");
            field.SetValue(bot, value);
        }
    }
}
