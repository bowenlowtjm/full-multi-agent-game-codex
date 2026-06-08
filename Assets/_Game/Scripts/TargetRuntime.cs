using System;
using UnityEngine;

namespace Pully.Game
{
    public class TargetRuntime : MonoBehaviour
    {
        public RulesetDefinition.TargetRule rule;
        public float remainingLifetime;

        public event Action<TargetRuntime> OnExpired;

        public void Initialize(RulesetDefinition.TargetRule targetRule, float lifetimeSeconds)
        {
            rule = targetRule;
            remainingLifetime = lifetimeSeconds;
            var sr = GetComponent<SpriteRenderer>();
            if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();
            sr.color = targetRule.color;
            name = $"Target_{targetRule.shape}_{targetRule.requiredGesture}";
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
