using System.Collections;
using NUnit.Framework;
using Pully.Game;
using UnityEngine;
using UnityEngine.TestTools;

namespace Pully.Tests
{
    public class SmokePlayModeTests
    {
        [UnityTest]
        public IEnumerator Bootstrap_CreatesCoreSystems_AndHasGameplayState()
        {
            var root = new GameObject("BootstrapRoot");
            root.AddComponent<MenuBootstrap>();

            yield return null;
            yield return null;

            var core = Object.FindFirstObjectByType<CoreLoopBootstrap>();
            var score = Object.FindFirstObjectByType<ScoreManager>();
            var state = Object.FindFirstObjectByType<GameStateManager>();
            var spawner = Object.FindFirstObjectByType<SpawnerManager>();
            var input = Object.FindFirstObjectByType<InputManager>();

            Assert.IsNotNull(core);
            Assert.IsNotNull(score);
            Assert.IsNotNull(state);
            Assert.IsNotNull(spawner);
            Assert.IsNotNull(input);
            Assert.AreEqual(GameState.GAMEPLAY, state.CurrentState);
            Assert.AreEqual(10, score.Lives);

            Object.Destroy(root);
            if (core != null) Object.Destroy(core.gameObject);
            if (score != null) Object.Destroy(score.gameObject);
            if (state != null) Object.Destroy(state.gameObject);
            if (spawner != null) Object.Destroy(spawner.gameObject);
            if (input != null) Object.Destroy(input.gameObject);

            yield return null;
        }
    }
}
