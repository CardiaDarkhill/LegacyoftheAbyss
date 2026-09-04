# Changelog

While a version is in development, one line per change, two at most. Say what changed for the
player and name the setting that controls it. At release those lines get folded down into the broad
categories below, because nobody is reading 370 bullet points. The git log keeps the detail.

Three things about how this file is written, because they are easy to get wrong:

- **The Knight and the Shade are "they", not "it".** Plenty of people read them as characters rather
  than objects, and "it" upsets some of them. Not worth doing that to anyone over a pronoun.
- **No em dashes.** Commas, brackets and full stops instead.
- Keep it casual, and don't oversell. If something is probably fixed rather than definitely fixed,
  say so.

## 2.0.0

The big one. There's a second playable character, the Shade can fight on their own, ten more charms,
and a lot of tidying up of things that were already here.

### The Knight of Hallownest

The Shade's "Skins" menu is now **Characters**, and the Knight joins the Shade as a companion you
can play as. They're a platformer character rather than a floating one, so they walk, jump,
wall-cling and pogo off Hornet with a down slash. The new **Swap Character** button switches between
the two wherever you're stood.
A HUGE thanks to Shownyoung for their incredible work on the Knight in Silksong mod, their permission
for its use, and their help with the some of the implementation here.

- Their abilities follow Hornet's: Mothwing Cloak with her sprint, Mantis Claw with her wall climb,
  Double Jump alongside it, Shade Cloak with Harpoon Dash.
- They use Hollow Knight's own art, animations and sounds, including Monarch Wings, Shade Cloak and
  the slug form for Shape of Unn. They pull out their own map, sit down beside Hornet at a bench,
  and ride balloons the way she does.
- Attacks are aimed with the movement stick, which frees up the down slash button for Jump. Casting
  stops them dead in the air like it does in Hollow Knight, so you can use a spell to hang over a
  gap.
- They carry a brighter light than the Shade (`knightLightRadiusMultiplier`,
  `knightLightIntensityMultiplier`) and step over small lips instead of catching on them
  (`knightStepHeight`).

### Shade AI

Turn on **Shade AI** and the Shade will play on their own. They pick a target, close on it, attack, cast
where a spell is actually worth the SOUL, try to step out of things that would hurt them, and heal
you both when either of you is low. They can still be killed, so you'll still need to revive them.

With nothing to fight they keep station just behind Hornet, and drop ahead of and below her when she
leaves the ground, close enough to pogo off over a gap. You can tell them where to stand with
**Command Shade** (middle mouse, or left stick click): tap twice for "hold here", or aim the reticle
to send them somewhere on screen.

It's switched off while the Knight is equipped. The AI works by faking controller input, which
doesn't really work for a character that has to jump and climb their way around.

### Charms

Ten more: Weaversong, Defender's Crest, Flukenest, Spore Shroom, Thorns of Agony, Glowing Womb,
Gathering Swarm, Grimmchild, Dream Wielder and Dreamshield. All of them work for both characters,
and each one is found out in the world rather than bought.

Every summoning charm now uses Hollow Knight's own art for whatever it throws or spawns, instead of
the charm icon, and at Hollow Knight's sizes and damage. Most of them needed fixing as well as
redrawing: weaverlings piled up, flukes burst the moment they were thrown, the Dreamshield pointed
the wrong way, Grimmchild drifted into things, and a couple of clouds were drawn at about a fifth of
what they actually damaged.

Charm behaviour got a pass too. Fragile Heart no longer makes your companion basically unkillable,
Hiveblood regenerates across rooms, taking a charm off shouldn't leave you worse than wearing
nothing any more, notch costs match Hollow Knight, and fragile charms stay equipped when they break
so a bench repairs the loadout you had.

### Spells

Spells should now deal Hollow Knight's own damage, flat: Vengeful Spirit 15, Shade Soul 30, Howling
Wraiths 13x3, Abyss Shriek 20x4, Desolate Dive 15+20, Descending Dark 15+48. They used to scale off
Hornet's needle, which ended up being worth several times the spell once the Knight's own upgrades
and spell charms stacked on top of it.

While I was in there: every spell was landing **twice**, the multi hit screams were only landing
once, Shaman Stone now varies per spell like it's supposed to, Desolate Dive's shockwave is actually
visible, and Vengeful Spirit rides over uneven ground instead of bursting on it.

### Difficulty and starting a game

- Starting a new game asks you a few things first: whether to reset that slot's shade progress, what
  difficulty to play at, and whether to play as the Shade or the Knight
  (`shadeNewGameOptionsEnabled`).
- A fourth difficulty, **Abyss**, which is meant to be unfair. Your companion keeps their healing
  and stays revivable, so a death doesn't end the second player's evening. What used to be Abyss is
  now Hard, so a run in progress keeps the same numbers under the new name.
- Difficulty belongs to the save profile now rather than the install, so two files can be played two
  different ways.
- **Assist Mode** is a switch on the Difficulty screen, damage is split four ways (Shade Nail, Shade
  Spells, Hornet Needle, Hornet Silk Skills), and **Shade Masks** sets your companion's health as a
  share of Hornet's.

### HUD and menus

- **Soul Vessels**: your companion earns one for every two increases to Hornet's silk maximum, up to
  three, each holding 33 SOUL beyond the meter. They show up as orbs next to the soul meter.
- A **buff bar** under the mask row, which currently shows Baldur Shell's remaining blows.
- The HUD uses Hollow Knight's own art now, at better than twice the resolution of what shipped
  before, and you can place every piece of it from `config.json` with **Ctrl+F5** to reread it live.
- The mod's menus fill the screen properly, explain whichever row is highlighted, follow the mouse,
  and hopefully stop throwing the highlight back to the top when you press something.
- **Wayward Compass** is now the **Abyssal Compass**, with new art, and marks rooms holding mod
  charms and notches you haven't picked up yet.
- The charm grid scales to fit the bigger roster, and draws at the right size on high resolution
  displays.
- Shadow wisps gather around the Shade as their SOUL fills, in the style of the first game. They
  double as the Shade Cloak cooldown readout and get pulled back in once it's ready.

### Controls

- Keyboard defaults are now **Hollow Knight's own layout**: arrows to move, Z jump, X attack, A
  focus, F quick cast, C dash. Hit Reset under Controls to pick them up on an existing config.
- **Assign Devices** replaces the four one click presets. Each player presses a button on whatever
  they're holding and the mod records which is which. Rebinding on a pad now remembers which pad it
  was.
- Every control is listed whichever character you have equipped, so you can set both schemes up
  front.
- Two controllers should work properly again. The second player's pad is reserved from Hornet, a
  rebound control only answers to its own pad, and holding a button across a room change no longer
  steals Hornet's input.

### Bug reports

Press **F8** to file one from inside the game. It freezes, asks what happened, and saves a
screenshot, the scene state, the logs and about thirty seconds of rolling samples into
`BepInEx/config/LegacyoftheAbyss/bug_reports/`. Nothing gets uploaded anywhere, it's all just local
files. Mod exceptions file one on their own.

### Other fixes worth mentioning

- Your companion shouldn't look see through any more, which was worst when they were a long way from
  Hornet. Their light was drawing its halo level with them instead of behind them.
- They also no longer draw over fog, snow, darkness, or scenery that Hornet is stood behind.
- Escape steps back one screen in the mod's menus instead of throwing you out to the pause menu.
- `config.json` and the shade save slots get written in one piece now, so a crash mid write can't
  leave you with a truncated file that reads as no settings and no progress.
- Your companion stops getting yanked back to Hornet after a room load, comes back at full health,
  and gets hidden for cutscenes and memory sequences.
- The Knight's pogo reaches as far as their nail does, works on the environment as well as enemies,
  and doesn't bounce off corpses or background scenery any more.
- Boss attacks that mark an area hit whoever is stood in it, rather than dragging Hornet across the
  arena into something the Shade walked into (`shadeBossAttackSharingEnabled`).
- Your companion should only be hurt by things that would hurt Hornet now. No more phantom hitboxes
  from telegraphs, marker volumes or switched off colliders.
- Hornet doesn't lose her keyboard bindings on a room change, or stand still for a second after a
  transition.
- You can now talk to Flick the Fixer after he fixes the Wishing Wall... somehow... I had this bug
  too and it just... went away at some point.

Enemies picking a target is still a bit hit and miss. Some of them will happily go after your
companion and others carry on ignoring them. I don't think that one ever really gets finished, but
it should be better than it was.

### Note

- These settings live in `BepInEx/config/LegacyoftheAbyss/config.json`.
