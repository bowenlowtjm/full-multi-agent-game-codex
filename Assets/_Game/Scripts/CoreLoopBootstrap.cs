using UnityEngine;

namespace Pully.Game
{
    public class CoreLoopBootstrap : MonoBehaviour
    {
        [SerializeField] private RulesetDefinition ruleset;

        private ScoreManager _score;
        private SpawnerManager _spawner;
        private GameStateManager _state;
        private InputManager _input;

        private void Awake()
        {
            EnsureMainCamera();

            if (ruleset == null)
            {
                ruleset = BuildDefaultRuleset();
            }

            _score = gameObject.AddComponent<ScoreManager>();
            _state = gameObject.AddComponent<GameStateManager>();
            _spawner = gameObject.AddComponent<SpawnerManager>();
            _input = gameObject.AddComponent<InputManager>();

            _score.Configure(ruleset);
            _spawner.Configure(ruleset, Camera.main);
            _input.Configure(_spawner);
        }

        private void Start()
        {
            _state.StartRound(ruleset.roundSeconds);
            _state.OnRoundEnded += () => Debug.Log($"[CoreLoop] Round ended. score={_score.Score}, lives={_score.Lives}");
            _spawner.OnGestureEvaluated += OnGestureEvaluated;
            _spawner.OnTargetExpired += _ => OnFailure();
        }

        private void OnGestureEvaluated(TargetRuntime target, GestureType gesture, bool success)
        {
            if (success)
            {
                _score.RegisterHit(target.rule.baseReward);
            }
            else
            {
                if ((RulesetDefinition.Gesture)gesture == RulesetDefinition.Gesture.SwipeTap)
                {
                    target.RestoreToSpawn();
                }
                OnFailure();
            }

            if (_score.Lives <= 0)
            {
                _state.SetState(GameState.GAME_OVER);
            }
        }

        private void OnFailure()
        {
            _score.RegisterMiss();
            if (_score.Lives <= 0)
            {
                _state.SetState(GameState.GAME_OVER);
            }
        }

        private static void EnsureMainCamera()
        {
            if (Camera.main != null) return;
            var go = new GameObject("Main Camera");
            var cam = go.AddComponent<Camera>();
            go.tag = "MainCamera";
            cam.orthographic = true;
            cam.transform.position = new Vector3(0f, 0f, -10f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.11f, 0.12f, 0.23f);
        }

        private static RulesetDefinition BuildDefaultRuleset()
        {
            var rs = ScriptableObject.CreateInstance<RulesetDefinition>();
            rs.rules.Clear();
            rs.rules.Add(new RulesetDefinition.TargetRule { shape = RulesetDefinition.Shape.Circle, color = Color.green, requiredGesture = RulesetDefinition.Gesture.SingleTap, baseReward = 1 });
            rs.rules.Add(new RulesetDefinition.TargetRule { shape = RulesetDefinition.Shape.Circle, color = Color.red, requiredGesture = RulesetDefinition.Gesture.LongPress, baseReward = 5 });
            rs.rules.Add(new RulesetDefinition.TargetRule { shape = RulesetDefinition.Shape.Square, color = Color.blue, requiredGesture = RulesetDefinition.Gesture.DoubleTap, baseReward = 3 });
            rs.rules.Add(new RulesetDefinition.TargetRule { shape = RulesetDefinition.Shape.Triangle, color = Color.yellow, requiredGesture = RulesetDefinition.Gesture.SwipeTap, baseReward = 5 });
            rs.rules.Add(new RulesetDefinition.TargetRule { shape = RulesetDefinition.Shape.Star, color = new Color(0.61f, 0.36f, 0.9f), requiredGesture = RulesetDefinition.Gesture.TwoFingerTap, baseReward = 8 });
            return rs;
        }
    }
}
