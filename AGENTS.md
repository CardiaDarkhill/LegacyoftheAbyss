# AGENTS.md file

## Where the documentation lives
- The project's prose documentation is a **GitHub Wiki**: <https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki>. This file is still self-contained — you can work most tasks from it alone — but the wiki holds the longer-form material that used to sit in `ROADMAP.md`, `PUBLISHING.md` and `REFACTOR_NOTES.md`, all three of which have been **deleted and superseded** by it. If you are looking for one of those files, that is why it is missing.
- Only `README.md`, `AGENTS.md` and `CHANGELOG.md` remain as repo-root markdown. **Do not add new `.md` files to the repo root** — new documentation belongs on the wiki.
- The pages most likely to be useful to an agent working here:

  | If the task is… | Read |
  | --- | --- |
  | "which file holds this method?" | [Code Map](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Code-Map) — the per-file responsibility map for every partial class |
  | Understanding how the pieces fit together | [Architecture Overview](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Architecture-Overview) |
  | Adding or changing a config field | [Configuration Reference](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Configuration-Reference) — every field, default and gotcha |
  | Anything path-related (assets, logs, save data) | [Asset and Data Paths](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Asset-and-Data-Paths) |
  | Build/deploy flags and the test suite | [Building and Testing](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Building-and-Testing) |
  | Release, packaging or CI work | [Publishing a Release](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Publishing-a-Release) |
  | Planned work, known bugs, feasibility notes | [Roadmap](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Roadmap) |
  | Triaging or extending bug capture | [Bug Report System](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Bug-Report-System) |
  | Input, charms or skins specifically | [Controls and Bindings](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Controls-and-Bindings), [Shade Charms](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Shade-Charms), [Skins](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Skins) |

- The wiki is a separate git repository (`LegacyoftheAbyss.wiki.git`). Editing it is a separate clone and push from the code repo — mention wiki updates that a change makes necessary rather than assuming a code commit carries them.

## Project overview
- This repository contains the **Legacy of the Abyss** BepInEx mod for *Hollow Knight: Silksong*. The mod adds a powerful Shade companion that fights alongside the player, exposes configuration menus, and persists its own progression.
- Nearly all runtime behaviour is hosted in the `LegacyHelper` partial class (see the `LegacyHelper.*.cs` files). Its `BaseUnityPlugin` entry point lives in `LegacyHelper.Core.cs` and wires up Harmony patches, HUD injection, and shade spawning.
- The Shade AI lives in the nested `LegacyHelper.ShadeController` partial class (`LegacyHelper.ShadeController.*.cs`). These files cover movement/AI (`Core`), state fields and animation data (`Fields`), persistence hooks (`Persistence`), slash/projectile combat (`Slash`), and charm-driven stat mutation (`Charms`).
- Systems under `Shade/` provide the persistent data layer for the companion:
  - `ShadeRuntime` mediates cross-scene state, notifications, and charm loadouts.
  - `ShadePersistentState` and `ShadeSaveSlotRepository` persist health, soul, charm unlocks, and save-slot separation.
  - `ShadeCharmInventory` plus the calculator/definition classes describe charm effects and derived stats.
- UI/HUD code is organised into partial classes under `HUD.*.cs`. `ShadeSettingsMenu.cs`, `ShadeInventoryPane.cs`, and related helpers construct and drive the custom pause-menu screens. `ShadeUnlockPopup.cs` & `ShadeUnlockPickup.cs` surface new ability notifications.
- Input configuration is split between `HornetInput.cs` and `ShadeInputConfig.cs`, while gameplay glue and helpers reside in files such as `LegacyHelper.Projectile.cs`, `LoggingManager.cs`, and `ModConfig.cs`.
- Decompiled game references can be found under `Decompiles/Assembly-CSharp`. Use these as your API reference when interacting with Silksong types that do not ship with public documentation.
- An export of all in game internal names from the localization can be found in `Decompiles/localization`, in the format of XML files.
- An export of all the FSM's in the game can be found at `Decompiles/fsms_part_1.txt` and `Decompiles/fsms_part_2.txt`, be warned that these files are trully enourmous, over 75MB, so be careful when searching through it.

## Repository layout
- This repo is checked out directly inside the live `BepInEx/plugins/` folder of a Silksong install (`.../BepInEx/plugins/LegacyoftheAbyss/`), for a fast edit-build-restart loop. **Because of this, `bin`/`obj` are deliberately relocated outside the `BepInEx/plugins` tree** (see `Directory.Build.props` below) — BepInEx recursively scans every `.dll` under `BepInEx/plugins`, so build output left in its default location becomes a second, independently loadable copy of the plugin sitting next to the real one. Two copies with no way to tell which is newer is exactly how a stale build can silently keep running after you've rebuilt. Do not remove or bypass this redirection.
- `Directory.Build.props` – redirects `BaseOutputPath`/`BaseIntermediateOutputPath` for **both** projects to `../../../LegacyoftheAbyss-DevBuild/{bin,obj}/<ProjectName>/`, i.e. a sibling of `BepInEx/` at the game root, outside anything BepInEx scans. Picked up automatically by any project in this folder or below; don't add a competing `BaseOutputPath` override in either csproj.
- `LegacyoftheAbyss.csproj` – main plugin project (netstandard2.1). Three deploy-adjacent MSBuild targets, all opt-in via an explicit property (a plain `dotnet build` never touches anything outside `LegacyoftheAbyss-DevBuild/`):
  - `DeployLocalDevBuild` (`-p:DeployLocalDevBuild=true`) – copies the freshly built DLL & PDB flat into `BepInEx/plugins/` (one level above this project). This is what makes a local rebuild actually show up in-game; without it, the build output stays in `LegacyoftheAbyss-DevBuild/` and the game keeps running whatever's already at `BepInEx/plugins/LegacyoftheAbyss.dll`. **It also implies `DeployDevProfile` below** whenever `DevProfile.props` supplies a profile path, so one command covers both ways of launching the game.
  - `DeployDevProfile` (`-p:DeployDevProfile=true`, needs a gitignored `DevProfile.props`) – copies the same DLL & PDB into `<profile>/BepInEx/plugins/LegacyoftheAbyss-Dev/`. Launching through Thunderstore Mod Manager / r2modman points doorstop at the *profile's* BepInEx, so the game folder's `BepInEx/plugins/` — where this repo lives — is never scanned on those launches. Without this, a manager launch runs whatever was last installed there, not the build you just made. Keep the published Thunderstore package disabled in that profile: two copies share one BepInPlugin GUID and only one loads, with no guarantee which.
  - `CopyMod` (needs `SilksongPath.props`, see below) – deploys into a self-contained `BepInEx/plugins/LegacyoftheAbyss/` subfolder (the portable layout a real end-user following install instructions expects) and exports a zip.
  - `PrepareReleasePackages` (`-p:CreateDistributionPackages=true`, Release only) – stages the Nexus/Thunderstore packages under `LegacyoftheAbyss-DevBuild/obj/...`. This one previously had no gate beyond `Configuration==Release` and silently created loadable duplicate copies of the plugin on every Release build — if you ever see stray plugin DLLs reappear under `BepInEx/plugins`, check this target hasn't regressed back to firing unconditionally.
- `LegacyoftheAbyss.sln` – solution including the runtime project and the xUnit test project under `Tests/`.
- `Tests/` – xUnit unit tests focused on persistence, charm stat maths, notifications, and config serialisation. These execute entirely in managed code (no Unity runtime required).
- `Assets/` – runtime asset folder used for configuration (`config.json`) and generated logs (`logs/`). `ModConfig` reads and writes directly to this directory, resolved relative to wherever the *loaded* DLL sits — see "Asset & logging paths" below, this is exactly what the `bin`/`obj` relocation above is protecting.
- `DLLDecompiles/` – decompiled Silksong assemblies for reference only.
- `LegacyoftheAbyss-DevBuild/` (sibling of this repo, at the game root) – all `bin`/`obj` output lands here now; safe to delete entirely at any time, it will be recreated on the next build.

## Build & packaging
1. Install the .NET 7 SDK (builds target `netstandard2.1` but the tests use `net7.0`).
2. Restore dependencies once with `dotnet restore` (the project already points at the BepInEx NuGet feed for Unity & Silksong libraries).
3. Build with `dotnet build -c Release` (from the repository root). The main artefact lands at `<game root>/LegacyoftheAbyss-DevBuild/bin/LegacyoftheAbyss/Release/netstandard2.1/LegacyoftheAbyss.dll` — **not** under this project's own `bin/`, see Repository layout above.
4. **To actually test a change in-game**, build with `dotnet build -c Release -p:DeployLocalDevBuild=true` — this also copies the DLL & PDB to `BepInEx/plugins/LegacyoftheAbyss.dll` (what a direct/Steam launch loads) and, when `DevProfile.props` is present, into the mod manager profile's `BepInEx/plugins/LegacyoftheAbyss-Dev/` (what a manager launch loads). A build without this flag compiles but does not deploy anywhere.
5. **Optional (distribution, not local dev):** to package a distributable zip for Nexus/Thunderstore, create a `SilksongPath.props` file that defines `SilksongFolder` (e.g. `dotnet new silksongpath --SilksongFolder="C:\Games\Silksong"`), and build with `-p:CreateDistributionPackages=true`.
6. Publishing itself is a manual GitHub Actions run, not part of any build here. The packaging traps it is designed around (tcli's silent empty-package failure, the trailing-slash bug, the unversioned Thunderstore staging folder, the pinned Nexus action tag) are documented on the [Publishing a Release](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Publishing-a-Release) wiki page. Read it before touching `.github/workflows/publish.yml`, `thunderstore.toml`, or the `PrepareReleasePackages` target.

## Testing
- Run `dotnet test -c Release` to execute the xUnit suite in `Tests/`.
- The tests exercise `ModConfig` serialisation, `ShadeRuntime` notification queues, and charm stat calculations without requiring a Unity runtime. If a Silksong installation is available, the test project will copy real game assemblies beside the test binaries for richer coverage, but this is optional.
- Test output (and any `Assets/config.json` the tests write) lands under `LegacyoftheAbyss-DevBuild/bin/LegacyoftheAbyss.Tests/...`, not in this repository's own `Assets/` folder, so there is nothing to restore with `git checkout` afterward.

## Asset & logging paths
- `ModConfig` resolves assets (`ModPaths.Assets`) relative to wherever the *loaded* DLL physically sits, not relative to this repository. For this install, that DLL is the flat `BepInEx/plugins/LegacyoftheAbyss.dll` deployed by `DeployLocalDevBuild`, so the mod actually reads and writes `BepInEx/plugins/Assets/` — a folder that sits **alongside this repository, not inside it**, and holds this install's live config and save-slot state. This repository's own `Assets/` folder is checked-in template/reference content (sprites, default `config.json`, `charm_placements.json`); it is a source for new assets to be added to, not the directory the running game reads from. Keep this distinction in mind before assuming a change to a file under this repo's `Assets/` will be visible in-game — it won't be, until also copied to `BepInEx/plugins/Assets/`.
- Shade sprite sheets live in `Assets/Knight_Shade_Sprites/`. Alternate skins go in `Assets/Knight_Shade_Sprites/Skins/<Skin Name>/` and only need the sheets they override — `ShadeSkinManager.ResolveSpritePath` falls back to the built-in set for anything missing. `Skins/skins.json` optionally controls menu order and display names. All sheet loading must stay routed through `LoadShadeSprites` in `LegacyHelper.ShadeController.Core.cs` or skin switching will silently stop working for the new sheet.
- Logs are written under `Assets/logs/` by `LoggingManager`. Clean up this folder before committing if you run the mod locally.

## Bug reports
- `Diagnostics/` holds the in-game bug capture system. Pressing the hotkey (`bugReportHotkey`, default `F8`) freezes the game, takes a screenshot of the frame *before* the overlay drew, snapshots game state, and writes everything alongside whatever message is typed into the overlay.
- A report is a folder of `report.md` (the summary you read first), `state.json` (full snapshot including the mod config and every loaded plugin), `log.txt` (the captured log ring, all BepInEx sources), `flight.csv` (rolling state samples leading into the capture) and `screenshot.png`.
- Reports are written to `ModPaths.UserData/bug_reports/` — i.e. `BepInEx/config/LegacyoftheAbyss/bug_reports/`, **outside this repository**, for the same update-safety reason save data lives there. They can never be committed by accident, and there is nothing to clean up before committing.
- Use the `/bug-triage` slash command (`.claude/commands/bug-triage.md`) to list open reports, work one, or close it out. `index.md` in that folder is the open/fixed ledger.
- The pieces: `BugReportLogRing`/`BugReportLogCollector` tap the whole BepInEx listener chain; `BugReportFlightRecorder` samples Hornet/Shade state on an interval; `BugReportStateCollector` reads the point-in-time snapshot; `BugReportStore` renders and writes; `BugReportSystem` is the `DontDestroyOnLoad` MonoBehaviour that drives all of it and hosts the IMGUI overlay.
- Unhandled exceptions from mod code auto-file a report with no typing involved (`bugReportAutoCaptureExceptions`), deduped by message plus first stack frame and capped per session so one throw in `Update` cannot write a report per frame.
- `BugReportSystem.IsCapturingText` is the mod-wide "the overlay owns the keyboard" flag. Anything that polls keys directly must respect it — `LegacyHelper.Update` and `ShadeInput.ShouldSuppressOption` already do. A new key handler that ignores it will fire on every matching letter typed into a report.
- Everything except the MonoBehaviour is plain managed code and is covered by `Tests/BugReportTests.cs`.
- The same system written up in full, including the capture sequence and the report file formats: [Bug Report System](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Bug-Report-System).

## Patching the game safely

Most of this repo is Harmony patches and reflection against an assembly we do not control, and that
combination has a specific failure mode: **a patch that does nothing looks exactly like a patch that
was not needed.** Several days of this project were lost to fixes that never ran once. The rules
below all exist because something here silently did nothing.

- **Never name an overloaded method through a `[HarmonyPatch]` attribute.** `AccessTools` resolves it
  with `Type.GetMethod(name, flags)`, which throws `AmbiguousMatchException` on more than one match -
  and the shipped assembly carries overloads the decompiled reference under `Decompiles/` does not.
  Resolve targets with a `TargetMethods()` that filters by parameter shape and yields nothing when it
  cannot find a match, so an unrecognised assembly disables one feature instead of the whole mod.
- **Never trust a reflective lookup; assert it.** `FsmOwnerDefault` exposes its two members as public
  *properties* over non-public fields. Code that looked for public fields found nothing, returned
  "unavailable", and disabled an entire subsystem on every call for two rounds of testing without a
  single error anywhere. Any `GetField`/`GetProperty`/`GetMethod` against game or PlayMaker types
  belongs in a test that asserts it resolves - `Tests/FsmOwnerDefaultResolutionTests.cs` and
  `Tests/GrabGateResolutionTests.cs` are the pattern. Those tests run against the real assemblies.
- **A failed resolution must log and disable, never fail silently.** If a lookup comes back empty, say
  so once at startup. "Feature is off because it could not find X" is a five-second diagnosis; the
  same state with no message costs a play session.
- **Check that a physics API answers the question you are asking.** `Collider2D.IsTouching` and
  `Collider2D.Overlap`-without-a-mask consult the layer collision matrix; `Collider2D.Distance` is
  pure geometry. Using the wrong one silently returns "no contact" forever for any pair of layers that
  do not interact - which is most of the interesting ones, because the Shade sits on Default and the
  things that hit it do not.
- **The Shade's damage intake is not the hero's, and the differences bite.** Hornet is damaged only
  by something physically touching her `HeroBox`, resolved with `GetComponent` on the object she
  actually touched. The Shade scans for overlaps with no layer mask and used to resolve with
  `GetComponentInParent`. Both differences caused bugs in opposite directions: the walk-up charged
  it a boss's body-contact damage for touching any child trigger, so attack telegraphs hurt it, and
  the layer-blind scan let it be hit by colliders that cannot touch Hornet at all. `ResolveDamager`
  and `CouldReachHornet` in `LegacyHelper.ShadeController.Combat.cs` hold the line - keep new damage
  paths behind both.
- **Removing a bug can remove the behaviour it was accidentally providing.** Correcting that walk-up
  left the Shade completely immune to Lace's cross slash, because the attack's hitbox carries no
  damage component at all - it damages the hero by calling `HeroController` from an FSM, and the
  Shade had only ever been hit by it through the bug. When a fix removes a code path, check what was
  depending on it, and expect "it stopped working entirely" as the next report.
- **Patch classes are applied one at a time** (`PatchAllTolerantly` in `LegacyHelper.Core.cs`) rather
  than through `Harmony.PatchAll`, which rethrows the first failure out of `Awake` and takes the HUD,
  the Shade and the bug reporter down with it. Do not switch back.

## Diagnosing from a bug report

The reports are the only instrument for anything that needs a live game, so treat gaps in them as
bugs in the tooling rather than as a reason to ask for another repro.

- **Correlation in `flight.csv` is not causation.** It samples state on a timer; cause and effect
  routinely land in the same row. Two separate fixes were shipped against a boss attack on the
  strength of "the Shade entered this hitbox and Hornet was hurt two frames later", and both were
  wrong - the entry was a symptom. Only `events.csv` names an agent.
- **Record the decision, not just the action.** An interception that declines must say so and why. A
  category that never appears is ambiguous between "the code never ran", "it ran and chose not to act"
  and "the situation never arose", and distinguishing those has cost more round trips here than any
  other single thing. See the `hero-repositioned-by` lines, which are written whether or not anything
  is redirected.
- **Record the discriminator, not the verdict.** "Shade inside" is not enough; "Shade inside
  [shade:hero damager]" says which collider was consulted and lets a wrong reading be recognised. An
  early version reported "has a DamageHero" using `GetComponentInParent`, which was true of every
  collider on a boss and made an attack hitbox indistinguishable from a harmless detection range.
- **Add the emitter before the next repro, not after.** If a report cannot answer the question, the
  fix is a new event category in the same turn.
- **Confirm an interception fired before believing it works.** "The bug persists" and "the fix never
  ran" look identical from the outside, and on this project they were confused repeatedly - once for
  two full rounds while a subsystem sat dead behind a failed reflective lookup. Every interception
  writes an event when it engages; check for it first, before theorising about why the behaviour did
  not change.

## Writing the changelog

`CHANGELOG.md` is for players, and it is short. **One line per change, two at most.** Grouped bullets
under `### Fixed` / `### Added`, not a heading per entry.

- Say what changed for the player, and name the setting that controls it. Not the cause, not the
  mechanism, not what it used to do internally.
- The reasoning goes in the commit message and in the code comment next to the fix, where the next
  person to touch that code will actually find it. It has never once been needed in the changelog.
- **Only net changes reach it.** A bug introduced and fixed before release never happened as far as
  the changelog is concerned - delete both entries rather than narrating the round trip. An entry
  like "fixed the thing added three commits ago" is noise to everyone outside that week.
- Write the entry when the change is finished, not per attempt. A day of bug hunting that fixes four
  things is four lines.

For scale: this file went from 167 lines to 52 without losing a single distinct fact, because most of
it was explanation nobody had asked for.

## Additional tips
- Use the decompiled `Assembly-CSharp` sources to mirror in-game behaviour when implementing new features or Harmony patches.
- Shade charms and abilities are centrally defined via `ShadeCharmDefinition` and `ShadeCharmStatBaseline`; extend these classes when adding new modifiers to keep stat calculations consistent.
- Always validate gameplay logic with `dotnet build -c Release` and `dotnet test -c Release` before submitting changes.
- The large partial classes are split by responsibility, not chronology. Put new code in the file whose responsibility it matches rather than at the end of whichever file you opened first — [Code Map](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Code-Map) is the authority on which that is.
- Prefer a config switch for anything that reaches into hero damage, hero movement or enemy AI.
  `shadeBossAttackSharingEnabled` exists so a misbehaving interception can be turned off without a
  rebuild, which matters because these are exactly the features that cannot be verified outside a
  running game.
- When a change alters documented behaviour, say which wiki page needs updating (new config fields always mean [Configuration Reference](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Configuration-Reference); finished roadmap items are **removed** from [Roadmap](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Roadmap) rather than checked off, since `CHANGELOG.md` and the git log are the record).
