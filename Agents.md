# AGENTS.md file

## Dev environment tips
- This project is a mod for the newly released "Hollow Knight: Silksong", there is very little documentation available for modding this game due to it's new release.
- To assist with the above, I have decompiled much of the games code, which you can find at: "\DummyDLLs\DLLDecompiles\Assembly-CSharp\Assembly-CSharp". Make extensive use of this code to inform your work.
- Keep in mind that in the above folder, there are child folders that contain more code potentially several folders deep, if you can't find what you need for your task, you might need to look around.
- In the "\DummyDLLs\DLLDecompiles" folder is a txt file called "DecompileRequests.txt". If the existing decompiled code doesn't have what you need, mention what DLL's from "\DummyDLLs" you think might contain the right code in that txt file and I will decompile those for your future use.
- This is a mod for a unity game using BepInEx to implement.


## Project Goals
- The ultimate goal of this project is to create a mod that creates a shade companion for Hollow Knight: Silksong that assists the player in combat
- This shade functions using the keyboard while the Hornet player uses the keyboard
- Ultimately the plan is to use a tool like "Steam play anything together" to enable online co-op, though if a more elegent online solution can be found, that would be ideal
- The shade is to be a powerful and helpful companion, that looks polished and appropriate for the environment, but ideally will both be fun to play and not totally break the game progression (Though some amount of that will be inevitable)


## To Do:
- At time of writing the following features still need to be implemented:

The shade should have a hitbox, which should allow it to be harmed by enemies and used by Hornet to "pogo" (Which should not damage the shade)
The shades melee attack currently provides silk to Hornet, which it shouldn't do. It also can't destroy destructible terrain objects or hit non-enemy hittable objects like vines and fruit, it should be able to do this.
The shade should be able to gain soul through its melee attack and spend it through using it's spells (Like the already mostly implemented shade projectile). The shade should not be able to use spells without any soul.
The shade can currently pass through all terrain, it should collide with all the same terrain that hornet does. When the shade collides with terrain hazards, such as those that would normally teleport hornet to the previous safe ground she was standing on, the shade should be teleported to Hornet.
The shade, when reduced to 0 life, should enter an inactive state, it can still move in this state but nothing else. It can be revived either by being near Hornet when she uses her Bind, or by Hornet dying or sitting at a bench.
The shade still needs two additional spells, Descending Dark and Abyss Shriek.
Each of the shades spells needs two versions, their "Soul" variant and "Abyss" varient, like in the first hollow knight game. They will unlock these are the same time that Hornet unlocks certain things.
The shade needs a "Focus" ability, which lets it spend soul to heal, if Hornet is near the shade when it does this, she will also heal for the same amount.
The shades nail damage should scale with Hornets.
The shade needs its own sprite sheet/animations, currently it's just a black square. All of it's spells and attacks also need animations (Some of these can use assets and code already in Silksong, others will need assets taken from the first game).
Sound effects need to be set up for all of the Shades attacks, spells and focus abilities.
The shades "Leash" system is currently quite basic, it should pull the shade slowly toward hornet when it gets too far from her, when the shade gets very far from hornet, it should disable control and collision for the shade and pull it to hornet at a much faster speed. If for some reason even this isn't enough to get the Shade within an acceptable distance of Hornet within a few seconds, the shade should just be teleported to Hornet.