# ADR 001: Core Loop Architecture

## Status
Accepted

## Context
Pully needed a deterministic, testable core loop for a one-thumb arcade game. The architecture needed to support:
- Data-driven rulesets (shape/color → gesture mapping)
- Seeded RNG for reproducible test runs
- Clean separation between input, spawning, scoring, and state
- PlayMode testability

## Decision
We adopted a component-based architecture with ScriptableObject rulesets:

1. **RulesetDefinition**: ScriptableObject holding all game parameters (mapping, timing, scoring, seed)
2. **SpawnerManager**: Handles target lifecycle with deterministic seeded RNG
3. **InputManager**: Touch/mouse abstraction, gesture classification
4. **ScoreManager**: Score/combo/lives tracking
5. **GameStateManager**: State machine for MENU → GAMEPLAY → PAUSE → GAME_OVER flow
6. **CoreLoopBootstrap**: Composition root wiring all components

## Consequences

### Positive
- Ruleset changes require no code edits (data-driven)
- Seeded runs enable bot testing and replay validation
- Clear boundaries enable unit testing per component
- IMGUI-based UI allowed rapid iteration without asset dependencies

### Negative
- IMGUI is immediate-mode and less performant than UI Toolkit/uGUI for complex UIs
- No visual editor for spawn zones (normalized rect in code)
- Component dependencies require careful initialization order

## Alternatives Considered
- **ECS (Entity Component System)**: Rejected for v1 complexity; would improve spawn throughput but overkill for max 4 concurrent targets
- **Visual scripting**: Rejected to maintain deterministic, version-controlled logic
- **uGUI prefab-based UI**: Would require asset generation pipeline; deferred to post-MVP

## Related
- spec/RULESET.md
- Assets/_Game/Scripts/CoreLoopBootstrap.cs
