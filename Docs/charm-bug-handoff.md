# Handoff: the charm bug backlog

Written 2026-08-30. 24 open reports, almost all from one charm-testing session on the
`Knight-in-silksong` branch. This is orientation plus a triaged worklist — read `CLAUDE.md` first,
it holds the rules; this holds the leads.

## Getting oriented

```bash
dotnet test -c Release                          # 459 tests, all green at handoff
dotnet build -c Release -p:DeployLocalDevBuild=true   # deploys to the game folder AND the profile
```

**The running game is the Thunderstore profile, not the game folder.** Live saves, `config.json`
and bug reports are under
`…/Thunderstore Mod Manager/DataFolder/HollowKnightSilksong/profiles/Silksong - Legacy/BepInEx/config/LegacyoftheAbyss/`.
The `shade_slot_*.json` in `plugins/Assets/` is a pre-migration copy and editing it does nothing —
that mistake has already been made once.

Reports: `bug_reports/<id>/` with `report.md` (read first), `state.json`, `log.txt`, `flight.csv`,
`events.csv`, `screenshot.png`. `index.md` is the ledger. Use `/bug-triage`.

**Save slot 2 has all 42 charms and 10 notches** for testing. `Ctrl` + `` ` `` also unlocks every
charm for a session but keeps the slot's own notch capacity, so it grants the charms without the
notches to wear them.

## Four rules that will save you a round trip

1. **A patch target resolved by name needs a case in `Tests/GameApiContract.cs`.** Harmony binds
   prefix parameters *by name*; a rename is a load error, not a compile error. That test has already
   caught one wrong parameter name (`col` vs `collider`) before it shipped dead.
2. **The companion shares Hornet's layer (9) and tag on purpose**, so the world cannot tell it from
   her. Every "the companion did something only Hornet should" bug is another `layer == 9` test:
   `InteractableBase.AddInside`/`LocalAddInside`, `TransitionPoint.TryDoTransition` and
   `TrackTriggerObjects.IsCounted` are already handled — see
   `LegacyHelper.Patches.ShadeInteraction.cs`.
3. **Read the Knight bundle, don't infer it.** `Tools/HudPreview.py` (UnityPy + Pillow) opens it
   offline. `LoadAllAssets<T>()` returns only *main* assets, so audio and textures referenced by
   components come back empty and must be reached through the components — that produced a
   documented "the bundle ships no audio" that was wrong by 162 clips.
4. **Anything positional you cannot see, ship as a knob.** `HUD.Tuning.cs` + `Ctrl+F5` (rereads
   `config.json`) exists because placing borrowed art from its dimensions was wrong four times
   running. Extend that pattern rather than guessing.

## Where the charm system lives

| Concern | File |
| --- | --- |
| Charm definitions, hooks, stat modifiers | `Shade/ShadeCharmInventory.Definitions.cs` |
| The stat pipeline (`BuildSnapshot`, baseline, modifiers) | `Shade/ShadeCharmDefinition.cs` |
| Charm state on the controller | `LegacyHelper.ShadeController.Charms.cs` |
| Summon charms (Weaversong, Glowing Womb, Grimmchild) | `Shade/ShadeCharmSummons.cs`, `LegacyHelper.ShadeCharmMinion.cs`, `LegacyHelper.ShadeController.CharmSummonEffects.cs` |
| Nail and slash | `LegacyHelper.ShadeController.Slash.cs` |
| Spells | `LegacyHelper.ShadeController.Spells.cs` |
| Focus | `LegacyHelper.ShadeController.Focus.cs` |
| Knight movement, animation, abilities | `LegacyHelper.ShadeController.KnightMovement.cs`, `Shade/Knight/KnightView.cs` |
| Charm menu layout | `ShadeInventoryPane.CharmGrid.cs` |

Stat charms go through `ShadeCharmStatModifiers` → `BuildSnapshot` → controller fields. Behaviour
charms use `ShadeCharmHooks` (`OnApplied` / `OnRemoved` / an update callback). If a charm needs a
number the baseline does not have, add it to `ShadeCharmStatBaseline` and `ShadeCharmStatModifiers`
together — `Shade/ShadeCharmDefinition.cs` around the `NailDuration` addition is a worked example
of the full plumbing.

**Turn on "Damage Summary File"** (or `F1`) before testing damage. It now logs what the companion
*deals* per hit, with the nail's cooldown, so a charm's effect can be read off one swing instead of
counted with a stopwatch — which is how Quick Slash came to look broken when it was not.

## The worklist

Grouped by what I would tackle together. Report ids are prefixes; `/bug-triage <prefix>` opens one.

### A. Likely one shared cause — the bench hold (2 reports, start here)

- `20260830-142315` Knight never appears at the entrance after transitioning while Hornet sprints.
- `20260830-142501` Knight will not teleport to Hornet while she is benched, even when stuck.

`LegacyHelper.ShadeController.KnightBench.cs` pins the Knight and clears queued input every frame
while `atBench`, and I never exempted the teleport action — that almost certainly explains the
second, and the first may be the same hold catching a transition. Check `UpdateKnightBench` and the
`benchWalking` gate in `HandleKnightMovement` before looking anywhere else.

### B. Charms that do nothing (5)

- `20260830-144903` Sprintmaster inert for the Knight. Needs +20% walk speed *and* the sprint walk
  cycle. The Knight's speed comes from `KnightRunSpeed` in `KnightMovement.cs`, not from
  `charmSnapshot.MoveSpeed` — that is probably the whole bug, and it likely affects every
  movement-stat charm on the Knight. Worth checking that class of charm as a group.
- `20260830-153243` Sharp Shadow inert for the Knight.
- `20260830-153504` Grubberfly's Elegy "very broken".
- `20260830-160054` Weaversong hits things near the Knight instead of summoning minions.
- `20260830-161737` Gathering Swarm inert. The ask is to reuse Hornet's existing rosary-drag tool
  behaviour rather than implement a new one, since dragging to the companion is pointless.

### C. Missing visuals (8) — all want bundle art, see rule 3

- `20260830-153123` Sharp Shadow's own Shade Cloak animation (use it for the Shade too).
- `20260830-160311` Defender's Crest has no toxic cloud.
- `20260830-160735` Flukenest fires Shade Souls instead of flukes. The bundle has `Spell Fluke`,
  `Fluke Spell Anim` and `hero_fluke_*` audio.
- `20260830-161120` Spore Shroom: no spore burst at the end of a focus.
- `20260830-161316` Thorns of Agony: no animation; should deal exactly one nail slash of damage.
- `20260830-161459` Glowing Womb: minions have no sprites, may not spawn at all.
- `20260830-161920` Grimmchild: no animation. The user says the sprite and fireball are already in
  `Assets/` under the "Grimmchild 3" skin — use those rather than the bundle.
- `20260830-162706` Dreamshield has no sprite.

### D. Mechanics that are wrong rather than missing (6)

- `20260830-150240` Shape of Unn: the Knight should move at 50% speed while focusing (not be
  frozen), using the bundle's separate focus sprite set.
- `20260830-150724` The Knight takes no knockback from its own nail hits. Current behaviour is
  what Steady Body should give; normal hits should push it back slightly.
- `20260830-151328` 100 SOUL should give 3 focus heals (1 left over), gives 2. `focusSoulCost` and
  the SOUL bookkeeping in `LegacyHelper.ShadeController.Focus.cs`.
- `20260830-155542` Baldur's Shell grants i-frames during focus, which is wildly overpowered.
- `20260830-151540` Fragile charms unequip when they break. The ask is that they stay equipped:
  repairing happens at a bench, which is also where you would swap them, so unequipping only makes
  the player redo the loadout. A user-experience change, not a bug.
- `20260830-145956` Debug-key damage is cosmetic: masks come back on an area transition and focus
  cannot heal it. Suggests the debug key writes the HUD rather than the persistent state.

### E. UI and copy (3)

- `20260830-144410` Charm menu sometimes lays out too spread out and overlaps neighbouring UI.
  Intermittent — `ShadeInventoryPane.CharmGrid.cs` has a `FitCharmCellsToBox` pass and a guard that
  records failures as a `Shade pane layout failure` row on the next report. Check for that row.
- `20260830-162414` Dream Wielder and Nailmaster's Glory describe functionality that does not
  exist. The report carries the replacement copy — read it, do not invent wording.
- `20260830-151502` `NullReferenceException` in `SimpleHUD.LoseHealth` at `HUD.UI.cs:444`, setting
  `.color` on a destroyed mask Image. The coroutine outlives a HUD rebuild. **Note:** the mask
  images moved to a child object this week (`HUD.UI.cs`, `BuildMasks`), and `HUD.Tuning.cs` now
  touches them every frame, so re-check the line number before trusting the stack.

## Traps

- Two characters share this code. `UsesGroundedMovement` is the Knight; the Shade floats. A charm
  fix usually needs to work for both, and the user has asked for that explicitly before.
- Charm ids persist **by ordinal** in save slots. Append to `ShadeCharmId`, never reorder.
- `Destroy` defers to end of frame. Where something must not run even once —
  stripping the Knight rig, for instance — it has to be `DestroyImmediate`. That mistake turned two
  stray sounds into a Shade Soul flying across the room.
- The bug report log ring folds consecutive identical lines now, but a per-frame thrower can still
  crowd it. If expected logging is missing, check for one before concluding the code did not run.
