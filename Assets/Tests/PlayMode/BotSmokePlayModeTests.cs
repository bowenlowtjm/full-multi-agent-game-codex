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
        [UnityTest]
        public IEnumerator BotSmoke_PlayMode_RunsActions_AndScores()
        {
            var root = new GameObject("BotSmokeBootstrapRoot");
            root.AddComponent<MenuBootstrap>();

            // Allow bootstrap/start coroutines to construct systems.
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

            Assert.IsNotNull(core, "CoreLoopBootstrap was not created by MenuBootstrap.");
            Assert.IsNotNull(spawner, "SpawnerManager was not created by bootstrap.");
            Assert.IsNotNull(score, "ScoreManager was not created by bootstrap.");
            Assert.IsNotNull(state, "GameStateManager was not created by bootstrap.");
            Assert.AreEqual(GameState.GAMEPLAY, state.CurrentState, "Game did not reach GAMEPLAY state.");

            var botGo = new GameObject("BotSmokePlayer");
            var bot = botGo.AddComponent<BotPlayer>();
            bot.Configure(spawner, score, state, seed: 12345);

            // Tight deterministic smoke settings for CI speed/stability.
            SetPrivateField(bot, "reactionTimeMin", 0.01f);
            SetPrivateField(bot, "reactionTimeMax", 0.02f);
            SetPrivateField(bot, "accuracyThreshold", 1.0f);

            bot.StartBotPlay();

            const float runTimeout = 6f;
            float runDeadline = Time.time + runTimeout;
            while (Time.time < runDeadline)
            {
                if (bot.TotalActions >= 1 && score.Score > 0)
                {
                    break;
                }

                yield return null;
            }

            bot.StopBotPlay();

            Assert.GreaterOrEqual(bot.TotalActions, 1, $"Bot performed no actions. Report: {bot.GetReport()}");
            Assert.Greater(score.Score, 0, $"Bot did not score points. Report: {bot.GetReport()}");
            Assert.AreEqual(0, bot.Misses, $"Expected zero misses in deterministic smoke mode. Report: {bot.GetReport()}");

            Object.Destroy(root);
            if (botGo != null) Object.Destroy(botGo);
            if (core != null) Object.Destroy(core.gameObject);
            if (spawner != null) Object.Destroy(spawner.gameObject);
            if (score != null) Object.Destroy(score.gameObject);
            if (state != null) Object.Destroy(state.gameObject);

            yield return null;
        }

        private static void SetPrivateField(BotPlayer bot, string fieldName, float value)
        {
            FieldInfo field = typeof(BotPlayer).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field, $"Expected private field '{fieldName}' on BotPlayer.");
            field.SetValue(bot, value);
        }
    }
}
