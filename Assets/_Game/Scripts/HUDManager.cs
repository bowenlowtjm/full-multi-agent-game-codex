using UnityEngine;

namespace Pully.Game
{
    public class HUDManager : MonoBehaviour
    {
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private GameStateManager stateManager;

        private GUIStyle _valueStyle;
        private GUIStyle _titleStyle;
        private GUIStyle _toastStyle;
        private Texture2D _panelTex;

        private float _toastUntil;
        private int _toastPoints;

        private static readonly Color Sky = new(112f / 255f, 197f / 255f, 206f / 255f, 0.88f);
        private static readonly Color Ink = new(47f / 255f, 79f / 255f, 79f / 255f);
        private static readonly Color Banana = new(247f / 255f, 226f / 255f, 107f / 255f);

        private void Awake()
        {
            if (scoreManager == null) scoreManager = FindFirstObjectByType<ScoreManager>();
            if (stateManager == null) stateManager = FindFirstObjectByType<GameStateManager>();

            _panelTex = new Texture2D(1, 1);
            _panelTex.SetPixel(0, 0, Sky);
            _panelTex.Apply();
        }

        private void Update()
        {
            if (scoreManager == null) scoreManager = FindFirstObjectByType<ScoreManager>();
            if (stateManager == null) stateManager = FindFirstObjectByType<GameStateManager>();

            if (scoreManager == null) return;
            if (scoreManager.LastAwardedPoints <= 0) return;
            if (scoreManager.LastAwardedPoints == _toastPoints && Time.time < _toastUntil) return;

            _toastPoints = scoreManager.LastAwardedPoints;
            _toastUntil = Time.time + 0.75f;
        }

        private void OnGUI()
        {
            if (scoreManager == null || stateManager == null) return;

            EnsureGuiStyles();
            DrawPanel();

            GUI.Label(new Rect(24, 16, 140, 24), "SCORE", _titleStyle);
            GUI.Label(new Rect(24, 34, 180, 34), scoreManager.Score.ToString(), _valueStyle);

            GUI.Label(new Rect(210, 16, 140, 24), "COMBO", _titleStyle);
            GUI.Label(new Rect(210, 34, 180, 34), $"x{scoreManager.Combo:F1}", _valueStyle);

            GUI.Label(new Rect(390, 16, 140, 24), "LIVES", _titleStyle);
            GUI.Label(new Rect(390, 34, 180, 34), $"{scoreManager.Lives}", _valueStyle);

            GUI.Label(new Rect(550, 16, 140, 24), "TIME", _titleStyle);
            GUI.Label(new Rect(550, 34, 180, 34), $"{stateManager.RemainingSeconds:F1}", _valueStyle);

            if (Time.time < _toastUntil)
            {
                GUI.Label(new Rect(Screen.width - 220, 22, 200, 36), $"+{_toastPoints}", _toastStyle);
            }
        }

        private void EnsureGuiStyles()
        {
            if (_titleStyle == null)
            {
                _titleStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 18,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = Ink }
                };
            }

            if (_valueStyle == null)
            {
                _valueStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 26,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = Ink }
                };
            }

            if (_toastStyle == null)
            {
                _toastStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 20,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleRight,
                    normal = { textColor = Banana }
                };
            }
        }

        private void DrawPanel()
        {
            if (_panelTex == null)
            {
                _panelTex = new Texture2D(1, 1);
                _panelTex.SetPixel(0, 0, Sky);
                _panelTex.Apply();
            }

            GUI.DrawTexture(new Rect(12, 10, Screen.width - 24, 72), _panelTex);
            GUI.Box(new Rect(12, 10, Screen.width - 24, 72), GUIContent.none);
        }
    }
}
