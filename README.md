In Legacy of the Abyss one player controls Hornet (Using controller) while the other controls the Shade (Using mouse and keyboard).

The Shade is a unique character that behaves completely differently to Hornet.

- The Shade can fly
- The Shade unlocks a dash and sprint at the same time as Hornet (So they can keep up with her)
- The Shade can attack with it's nail, this damage scales with Hornet's needle upgrades. (Sideslash, upslash and downslash have their own hotkeys to account for a flying character not really being able to use the standard 1-button-slash setup very easily.)
- The Shade can cast the same iconic spells from the first game. These spells unlock/upgrade each time hornet unlocks one of her six spells.
- The Shade can heal using Focus or by being near Hornet when she uses Bind while close to  them (Which will also revive a dead Shade if the inactive shade is close enough to Hornet)
- The Shade can teleport back to Hornet if it gets stuck (This can happen due to the fact that the Shade gets pulled toward Hornet if it gets too far from her)
- The Shade has half the number of masks that Hornet does (Rounding up), but Hornet can revive them if they die by using Bind near them.
- You can toggle a Mario-style "assist mode" that just makes the shade immune to damage


WARNING: The mod is still very early in development, I have not had to time to complete a full play through (Or even a significant chunk of one) while using it. There WILL be issues, likely pretty significant ones. Please let me know in the discussion section (Ideally in a nice and informative way) and I'll do my best to fix it.


FAQ:

Doesn't this make this game much easier?

- Yes, enormously so. With the general discourse with regards to difficulty as it is, I figured my first implementation should probably make things easier. I have now implemented menu options to scale the Shade and Hornets damage anywhere between 20% and 200%, alternatively change how much the Shade and Hornet heal themselves and each other for.

Okay, so Hornet can pogo using the shade, doesn't that make a huge number of skips possible and break progression?

- Yes. If you want to use the Shade pogo's to go places the game doesn't expect you to go, feel free, but I haven't added any code to the game to balance for that or to make sure it doesn't break your story progression. So do so at your own risk.

How do I upgrade the shades damage?

- As mentioned above, the shades damage should all scale with Hornet's Needle damage.

How do I get new spells for the shade?

- Each time Hornet unlocked one of her six "silk" spells, you gain a spell. Once you have all three, it starts upgrading your spells to their abyss variants.

I found a bug!

- Unsurprising, you're in Pharloom, they live here. Jokes aside, testing the entire game for a mod of this scale is frankly impossible for me to do by myself, I'll be relying on reports by users and will fix things as soon as I can. 

Why create a whole new character rather than just putting in a second Hornet, or even the Knight?

- For local co-op, that probably would have been fine, but Steam-Play-Together (What I first had in mind when designing this mod) introduces an amount of lag that would make playing a character with Hornet's move-set a very bad experience. Maybe in the future I'll think about adding a "Knight-like" skin and move-set, but for now that's very far out of scope.

The animations look bad! Why didn't you just use the ones from the first game?

- To a degree, I did, but Hollow Knight's sprite sheets are a total mess to use when you don't have access to more advanced sprite-work tools built into unity. I'm using pure code here, all my animations are functionally just "Make X image appear at Y co-ordinates for Z frames" and stringing that together just barely well enough to make it look animated. The animation work that I did manage to get done here was probably about 50% of the time I spent working on the mod. Trust me, if I had an easier option (That I knew about), I would have taken it.

Why do all your assets look really low-res?

- Because that's what I had access to. If the mod gets popular enough, I'll try to get someone who knows what they're doing artistically to help me with it, but outside of using AI (Which is simply not on the cards here, I refuse to intentionally be the one to turn all of Team Cherry's assets into training data), I don't have the knowledge/resources to remake or recreate these assets myself.

This mod is janky, it should be better!

- Not a question, but even in testing I had people tease me that the mod wasn't up to the standards of Team Cherry or even mods from late Hollow Knight. I want to be clear, I'm working with exactly zero tools here, but yes, you are right, it could be better. I will be attempting to improve the mod in the future, because I think it's cool and I want to use it more myself.

Can I copy/redistrubute your mod, if I make my own changes?

- Only if you get my permission, which I'm only likely to grant to someone with a proven track record of modding, will provide me credit, and can tell me exactly what the plan to do with my mod (Which I then need to think is a cool idea)

Did you use AI for this mod?

- For the art? No, never. But I'm a rusty coder who hadn't touched C# in 10 years prior to starting this project, I used the crap outa chatGPT to troubleshoot my code.