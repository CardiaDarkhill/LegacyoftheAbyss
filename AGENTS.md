# AGENTS.md file

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
  - `DeployLocalDevBuild` (`-p:DeployLocalDevBuild=true`) – copies the freshly built DLL & PDB flat into `BepInEx/plugins/` (one level above this project). This is what makes a local rebuild actually show up in-game; without it, the build output stays in `LegacyoftheAbyss-DevBuild/` and the game keeps running whatever's already at `BepInEx/plugins/LegacyoftheAbyss.dll`.
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
4. **To actually test a change in-game**, build with `dotnet build -c Release -p:DeployLocalDevBuild=true` — this also copies the DLL & PDB to `BepInEx/plugins/LegacyoftheAbyss.dll`, which is what the running game actually loads. A build without this flag compiles but does not deploy.
5. **Optional (distribution, not local dev):** to package a distributable zip for Nexus/Thunderstore, create a `SilksongPath.props` file that defines `SilksongFolder` (e.g. `dotnet new silksongpath --SilksongFolder="C:\Games\Silksong"`), and build with `-p:CreateDistributionPackages=true`.

## Testing
- Run `dotnet test -c Release` to execute the xUnit suite in `Tests/`.
- The tests exercise `ModConfig` serialisation, `ShadeRuntime` notification queues, and charm stat calculations without requiring a Unity runtime. If a Silksong installation is available, the test project will copy real game assemblies beside the test binaries for richer coverage, but this is optional.
- Test output (and any `Assets/config.json` the tests write) lands under `LegacyoftheAbyss-DevBuild/bin/LegacyoftheAbyss.Tests/...`, not in this repository's own `Assets/` folder, so there is nothing to restore with `git checkout` afterward.

## Asset & logging paths
- `ModConfig` resolves assets (`ModPaths.Assets`) relative to wherever the *loaded* DLL physically sits, not relative to this repository. For this install, that DLL is the flat `BepInEx/plugins/LegacyoftheAbyss.dll` deployed by `DeployLocalDevBuild`, so the mod actually reads and writes `BepInEx/plugins/Assets/` — a folder that sits **alongside this repository, not inside it**, and holds this install's live config and save-slot state. This repository's own `Assets/` folder is checked-in template/reference content (sprites, default `config.json`, `charm_placements.json`); it is a source for new assets to be added to, not the directory the running game reads from. Keep this distinction in mind before assuming a change to a file under this repo's `Assets/` will be visible in-game — it won't be, until also copied to `BepInEx/plugins/Assets/`.
- Shade sprite sheets live in `Assets/Knight_Shade_Sprites/`. Alternate skins go in `Assets/Knight_Shade_Sprites/Skins/<Skin Name>/` and only need the sheets they override — `ShadeSkinManager.ResolveSpritePath` falls back to the built-in set for anything missing. `Skins/skins.json` optionally controls menu order and display names. All sheet loading must stay routed through `LoadShadeSprites` in `LegacyHelper.ShadeController.Core.cs` or skin switching will silently stop working for the new sheet.
- Logs are written under `Assets/logs/` by `LoggingManager`. Clean up this folder before committing if you run the mod locally.

## Additional tips
- Use the decompiled `Assembly-CSharp` sources to mirror in-game behaviour when implementing new features or Harmony patches.
- Shade charms and abilities are centrally defined via `ShadeCharmDefinition` and `ShadeCharmStatBaseline`; extend these classes when adding new modifiers to keep stat calculations consistent.
- Always validate gameplay logic with `dotnet build -c Release` and `dotnet test -c Release` before submitting changes.
