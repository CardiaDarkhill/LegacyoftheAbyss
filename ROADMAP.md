# Roadmap — Legacy of the Abyss

Treat this as a living document — update it as work gets done or priorities change. This is a to-do
list, not a changelog: finished work is removed rather than kept as a checked-off entry. Git history
and `REFACTOR_NOTES.md` are the record of what was done and when.

## Status as of this writing

Refactor/split, skin selector, save-data relocation, the Key1-5 keybind fix, the updraft/aggro
region-exclusion fix, the SlideSurface crash fix, the sprite-texture leak fix, the Shade-device
menu/pause input fixes, the sub-menu highlight fix, the Shade charm-tab fixes, and the test-suite
cleanup are all shipped and confirmed. See git log for details on any of these if needed. The suite
currently sits at 125 passing, 0 failing.

## Known bugs — feasibility notes

**1. Enemies don't redirect their attention to the Shade — both halves built, needs a live pass.**
The old note said this meant rewriting target resolution in each of the 184 files referencing
`HeroController.instance`. That was the wrong read: almost none of those are enemies locating the
player, they are things done *to* the hero (damage, cState, invulnerability, input blocking), and not
one of the 53 PlayMaker actions among them is a "find the hero" action. There were two real gaps, and
alerting turned out to matter more than targeting — redirecting an enemy's target is meaningless while
the enemy never leaves its idle state.

*Alerting.* `TrackTriggerObjects` only admits an object into `insideGameObjects` if it passes the
range's `ignoreLayers`/`tagIncludeList`/`tagExcludeList`, which the Shade's aggro proxy does not — so
`InsideCount` read 0, `IsInside` read false and `GetClosestInside` returned null however close the
Shade stood. `AlertRange_FixedUpdate_Patch` already covered the single question
`AlertRange.IsHeroInRange()` answers, which is why registration logged correctly while enemies still
walked straight past. Three patches on the base class, scoped to `AlertRange` (the enemy-alerting
subclass, which inherits rather than overrides these members): `InsideCount`, `GetClosestInside`, and
the three-argument `GetClosestInsideLineOfSight` — the two-argument overload delegates to it, so
patching both would double-run. That covers four of the five FSM actions that consult a range:
`CheckTrackTriggerCount`, `CheckTrackTriggerCountV2` and `GetTrackTriggerCount` go through
`InsideCount`; `GetTrackTriggerClosestObject` goes through `IsInside` and `GetClosestInside`.

*Targeting.* Enemy AI is PlayMaker-driven and the actions that move an enemy toward something are one
tagged set: `ActionCategory("Enemy AI")`, 63 types, of which 36 name their target in a public
`FsmGameObject` field alongside an `FsmOwnerDefault` for the enemy itself — `ChaseObject*`,
`FaceObject*`, `DistanceFly*`, `FireAtTarget`, `GetAngleToTarget2D`. `LegacyHelper.EnemyAiRetargeting`
borrows that field for the duration of each call when the target is Hornet and `ShadeAggroTargeting`
says the Shade is the better one (per-enemy latched decisions, re-examined every 0.75s, 20%
hysteresis, 30-unit cap).

The split is deliberate and worth keeping: alerting answers facts (is the Shade in this range, what is
nearest) with no preference applied, targeting answers policy (who should this enemy chase).
`ModConfig.shadeEnemyTargetingEnabled` turns both off. Tests: `ShadeAggroTargetingTests` for the
comparison, `EnemyAiActionSelectionTests` reflecting over the real game assembly so a game update that
renames the category or changes the field type fails loudly instead of silently patching nothing.

Live pass: with `logShade` on, an enemy the Shade walks up to should leave idle, and
`Shade aggro: '<enemy>' now targeting the Shade` should appear. Known gaps if it needs to go further,
in rough priority order:

- `StoreTrackTriggerListAsArray` is the fifth FSM action and the one not covered — it enumerates
  `InsideGameObjects` directly. That property is the single chokepoint all five ultimately share, so
  patching it instead of the three members above would cover everything; it was not done that way
  because it changes more at once and the three-member version was verifiable against known callers.
  Note the two cannot both be applied — `InsideCount` enumerates `InsideGameObjects`, so patching both
  would double-count.
- Enemies whose AI is hand-written C# rather than FSM-driven (`Walker`, `Chaser` and similar).
- Attacks that resolve `HeroController.instance` at the moment of firing rather than reading a target
  field.
- No aggro stickiness beyond distance, so an enemy will not stay on the Shade because the Shade hit it.

Some enemies do appear to still ignore the shade, whilst others attack it. We'll likely need to do a very
fine-tooth testing run over a full playthrough to work out what's ignoring it and fix those case by case.

## Planned features — feasibility notes

**1. Shade graphical layering** (Easy, mechanical). The Shade's `SpriteRenderer` is always
pinned to `hornetRenderer.sortingOrder + 1` on the same sorting layer as Hornet
(`LegacyHelper.Core.cs` spawn code, and consistently through the old `ShadeController.Core.cs`
— now mostly in `LegacyHelper.ShadeController.Movement.cs` / `.Combat.cs` /
`.FocusAndAudio.cs` after the split). That's why fog/snow/lighting don't interact with it
correctly. No `Light2D` references exist anywhere in the mod. First step: inspect a few
representative scenes' sorting layer setup and Hornet's own renderer relative to weather
effects, then give the Shade its own explicit sorting layer instead of "Hornet's layer + 1",
and add whatever `Light2D`-compatible material Hornet's renderer uses.

**3. Optional gravity** (Easy-Medium). The Shade's `Rigidbody2D` already runs with `gravityScale = 0f`
and fully custom float/knockback code (now in `LegacyHelper.ShadeController.Movement.cs`).
Making gravity optional is close to flipping `gravityScale` based on a config bool and adding
a grounded/fall state to the existing movement state machine — not building physics from
scratch. We will need to update the Shades movement logic here to match with the Knight's 
movement mechanics from the first Hollow Knight game.

**4. Additional shades (up to 3)** — see item 10, "Multiplayer (up to three Shades)", below;
kept as its own numbered entry there since it's the biggest single feature on this list.

**5. Shade fury attack (4th spell)** (Moderate). The spell system in
`LegacyHelper.ShadeController.Spells.cs` (post-split) already has a working pattern for
gated, soul-cost, milestone-unlocked abilities. Adding a 4th spell is mostly new sprite
frames, new attack logic modeled on the existing VFX pipeline (`PlayShriekFx`,
`PlayDescendAura`, etc.), a new unlock milestone, and settings/HUD wiring.

**6. Radiance boss fight** (Very hard, correctly flagged as a stretch). Confirmed via search
of the decompiled Silksong reference assemblies: zero Radiance-related classes, FSMs, or
assets exist in this game. This is a from-scratch boss build, not a port — new arena/scene
hook into the Hornet-flower memory sequence, hand-rolled AI in C# (Silksong bosses are
Playmaker-FSM-driven; this mod's Harmony-patch approach doesn't generate FSM data, so the
boss AI would need to be built directly in C#), original attack patterns, and
animations/sprites either commissioned or adapted (raises an asset-rights question separate
from the engineering). Prototype standalone before wiring into the actual cutscene.

**7. Shade shadow particles** (Easy–moderate). Purely additive VFX: a small emitter following
the Shade, tuned to Hollow Knight 1's black-wisp look. Scaling intensity with current SOUL is
a one-value hookup once the emitter exists — `ShadeRuntime`'s soul value is already read
elsewhere for HUD/spell-gating, so the emitter just needs the same read. No architectural
dependencies; safe to do any time.

**8. Skin-preview anti-aliasing / global filtering** (Easy). The pixelation is a `Sprite`/
texture-import filtering issue (point/nearest filtering on low-res HK1-sourced art, viewed at
a large on-screen size in the skin-selector preview column in `ShadeSettingsMenu.Skins.cs`).
Likely fixable by switching the preview `Image`/`SpriteRenderer` to bilinear filtering plus a
mild `Material`-level smoothing pass, and — if it reads well — applying the same filtering to
the in-game Shade sprites so it doesn't look inconsistent next to Hornet's higher-res art.
Worth a quick visual A/B before committing to the global change, since some players may prefer
the crisp pixel look.

**9. Dynamic camera option** (Moderate–hard). Depends on how item 1 in "Known bugs" (Shade
docking at Hornet's side during locked-control states) lands, since that's the same
Shade-follows-Hornet binding this option would need to loosen outside boss fights/gauntlets.
Implementation is a config-gated camera-target blend (midpoint of Hornet and Shade positions)
sitting alongside the base game's existing camera-follow target — should stay off by default
outside boss/gauntlet scenes per the original ask, and needs an explicit fallback to
Hornet-only framing whenever a boss/gauntlet flag is active.

**10. Multiplayer (up to three Shades)** (Hard — the real architectural project, listed as
"Additional shades" above). Blocked on the multi-shade groundwork already flagged as
deliberately deferred: `ShadeRuntime`, `ShadePersistentState`, `ShadeCharmInventory`, and the
`LegacyHelper.helper` field are all static/singleton — the whole process assumes exactly one
Shade exists. Plan stands as written: wrap the current single Shade in a list-of-one first,
migrate static call sites to route through it, get that fully working and tested, then extend
to 2–3. Once that's in place, decide whether all Shades share one charm loadout/config or have
independent ones (a design question, not just engineering — affects
`ShadeSettingsMenu`/`ShadeInventoryPane` UI scope too), and update the Harmony patches in
`LegacyHelper.Patches.cs` that currently assume "the" Shade to iterate a collection instead.
Do not attempt the 3-shade case directly — get 1-in-a-list solid first.

**11. Shade AI** (Hard, large — but stageable, and Stage 1 alone is a real win for solo
testing). Three stages as scoped:
- **Stage 1** (Moderate): Shade stays in its existing invincible assist mode, floats toward
  the nearest enemy and attacks; when a spell would hit a boss or multiple basic enemies, it
  casts. This is state-machine work layered on top of the existing movement/combat/spell code
  in `LegacyHelper.ShadeController.Movement.cs` / `.Combat.cs` / `.Spells.cs` — no new physics
  or targeting infrastructure, just an AI driver reading the same state player input currently
  drives.
- **Stage 2** (Hard): Shade operates without assist mode, so it needs hit-avoidance based on
  live enemy hitbox data, plus a SOUL-conservation policy that holds spells back when either
  Hornet or the Shade is low on health and prioritizes healing. This is the substantial jump —
  it's real combat AI, not just movement-toward-target — and should be scoped and tested as its
  own milestone rather than bundled with Stage 1.
- **Stage 3** (Hard, exploratory): Shade identifies large platform gaps / parkour hazards and
  positions itself to be pogo'd by Hornet for a safer crossing. This is the least-scoped of the
  three — needs a way to read level geometry/gap data that doesn't exist in the mod today —
  and is the right one to defer until Stages 1–2 are shipped and the team has a feel for how
  much AI infrastructure is worth building.

## Working notes for whoever picks this up

- Toolchain: BepInEx 5.x is still current for Silksong (checked against the maintained
  Thunderstore pack, `silksong_modding-BepInExPack_Silksong`, at 5.4.23.4 vs. this project's
  referenced `BepInEx.Core 5.4.21`) — no framework migration needed, just code organization.
- Build: `dotnet build -c Release` from the repo root. `SilksongPath.props` (gitignored) or
  the auto-detected relative path (this mod lives directly inside the live game's
  `BepInEx/plugins/` folder, so the 4-levels-up fallback in `Tests/LegacyoftheAbyss.Tests.csproj`
  resolves correctly) points the build at the game install for reference assemblies.
- `AGENTS.md` in the repo root has the original repository-layout and build/test
  instructions from earlier work on this project — still accurate, read it alongside this file.
