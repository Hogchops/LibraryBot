using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Collections.Generic;

namespace LibraryBot
{
    public class Commands : ModuleBase
    {
        protected static List<GameObj> games;
        private static IRole dmRole;
        protected static GameObj findGame(IUser usr)
        {
            for (int i = 0; i < games.Count; i++)
            {
                if (games[i].getDm().Equals(usr))
                {
                    return games[i];
                }
            }
            return null;
        }
        protected static GameObj findGame(IMessageChannel chnl)
        {
            for (int i = 0; i < games.Count; i++)
            {
                if (games[i].getChannel().Equals(chnl))
                {
                    return games[i];
                }
            }
            return null;
        }
        protected static bool checkVerification(ICommandContext Context)
        {
            IChannel chnl = Context.Channel;
            IUser usr = Context.User;
            GameObj gm = findGame(usr);
            if (gm == null)
            {
                return false;
            }
            if (gm.getChannel().Equals(chnl) && usr.Equals(gm.getDm()))
            {
                return true;
            }
            return false;
        }
        public async Task notVerified()
        {
            await Context.Channel.SendMessageAsync("You are not the DM for this specific channel.");
        }

        [Command("roll")]
        [Summary("Roll a die")]
        [Alias("r")]
        public async Task Roll([Remainder, Summary("Dice info")] string s)
        {
            s = s.ToLower().Trim();
            s = s.Replace(" ", "");
            int index = s.IndexOf("d");
            int indexOfPlus = s.IndexOf("+");
            int indexOfMinus = s.IndexOf("-");
            int firstNum = 0;
            int secondNum = 0;
            int thirdNum = 0;
            if (index == 0)
            {
                firstNum = 1;
            }
            else
            {
                try
                {
                    firstNum = Int32.Parse(s.Substring(0, index));
                }
                catch (FormatException)
                {
                    await Context.Channel.SendMessageAsync("1)That is not a number.");
                    return;
                }
            }
            try
            {
                if (indexOfPlus == -1 && indexOfMinus == -1)
                {
                    secondNum = Int32.Parse(s.Substring(index + 1));
                }
                else
                {
                    if (indexOfPlus != -1)
                    {
                        secondNum = Int32.Parse(s.Substring(index + 1, indexOfPlus - (index + 1)));
                    }
                    else if (indexOfMinus != -1)
                    {
                        secondNum = Int32.Parse(s.Substring(index + 1, indexOfMinus - (index + 1)));
                    }
                }
            }
            catch (FormatException)
            {
                await Context.Channel.SendMessageAsync("That is not a number.");
                return;
            }
            try
            {
                if (indexOfPlus != -1)
                {
                    thirdNum = Int32.Parse(s.Substring(indexOfPlus + 1));
                }
                else if (indexOfMinus != -1)
                {
                    thirdNum = Int32.Parse(s.Substring(indexOfMinus + 1));
                }
            }
            catch (FormatException)
            {
                await Context.Channel.SendMessageAsync("3)That is not a number.");
            }
            if (secondNum != 4 && secondNum != 6 && secondNum != 8 && secondNum != 10 && secondNum != 12 && secondNum != 20 && secondNum != 100)
            {
                await Context.Channel.SendMessageAsync("That is not a valid die.");
            }
            else
            {
                int sum = 0;
                Random rnd = new Random();
                string rolls = "";
                for (int i = 0; i < firstNum; i++)
                {
                    int roll = rnd.Next(1, secondNum + 1);
                    sum += roll;
                    if (i + 1 == firstNum)
                    {
                        rolls += "" + roll;
                    }
                    else
                    {
                        rolls += roll + ", ";
                    }
                }
                if (indexOfPlus != -1)
                {
                    sum += thirdNum;
                }
                else if (indexOfMinus != -1)
                {
                    sum -= thirdNum;
                }

                if (firstNum == 1)
                {
                    await Context.Channel.SendMessageAsync(Context.User.Mention + " rolled a **" + sum + "**.");
                }
                else
                {
                    await Context.Channel.SendMessageAsync(rolls);
                    await Context.Channel.SendMessageAsync(Context.User.Mention + " rolled a **" + sum + "**.");
                }
            }
        }
		
		[Command("newgame")]
        [Summary("Setup the bot for DnD")]
        [Alias("ng")]
        [RequireContext(ContextType.Guild)]
        public async Task newGame()
        {
            ITextChannel chnl = await Context.Guild.GetTextChannelAsync(481285807651553290);
            await chnl.SendMessageAsync("**New Game**\nDM: " + Context.User.Mention + "\nChannel: " + Context.Channel + "\nTime: " + DateTime.Now.TimeOfDay.ToString().Substring(0, 8));

            dmRole = Context.Guild.GetRole(708821099399610369);
            if (dmRole == null)
            {
                dmRole = await Context.Guild.CreateRoleAsync("Dungeon Master", null, Color.DarkRed);
            }
            if (games == null)
            {
                games = new List<GameObj>();
            }
            bool alreadyDm = false;
            bool alreadyChannel = false;
            for (int i = 0; i < games.Count; i++)
            {
                if (Context.User == games[i].getDm())
                {
                    await Context.Channel.SendMessageAsync("You are already a DM in " + games[i].getChannel().Name);
                    alreadyDm = true;
                }
            }
            for (int i = 0; i < games.Count; i++)
            {
                if (Context.Channel == games[i].getChannel())
                {
                    await Context.Channel.SendMessageAsync("There is already a game happening in this channel.");
                    alreadyChannel = true;
                }
            }
            if (alreadyDm == false && alreadyChannel == false)
            {
                games.Add(new GameObj(Context.User, Context.Channel));
                await Context.Guild.GetUserAsync(Context.User.Id).Result.AddRoleAsync(dmRole);
                await Context.Channel.SendMessageAsync("I am ready to collect player names for DnD!\nPlayers, enter: **!p a** to add yourself to the party.");
            }
        }

        [Command("help")]
        [Summary("List of commands.")]
        [Alias("h")]
        public async Task Help()
        {
            await Context.Channel.SendMessageAsync("Roll some dice! Commands:\n"
                + "**!roll** num d num +/- num** | Examples: **!roll 4d8 + 2** | **!roll 2d10 - 3** | **!roll d20**"
                + "Some other useful commands during a DnD session:\n"
                + "**!chelp** | Help page for character creation.\n"
                + "**!p a** | Add yourself to the party\n"
                + "**!p l** | List all the players\n"
                + "**!i e** [num] | Enter your initiative with value [num]\n"
                + "**!i l** | List all the initiatives\n"
                + "**!next** | Signal the next player it is their turn\n"
                + "\nIf you are a dm, use **!dmh** for dm help.");
        }

        [Command("dmhelp")]
        [Summary("A list of commands for DMs")]
        [Alias("dmh")]
        public async Task dmHelp()
        {
            await Context.User.SendMessageAsync("Here are the commands:\n"
                + "General setup commands, setting up the game (these are in order of use):\n"
                + "**!ng** | Setup the game, this will allow players to use **!p a** to add themselves to the party.\n"
                + "**!p d** | Stop collecting new party members\n"
                + "**!p r** | Reset the party, including initiatives. Run **!setup** again to start collecting new players.\n"
                + "\nNext, during a combat, you want to setup initiatives:\n"
                + "**!i g** | Get initiatives from players. This will allow players to use **!i e** to enter their initiatives.\n"
                + "**!i l** | Show all the initiatives.\n"
                + "**!i r** | reset only the initiatives\n"
                + "\nThen, you want to setup monsters (you can dm these commands to the bot to keep them hidden):\n"
                + "**!m a** [initiative] [monstername] | Add a monster. Sample: **!m a 14 monster1** to add a monster called monster1 with initiative 14.\n"
                + "**!m l** | List all the monsters in order of their initiative.\n"
                + "**!m k** [monstername] | Kill the target monster. Sample: **!m k monster1** to kill monster1, removing them from the list.\n"
                + "**!m r** | Reset the monster lists.\n"
                + "\nAfter both initiatives and monsters, to run the combat encounter, here are commands to help manage turns:\n"
                + "**!next** | ping the next player or monster\n"
                + "**!rt** | reset the current turn to max initiative\n"
                + "\nAdditionally, you have the ability to manage nicknames, and to mute, deafen, and move people in voice channels.\n"
                + "You also have the ability for priority speaking, google that for more information.\nWhen you are done, enter **!eg**. **__Please remember to do this!__**");
        }


        [Command("next")]
        [Summary("Get the next player")]
        [Alias("n")]
        public async Task nextPlayer()
        {
            await Context.Channel.SendMessageAsync(findGame(Context.Channel).nextPlayer());
        }

        [Command("resetTurn")]
        [Summary("Reset the turn number to max initiative")]
        [Alias("rt")]
        public async Task resetTurn()
        {
            bool verification = checkVerification(Context);
            if (verification)
            {
                findGame(Context.Channel).resetTurns();
                await Context.Channel.SendMessageAsync("The current player has been reset.");
            }
            else
            {
                await notVerified();
            }
        }

        [Group("player")]
        [Summary("Groups to manage players")]
        [Alias("p")]
        public class Player : ModuleBase
        {
            public async Task notVerified()
            {
                await Context.Channel.SendMessageAsync("You are not the DM for this specific channel.");
            }

            [Command("add")]
            [Summary("Add a player to the list")]
            [Alias("a")]
            public async Task Add()
            {
                int error = findGame(Context.Channel).addPlayer(Context.User);
                if (error == 1)
                {
                    await Context.Channel.SendMessageAsync("The DM is no longer collecting additional players.");
                }
                else if (error == 2)
                {
                    await Context.Channel.SendMessageAsync("You are already in the party.");
                }
                else
                {
                    await Context.Channel.SendMessageAsync("You have been successfully added to the party.");
                }
            }

            [Command("done")]
            [Summary("Stop collecting players")]
            [Alias("d")]
            public async Task stopPlayers()
            {
                bool verification = checkVerification(Context);
                if (verification)
                {
                    findGame(Context.Channel).donePlayers();
                    await Context.Channel.SendMessageAsync("No longer accepting new players.");
                    await list();
                }
                else
                {
                    await notVerified();
                }
            }

            [Command("reset")]
            [Summary("Reset the initiatives and players")]
            [Alias("r")]
            public async Task reset()
            {
                bool verification = checkVerification(Context);
                if (verification)
                {
                    findGame(Context.Channel).resetPlayers();
                    await Context.Channel.SendMessageAsync("Players and initiatives reset.");
                }
                else
                {
                    await notVerified();
                }
            }

            [Command("list")]
            [Summary("List the players")]
            [Alias("l")]
            public async Task list()
            {
                string msg = findGame(Context.Channel).listPlayers();
                await Context.Channel.SendMessageAsync(msg);
            }

            [Group("set")]
            [Summary("Set stats")]
            [Alias("s")]
            public class Stats : ModuleBase
            {

            }
        }

        [Group("initiative")]
        [Summary("Initiative commands")]
        [Alias("i")]
        public class Initiatives : ModuleBase
        {
            public async Task notVerified()
            {
                await Context.Channel.SendMessageAsync("You are not the DM for this specific channel.");
            }

            [Command("get")]
            [Summary("Get initiatives")]
            [Alias("g")]
            public async Task getInitiatives()
            {
                bool verification = checkVerification(Context);
                if (verification)
                {
                    int error = findGame(Context.Channel).getInitiatives();
                    if (error == 1)
                    {
                        await Context.Channel.SendMessageAsync("The party is not yet finished forming. Enter **!p d** to lock the party.");
                    }
                    else
                    {
                        string msg = findGame(Context.Channel).alertPlayers();
                        await Context.Channel.SendMessageAsync(msg + "it is time to roll for initiative!\nPlease enter **!i e [num]** to enter your initiative!");
                    }
                }
                else
                {
                    await notVerified();
                }

            }

            [Command("enter")]
            [Summary("Input initiatives")]
            [Alias("e")]
            public async Task Input(int num)
            {
                int error = findGame(Context.Channel).enterInitiative(Context.User, num);
                if (error == 1)
                {
                    await Context.Channel.SendMessageAsync("Initiatives are not being collected.");
                }
                else if (error == 2)
                {
                    await Context.Channel.SendMessageAsync("You are not in the party.");
                }
                else
                {
                    await Context.Channel.SendMessageAsync(Context.User.Mention + "'s initiative is " + num);
                    if (error == 3)
                    {
                        await Context.Channel.SendMessageAsync("All players have now entered their initiatives.");
                        await doneMethod();
                    }
                }
            }

            [Command("done")]
            [Summary("Stop collecting initiatives")]
            [Alias("d")]
            public async Task Done()
            {
                bool verification = checkVerification(Context);
                if (verification)
                {
                    int error = findGame(Context.Channel).doneInitiative();
                    if (error == 1)
                    {
                        await Context.Channel.SendMessageAsync("Initiatives are not being collected.");
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("No longer accepting new inititatives.\nThe players have been sorted according to their initiative.");
                        await List();
                    }
                }
                else
                {
                    await notVerified();
                }
            }

            public async Task doneMethod()
            {
                int error = findGame(Context.Channel).doneInitiative();
                if (error == 1)
                {
                    await Context.Channel.SendMessageAsync("Initiatives are not being collected.");
                }
                else
                {
                    await Context.Channel.SendMessageAsync("No longer accepting new inititatives.\nThe players have been sorted according to their initiative.");
                    await List();
                }
            }

            [Command("reset")]
            [Summary("Reset the initiatives")]
            [Alias("r")]
            public async Task resetInitiatives()
            {
                bool verification = checkVerification(Context);
                if (verification)
                {
                    findGame(Context.Channel).resetInitiatives();
                    await Context.Channel.SendMessageAsync("Initiatives reset.");
                }
                else
                {
                    await notVerified();
                }
            }

            [Command("list")]
            [Summary("List all the initiatives")]
            [Alias("l")]
            public async Task List()
            {
                string msg = findGame(Context.Channel).listInitiatives();
                await Context.Channel.SendMessageAsync(msg);
            }
        }

        [Group("monster")]
        [Summary("Monster commands")]
        [Alias("m")]
        public class Monster : ModuleBase
        {
            public async Task notVerified()
            {
                await Context.Channel.SendMessageAsync("You are not the DM for this specific channel.");
            }

            [Command("add")]
            [Summary("Add a monster")]
            [Alias("a")]
            public async Task Add(int initiative, [Remainder, Summary("Monster name")] string monster)
            {
                bool verification = checkVerification(Context);
                if (verification)
                {
                    int error = findGame(Context.Channel).addMonster(monster, initiative);
                    if (error == -1)
                    {
                        await Context.Channel.SendMessageAsync("That monster already exists, please pick a new name.");
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("I have added the monster for you in sorted order.");
                    }
                }
                else
                {
                    await notVerified();
                }
            }

            [Command("list")]
            [Summary("Show the monsters")]
            [Alias("l")]
            public async Task Show()
            {
                bool verification = checkVerification(Context);
                if (verification)
                {
                    string msg = findGame(Context.Channel).listMonsters();
                    await Context.Channel.SendMessageAsync(msg);
                }
                else
                {
                    await notVerified();
                }
            }

            [Command("reset")]
            [Summary("Reset the monsters")]
            [Alias("r")]
            public async Task Reset()
            {
                bool verification = checkVerification(Context);
                if (verification)
                {
                    findGame(Context.Channel).resetMonsters();
                    await Context.Channel.SendMessageAsync("The monsters have been reset.");
                }
                else
                {
                    await notVerified();
                }
            }

            [Command("kill")]
            [Summary("Kill a monster")]
            [Alias("k")]
            public async Task Kill(string monster)
            {
                bool verification = checkVerification(Context);
                if (verification)
                {
                    int error = findGame(Context.Channel).removeMonster(monster);
                    if (error == -1)
                    {
                        await Context.Channel.SendMessageAsync("That monster was not found. Please see **!p l** to see the monster names.");
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("The monster has been removed.");
                    }
                }
                else
                {
                    await notVerified();
                }
            }

            [Command("damage")]
            [Summary("Damage a monster")]
            [Alias("d")]
            public async Task damage(int damage, string monsterName)
            {
                bool verification = checkVerification(Context);
                if (verification)
                {
                    int error = findGame(Context.User).attackMonster(monsterName, damage);
                    if (error == 1)
                    {
                        await Context.User.SendMessageAsync("Monster not found.");
                    }
                    else if (error == 2)
                    {
                        await Context.User.SendMessageAsync("The monster has died.");
                    }
                    else
                    {
                        await Context.User.SendMessageAsync("The monster took " + damage + " damage.");
                    }
                }
                else
                {
                    await notVerified();
                }
            }
        }

        [Command("endgame")]
        [Summary("End your game")]
        [Alias("eg")]
        public async Task endGame()
        {
            bool verification = checkVerification(Context);
            if (verification)
            {
                ITextChannel chnl = await Context.Guild.GetTextChannelAsync(481285807651553290);
                await chnl.SendMessageAsync("**Game Deletion**\nDM: " + Context.User.Mention + "\nChannel: " + Context.Channel + "\nTime: " + DateTime.Now.TimeOfDay.ToString().Substring(0, 8));
                GameObj game1 = findGame(Context.Channel);
                games.Remove(game1);
                await Context.Guild.GetUserAsync(Context.User.Id).Result.RemoveRoleAsync(dmRole);
                await Context.Channel.SendMessageAsync("The game has been deleted. Thank you for playing!");
            }
            else
            {
                await notVerified();
            }
        }
    }
}