# Roadmap — Legacy of the Abyss

Treat this as a living document — update it as work gets done or priorities change.

## Status as of this writing

**Done:**
- Codebase read-through and feasibility assessment for a refactor + performance pass plus
  six planned features (see "Planned features" below).
- File-splitting refactor, first pass: `ShadeInventoryPane.cs` (was 8,407 lines),
  `ShadeSettingsMenu.cs` (was 3,552 lines), and `LegacyHelper.ShadeController.Core.cs`
  (was 4,234 lines) split into 17 files total along thematic seams, each a `partial` slice
  of the original class. No logic changes — pure code movement, verified by exact line
  coverage and brace-balance checks before commit, then confirmed with a real
  `dotnet build -c Release`. See `REFACTOR_NOTES.md` in the repo root for exactly what moved
  where. Along the way, discovered `ShadeInventoryPane.cs` was hiding two extra top-level
  types (`SimpleCanvasNestedFadeGroup`, `ShadeInventoryPaneIntegration`) that got pulled out
  into their own file.
- **Skin selector (feature 2 below).** Shipped with 7 alternate skins plus a refreshed
  default set. See "Skin system" below for the on-disk convention.

**Known pre-existing issues, not caused by the refactor above (confirmed via `git stash` +re-test against the unmodified code):**
- 9 test failures (`InputDeviceBlockerTests`, `ShadeRuntimeDebugTests`,
  `ShadeCharmInventoryTests.*`) throw `FileNotFoundException` for `Unity.ResourceManager.dll`
  / `TeamCherry.SharedUtils.dll`. Root cause: `Tests/LegacyoftheAbyss.Tests.csproj`'s
  `<UnityRuntimeLib>` item list (which copies Unity DLLs from the Silksong install next to
  the test binaries) doesn't include those two assemblies, even though `ShadeRuntime`
  transitively needs one of them at runtime. Fix is to add the missing DLL(s) to that list.
- 6 assertion-mismatch failures in `ShadePersistentStateTests.CaptureClampsValues` and the
  `ShadeCharmPlacementDatabaseTests` group — logic/data issues in `Shade/ShadePersistentState.cs`
  and `Shade/ShadeCharmPlacementDatabase.cs` / `Assets/charm_placements.json`, unrelated to
  the file split. Not investigated further yet.
- Previously flagged as "out of scope unless blocking" — now explicitly prioritized, see
  item 8 in "Suggested order" below. Test debt has been sitting long enough that it's
  starting to hide real regressions in bug work.

## Known bugs — feasibility notes

**1. Save-data persistence across Thunderstore updates** (Contained, but the fix matters more
than the effort). Root cause found: `ModPaths.Root` (`ModConfig.cs`) is
`Path.GetDirectoryName(typeof(ModPaths).Assembly.Location)` — the plugin DLL's own folder —
and every save artifact (`ShadeSaveSlotRepository`'s slot files, `ModPaths.Config` /
`config.json`) is written under `ModPaths.Assets`, i.e. inside
`BepInEx/plugins/LegacyoftheAbyss/`. Thunderstore-style mod managers (r2modman, the
Thunderstore App) install updates by deleting and replacing the whole package folder, which
takes the save data with it; a manual Nexus install typically only overwrites the files the
new zip contains, so extras survive by accident. Fix: move persisted save state (slot files,
config) to a location that isn't the versioned plugin folder — e.g. a sibling directory under
`BepInEx/config/` or a dedicated folder next to `BepInEx/` — and add a one-time migration that
picks up any existing save sitting in the old `Assets/` location. Bonus ask from the report:
keep it a flat, copyable folder so a player can carry it to a new device by hand.

**2. Inventory-pane tab-index mismatch** (Moderate). Confirmed mechanism: our
`InventoryPaneList_EnsureShadePane_Patch` (`LegacyHelper.Patches.cs`) inserts a new Shade pane
into `InventoryPaneList`, which shifts every pane after it up one index. The base game's own
input plumbing (`ListenForInventoryShortcut`, `SetCurrentInventoryPane`, and the
`GetAdditionalMapZone`-style numeric-index shortcuts referenced in the bug report) opens panes
by numeric position, not by identity, so inserting a pane silently reassigns every keyboard
shortcut and the "open map" / "open journal" bind after it. Two fixes proposed by the reporter:
insert the Shade pane last instead of mid-list (simplest, may still not be "correct" positionally),
or intercept `ListenForInventoryShortcut`/the shortcut-to-pane mapping ourselves and remap by
identity instead of index. The second is more robust and also fixes bug 3 below (menu-selection
sub-item) since it's the same numeric-index assumption.

**3. Bench behavior cluster** (Moderate, one connected fix). Three symptoms, one missing piece:
there is currently no "Hornet's controls are locked" hook that the Shade's movement state
machine listens to — a grep of `LegacyHelper.ShadeController.Movement.cs` turns up no bench,
sitting, or cutscene handling at all, confirming this was never built rather than regressed.
Needs: (a) detect the same state the game uses for "Hornet is sitting at a bench" /
"Hornet is in a cutscene" (`RestBenchHelper` / `HeroController`'s input-locked state in the
decompiled reference are the likely hooks), (b) drive the Shade into a "dock at Hornet's
side, face her direction, stop accepting movement input" state whenever that's true (outside
hit-stun), and (c) route the Shade's HUD visibility and its ability to open charms/map/etc
off the same flag — hide HUD when Hornet's menu is open, keep menu-open input alive
regardless of which device (Hornet's or Shade's) is bound to it. Worth building as one flag
and wiring three consumers to it rather than three separate patches.

**4. Shade can't pause the game** (Moderate). `InputDeviceBlocker` and `MenuStateUtility.cs`
already exist and are under test (`Tests/InputDeviceBlockerTests.cs`), so the plumbing for
"which device does what" is there — this is most likely the Shade's owned device being
excluded from the pause/menu-open shortcut check rather than a missing subsystem. Confirm
by tracing what `InputDeviceBlocker` allows through when a device is bound to the Shade,
and make sure Start/Escape ignores Shade-ownership entirely (pause should never be
assignable away from Hornet's side).

**5. Janky sub-menu back-out highlight** (Easy once 2 and 4 are done). Symptom is a
one-frame flash of the correct selection before it jumps to the top item — classic "selection
gets reset on pane rebuild, then restored one frame late" ordering bug. Likely a case of the
pane-open code selecting a default item before the "return to here" selection is applied, or
an `OnEnable`/`Start` ordering issue in whatever handles the Shade-pane back-out path added
alongside bug 2. Cheap to fix once the surrounding input-routing code is already open for 2/4.

**6. `SlideSurface.UpdateFacing` NullReferenceException in Mount Fae** (Needs a repro).
`SlideSurface.cs` (decompiled reference) is a base-game component, unmodified by this mod as
far as current patches show — likely the Shade is interacting with a slide surface in a way
the base game never expects (e.g. the Shade triggering `SlideSurface` logic that assumes only
Hornet's `HeroController` can be the actor). First step is reproducing in Mount Fae with a
debug build to get a full stack past the trimmed Unity log, then checking whether any of our
Harmony patches touch `HeroController` facing/velocity while the Shade is nearby a slide
surface.

**7. Updraft affects Hornet based on the Shade's position** (Moderate). Decompiled reference
shows updraft regions (`EnemyUpdraftRegion.cs` and similar) query the entering collider's
owner to apply the lift. If the Shade shares a layer/tag/trigger setup with Hornet, or if our
movement code reads a shared "in updraft" flag instead of a per-entity one, the Shade entering
an updraft can flip a flag that Hornet's Drifter's Cloak code also reads. Fix is to confirm
the updraft trigger and the cloak-float logic are keyed per-`GameObject`/per-controller, not
off a single static/shared flag — same class of bug as the static-singleton issue blocking
multiplayer, so worth doing with that context in mind even before the multi-shade refactor
starts.

**8. Fix existing test failures + generate more tests** (Moderate). The two known clusters are
already diagnosed above ("Known pre-existing issues"): missing `<UnityRuntimeLib>` entries for
`Unity.ResourceManager.dll` / `TeamCherry.SharedUtils.dll`, and real assertion mismatches in
`ShadePersistentStateTests.CaptureClampsValues` / `ShadeCharmPlacementDatabaseTests`. Fix both,
then add coverage for whatever the bench-lock (bug 3), pause-routing (bug 4), and updraft
(bug 7) fixes touch, since none of that is under test today and is exactly the kind of
state-machine code that regresses silently.

**9. Known adjacent leak, pre-existing and untouched:** `ShadeController` never frees its sprite
textures on destroy, so toggling the Shade off/on re-decodes ~20 MB each time. The new skin
swap path does free them; the destroy path still does not.

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
