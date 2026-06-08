using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pully.Game
{
    public class SpawnerManager : MonoBehaviour
    {
        [SerializeField] private RulesetDefinition ruleset;
        [SerializeField] private Rect safeZoneNormalized = new Rect(0.05f, 0.10f, 0.90f, 0.75f);
        [SerializeField] private float minSeparationPixels = 80f;

        private readonly List<TargetRuntime> _active = new();
        private System.Random _rng;
        private Camera _cam;
        private float _spawnInterval;
        private float _spawnTimer;
        private float _elapsed;

        public IReadOnlyList<TargetRuntime> ActiveTargets => _active;
        public RulesetDefinition Ruleset => ruleset;

        public event Action<TargetRuntime, GestureType, bool> OnGestureEvaluated;
        public event Action<TargetRuntime> OnTargetExpired;

        public void Configure(RulesetDefinition definition, Camera cam)
        {
            ruleset = definition;
            _cam = cam != null ? cam : Camera.main;
            _rng = new System.Random(ruleset.seed);
            _spawnInterval = ruleset.spawnIntervalStart;
            _spawnTimer = 0f;
            _elapsed = 0f;
        }

        private void Update()
        {
            if (ruleset == null || _cam == null) return;

            _elapsed += Time.deltaTime;
            _spawnTimer += Time.deltaTime;

            float t = Mathf.Clamp01(_elapsed / Mathf.Max(0.01f, ruleset.roundSeconds));
            _spawnInterval = Mathf.Lerp(ruleset.spawnIntervalStart, ruleset.spawnIntervalEnd, t);

            if (_spawnTimer >= _spawnInterval && _active.Count < ruleset.maxConcurrentTargets)
            {
                _spawnTimer = 0f;
                TrySpawnOne();
            }
        }

        private void TrySpawnOne()
        {
            if (ruleset.rules == null || ruleset.rules.Count == 0) return;

            var rule = ruleset.rules[_rng.Next(0, ruleset.rules.Count)];
            for (int i = 0; i < 20; i++)
            {
                var screenPos = new Vector2(
                    Mathf.Lerp(Screen.width * safeZoneNormalized.xMin, Screen.width * safeZoneNormalized.xMax, (float)_rng.NextDouble()),
                    Mathf.Lerp(Screen.height * safeZoneNormalized.yMin, Screen.height * safeZoneNormalized.yMax, (float)_rng.NextDouble()));

                if (!IsFarEnough(screenPos)) continue;

                var world = _cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Mathf.Abs(_cam.transform.position.z)));
                world.z = 0f;

                var go = new GameObject("Target");
                go.transform.position = world;
                var target = go.AddComponent<TargetRuntime>();
                target.Initialize(rule, ruleset.targetLifetime);
                target.OnExpired += HandleExpired;
                _active.Add(target);
                return;
            }
        }

        private bool IsFarEnough(Vector2 screenPos)
        {
            foreach (var t in _active)
            {
                if (t == null) continue;
                var sp = _cam.WorldToScreenPoint(t.transform.position);
                if (Vector2.Distance(screenPos, sp) < minSeparationPixels) return false;
            }
            return true;
        }

        private void HandleExpired(TargetRuntime target)
        {
            _active.Remove(target);
            OnTargetExpired?.Invoke(target);
        }

        public bool TryResolve(TargetRuntime target, GestureType gesture)
        {
            if (target == null) return false;
            bool success = target.rule.requiredGesture == (RulesetDefinition.Gesture)gesture;
            OnGestureEvaluated?.Invoke(target, gesture, success);
            if (success)
            {
                _active.Remove(target);
                Destroy(target.gameObject);
            }
            return success;
        }

        public void RemoveTarget(TargetRuntime target)
        {
            if (target == null) return;
            _active.Remove(target);
            Destroy(target.gameObject);
        }
    }
}
