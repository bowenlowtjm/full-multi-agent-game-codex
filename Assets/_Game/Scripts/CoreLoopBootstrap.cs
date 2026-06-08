using UnityEngine;

namespace Pully.Game
{
    public class CoreLoopBootstrap : MonoBehaviour
    {
        [SerializeField] private RulesetDefinition ruleset;

        private ScoreManager _score;
        private SpawnerManager _spawner;
        private GameStateManager _state;

        private void Awake()
        {
            if (ruleset == null)
            {
                ruleset = ScriptableObject.CreateInstance<RulesetDefinition>();
                ruleset.rules.Add(new RulesetDefinition.TargetRule
                {
                    shape = RulesetDefinition.Shape.Circle,
                    color = Color.green,
                    requiredGesture = RulesetDefinition.Gesture.SingleTap,
                    baseReward = 1
                });
                ruleset.rules.Add(new RulesetDefinition.TargetRule
                {
                    shape = RulesetDefinition.Shape.Circle,
                    color = Color.red,
                    requiredGesture = RulesetDefinition.Gesture.LongPress,
                    baseReward = 5
                });
            }

            _score = gameObject.AddComponent<ScoreManager>();
            _state = gameObject.AddComponent<GameStateManager>();
            _spawner = gameObject.AddComponent<SpawnerManager>();

            _score.Configure(ruleset);
            _spawner.Configure(ruleset, Camera.main);
        }

        private void Start()
        {
            _state.StartRound(ruleset.roundSeconds);
            _state.OnRoundEnded += () => Debug.Log($"[CoreLoop] Round ended. score={_score.Score}, lives={_score.Lives}");
            _spawner.OnGestureEvaluated += OnGestureEvaluated;
        }

        private void OnGestureEvaluated(TargetRuntime target, GestureType gesture)
        {
            bool success = target != null && target.rule.requiredGesture == (RulesetDefinition.Gesture)gesture;
            if (success) _score.RegisterHit(target.rule.baseReward);
            else _score.RegisterMiss();
        }
    }
}
