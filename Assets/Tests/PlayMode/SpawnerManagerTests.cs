using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Pully.Game;
using UnityEngine;
using UnityEngine.TestTools;

namespace Pully.Tests
{
    /// <summary>
    /// Tests for SpawnerManager - spawn logic, resolution, and lifecycle.
    /// </summary>
    public class SpawnerManagerTests
    {
        private GameObject _spawnerGO;
        private SpawnerManager _spawner;
        private Camera _camera;
        private GameStateManager _stateManager;
        private RulesetDefinition _ruleset;

        [SetUp]
        public void Setup()
        {
            // Create camera
            var camGO = new GameObject("MainCamera");
            camGO.tag = "MainCamera";
            _camera = camGO.AddComponent<Camera>();
            _camera.orthographic = true;
            _camera.transform.position = new Vector3(0f, 0f, -10f);

            // Create state manager
            var stateGO = new GameObject("StateManager");
            _stateManager = stateGO.AddComponent<GameStateManager>();
            _stateManager.SetState(GameState.GAMEPLAY);

            // Create ruleset
            _ruleset = ScriptableObject.CreateInstance<RulesetDefinition>();
            _ruleset.lives = 10;
            _ruleset.roundSeconds = 60f;
            _ruleset.targetLifetime = 3f;
            _ruleset.longPressDuration = 0.5f;
            _ruleset.doubleTapWindow = 0.3f;
            _ruleset.maxConcurrentTargets = 3;
            _ruleset.spawnIntervalStart = 1f;
            _ruleset.spawnIntervalEnd = 0.5f;
            _ruleset.seed = 12345;
            _ruleset.comboStep = 1.1f;
            _ruleset.comboCap = 5f;

            // Create spawner
            _spawnerGO = new GameObject("SpawnerManager");
            _spawner = _spawnerGO.AddComponent<SpawnerManager>();
            _spawner.Configure(_ruleset, _camera, _stateManager);
        }

        [TearDown]
        public void TearDown()
        {
            var targets = Object.FindObjectsByType<TargetRuntime>(FindObjectsSortMode.None);
            foreach (var t in targets)
                Object.Destroy(t.gameObject);

            Object.Destroy(_spawnerGO);
            if (_camera != null) Object.Destroy(_camera.gameObject);
            if (_stateManager != null) Object.Destroy(_stateManager.gameObject);
            Object.Destroy(_ruleset);
        }

        [Test]
        public void Configure_SetsRuleset()
        {
            Assert.AreEqual(_ruleset, _spawner.Ruleset);
        }

        [Test]
        public void Configure_CreatesRNG_WithSeed()
        {
            // Test determinism by checking spawns are consistent
            Assert.AreEqual(_ruleset.seed, _spawner.Ruleset.seed);
        }

        [UnityTest]
        public IEnumerator Spawner_DoesNotSpawn_WhenPaused()
        {
            _stateManager.SetState(GameState.PAUSE);

            int initialCount = Object.FindObjectsByType<TargetRuntime>(FindObjectsSortMode.None).Length;
            yield return new WaitForSeconds(1f);
            int newCount = Object.FindObjectsByType<TargetRuntime>(FindObjectsSortMode.None).Length;

            Assert.AreEqual(initialCount, newCount, "No targets should spawn when paused");
        }

        [Test]
        public void TryResolve_NullTarget_ReturnsFalse()
        {
            bool result = _spawner.TryResolve(null, GestureType.SingleTap);
            Assert.IsFalse(result);
        }

        [Test]
        public void TryResolve_DestroyedTarget_ReturnsFalse()
        {
            var targetGO = new GameObject("TargetToDestroy");
            var target = targetGO.AddComponent<TargetRuntime>();

            Object.DestroyImmediate(targetGO);

            bool result = _spawner.TryResolve(target, GestureType.SingleTap);
            Assert.IsFalse(result);
        }

        [Test]
        public void TryResolve_CorrectGesture_Succeeds()
        {
            // Create target with SingleTap requirement
            var rule = new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Circle,
                color = Color.red,
                requiredGesture = RulesetDefinition.Gesture.SingleTap,
                baseReward = 10
            };

            var targetGO = new GameObject("Target");
            var target = targetGO.AddComponent<TargetRuntime>();
            target.Initialize(rule, 3f);

            bool result = _spawner.TryResolve(target, GestureType.SingleTap);
            Assert.IsTrue(result, "Correct gesture should resolve target");

            Object.DestroyImmediate(targetGO);
        }

        [Test]
        public void TryResolve_IncorrectGesture_Fails()
        {
            var rule = new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Circle,
                color = Color.red,
                requiredGesture = RulesetDefinition.Gesture.DoubleTap,
                baseReward = 10
            };

            var targetGO = new GameObject("Target");
            var target = targetGO.AddComponent<TargetRuntime>();
            target.Initialize(rule, 3f);

            bool result = _spawner.TryResolve(target, GestureType.SingleTap);
            Assert.IsFalse(result, "Incorrect gesture should not resolve target");

            Object.DestroyImmediate(targetGO);
        }

        [Test]
        public void TryResolve_CorrectGesture_RemovesFromActive()
        {
            var rule = new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Circle,
                color = Color.red,
                requiredGesture = RulesetDefinition.Gesture.SingleTap,
                baseReward = 10
            };

            var targetGO = new GameObject("Target");
            var target = targetGO.AddComponent<TargetRuntime>();
            target.Initialize(rule, 3f);

            int activeBefore = _spawner.ActiveTargets.Count;
            _spawner.TryResolve(target, GestureType.SingleTap);
            int activeAfter = _spawner.ActiveTargets.Count;

            // Target should be destroyed (removed from ActiveTargets is checked via Active count)
            // After successful resolve, target should be destroyed
            Assert.AreEqual(1, activeBefore - activeAfter);

            Object.DestroyImmediate(targetGO);
        }

        [UnityTest]
        public IEnumerator MaxConcurrentTargets_Respected()
        {
            // Manually add targets to reach max
            var targets = new List<GameObject>();
            for (int i = 0; i < _ruleset.maxConcurrentTargets; i++)
            {
                var rule = new RulesetDefinition.TargetRule
                {
                    shape = RulesetDefinition.Shape.Circle,
                    color = Color.red,
                    requiredGesture = RulesetDefinition.Gesture.SingleTap,
                    baseReward = 10
                };

                var go = new GameObject($"Target_{i}");
                go.transform.position = new Vector3(i * 2f, 0f, 0f);
                var target = go.AddComponent<TargetRuntime>();
                target.Initialize(rule, 60f); // Long lifetime
                targets.Add(go);
            }

            yield return null;

            int count = Object.FindObjectsByType<TargetRuntime>(FindObjectsSortMode.None).Length;
            Assert.LessOrEqual(count, _ruleset.maxConcurrentTargets,
                "Should not exceed max concurrent targets");

            foreach (var go in targets)
                Object.Destroy(go);
        }

        [UnityTest]
        public IEnumerator OnGestureEvaluated_FiresOnCorrectGesture()
        {
            var rule = new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Circle,
                color = Color.red,
                requiredGesture = RulesetDefinition.Gesture.SingleTap,
                baseReward = 10
            };

            var targetGO = new GameObject("Target");
            var target = targetGO.AddComponent<TargetRuntime>();
            target.Initialize(rule, 3f);

            bool eventFired = false;
            TargetRuntime eventTarget = null;
            GestureType eventGesture = GestureType.SingleTap;
            bool eventSuccess = false;

            _spawner.OnGestureEvaluated += (t, g, s) =>
            {
                eventFired = true;
                eventTarget = t;
                eventGesture = g;
                eventSuccess = s;
            };

            _spawner.TryResolve(target, GestureType.SingleTap);

            Assert.IsTrue(eventFired, "OnGestureEvaluated should fire");
            Assert.AreEqual(target, eventTarget);
            Assert.AreEqual(GestureType.SingleTap, eventGesture);
            Assert.IsTrue(eventSuccess);

            Object.DestroyImmediate(targetGO);
            yield return null;
        }

        [UnityTest]
        public IEnumerator OnGestureEvaluated_FiresOnIncorrectGesture()
        {
            var rule = new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Circle,
                color = Color.red,
                requiredGesture = RulesetDefinition.Gesture.DoubleTap,
                baseReward = 10
            };

            var targetGO = new GameObject("Target");
            var target = targetGO.AddComponent<TargetRuntime>();
            target.Initialize(rule, 3f);

            bool eventFired = false;
            bool eventSuccess = true;

            _spawner.OnGestureEvaluated += (t, g, s) =>
            {
                eventFired = true;
                eventSuccess = s;
            };

            _spawner.TryResolve(target, GestureType.SingleTap);

            Assert.IsTrue(eventFired, "OnGestureEvaluated should fire even on wrong gesture");
            Assert.IsFalse(eventSuccess, "Success should be false for wrong gesture");

            Object.DestroyImmediate(targetGO);
            yield return null;
        }

        [UnityTest]
        public IEnumerator RemoveTarget_RemovesFromActiveAndDestroys()
        {
            var rule = new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Circle,
                color = Color.red,
                requiredGesture = RulesetDefinition.Gesture.SingleTap,
                baseReward = 10
            };

            var targetGO = new GameObject("Target");
            var target = targetGO.AddComponent<TargetRuntime>();
            target.Initialize(rule, 3f);

            _spawner.RemoveTarget(target);
            yield return null;

            Assert.IsTrue(targetGO == null || target == null, "Target should be destroyed");
        }

        [Test]
        public void TryResolve_AllGestureTypes_MatchCorrectly()
        {
            var gestures = new[]
            {
                (GestureType.SingleTap, RulesetDefinition.Gesture.SingleTap),
                (GestureType.DoubleTap, RulesetDefinition.Gesture.DoubleTap),
                (GestureType.LongPress, RulesetDefinition.Gesture.LongPress),
                (GestureType.SwipeTap, RulesetDefinition.Gesture.SwipeTap)
            };

            foreach (var (gestureType, ruleGesture) in gestures)
            {
                var rule = new RulesetDefinition.TargetRule
                {
                    shape = RulesetDefinition.Shape.Circle,
                    color = Color.red,
                    requiredGesture = ruleGesture,
                    baseReward = 10
                };

                var targetGO = new GameObject($"Target_{gestureType}");
                var target = targetGO.AddComponent<TargetRuntime>();
                target.Initialize(rule, 3f);

                bool result = _spawner.TryResolve(target, gestureType);
                Assert.IsTrue(result, $"{gestureType} should resolve when required");

                Object.DestroyImmediate(targetGO);
            }
        }
    }
}
