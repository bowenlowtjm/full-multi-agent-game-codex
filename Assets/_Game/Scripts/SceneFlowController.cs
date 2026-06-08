using System.Collections;
using UnityEngine;

namespace Pully.Game
{
    public class SceneFlowController : MonoBehaviour
    {
        [SerializeField] private CoreLoopBootstrap core;
        [SerializeField] private GameStateManager state;

        private IEnumerator Start()
        {
            if (core == null) core = FindFirstObjectByType<CoreLoopBootstrap>();
            if (state == null) state = FindFirstObjectByType<GameStateManager>();
            if (core == null || state == null) yield break;

            state.SetState(GameState.INIT);
            yield return new WaitForSeconds(0.25f); // splash
            state.SetState(GameState.MAIN_MENU);    // menu/how-to placeholder
            yield return new WaitForSeconds(0.25f);
            core.BeginGameplay();
        }

        private void Update()
        {
            if (core == null || state == null) return;

            if (state.CurrentState == GameState.GAMEPLAY && Input.GetKeyDown(KeyCode.Escape))
            {
                core.PauseGameplay();
            }
            else if (state.CurrentState == GameState.PAUSE && Input.GetKeyDown(KeyCode.Return))
            {
                core.ResumeGameplay();
            }
            else if (state.CurrentState == GameState.GAME_OVER && Input.GetKeyDown(KeyCode.Space))
            {
                core.BeginGameplay();
            }
        }
    }
}
