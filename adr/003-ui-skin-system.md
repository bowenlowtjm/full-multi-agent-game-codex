# ADR 003: UI Skin System

## Status
Accepted

## Context
The game has 6 scenes (Splash, Tutorial, Menu, Settings, Game, GameOver) requiring consistent visual styling. We needed rapid iteration without creating UI prefab assets.

## Decision
Created `UISkin` static class providing shared IMGUI styling:

- **Background**: Procedural gradient texture (sky color)
- **Card**: Centered panel with rounded appearance
- **Button**: Custom GUIStyle with hover/active states
- **Text**: Title, body, and accent text styles

**Flappy-Bird palette locked:**
- Sky: #70C5CE
- Accent: #F7E26B
- Target colors: green, red, blue, yellow, purple

## Consequences

### Positive
- Single source of truth for styling
- No prefab/asset dependencies for UI
- Rapid visual iteration via code
- Consistent across all scenes

### Negative
- IMGUI limitations: no true rounded corners (faked with box style)
- No visual editor access for designers
- Font size scaling is manual, not responsive

## Related
- Assets/_Game/Scripts/UISkin.cs
- DESIGN.md
