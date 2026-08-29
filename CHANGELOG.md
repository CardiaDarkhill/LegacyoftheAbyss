# Changelog

One line per change, two at most. Say what changed for the player and name the setting that
controls it; the reasoning belongs in the commit and the code.

## Unreleased

### Added

- The Shade's "Skins" menu is now "Characters", and the Knight of Hallownest joins the Shade as a
  playable companion. It walks instead of floating, and pogos off Hornet with a down slash.
- The Knight's abilities follow Hornet's: Mothwing Cloak with her sprint, Mantis Claw with her wall
  climb, Double Jump alongside it, and Shade Cloak with Harpoon Dash. Jump is a new binding in
  Controls, unbound to anything the Shade uses.
- Ten charms join the roster: Weaversong, Defender's Crest, Flukenest, Spore Shroom, Thorns of
  Agony, Glowing Womb, Gathering Swarm, Grimmchild, Dream Wielder and Dreamshield. All work for
  both the Shade and the Knight, sold by the Bonebottom shopkeeper and Grindle.
- The charm inventory now scales its grid to fit the larger roster instead of running off the pane.
- The camera now leans toward the midpoint between Hornet and her companion, and widens by up to a
  quarter once they no longer both fit, so the pair stay on screen. The Knight is held inside the
  view as a backstop. "Co-op Camera" in the Shade settings turns both off for anyone who prefers
  the camera to stay on Hornet; `companionCameraMaxZoom` sets how far it may widen.
- The Knight carries a brighter light than the Shade. `knightLightRadiusMultiplier` and
  `knightLightIntensityMultiplier` control it.
- The Knight controls like the Knight: attacks are aimed with the movement stick, the freed
  down-slash button is Jump, and the Controls screen shows the scheme for whichever character is
  equipped. It takes out its own map on the quick map, and has its Monarch Wings and Shade Cloak
  animations from Hollow Knight.
- Shade Cloak now has a cooldown, and the shadow wisps are its readout: they gather while it
  recharges and are drawn back into the body the moment it is ready. The Shade adopts the same tell
  once it reaches Shade Cloak.
- The Knight uses Hollow Knight's own sounds where the asset bundle carries them: its dash, and its
  Shade Cloak. They play from the Knight rather than from Hornet, so distance is audible.
- Shade AI is unavailable while the Knight is equipped and reads as such in the menu — it steers by
  synthesising input, which cannot drive a character that has to jump and climb. Your setting is
  kept and returns with the Shade.

## 1.2.0

### Fixed

- Cycling the Shade's mask setting in the Difficulty menu no longer leaves it on 1 health. The
  Shade now unpauses with the health it had when you paused, or its new maximum, whichever is
  lower.
- The Shade's maximum health now follows Hornet's while you play, instead of being fixed when it
  spawned.
- The controller preset no longer overwrites your saved keyboard inventory keys with 1-5. The
  Shade still gets those keys; they are bound to it directly rather than through the game's
  own keyboard settings.
- The Shade no longer attacks enemies from a gauntlet wave that has not started yet. Those
  enemies sit in the scene invisible and inert, and the Shade would stand slashing at nothing.
- The Shade's charm menu now slides in and out with the same animation as every other inventory
  tab, whether you reach it by cycling or by its hotkey, instead of snapping into place.
- The Shade now lights dark rooms, so it stays visible when it wanders away from Hornet. Its light
  fades in with distance from her and is absent at her side, where hers already covers it.
  `shadeLightEnabled`, `shadeLightIntensity`, `shadeLightRadiusScale` and `shadeLightFalloffRadius`
  control it.
- The charm menu no longer runs its text out under the frame, no longer shows the notch cost of a
  charm you have not discovered, and draws undiscovered charms at a sensible size.
- Charm menu headings are no longer shouted in full capitals.
- The charm menu names every way to equip the charm you are looking at - the Shade's key and
  Hornet's button - rather than only the Shade's first binding, and calls the mouse buttons
  LMB/RMB/MMB.
- The Shade takes the short way round a ledge instead of crossing the room to get past it.
- Boss attacks that mark an area now hit whoever is standing in it — the Shade, Hornet, or both.
  Previously an attack the Shade walked into would drag Hornet across the arena into it.
  `shadeBossAttackSharingEnabled` turns this off.
- The Shade is now hurt only by things that would hurt Hornet: attack telegraphs and marker volumes
  no longer damage it, and neither do colliders it could never really be touched by.
- The Shade's charm menu no longer handles every input twice, so navigation moves one step per press
  and A equips a charm on a controller instead of playing the animation and changing nothing.
- The Shade comes back from death at full health rather than one mask, wherever you respawn.
- Particle hazards such as acid sprays can hit the Shade.
- The Shade and its HUD are hidden for cutscenes and the memory sequences.
- The Shade no longer blinks out of existence inside a memory whenever Hornet mantles a ledge, dashes
  or rides an updraft.
- Fixed a bug where Hornet would sometimes be healed twice while binding.
- The Shade no longer draws on top of fog, snow and darkness; it sits on a character sorting layer
  and uses Hornet's sprite material. `shadeUseHornetMaterial` and `shadeSortingOrderOffset` restore
  the old always-on-top look.
- Skin previews are now anti-aliased to look better when view up close.
- You can now talk to Flick the Fixer after he fixes the Wishing Wall... somehow... I had this bug too and it just... went away at some point

### Changed

- **Wayward Compass** now marks the rooms holding Legacy of the Abyss charms and notches you have
  not collected yet, on the quick map and the full map, instead of repeating the position Hornet's
  own compass already shows. Pins only appear in areas whose map you own.

### Added

- The Shade can fight on its own: turn on **Shade AI** on its own options screen. It picks a
  target, closes on it, attacks, casts only where a spell would hit a boss or several enemies at
  once, steps out of what would hurt it, and heals you both when either of you is low. It can be
  killed, so you will still need to revive it.
- With nothing to fight it keeps station just behind and above Hornet, and drops ahead of and below
  her the moment she leaves the ground - close enough to pogo off over a gap.
- Tell it where to stand with **middle mouse** or **left stick click** ("Command Shade" in Controls):
  tap twice for "hold here", or aim the reticle - mouse or right stick - to send it anywhere on
  screen. A Shade told to wait holds its ground until Hornet lands, instead of being recalled
  halfway across a platforming section.
- While the AI drives, Hornet answers to the keyboard and the controller at once whichever control
  preset is set, and the Shade's own controls do nothing - there is no second player to share them
  with. `shadeAiVanillaControls`.
- Yes/no settings now read "Setting: On" like the Shade Enabled row, instead of a checkbox square.
- Shadow particles around the Shade, in the style of the first game, growing denser as its soul
  fills.
- Press **F8** to file a bug report from inside the game. It freezes, asks what happened, and saves a
  screenshot of the frame before the panel appeared along with scene state, all mod logs, about
  thirty seconds of rolling samples and a log of what happened, into
  `BepInEx/config/LegacyoftheAbyss/bug_reports/`. Nothing is uploaded anywhere. Mod exceptions file
  one automatically.
- Difficulty presets: **Easy** plays vanilla Silksong with the Shade's help, **Normal** weakens both
  of you to keep the vanilla difficulty curve, and **Abyss** asks for sharper combat than vanilla.
- **Assist Mode** now has a switch on the Difficulty screen, and the Shade's damage is split into
  **Shade Nail** and **Shade Spells** with Hornet's split into **Hornet Needle** and
  **Hornet Silk Skills**.
- **Shade Masks** sets how many masks the Shade carries as a share of Hornet's, in tenths, and
  **Full Masks Focus** lets it Focus while undamaged so it can still heal her.

### Changed

- The Difficulty screen is laid out as Damage and Healing side by side, uses the game's own sliders
  instead of plain grey ones, and explains whichever setting is highlighted.
- Assist Mode and Shade AI no longer have key bindings; both are switches in the menus. **Shade
  Enabled** has moved to Debug Options.
- The mod's menu screens now fill the display rather than the middle two thirds of it, sitting in a
  centred column with a clear band above them. The Controls screen no longer needs a scrollbar.
- Pressing a toggle or a preset no longer throws the highlight back to the top of the screen, and
  backing out of a sub-menu returns to the row that opened it.
- On the Difficulty screen, the shoulder buttons move between the Damage and Healing columns, which
  the sliders' own left/right could not. The prompt shows only while the button would actually move
  you.
- The Controls screen explains the highlighted preset at the bottom of the screen like every other
  screen, instead of printing all four descriptions at once.
- Hovering a row with the mouse now selects it, so the selection markers and the explanation line
  follow the cursor instead of staying on whichever row the screen opened with.
- Typing a bug report no longer reaches the game: the keyboard is locked out for the whole message,
  so a report written while paused stops changing settings, and one written in play stops queueing
  moves for Hornet to act out on submit.

### Note

- These settings live in `BepInEx/config/LegacyoftheAbyss/config.json`.
