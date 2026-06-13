using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using InputPhase = UnityEngine.InputSystem.TouchPhase;

namespace Pully.Game
{
    /// <summary>
    /// Automated QA player for runtime verification.
    ///
    /// Default mode is Input: injects real touch events into the Input System and lets
    /// InputManager + gesture recognition resolve targets (input -> recognizer -> resolve).
    ///
    /// LogicOnly mode is an explicit fast bypass for non-input tests. It calls
    /// SpawnerManager.TryResolve directly and does NOT validate recognizer/input parsing.
    /// </summary>
    public class BotPlayer : MonoBehaviour
    {
        public enum BotInputMode
        {
            Input,
            LogicOnly
        }

        [SerializeField] private BotInputMode inputMode = BotInputMode.Input;
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
        public int InputEventsQueued { get; private set; }
        public int DirectResolveCalls { get; private set; }
        public List<BotAction> ActionLog { get; private set; } = new();

        public struct BotAction
        {
            public float time;
            public string targetName;
            public string gesture;
            public bool success;
            public int scoreDelta;
        }

        public void Configure(SpawnerManager spawner, ScoreManager score, GameStateManager state, int seed, BotInputMode mode = BotInputMode.Input)
        {
            _spawner = spawner;
            _score = score;
            _state = state;
            _camera = Camera.main;
            _rng = new System.Random(seed);
            inputMode = mode;
            IsPlaying = false;
        }

        public void SetInputMode(BotInputMode mode)
        {
            inputMode = mode;
        }

        public void StartBotPlay()
        {
            if (IsPlaying) return;
            IsPlaying = true;
            TotalActions = 0;
            CorrectActions = 0;
            Misses = 0;
            InputEventsQueued = 0;
            DirectResolveCalls = 0;
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
                    // Pick the most urgent target (lowest remaining lifetime).
                    TargetRuntime target = PickTarget(targets);
                    if (target != null)
                    {
                        yield return PerformGesture(target);
                    }
                }

                yield return null;
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

            targets.Sort((a, b) => GetRemainingTime(a).CompareTo(GetRemainingTime(b)));
            return targets[0];
        }

        private static float GetRemainingTime(TargetRuntime target)
        {
            return target != null ? target.RemainingLifetime : float.MaxValue;
        }

        private IEnumerator PerformGesture(TargetRuntime target)
        {
            if (target == null || target.gameObject == null || _spawner == null || _state == null) yield break;

            float reactionTime = RandomRange(reactionTimeMin, reactionTimeMax);
            if (reactionTime > 0f)
            {
                yield return new WaitForSeconds(reactionTime);
            }

            if (target == null || target.gameObject == null || _state.CurrentState != GameState.GAMEPLAY) yield break;

            bool shouldSucceed = RandomRange(0f, 1f) <= accuracyThreshold;
            int scoreBefore = _score != null ? _score.Score : 0;
            int livesBefore = _score != null ? _score.Lives : 0;

            if (shouldSucceed)
            {
                yield return ExecuteCorrectGesture(target, target.rule.requiredGesture);
            }
            else
            {
                yield return ExecuteWrongGesture(target);
            }

            int scoreAfter = _score != null ? _score.Score : scoreBefore;
            int livesAfter = _score != null ? _score.Lives : livesBefore;

            bool success = scoreAfter > scoreBefore;
            if (success) CorrectActions++;
            if (livesAfter < livesBefore) Misses++;

            TotalActions++;
        }

        private IEnumerator ExecuteCorrectGesture(TargetRuntime target, RulesetDefinition.Gesture gesture)
        {
            if (inputMode == BotInputMode.LogicOnly)
            {
                var mapped = MapGesture(gesture);
                _ = ResolveDirect(target, mapped);
                LogAction(target, gesture.ToString(), true);
                yield break;
            }

            Vector2 screenPos = _camera.WorldToScreenPoint(target.transform.position);
            yield return SimulateGestureInput(gesture, screenPos);
            LogAction(target, gesture.ToString(), true);
        }

        private IEnumerator ExecuteWrongGesture(TargetRuntime target)
        {
            GestureType required = MapGesture(target.rule.requiredGesture);
            GestureType wrong = ChooseDeterministicWrongGesture(required);

            if (inputMode == BotInputMode.LogicOnly)
            {
                _ = ResolveDirect(target, wrong);
                LogAction(target, "WrongGesture", false);
                yield break;
            }

            Vector2 screenPos = _camera.WorldToScreenPoint(target.transform.position);
            yield return SimulateGestureInput(ToRulesetGesture(wrong), screenPos);
            LogAction(target, "WrongGesture", false);
        }

        private IEnumerator SimulateGestureInput(RulesetDefinition.Gesture gesture, Vector2 screenPos)
        {
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
                default:
                    yield return SimulateTap(screenPos);
                    break;
            }
        }

        private IEnumerator SimulateTap(Vector2 screenPos)
        {
            var ts = EnsureTouchscreen();
            QueueTouch(ts, 1, screenPos, InputPhase.Began);
            InputSystem.Update();
            yield return null;

            float tapHold = Mathf.Min(0.02f, _spawner.Ruleset.doubleTapWindow * 0.25f);
            if (tapHold > 0f) yield return new WaitForSeconds(tapHold);

            QueueTouch(ts, 1, screenPos, InputPhase.Ended);
            InputSystem.Update();
            yield return null;
        }

        private IEnumerator SimulateDoubleTap(Vector2 screenPos)
        {
            yield return SimulateTap(screenPos);

            float gap = Mathf.Max(0.01f, _spawner.Ruleset.doubleTapWindow * 0.3f);
            gap = Mathf.Min(gap, Mathf.Max(0.01f, _spawner.Ruleset.doubleTapWindow - 0.02f));
            yield return new WaitForSeconds(gap);

            yield return SimulateTap(screenPos);
        }

        private IEnumerator SimulateLongPress(Vector2 screenPos)
        {
            var ts = EnsureTouchscreen();
            QueueTouch(ts, 1, screenPos, InputPhase.Began);
            InputSystem.Update();
            yield return null;

            float holdDuration = Mathf.Max(_spawner.Ruleset.longPressDuration + 0.05f, 0.06f);
            yield return new WaitForSeconds(holdDuration);

            QueueTouch(ts, 1, screenPos, InputPhase.Ended);
            InputSystem.Update();
            yield return null;
        }

        private IEnumerator SimulateSwipe(Vector2 screenPos)
        {
            var ts = EnsureTouchscreen();
            float swipeDistance = Mathf.Max(_spawner.Ruleset.swipeMinDistance + 10f, 20f);

            bool horizontal = _rng.NextDouble() < 0.5;
            float direction = _rng.NextDouble() < 0.5 ? -1f : 1f;
            Vector2 endPos = horizontal
                ? screenPos + new Vector2(direction * swipeDistance, 0f)
                : screenPos + new Vector2(0f, direction * swipeDistance);

            QueueTouch(ts, 1, screenPos, InputPhase.Began);
            InputSystem.Update();
            yield return null;

            Vector2 mid = Vector2.Lerp(screenPos, endPos, 0.5f);
            QueueTouch(ts, 1, mid, InputPhase.Moved);
            InputSystem.Update();
            yield return null;

            QueueTouch(ts, 1, endPos, InputPhase.Moved);
            InputSystem.Update();
            yield return null;

            QueueTouch(ts, 1, endPos, InputPhase.Ended);
            InputSystem.Update();
            yield return null;
        }

        private IEnumerator SimulateTwoFingerTap(Vector2 screenPos)
        {
            var ts = EnsureTouchscreen();
            Vector2 offset = new Vector2(18f, 0f);
            Vector2 p1 = screenPos - offset;
            Vector2 p2 = screenPos + offset;

            // Same frame began for both touches so InputManager can detect two-finger begin window.
            QueueTouch(ts, 1, p1, InputPhase.Began);
            QueueTouch(ts, 2, p2, InputPhase.Began);
            InputSystem.Update();
            yield return null;

            float hold = Mathf.Min(0.03f, _spawner.Ruleset.doubleTapWindow * 0.2f);
            if (hold > 0f) yield return new WaitForSeconds(hold);

            QueueTouch(ts, 1, p1, InputPhase.Ended);
            QueueTouch(ts, 2, p2, InputPhase.Ended);
            InputSystem.Update();
            yield return null;
        }

        private static GestureType MapGesture(RulesetDefinition.Gesture gesture)
        {
            return gesture switch
            {
                RulesetDefinition.Gesture.SingleTap => GestureType.SingleTap,
                RulesetDefinition.Gesture.DoubleTap => GestureType.DoubleTap,
                RulesetDefinition.Gesture.LongPress => GestureType.LongPress,
                RulesetDefinition.Gesture.SwipeTap => GestureType.SwipeTap,
                RulesetDefinition.Gesture.TwoFingerTap => GestureType.TwoFingerTap,
                _ => GestureType.SingleTap
            };
        }

        private static RulesetDefinition.Gesture ToRulesetGesture(GestureType gesture)
        {
            return gesture switch
            {
                GestureType.SingleTap => RulesetDefinition.Gesture.SingleTap,
                GestureType.DoubleTap => RulesetDefinition.Gesture.DoubleTap,
                GestureType.LongPress => RulesetDefinition.Gesture.LongPress,
                GestureType.SwipeTap => RulesetDefinition.Gesture.SwipeTap,
                GestureType.TwoFingerTap => RulesetDefinition.Gesture.TwoFingerTap,
                _ => RulesetDefinition.Gesture.SingleTap
            };
        }

        private static GestureType ChooseDeterministicWrongGesture(GestureType required)
        {
            // Ensure wrong gesture still goes through resolver path and can produce a miss.
            if (required != GestureType.SingleTap) return GestureType.SingleTap;
            return GestureType.LongPress;
        }

        private bool ResolveDirect(TargetRuntime target, GestureType gesture)
        {
            if (_spawner == null || target == null || target.gameObject == null) return false;
            DirectResolveCalls++;
            return _spawner.TryResolve(target, gesture);
        }

        private Touchscreen EnsureTouchscreen()
        {
            var ts = Touchscreen.current;
            if (ts == null)
            {
                ts = InputSystem.AddDevice<Touchscreen>();
            }
            return ts;
        }

        private void QueueTouch(Touchscreen ts, int touchId, Vector2 screenPos, InputPhase phase)
        {
            var state = new TouchState
            {
                touchId = touchId,
                phase = phase,
                position = screenPos,
                delta = Vector2.zero,
                pressure = phase == InputPhase.Ended ? 0f : 1f
            };

            InputSystem.QueueStateEvent(ts, state);
            InputEventsQueued++;
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
            return $"Bot Report: Mode={inputMode}, Actions={TotalActions}, Correct={CorrectActions}, Misses={Misses}, Accuracy={accuracy:F1}%, InputEvents={InputEventsQueued}, DirectResolveCalls={DirectResolveCalls}";
        }
    }
}
