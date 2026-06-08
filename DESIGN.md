# DESIGN.md — Pully art taste memory

The Game Art agent reads this before generating and updates it as the style locks. Goal: visual consistency across sprites, UI, and FX — and across runs.

## Theme & personality (make it charming, not abstract)
- **Theme:** Critters (L3 run default) — each shape is a little creature with a reacting face.
- **Personality cues:** squash on hit, idle blink, panic-wobble before expiry; combo state increases glow + SFX pitch.
- **Reference games for *feel*** (see `spec/GAME-SPEC.md`): Bop It (action-on-cue), Fruit Ninja (swipe juice), Piano Tiles/Beatstar (tension), Whack-a-Mole (pop feedback).

## Art style
- **Style:** flat-vector (from run defaults for this L3 run).
- **Mood:** playful, high-contrast, fast-read arcade.
- **Pixels-per-unit / resolution target:** 100 PPU for gameplay sprites and UI icons.

## Palette (lock these hex values)
| Role | Hex | Used for |
|------|-----|----------|
| Background | #1B1F3B | play-field |
| Accent / UI | #FF8C42 | buttons, HUD |
| Circle-Green target | #5CD65C | single-tap |
| Circle-Red target | #FF4B5C | long-press trap |
| Square-Blue target | #4A90E2 | double-tap |
| Triangle-Yellow target | #FFD166 | swipe-tap |
| Star-Purple target | #9B5DE5 | two-finger |

## Readability rules (these are scored — see spec/ACCEPTANCE.md)
- Each shape distinguishable by **silhouette AND color** — never color alone (red/green colorblind safe).
- Consistent stroke/weight across all targets.
- HUD legible over the busiest play-field state.

## Asset inventory (check off as produced)
- [ ] 5 target sprites (one per shape/color in spec/RULESET.md)
- [ ] Hit-pop FX, miss-flash FX
- [ ] UI: Play / Retry / Menu buttons, HUD frame
- [ ] Packed atlas (no import warnings)

## Decisions log (style)
- 2026-06-09 — Selected Critters + flat-vector to maximize mobile readability and unblock early gameplay while art agent iterates atlas assets.
