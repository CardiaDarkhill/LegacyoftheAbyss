# Branch review — 3 September 2026

A pass over `Knight-in-silksong` looking for defects introduced by this branch's own bug fixes and
features, and for duplication and dead code. Build stayed at zero warnings and the suite green after
every batch.

Net effect across both rounds: **56 files, 1230 insertions against 1668 deletions**, tests 535 to 553.

## Defects found and fixed

| What | Where | Why it mattered |
| --- | --- | --- |
| The HUD frame was never rotated on the first build | `HUD.Assets.cs` | `IsSpriteRotated` was asked *before* `TryBuildSprite` populated the packing set, so it answered "not turned" for a frame the atlas stores turned. Order-dependent: a later HUD rebuild would answer correctly, so the plate could come up wrong and then fix itself. `IsSpriteRotated` now resolves the frame first, so the trap cannot be repeated. |
| A pogo beside a wall gave no height | `KnightMovement.cs` | The wall cling lost its `velocity <= 0` guard when the catch was made immediate. The probe's catch-frame zeroing already makes it immediate, so the guard could come back — without it any upward impulse next to a wall became a wall slide on the following frame. |
| The balloon launch outlived every hold | `KnightMovement.cs` | Its branch returns before the control-lock gate, so a hazard respawn, cutscene, bench or room-entry placement could not stop a rise in progress. It now stands down for each, and is held on screen while it runs. |
| The Knight walked off the playable plane | `KnightMovement.cs` | `ApplyKnightMotion` assigned a `Vector2` into `transform.position`, which fills z with zero. A candidate cause of the Knight-in-front-of-grass reports. |
| Assign Controllers ignored rebound controls | `ShadeInputConfig.cs` | A control rebound on a pad stores that pad in the binding, and the stored device outranks the config index — so the assignment did nothing for anyone who had rebound anything. `ApplyControllerAssignment` now moves both, with Command Shade going the other way onto Hornet's pad. Three tests. |
| Charm clouds were scaled twice | `KnightEffects.cs` | `ScaleCloudToRadius` multiplied the instance's scale, compounding with any `effectScale` the caller had passed. Defender's Crest asked for 0.65 and came out at 0.22. It now sets the scale from the prefab. |
| The keyboard watchdog could hammer | `HornetInput.cs` | With the throttle removed it would call the game's mapper every frame for the session if a remap produced nothing. Now: immediate first attempt, one-second retry only after a failure. |
| An audio source that had been destroyed was kept | `ShadeController.Audio.cs` | `??=` does not see Unity's destroyed-object null, so a source whose object had gone would be kept and every later `PlayOneShot` silently skipped. |
| Dead branches and dead parameters | several | `SetKnightIntangible(false)` never did anything; `ApplyParticleTuning`'s `scale` was never read; `EnforceKnightLeash` refunded the air jump but not the air dash. |
| An array allocated per enemy per AI scan | `ShadeAiTargetScanner.cs` | `IsDrawingAnything` used `GetComponentsInChildren<Renderer>()` for a diagnostic counter. Now a shared list. |

## Entropy removed

**Dead code, about 490 lines.** `TemplateSyncHost` and `SyncRequest` — a `MonoBehaviour` nothing ever
constructs — plus the three helpers only it used; `MenuTransferSaveScope` and its factory, a
disposable that disposes nothing; `GrantAllCharms`; `TryGetPersistentState`; `IsCharmDiscovered`;
`ReloadDefinitions`; `GetSourcePath`; `FromController`; `ResetPickup`; `ClearShadeSoulOverride`;
`CutsceneVideo.Invalidate`; `WithControllerIndex`; `ApplySharedKeyboardPreset`; `CopyBindingsFrom`;
and four charm stat hooks nothing hooks.

**Duplication collapsed.** The largest was four hand-written binding roll-calls in `ShadeInputConfig`
— one per question about which pad belongs to whom — which had already drifted from each other; they
are now one sweep over `AllActions`. Also: two controller presets into one layout; the charm health
snapshot taken by hand in three places; the FSM action enumeration and `SafeGetTypes` duplicated
across both retargeting features; two HUD stage-sprite resolvers; three HUD slot-art builders; the
description footer built twice; the first-selectable pair written out five times; the summon terrain
sweep duplicated between two minions; the row-label writer implemented three times; equip and
unequip, which differed by one word; the shop's placement-match loop; the CSV escaping in both
report writers; and the `TextStyle`/`ShadowStyle` capture shared by the settings menu and the
inventory pane, now `Shade/UiTextStyle.cs`.

**Stale documentation.** `TerrainMask` carried a summary describing a mask resolution that no longer
exists; the keyboard watchdog claimed a cadence it no longer has; an orphaned comment sat in
`TryKnightPogo` after the overlap it described had moved; the hazard lock duration was declared as
two constants in two files.

## Second round

**Desolate Dive's shockwave was invisible, not absent.** Both quake spells spawn the same two damage
volumes; what differs is the effect drawn over them, and it was being sized by the sprite *cell*
rather than by the art inside it. Desolate Dive's sheet fills 153 pixels of a 520-wide cell where
Descending Dark's fills 334 of 510, so one desired width drew Dive's burst about four units across
against Dark's ten — small, dark, on a dark background, for a tenth of a second against Dark's three
tenths. Both are now sized from their art to span the strip they damage, and held for the same time
whatever their frame count.

**Spells now deal Hollow Knight's own damage**, taken from the wiki and stated flat in
`Shade/ShadeSpellDamage.cs`: Vengeful Spirit 15, Shade Soul 30, Howling Wraiths 13 × 3, Abyss Shriek
20 × 4, Desolate Dive 15 + 20, Descending Dark 15 + 48. Charm and difficulty multipliers still
apply; the nail does not enter into it.

The first attempt stated them as multiples of the nail, reasoning that a fixed figure falls behind
Hornet's upgrades. That was wrong about whose spells these are. Hers scale with her needle because
her silk skills have no upgrades of their own; the Knight's spells upgrade in their own right, and it
has spell charms — Shaman Stone, Spell Twister, the SOUL-gain charms — with no counterpart on her
side. Scaling compounded all three, and at a late-game needle put Abyss Shriek high enough to kill
the final boss in three casts.

Two other things fell out of doing it properly: the shriek was landing **one** hit where Hollow
Knight lands three and four, and the damage log was printing one hit as though it were the spell —
which is what made Howling Wraiths look like it was dealing double. The log now reads
`13 x 3 hits = 39 max`. Ten tests check each spell's pieces against the wiki's totals, including one
that exists purely to keep the nail out of it.

**`KnightMovement.cs` split**, 1784 lines to 1165, into:

- `LegacyHelper.ShadeController.KnightTerrain.cs` — what the body is standing on and holding, the
  swept casts, the step-over and the ground settle.
- `LegacyHelper.ShadeController.KnightPogo.cs` — the down slash's bounce, what may be bounced off,
  and the launch a balloon gives instead.

### Code Map entries to add to the wiki

| File | Responsibility |
| --- | --- |
| `LegacyHelper.ShadeController.KnightTerrain.cs` | Where the Knight's body meets the world: ground and wall probes, the terrain mask, the swept collision, the step-over and the ground settle. |
| `LegacyHelper.ShadeController.KnightPogo.cs` | The down slash's bounce: the probe, the surface classification read off `HeroDownAttack`, and the balloon launch. |
| `Shade/ShadeSpellDamage.cs` | Hollow Knight's own spell damage, flat, with the hit counts, and why it is not scaled off the nail. |
| `Shade/UiTextStyle.cs` | The captured typography the settings menu and the inventory pane both restore onto cloned rows. |
| `Diagnostics/CsvText.cs` | CSV field escaping shared by the event ring and the flight recorder. |

## Left alone, deliberately

- **The reservation sweep still excludes the debug actions**, exactly as each of the four lists it
  replaced did. Including them would change which pad gets reserved, which is a behaviour change
  rather than a refactor.
- **The `ApplyTextStyle` implementations were not merged**, only the record they consume. The two
  differ in substance — the pane takes its fallbacks as arguments where the menu holds them as
  statics and applies a font scale — so sharing them would mean picking one behaviour for both.
- **Descending Dark's asymmetric bursts are not reproduced.** Hollow Knight deals 35 on one side of
  the first burst and 30 on the other; here both bursts are taken together as one figure, which
  lands the spell at 63 inside its 60-to-65 band without a left-and-right volume to maintain.
