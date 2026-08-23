# Changelog

One line per change, two at most. Say what changed for the player and name the setting that
controls it; the reasoning belongs in the commit and the code.

## 1.2.0

### Fixed

- Boss attacks that mark an area now hit whoever is standing in it — the Shade, Hornet, or both.
  Previously an attack the Shade walked into would drag Hornet across the arena into it.
  `shadeBossAttackSharingEnabled` turns this off.
- The Shade is now hurt only by things that would hurt Hornet: attack telegraphs and marker volumes
  no longer damage it, and neither do colliders it could never really be touched by.
- The Shade's charm menu no longer handles every input twice, so navigation moves one step per press
  and A equips a charm on a controller instead of playing the animation and changing nothing.
- The Shade comes back from death at full health rather than one mask, wherever you respawn.
- Particle hazards such as acid sprays can hit the Shade.
- The Shade and its HUD are hidden for cutscenes and the memory sequences, and stay under your
  control for the playable stretches inside them.
- The Shade no longer blinks out of existence inside a memory whenever Hornet mantles a ledge, dashes
  or rides an updraft.
- Fixed a bug where Hornet would sometimes be healed twice while binding.
- The Shade no longer draws on top of fog, snow and darkness; it sits on a character sorting layer
  and uses Hornet's sprite material. `shadeUseHornetMaterial` and `shadeSortingOrderOffset` restore
  the old always-on-top look.
- Skin previews are now anti-aliased to look better when view up close.
- You can now talk to Flick the Fixer after he fixes the Wishing Wall... somehow... I had this bug too and it just... went away at some point

### Added

- Shadow particles around the Shade, in the style of the first game, growing denser as its soul
  fills.
- Press **F8** to file a bug report from inside the game. It freezes, asks what happened, and saves a
  screenshot of the frame before the panel appeared along with scene state, all mod logs, about
  thirty seconds of rolling samples and a log of what happened, into
  `BepInEx/config/LegacyoftheAbyss/bug_reports/`. Nothing is uploaded anywhere. Mod exceptions file
  one automatically.

### Note

- These settings live in `BepInEx/config/LegacyoftheAbyss/config.json`.
