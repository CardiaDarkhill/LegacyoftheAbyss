# Changelog

One line per change, two at most. Say what changed for the player and name the setting that
controls it; the reasoning belongs in the commit and the code.

## Unreleased

### Added

- A Swap Character button puts the companion into the other body where it is standing - R, or the
  right stick click. Rebind it under Controls; coming back to the Shade restores its last skin.
- Every control is listed under Controls whichever character is equipped, so the Knight's and the
  Shade's can both be set before the swap button needs them.
- Assign Devices is offered on the new-game screen as well as under Controls.

### Fixed

- The companion draws on Hornet's own sorting layer at her own order and depth, so the world sorts
  the two of them alike. It was appearing over scenery she is behind (`shadeSortingLayer`, now blank
  for "match Hornet").
- Escape steps back one screen in the mod's menus instead of leaving the pause menu outright, and
  cancels a binding prompt without closing the screen behind it.
- Assign Devices accepts a keyboard press, so either player can be put on the keyboard. It replaces
  the four control presets, which are gone.
- The Knight's spell effects keep their own art instead of taking whichever Shade skin was picked
  last.
- The HUD plate sits where its socket says it should. It was being placed against the plate's stored
  size while it is drawn turned, and `hudFrameOffsetX` existed to cancel the difference - it now
  defaults to 0.
- Hornet no longer stands still for a second after a room transition. Her keyboard bindings are put
  back on the frame they go missing rather than a second later.
- Vengeful Spirit rides over uneven ground instead of bursting on it. It holds its line and shifts
  its height to clear what it is scraping, as Hollow Knight's fireball does, up to about a
  character's height from the line it was thrown along - so a wall still stops it.
- Desolate Dive's shockwave is visible. Its burst was being drawn about four units across against
  Descending Dark's ten, for a tenth of a second - the damage was always there, the effect was not.
- Shaman Stone matches Hollow Knight per spell rather than adding a flat 30% to everything: a third
  on the projectiles, half on the screams, 51% and 47% on the two quakes. Desolate Dive with the
  charm is 53, as the wiki has it.
- Spells deal Hollow Knight's own damage, flat: Vengeful Spirit 15, Shade Soul 30, Howling Wraiths
  13 x 3, Abyss Shriek 20 x 4, Desolate Dive 15 + 20, Descending Dark 15 + 48. They no longer scale
  with Hornet's needle - the Knight's spells upgrade in their own right and its spell charms are far
  stronger than her equivalents, so scaling on top of both was worth several times the spell.
- Howling Wraiths and Abyss Shriek land all their hits. They were one hit where Hollow Knight has
  three and four.
- The damage log states a spell's maximum rather than one hit of it, which is what made Howling
  Wraiths look like it was dealing twice what it said.
- A pogo taken beside a wall gives its height back. The wall cling was catching the bounce and
  turning it into a wall slide on the next frame.
- Balloon launches stand down for a hazard respawn, a cutscene, a bench or a room change, and stay
  on screen rather than leaving the frame and being snapped back.
- Assign Controllers moves controls that were rebound on a pad. A rebound control remembers its own
  pad, and that outranks the assignment - so it appeared not to work for anyone who had rebound
  anything.
- Defender's Crest's cloud is drawn at the size it damages. It was being shrunk twice and came out
  at a fifth of it.
- The Knight's pogo reaches as far as its nail does. The two were sized separately and the nail was
  larger, so a swing could register a hit and give no height.
- Balloons and hanging pods can be pogoed at all. Both carry the marker that says "do not bounce off
  this", which in their case means "this object does the bouncing itself" - and it only does it for
  Hornet.
- The Knight's pogo reaches further down for Hornet specifically. Her collider is nothing like her
  silhouette, so hits that plainly looked like hits were missing her.
- The Knight is put back beside Hornet a quarter second after a room load rather than three quarters.
- Spore Shroom is Hollow Knight's own size - 8.18 units, read from the prefab - and the cloud is
  drawn at exactly what it damages. It was a 3.4 unit circle under an effect covering most of the
  room, so an enemy plainly inside the cloud took nothing.
- The companion is hidden while a pre-rendered cutscene plays.
- Every Shade spell was landing twice. The damage log said 14 for Howling Wraiths and the enemy took
  28; the numbers in the log are now the numbers dealt.
- Shade sounds no longer fail the first time they are used in a room. The fireball, both screams,
  both quakes and the focus sounds were all silent on their first use and correct after it.
- Hornet no longer loses her keyboard on a room change. She could be left with no bindings at all,
  and nothing put them back before a restart; they are now restored within a second.
- The Knight can pogo off the environment - bouncers, tinkable fixtures, levers, breakables - rather
  than only off enemies and Hornet.
- The Knight can no longer pogo off background scenery, which is the same prop as the foreground one
  pushed back behind the playable plane.
- The Shade and the Knight no longer take damage from switched-off hitboxes: the phantom hitbox in
  the arena and the inactive enemy were both colliders parked on the layer the game retires things to.
- The Knight holds still and cannot be hurt for a second after a hazard puts it back, instead of
  walking straight back in on the input still being held.
- Holding a wall gives the Knight back its air dash and its double jump, as the ground does. The
  wall only ever refunded the double jump when you jumped off it.
- The Knight catches a wall the instant it touches one, with its upward momentum cancelled, instead
  of drifting up it for several frames before the wall jump would answer.
- The Knight can no longer jump or dash while focusing. Shape of Unn still buys back walking.
- The Knight is put back beside Hornet 0.75s after a room load, so rooms that used to drop it out of
  the world no longer do.
- An up slash cancels the Knight momentum and holds it still for five frames, but only when the
  swing connects with something - it was doing it on every swing, including into open air.
- The Hiveblood masks are Hollow Knight own Hiveblood art rather than the plain mask painted orange,
  which had stopped working and was drawing five orange rectangles.
- Soul Vessels no longer refill the meter while the Shade is spending from it. Any spend restarts the
  wait, set by Soul vessel drain delay.
- The Knight sits down beside Hornet when it starts a rest on a different level to her, instead of
  ending up in the air beside the bench or under the floor.
- A hit from almost directly above or below now knocks the Knight back, rather than being shrugged
  off and letting the same enemy hit again immediately.
- The Knight dash has its recharge back (0.4s), and Dashmaster shortens it — it had none, which left
  the charm with nothing to do.
- Hornet is no longer left without input when the Shade player holds a button across a room
  transition or while the quick map is open. Their pad was becoming the active device during those
  moments — when input blocking is deliberately off — and staying there for as long as it was held.
- Rebinding a Shade control on a controller now records which controller it was pressed on, so it
  answers to that pad alone. It was being stored as a device-agnostic pad button, which fired on
  every attached controller — a rebound Shade control could be triggered from Hornet's pad. Any
  binding already saved that way is cleared on load, with a warning, so it can be rebound.
- A Shade control whose controller is unplugged now does nothing, instead of falling back to
  whichever pad was last used.
- Two controllers work again: the Shade player's pad is reserved from Hornet as it should be. The
  button that orders the Shade about lives on Hornet's own pad, and counting it as the Shade's made
  the mod think the Shade had claimed both - so neither was reserved and one stick drove both
  characters.
- The Characters screen no longer shows two characters as equipped at once.
- The Hornet Needle damage slider now does something. It was scaling a field that needle strikes
  never read, so only Silk Skills responded.
- "Legacy of the Abyss" now sits directly above Quit in the pause menu instead of below it, in both
  the drawn order and the order the stick and keyboard walk.
- Glowing Womb takes the SOUL it is supposed to for each hatchling again, and no charm summons
  anything while the companion is down waiting to be revived.
- The Shade AI stops spending SOUL on things not worth it: enemies below
  `shadeAiSpellMinTargetHealth` (new, default 6), and anything no spell could land on from any side,
  no longer count toward a cast. Armoured enemies still do — their armour faces one way and a cast
  can come in from another. Everything skipped is still attacked normally.

### Changed

- Difficulty settings now belong to the save profile rather than to the install, so two files can be
  played at two difficulties. A profile with none stored keeps whatever `config.json` holds, so an
  existing save carries on at the difficulty it was being played at.

### Added

- Assign Controllers, at the top of the Controls screen: each player presses a button on their own
  pad and the mod records which is which.
- Balloons launch the Knight the way they launch Hornet - 18 units a second for half a second, with
  its air moves back at the top - instead of doing nothing for it.
- The debug keys have defaults: = and - for soul, [ and ] to hurt and heal the companion, \ to empty
  its soul. A key already bound to something else is left alone.
- Bug reports record both characters' sorting layers, draw orders and depths.
- Bug reports record who owns which controller: every pad and whether it is attached, which one the
  game has as active, and what Hornet own action set is holding.
- Soul Vessels. The companion earns one for every two increases to Hornet's silk maximum, up to
  three, each holding 33 SOUL beyond the meter and refilling it a second after it has room.
  `shadeSoulVesselsEnabled` turns them off; `shadeSoulVesselDrainDelay` and
  `shadeSoulVesselDrainRate` tune the refill.
- The vessels show as a column of orbs beside the soul meter. `hudVesselsEnabled`,
  `hudVesselOffsetX`, `hudVesselOffsetY`, `hudVesselSize`, `hudVesselScale` and `hudVesselSpacing`
  place them, and Ctrl+F5 rereads them.
- Starting a new game now asks first: whether to reset that save slot's shade progress, which
  difficulty to play at, and whether to play as the Shade or the Knight. `shadeNewGameOptionsEnabled`
  turns the screen off.
- A fourth difficulty, Abyss, which is deliberately unfair - though the companion keeps its healing
  and stays revivable, so a death does not end the second player's evening. What used to be Abyss is
  now Hard, so a run already being played at those values keeps them under the new name.
- The Knight steps over small lips and seams in the ground instead of stopping dead at them.
  `knightStepHeight` sets how high a lip that is, as a share of its own height.
- "Map Shows Collected Pickups" in Debug Options makes Wayward Compass mark every pickup on the map
  rather than only the ones still out there, for working out where a new one should go. A room
  holding several now shows several pins instead of one.

- A buff bar under the companion's mask row, showing Baldur Shell's remaining blows. "Buff Bar" is
  on by default; `hudBuffBarOffsetX`, `hudBuffBarOffsetY`, `hudBuffIconSize`, `hudBuffIconScale` and
  `hudBuffIconSpacing` place it, and Ctrl+F5 rereads them.

- The Shade's "Skins" menu is now "Characters", and the Knight of Hallownest joins the Shade as a
  playable companion. It walks instead of floating, and pogos off Hornet with a down slash.
- The Knight's abilities follow Hornet's: Mothwing Cloak with her sprint, Mantis Claw with her wall
  climb, Double Jump alongside it, and Shade Cloak with Harpoon Dash. Jump is a new binding in
  Controls, unbound to anything the Shade uses.
- Ten charms join the roster: Weaversong, Defender's Crest, Flukenest, Spore Shroom, Thorns of
  Agony, Glowing Womb, Gathering Swarm, Grimmchild, Dream Wielder and Dreamshield. All work for
  both the Shade and the Knight, and each is found out in the world rather than bought.
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
- HUD layout can be tuned live: the frame, orb and mask row read their placement from `config.json`,
  and Ctrl+F5 rereads it without a restart.
- The HUD's masks and frame now use Hollow Knight's own art from the Knight bundle, at better than
  twice the resolution of the stills that shipped before.
- "Damage Summary File" now also records what the Shade and the Knight *deal* - the final damage of
  every nail hit and spell, and the nail's cooldown, so a charm's effect can be read off one swing.
- Casting a spell stops the Knight dead in the air for the cast, as in Hollow Knight, so a spell can
  be used to hang in place. Descending Dark holds only until its own dive starts.
- The Knight's asset bundle is read in the background at launch instead of the first time the
  Characters menu needs it, which is where the one-second freeze came from.
- The Knight sits down beside Hornet when she rests at a bench, walking over for it and being placed
  there if the way is blocked.
- Shade AI is unavailable while the Knight is equipped and reads as such in the menu — it steers by
  synthesising input, which cannot drive a character that has to jump and climb. Your setting is
  kept and returns with the Shade.

## 1.2.0

### Fixed

- The Knight now comes back to Hornet while her controls are locked, so it appears at the entrance
  after a transition instead of being left behind, and it stays put on a bench instead of falling.
- Grimmchild flies at its bearer's shoulder and spits fireballs at any angle, animated from the
  Grimmchild III art, instead of drifting into foes. It and its fireballs stop at terrain.
- Weaversong, Glowing Womb, Dreamshield, Defender's Crest, Spore Shroom, Thorns of Agony and
  Flukenest now use Hollow Knight's own art for what they summon or throw, instead of the charm
  icon. Weaverlings run along the ground rather than drifting over it.
- Sharp Shadow gives the Shade the sharpened cloak animation the Knight already had.
- Grubberfly's Elegy throws Hollow Knight's own crescent beam, one per direction, instead of a
  placeholder bolt.
- Flukenest throws 9 flukes, or 16 with Shade Soul, lobbed on an arc that lands them spread across
  the ground ahead of the caster. They are smaller, and they burst on terrain.
- Defender's Crest's cloud is smaller and much fainter.
- The Dreamshield circles the bearer evenly with its point turned outward.
- Idle weaverlings wander instead of piling up, and one that cannot reach its target goes back to
  the bearer and tries again.
- Thorns of Agony bursts Hollow Knight's own vines instead of stray lines, at the companion's own
  size and centred on it, with the companion standing aside so the burst is its body rather than a
  second one. Defender's Crest no longer throws orange streaks across the room.
- Grubberfly's Elegy beams face the way they are travelling.
- Flukes carry a hitbox their own size, so they no longer burst the instant they are thrown.
- The Dreamshield is smaller, closer and slower.
- The Shade's charm grid is drawn at the right size on high-resolution displays instead of at twice
  its intended size, where it overlapped the notch row and the charm description. It also no longer
  changes size between the first and later times it is opened in a scene.
- Grubberfly's Elegy beams point the way they are fired, leave from directly above and below on
  vertical shots, carry about an arm's reach past the companion, and stop at walls.
- The debug HP keys now trigger the charms that react to damage, so Thorns of Agony, Baldur Shell
  and Carefree Melody can be tested with them. Requires "Debug Keys (HP/Soul)".
- Thorns of Agony bursts its vines again. Borrowed Hollow Knight effects are switched on when they
  are spawned, which several charms needed and none of them were getting.
- The Shade's charm grid size can be set with `shadeCharmGridScale`, reread with Ctrl+F5.
- The Knight is pushed back by its own nail hits and by damage, and Steady Body stops it again. The
  push no longer spins it round to face away from whatever it just hit.
- Sprintmaster, and every other movement-speed charm, now works for the Knight, with its own walk
  cycle. Shape of Unn slows a focusing Knight to half speed rather than the Knight being able to
  walk freely, and draws Hollow Knight's slug form while it channels.
- Sharp Shadow works for the Knight's Shade Cloak, and plays the charm's own cloak animation.
- Grubberfly's Elegy adds a half-damage beam to the companion's own swing instead of replacing its
  nail with Hornet's and firing from her.
- Weaversong's weaverlings deal exactly 3 and only on contact. Flukenest's flukes deal 4, or 5 with
  Shaman Stone, in a tighter fan. Thorns of Agony deals exactly one nail slash.
- Defender's Crest and Spore Shroom now leave clouds that damage over time instead of landing as a
  single hit.
- Baldur Shell no longer makes focusing invulnerable: the hit still lands and still breaks the
  channel, the shell absorbs four of them, and a bench mends it.
- Gathering Swarm draws loose rosaries to Hornet, using the game's own magnet.
- A full SOUL meter is worth three focus heals again, instead of two.
- Fragile charms stay equipped when they break, so a bench repairs the loadout you had.
- The debug HP keys damage the companion for real, so focus can heal it and it survives a room
  change. Requires "Debug Keys (HP/Soul)".
- Dream Wielder and Nailmaster's Glory describe what they actually do, and say that the mechanics
  behind them are still to come.
- Losing a mask no longer throws an error when the HUD is rebuilt mid-animation.

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
