# File-splitting refactor — what moved where

Mechanical split only: every line of code was moved verbatim (via a Python line-range
extraction script, not retyped) into new files of the *same* class, now declared `partial`.
No logic was changed. I can't compile this in my sandbox (no network access to the
BepInEx/Silksong NuGet feed, no local .NET SDK), so please run `dotnet build -c Release`
(and `dotnet test -c Release`) before committing — that's the real verification step.

What I checked on my end, without a compiler:
- Every original line (all 16,193 across the three files) is accounted for exactly once
  across the new files — nothing dropped, nothing duplicated.
- Every method/property signature-like line appears exactly once in the combined output,
  matching the original file exactly (234 for ShadeInventoryPane, 121 for
  ShadeSettingsMenu, 142 for ShadeController.Core).
- Every output file has balanced `{}`, `()`, and `[]`.

## ShadeInventoryPane.cs (was 8,407 lines → now 7 files)

Also uncovered along the way: this file actually held **three** top-level types, not one
(`ShadeInventoryPane`, plus `SimpleCanvasNestedFadeGroup` and `ShadeInventoryPaneIntegration`
after it). Split accordingly:

- `ShadeInventoryPane.cs` (952 lines) — class declaration (now `partial`), fields/constants,
  nested structs/enums, text/style utility statics.
- `ShadeInventoryPane.Templates.cs` (1,024 lines) — template-root resolution, rect logging,
  MonoBehaviour lifecycle (Awake/OnEnable/OnDisable/OnDestroy), pane submit/inventory-changed handlers.
- `ShadeInventoryPane.RootSizing.cs` (1,455 lines) — ConfigureFromTemplate, standalone
  root-size fallback logic, Trajan font resolution, overlay canvas setup, inventory binding,
  pane list attach/detach.
- `ShadeInventoryPane.CharmGrid.cs` (1,382 lines) — charm grid layout metrics/columns,
  highlight rect, RebuildUI/BuildUI (UI construction), icon pool building, notch sprites,
  equipped-overcharm backdrop/bounds.
- `ShadeInventoryPane.Animation.cs` (1,642 lines) — overcharm attempt + charm-flight
  animations, shake animation, overlay animation bookkeeping, notch meter rendering,
  equipped row, selection/refresh state.
- `ShadeInventoryPane.InputAndDetail.cs` (735 lines) — shade/hero directional input polling,
  Update/LateUpdate, detail panel rendering, InventoryPane overrides.
- `ShadeInventoryPaneIntegration.cs` (1,309 lines, **new file**) — `SimpleCanvasNestedFadeGroup`
  and `ShadeInventoryPaneIntegration` (the reflection-based hook that registers the pane into
  the game's `InventoryPaneList`, plus input-binding capture/restore). These were never part
  of the `ShadeInventoryPane` class itself, just living in the same file — moved out whole,
  not split further, since they're already reasonably sized.

## ShadeSettingsMenu.cs (was 3,552 lines → now 5 files)

- `ShadeSettingsMenu.cs` (115 lines) — class declaration (now `partial`), fields/constants,
  logging helpers, `CancelTarget` enum.
- `ShadeSettingsMenu.Drivers.cs` (947 lines) — nested MonoBehaviour driver components
  (CancelRouter, Slider/Toggle/CharmButton drivers, CharmMenuController, BindingMenuDriver,
  ShadeToggleDriver, RowHighlightDriver, MenuFocusDriver) and their notify/preset-apply helpers.
- `ShadeSettingsMenu.Widgets.cs` (923 lines) — reusable widget/template construction:
  fallback sprites, slider/toggle templates, animator cloning, shadow/text style
  capture+apply, navigation wiring, CreateSlider/CreateToggle.
- `ShadeSettingsMenu.Screens.cs` (1,114 lines) — screen construction: DestroyScreens/
  StripTemplateComponents/InitializeScreen, content root + back button setup,
  CreateMenuButton, BuildMainMenu/BuildCharmsMenu/BuildDifficultyMenu/BuildControlsMenu/
  BuildLoggingMenu, show/hide helpers, HandlePauseToggle.
- `ShadeSettingsMenu.Lifecycle.cs` (525 lines) — top-level entry points: Build, Inject,
  Show, HideImmediate, Hide, Clear.

## LegacyHelper.ShadeController.Core.cs (was 4,234 lines → now 5 files)

This was already one of five partial slices of `ShadeController` (alongside Fields/
Persistence/Slash/Charms) — it just needed splitting further itself.

- `LegacyHelper.ShadeController.Core.cs` (677 lines) — aggroProxy fields, Start/OnDestroy,
  charm loadout application/recompute, persistence suppression, sprite loading, spawn
  entrance animation.
- `LegacyHelper.ShadeController.Movement.cs` (800 lines) — Update/FixedUpdate loop, leash/
  movement math (dynamic leash limits, axis clamping), HandleMovementAndFacing, sprint
  unlock/burst, fury-mode toggle.
- `LegacyHelper.ShadeController.Spells.cs` (719 lines) — fury aura, dash SFX trigger,
  HandleFire/HandleShriek entry points, DescendingDark cast, spell unlock/upgrade checks,
  AoE/shriek/quake/descend VFX spawning.
- `LegacyHelper.ShadeController.Combat.cs` (1,068 lines) — physics setup, collision-ignore
  management, damage intake (OnCollisionEnter2D/OnTriggerEnter2D/TryProcessDamageHero),
  Sharp Shadow dash hitbox, teleport/scene-transition protection, knockback, damage
  resolution + death.
- `LegacyHelper.ShadeController.FocusAndAudio.cs` (1,042 lines) — focus heal, Baldur Shell
  renderer/animation, SFX loading + playback, shade point-light setup/sync, teleport channel
  handling, AggroProxyTracker, hornet nail damage lookup.

## Not touched this pass

Multi-shade architecture groundwork (the static/singleton `ShadeRuntime`/`ShadeController`
state) — deliberately out of scope for this pass, per your last message. `Shade/*.cs`,
`LegacyHelper.ShadeController.Fields/Persistence/Slash/Charms.cs`, and everything else in
the project is untouched.
