using UnityEngine;

namespace Pully.Game
{
    public class HUDManager : MonoBehaviour
    {
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private GameStateManager stateManager;

        private GUIStyle _label;

        private void Awake()
        {
            if (scoreManager == null) scoreManager = FindFirstObjectByType<ScoreManager>();
            if (stateManager == null) stateManager = FindFirstObjectByType<GameStateManager>();
            _label = new GUIStyle { fontSize = 24, normal = { textColor = Color.white } };
        }

        private void OnGUI()
        {
            if (scoreManager == null || stateManager == null) return;

            GUI.Label(new Rect(20, 20, 400, 40), $"Score: {scoreManager.Score}", _label);
            GUI.Label(new Rect(20, 55, 400, 40), $"Combo: x{scoreManager.Combo:F1}", _label);
            GUI.Label(new Rect(20, 90, 400, 40), $"Lives: {scoreManager.Lives}", _label);
            GUI.Label(new Rect(20, 125, 400, 40), $"Time: {stateManager.RemainingSeconds:F1}", _label);
            GUI.Label(new Rect(20, 160, 600, 40), $"State: {stateManager.CurrentState}", _label);
        }
    }
}
