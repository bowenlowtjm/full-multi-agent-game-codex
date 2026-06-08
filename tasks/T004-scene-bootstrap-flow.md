---
id: T004
title: Scene + bootstrap flow (menu/game/pause/gameover wiring)
status: done
milestone: M1
owner: unity-scene
depends_on: [T002]
branch: feature/T004-scene-flow
---

## Goal
Create runtime scene/bootstrap wiring for game state flow and HUD essentials so core loop can be played and tested.

## Acceptance
- [ ] State machine includes INIT, MAIN_MENU, GAMEPLAY, PAUSE, GAME_OVER.
- [ ] Play/Pause/Retry flow can be triggered and observed in Editor.
- [ ] Build Settings scene list is populated for local build entrypoint.
- [ ] PlayMode smoke evolves to basic flow verification.

## Notes
- Keep setup code deterministic and simple; no art dependency for M1.
