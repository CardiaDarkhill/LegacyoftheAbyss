# AGENTS.md file

## Where the documentation lives

Prose documentation is the [GitHub Wiki](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki). This file is self-contained enough for most tasks; the wiki holds the long-form material that used to be `ROADMAP.md`, `PUBLISHING.md` and `REFACTOR_NOTES.md`, all three deleted and superseded by it. Only `README.md`, `AGENTS.md` and `CHANGELOG.md` remain as repo-root markdown — **do not add new `.md` files to the repo root.**

| If the task is… | Read |
| --- | --- |
| "which file holds this method?" | [Code Map](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Code-Map) — per-file responsibility map for every partial class |
| Understanding how the pieces fit | [Architecture Overview](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Architecture-Overview) |
| Adding or changing a config field | [Configuration Reference](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Configuration-Reference) |
| Anything path-related (assets, logs, save data) | [Asset and Data Paths](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Asset-and-Data-Paths) |
| Build/deploy flags and the test suite | [Building and Testing](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Building-and-Testing) |
| Release, packaging or CI work | [Publishing a Release](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Publishing-a-Release) |
| Planned work, known bugs, feasibility notes | [Roadmap](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Roadmap) |
| Triaging or extending bug capture | [Bug Report System](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Bug-Report-System) |
| Input, charms or skins specifically | [Controls and Bindings](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Controls-and-Bindings), [Shade Charms](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Shade-Charms), [Characters](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Characters) |

The wiki is a separate git repository (`LegacyoftheAbyss.wiki.git`). Editing it is a separate clone and push — mention wiki updates a change makes necessary rather than assuming a code commit carries them.

## Project overview

**Legacy of the Abyss** is a BepInEx mod for *Hollow Knight: Silksong* adding a Shade companion that fights alongside the player, with its own config menus and persistence.

- Runtime behaviour lives in the `LegacyHelper` partial class (`LegacyHelper.*.cs`). The `BaseUnityPlugin` entry point is `LegacyHelper.Core.cs`, which wires up Harmony patches, HUD injection and shade spawning.
- **A companion is not "the Shade" any more.** `ShadeCompanionRegistry` holds one `ShadeCompanion` per slot, each with its own character, health, soul, charms and body; `ShadeRuntime` keeps its static API but delegates to the primary. The trap that replaced the old singleton is quieter: `ShadeRuntime.Charms` and `ShadeController.PrimaryInstance` are the *primary's*. Scale a companion's own stats from `OwnCharms`, and broadcast over `ShadeController.ActiveInstances` — reading the primary where you meant "each" is silent and looks exactly like the feature not working.
- **The Knight is a second character on that same controller,** so charms, damage and persistence are shared, but movement and rendering are not: it is a platformer body (`…ShadeController.KnightMovement.cs`) drawing through a tk2d animator rather than sprite sheets. Anything touching movement or rendering has to branch on it. Its unlocks follow `KnightAbilityMap`, which mirrors the Knight in Silksong mod's sync table rather than our Shade's combined spell track.
- The Shade is the nested `LegacyHelper.ShadeController` partial (`LegacyHelper.ShadeController.*.cs`): movement and leash (`Movement`), state fields and animation data (`Fields`), persistence hooks (`Persistence`), slash/projectile combat (`Slash`), charm-driven stat mutation (`Charms`).
- **Shade AI is split deliberately.** `Shade/Ai/ShadeAiBrain.cs` decides what to do and touches no Unity object, so `Tests/ShadeAiBrainTests.cs` covers it; `LegacyHelper.ShadeController.Ai.cs` gathers what it sees and applies what it decides; `…ShadeController.AiCommand.cs` holds the targeting reticle. Keep that line: if a decision needs a raycast, put the raycast on the driver side and pass the answer in via `ShadeAiSnapshot`.
- The AI adds no movement or combat code of its own. `Shade/Ai/ShadeAiInput.cs` publishes each decision as one frame of the same inputs a second player would give, so `CaptureMovementInput`, `HandleNailAttack`, `HandleFire` and the rest drive it unchanged. A new Shade ability reaches the AI by being added there, not by teaching the brain to call it.
- `Shade/` is the persistent data layer: `ShadeRuntime` (cross-scene state, notifications, charm loadouts), `ShadePersistentState` + `ShadeSaveSlotRepository` (health, soul, charm unlocks, save-slot separation), `ShadeCharmInventory` and the calculator/definition classes (charm effects and derived stats). `Shade/Ai/` is the exception — behaviour, not persistence.
- UI/HUD is `HUD.*.cs`; `ShadeSettingsMenu.cs`, `ShadeInventoryPane.cs` and their helpers build the custom pause-menu screens; `ShadeUnlockPopup.cs` and `ShadeUnlockPickup.cs` surface new-ability notifications.
- Input config is split between `HornetInput.cs` and `ShadeInputConfig.cs`; gameplay glue lives in `LegacyHelper.Projectile.cs`, `LoggingManager.cs` and `ModConfig.cs`.
- Reference material: decompiled game sources in `Decompiles/Assembly-CSharp` (the API reference for undocumented Silksong types), in-game internal names in `Decompiles/localization` (XML), and every FSM in `Decompiles/fsms_part_1.txt` and `fsms_part_2.txt` — over 75 MB each, so search them carefully.

## Repository layout

This repo is checked out inside the live `BepInEx/plugins/` folder of a Silksong install, for a fast edit-build-restart loop. **`bin`/`obj` are therefore relocated outside the `BepInEx/plugins` tree**: BepInEx recursively scans every `.dll` beneath it, so build output left in place becomes a second loadable copy of the plugin and a stale build can silently keep running. Do not bypass this redirection.

- `Directory.Build.props` — redirects `BaseOutputPath`/`BaseIntermediateOutputPath` for both projects to `../../../LegacyoftheAbyss-DevBuild/{bin,obj}/<ProjectName>/`, a sibling of `BepInEx/` at the game root. Picked up automatically; do not add a competing `BaseOutputPath` in either csproj.
- `LegacyoftheAbyss.csproj` — main plugin (netstandard2.1). Four deploy targets, all opt-in, so a plain `dotnet build` touches nothing outside `LegacyoftheAbyss-DevBuild/`:
  - `DeployLocalDevBuild` (`-p:DeployLocalDevBuild=true`) — copies DLL & PDB flat into `BepInEx/plugins/`, which is what makes a rebuild show up in-game. Implies `DeployDevProfile` when `DevProfile.props` supplies a profile path.
  - `DeployDevProfile` (`-p:DeployDevProfile=true`, needs a gitignored `DevProfile.props`) — copies into `<profile>/BepInEx/plugins/LegacyoftheAbyss-Dev/`. Thunderstore Mod Manager / r2modman point doorstop at the *profile's* BepInEx, so the game folder's plugins are never scanned on those launches. Keep the published Thunderstore package disabled in that profile: two copies share one BepInPlugin GUID and only one loads, unpredictably.
  - `CopyMod` (needs `SilksongPath.props`) — deploys the portable `BepInEx/plugins/LegacyoftheAbyss/` layout an end-user expects, and exports a zip.
  - `PrepareReleasePackages` (`-p:CreateDistributionPackages=true`, Release only) — stages Nexus/Thunderstore packages under `LegacyoftheAbyss-DevBuild/obj/...`. This once fired on every Release build and created loadable duplicate plugins; if stray plugin DLLs reappear under `BepInEx/plugins`, check it has not regressed.
- `LegacyoftheAbyss.sln` — runtime project plus the xUnit test project.
- `Tests/` — xUnit tests for persistence, charm stat maths, notifications and config serialisation. Pure managed code, no Unity runtime.
- `Assets/` — checked-in template/reference content (sprites, default `config.json`, `charm_placements.json`). See "Asset & logging paths" — this is **not** the folder the running game reads.
- `DLLDecompiles/` — decompiled Silksong assemblies, reference only.
- `LegacyoftheAbyss-DevBuild/` (sibling of this repo, at the game root) — all build output; safe to delete at any time.

## Build, test and packaging

1. Install the .NET 7 SDK (the plugin targets `netstandard2.1`, the tests `net7.0`).
2. `dotnet restore` once — the project already points at the BepInEx NuGet feed.
3. `dotnet build -c Release` from the repo root. The artefact lands in `LegacyoftheAbyss-DevBuild/bin/...`, **not** under this project's `bin/`.
4. **To test in-game:** `dotnet build -c Release -p:DeployLocalDevBuild=true`. A build without this flag compiles but deploys nowhere.
5. `dotnet test -c Release` runs the suite. Test output and any `Assets/config.json` the tests write land under `LegacyoftheAbyss-DevBuild/`, so there is nothing to `git checkout` afterwards. If a Silksong install is available the test project copies real game assemblies beside the test binaries for richer coverage.
6. Distribution zips: create a `SilksongPath.props` defining `SilksongFolder` (e.g. `dotnet new silksongpath --SilksongFolder="C:\Games\Silksong"`) and build with `-p:CreateDistributionPackages=true`. Publishing itself is a manual GitHub Actions run — read [Publishing a Release](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Publishing-a-Release) before touching `.github/workflows/publish.yml`, `thunderstore.toml` or `PrepareReleasePackages`.

Always validate with `dotnet build -c Release` and `dotnet test -c Release` before submitting.

## Asset & logging paths

- `ModConfig` resolves assets (`ModPaths.Assets`) relative to wherever the *loaded* DLL sits. For this install that is the flat `BepInEx/plugins/LegacyoftheAbyss.dll`, so the mod reads and writes `BepInEx/plugins/Assets/` — **alongside this repository, not inside it.** A change to this repo's `Assets/` is not visible in-game until copied there.
- The Knight's art is an asset bundle at `Assets/Knight/knight.bundle`, loaded lazily by `KnightAssets` only when a companion wears the Knight. Its materials keep their shaders by name only, so `MaterialShaderMap.json` has to re-point every one at a live shader or the Knight draws magenta; shaders are gathered across scene loads because one cannot be found until a scene using it has loaded.
- Shade sprite sheets live in `Assets/Knight_Shade_Sprites/`. Alternate skins go in `Assets/Knight_Shade_Sprites/Skins/<Skin Name>/` and need only the sheets they override; `ShadeSkinManager.ResolveSpritePath` falls back to the built-in set. `Skins/skins.json` optionally controls menu order and display names. **All sheet loading must stay routed through `LoadShadeSprites` in `LegacyHelper.ShadeController.Core.cs`** or skin switching silently stops working for the new sheet.
- Logs are written under `Assets/logs/` by `LoggingManager`. Clean that folder before committing if you have run the mod locally.
- Save data and bug reports live in `ModPaths.UserData` (`BepInEx/config/LegacyoftheAbyss/`), outside this repository, so a mod update cannot destroy them and they cannot be committed by accident.
- **The running game is the mod-manager profile, not the game folder.** `-p:DeployLocalDevBuild=true` copies to both, but `ModPaths.UserData` resolves next to the *loaded* DLL, so live saves, `config.json` and reports are under `Thunderstore Mod Manager/.../profiles/<profile>/BepInEx/config/LegacyoftheAbyss/`. The `shade_slot_*.json` still sitting in `plugins/Assets/` is the pre-migration location and editing it does nothing.
- **Read the Knight bundle rather than inferring what is in it.** `Tools/BundleInspect.py` answers what is in a prefab, what a clip's frames are, and which prefabs can play a given clip; `Tools/HudPreview.py` renders the art itself. Both open the bundle offline (UnityPy). `LoadAllAssets<T>()` returns only a bundle's *main* assets, so audio and textures referenced by components come back empty from it and must be reached through the components instead — that mistake produced a documented "the bundle ships no audio" that was wrong by 162 clips.
- **A borrowed prefab's name is not a promise about what it is or which way it faces.** `Charm Thorn Counter` is the charm's inventory icon, not its vines; `Grubberfly BeamD` is byte-for-byte `BeamL`, and `BeamU` is `BeamL` turned 90 degrees, which points it *down*. The game sets the real orientation from the FSM that spawns it, and that FSM is the first thing stripped from anything borrowed. Check with `BundleInspect.py prefabs` before naming one, and prefer taking one base prefab and orienting it in code over trusting four.
- **Effect prefabs ship switched off.** Their renderers and their animators are both disabled, because Hollow Knight enables them from the same FSM. A borrowed effect that is not woken instantiates, parents and positions perfectly and draws nothing — with no error anywhere. `KnightEffects.WakeArt` is the one place that puts them back; go through `KnightEffects` rather than instantiating a bundle prefab by hand.

## Bug reports

`Diagnostics/` holds the in-game capture system. The hotkey (`bugReportHotkey`, default `F8`) freezes the game, screenshots the frame *before* the overlay drew, snapshots state and writes it with whatever message is typed in.

- A report is a folder: `report.md` (read first), `state.json` (full snapshot including mod config and every loaded plugin), `log.txt` (captured log ring, all BepInEx sources), `flight.csv` (rolling samples leading into the capture), `screenshot.png`. `index.md` in `bug_reports/` is the open/fixed ledger.
- Use the `/bug-triage` slash command (`.claude/commands/bug-triage.md`) to list, work and close reports.
- The pieces: `BugReportLogRing`/`BugReportLogCollector` tap the BepInEx listener chain; `BugReportFlightRecorder` samples Hornet/Shade state on an interval; `BugReportStateCollector` reads the point-in-time snapshot; `BugReportStore` renders and writes; `BugReportSystem` is the `DontDestroyOnLoad` MonoBehaviour driving it and hosting the IMGUI overlay.
- Unhandled exceptions from mod code auto-file a report (`bugReportAutoCaptureExceptions`), deduped by message plus first stack frame and capped per session.
- `BugReportSystem.IsCapturingText` is the mod-wide "the overlay owns the keyboard" flag. Anything polling keys directly must respect it (`LegacyHelper.Update` and `ShadeInput.ShouldSuppressOption` do); a handler that ignores it fires on every matching letter typed into a report.
- Everything except the MonoBehaviour is plain managed code, covered by `Tests/BugReportTests.cs`.

## Patching the game safely

Most of this repo is Harmony patches and reflection against an assembly we do not control, and the failure mode is specific: **a patch that does nothing looks exactly like a patch that was not needed.**

- **`catch { }` around a *static* member of a game type is load-bearing; around an instance member it usually is not.** Reading even a private static field runs that type's initializer, and several call into the engine (`UIManager` does `Animator.StringToHash`), so they throw `SecurityException`/`TypeInitializationException` outside a player loop — which is where the pure-managed tests run, and they depend on those paths degrading to "nothing found". Stripping such a guard compiles and fails only at `dotnet test`. Guards around an object you already hold, around cached `FieldInfo.GetValue`/`SetValue`, or around plain Unity calls like `Destroy` and `Mathf.Atan2` guard nothing and should go.
- **Never name an overloaded method through a `[HarmonyPatch]` attribute.** `AccessTools` resolves it with `Type.GetMethod(name, flags)`, which throws `AmbiguousMatchException` on more than one match — and the shipped assembly carries overloads the `Decompiles/` reference does not. Resolve targets with a `TargetMethods()` that filters by parameter shape and yields nothing when it cannot match, so an unrecognised assembly disables one feature instead of the whole mod.
- **Never trust a reflective lookup; assert it.** `Tests/GameApiContract.cs` holds an assertion for every `GetField`/`GetProperty`/`GetMethod` against game or PlayMaker types, run against the real assemblies so a mismatch fails at `dotnet test`. Adding a lookup without adding a case there is how this project has shipped subsystems that never ran. (`FsmOwnerDefault`, for one, exposes its two members as public *properties* over non-public fields.)
- **A failed resolution must log and disable, never fail silently.** "Feature is off because it could not find X" is a five-second diagnosis; the same state with no message costs a play session.
- **Check that a physics API answers the question you are asking.** `Collider2D.IsTouching` and maskless `Collider2D.Overlap` consult the layer collision matrix; `Collider2D.Distance` is pure geometry. The wrong one returns "no contact" forever for any layer pair that does not interact — which is most of the interesting ones, because the Shade sits on Default and the things that hit it do not.
- **The Shade's damage intake is not the hero's.** Hornet is damaged only by something touching her `HeroBox`, resolved with `GetComponent` on the object touched. A layer-blind scan resolved with `GetComponentInParent` breaks both ways: it charges body-contact damage for touching any child trigger, and lets colliders that cannot touch Hornet at all hit the Shade. `ResolveDamager` and `CouldReachHornet` in `LegacyHelper.ShadeController.Combat.cs` hold that line — keep new damage paths behind both.
- **Removing a bug can remove the behaviour it was accidentally providing.** Lace's cross slash carries no damage component; it damages the hero by calling `HeroController` from an FSM, so the Shade had only ever been hit by it through a bug. When a fix removes a code path, check what depended on it and expect "it stopped working entirely" as the next report.
- **Patch classes are applied one at a time** (`PatchAllTolerantly` in `LegacyHelper.Core.cs`) rather than through `Harmony.PatchAll`, which rethrows the first failure out of `Awake` and takes the HUD, the Shade and the bug reporter down with it. Do not switch back.

- **The companion is on the hero's layer and tag on purpose**, so the world cannot tell it from Hornet. Anything that acts on the hero has to be taught the difference: `InteractableBase.AddInside`/`LocalAddInside` for benches, levers and doors, `TransitionPoint.TryDoTransition` for room changes, `TrackTriggerObjects.IsCounted` for ranges. A new "the companion did something only Hornet should" bug is almost always another `layer == 9` test somewhere.

## Borrowing the game's UI

The pause-menu screens clone the game's own prefabs, and **a clone brings everything, and what it brings is invisible until it draws on top of your own work.**

- **Clone the control, then cut it down through its own references.** Do not rebuild a borrowed widget by hand, and do not trust what is on an object you picked — a `Slider`'s own object also held a label, a value readout, a full-row cursor hotspot and two selection fleurs. `StripToSliderParts` walks up from `fillRect` and `handleRect`, keeps what else is drawn the same size as the fill's container, and destroys the rest. Identify nothing by name or component type: the game ships *two* text stacks (`TMPro` and `TMProOld`).
- **A clone measures against the hierarchy it came from.** Read sizes off the original, where its parent still exists; a clone under a holder of your own resolves stretched rects against nothing and measures zero. Decide on the original and apply to the clone by child index. These screens are drawn at about two thirds of the canvas (`StretchScreenOverCanvas`), so a rect copied off a screen that is not lands two thirds undersized — `SliderUnitScale` reconciles them. Resize rects rather than setting `localScale`: a scaled rect still reports its unscaled size to the layout around it.
- **Search scenes, not `Resources.FindObjectsOfTypeAll` alone.** It also returns prefab assets, in unstable order, so the same code clones a live row one run and a generic prefab the next. Filter on `gameObject.scene.IsValid()`.
- **A cloned `MenuButton` must be `MenuButtonType.Activate`.** Every other type calls `ForceDeselect()` in `OnSubmit`, clearing the EventSystem's selection, so a toggle throws the highlight back to the screen's default row and `ShowScreen` has nothing to remember when a row opens a sub-menu.
- **A `ButtonSkin` is two halves.** `skin.sprite` is often a blank key cap with the letter in `skin.symbol`, drawn on top; assign both, as `ActionButtonIconBase.GetButtonIcon` does, or keyboard prompts render as empty boxes.
- **Anything device-dependent has to be re-asked.** These screens are built seconds after launch, before a pad has necessarily been seen, and kept for the session, so a glyph resolved at build time shows key caps to a pad player all run. Subscribe to `InputHandler.RefreshActiveControllerEvent`, and redraw on `OnEnable` for what changed while the screen was shut.
- **Do not assume where a helper put the thing you need.** `CreateSlider` returns the row's selectable, which lives on the clone or on the row depending on the template, so deriving the row from `selectable.transform.parent` can move a whole panel. Hand the caller what it needs explicitly.
- **The Shade's Charms pane is drawn on its own screen-space canvas**, not under the pane GameObject, so the "Inventory Control" FSM's pane tween moves an empty transform. `ShadeInventoryPaneSlide` mirrors that transform onto the overlay rather than re-timing the animation. Convert by ratio, not by projecting through a camera: the inventory exposes no canvas to resolve one from, so `WorldToScreenPoint` reads the pane's world position as screen pixels and yields a 15px slide on a 4K display.
- **Scene darkness has three parts, and a light needs two of them.** `Hero_Hornet(Clone)/Vignette` (sorting layer `Vignette`, shader `Sprites/Darkness Sprite`) is the hero-centred overlay that darkens the room. Hornet stays lit because `white_light_donut` sits on the `Over` sorting layer, above it, with a `Sprites/Screen` blend; and because `Vignette Cutout` on the **TransparentFX** layer - the only layer `DarknessCameraEffect`'s camera renders - feeds the `_DarknessCutout` texture that shader samples. Cloning the glow alone lights nothing. `EnsureShadeLight` copies both, and re-centres by rendered bounds because Hornet's rig hangs its glow ~5.7 units above her transform origin.
- **A `Canvas` rewrites its own `RectTransform` every frame** to match the screen, so anything written to that rect - position, size, anchors - is silently discarded. Content that needs to move lives under a child container, which is what `overlayRoot` is.

## Changing anything visual

Placement, rotation and scale cannot be reasoned out from sprite dimensions. Every attempt cost a round trip for something an eye catches instantly.

- **Look at the art before writing code against it.** `Tools/HudPreview.py` extracts each sprite, renders both rotations of anything the atlas stored turned, and composes a mock layout. It caught a 90-degree rotation, a mirror, and a plate that already contained the disc being drawn behind it.
- **tk2d packs sprites rotated.** `tk2dSpriteDefinition.flipped` says so, and a Unity `Sprite` has nowhere to carry it, so the drawing code has to turn it back. Derive the displayed size from that too, or the layout uses the atlas orientation.
- **Rotation and a mirroring `localScale` compound.** Pick one. Mirror by where pieces are placed, not by a negative scale, wherever a rotation is also involved.
- **Measure the anchor off the art.** Borrowed pieces are rarely centred on what matters; `HudPreview.py`'s `socket_centre` derives `hudFrameSocketX/Y` rather than leaving it to be nudged.
- **Ship the knob, not the guess.** HUD layout values live in `config.json` and are re-applied every frame by `HUD.Tuning.cs`; **Ctrl+F5** rereads the file. Anything positional that cannot be verified here belongs there so the developer can dial it in against the screen in seconds.

## Diagnosing from a bug report

Reports are the only instrument for anything needing a live game, so treat gaps in them as bugs in the tooling rather than as a reason to ask for another repro.

- **Correlation in `flight.csv` is not causation.** It samples state on a timer; cause and effect routinely land in the same row. Only `events.csv` names an agent.
- **Record the decision, not just the action.** An interception that declines must say so and why. A category that never appears is ambiguous between "the code never ran", "it ran and chose not to act" and "the situation never arose". See the `hero-repositioned-by` lines, written whether or not anything is redirected.
- **Record the discriminator, not the verdict.** "Shade inside" is not enough; "Shade inside [shade:hero damager]" names the collider consulted and lets a wrong reading be recognised. "Has a DamageHero" via `GetComponentInParent` is true of every collider on a boss, making an attack hitbox indistinguishable from a detection range.
- **Add the emitter before the next repro, not after.** If a report cannot answer the question, the fix is a new event category in the same turn.
- **A decision made far from where the bug shows up belongs in the snapshot, not the log.** The settings menu is built seconds after launch and the log ring keeps a few hundred lines, so the line naming the slider template is gone by the time anyone reports the sliders look wrong. `BugReportState`'s `Menu slider template` row carries it instead, and `state.Config` covers settings the same way. Extend that pattern rather than adding a log line nobody will still have.
- **A per-frame thrower starves the log ring.** It folds consecutive identical lines now, but check for one before concluding that code did not run — DebugMod's `GUIController` NRE spent the whole 800-line ring for a day and hid three separate diagnoses.
- **Confirm an interception fired before believing it works.** "The bug persists" and "the fix never ran" look identical from outside. Every interception writes an event when it engages; check for that first.

## Writing the changelog

`CHANGELOG.md` is for players and it is short. **One line per change, two at most**, as grouped bullets under `### Fixed` / `### Added`, not a heading per entry.

- Say what changed for the player and name the setting that controls it. Not the cause, not the mechanism, not what it used to do internally. The reasoning belongs in the commit message and in the code comment next to the fix.
- **Only net changes reach it.** A bug introduced and fixed before release never happened — delete both entries rather than narrating the round trip.
- Write the entry when the change is finished, not per attempt. A day of bug hunting that fixes four things is four lines.

## Additional tips

- Ask the developer for a bug report under specific conditions when that removes guesswork from a diagnosis.
- Shade charms and abilities are defined centrally via `ShadeCharmDefinition` and `ShadeCharmStatBaseline`; extend those when adding modifiers so stat calculations stay consistent.
- The large partial classes are split by responsibility, not chronology. Put new code in the file whose responsibility it matches — [Code Map](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Code-Map) is the authority.
- Prefer a config switch for anything reaching into hero damage, hero movement or enemy AI. `shadeBossAttackSharingEnabled` exists so a misbehaving interception can be turned off without a rebuild — exactly the features that cannot be verified outside a running game.
- When a change alters documented behaviour, say which wiki page needs updating. New config fields always mean [Configuration Reference](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Configuration-Reference); finished roadmap items are **removed** from [Roadmap](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Roadmap) rather than checked off, since `CHANGELOG.md` and the git log are the record.
- Comments here earn their place by what they let the next person change safely. Keep the constraint, drop the account of how it was discovered.
