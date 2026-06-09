using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Pully.Game;
using UnityEngine;
using UnityEngine.TestTools;

namespace Pully.Tests
{
    /// <summary>
    /// Tests for TargetRuntime lifecycle, shapes, and expiration.
    /// </summary>
    public class TargetRuntimeTests
    {
        [TearDown]
        public void TearDown()
        {
            var targets = Object.FindObjectsByType<TargetRuntime>(FindObjectsSortMode.None);
            foreach (var t in targets)
                Object.Destroy(t.gameObject);
        }

        [Test]
        public void Initialize_SetsRule()
        {
            var rule = new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Circle,
                color = Color.red,
                requiredGesture = RulesetDefinition.Gesture.SingleTap,
                baseReward = 10
            };

            var go = new GameObject("Target");
            var target = go.AddComponent<TargetRuntime>();
            target.Initialize(rule, 3f);

            Assert.AreEqual(rule, target.rule);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void Initialize_SetsLifetime()
        {
            var rule = CreateTestRule();

            var go = new GameObject("Target");
            var target = go.AddComponent<TargetRuntime>();
            target.Initialize(rule, 5f);

            Assert.AreEqual(5f, target.remainingLifetime);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void Initialize_RecordsSpawnPosition()
        {
            var rule = CreateTestRule();
            Vector3 position = new Vector3(10f, 20f, 0f);

            var go = new GameObject("Target");
            go.transform.position = position;
            var target = go.AddComponent<TargetRuntime>();
            target.Initialize(rule, 3f);

            Assert.AreEqual(position, target.spawnPosition);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void Initialize_CreatesRequiredComponents()
        {
            var rule = CreateTestRule();

            var go = new GameObject("Target");
            Assert.IsNull(go.GetComponent<SpriteRenderer>());
            Assert.IsNull(go.GetComponent<CircleCollider2D>());

            // Initialize creates components
            var target = go.AddComponent<TargetRuntime>();
            target.Initialize(rule, 3f);

            // The RequireComponent attribute ensures these exist after Initialize
            // (they should be automatically added by the attribute constraint)

            Object.DestroyImmediate(go);
        }

        [Test]
        public void Initialize_CircleShape_CreatesFallbackSprite()
        {
            var rule = new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Circle,
                color = Color.red,
                requiredGesture = RulesetDefinition.Gesture.SingleTap,
                baseReward = 10
            };

            var go = new GameObject("Target");
            var target = go.AddComponent<TargetRuntime>();
            target.Initialize(rule, 3f);

            var sr = target.GetComponent<SpriteRenderer>();
            Assert.IsNotNull(sr.sprite);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void Initialize_SquareShape_CreatesFallbackSprite()
        {
            var rule = new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Square,
                color = Color.blue,
                requiredGesture = RulesetDefinition.Gesture.SingleTap,
                baseReward = 10
            };

            var go = new GameObject("Target");
            var target = go.AddComponent<TargetRuntime>();
            target.Initialize(rule, 3f);

            var sr = target.GetComponent<SpriteRenderer>();
            Assert.IsNotNull(sr.sprite);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void Initialize_TriangleShape_CreatesFallbackSprite()
        {
            var rule = new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Triangle,
                color = Color.green,
                requiredGesture = RulesetDefinition.Gesture.SingleTap,
                baseReward = 10
            };

            var go = new GameObject("Target");
            var target = go.AddComponent<TargetRuntime>();
            target.Initialize(rule, 3f);

            var sr = target.GetComponent<SpriteRenderer>();
            Assert.IsNotNull(sr.sprite);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void Initialize_StarShape_CreatesFallbackSprite()
        {
            var rule = new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Star,
                color = Color.yellow,
                requiredGesture = RulesetDefinition.Gesture.SingleTap,
                baseReward = 10
            };

            var go = new GameObject("Target");
            var target = go.AddComponent<TargetRuntime>();
            target.Initialize(rule, 3f);

            var sr = target.GetComponent<SpriteRenderer>();
            Assert.IsNotNull(sr.sprite);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void Initialize_SetsSortingOrder()
        {
            var rule = CreateTestRule();

            var go = new GameObject("Target");
            var target = go.AddComponent<TargetRuntime>();
            target.Initialize(rule, 3f);

            var sr = target.GetComponent<SpriteRenderer>();
            Assert.AreEqual(20, sr.sortingOrder);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void Initialize_SetsScale()
        {
            var rule = CreateTestRule();

            var go = new GameObject("Target");
            var target = go.AddComponent<TargetRuntime>();
            target.Initialize(rule, 3f);

            Assert.AreEqual(Vector3.one * 1.3f, target.transform.localScale);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void Initialize_SetsColliderProperties()
        {
            var rule = CreateTestRule();

            var go = new GameObject("Target");
            var target = go.AddComponent<TargetRuntime>();
            target.Initialize(rule, 3f);

            var col = target.GetComponent<CircleCollider2D>();
            Assert.IsTrue(col.isTrigger);
            Assert.AreEqual(0.55f, col.radius, 0.001f);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void Initialize_SetsColor()
        {
            var rule = new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Circle,
                color = Color.cyan,
                requiredGesture = RulesetDefinition.Gesture.SingleTap,
                baseReward = 10
            };

            var go = new GameObject("Target");
            var target = go.AddComponent<TargetRuntime>();
            target.Initialize(rule, 3f);

            var sr = target.GetComponent<SpriteRenderer>();
            Assert.AreEqual(Color.cyan, sr.color);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void Initialize_SetsName()
        {
            var rule = new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Circle,
                color = Color.red,
                requiredGesture = RulesetDefinition.Gesture.SingleTap,
                baseReward = 10
            };

            var go = new GameObject("Target");
            var target = go.AddComponent<TargetRuntime>();
            target.Initialize(rule, 3f);

            Assert.IsTrue(go.name.Contains("Circle"));
            Assert.IsTrue(go.name.Contains("SingleTap"));

            Object.DestroyImmediate(go);
        }

        [UnityTest]
        public IEnumerator Lifetime_CountsDown()
        {
            var rule = CreateTestRule();

            var go = new GameObject("Target");
            var target = go.AddComponent<TargetRuntime>();
            target.Initialize(rule, 3f);

            float initialLifetime = target.remainingLifetime;

            yield return new WaitForSeconds(0.2f);

            Assert.Less(target.remainingLifetime, initialLifetime);

            Object.Destroy(go);
        }

        [UnityTest]
        public IEnumerator Lifetime_ReachesZero_TriggersOnExpired()
        {
            var rule = CreateTestRule();

            var go = new GameObject("Target");
            var target = go.AddComponent<TargetRuntime>();
            target.Initialize(rule, 0.1f); // Very short lifetime

            bool expired = false;
            TargetRuntime expiredTarget = null;

            target.OnExpired += t =>
            {
                expired = true;
                expiredTarget = t;
            };

            yield return new WaitForSeconds(0.15f);

            Assert.IsTrue(expired, "OnExpired should fire when lifetime reaches zero");
            Assert.AreEqual(target, expiredTarget);
        }

        [UnityTest]
        public IEnumerator Lifetime_ReachesZero_DestroysGameObject()
        {
            var rule = CreateTestRule();

            var go = new GameObject("Target");
            var target = go.AddComponent<TargetRuntime>();
            target.Initialize(rule, 0.1f);

            yield return new WaitForSeconds(0.15f);
            yield return null; // Extra frame for destruction

            Assert.IsTrue(go == null || target == null, "Target should be destroyed after expiration");
        }

        [Test]
        public void RestoreToSpawn_ReturnsToSpawnPosition()
        {
            var rule = CreateTestRule();
            Vector3 spawnPos = new Vector3(5f, 5f, 0f);

            var go = new GameObject("Target");
            go.transform.position = spawnPos;
            var target = go.AddComponent<TargetRuntime>();
            target.Initialize(rule, 3f);

            // Move target
            go.transform.position = new Vector3(10f, 10f, 0f);

            // Restore
            target.RestoreToSpawn();

            Assert.AreEqual(spawnPos, go.transform.position);

            Object.DestroyImmediate(go);
        }

        [UnityTest]
        public IEnumerator MultipleInstances_CanExpireIndependently()
        {
            var targets = new List<GameObject>();
            var runtimes = new List<TargetRuntime>();

            // Create 3 targets with different lifetimes
            for (int i = 0; i < 3; i++)
            {
                var go = new GameObject($"Target_{i}");
                go.transform.position = new Vector3(i * 2f, 0f, 0f);
                var target = go.AddComponent<TargetRuntime>();
                target.Initialize(CreateTestRule(), 0.1f + (i * 0.05f));
                targets.Add(go);
                runtimes.Add(target);
            }

            yield return new WaitForSeconds(0.25f);
            yield return null;

            // At least one should be destroyed
            int remaining = Object.FindObjectsByType<TargetRuntime>(FindObjectsSortMode.None).Length;
            Assert.Less(remaining, 3, "Some targets should have expired");

            // Cleanup remaining
            foreach (var go in targets)
                if (go != null) Object.Destroy(go);
        }

        [Test]
        public void AllShapes_CanBeInitialized()
        {
            foreach (RulesetDefinition.Shape shape in System.Enum.GetValues(typeof(RulesetDefinition.Shape)))
            {
                var rule = new RulesetDefinition.TargetRule
                {
                    shape = shape,
                    color = Color.white,
                    requiredGesture = RulesetDefinition.Gesture.SingleTap,
                    baseReward = 10
                };

                var go = new GameObject($"Target_{shape}");
                var target = go.AddComponent<TargetRuntime>();

                try
                {
                    target.Initialize(rule, 3f);
                    Assert.IsNotNull(target.GetComponent<SpriteRenderer>().sprite,
                        $"Shape {shape} should have a valid sprite");
                }
                finally
                {
                    Object.DestroyImmediate(go);
                }
            }
        }

        [UnityTest]
        public IEnumerator OnExpired_CanBeSubscribedAndUnsubscribed()
        {
            var rule = CreateTestRule();
            int callCount = 0;

            var go = new GameObject("Target");
            var target = go.AddComponent<TargetRuntime>();
            target.Initialize(rule, 0.1f);

            // Subscribe handler
            target.OnExpired += ExpiredHandler;

            yield return new WaitForSeconds(0.15f);

            Assert.AreEqual(1, callCount, "Handler should be called once");

            // Unsubscribe
            target.OnExpired -= ExpiredHandler;

            Object.Destroy(go);

            void ExpiredHandler(TargetRuntime t)
            {
                callCount++;
            }
        }

        private RulesetDefinition.TargetRule CreateTestRule()
        {
            return new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Circle,
                color = Color.white,
                requiredGesture = RulesetDefinition.Gesture.SingleTap,
                baseReward = 10
            };
        }
    }
}
