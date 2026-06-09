using System;
using UnityEngine;

namespace Pully.Game
{
    [RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D))]
    public class TargetRuntime : MonoBehaviour
    {
        private static Sprite _fallbackSprite;

        public RulesetDefinition.TargetRule rule;
        public float remainingLifetime;
        public Vector3 spawnPosition;

        public event Action<TargetRuntime> OnExpired;

        public void Initialize(RulesetDefinition.TargetRule targetRule, float lifetimeSeconds)
        {
            rule = targetRule;
            remainingLifetime = lifetimeSeconds;
            spawnPosition = transform.position;

            var sr = GetComponent<SpriteRenderer>();
            sr.sprite = GetFallbackSprite();
            sr.color = targetRule.color;
            sr.sortingOrder = 20;
            transform.localScale = Vector3.one * 1.3f;

            var col = GetComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.55f;

            name = $"Target_{targetRule.shape}_{targetRule.requiredGesture}";
        }

        public void RestoreToSpawn()
        {
            transform.position = spawnPosition;
        }

        private static Sprite GetFallbackSprite()
        {
            if (_fallbackSprite != null) return _fallbackSprite;

            const int size = 64;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;

            float r = size * 0.5f;
            Vector2 c = new Vector2(r, r);
            var pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), c);
                    pixels[y * size + x] = d <= r ? Color.white : Color.clear;
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();

            _fallbackSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 64f);
            _fallbackSprite.name = "TargetFallbackSprite";
            return _fallbackSprite;
        }

        private void Update()
        {
            remainingLifetime -= Time.deltaTime;
            if (remainingLifetime <= 0f)
            {
                OnExpired?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }
}
