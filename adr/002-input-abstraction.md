# ADR 002: Input Abstraction

## Status
Accepted

## Context
The game must work on both mobile (touch) and PC (mouse) for development and testing. We needed to support 5 distinct gestures with clear classification logic.

## Decision
Implemented unified input handling with PC-to-mobile mapping:

| Mobile | PC |
|--------|-----|
| Single tap | Left mouse click |
| Double tap | Double-click |
| Long press | Hold mouse button |
| Swipe-tap | Drag + release |
| Two-finger tap | *Not fully implemented on PC* |

**Key implementation choices:**
1. `ProcessMouse()` maps mouse states to touch ID 0
2. Double-tap detection uses time + position window (0.3s, 20px drift)
3. Long-press threshold: 0.5s
4. Swipe threshold: 10px drag distance

## Consequences

### Positive
- Single code path for input handling
- Editor testing with mouse matches mobile behavior
- Gesture classification centralized in InputManager

### Negative
- Two-finger tap has no PC equivalent (requires key modifier or unimplemented)
- Double-tap timing window is fixed (could be data-driven)
- No multi-touch gesture combinations (pinch, rotate)

## Related
- Assets/_Game/Scripts/InputManager.cs
- spec/GAME-SPEC.md gesture vocabulary
