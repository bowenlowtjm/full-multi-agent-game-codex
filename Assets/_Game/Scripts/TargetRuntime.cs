using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pully.Game
{
    [RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D))]
    public class TargetRuntime : MonoBehaviour
    {
        private static readonly Dictionary<RulesetDefinition.Shape, Sprite> FallbackSprites = new();

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
            sr.sprite = GetFallbackSprite(targetRule.shape);
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

        private static Sprite GetFallbackSprite(RulesetDefinition.Shape shape)
        {
            if (FallbackSprites.TryGetValue(shape, out var sprite) && sprite != null)
            {
                return sprite;
            }

            const int size = 64;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point
            };

            var pixels = new Color[size * size];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.clear;

            float half = size * 0.5f;
            Vector2 center = new Vector2(half, half);
            float radius = size * 0.43f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    var p = new Vector2(x + 0.5f, y + 0.5f);
                    bool filled = shape switch
                    {
                        RulesetDefinition.Shape.Circle => Vector2.Distance(p, center) <= radius,
                        RulesetDefinition.Shape.Square => Mathf.Abs(p.x - center.x) <= radius * 0.9f && Mathf.Abs(p.y - center.y) <= radius * 0.9f,
                        RulesetDefinition.Shape.Triangle => InTriangle(p,
                            new Vector2(center.x, center.y + radius),
                            new Vector2(center.x - radius, center.y - radius),
                            new Vector2(center.x + radius, center.y - radius)),
                        RulesetDefinition.Shape.Star => InStar(p, center, radius, radius * 0.45f, 5),
                        _ => Vector2.Distance(p, center) <= radius,
                    };

                    if (filled)
                    {
                        pixels[y * size + x] = Color.white;
                    }
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();

            sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 64f);
            sprite.name = $"TargetFallback_{shape}";
            FallbackSprites[shape] = sprite;
            return sprite;
        }

        private static bool InTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            float s1 = Sign(p, a, b);
            float s2 = Sign(p, b, c);
            float s3 = Sign(p, c, a);
            bool hasNeg = (s1 < 0) || (s2 < 0) || (s3 < 0);
            bool hasPos = (s1 > 0) || (s2 > 0) || (s3 > 0);
            return !(hasNeg && hasPos);
        }

        private static bool InStar(Vector2 p, Vector2 center, float outerR, float innerR, int points)
        {
            float angle = Mathf.Atan2(p.y - center.y, p.x - center.x);
            float dist = Vector2.Distance(p, center);
            float sector = Mathf.PI / points;
            float local = Mathf.Repeat(angle + Mathf.PI, 2f * sector);
            float t = Mathf.Abs(local - sector) / sector;
            float boundary = Mathf.Lerp(innerR, outerR, 1f - t);
            return dist <= boundary;
        }

        private static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
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
