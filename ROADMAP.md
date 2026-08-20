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
2. **Skin selector**
3. **Shade fury attack** (4th spell)
4. **Multi-shade architecture groundwork** (deferred item above) → then **Additional shades**
5. **Radiance boss fight** — prototype as a standalone arena/enemy first, wire into the
   Hornet-memory cutscene only once the fight itself is confirmed fun.

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

**2. Skin selector** (Easy–moderate). Sprites already load from flat PNGs on disk at
runtime (`HUD.Assets.cs`), not baked into the DLL. Mostly: define a skin-folder convention
under `Assets/`, add a `selectedSkin` field to `ModConfig`, and centralize the sprite-load
paths in `HUD.Assets.cs` to resolve through the selected skin. Audit first that every place
loading a Shade sprite goes through one lookup path rather than being hardcoded individually.

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
