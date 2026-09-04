In Legacy of the Abyss one player controls Hornet while the other controls a second character. You can pick between two of them in the Characters menu, and swap between them at any point with a button.

**The Shade** is a unique character that behaves completely differently to Hornet.

- The Shade can fly
- The Shade unlocks a dash and sprint at the same time as Hornet (So they can keep up with her)
- The Shade can attack with their nail, this damage scales with Hornet's needle upgrades. (Sideslash, upslash and downslash have their own hotkeys to account for a flying character not really being able to use the standard 1-button-slash setup very easily.)
- The Shade can cast the same iconic spells from the first game. These spells unlock/upgrade each time hornet unlocks one of her six spells.
- The Shade can heal using Focus or by being near Hornet when she uses Bind while close to  them (Which will also revive a dead Shade if the inactive shade is close enough to Hornet)
- The Shade can teleport back to Hornet if they get stuck (This can happen due to the fact that the Shade gets pulled toward Hornet if they get too far from her)
- The Shade has half the number of masks that Hornet does (Rounding up), but Hornet can revive them if they die by using Bind near them. You can change that share in the Difficulty menu.
- You can toggle a Mario-style "assist mode" that just makes the Shade immune to damage

**The Knight** of Hallownest is the other option, and they play much more like you'd expect. They walk and jump rather than flying, they pogo off Hornet with a down slash, and their abilities unlock alongside Hornet's own (Mothwing Cloak with her sprint, Mantis Claw with her wall climb, and so on). They use Hollow Knight's own art, animations and sounds.

Some other things that are in as of 2.0:

- The Shade can play themselves if you don't have a second player around. Turn on Shade AI and they'll pick targets, attack, cast, try to dodge things and heal you both. You can also point them at a spot on screen and tell them to hold it.
- Ten more charms, all of which work for either character: Weaversong, Defender's Crest, Flukenest, Spore Shroom, Thorns of Agony, Glowing Womb, Gathering Swarm, Grimmchild, Dream Wielder and Dreamshield.
- Difficulty presets and a new-game screen that asks how you want to play before you start.
- Soul Vessels, so your companion can bank SOUL past a full meter.
- A co-op camera that leans toward whichever of you is where, and widens a bit when you split up.

The quickest way to report bugs: press F8 straight away. The game freezes and asks what happened, then saves your description together with a screenshot, the game state, the logs and the last thirty seconds of what the Shade and Hornet were doing, into its own folder under BepInEx/config/LegacyoftheAbyss/bug_reports/. Zip that folder up and attach it and I won't need to ask you any follow-up questions. Nothing is uploaded anywhere by itself.


FAQ:

Doesn't this make this game much easier?

- Yes, enormously so, but there are now menu options to scale the Shade and Hornet's damage anywhere between 20% and 200%, alternatively change how much the Shade and Hornet heal themselves and each other for. There are also presets if you'd rather not fiddle with sliders, including one (Abyss) that's meant to be a lot harder than the base game. Ultimately these settings are just sliders though, I'll like into better ways of improving the difficulty customisation in the future.

Okay, so Hornet can pogo using the shade, doesn't that make a huge number of skips possible and break progression?

- Yes. If you want to use the Shade pogos to go places the game doesn't expect you to go, feel free, but I haven't added any code to the game to balance for that or to make sure it doesn't break your story progression. So do so at your own risk.

How do I upgrade the Shade's damage?

- Nail damage scales with Hornet's Needle damage, as mentioned above. Spells don't any more, as of 2.0 they deal that same as the first Hollow Knight's spells.

How do I get new spells for the shade?

- Each time Hornet unlocks one of her six "silk" spells, you gain a spell. Once you have all three, it starts upgrading your spells to their abyss variants.

I found a bug!

- Unsurprising, you're in Pharloom, they live here. Jokes aside, testing the entire game for a mod of this scale is frankly impossible for me to do by myself, I'll be relying on reports by users and will fix things as soon as I can. 

Why create a whole new character rather than just putting in a second Hornet, or even the Knight?

- For local co-op, that probably would have been fine, but Steam-Play-Together (What I first had in mind when designing this mod) introduces an amount of lag that would make playing a character with Hornet's move-set a very bad experience. That's why the Shade came first and is still the default. The Knight did eventually happen in 2.0 for anyone who does want to platform their way around, so if that's what you were after, it's in there now.

- This got a fair bit better in 2.0. The Knight animates from Hollow Knight's actual animation data rather than anything I strung together, so they should look about right.

Why do all your assets look really low-res?

- Because that's what I had access to. If the mod gets popular enough, I'll try to get someone who knows what they're doing artistically to help me with it, but outside of using AI (Which is simply not on the cards here, I refuse to intentionally be the one to turn all of Team Cherry's assets into training data), I don't have the knowledge/resources to remake or recreate these assets myself. The HUD and most of the charm effects are better as of 2.0, since those now come from Hollow Knight's own art at a much higher resolution than what I'd made.

This mod is janky, it should be better!

- Not a question, but even in testing I had people tease me that the mod wasn't up to the standards of Team Cherry or even mods from late Hollow Knight. I want to be clear, I'm working with exactly zero tools here, but yes, you are right, it could be better. I will be attempting to improve the mod in the future, because I think it's cool and I want to use it more myself.

Can I copy/redistribute your mod, if I make my own changes?

- I'm more lax on this now, feel free to use the mod as you see fit. I do request credit be provided to TheWr13r, Shopcreeper01 and myself.

Did you use AI for this mod?

- For the art? No, never.
- The mod did start as a test run about a year ago for this newfangled "Codex" I kept hearing about, so I used a lot of AI coding initially.
To be honest, it caused more problems than it solved and I'm much better at coding now than I was then. I still use Claude Code a bit, mostly to create plans of action or basic code drafts for features that I then mostly rewrite.