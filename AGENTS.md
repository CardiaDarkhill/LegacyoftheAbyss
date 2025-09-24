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

## Repository layout
- `LegacyoftheAbyss.csproj` – main plugin project (netstandard2.1). Imports `SilksongPath.props` if present and defines a `CopyMod` target that zips & copies the built DLL into the game install.
- `LegacyoftheAbyss.sln` – solution including the runtime project and the xUnit test project under `Tests/`.
- `Tests/` – xUnit unit tests focused on persistence, charm stat maths, notifications, and config serialisation. These execute entirely in managed code (no Unity runtime required).
- `Assets/` – runtime asset folder used for configuration (`config.json`) and generated logs (`logs/`). `ModConfig` reads and writes directly to this directory.
- `DLLDecompiles/` – decompiled Silksong assemblies for reference only.
- `obj/` and `bin/` – standard MSBuild output directories (safe to clean).

## Build & packaging
1. Install the .NET 7 SDK (builds target `netstandard2.1` but the tests use `net7.0`).
2. Restore dependencies once with `dotnet restore` (the project already points at the BepInEx NuGet feed for Unity & Silksong libraries).
3. Build with `dotnet build -c Release` (from the repository root). The main artefact is `bin/Release/netstandard2.1/LegacyoftheAbyss.dll`.
4. **Optional:** to automatically copy the plugin into a local Silksong install and produce a distributable zip, create a `SilksongPath.props` file that defines `SilksongFolder` (e.g. `dotnet new silksongpath --SilksongFolder="C:\Games\Silksong"`). When present, the `CopyMod` MSBuild target copies the DLL & PDB into `<SilksongFolder>/BepInEx/plugins/LegacyoftheAbyss` and exports a zip into `bin/Publish/`.

## Testing
- Run `dotnet test -c Release` to execute the xUnit suite in `Tests/`.
- The tests exercise `ModConfig` serialisation, `ShadeRuntime` notification queues, and charm stat calculations without requiring a Unity runtime. If a Silksong installation is available, the test project will copy real game assemblies beside the test binaries for richer coverage, but this is optional.
- **Important:** `ModConfigTests` writes to `Assets/config.json`. After running the tests, restore the baseline config with `git checkout -- Assets/config.json` before committing.

## Asset & logging paths
- `ModConfig` resolves assets relative to the compiled assembly location. In development it will fall back to this repository's `Assets/` directory, so ensure any new runtime assets live here.
- Logs are written under `Assets/logs/` by `LoggingManager`. Clean up this folder before committing if you run the mod locally.

## Additional tips
- Use the decompiled `Assembly-CSharp` sources to mirror in-game behaviour when implementing new features or Harmony patches.
- Shade charms and abilities are centrally defined via `ShadeCharmDefinition` and `ShadeCharmStatBaseline`; extend these classes when adding new modifiers to keep stat calculations consistent.
- Always validate gameplay logic with `dotnet build -c Release` and `dotnet test -c Release` before submitting changes.
