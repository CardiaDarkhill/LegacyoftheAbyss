# Roadmap — Legacy of the Abyss

Treat this as a living document — update it as work gets done or priorities change. This is a to-do
list, not a changelog: finished work is removed rather than kept as a checked-off entry. Git history
and `REFACTOR_NOTES.md` are the record of what was done and when.

## Status as of this writing

Refactor/split, skin selector, save-data relocation, the Key1-5 keybind fix, the updraft/aggro
region-exclusion fix, the SlideSurface crash fix, the sprite-texture leak fix, and the test-suite
cleanup (93 passing, 0 failing) are all shipped and confirmed. See git log for details on any of
these if needed.

## Known bugs — feasibility notes

**1. Shade-owned device needs a control preset re-applied every session before it can open menus or
pause.** Confirmed still broken after the most recent fix attempt. What's been ruled out: the
bindings themselves. `HornetInput.EnsureShadeInventoryBindings` now runs automatically from
`MenuInputBridge.EnsureBindings` (fires at `InputHandler.OnAwake` and on every
`OnUpdateHeroActions`), not only from a preset-button click, and logging confirmed the Shade's
`OpenInventory` binding (Key1 on keyboard, or Back/Select on controller, depending on which device is
free) is genuinely present immediately after boot. The bindings existing isn't enough - something
else, downstream of "does a binding exist," is what actually starts working only after a preset is
re-applied that session. `InputDeviceBlocker.ShouldBlockShadeDeviceInput`/`IsRestrictedDevice`/
`RefreshShadeDevices` are the next things to instrument - logging their return values from the very
first frame after boot, before any preset click, compared to their values right after one, should
show what's actually different. Note this also affects Pause specifically, which has nothing to do
with the Shade's `OpenInventory` binding at all (it's a native, always-present binding) - so whatever
this is, it's likely something shared and lower-level than the inventory-open path, not specific to
either.

**2. Bench behavior cluster** (Moderate, one connected fix). Three symptoms, one missing piece: there
is currently no "Hornet's controls are locked" hook that the Shade's movement state machine listens
to - a grep of `LegacyHelper.ShadeController.Movement.cs` turns up no bench, sitting, or cutscene
handling at all, confirming this was never built rather than regressed. Needs: (a) detect the same
state the game uses for "Hornet is sitting at a bench" / "Hornet is in a cutscene" (`RestBenchHelper` /
`HeroController`'s input-locked state in the decompiled reference are the likely hooks), (b) drive the
Shade into a "dock at Hornet's side, face her direction, stop accepting movement input" state whenever
that's true (outside hit-stun), and (c) route the Shade's HUD visibility and its ability to open
charms/map/etc off the same flag - hide HUD when Hornet's menu is open, keep menu-open input alive
regardless of which device (Hornet's or Shade's) is bound to it. Worth building as one flag and wiring
three consumers to it rather than three separate patches.

**3. Janky sub-menu back-out highlight.** Symptom is a one-frame flash of the correct selection before
it jumps to the top item - classic "selection gets reset on pane rebuild, then restored one frame
late" ordering bug. Likely a case of the pane-open code selecting a default item before the
"return to here" selection is applied, or an `OnEnable`/`Start` ordering issue. Worth checking whether
it's related to entry 5 below (the Shade-tab capture/restore mechanism) before assuming it's isolated.

**4. `SlideSurface.UpdateFacing` NullReferenceException in Mount Fae — fix shipped, never confirmed
live.** The mechanism was diagnosed from decompiled source rather than a real repro (see git log for
the full trace), and the fix has had no contradicting reports since, but nobody has actually stood in
Mount Fae with the fixed build watching for the crash. Worth a five-minute confirmation pass next time
someone's in that area, then this entry can go.

**5. Shade charm-tab access has two related, deferred bugs sharing one root cause.** (a) Jumping to
the Shade tab specifically *from Inv* (not from Tools/Quests/Journal/Map, whether via the Key 6
shortcut or by cycling with LB/RB) makes the Shade pane vanish again after about a second - log
evidence shows the Shade pane's GameObject going through an extra, unprompted `OnEnable`→`PaneStart`→
`OnDisable`→`PaneEnd` cycle that doesn't happen from other source panes. (b) Once the Shade tab has
been reached at all in a session - by any path - every one of the 6 inventory shortcuts closes the
whole inventory instead of switching panes, for the rest of that session, regardless of which tab is
open or which key is pressed. Mechanism for (b): reaching the Shade tab makes
`ShadeInventoryPaneIntegration.BindInput` set every *other* real pane's own `InventoryPaneInput.
paneControl` to `None` too (needed so the Shade can intercept Submit/Direction while its tab is
showing), and that's supposed to be restored via `RestoreInputBindings` when the Shade tab is left;
`paneControl == None` is exactly what makes the native `InventoryPaneInput.Update()` switch treat
*every* shortcut as "this pane's own key was pressed" and unconditionally cancel. (a) is the leading
suspect for why the restore in (b) sometimes doesn't complete cleanly, since it's an unexpected extra
enable/disable cycle exactly where the capture/restore bookkeeping lives - but this isn't confirmed,
and needs logging inside `RestoreInputBindings`/`RestoreSingleInput` specifically (what `paneControl`
each snapshot captured, whether `OriginalInputBindings` still holds an entry for each input by the
time restore runs) rather than another guess. Fix (a) first and retest (b) before assuming they need
separate work.

**6. Enemies don't redirect their attention to the Shade - architectural, needs a scoping decision.**
Aggro *registration* works correctly (confirmed via live log: `Shade aggro proxy entered/exited alert
range` fires as expected) - the gap is in *target selection* once alerted. Checked how common this is
across the decompiled base game: the generic "find nearest object in range" API
(`TrackTriggerObjects.GetClosestInside`) has exactly one reference in the entire codebase - its own
declaring file - versus 184 files that reference `HeroController.instance` directly. So this isn't a
quick fix or a regression from anything in this project; it's how the base game's enemy AI is built.
Making enemies actually attack the Shade (not just notice it) means rewriting target resolution in
each individual enemy script that hardcodes the hero reference - a large, standalone project, not a
bug fix. Needs a decision on whether to pursue it at all before any code gets written.

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

**2. Skin selector** — **done.** See "Skin system" above. (The audit called for here came back
clean: `LegacyHelper.ShadeController.Core.cs::LoadShadeSprites` was already the only place
Shade sheets were loaded, so centralizing was a one-function change. Note the sprites load
there, not in `HUD.Assets.cs` as this entry originally guessed — that file handles HUD art.)

**3. Optional gravity** (Easy). The Shade's `Rigidbody2D` already runs with `gravityScale = 0f`
and fully custom float/knockback code (now in `LegacyHelper.ShadeController.Movement.cs`).
Making gravity optional is close to flipping `gravityScale` based on a config bool and adding
a grounded/fall state to the existing movement state machine — not building physics from
scratch. Budget as quick-and-dirty per the original ask.

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

**9. Dynamic camera option** (Moderate–hard). Depends on how item 3 in "Known bugs" (Shade
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
