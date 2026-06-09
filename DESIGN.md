# DESIGN.md — Pully art taste memory

The Game Art agent reads this before generating and updates it as the style locks. Goal: visual consistency across sprites, UI, and FX — and across runs.

## Theme & personality (make it charming, not abstract)
- **Theme:** Critters (L3 run default) — each shape is a little creature with a reacting face.
- **Personality cues:** squash on hit, idle blink, panic-wobble before expiry; combo state increases glow + SFX pitch.
- **Reference games for *feel*** (see `spec/GAME-SPEC.md`): Bop It (action-on-cue), Fruit Ninja (swipe juice), Piano Tiles/Beatstar (tension), Whack-a-Mole (pop feedback).

## Art style
- **Style:** Flappy-Bird-like 2D arcade style (clean cartoon silhouettes, bold outlines, simple shapes, bright palette).
- **Mood:** cheerful, bouncy, highly legible, retro-mobile arcade.
- **Pixels-per-unit / resolution target:** 100 PPU for gameplay sprites and UI icons; crisp edges and strong silhouette readability.

## Palette (lock these hex values)
| Role | Hex | Used for |
|------|-----|----------|
| Background | #70C5CE | play-field sky tone |
| Accent / UI | #F7E26B | buttons, HUD highlights |
| Circle-Green target | #7ED957 | single-tap |
| Circle-Red target | #FF5A5F | long-press trap |
| Square-Blue target | #4D96FF | double-tap |
| Triangle-Yellow target | #FFD93D | swipe-tap |
| Star-Purple target | #A66CFF | two-finger |

## Readability rules (these are scored — see spec/ACCEPTANCE.md)
- Each shape distinguishable by **silhouette AND color** — never color alone (red/green colorblind safe).
- Consistent stroke/weight across all targets.
- HUD legible over the busiest play-field state.

## Asset inventory (check off as produced)
- [x] 5 target fallback sprites (procedural per shape/color ruleset in runtime)
- [x] Hit/miss feedback pass (audio + haptics + score toast UI)
- [x] UI polish pass: menu/game controls + HUD skin updated to Flappy-style palette
- [ ] Packed atlas (no import warnings)

## Decisions log (style)
- 2026-06-09 — Selected Critters + flat-vector to maximize mobile readability and unblock early gameplay while art agent iterates atlas assets.
- 2026-06-09 — User requested Flappy-Bird-like art direction; updated style/mood/palette to bright sky, bold outlines, simple cartoon readability.
