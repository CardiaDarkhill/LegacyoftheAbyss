# Branch review — 4 September 2026

A second unattended pass over `Knight-in-silksong`, hunting defects and design oversights across the
whole source and cutting entropy as it went. Build stayed at zero warnings and the suite green after
every batch.

Tests 580 → 629. Two new test files.

## The one to look at first

**Two charms were charging the wrong number of notches.** Nailmaster's Glory 3 where Hollow Knight
charges 1, and Lifeblood Core 4 where it charges 3. Nailmaster's Glory is inert in this mod, so it
was charging three notches for a charm whose own description says it does nothing yet. Both
corrected.

Hollow Knight's table is the starting point here, not the rule — it makes some questionable choices
of its own and these are expected to move on feedback — so the costs are deliberately **not** pinned
by a test. A third charm, Carefree Melody, was flagged against a stale figure of mine and is
correct at 3; that is on me, not on the code.

## Defects found and fixed

| What | Where | Why it mattered |
| --- | --- | --- |
| The companion was yanked back to Hornet every 0.1s after a room load | `Combat.cs` | `SceneProtectionBlockedByOverlap` was the only one of three copies of the damage cascade missing `CouldReachHornet` and `IsSwitchedOffCollider`. A defeated enemy's collider on `Ignore Raycast` read as a live hazard, so the room-entry protection never expired: it re-armed, teleported the companion to Hornet, and left it invulnerable. Fixed by extracting `ClassifyDamage`, which all three now share. |
| The Knight could pogo off a corpse | `KnightPogo.cs` | `ClassifyKnightPogoSurface` reached its `HealthManager` test without ever asking about `Ignore Raycast`, the layer the game parks finished things on. |
| A charm's "new" marker came back on the next launch | `ShadeCharmInventory.cs` | `MarkCharmSeen` mutated the set without raising `StateChanged`, and that event is the only thing that writes the inventory to the slot. It cleared only if the player also happened to equip something. |
| A throw inside `LoadState` disabled every later notification | `ShadeCharmInventory.cs` | `_suppressStateChanged` was set and cleared without a `try/finally`, so one exception left the inventory permanently silent. |
| The config could be blanked | `ModConfig.cs` | `Save()` wrote `Serialize()`'s result unconditionally, and `Serialize` returns `string.Empty` when both Newtonsoft and Unity refuse. The empty string was what actually lost the settings. |
| A crash mid-write truncated the config or a save slot | `ModConfig.cs`, `ShadeSaveSlotRepository.cs` | Both used `File.WriteAllText` straight over the live file, and the slot is rewritten on every charm change. New `ModPaths.WriteFileAtomically` stages beside the target and swaps. A truncated file does not read as an error; it reads as no settings and no progress. |
| Save and load failures were silent | `ShadeSaveSlotRepository.cs` | Both `catch { }`. A slot that fails to load is indistinguishable from a slot that was never written. |
| The companion could be drawn magenta | `ShadeRendering.cs` | `??=` and `??` on a `Material` do not see Unity's destroyed-object null, so a lost default was kept and assigned — and with nothing resolved at all, `sharedMaterial` was set to null outright. |
| Fury of the Fallen's aura and the sprint burst were never drawn on a build without their shaders | `Spells.cs`, `Movement.cs` | A third `Shader.Find` for a name that had just answered null, then `new Material(null)`, which threw into the surrounding catch. |
| Every companion spawn leaked a texture and a sprite | `Slash.cs` | `MakeDotSprite()` built both afresh on each of its four call sites and destroyed neither. Now built once, `HideAndDontSave`, re-checked with Unity's null. |
| A failed effect spawn leaked its staging object and the instance on it | `KnightEffects.cs` | The stage was destroyed on two paths and not in a `finally`, so anything thrown past the inner catch left both in the scene for the session. |
| A malformed WAV divided by zero | `Audio.cs` | `channels` and `bitsPerSample` came straight off the file and were divided by. The catch turned it into "no such sound". |
| Turkish `I` | `LegacyHelper.Core.cs` | `DisableStartup` folded field names with `ToLower()`. |
| A preset change wrote `config.json` and the save slot nine times | `ShadeSettingsMenu.Difficulty.cs` | Pushing preset values into the sliders fires each one's `onValueChanged`, and every one of those asks for a refresh. `RefreshAll` guarded itself; the save sat outside that guard. |

## Subsystems that had never run

Both found by the reflection audit below.

- **`FindHurtClipsFromHornetFSM`** walked Hornet's PlayMaker FSM for damage clips through a field
  named `HeroFSM`. `HeroController` has no such field — the game names them `damageEffectFSM`,
  `sprintFSM` and so on — so it returned an empty list on every build since it was written. The
  name-scored scan beside it has always been what answered. Deleted, with the three reflective
  lookups, the property cache and `CollectClipsFromAction` that only it used: `HUD.Audio.cs` 293 → 182
  lines.
- **`InputDeviceBlocker.ShouldSuppressShadeOption`** was `=> false`, left behind when the "hand menus
  to Hornet while the Shade holds a controller" feature was replaced. Every Shade input read went
  through a `try/catch` around a constant.

## Reflection: from 51 untested lookups to none

`AGENTS.md` says a reflective lookup in shipped code has a matching assertion in
`Tests/GameApiContract.cs`. Fifty-one did not. All are now covered — and every one of them resolves,
except the two subsystems above, which is how they were found.

Two more contract tests came out of it:

- **`NoPatchNamesAMethodThatCannotBeResolved`** walks every `[HarmonyPatch]` in the assembly and
  fails on a method name that is overloaded (`AmbiguousMatchException`, which took the whole mod down
  once) or resolves to nothing (a patch that binds nothing). It counts what it checked, so it cannot
  pass by looking at nothing.
- **`EveryDifficultyValueSurvivesTheConfigRoundTrip`** is driven off the property list rather than a
  written-out set, so a new difficulty setting is covered by existing.

Where reflection was not needed at all it is gone: four sites resolved `TMPro.TextMeshProUGUI` by
assembly-qualified name and then fetched `text`, `color`, `fontSize` by string, while the charm pane
has referenced TMPro directly all along. Two of three `TryLoadImage` copies reflected for
`UnityEngine.ImageConversion` while the third called it outright.

## Entropy removed

**Dead code.** `ShadeUnlockPickup.cs` — 150 lines of MonoBehaviour nothing constructs, superseded by
`ShadeCharmSavedItem`, carrying an editor-only `Reset()` and a 3D `OnTriggerEnter` in a 2D game.
`s_heroConfigGroupFields`, the last of the removed AlternateSlash lookup. Seven write-only fields and
the work that produced them, including a whole `LoadSpriteSheet`. Three placement options the JSON
schema advertised and no handler read. `LogCharmHealthEvent(FormattableString)`, which overload
resolution never selected — so the invariant formatting it existed for never happened; applied at the
call sites instead. Fourteen parameters nothing read.

**Duplication collapsed.** `ApplyLeftSideLayout` held the saved layout and the live bindings as two
hand-written lists of the same eighteen keys, and the number row had already drifted between them
once — now one table, with `Tests/HornetKeyboardPresetTests` on it. `DifficultyPreset` and
`ShadeDifficultySettings` were parallel copies of the same ten values; the preset now holds one.
`ShadeAiInput` had two action lists differing by one entry. `ClearAndApplyShadows` existed twice and
only one copy checked anything. Two identical "patch each method tolerantly" loops. The damage
cascade, three times.

**Guards that guard nothing.** `ScoreTemplateRootCandidate` had six `try`/`catch` blocks around
instance reads on an object its own Unity null check had already proven live, each with a
double-initialised local. `ApplyAxisLimit` clamped two `ref` parameters both branches then overwrote.

**Splits.** `ShadeController.Update` 283 → ~190 lines: `UpdateSceneProtection`, `UpdateAggroProxy`,
`ClearMovementState`, `AbandonActionsInFlight`.

## Second round

**Taking a stat charm off left the companion worse than bare.** `RecomputeCharmLoadout` reset every
charm-derived stat to baseline and *then* called `ApplyCharmLoadout`, which dispatches `OnRemoved`
for charms that have left. Those hooks undo their own arithmetic - `MultiplyNailScale(1f / 1.25f)`,
`AddMaxHpBonus(-2)`, `AddSoulGainBonus(-3)` - so against an already-clean baseline they applied the
inverse rather than cancelling anything. Unequipping Mark of Pride left the nail at 0.8x its bare
length, Fragile Heart left the companion two masks under its own maximum, Quick Focus left focus
*slower* than base, Stalwart Shell left the damage stagger 2.5x longer. It stood until the next
recompute - another charm change, or a room load - happened to clear it.

The three steps now run in the order that makes them mean what they say: `OnRemoved` first, while
the arithmetic it undoes is still standing; then the reset, which is what actually removes a
charm's effect; then `OnApplied`. The reset moved into `ApplyCharmLoadout` as `ResetStatsToBaseline`
so the ordering lives in one method with the reason on it. `ResetCharmDerivedStats` also now clears
Flukenest, Spore Shroom and Gathering Swarm, which only their own `OnRemoved` had been clearing, so
that one method is a complete statement of "no charms equipped". The arithmetic halves of the
`OnRemoved` hooks are redundant now rather than harmful, and were left alone: removing twenty of
them from the definition table buys nothing and risks dropping a side effect that is not redundant.

`applyingCharmLoadout` fell out as genuinely unused once the hook dispatch was restructured - the
compiler said so, which the earlier IDE0052 sweep could not, because the field was read once to
save its own previous value.

Found by re-reading this pass's own changes and by two new scans.

- **The aggro proxy's throttle map grew without bound.** `_lastProxyEntryTimes` keyed a
  `Dictionary` by `Collider2D` and never dropped anything. The companion's body survives room
  changes, so it held an entry - and a live reference to a by-then destroyed collider - for every
  hitbox and detection range that had touched the proxy all session. Now pruned past a cap, of
  everything the throttle can no longer suppress.
- **Seven doc comments had drifted off their member**, stacked above the next one's, leaving one
  member with two `<summary>` blocks and another with none. `BeginKnightCastFreeze`,
  `ClearControllerKeyBindings`, `ClaimBackNavigation`, `KnightView.Play`,
  `ThePauseMenuListCanBeExtended`, `DifficultyPresetsAreDistinct` and `CouldReachHornet` were all
  undocumented as a result. A scan for the shape is two lines of Python and now reads zero.
- **A per-frame array allocation in the co-op camera.** `FindSecondPlayer` walked
  `ShadeCompanionRegistry.All`, which copies. `ShadeController.ActiveInstances` is the list kept for
  exactly this, and is now indexed rather than enumerated.
- `CreateDescriptionFooter` still built its label by hand in one of the two places, after the
  consolidation its own doc describes.
- `HandleFocus`'s completion path and `CancelFocus` were separate copies of the same nine
  statements, differing only in the order two of them ran in.

Three of this pass's own changes were walked back or tightened on review:

- `EnableCollisions` was switched to the cached collider list, which is only rebuilt when one of its
  entries is destroyed - so a collider added since (the aggro proxy, the Sharp Shadow box) would
  have been left switched on. Reverted to the fresh query, with a comment saying why.
- `DifficultyPreset.Values` handed out the shared settings instance, where the per-value properties
  it replaced were `private set`. It hands out a copy now, and `ApplyTo`/`Matches` use the field
  directly so the hot path does not pay for it.
- `RefreshDifficultyHeader`'s new re-entrancy guard sat above a one-line summary that belonged to
  the method, not the field.

## Third round — pulling the charm-hook thread

Fixing the hook *ordering* raised a second question: `OnApplied` runs on **every** recompute, and a
recompute happens on every charm change *and* every scene load (`BeginScene` -> `SyncActiveSlot` ->
`SetActiveSlot`, which flushes and recomputes even when the slot has not changed). So any hook whose
effect is a one-off repeats every time the player walks through a door.

**Fragile Heart healed the companion in full on every room transition.** Two defects, each hiding
the other:

- `AddMaxHpBonus`'s fill was `newLoadoutMax - before.NormalHp` — a top-up to the new maximum, not
  the masks just added. `previousLoadoutMax` was computed one line above and used only in the log
  line, which is what gave the intent away.
- `ResetStatsToBaseline` also wrote `shadeMaxHP = baseShadeMaxHP`. That field is derived output,
  owned by `ApplyCharmHealthModifiers` and recomputed from `baseShadeMaxHP + charmMaxHpBonus` —
  and `charmMaxHpBonus` *is* reset. Dropping the ceiling here as well lowered it mid-rebuild, and
  `CaptureCharmHealth` runs *after* it and clamps current health against it. A companion above the
  base maximum therefore read back short by exactly the charm's bonus.

Fix only the first and a full-health companion loses two masks per room; fix only the second and the
free heal remains. Both went together.

The first attempt at the fill was wrong in a way worth recording, because it is the trap this whole
area sets. It gated the fill on a `ShadeCharmContext.IsNewlyEquipped` flag, computed in
`ApplyCharmLoadout` by diffing against the charms the controller was already wearing. That reduced
the per-room heal from "full" to "+2" rather than removing it — because
`LegacyHelper.Core.cs:HandleFinishedEnteringScene` **respawns the companion on every scene change**
(the comment above `RetireShadeSheets` says so outright), so the controller is new every room and
has never seen any of its charms before. Nothing held on the controller can distinguish a fresh
equip from a rebuild.

What can is the maximum the companion is *already standing on*, which persistence restores with the
charm's masks counted:

```
fill = max(0, newLoadoutMax - max(previousLoadoutMax, currentMax))
```

Equipped mid-run, `currentMax` is the maximum without the charm and its two masks arrive filled; on
a respawn or a reload `currentMax` already includes them and nothing is owed, however many times the
rebuild runs. Unequipping gives a negative difference and so zero. The flag was removed rather than
left in place — a signal that reads "new" every room is worse than no signal, and the comment where
it stood now says why. `ResolveMaxHpFill` is a pure static so `ShadeCharmLoadoutTests` can pin all
five cases, as `ResolveResizeRefill` already is.

### The same defect, twice more, once seriously

Re-auditing the hook table against the *correct* model — `OnApplied` runs on a **fresh controller**
at every scene change — turned up two more one-off effects being redone every room.

**Hiveblood was broken outright.** Its `OnApplied` cleared `_hivebloodTimer` *and*
`_hivebloodPendingMaskRestore`. The second is the flag set when the companion takes damage, meaning
"a mask is owed"; clearing it at every door did not delay the ten-second regeneration, it cancelled
it. Hiveblood paid out only for a player who stood still in one room for ten seconds after being
hit — which in this game is close to never.

**Kingsoul** restarted its 1.5-second SOUL accrual the same way, costing up to a period per door.
Minor by comparison, and not worth a changelog line, but the same defect.

Both fixes are a deletion. The state lives on `ShadeCharmInventory`, which belongs to the companion
and therefore *survives* the respawn — that is precisely why it is held there rather than on the
controller. `OnRemoved` already clears it, and the fields default to empty, so a genuine equip
starts clean without an `OnApplied` at all. The reset was redundant on the case it was written for
and destructive on the one that actually happens.

The rest of the hook table is clean: the `SetXEquipped`
flags and the multipliers are all rebuilt from a reset baseline, `ShadeCharmSummons.Spawn`
self-dismisses before spawning, and both timers are per-inventory rather than shared - each
companion builds its own `_definitions` in its own constructor, so the closure locals are per
companion too. Joni's Blessing and the lifeblood charms are order-independent because
`ApplyCharmHealthModifiers` recomputes the whole capacity from `charmLifebloodBonus` each time, so
whichever hook runs last still lands on the right total. Definition identity is likewise stable across recomputes *within* one
companion, because the list is built once in the inventory's constructor and `ShadeCharmDefinition`
has no `Equals` override — which is what the `OnRemoved` diff relies on. It does not survive a
respawn, which is the trap described above.

### The save file was rewritten on every room transition

The same `SetActiveSlot` path flushes the whole inventory through `PersistInventoryToSlot` ->
`WriteBatch` -> `PersistSlot`, and `PersistSlot` had no dirty check — so every room change
serialised the slot and replaced the file on disk, almost always byte-identical.
`ShadeSaveSlotRepository` now keeps the JSON it last wrote per slot and skips a write that would
change nothing (the path is checked too, so a file deleted underneath us is written again). The
regression test drives it through `WriteBatch`, not a setter: the setters already decline to persist
an unchanged value, so a test written against one passes without touching the cache at all.

### A latent immunity in the damage path, reported rather than changed

`TryProcessDamageHero` gates on `bodyCol.IsTouching(col)`. `IsTouching` consults the layer collision
matrix, and the companion's body is deliberately on **Default**, not the hero layer — so a damager
on a layer that does not interact with Default reads as "not touching" however far inside the body
it is, and the hit is dropped silently. This is the trap `ShadeGrabRetargeting` documents avoiding.

Not changed. It is belt-and-braces over `ResolveDamager`/`CouldReachHornet`, which hold the real
line; Unity leaves Default interacting with most layers, so it probably works in practice; and
widening it is exactly the "removing a bug removes the behaviour it accidentally provided" case,
in the one path that cannot be judged outside a running game. Instead there is now a
`shade-contact-refused` event carrying both layer names and — measured with `Collider2D.Distance`,
which is pure geometry — whether the two actually overlapped. `overlapped=True` in a report is the
matrix refusing a hit that landed, and is the reading that would justify moving the guard.

### Swept clean

- Reflective lookups: 92, none untested.
- `IDE0051` under `-p:EnforceCodeStyleInBuild=true`: 76 hits, **all** Unity message handlers. No
  genuinely unused private member remains. `IDE0059`/`IDE0060` found the four below and nothing else.
- Per-frame allocation: four allocation-shaped lines inside `Update`, three in exception or toggle
  branches and the fourth (`KnightView`) behind a hide-deadline that fires once per effect.
- Multi-companion: every `ShadeRuntime.Charms` / `PrimaryInstance` read is either UI, or a
  documented "one global setting, the primary answers for all". `ApplyShadeMaskFractionToLiveShade`
  and `SetShadeAssistMode` both broadcast over `ActiveInstances` correctly.
- `ShadeCharmSummons.s_sets` does not leak: `OnDestroy` calls `DismissAll`, which also sweeps
  entries whose controller has already gone.
- `ShadeAiInput` is static, but each instance publishes immediately before reading in its own
  `Update`, so two AI companions cannot cross.
- All 27 `[HarmonyPatch(typeof(X), "name")]` attributes resolve unambiguously —
  `NoPatchNamesAMethodThatCannotBeResolved` checks each against the real assembly and catches
  `AmbiguousMatchException` specifically.

### Charm ids are the save format, and nothing said so

A slot records owned, discovered and equipped charms as **plain integers** — dumped from a real
write, `QuickSlash` is the number `5` on disk, in `Collected`, `DiscoveredCharmIds` and
`EquippedCharmLoadouts` alike. There is no `StringEnumConverter` in the repository's serializer
settings and the DTO fields are `ShadeCharmId[]`, so the ordinal *is* the wire format.

Inserting a charm anywhere but the end of `ShadeCharmId` therefore renumbers every charm below it
and silently hands existing saves the wrong ones — and nothing about that appears in the diff of a
reorder. The enum carried a comment saying "appended, never reordered", but it sat halfway down,
covering only the block below it, and nothing enforced it. With ten more Knight charms still owed,
this was going to be found the hard way.

The constraint is now the enum's own doc comment, and `ShadeCharmInventoryTests` pins the whole
name-to-ordinal map. Adding a charm costs one line there; moving one that has shipped fails the
build. Verified by reordering `QuickSlash` and `MarkOfPride` and watching it fail.

### A cache that would have kept a destroyed sprite

`ShadeCharacterManager.s_previewCache` returned its hit on a plain dictionary lookup. The Knight's
preview is built from `knight.bundle`, and `KnightAssets.Unload` tears that down with
`unloadAllLoadedObjects: true` — which destroys the texture behind the sprite while leaving this
dictionary, in another class, holding it. A plain hit then hands back a dead sprite for the rest of
the session and the row draws nothing, with no error.

Defensive rather than a live bug: `Unload` is only reachable from the failed-load branch, which
disables the Knight anyway, so the sequence needs a failure after a success. Fixed regardless,
because it is the exact trap the house rules name and it costs four lines. The read now asks with
`ReferenceEquals` **and** Unity's operator, which is what separates a cached *absence* — a real
null, which must stay cached or a character with no art re-reads and re-warns on every focus — from
a cached object that has since been destroyed.

`ShadeCharmIconLoader` had the mirror problem: it tested `cached != null`, which is Unity's operator
and so correctly rebuilds a destroyed sprite, but it therefore never negative-cached either. Every
`ShadeCharmInventory` builds its own definitions, and each definition asks for up to four icon
names, so a charm with no icon of its own re-ran the whole filesystem candidate sweep once per
companion. Now split the same way. `ShadeSkinManager.PreviewCache` and the `KnightAssets` caches
were already correct — the latter because `Unload` clears them itself.

### Entropy

- `TryCalculateEquippedIconBounds` returned four out-params; `size` was read by nobody and `center`
  by one of two callers. Now one `Rect`, which carries all four for free.
- `ShadeInventoryPane.BackgroundAlpha`, a `private const` nothing read.
- `Tools/SourceAudit.py` gained the two checks this round was run by hand: `perframe` (allocations
  inside a Unity per-frame message) and `subscriptions` (`+=` on an event with no `-=` anywhere).

### Also checked and found sound

- Input bindings persist by **field name**, not by `ShadeAction` ordinal, so that enum is free to
  move.
- `registeredEnterSceneHandler` is reset in `GameManager.Awake`, so a recreated `GameManager` does
  re-subscribe. `Camera.onPreCull += ApplyProjectionZoom` is static-to-static and self-guarding.
- Swapping character preserves everything it should: health, soul and charms live on
  `ShadeCompanion.State`/`.Charms` and the setter only raises `AppearanceChanged`.

## `Tools/SourceAudit.py`

The three scanners this and the previous pass kept rebuilding, now in the repo: reflection coverage
against `GameApiContract`, declarations nothing mentions, and repeated statement runs. Its docstring
also records the more exact route for the second of those — Roslyn's own IDE0051/IDE0052 through
`-p:EnforceCodeStyleInBuild=true`, which is off in the normal build because Unity message handlers
would fail the zero-warning rule.

## Left alone, deliberately

- **Hiveblood still cannot regenerate *lifeblood* across a room change.** The mask half is fixed
  above, because `_hivebloodPendingMaskRestore` lives on `ShadeCharmInventory` and survives the
  respawn. Its lifeblood twin, `hivebloodPendingLifebloodRestore`, is a field on the **controller**,
  which every scene change destroys — so with Joni's Blessing worn, the "a lifeblood mask is owed"
  obligation is dropped at every door exactly as the mask one used to be.

  Not fixed here on purpose. The flag is woven through ten sites in the controller's health
  arithmetic — `ApplyCharmHealthModifiers` maintains it, `Combat` raises it, `Persistence` clears it
  on the spawn path that would have to restore it — and that arithmetic has already been changed
  twice in this pass, in code that cannot be exercised outside a running game. The narrower blast
  radius (it needs Joni's Blessing *and* Hiveblood together) does not justify a third structural
  change on top. The fix, when it is made, is to move the flag to the inventory beside the mask one
  and give the controller a getter for Joni's state, which also removes the asymmetry that hid this.

  Everything else named `pending*` on the controller is per-frame plumbing — HUD sync flags, the
  deferred health sync, the spawn animation — and is meant to die with the body.

- Charm effect magnitudes. Deep Focus is `1.4x` focus time where Hollow Knight is about `1.65x`, and
  Stalwart Shell `1.35x` i-frames against `1.3x`. These are balance, and balance here starts from
  the first game rather than being bound to it.
- `KnightMovement.cs` is down to 1165 lines after the last review's Terrain/Pogo split, so it is no
  longer the largest file - `ShadeSettingsMenu.Drivers.cs`, `ShadeInventoryPane.CharmGrid.cs` and
  `ShadeInventoryPane.Animation.cs` are all bigger. Any of those splits needs a matching [Code Map]
  row and the wiki cannot be published from here.
- `ClampKnightToCameraView` and `EnforceKnightLeash` write `transform.position` in the same
  `FixedUpdate` as `rb.MovePosition`. Mixing the two on a dynamic body is a documented Unity hazard
  and could be why the clamp sometimes lags a frame — but it cannot be judged without the running
  game.
- `obj/` in the repo holds 38 MB of pre-redirect build leftovers, including a stale
  `PluginInfo.cs`. Gitignored and inert behind `DefaultItemExcludes`, but it is the CS0579 case
  `Directory.Build.props` describes, so it is worth clearing by hand.
