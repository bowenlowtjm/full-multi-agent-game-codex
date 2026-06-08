---
id: T003
title: Core loop foundations (state, spawning, gesture, scoring)
status: done
milestone: M1
owner: game-logic
depends_on: [T002]
branch: feature/T003-core-loop
---

## Goal
Implement functional first slice of gameplay: target spawn, gesture classification, ruleset-driven validation, score/combo/lives update.

## Acceptance
- [ ] Gameplay loop runs for 60s timed round with lives mode fallback per spec.
- [ ] RulesetDefinition drives gesture mapping and rewards (no hardcoding).
- [ ] Wrong/miss/expiry resets combo and applies life penalty.
- [ ] EditMode tests cover scoring/combo + basic gesture classification.

## Notes
- Keep deterministic seed path from `RulesetDefinition.seed`.
- Align with PRD target matrix while preserving authoritative GAME-SPEC 5-gesture system.
