using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Pully.Game;
using UnityEngine;
using UnityEngine.TestTools;

namespace Pully.Tests
{
    /// <summary>
    /// Determinism verification: same seed must produce identical results.
    /// </summary>
    public class DeterminismTests
    {
        [UnityTest]
        public IEnumerator SameSeed_ProducesIdenticalTargetSequence()
        {
            const int seed = 12345;
            const int spawnCount = 10;

            // Run 1
            var sequence1 = new List<string>();
            var runner1 = CreateSeededRunner(seed);
            yield return CollectSpawnSequence(runner1, sequence1, spawnCount);

            // Run 2 with same seed
            var sequence2 = new List<string>();
            var runner2 = CreateSeededRunner(seed);
            yield return CollectSpawnSequence(runner2, sequence2, spawnCount);

            // Verify identical sequences
            Assert.AreEqual(sequence1.Count, sequence2.Count);
            for (int i = 0; i < sequence1.Count; i++)
            {
                Assert.AreEqual(sequence1[i], sequence2[i], 
                    $"Mismatch at index {i}: {sequence1[i]} vs {sequence2[i]}");
            }

            Cleanup(runner1);
            Cleanup(runner2);
        }

        [UnityTest]
        public IEnumerator DifferentSeeds_ProduceDifferentSequences()
        {
            const int spawnCount = 5;

            // Run 1
            var sequence1 = new List<string>();
            var runner1 = CreateSeededRunner(11111);
            yield return CollectSpawnSequence(runner1, sequence1, spawnCount);

            // Run 2 with different seed
            var sequence2 = new List<string>();
            var runner2 = CreateSeededRunner(99999);
            yield return CollectSpawnSequence(runner2, sequence2, spawnCount);

            // Verify sequences are different (with high probability)
            bool anyDifferent = false;
            for (int i = 0; i < Mathf.Min(sequence1.Count, sequence2.Count); i++)
            {
                if (sequence1[i] != sequence2[i])
                {
                    anyDifferent = true;
                    break;
                }
            }
            Assert.IsTrue(anyDifferent, "Different seeds should produce different sequences");

            Cleanup(runner1);
            Cleanup(runner2);
        }

        private GameObject CreateSeededRunner(int seed)
        {
            var go = new GameObject($"DeterminismRunner_{seed}");
            var bootstrap = go.AddComponent<CoreLoopBootstrap>();
            
            // Create ruleset with specific seed
            var ruleset = ScriptableObject.CreateInstance<RulesetDefinition>();
            ruleset.seed = seed;
            ruleset.lives = 10;
            ruleset.roundSeconds = 60f;
            ruleset.targetLifetime = 2f;
            ruleset.spawnIntervalStart = 1f;
            ruleset.spawnIntervalEnd = 0.5f;
            ruleset.maxConcurrentTargets = 4;
            
            // Add at least one rule
            ruleset.rules.Add(new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Circle,
                color = Color.red,
                requiredGesture = RulesetDefinition.Gesture.SingleTap,
                baseReward = 10
            });

            // Inject ruleset (via reflection or make Configure public)
            var spawner = go.AddComponent<SpawnerManager>();
            spawner.Configure(ruleset, null);

            return go;
        }

        private IEnumerator CollectSpawnSequence(GameObject runner, List<string> sequence, int count)
        {
            var spawner = runner.GetComponent<SpawnerManager>();
            float timer = 0f;
            float timeout = 30f;

            while (sequence.Count < count && timer < timeout)
            {
                // Check for new spawns
                var targets = Object.FindObjectsByType<TargetRuntime>(FindObjectsSortMode.None);
                foreach (var t in targets)
                {
                    string key = $"{t.rule.shape}:{t.rule.color}:{Time.frameCount}";
                    if (!sequence.Contains(key))
                    {
                        sequence.Add(key);
                    }
                }

                timer += Time.deltaTime;
                yield return null;
            }
        }

        private void Cleanup(GameObject runner)
        {
            if (runner != null)
            {
                Object.Destroy(runner);
            }
        }
    }
}
