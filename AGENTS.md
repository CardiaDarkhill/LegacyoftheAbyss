# AGENTS.md file

## Dev environment tips
- This project is a mod for the newly released "Hollow Knight: Silksong", there is very little documentation available for modding this game due to it's new release.
- To assist with the above, I have decompiled much of the games code, which you can find at: "\DLLDecompiles\Assembly-CSharp". Make extensive use of this code to inform your work.
- This is a mod for a unity game using BepInEx to implement.


## Project Goals
- The ultimate goal of this project is to create a mod that creates a shade companion for Hollow Knight: Silksong that assists the player in combat
- Both the Shade and Hornet have seperate reconfigurable controls. The shade should be able to control the menus in the same manner that the Hornet player can
- Currently uses "PlayTogetherWhatever" to enable online co-op, though if a more elegent online solution can be found, that would be ideal
- The shade is to be a powerful and helpful companion, that looks polished and appropriate for the environment, but ideally will both be fun to play and not totally break the game progression (Though some amount of that will be inevitable)
- The shade should feel like a full, seemless inclusion in the game, with its own progression and build options


## To Do:
- At time of writing the following features requests need to be actioned:

- Charms, notches and other progression items for the shade, independant of just using Hornet's progression (A major issue persists where the Shade's charm menu displays nothing)
- Demo video of gameplay for the Nexusmods page
- Ability unlock pop-ups for when the shade gets new abilities (These should like identical to those that pop up for when Hornet gains an item)
- Improved damage logging for the shade, for some reason, certain hits that should be damaging the shade are both not damaging it and not showing up in the logs as a blocked hit


## Testing:

- The program must compile with the "dotnet build -c Release" command

