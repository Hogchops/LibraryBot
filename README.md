# LibraryBot
This bot manages a DnD game. Begin by entering !ng in a text channel on a guild, and it will give you the guildmaster rank.
Afterwards, follow the commands and check the !help page to help run your game.
You can create characters and save them, check the !chelp page for more information.

## Code Directory
The following is a general summary of the different files.
### Program.cs
This will ge the bot online and running. Token obviously needs to be changed to an actual bot token.
### CharacterNode.cs
This is the class for character objects, keeping track of all their data. Further development will hope to add class functionality.
### Commands.cs
This is the general command class, adding the majority of commands to the bot.
### CharacterCommands.cs
This adds character related commands to the bot.
### GameObj.cs
This is a class that is the bulk of the game-related commands, manipulating the players, monsters, and related. Most commands found in Commands.cs have a partner method in this class for implementation.
### MonsterNode.cs
This contains all the information needed for monsters.
### UserNode.cs
This contains all the information needed for players.
