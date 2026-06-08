# Art Execution Plan (M2-M4)

Scope: planning only. No binary assets generated in this phase.

Inputs aligned:
- `DESIGN.md` (Critters theme, flat-vector style, locked palette, readability rules, 100 PPU)
- `spec/GAME-SPEC.md` (M2 placeholders -> generated sprites + atlas, M4 release polish)
- `spec/RULESET.md` (5 shape/color gameplay targets)
- `spec/ACCEPTANCE.md` (gates and verification artifacts)
- `08-Roadmap.md` + `RUN-PROTOCOL.md` (M2/M3/M4 milestone expectations)

## 1) Asset list by milestone (M2-M4)

## M2 — Playable build + art pass (must replace gameplay placeholders)
Gameplay targets (ruleset-driven):
1. Circle-Green (single tap) — critter variant A
2. Circle-Red (long press trap) — critter variant B, danger expression
3. Square-Blue (double tap)
4. Triangle-Yellow (swipe-tap)
5. Star-Purple (two-finger tap)

Target state variants (minimum to support feel + readability):
- Idle (base)
- Hit (squash/pop frame)
- Expiry warning (panic/wobble pre-expiry)

FX sprites (first pass juice):
- Hit-pop burst (small/medium)
- Miss-flash overlay
- Score-popup burst glyphs (simple)

UI sprites (minimum M2):
- HUD frame/backplate
- Buttons: Play, Retry, Menu (normal + pressed)
- Pause icon button (normal + pressed)
- Lives icon (3-step readability-safe heart/pip style)

Deliverable intent at M2 exit:
- No primitive gameplay placeholders remain in active game scene.
- Atlas packs cleanly, import settings consistent.

## M3 — Balance + robustness support assets
Readability/tuning support variants:
- High-clarity outline variants for all 5 targets (thicker stroke option)
- Colorblind assist alternates (pattern overlays per target family)
- Optional low-clutter FX variants for performance/readability tests

Instrumentation/debug art for deterministic review:
- Gesture hint icons (tap/double/hold/swipe/two-finger) for tutorial/testing overlays
- Spawn warning ring sprite for timing readability checks

Deliverable intent at M3 exit:
- Asset set supports readability tuning and seeded test comparisons without code-side asset churn.

## M4 — Release polish assets
Onboarding + full flow:
- Splash art background + logo lockup
- Tutorial panels/cards for 5 gestures (skippable flow)
- Settings icons (audio/music/haptics/colorblind/tutorial replay)
- Pause panel art framing
- Game-over panel + new-high-score celebratory badge/sting visual

Polish pass variants:
- Button transition states (hover/focus where applicable, pressed, disabled)
- Combo escalation visuals (x2/x3/x4/x5 intensity accents)
- Refined FX set (hit/miss/combo milestones) with consistent style language

Branding:
- App icon source set (foreground mark + background variants)

Deliverable intent at M4 exit:
- No programmer-art placeholders anywhere in shipped flow.
- Visual consistency across splash -> menu -> settings -> game -> pause -> game over.

## 2) Sprite atlas plan

Atlas strategy (Unity Sprite Atlas, under `Assets/_Game/Sprites/`):
- `GameplayTargetsAtlas` (primary runtime-critical)
  - All target sprites + target state variants + core hit/miss FX
- `UIFlowAtlas`
  - Menu/settings/pause/game-over/tutorial/button assets
- `BrandingAtlas`
  - Splash/logo/app-icon-source support sprites (runtime if needed)

Packing + import rules:
- Power-of-two atlas sizes (start 2048, scale by usage)
- Padding: 4-8 px to avoid bleeding on scaled UI
- 100 PPU baseline per `DESIGN.md`
- Compression/profile tuned per atlas type (gameplay clarity first)
- Naming convention: `spr_<domain>_<asset>_<state>`
  - examples: `spr_target_circle_green_idle`, `spr_ui_btn_play_pressed`

Versioning approach:
- Keep stable sprite names once wired to prefabs to avoid scene churn.
- Add variant suffixes instead of replacing names during M3 tuning (`_cb`, `_hc`, `_perf`).

## 3) Placeholder replacement sequence

Phase order is designed to keep gameplay unblocked while reducing rework:
1. Lock style + naming in `DESIGN.md` and this plan.
2. Replace gameplay-critical placeholders first (5 targets + idle/hit/expiry states).
3. Wire core FX placeholders -> first-pass hit/miss FX (M2 juice requirement).
4. Replace HUD/buttons used in main play loop (Play/Retry/Menu/Pause/HUD/lives).
5. Add tutorial/settings/pause/game-over visual set (M4 flow completeness).
6. Swap splash/app-icon assets after full-flow UI is stable.
7. Run final placeholder sweep across all scenes/prefabs and remove unused placeholder sprites.

Dependency notes:
- Steps 2-4 are M2 gate-critical.
- Step 5+ are M4 release-polish-critical.
- M3 variants should be additive, not disruptive to existing prefab links.

## 4) Colorblind and readability checks

Mandatory checks (run each asset batch):
1. Silhouette-first recognition:
   - Each target identifiable in grayscale and at small HUD-scale preview.
2. Red/green safety:
   - Circle-Green vs Circle-Red differentiated by face expression + internal motif + warning ring behavior, not color alone.
3. Contrast checks:
   - Targets and HUD remain legible over `#1B1F3B` background and during FX-heavy moments.
4. Stroke consistency:
   - Uniform outline weight family across all targets/UI icons.
5. Busy-state HUD readability:
   - Score/combo/lives/timer remain readable during max concurrent targets + hit/miss FX.

Implementation hooks:
- Maintain optional colorblind mode variants (`_cb`) with pattern-coded interiors:
  - Green circle: dot motif
  - Red circle: hazard stripe motif
  - Square blue: grid motif
  - Triangle yellow: chevron motif
  - Star purple: radial motif

Pass criteria:
- New player can map target class without relying solely on hue.
- No target confusion in simulated colorblind previews and grayscale checks.

## 5) Acceptance checks mapped to ACCEPTANCE gates

Art-specific acceptance mapping:

1) Gate: "Game Art pass done: generated 2D sprites + packed atlas, palette consistent with DESIGN.md"
- Check: all M2 target/UI/FX sprites exist and are not primitives.
- Check: atlases present and assigned; no import warnings.
- Evidence: atlas asset list + before/after scene capture + palette conformance note.

2) Gate: "Release-quality polish met" (includes no placeholders, icon/splash, transitions, complete UX)
- Check: M4 assets exist for splash/tutorial/settings/pause/game-over and are wired.
- Check: no placeholder art in any player-facing screen.
- Evidence: full flow capture (onboarding -> menu -> settings -> pause -> game -> game-over), icon screenshot.

3) Gate: "Full screen flow exists and navigates"
- Check: every flow screen has final art pass (not wireframe placeholders).
- Evidence: navigation capture with final UI skin.

4) Gate: "Gameplay-quality bar readability/finish"
- Check: silhouette + color accessibility checks passed.
- Check: HUD readable in busiest state.
- Evidence: readability checklist output + grayscale/colorblind snapshots.

5) Gate: "Self-report honesty"
- Check: any missing art replacements or readability regressions logged explicitly in run-log.
- Evidence: run-log entry with outstanding items (if any).

## 6) Definition of done for this planning task

This document is complete when:
- Asset plan is decomposed for M2/M3/M4.
- Atlas architecture is fixed enough for scene integration.
- Replacement sequence is dependency-aware and milestone-aligned.
- Colorblind/readability checks are explicit and testable.
- Acceptance linkage is traceable to `spec/ACCEPTANCE.md` gates.
