# Roadmap — Legacy of the Abyss

Working plan for Claude Code picking this project up. Written after a Cowork session that
read through the codebase, assessed feasibility of the features below, and did a first-pass
refactor. Treat this as a living document — update it as work gets done or priorities change.

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

**Deliberately deferred, not started:**
- **Multi-shade architecture groundwork.** `ShadeRuntime`, `ShadePersistentState`,
  `ShadeCharmInventory`, and the `LegacyHelper.helper` field are all static/singleton —
  the whole process assumes exactly one Shade exists. This is the real blocker for
  "Additional Shades" (see below) and needs to happen before that feature starts. Plan is
  to wrap the current single Shade in a list-of-one first (introduce a `ShadeInstance`
  concept, migrate the static call sites to route through it), get that fully working and
  tested, *then* extend the collection to 2–3. Do not attempt the 3-shade case directly.

**Known pre-existing issues, not caused by the refactor above (confirmed via `git stash` +
re-test against the unmodified code):**
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
- These are out of scope for the feature work below unless they start blocking something —
  flagging them so they aren't mistaken for new regressions.

## Suggested order for what's next

1. **Graphical layering fix** and **optional gravity toggle** — both fast, contained, no
   architectural dependency on anything else. Good to do together or in either order.
2. ~~**Skin selector**~~ — done, see "Skin system" below.
3. **Shade fury attack** (4th spell)
4. **Multi-shade architecture groundwork** (deferred item above) → then **Additional shades**
5. **Radiance boss fight** — prototype as a standalone arena/enemy first, wire into the
   Hornet-memory cutscene only once the fight itself is confirmed fun.

## Skin system

Shipped. Structure on disk:

- `Assets/Knight_Shade_Sprites/*.png` — the built-in ("Default Shade") set, and the fallback
  for every other skin.
- `Assets/Knight_Shade_Sprites/Skins/<Skin Name>/` — one folder per alternate skin. A skin
  only needs the sheets it actually changes; anything it omits resolves back to the built-in
  set. That is why e.g. `Cozy Shade` ships 7 files and `Grimmchild Phase 3` ships all 16.
- `Assets/Knight_Shade_Sprites/Skins/skins.json` — optional. Sets menu order and display
  names. Folders missing from it are still discovered and appended alphabetically, so a
  drop-in skin folder needs no config edit at all.

Code:

- `Shade/ShadeSkinManager.cs` discovers skins and owns `ResolveSpritePath` (skin folder →
  built-in fallback). `Shade/ShadeSkinDefinition.cs` is the record type.
- `LegacyHelper.ShadeController.Core.cs::LoadShadeSprites` is still the single sprite-load
  path — it now resolves every sheet through `ShadeSkinManager`. Keep it that way; adding a
  second load site would break skin switching.
- `ReloadSkinSprites` swaps sheets on a live Shade and frees the outgoing skin's textures on
  a short delay (in-flight spell VFX hold those arrays). Entry point is
  `LegacyHelper.SetShadeSkin`.
- UI is `ShadeSettingsMenu.Skins.cs` — a "Skins" screen off the shade settings menu with a
  preview column and one row per skin.
- Selection persists as `shadeSkin` in `ModConfig`. An unknown/deleted id falls back to
  Default rather than erroring.

**Note on frame counts:** every sheet's frame count is still hardcoded in `LoadShadeSprites`
(idle 9, float 6, …). All shipped skins match those dimensions exactly. A skin with a
different frame count would need per-skin metadata, which does not exist yet.

**Known adjacent leak, pre-existing and untouched:** `ShadeController` never frees its sprite
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

**4. Additional shades (up to 3)** (Hard — the real architectural project). Blocked on the
multi-shade groundwork above. Once that's in place: decide whether all 3 Shades share one
charm loadout/config or have independent ones (a design question, not just engineering —
affects `ShadeSettingsMenu`/`ShadeInventoryPane` UI scope too), and update the Harmony
patches in `LegacyHelper.Patches.cs` that currently assume "the" Shade to iterate a
collection instead.

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
