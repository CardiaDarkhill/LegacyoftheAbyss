# Changelog

## 1.1.0

So, vanished for about a year there... Sorry about that.
Things kept coming up and by the time I had spare time, my passion of Silksong had moved on.
I'm back at it now and very excited to have people try Legacy of the Abyss in a significantly less buggy state!

---

### ⚠️ Read this before you update

**Back up your Shade save files first.**

Shade progression and settings used to be written next to the mod's DLL, inside the mod's own
folder. That was a mistake: Thunderstore Mod Manager, r2modman and Gale all update a mod by
deleting its package folder and re-extracting the new version, which quietly took your Shade
progression with it every single time. This caused a total wipe of all shade progression every time
I updated the mod. This is fixed now, you'll lose your save data on this update unless you back it up
(instructions below), but this issue should never happen again.

From 1.1.0 onward, all LotA save data lives in `BepInEx/config/LegacyoftheAbyss/` instead — a folder
owned by BepInEx rather than by the mod, so it survives updates, reinstalls, and can be copied
to another machine by hand.

The mod will automatically pick up an old save if it can still find one, **but it can only do
that if the files still exist when it first runs.** If your mod manager wipes the old folder
during the update, there's nothing left to migrate. So before you update:

1. Find your old `Assets` folder inside the mod's install directory.
2. Copy `config.json` and any `shade_slot_*.json` files somewhere safe.
3. Update the mod.
4. If your settings/progression didn't carry over, drop those files into
   `BepInEx/config/LegacyoftheAbyss/` and restart the game.

Sorry about this one.

---

### New: Shade skins

There's a Skins screen in the pause menu now. The default Shade sprites have been redrawn,
and there are seven alternates to switch between on the fly:

- Low Horn 
- High Horn
- 3 Horn
- 4 Horn
- Grimmchild Phase 2
- Grimmchild Phase 3 
- Cozy Shade (Huge thanks to Shopcreeper01 for this one!)

All of these skins (And the vanilla skin edit) with the exception of the last were provided to me
by TheWr13r, who I can't thank enough for their help!


### New: enemies actually notice the Shade

Previously enemies would largely ignore the Shade and keep walking straight past it. Whilst
some enemies would notice, the only reason this ever worked was actually due to a bug I had 
to fix for other reasons. Two separate things were wrong, and both are fixed:

- **Enemies can now see the Shade at all.** The Shade wasn't being counted by the alert ranges
  enemies use to decide whether anything is nearby, so as far as they were concerned the Shade
  didn't exist.
- **Enemies can now choose to chase the Shade.** When the Shade is meaningfully the better
  target, enemy AI will redirect onto it. I havn't played around with this much so the values
  might need a tweak, but in theory they should be *too* dumb about it.

Enemies should always ignore the Shade while assist mode is on (All the shades hitboxes
are turned off in Assist Mode, so they have no way to see the shade anyway.)

Some enemy types still ignore the Shade. hand-written (non-FSM) enemies, and attacks that look up Hornet
at the moment they fire rather than reading a target. There's also no "it hit me so I'm staying
on it" stickiness yet; it's purely distance-based. Please give me bug reports based on enemies that still
ignore the shade, I'll need to fix them case-by-case.

### New: The Shade's movement is now locked during cutscenes and dialogue.

As funny as this was, it did detract a bit from the experience. When talking or in a cutscene, the shade
now moves to Hornets side and faces in the same direction as her. I havn't done a full playthrough
with this feature yet, so I'm 100% sure there's going to be scenes where it doesn't work properly or
causes problems.

### Fixed: the Shade no longer sets off Hornet-only world triggers

The Shade carries an invisible collider that mimics Hornet's layer and tag (that's how enemies
notice it). Unfortunately that also made it indistinguishable from Hornet to every trigger
volume in the game, so the Shade drifting through one could fire it as though Hornet had walked
in. Updrafts and wind regions, frost, darkness and atmosphere regions, bench work ranges and
pickup triggers now all correctly ignore the Shade, while enemy alert ranges — the one type
that *should* see it — still do. Again, testing here has been limited, please report any bugs
that crop up (You know... Not the Pharloom ones, the code ones)

### Fixed: crash on slide surfaces

Slide surfaces assumed only Hornet could ever enter them. The Shade entering one overwrote the
surface's cached reference to Hornet with null while leaving its "the hero is here" state
switched on, and the next frame died with a NullReferenceException. The Shade also cleared that
state on the way out, while Hornet was still standing on the slide. The Shade is now ignored by
slide surfaces entirely — it floats, it was never going to slide anyway.

### Fixed: memory leak on every scene change

The Shade is respawned on each scene transition, and its decoded sprite sheets were being
dropped without being freed — a full sheet set leaked per transition, which added up badly over
a long session. Cleanup now runs on the plugin itself (which survives scene loads) instead of on
the Shade (which doesn't), with a short grace period so in-flight spell VFX still holding those
sprites don't pop.

For context here, after you'd moved through 100 rooms (Or turned the shade on/off 100 times), 
the mod was using 3 gigabytes of ram on... Nothing at all. To say I messed up here understates 
things a litte.

### Fixed: menus, binds and the inventory

A pile of input and UI fixes:

- Shade keybinds no longer go missing after re-applying a preset from the settings menu.
- Controller pause / inventory binds fixed, including for pads that don't expose the buttons
  the game expected.
- Two-controller setups behave properly. (Hopefully, I only own 1 controller lol)
- Number-key binds (1–5) work again. (WHYYY are these so hard to modify Team Cherry?!?!?)
- Sub-menu highlighting no longer gets stuck on the wrong entry.
- Assorted fixes to the Shade's charm tab and its inventory buttons.

### Performance

A fairly aggressive performance pass was done. This being needed was honestly the main thing
killing my motivation to work on the project again. This refactor/performance pass took me over
a week and at the end of it all I noticed exactly 0 difference to the final product (My PC is a monster)
But it had to be done.

The mod was doing reflection lookups by name inside per-frame loops. fifteen field handles re-resolved 
on *every single slash*, a hazard-type lookup per collider per frame, an audio-clip search that re-ran 
up to nine substring matches per clip on every sort comparison, and hundreds of redundant physics interop 
calls per second in busy arenas. All of that is resolved once and cached now.

### Settings menu additions

- **Skins** screen (above).
- **Debug Options** screen, off by default: optional debug keys for HP/Soul, and a damage summary
  file for bug reports.
- Toggle for the new enemy-targeting behaviour.

### Under the hood

Not player-facing, but for the curious: the oversized source files were split into partial
classes, the test suite grew to 136 passing tests (including tests that run against the real game
assembly, so a Silksong update that renames something fails loudly instead of silently doing
nothing), and releases now go out to Thunderstore and Nexus through a single automated pipeline.

---

### Still known-broken

- The charms menu snaps into view instead of playing the game's slide-in animation when you
  navigate to it. Purely cosmetic, but it looks janky.
- Skin preview pixelation (see above).
- Enemy aggro coverage is partial (see above).

As always: this mod is large, the game is larger, and I can't test all of it alone. Bug reports
in the discussion section are hugely appreciated, the more specific, the better.
