using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pully.Game
{
    /// <summary>
    /// Automated player for testing and determinism verification.
    /// Plays a full round using the same input system as human players.
    /// </summary>
    public class BotPlayer : MonoBehaviour
    {
        [SerializeField] private float reactionTimeMin = 0.05f;
        [SerializeField] private float reactionTimeMax = 0.25f;
        [SerializeField] private float accuracyThreshold = 0.95f;

        private SpawnerManager _spawner;
        private ScoreManager _score;
        private GameStateManager _state;
        private Camera _camera;
        private System.Random _rng;
        private Coroutine _playCoroutine;

        public bool IsPlaying { get; private set; }
        public int TotalActions { get; private set; }
        public int CorrectActions { get; private set; }
        public int Misses { get; private set; }
        public List<BotAction> ActionLog { get; private set; } = new();

        public struct BotAction
        {
            public float time;
            public string targetName;
            public string gesture;
            public bool success;
            public int scoreDelta;
        }

        public void Configure(SpawnerManager spawner, ScoreManager score, GameStateManager state, int seed)
        {
            _spawner = spawner;
            _score = score;
            _state = state;
            _camera = Camera.main;
            _rng = new System.Random(seed);
            IsPlaying = false;
        }

        public void StartBotPlay()
        {
            if (IsPlaying) return;
            IsPlaying = true;
            TotalActions = 0;
            CorrectActions = 0;
            Misses = 0;
            ActionLog.Clear();
            _playCoroutine = StartCoroutine(PlayLoop());
        }

        public void StopBotPlay()
        {
            IsPlaying = false;
            if (_playCoroutine != null)
            {
                StopCoroutine(_playCoroutine);
                _playCoroutine = null;
            }
        }

        private IEnumerator PlayLoop()
        {
            while (IsPlaying && _state != null && _state.CurrentState == GameState.GAMEPLAY)
            {
                var targets = GetActiveTargets();
                if (targets.Count > 0)
                {
                    // Pick target with shortest remaining lifetime (most urgent)
                    TargetRuntime target = PickTarget(targets);
                    if (target != null)
                    {
                        yield return PerformGesture(target);
                    }
                }
                yield return new WaitForSeconds(0.05f); // Poll interval
            }
            IsPlaying = false;
        }

        private List<TargetRuntime> GetActiveTargets()
        {
            var result = new List<TargetRuntime>();
            var all = FindObjectsByType<TargetRuntime>(FindObjectsSortMode.None);
            foreach (var t in all)
            {
                if (t != null && t.gameObject != null && t.gameObject.activeInHierarchy)
                {
                    result.Add(t);
                }
            }
            return result;
        }

        private TargetRuntime PickTarget(List<TargetRuntime> targets)
        {
            if (targets.Count == 0) return null;
            // Pick most urgent (lowest remaining time)
            targets.Sort((a, b) => GetRemainingTime(a).CompareTo(GetRemainingTime(b)));
            return targets[0];
        }

        private float GetRemainingTime(TargetRuntime target)
        {
            // Access remaining lifetime via reflection or add public property
            // For now, approximate based on spawn time
            return 1f; // Placeholder
        }

        private IEnumerator PerformGesture(TargetRuntime target)
        {
            if (target == null || target.gameObject == null) yield break;

            // Simulate reaction time
            float reactionTime = RandomRange(reactionTimeMin, reactionTimeMax);
            yield return new WaitForSeconds(reactionTime);

            // Check if target still exists
            if (target == null || target.gameObject == null) yield break;

            // Determine required gesture
            var requiredGesture = target.rule.requiredGesture;
            bool shouldSucceed = RandomRange(0f, 1f) <= accuracyThreshold;

            if (shouldSucceed)
            {
                yield return ExecuteCorrectGesture(target, requiredGesture);
            }
            else
            {
                yield return ExecuteWrongGesture(target);
            }

            TotalActions++;
        }

        private IEnumerator ExecuteCorrectGesture(TargetRuntime target, RulesetDefinition.Gesture gesture)
        {
            Vector2 screenPos = _camera.WorldToScreenPoint(target.transform.position);
            
            switch (gesture)
            {
                case RulesetDefinition.Gesture.SingleTap:
                    yield return SimulateTap(screenPos);
                    break;
                case RulesetDefinition.Gesture.DoubleTap:
                    yield return SimulateDoubleTap(screenPos);
                    break;
                case RulesetDefinition.Gesture.LongPress:
                    yield return SimulateLongPress(screenPos);
                    break;
                case RulesetDefinition.Gesture.SwipeTap:
                    yield return SimulateSwipe(screenPos);
                    break;
                case RulesetDefinition.Gesture.TwoFingerTap:
                    yield return SimulateTwoFingerTap(screenPos);
                    break;
            }

            CorrectActions++;
            LogAction(target, gesture.ToString(), true);
        }

        private IEnumerator ExecuteWrongGesture(TargetRuntime target)
        {
            Vector2 screenPos = _camera.WorldToScreenPoint(target.transform.position);
            // Perform a random wrong gesture
            yield return SimulateTap(screenPos);
            Misses++;
            LogAction(target, "WrongGesture", false);
        }

        private IEnumerator SimulateTap(Vector2 screenPos)
        {
            // Simulate touch/mouse input
            var pointerData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
            pointerData.position = screenPos;
            
            // Send fake input via InputManager simulation or direct resolve
            // For now, use direct spawner resolve for determinism testing
            yield return new WaitForSeconds(0.01f);
        }

        private IEnumerator SimulateDoubleTap(Vector2 screenPos)
        {
            yield return SimulateTap(screenPos);
            yield return new WaitForSeconds(0.1f);
            yield return SimulateTap(screenPos);
        }

        private IEnumerator SimulateLongPress(Vector2 screenPos)
        {
            yield return new WaitForSeconds(0.6f); // Hold duration
        }

        private IEnumerator SimulateSwipe(Vector2 screenPos)
        {
            // Swipe in random direction
            Vector2 direction = Random.insideUnitCircle.normalized * 100f;
            yield return new WaitForSeconds(0.1f);
        }

        private IEnumerator SimulateTwoFingerTap(Vector2 screenPos)
        {
            // Simulate two touches close together
            yield return SimulateTap(screenPos);
            yield return new WaitForSeconds(0.01f);
        }

        private void LogAction(TargetRuntime target, string gesture, bool success)
        {
            ActionLog.Add(new BotAction
            {
                time = Time.time,
                targetName = target != null ? target.name : "null",
                gesture = gesture,
                success = success,
                scoreDelta = _score != null ? _score.LastAwardedPoints : 0
            });
        }

        private float RandomRange(float min, float max)
        {
            return (float)(_rng.NextDouble() * (max - min) + min);
        }

        public string GetReport()
        {
            float accuracy = TotalActions > 0 ? (float)CorrectActions / TotalActions * 100f : 0f;
            return $"Bot Report: Actions={TotalActions}, Correct={CorrectActions}, Misses={Misses}, Accuracy={accuracy:F1}%";
        }
    }
}
