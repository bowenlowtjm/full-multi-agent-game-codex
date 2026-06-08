using System;
using UnityEngine;

namespace Pully.Game
{
    [RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D))]
    public class TargetRuntime : MonoBehaviour
    {
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
            sr.color = targetRule.color;

            var col = GetComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.55f;

            name = $"Target_{targetRule.shape}_{targetRule.requiredGesture}";
        }

        public void RestoreToSpawn()
        {
            transform.position = spawnPosition;
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
