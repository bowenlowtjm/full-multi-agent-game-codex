using UnityEngine;

namespace Pully.Game
{
    public static class UISkin
    {
        private static Texture2D _skyTex;
        private static Texture2D _panelTex;
        private static Texture2D _buttonTex;

        private static GUIStyle _title;
        private static GUIStyle _subtitle;
        private static GUIStyle _body;
        private static GUIStyle _button;
        private static GUIStyle _chip;

        private static readonly Color Sky = new(112f / 255f, 197f / 255f, 206f / 255f, 1f);      // #70C5CE
        private static readonly Color Panel = new(1f, 1f, 1f, 0.88f);
        private static readonly Color Button = new(247f / 255f, 226f / 255f, 107f / 255f, 0.98f); // #F7E26B
        private static readonly Color Ink = new(47f / 255f, 79f / 255f, 79f / 255f);

        public static void DrawBackground()
        {
            Ensure();
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _skyTex);
        }

        public static Rect DrawCard(float top = 70f, float horizontalMargin = 32f, float minHeight = 220f)
        {
            Ensure();
            float width = Screen.width - horizontalMargin * 2f;
            float height = Mathf.Max(minHeight, Screen.height - top - 40f);
            var rect = new Rect(horizontalMargin, top, width, height);
            GUI.DrawTexture(rect, _panelTex);
            GUI.Box(rect, GUIContent.none);
            return rect;
        }

        public static GUIStyle TitleStyle { get { Ensure(); return _title; } }
        public static GUIStyle SubtitleStyle { get { Ensure(); return _subtitle; } }
        public static GUIStyle BodyStyle { get { Ensure(); return _body; } }
        public static GUIStyle ButtonStyle { get { Ensure(); return _button; } }
        public static GUIStyle ChipStyle { get { Ensure(); return _chip; } }

        private static void Ensure()
        {
            if (_skyTex == null)
            {
                _skyTex = MakeTex(Sky);
                _panelTex = MakeTex(Panel);
                _buttonTex = MakeTex(Button);
            }

            if (_title == null)
            {
                _title = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 42,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = Ink }
                };
            }

            if (_subtitle == null)
            {
                _subtitle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 26,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = Ink }
                };
            }

            if (_body == null)
            {
                _body = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 20,
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true,
                    normal = { textColor = Ink }
                };
            }

            if (_button == null)
            {
                _button = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 22,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = Ink, background = _buttonTex },
                    hover = { textColor = Ink, background = _buttonTex },
                    active = { textColor = Ink, background = _buttonTex }
                };
            }

            if (_chip == null)
            {
                _chip = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 16,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = Ink }
                };
            }
        }

        private static Texture2D MakeTex(Color c)
        {
            var t = new Texture2D(1, 1);
            t.SetPixel(0, 0, c);
            t.Apply();
            return t;
        }
    }
}
