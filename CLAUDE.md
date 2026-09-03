# Legacy of the Abyss

A BepInEx/HarmonyX mod for *Hollow Knight: Silksong* adding a second-player companion — the Shade, or the Knight — with its own charms, spells, HUD, menus and persistence.

## Where the documentation lives

Long-form prose is the [wiki](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki), a **separate git repo** (`LegacyoftheAbyss.wiki.git`): a code commit never carries wiki edits, so say which page a change makes stale. Only `README.md`, `CLAUDE.md` and `CHANGELOG.md` live in the repo root — **add no new root `.md`**; handover notes go in `Docs/`.

| Task | Page |
| --- | --- |
| "which file holds this method?" | **[Code Map](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Code-Map)** — the authority on where new code goes |
| How the pieces fit | [Architecture Overview](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Architecture-Overview) |
| Adding/changing a config field | [Configuration Reference](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Configuration-Reference) (always update) |
| Assets, logs, save data | [Asset and Data Paths](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Asset-and-Data-Paths) |
| Build flags, tests | [Building and Testing](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Building-and-Testing) |
| Release, packaging, CI | [Publishing a Release](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Publishing-a-Release) |
| Planned work, known bugs | [Roadmap](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Roadmap) — finished items are **deleted**, not ticked |
| Bug capture internals | [Bug Report System](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Bug-Report-System) |
| Input / charms / characters | [Controls and Bindings](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Controls-and-Bindings), [Shade Charms](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Shade-Charms), [Characters](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Characters) |

## Architecture

Runtime behaviour is the `LegacyHelper` partial (`LegacyHelper.*.cs`); `LegacyHelper.Core.cs` is the `BaseUnityPlugin` entry point wiring Harmony, HUD and spawning.

- **A companion is not "the Shade".** `ShadeCompanionRegistry` holds one `ShadeCompanion` per slot with its own character, health, soul, charms and body. `ShadeRuntime.Charms` and `ShadeController.PrimaryInstance` are the **primary's** — scale a companion's own stats from `OwnCharms`, and broadcast over `ShadeController.ActiveInstances`. Reading the primary where you meant "each" is silent and looks exactly like the feature not working.
- **The Knight is a second character on the same controller.** Charms, damage and persistence are shared; movement and rendering are not — it is a platformer body drawing through a tk2d animator, so anything touching either must branch on `UsesGroundedMovement`. Its files: `…ShadeController.KnightMovement.cs` (the step, jump, dash, freezes), `.KnightTerrain.cs` (probes, mask, swept collision, step-over), `.KnightPogo.cs` (bounce, surface rules, balloon launch), `.KnightBench.cs`. Unlocks follow `KnightAbilityMap`, mirroring the Knight in Silksong mod's sync table, not our Shade's spell track.
- The Shade is the nested `LegacyHelper.ShadeController` partial: `Movement` (flight, leash), `Fields`, `Persistence`, `Slash` (nail, projectiles), `Combat` (damage intake), `Charms` (stat mutation), `Spells`, `SoulVessels`.
- **AI is split deliberately.** `Shade/Ai/ShadeAiBrain.cs` decides and touches no Unity object (so `Tests/ShadeAiBrainTests.cs` covers it); `…ShadeController.Ai.cs` gathers and applies. If a decision needs a raycast, do the raycast on the driver side and pass the answer in via `ShadeAiSnapshot`. The AI adds no movement or combat code: `Shade/Ai/ShadeAiInput.cs` publishes each decision as one frame of the inputs a second player would give, so a new ability reaches the AI by being added there.
- `Shade/` is the persistent data layer — `ShadeRuntime`, `ShadePersistentState` + `ShadeSaveSlotRepository`, `ShadeCharmInventory` and the charm definition/calculator classes. `Shade/Ai/` and `Shade/Knight/` are the exceptions (behaviour and assets).
- UI is `HUD.*.cs`, `ShadeSettingsMenu.*.cs`, `ShadeInventoryPane*.cs`. Input config splits `HornetInput.cs` (hers) / `ShadeInputConfig.cs` (the companion's).
- Reference: `Decompiles/Assembly-CSharp` is the API reference for undocumented Silksong types; `Decompiles/localization` is in-game names; `Decompiles/fsms_part_*.txt` are every FSM (75 MB each — search carefully).

## Repo layout and build

This repo is checked out **inside the live `BepInEx/plugins/` folder** for a fast edit-build-restart loop. BepInEx recursively scans every `.dll` beneath it, so `Directory.Build.props` redirects all `bin`/`obj` to `../../../LegacyoftheAbyss-DevBuild/` (a sibling of `BepInEx/`). Build output left in place becomes a second loadable copy of the plugin and a stale build silently keeps running. **Do not bypass this**, and do not add a competing `BaseOutputPath`.

Deploy targets on `LegacyoftheAbyss.csproj` (netstandard2.1), all opt-in:

- `-p:DeployLocalDevBuild=true` — flat into `BepInEx/plugins/`; **this is what makes a rebuild show up in-game.** Implies `DeployDevProfile` when `DevProfile.props` exists.
- `-p:DeployDevProfile=true` — into the mod-manager profile. Keep the published Thunderstore package disabled there: two copies share one BepInPlugin GUID and only one loads, unpredictably.
- `CopyMod` (needs `SilksongPath.props`) — the portable end-user layout, plus a zip.
- `-p:CreateDistributionPackages=true` (Release) — stages Nexus/Thunderstore packages. This once fired on every Release build and created duplicate loadable plugins; if stray DLLs reappear under `plugins/`, check it has not regressed.

```
dotnet build -c Release                              # compiles, deploys nowhere
dotnet build -c Release -p:DeployLocalDevBuild=true  # to test in-game
dotnet test  -c Release                              # xUnit, pure managed, no Unity runtime
```

**Both must stay at zero warnings.** Validate with both before submitting. Test output lands under `LegacyoftheAbyss-DevBuild/`, so there is nothing to `git checkout` afterwards.

## Paths

- `ModPaths` resolves relative to the **loaded** DLL, so the mod reads `BepInEx/plugins/Assets/` — *alongside* this repo, not inside it. A change to this repo's `Assets/` is invisible in-game until copied there.
- **The running game is usually the mod-manager profile, not the game folder.** Live saves, `config.json` and bug reports are under `Thunderstore Mod Manager/.../profiles/<profile>/BepInEx/config/LegacyoftheAbyss/`. Anything in `plugins/Assets/shade_slot_*.json` is pre-migration and editing it does nothing.
- The Knight's art is `Assets/Knight/knight.bundle`, loaded lazily by `KnightAssets`. Its materials keep shaders by name only, so `MaterialShaderMap.json` must re-point every one at a live shader or the Knight draws magenta.
- Shade sheets are `Assets/Knight_Shade_Sprites/`; skins go in `Skins/<Name>/` and need only the sheets they override. **All sheet loading must stay routed through `LoadShadeSprites`** or skin switching silently stops working for the new sheet.
- Logs go to `Assets/logs/`. Clean before committing if you have run the mod.

## Working with the Knight bundle

- **Read it, do not infer.** `Tools/BundleInspect.py` (prefabs, clips, frames, hosts) and `Tools/HudPreview.py` (renders the art, both rotations, a mock layout) open it offline with UnityPy. `LoadAllAssets<T>()` returns only *main* assets — audio and textures referenced by components come back empty, which produced a documented "the bundle ships no audio" that was wrong by 162 clips.
- **A prefab's name is not a promise.** `Charm Thorn Counter` is the inventory icon, not the vines; `Grubberfly BeamD` is byte-for-byte `BeamL`, and `BeamU` is `BeamL` turned 90° — pointing it *down*. The FSM that set the real orientation is the first thing stripped. Prefer one base prefab oriented in code over trusting four.
- **Effect prefabs ship switched off** — renderers *and* animators disabled, because Hollow Knight enables them from the FSM. A borrowed effect that is not woken instantiates, parents and positions perfectly and draws nothing, with no error. Go through `KnightEffects` (its `WakeArt`) rather than instantiating by hand.
- **tk2d packs sprites rotated** (`tk2dSpriteDefinition.flipped`); a Unity `Sprite` cannot carry that, so the drawing code turns it back and derives the displayed size from it. `KnightAssets.IsSpriteRotated` resolves the frame first, because the packing is recorded while the sprite is cut — asking before building answers "not turned" for everything.
- **Size borrowed art by its ink, not its cell.** Two sheets can fill wildly different fractions of their frames (29% against 65%), so one "desired width" draws one effect at four units and the other at ten.

## Patching the game safely

Most of this repo is Harmony and reflection against an assembly we do not control, and the failure mode is specific: **a patch that does nothing looks exactly like a patch that was not needed.**

- **`catch { }` around a *static* member of a game type is load-bearing; around an instance member it usually is not.** Reading even a private static field runs that type's initializer, and several call into the engine, so they throw outside a player loop — which is where the pure-managed tests run, and they depend on those paths degrading to "nothing found". Stripping such a guard compiles and fails only at `dotnet test`. Guards around an object you already hold, or around `Destroy`/`Mathf`, guard nothing.
- **Never name an overloaded method in a `[HarmonyPatch]` attribute.** `AccessTools` uses `Type.GetMethod(name, flags)`, which throws `AmbiguousMatchException` — and the shipped assembly carries overloads `Decompiles/` does not. Use a `TargetMethods()` filtering by parameter shape that yields nothing when it cannot match.
- **Never trust a reflective lookup; assert it.** `Tests/GameApiContract.cs` holds a case for every `GetField`/`GetProperty`/`GetMethod` against game types, run against real assemblies. Adding a lookup without a case there is how this project has shipped subsystems that never ran.
- **A failed resolution must log and disable, never fail silently.** Patch classes are applied one at a time (`PatchAllTolerantly`), not via `Harmony.PatchAll`, which rethrows out of `Awake` and takes the HUD, companion and bug reporter with it.
- **Check the physics API answers the question asked.** `Collider2D.IsTouching` and maskless `Overlap` consult the layer collision matrix; `Collider2D.Distance` is pure geometry. The wrong one returns "no contact" forever for any layer pair that does not interact — which is most of the interesting ones.
- **The companion's damage intake is not the hero's.** Hornet is hit only through her `HeroBox`, resolved with `GetComponent` on the object touched. `GetComponentInParent` charges body-contact damage for touching any child trigger. `ResolveDamager` and `CouldReachHornet` hold that line — keep new damage paths behind both.
- **`Ignore Raycast` (layer 2) is the game's switched-off layer** — `HeroController` moves *itself* there on death. Colliders there are refused as damage sources; that was two "phantom hitbox" reports.
- **`HitTaker.Hit` already hits every `IHitResponder` up three parents**, and an enemy's `HealthManager` is one. Calling `hm.Hit` after it applies the whole `HitInstance` twice — every spell dealt double for months.
- **The companion shares the hero's layer and tag on purpose**, so the world cannot tell it apart. Anything acting on the hero must be taught the difference (`InteractableBase.AddInside`, `TransitionPoint.TryDoTransition`, `TrackTriggerObjects.IsCounted`). A new "the companion did something only Hornet should" bug is almost always another `layer == 9` test.
- **Removing a bug can remove the behaviour it accidentally provided.** Lace's cross slash damages the hero from an FSM, so the companion had only ever been hit by it through a bug. Expect "it stopped working entirely" as the next report.
- **The game clears bindings before remapping them** (`InputHandler.LoadSavedInputBindings`, on every save-store mount). Patching only the remap half does not decline to add bindings — it strips Hornet of hers. `HornetInput.EnsureHornetKeyboardBindings` re-asserts the invariant every frame.

## Unity traps that compile

- **`??=` and `??` do not see Unity's destroyed-object null.** A cached component whose object is gone is kept forever, and the `!= null` at the point of use then skips silently. Use `if (!field)`.
- **Assigning a `Vector2` into `transform.position` fills z with zero**, walking an object off the playable plane (`|z - 0.004| <= 1.8`, `Extensions.IsOnHeroPlane`). Silksong's background scenery is the same prop pushed back in z with colliders intact, so any geometry probe must ask.
- **Resolve before you ask.** Several lookups here record a fact as a side effect of building the thing; querying first answers with the default, and the result is order-dependent — right on the second call and wrong on the first.
- Enumerating `ReadOnlyCollection<T>` allocates; index it in per-frame code.

## Fidelity to Hollow Knight

Where the mod reproduces HK, use HK's own figures and cite them. `Shade/ShadeSpellDamage.cs` holds spell damage **flat** — Vengeful Spirit 15, Abyss Shriek 20×4 — *not* scaled off Hornet's needle. Hers scale because her silk skills have no upgrades; the Knight's spells upgrade in their own right and its spell charms (Shaman Stone, Spell Twister, SOUL gain) have no equivalent on her side, so scaling compounds all three. Shaman Stone likewise differs per spell (33% / 47% / 51% / 50%), not one multiplier. Pogo rules are read off `HeroDownAttack` rather than invented. Tests assert each against the wiki's totals.

## Bug reports

`Diagnostics/` holds the in-game capture. `bugReportHotkey` (default `F8`) freezes the game, screenshots the frame *before* the overlay drew, and writes a folder: `report.md` (read first), `state.json` (full snapshot incl. config and loaded plugins), `log.txt`, `flight.csv`, `events.csv`, `screenshot.png`. `index.md` is the open/fixed ledger. Use `/bug-triage` to list, work and close. Unhandled mod exceptions auto-file, deduped and capped. `BugReportSystem.IsCapturingText` is the mod-wide "the overlay owns the keyboard" flag — anything polling keys must respect it.

**Reports are the only instrument for anything needing a live game, so treat gaps in them as bugs in the tooling, not as a reason to ask for another repro.**

- **Correlation in `flight.csv` is not causation** — it samples on a timer, so cause and effect land in the same row. Only `events.csv` names an agent.
- **Record the decision, not just the action.** A category that never appears is ambiguous between "never ran", "ran and declined" and "the situation never arose". An interception that declines must say so and why.
- **Record the discriminator, not the verdict.** "Shade inside" is not enough; "Shade inside [shade:hero damager]" names the collider consulted and lets a wrong reading be recognised.
- **A decision made far from where the bug shows up belongs in `state.json`, not the log.** The menu is built seconds after launch and the ring keeps a few hundred lines. The `Menu slider template`, `InputDevices` and `Sorting` rows exist for exactly that.
- **Add the emitter before the next repro, not after.** If a report cannot answer the question, the fix is a new event category in the same turn.
- **A per-frame thrower starves the ring.** Repeats are folded now, but check for one before concluding code did not run — DebugMod's `GUIController` NRE hid three diagnoses for a day.

## Borrowing the game's UI

A clone brings everything, and what it brings is invisible until it draws on top of your own work.

- **Clone the control, then cut it down through its own references.** A `Slider`'s object also held a label, a value readout, a full-row cursor hotspot and two fleurs. `StripToSliderParts` walks up from `fillRect`/`handleRect` and destroys the rest. Identify nothing by name or component type — the game ships *two* text stacks (`TMPro` and `TMProOld`). Destroy borrowed `EventTrigger`s too, or every row also opens the screen it was cloned from.
- **A clone measures against the hierarchy it came from.** Read sizes off the original, apply to the clone by child index. These screens draw at ~⅔ of the canvas (`StretchScreenOverCanvas`), so a rect copied off one that is not lands ⅔ undersized. Resize rects rather than setting `localScale`.
- **Search scenes, not `Resources.FindObjectsOfTypeAll` alone** — it also returns prefab assets in unstable order. Filter on `gameObject.scene.IsValid()`.
- **A cloned `MenuButton` must be `MenuButtonType.Activate`.** Every other type calls `ForceDeselect()` in `OnSubmit`, throwing the highlight back to the screen's default row.
- **Anything device-dependent must be re-asked.** These screens are built before a pad has necessarily been seen and kept for the session. Subscribe to `InputHandler.RefreshActiveControllerEvent` and redraw on `OnEnable`.
- **A `Canvas` rewrites its own `RectTransform` every frame**, so anything written to that rect is discarded. Content that moves lives under a child container.
- **Scene darkness has three parts and a light needs two**: the `Vignette` overlay, `white_light_donut` on the `Over` layer, and `Vignette Cutout` on **TransparentFX** feeding `_DarknessCutout`. Cloning the glow alone lights nothing (`EnsureShadeLight`).

## Changing anything visual

Placement, rotation and scale cannot be reasoned out from sprite dimensions — every attempt costs a round trip for something an eye catches instantly.

- **Look at the art first** with `Tools/HudPreview.py`. It has caught a 90° rotation, a mirror, and a plate that already contained the disc being drawn behind it.
- **Rotation and a mirroring `localScale` compound.** Pick one; mirror by placement.
- **Measure the anchor off the art** (`HudPreview.py`'s `socket_centre` derives `hudFrameSocketX/Y`).
- **Ship the knob, not the guess.** HUD layout lives in `config.json`, re-applied every frame by `HUD.Tuning.cs`; **Ctrl+F5** rereads it. Anything positional that cannot be verified here belongs there.

## Conventions

- **`CHANGELOG.md` is for players and it is short.** One line per change, two at most, under `### Fixed` / `### Added`. Say what changed for the player and name the setting that controls it — not the cause, not the mechanism. Only *net* changes reach it: a bug introduced and fixed before release never happened, so delete both entries. A day of hunting that fixes four things is four lines.
- **Comments earn their place by what they let the next person change safely.** Keep the constraint, drop the account of how it was discovered — except in regression-test docstrings, where the bug *is* the specification.
- Put new code in the file whose responsibility it matches; state where a partial should be split rather than appending. A split needs a matching Code Map row.
- Extend `ShadeCharmDefinition` / `ShadeCharmStatBaseline` for new modifiers so stat calculation stays consistent. Express a sweep over `ShadeInputConfig.AllActions`, never a hand-written list of every action — four such lists had already drifted apart.
- Prefer a config switch for anything reaching into hero damage, hero movement or enemy AI (`shadeBossAttackSharingEnabled` is the pattern) — exactly the features that cannot be verified outside a running game.
- Ask for a bug report under specific conditions when that removes guesswork from a diagnosis.
