using UnityEngine;

namespace Pully.Game
{
    public class ScoreManager : MonoBehaviour
    {
        public int Score { get; private set; }
        public float Combo { get; private set; } = ScoreCalculator.StartingCombo;
        public int Lives { get; private set; }

        private RulesetDefinition _ruleset;

        public void Configure(RulesetDefinition ruleset)
        {
            _ruleset = ruleset;
            Lives = ruleset.lives;
            ResetSession();
        }

        public void ResetSession()
        {
            Score = 0;
            Combo = ScoreCalculator.StartingCombo;
            if (_ruleset != null) Lives = _ruleset.lives;
        }

        public void RegisterHit(int baseReward)
        {
            Score += ScoreCalculator.ScoreFor(baseReward, Combo);
            Combo = ScoreCalculator.NextCombo(Combo, _ruleset.comboStep, _ruleset.comboCap);
        }

        public void RegisterMiss()
        {
            Combo = ScoreCalculator.StartingCombo;
            Lives = Mathf.Max(0, Lives - 1);
        }
    }
}
