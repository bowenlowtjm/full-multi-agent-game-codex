using NUnit.Framework;
using Pully.Game;
using UnityEngine;

namespace Pully.Tests
{
    public class ScoreCalculatorTests
    {
        [Test]
        public void NextCombo_AppliesStep()
        {
            Assert.AreEqual(1.1f, ScoreCalculator.NextCombo(1f, 1.1f, 5f), 1e-4f);
        }

        [Test]
        public void NextCombo_ClampsToCap()
        {
            Assert.AreEqual(5f, ScoreCalculator.NextCombo(4.8f, 1.1f, 5f), 1e-4f);
        }

        [TestCase(1, 1f, 1)]
        [TestCase(8, 1.1f, 9)]
        [TestCase(5, 5f, 25)]
        public void ScoreFor_RoundsToNearest(int baseReward, float combo, int expected)
        {
            Assert.AreEqual(expected, ScoreCalculator.ScoreFor(baseReward, combo));
        }

        [Test]
        public void ScoreManager_RegisterMiss_ResetsComboAndConsumesLife()
        {
            var go = new GameObject("score");
            var mgr = go.AddComponent<ScoreManager>();
            var ruleset = ScriptableObject.CreateInstance<RulesetDefinition>();
            ruleset.lives = 10;
            ruleset.comboStep = 1.1f;
            ruleset.comboCap = 5f;
            mgr.Configure(ruleset);

            mgr.RegisterHit(5);
            Assert.Greater(mgr.Combo, 1f);

            mgr.RegisterMiss();
            Assert.AreEqual(1f, mgr.Combo, 0.0001f);
            Assert.AreEqual(2, mgr.Lives);

            Object.DestroyImmediate(go);
        }
    }
}
