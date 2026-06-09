---
id: T009
title: Full screen flow implementation (splash/how-to/menu/settings/game/pause/game-over/menu)
status: done
milestone: M2
owner: unity-scene
depends_on: [T004]
branch: feature/T009-screen-flow
---

## Goal
Implement the complete navigable screen flow required by acceptance: splash -> tutorial -> menu -> settings -> game -> pause -> game-over -> menu.

## Acceptance
- [x] Scene assets created for all required screens.
- [x] Scene transitions wired through controllers.
- [x] Build Settings scene list populated in required order.
- [x] Compile check clean after scene/controller generation.

## Notes
- Scenes were generated in batchmode via `BuildTools.GenerateCoreScenes`.
- Runtime UI currently uses IMGUI placeholders and will be polished in later M2/M4 tasks.
