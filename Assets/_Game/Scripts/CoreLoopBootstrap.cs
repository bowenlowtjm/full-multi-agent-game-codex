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
            _spawner.Configure(ruleset, Camera.main, _state);
            _input.Configure(_spawner, _state);
        }

        private void Start()
        {
            _state.OnRoundEnded += () => Debug.Log($"[CoreLoop] Round ended. score={_score.Score}, lives={_score.Lives}");
            _spawner.OnGestureEvaluated += OnGestureEvaluated;
            _spawner.OnTargetExpired += _ => OnFailure();
        }

        public void BeginGameplay()
        {
            _score.ResetSession();
            _state.StartRound(ruleset.roundSeconds);
        }

        public void PauseGameplay()
        {
            _state.SetState(GameState.PAUSE);
        }

        public void ResumeGameplay()
        {
            _state.SetState(GameState.GAMEPLAY);
        }

        private void OnGestureEvaluated(TargetRuntime target, GestureType gesture, bool success)
        {
            if (success)
            {
                _score.RegisterHit(target.rule.baseReward);
                if (AudioManager.Instance != null) AudioManager.Instance.PlayHit();
                HapticsManager.Hit();
            }
            else
            {
                if ((RulesetDefinition.Gesture)gesture == RulesetDefinition.Gesture.SwipeTap)
                {
                    target.RestoreToSpawn();
                }
                if (AudioManager.Instance != null) AudioManager.Instance.PlayMiss();
                HapticsManager.Miss();
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
            if (AudioManager.Instance != null) AudioManager.Instance.PlayMiss();
            HapticsManager.Miss();
            if (_score.Lives <= 0)
            {
                _state.SetState(GameState.GAME_OVER);
            }
        }

        private static void EnsureMainCamera()
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                var go = new GameObject("Main Camera");
                go.tag = "MainCamera";
                cam = go.AddComponent<Camera>();
                cam.orthographic = true;
                cam.transform.position = new Vector3(0f, 0f, -10f);
            }

            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(112f / 255f, 197f / 255f, 206f / 255f); // #70C5CE sky tone

            if (cam.GetComponent<AudioListener>() == null)
            {
                cam.gameObject.AddComponent<AudioListener>();
            }
        }

        private static RulesetDefinition BuildDefaultRuleset()
        {
            var rs = ScriptableObject.CreateInstance<RulesetDefinition>();
            rs.rules.Clear();
            rs.rules.Add(new RulesetDefinition.TargetRule { shape = RulesetDefinition.Shape.Circle, color = new Color(126f / 255f, 217f / 255f, 87f / 255f), requiredGesture = RulesetDefinition.Gesture.SingleTap, baseReward = 1 });    // #7ED957
            rs.rules.Add(new RulesetDefinition.TargetRule { shape = RulesetDefinition.Shape.Circle, color = new Color(1f, 90f / 255f, 95f / 255f), requiredGesture = RulesetDefinition.Gesture.LongPress, baseReward = 5 });          // #FF5A5F
            rs.rules.Add(new RulesetDefinition.TargetRule { shape = RulesetDefinition.Shape.Square, color = new Color(77f / 255f, 150f / 255f, 1f), requiredGesture = RulesetDefinition.Gesture.DoubleTap, baseReward = 3 });      // #4D96FF
            rs.rules.Add(new RulesetDefinition.TargetRule { shape = RulesetDefinition.Shape.Triangle, color = new Color(1f, 217f / 255f, 61f / 255f), requiredGesture = RulesetDefinition.Gesture.SwipeTap, baseReward = 5 });    // #FFD93D
            rs.rules.Add(new RulesetDefinition.TargetRule { shape = RulesetDefinition.Shape.Star, color = new Color(166f / 255f, 108f / 255f, 1f), requiredGesture = RulesetDefinition.Gesture.TwoFingerTap, baseReward = 8 });    // #A66CFF
            return rs;
        }
    }
}
