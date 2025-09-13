# AGENTS.md file

## Dev environment tips
- This project is a mod for the newly released "Hollow Knight: Silksong", there is very little documentation available for modding this game due to it's new release.
- To assist with the above, I have decompiled much of the games code, which you can find at: "\DummyDLLs\DLLDecompiles\Assembly-CSharp\Assembly-CSharp". Make extensive use of this code to inform your work.
- Keep in mind that in the above folder, there are child folders that contain more code potentially several folders deep, if you can't find what you need for your task, you might need to look around.
- In the "\DummyDLLs\DLLDecompiles" folder is a txt file called "DecompileRequests.txt". If the existing decompiled code doesn't have what you need, mention what DLL's from "\DummyDLLs" you think might contain the right code in that txt file and I will decompile those for your future use.
- This is a mod for a unity game using BepInEx to implement.


## Project Goals
- The ultimate goal of this project is to create a mod that creates a shade companion for Hollow Knight: Silksong that assists the player in combat
- This shade functions using the keyboard while the Hornet player uses the controller
- Ultimately the plan is to use a tool like "Steam play anything together" to enable online co-op, though if a more elegent online solution can be found, that would be ideal
- The shade is to be a powerful and helpful companion, that looks polished and appropriate for the environment, but ideally will both be fun to play and not totally break the game progression (Though some amount of that will be inevitable)


## To Do:
- At time of writing the following bugs still need to be fixed:

- For some reason whenever we compile we're dumping a whole bunch of extra files into the plugins folder. The only thing that we should be outputing when we compile is LegacyoftheAbyss.dll and LegacyoftheAbyss.pdb
- The additional weirdly scaling nail strikes are still happening
- Desolate Dive/Descending Dark seems to stop it's dive when impacting specifically the top of Hornet's hitbox, it shouldn't stop until it hits the ground (The easiest way to target it should be using it from directly above an enemy and abusing the iframes it gives to dive directly into them and then quickly run out)



## Testing:

- The program must compile with the "dotnet build -c Release" command

## Scripts
- LegacyHelper.Core.cs: Plugin entry, scene events, shade spawn, state persistence.
- LegacyHelper.Patches.cs: Harmony patches (scene flow, bench/bind heals, input mapping).
- LegacyHelper.ShadeController.Core.cs: Shade movement/leash, hazards, HUD sync, light.
- LegacyHelper.ShadeController.Slash.cs: Shade nail slash + projectile spawning (no forward filter).
- LegacyHelper.Projectile.cs: Projectile hit logic (HitInstance/HitTaker).
- HUD.Core.cs: HUD lifecycle, debug keys (HP/Soul) with minimal logs.
- HUD.UI.cs: Canvas, soul orb, health masks, refresh logic.
- HUD.Assets.cs: Asset loading and sprite helpers.
- HUD.Audio.cs: Hurt SFX selection and playback.
