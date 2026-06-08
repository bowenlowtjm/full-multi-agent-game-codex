using System;
using UnityEngine;

namespace Pully.Game
{
    public enum GameState
    {
        INIT,
        MAIN_MENU,
        GAMEPLAY,
        PAUSE,
        GAME_OVER
    }

    public class GameStateManager : MonoBehaviour
    {
        public GameState CurrentState { get; private set; } = GameState.INIT;
        public float RemainingSeconds { get; private set; }

        public event Action<GameState, GameState> OnStateChanged;
        public event Action OnRoundEnded;

        public void SetState(GameState next)
        {
            if (CurrentState == next) return;
            var old = CurrentState;
            CurrentState = next;
            OnStateChanged?.Invoke(next, old);
        }

        public void StartRound(float durationSeconds)
        {
            RemainingSeconds = Mathf.Max(1f, durationSeconds);
            SetState(GameState.GAMEPLAY);
        }

        private void Update()
        {
            if (CurrentState != GameState.GAMEPLAY) return;
            RemainingSeconds -= Time.deltaTime;
            if (RemainingSeconds <= 0f)
            {
                RemainingSeconds = 0f;
                SetState(GameState.GAME_OVER);
                OnRoundEnded?.Invoke();
            }
        }
    }
}
