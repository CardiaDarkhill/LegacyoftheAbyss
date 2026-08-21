# Handoff: Shade Controls menu work (in progress)

Context for whoever picks this up next. Branch: `performance-and-refactor`. This document
covers a chain of related work spanning several sessions — read it before touching
`ShadeSettingsMenu.*.cs`, `LegacyoftheAbyss.csproj`, or `Directory.Build.props` again.

## How to build and actually see your change in-game

**This is not a normal `dotnet build`.** Read `AGENTS.md`'s "Repository layout" and "Build &
packaging" sections in full before doing anything else — they document a build-output
relocation and a deploy step that are easy to silently skip.

Short version: build output goes to `../LegacyoftheAbyss-DevBuild/` (a sibling of `BepInEx/`
at the game root), **not** this project's own `bin/`. A plain `dotnet build -c Release`
compiles but does not deploy. To actually update what the game loads:

```bash
dotnet build -c Release -p:DeployLocalDevBuild=true
```

Then the user needs to **restart the game** — the pause-menu screens are built once per
session and cached (`ShadeSettingsMenu.Build()` early-returns if already built for the
current `UIManager`), so re-opening an already-open menu will not pick up a new build.

If a fix "does nothing" after a rebuild, check `BepInEx/plugins/LegacyoftheAbyss.dll`
against `LegacyoftheAbyss-DevBuild/bin/LegacyoftheAbyss/Release/netstandard2.1/LegacyoftheAbyss.dll`
with `cmp` before assuming the code is wrong — a missed `-p:DeployLocalDevBuild=true` is a
resurfacing failure mode.

## What this whole thread of work is

1. A full codebase audit (performance + readability), all findings fixed.
2. A duplicate-plugin-copy bug found during that work (multiple loadable `.dll` copies
   under `BepInEx/plugins`, no way for BepInEx to tell which was newest) — fixed by
   relocating build output out of the scanned tree (see above) and by wiring the
   `[BepInPlugin]` version to the real build version (`LegacyHelper.Core.cs`).
3. A feature add: the debug HP/soul cheat keys (previously hardcoded, unrebindable) are now
   real `ShadeAction`s (`ShadeInputConfig.cs`), rebindable via the Controls menu, gated
   behind a `debugKeysEnabled` config flag surfaced in the renamed "Debug Options" menu.
4. **The current, unfinished thread**: adding those 5 debug rows to the Controls menu
   exposed pre-existing layout bugs in `ShadeSettingsMenu.Screens.cs`'s `BuildControlsMenu`
   that had nothing to do with the new rows — overlap, a scroll view that barely showed any
   content, and broken 2D keyboard/controller navigation. This has been through several
   rounds of screenshot-driven fixes.

## Known-good facts about this specific screen, established via logged diagnostics

- The user runs 4K (`Screen.width/height` = 3840x2160) but the UI canvas is logical
  1920x1080 (`ms.transform.rect` via `RectTransform`) — a 2x CanvasScaler factor.
  **`Screen.width`/`Screen.height` must never be used for sizing math on this screen** (or
  probably any screen in this file) — use `((RectTransform)ms.transform).rect.width/height`
  instead. This was wrong everywhere in `BuildControlsMenu` and has been fixed there
  specifically; **other `BuildXxxMenu` methods in this file have not been audited for the
  same bug** and may have it too, just less visibly (their content is short enough that the
  over-large margins didn't visibly break anything).
- `content`'s `VerticalLayoutGroup` (`childControlHeight/Width = true`, set up in
  `CreateContentRoot`) **does not reliably apply computed sizes to `BuildControlsMenu`'s
  direct children**, confirmed via logged diagnostics: a `LayoutElement.preferredHeight`
  that was demonstrably set to a correct nonzero value still resulted in the actual
  `RectTransform.rect.height` coming out as `0`, and a sibling never got sized past
  Unity's raw `100x100` default even after an explicit
  `LayoutRebuilder.ForceRebuildLayoutImmediate`. Root cause not found (see "Unresolved
  mysteries" below) — worked around by disabling `content`'s layout group for this screen
  specifically (`contentLayout.enabled = false;`) and positioning/sizing `info`,
  `presetRow`, and the scroll wrapper explicitly in code (a manually-tracked
  `sectionCursorY` stacks them top-to-bottom; see the top third of `BuildControlsMenu`).
- Separately, `LayoutElement` + a `Text` component **on the same GameObject**: the
  `LayoutElement`'s default `layoutPriority` (1) excludes the `Text` component's own natural
  size reporting (priority 0) **entirely**, regardless of what value the `LayoutElement`
  itself holds — including `-1`/"unset". This is *not* "unset falls through to the next
  component," despite that being Unity's documented-sounding behavior; it did not work that
  way in practice here. This caused the original preset-description overlap bug (two
  nested instances of it — the description's own `LayoutElement`, and separately each
  preset card's own `LayoutElement`). Fixed by measuring text height directly via
  `Text.preferredHeight` (which is unaffected by this — it's a plain property read on the
  `Text` component itself, no priority resolution involved) and stamping the result as an
  explicit `RectTransform.sizeDelta` at every level, rather than trusting any layout group's
  automatic propagation. See the long comment block inside `AddPresetOption` for the full
  reasoning.
- `Resources`/`ContentSizeFitter`-based auto-sizing was also unreliable for the scroll
  content (`bindingsRect`, three layout groups deep: row HorizontalLayoutGroup → column
  VerticalLayoutGroup → fixed-height buttons). Replaced with a direct arithmetic
  calculation (`rows * rowHeight + gaps * spacing`) since every row's height is fixed and
  known — no layout inference needed at all. See the `contentHeight` calculation near the
  end of `BuildControlsMenu`.
- **General lesson from this whole thread**: for this specific menu screen, prefer
  explicit, code-computed `RectTransform.sizeDelta`/`anchoredPosition` over any reliance on
  Unity's `LayoutGroup`/`LayoutElement`/`ContentSizeFitter` automatic propagation. Every
  single time automatic sizing was trusted here, it silently produced wrong values with no
  error or exception logged anywhere. This may be specific to this screen's structure
  (mixing hand-built layout groups with a `ScrollRect`, which additionally implements
  `ILayoutElement`/`ILayoutGroup` itself), or may generalize to other screens in this file —
  not established either way.
- `plain Debug.Log calls are not captured anywhere in this install's BepInEx/LogOutput.log`
  — this BepInEx setup logs `[Error: BepInEx] Unable to start Unity log writer` at startup,
  meaning its Unity log hook never initialized. Any temporary diagnostic logging **must**
  go through a `BepInEx.Logging.ManualLogSource` (e.g. `ShadeSettingsMenu`'s own `log`
  field) to actually show up. `Debug.Log` silently goes nowhere on this machine.
- Live save/config data lives at `BepInEx/plugins/Assets/` (sibling to this repo, not
  inside it) — see `AGENTS.md`'s "Asset & logging paths" section, this tripped up an
  earlier part of this same session (a since-fixed asset-resolution bug).

## Unresolved mysteries (did not chase further, in the interest of time)

- **Why** `content`'s `VerticalLayoutGroup` failed to apply computed sizes to its direct
  children was never actually root-caused — only worked around by disabling it. If the
  workaround (manual positioning) ever needs to be reverted or the same bug shows up
  elsewhere, this is worth actually solving properly with a debugger attached to the game,
  which wasn't available in this session (headless build-and-deploy only, no way to launch
  or inspect the live game from here).
- The preset-card horizontal padding has a pre-existing, clearly-backwards clamp:
  `Mathf.Clamp(canvasWidth * 0.04f, 36f, -80f)` (`sidePadding` in `BuildControlsMenu`) — min
  (36) is greater than max (-80), so this always evaluates to -80 (a negative padding).
  Predates this session's changes; **not fixed**, since its effect is presumably already
  accounted for in how the cards currently look, and changing it is a visual-design call,
  not a bug fix, given nobody has complained about the preset row's horizontal spacing.
  Flag it to the user before touching it.

## Current state as of this handoff (just deployed, unconfirmed)

Three most-recent changes, all in `BuildControlsMenu` (`ShadeSettingsMenu.Screens.cs`),
none yet confirmed in-game:

1. Increased the gap between the preset row and the binding list specifically (new
   `PresetToBindingsSpacing = 40f`, separate from the smaller `SectionSpacing = 20f` used
   between the info text and the presets).
2. Shrunk `BindingRowHeight` (70→58) and `bindingRowSpacing` (32→18) to fit more rows in
   the same vertical space.
3. Added auto-hide for the scrollbar: computed `needsScroll = contentHeight >
   scrollWrapperHeight`, and when false, hides the scrollbar GameObject, disables
   `scrollRect.vertical`, and widens the viewport to reclaim the space that was reserved
   for the scrollbar (`viewportRect.offsetMax`).

All three are reasoned-through but **not yet visually verified** — this session has no way
to launch the game, only to build/deploy and read back BepInEx's log file after the user
manually checks in-game. If the user reports another visual issue, the diagnostic log line
already in place (search for `"Controls layout:"` in `ShadeSettingsMenu.Screens.cs`, logged
via `log.LogInfo`) is still there and safe to extend further — it was invaluable for
resolving the resolution and layout-group bugs above, far more reliable than reasoning from
screenshots alone. Consider adding a similar line for the scrollbar-visibility decision if
that specific piece needs debugging.

**To check current state**: ask the user to restart, open the settings menu once, then
read `BepInEx/LogOutput.log` in the game's install directory directly (grep for `"Controls
layout"`) rather than asking for another screenshot first — it gives exact numbers instead
of an estimate, and has been strictly more useful every time it's been used in this thread.

Safe to delete this file once the Controls menu is confirmed working correctly and this
context has been absorbed / is no longer needed.
