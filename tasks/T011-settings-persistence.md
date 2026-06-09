---
id: T011
title: Settings persistence + audio/haptics toggles integration
status: done
milestone: M2
owner: game-logic
depends_on: [T009]
branch: feature/T011-settings-persistence
---

## Goal
Persist player settings and apply them to runtime systems, including mute/volume + haptics toggles.

## Acceptance
- [x] Settings values persisted via PlayerPrefs.
- [x] Audio manager applies mute/music/sfx preferences at runtime.
- [x] Haptics follows toggle state for hit/miss feedback paths.
- [x] Headless compile check clean.

## Notes
- Runtime audio uses simple AudioSource controls (no external clips yet).
- Haptics currently uses Handheld.Vibrate guard by platform and preference toggle.
