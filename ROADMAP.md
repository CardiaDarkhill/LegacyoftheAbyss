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

**Test suite — green as of this pass.** 93 passing, 0 failing (was 15 failing of 46). What the
cleanup turned up, in order of discovery:
- `dotnet test` did not even build: leftover in-tree `bin/`/`obj/` folders from before the output
  redirection were no longer covered by MSBuild's derived default excludes, so their stale generated
  `AssemblyInfo.cs` compiled a second time (`CS0579 Duplicate ...Attribute`). `Directory.Build.props`
  now excludes `bin/**;obj/**` explicitly.
- The `FileNotFoundException` cluster was the missing `<UnityRuntimeLib>` entries as diagnosed;
  `Unity.ResourceManager`, `Unity.Addressables`, `TeamCherry.SharedUtils`, `TeamCherry.Localization`,
  `TeamCherry.NestedFadeGroup` and two UnityEngine modules are now copied beside the test binaries.
- `CaptureClampsValues` was a **real** bug: `Capture` synced `BaseMaxHP` to `MaxHP` *before* the
  floor that bumps a zeroed-out `MaxHP` to 1, so a fully-zeroed capture persisted
  `MaxHP == 1 / BaseMaxHP == 0`. Reordered.
- `ShadeCharmPlacementDatabaseTests` were stale, not broken — pinned to counts and coordinates from
  an older `charm_placements.json`. Rewritten to assert the matching *rules* (exact scene names,
  `sceneContainsAll` tokens, unscoped shop/boss-drop entries offered to every scene) instead of data
  that legitimately churns.
- `TryEquipRequiresOvercharmAttemptsBeforeExceedingCapacity` asserted copy that was deliberately
  changed in `53fae78` ("Shade resists overcharming..." → "Not enough notches available."). Test updated.
- Once the DLLs were present, two further **real** bugs surfaced underneath.
  `HandleCharmInventoryChanged` called `EnsureActiveSlot()`, which re-entrantly synced the inventory
  *from* the slot and so discarded the very mutation that raised the notification — then persisted
  the stale value back to disk. Split into a write-path slot claim plus a re-entrancy guard. Test
  isolation was also missing: `ShadeRuntime` is a process-wide singleton and the classes touching it
  ran in parallel, so results depended on ordering. Added a non-parallel `ShadeRuntimeCollection` and
  explicit `SaveSlots.ResetAll()` where tests need a clean slot.
- `InputDeviceBlockerTests` could never have passed outside Unity: `Time.timeScale`,
  `new GameObject(...)` and `UIManager`'s static initializer (`Animator.StringToHash`) are all extern
  engine calls. The blocker's decision is now split into an uncached
  `EvaluateShouldBlockShadeDeviceInput()` that the tests drive directly, with the ui/menu-state
  classification covered through `MenuStateUtility.IsMenuStateName`.

## Known bugs — feasibility notes

**1. Save-data persistence across Thunderstore updates** — **fixed.** `ModPaths.UserData` now
resolves to `BepInEx/config/LegacyoftheAbyss/` (via `BepInEx.Paths.ConfigPath`, falling back to
walking up for a `BepInEx` folder, then to the old `Assets` location as a last resort), and both
`ModPaths.Config` and `ShadeSaveSlotRepository`'s default storage root point at it. A one-time
migration copies any `config.json` / `shade_slot_*.json` left in the old `Assets/` folder on first
run, never overwriting an existing destination file and never deleting the originals. The folder is
flat and copyable, per the bonus ask; `LEGACYOFTHEABYSS_DATA` overrides the location if set.
Original diagnosis, for reference: every save artifact was written under `ModPaths.Assets`, i.e.
inside `BepInEx/plugins/LegacyoftheAbyss/`, because `ModPaths.Root` is the plugin DLL's own folder.
Thunderstore-style mod managers (r2modman, the Thunderstore App) install updates by deleting and
replacing the whole package folder, which took the save data with it; a manual Nexus install
typically only overwrites the files the new zip contains, so extras survived by accident.

**2. Inventory-pane tab-index mismatch** — **fixed, root cause was not what it looked like.**
Appending the Shade pane (rather than inserting mid-list) was correct and necessary work — confirmed
in-game via the pane-layout dump (`[0]Inv [1]Tools [2]Quests [3]Journal [4]Map [5]ShadeInventoryPane`)
and a full trace of every `SetCurrentPane(requested=N) -> [N] name` call, which resolved correctly on
every single press. That ruled out the array and the native pane-switch code entirely, meaning the
actual bug had to be upstream of `SetCurrentPane` — in which `PaneTypes` value a keypress produces in
the first place, not in how that value gets resolved.

It was `HornetInput.cs`, this mod's own "left-side keyboard layout" default preset, applied when a
player picks a keyboard binding preset in the settings menu. Its number-key table was
`Key1->Inv, Key2->Map, Key3->Journal, Key4->Tools, Key5->Quests`, which does not match
`InventoryPaneList.PaneTypes` order (`Inv, Tools, Quests, Journal, Map`) — i.e. does not match the
left-to-right order the tabs actually appear in. A player pressing "2" expecting the 2nd visible tab
(Tools) got Map instead. The traced request sequence from the first five presses of a test session —
`0, 4, 3, 1, 2` — is an exact, unmistakable match for that binding table in order, about as close to
a smoking gun as this kind of bug gets. Reordered to
`Key1->Inv, Key2->Tools, Key3->Quests, Key4->Journal, Key5->Map`, matching the tab order exactly.

Two things worth flagging: this bug is **entirely unrelated** to the pane-array append fix above —
it would have reproduced with zero Shade pane and zero changes to `ShadeInventoryPaneIntegration.cs`,
since it lives purely in which `HeroActions` field gets bound to which key. And **the fix requires the
player to re-pick a keyboard preset from the mod's settings menu** after updating — the previous
(wrong) key assignments are already written into the game's own `gameSettings` and InControl bindings
and persist across launches; redeploying the DLL alone does not retroactively fix bindings already
saved to disk.

Separately, "the 1 key always opens the tab you were previously looking at" is not a bug: pressing the
generic "open inventory" key while the inventory is closed reopens whichever pane was last viewed,
matching how the base game's own inventory-open button has always worked (comparable to Hollow
Knight 1's Tab key). Key 1 was never part of the scrambled mapping either way.

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

**4. Shade can't pause the game** — **fixed.** Traced as suggested, and it was not the allow-list
check itself. InControl only ever polls `InputManager.ActiveDevice` for a `PlayerActionSet`
(`PlayerActionSet.Update` → `FindActiveDevice`), and
`InputManager_UpdateActiveDevice_BlockShadeDevices` deliberately reverts the active device whenever a
Shade-owned pad tries to claim it during gameplay — so `HeroActions.Pause` never saw that pad at all,
and the `AllowedHeroActions` whitelist was dead code in practice. The postfix now leaves the pad
active for any frame in which it is actually driving one of the allowed actions
(`InputDeviceBlocker.IsDrivingAllowedHeroAction`, evaluated against the real `PlayerAction.Bindings`),
while `PlayerAction_Update_BlockShadeGameplay` keeps nulling the device for everything else. Also
fixed a plain typo in that whitelist: the quick-map action is named `"Quick Map"` in `HeroActions`,
not `"QuickMap"`, so the ordinal lookup had silently been matching nothing and blocking quick-map
along with the gameplay actions. Keyboard was never affected — `KeyBindingSource.GetState` ignores
the active device, so Escape always worked.

**5. Janky sub-menu back-out highlight** (Easy once 2 and 4 are done). Symptom is a
one-frame flash of the correct selection before it jumps to the top item — classic "selection
gets reset on pane rebuild, then restored one frame late" ordering bug. Likely a case of the
pane-open code selecting a default item before the "return to here" selection is applied, or
an `OnEnable`/`Start` ordering issue in whatever handles the Shade-pane back-out path added
alongside bug 2. Cheap to fix once the surrounding input-routing code is already open for 2/4.

**6. `SlideSurface.UpdateFacing` NullReferenceException in Mount Fae** — **fix shipped, not yet
confirmed** (no crash observed in testing, but the original crash was never reproduced either, so
absence of the crash proves little). `SlideSurface.OnTriggerEnter2D` flips `isHeroInside` and bumps
the static `_heroInsideCount` *before* it checks who entered, then does
`this.hc = collision.GetComponent<HeroController>()` and bails when that comes back null — so a
non-Hornet entrant overwrites the cached hero reference with null while leaving the surface's
"hero is here" bookkeeping switched on. The next follow tick calls `UpdateFacing()`, which
dereferences `this.hc.cState`, and the frame dies. `OnTriggerExit2D` has the mirror problem: it calls
`HeroNotInside()` unconditionally, so the Shade leaving clears the flag while Hornet is still on the
slide. All three trigger callbacks now drop Shade-owned colliders before they touch any state.

**7. Updraft affects Hornet based on the Shade's position** — **re-fixed after a wrong first
attempt.** The shared-flag diagnosis in the original entry was right; the component named in it was
not, and neither was the first fix.

Mechanism, confirmed from the FSM dump rather than guessed: updraft lift is not `EnemyUpdraftRegion`
(that bails immediately on anything without a `PlayMakerFSM`, so the Shade never reaches it) and it is
not `WindRegion` either (that only sets `cState.inWindRegion`, which drives lean animation). It is a
PlayMaker FSM on the updraft object that polls `CheckTrackTriggerCount` →
`TrackTriggerObjects.InsideCount` every fixed update and calls `HeroController.EnterUpdraft` /
`ExitUpdraft`. `TrackTriggerObjects` filters entrants purely on layer and tag, and the Shade's
`ShadeAggroProxy` child deliberately copies Hornet's layer *and* tag so enemies notice it — so the
Shade counts as an occupant, the count never reaches zero while it is inside, and EXIT never fires
however far away Hornet walks.

The fix hooks `TrackTriggerObjects.IsCounted` (and the `TrackTriggerObjectsLineOfSight` override),
which is the only filter `InsideCount` applies. Shade-owned objects stop being counted, so the FSM
fires EXIT the moment Hornet leaves. The same getter backs `IsInside`, which the rest of the game uses
for bench work ranges, breakable ranges, pickup triggers, camera shake, music, frost and driftfly
dispersal — all "is Hornet here?" questions the Shade should not answer.

**`AlertRange` is explicitly exempt**, because some enemy FSMs read their alert range through the same
`CheckTrackTriggerCount` action. `Tests/ShadeRegionExclusionTests.cs` pins that exemption.

**What the first attempt got wrong, and why it matters:** it prefixed
`TrackTriggerObjects.OnTriggerEnter2D` to drop Shade colliders outright, filtered by a list of named
Hornet-state region types (`WindRegion`, `FrostRegion`, `CameraLockArea`, ...). That list was derived
from reading the region classes rather than from tracing the actual symptom, and it did not contain
the plain `TrackTriggerObjects` the updraft uses — so the updraft was never fixed. It also stopped
enemies noticing the Shade, which the type list should have prevented and did not. Hooking `IsCounted`
instead leaves `insideGameObjects` and the `OnTrackTriggerEntered` callback completely untouched, so
aggro registration cannot be affected by construction rather than by a filter that has to be correct.

**8. Fix existing test failures + generate more tests** — **first half done**; see the test-suite
entry under "Status as of this writing" for the full breakdown. Suite is green at 91 passing. New
coverage added alongside the fixes: pause/menu routing (`InputDeviceBlockerTests`, including a guard
that the allow-list action names match `HeroActions` exactly — that is what caught the `"QuickMap"`
typo) and region exclusion (`ShadeRegionExclusionTests`).

**Still outstanding:** coverage for the bench-lock work (bug 3), which is not built yet. Note the
hard ceiling this suite runs into — anything reaching a Unity extern (`Time.*`, `new GameObject`,
`UIManager`'s static initializer) throws `SecurityException: ECall methods must be packaged into a
system module` in a plain test host, so testable logic has to be split out of the Unity-facing code
deliberately, as `EvaluateShouldBlockShadeDeviceInput` and `IsHeroStateRegionType` now are.

**9. Sprite-texture leak on destroy** — **first fix was insufficient; revised.** The first attempt
called `Object.Destroy(tex, RetiredSkinTextureLifetime)` from `ShadeController.OnDestroy`. Memory kept
climbing, and the likely reason is lifetime: Unity's delayed-destroy queue is torn down with the
scene, so a Shade destroyed *as part of a scene unload* had its pending texture destroys dropped —
and since the Shade is respawned on every scene change, that is the common path, not the rare one.
The same applies to `ReloadSkinSprites`, which started its release coroutine on the Shade itself.

Both paths now hand the work to `LegacyHelper.RetireShadeSpriteAssets`, which runs on the BepInEx
plugin behaviour and therefore survives scene loads. Three other things changed while it was open:
the `Sprite` objects cut from each sheet are now destroyed too (roughly seventy per Shade, and each
holds a reference to its source texture); sheets are loaded with `markNonReadable: true`, which drops
the CPU-side copy Unity otherwise keeps for the texture's whole lifetime and roughly halves resident
sheet memory; and the release logs a texture/sprite count behind `logShade`, so it is possible to tell
"the release never ran" from "the release ran and memory still climbs".

If it still climbs with that log firing, the leak is not the sheets and the next step is a profiler
capture rather than more guessing.

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
