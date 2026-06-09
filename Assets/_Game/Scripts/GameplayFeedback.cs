using UnityEngine;

namespace Pully.Game
{
    /// <summary>
    /// Connects gameplay events (hit/miss/combo) to audio and haptics feedback.
    /// Attach to CoreLoopBootstrap or game controller.
    /// </summary>
    public class GameplayFeedback : MonoBehaviour
    {
        private ScoreManager _score;
        private float _lastCombo = 1f;

        public void Configure(ScoreManager score)
        {
            _score = score;
        }

        private void Update()
        {
            if (_score == null) return;

            // Detect combo changes
            if (_score.Combo > _lastCombo && _lastCombo >= 1f)
            {
                OnComboIncreased();
            }
            _lastCombo = _score.Combo;
        }

        public void OnHit(int points)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayHit();
            }
            if (HapticsManager.Instance != null)
            {
                HapticsManager.Instance.HitFeedback();
            }
        }

        public void OnMiss()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayMiss();
            }
            if (HapticsManager.Instance != null)
            {
                HapticsManager.Instance.MissFeedback();
            }
            _lastCombo = 1f;
        }

        private void OnComboIncreased()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCombo();
            }
            if (HapticsManager.Instance != null)
            {
                HapticsManager.Instance.ComboFeedback();
            }
        }

        public void OnGameOver()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopMusic();
                AudioManager.Instance.PlayGameOver();
            }
            if (HapticsManager.Instance != null)
            {
                HapticsManager.Instance.GameOverFeedback();
            }
        }

        public void OnHighScore()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayHighScore();
            }
        }
    }
}
