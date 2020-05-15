using System;
using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace LibraryBot
{
    public class CharacterCommandds : ModuleBase
    {

        private static List<CharacterNode> characters;

        protected static CharacterNode findCharacter(IUser user)
        {
            if (characters == null)
            {
                characters = new List<CharacterNode>();
            }
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i].GetUser().Equals(user))
                {
                    return characters[i];
                }
            }
            return null;
        }

        [Command("chelp")]
        [Summary("Help for character creation.")]
        [Alias("ch")]
        public async Task characterHelp()
        {
            string msg = "Here are the commands for Characters:\n"
                + "**!v** | View your character. I will DM you your Character's information.\n"
                + "**!cc [character name]** | Create a blank character with a name of your choosing.\n"
                + "**!o [file name]** | Open your active character with a specific filename.\n"
                + "**!s [file name]** | Save your character with a specific filename. Make sure to remember it!\n"
                + "**!set [stat] [number] | See **!c hc** for more information.\n"
                + "**!hc | Help page for creating a character.";
            await Context.User.SendMessageAsync(msg);
        }

        [Command("helpcreate")]
        [Summary("Help for character creation.")]
        [Alias("hc")]
        public async Task helpCreate()
        {
            string msg = "Here is some help setting up your character!\nYou will need to setup all the basic information for your character.\n"
                + "Here is the basic command you will be using during this entire process:\n"
                + "**!set [field] [data]** | Set a specific field of information with a specific data entry.\n"
                + "More general, you will need to do **!c set** for each statistic your character needs in order to function. Here are some examples:\n"
                + "**!set str 14** | **!c set level 3** | **!c set race Dragonborn**\n"
                + "Here are all the fields that you need to fill out before your character is ready to go:\n"
                + "ac | hp | str | con | dex | int | wis | cha | class | race | level | hitdice\n"
                + "Once done setting these values, you need to setup your proficiencies. Here are the following commands:\n"
                + "**!p add [type] [value] | Add a proficiency for value within type. Here are some examples:\n"
                + "**!p add st str** | **!p remove ability arcana** | !**p add e heavy armor**\n"
                + "Available types: saving throw (st), equipment (e), ability (a)\n"
                + "Available values: Anything for equipment, str, con, etc for saving throws, and arcana, survival, etc for ability.\n"
                + "If you have any problems in your character creation process, please let the active DM know.\n"
                + "Additionally, when you are done, type !c save [filename], to save your character for later! Make sure to remember the filename.";
            await Context.User.SendMessageAsync(msg);
        }

        [Command("view")]
        [Summary("View your character")]
        [Alias("v")]
        public async Task view()
        {
            if (characters == null)
            {
                characters = new List<CharacterNode>();
            }
            CharacterNode character = findCharacter(Context.User);
            if (character == null)
            {
                await Context.Channel.SendMessageAsync("You do not have an active character.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(character.toString());
            }
        }

        [Command("open")]
        [Summary("Open a saved character")]
        [Alias("o")]
        public async Task open([Remainder, Summary("File name")] string filename)
        {
            if (characters == null)
            {
                characters = new List<CharacterNode>();
            }
            CharacterNode old = findCharacter(Context.User);
            if (old != null)
            {
                characters.Remove(old);
            }
            CharacterNode character = new CharacterNode("temp");
            int error = character.openCharacter(filename, Context.User);
            if (error == 2)
            {
                await Context.Channel.SendMessageAsync("You are not the owner of that file");
            }
            else if (error == 3)
            {
                await Context.Channel.SendMessageAsync("I could not locate that file.");
            }
            else
            {
                await Context.Channel.SendMessageAsync("I successfully loaded the file. Enter **!c view** to view it.");
            }
            characters.Add(character);
        }

        [Command("create")]
        [Summary("Create a character")]
        [Alias("cc")]
        public async Task Create([Remainder, Summary("Character name")] string characterName)
        {
            if (characters == null)
            {
                characters = new List<CharacterNode>();
            }
            CharacterNode old = findCharacter(Context.User);
            if (old != null)
            {
                characters.Remove(old);
            }
            CharacterNode character = new CharacterNode(Context.User, characterName);
            characters.Add(character);
            await Context.User.SendMessageAsync("You created a new character named " + characterName +". Enter !hc for help creating your character.");
        }

        [Command("save")]
        [Summary("Save a character to a filename")]
        [Alias("s")]
        public async Task Save([Remainder, Summary("Filename")] string filename)
        {
            if (characters == null)
            {
                characters = new List<CharacterNode>();
            }
            CharacterNode old = findCharacter(Context.User);
            if (old == null)
            {
                await Context.Channel.SendMessageAsync("You have no active character.");
            }
            else
            {
                if (filename.Contains(' ') || filename.Contains('\\') || filename.Contains('/'))
                {
                    await Context.Channel.SendMessageAsync("That is not an adequite filename.");
                }
                else
                {
                    int error = old.saveToFile(Context.User, filename);
                    if (error == 1)
                    {
                        await Context.Channel.SendMessageAsync("An error occured while saving your character. Please try again later and contact a developer.");
                    }
                    else if (error == 2)
                    {
                        await Context.Channel.SendMessageAsync("That file already exists. Please select a new filename.");
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Character saved to " + filename);
                    }
                }
            }
        }

        [Group("set")]
        [Summary("Set different features about your class.")]
        public class Set : ModuleBase
        {

            [Command("strength")]
            [Summary("Set your strength score")]
            [Alias("str")]
            public async Task setStr(int num)
            {
                if (characters == null)
                {
                    characters = new List<CharacterNode>();
                }
                CharacterNode character = findCharacter(Context.User);
                if (character == null)
                {
                    await Context.User.SendMessageAsync("You have no active character.");
                }
                else
                {
                    character.setStr(num);
                    await Context.User.SendMessageAsync("You have updated your Strength to " + num);
                }
            }

            [Command("constitution")]
            [Summary("Set your constitution")]
            [Alias("con")]
            public async Task setCon(int num)
            {
                if (characters == null)
                {
                    characters = new List<CharacterNode>();
                }
                CharacterNode character = findCharacter(Context.User);
                if (character == null)
                {
                    await Context.User.SendMessageAsync("You have no active character.");
                }
                else
                {
                    character.setCon(num);
                    await Context.User.SendMessageAsync("You have updated your Constitution to " + num);
                }
            }

            [Command("dexterity")]
            [Summary("Set your dexterity")]
            [Alias("dex")]
            public async Task setDex(int num)
            {
                if (characters == null)
                {
                    characters = new List<CharacterNode>();
                }
                CharacterNode character = findCharacter(Context.User);
                if (character == null)
                {
                    await Context.User.SendMessageAsync("You have no active character.");
                }
                else
                {
                    character.setDex(num);
                    await Context.User.SendMessageAsync("You have updated your Dexterity to " + num);
                }
            }

            [Command("intelligence")]
            [Summary("Set your intelligence")]
            [Alias("int")]
            public async Task setInt(int num)
            {
                if (characters == null)
                {
                    characters = new List<CharacterNode>();
                }
                CharacterNode character = findCharacter(Context.User);
                if (character == null)
                {
                    await Context.User.SendMessageAsync("You have no active character.");
                }
                else
                {
                    character.setIntel(num);
                    await Context.User.SendMessageAsync("You have updated your Intelligence to " + num);
                }
            }

            [Command("wisdom")]
            [Summary("Set your wisdom")]
            [Alias("wis")]
            public async Task setWis(int num)
            {
                if (characters == null)
                {
                    characters = new List<CharacterNode>();
                }
                CharacterNode character = findCharacter(Context.User);
                if (character == null)
                {
                    await Context.User.SendMessageAsync("You have no active character.");
                }
                else
                {
                    character.setWis(num);
                    await Context.User.SendMessageAsync("You have updated your Wisdom to " + num);
                }
            }

            [Command("charisma")]
            [Summary("Set your charisma")]
            [Alias("cha")]
            public async Task setCha(int num)
            {
                if (characters == null)
                {
                    characters = new List<CharacterNode>();
                }
                CharacterNode character = findCharacter(Context.User);
                if (character == null)
                {
                    await Context.User.SendMessageAsync("You have no active character.");
                }
                else
                {
                    character.setChr(num);
                    await Context.User.SendMessageAsync("You have updated your Charisma to " + num);
                }
            }

            [Command("ac")]
            [Summary("Set your ac")]
            public async Task setAC(int num)
            {
                if (characters == null)
                {
                    characters = new List<CharacterNode>();
                }
                CharacterNode character = findCharacter(Context.User);
                if (character == null)
                {
                    await Context.User.SendMessageAsync("You have no active character.");
                }
                else
                {
                    character.setAc(num);
                    await Context.User.SendMessageAsync("You have updated your AC to " + num);
                }
            }

            [Command("hp")]
            [Summary("Set your hitpoints")]
            public async Task setHP(int num)
            {
                if (characters == null)
                {
                    characters = new List<CharacterNode>();
                }
                CharacterNode character = findCharacter(Context.User);
                if (character == null)
                {
                    await Context.User.SendMessageAsync("You have no active character.");
                }
                else
                {
                    character.setHp(num);
                    character.setMaxHP(num);
                    await Context.User.SendMessageAsync("You have updated your HP to " + num);
                }
            }

            [Command("level")]
            [Summary("Set your class level")]
            [Alias("l")]
            public async Task level(int num)
            {
                if (characters == null)
                {
                    characters = new List<CharacterNode>();
                }
                CharacterNode character = findCharacter(Context.User);
                if (character == null)
                {
                    await Context.User.SendMessageAsync("You have no active character.");
                }
                else
                {
                    character.setLevel(num);
                    await Context.User.SendMessageAsync("You have updated your Level to " + num);
                }
            }

            [Command("class")]
            [Summary("Set your class")]
            [Alias("c")]
            public async Task setClass(string className)
            {
                if (characters == null)
                {
                    characters = new List<CharacterNode>();
                }
                CharacterNode character = findCharacter(Context.User);
                if (character == null)
                {
                    await Context.User.SendMessageAsync("You have no active character.");
                }
                else
                {
                    character.setDndClass(className);
                    await Context.User.SendMessageAsync("You have updated your Class to " + className);
                }
            }

            [Command("race")]
            [Summary("Set your race")]
            [Alias("r")]
            public async Task setRace(string raceName)
            {
                if (characters == null)
                {
                    characters = new List<CharacterNode>();
                }
                CharacterNode character = findCharacter(Context.User);
                if (character == null)
                {
                    await Context.User.SendMessageAsync("You have no active character.");
                }
                else
                {
                    character.setRace(raceName);
                    await Context.User.SendMessageAsync("You have updated your Race to " + raceName);
                }
            }

            [Command("hitdice")]
            [Summary("Set the amount of hitdice")]
            [Alias("hd")]
            public async Task setHitDice(int num)
            {
                if (characters == null)
                {
                    characters = new List<CharacterNode>();
                }
                CharacterNode character = findCharacter(Context.User);
                if (character == null)
                {
                    await Context.User.SendMessageAsync("You have no active character.");
                }
                else
                {
                    character.setHitDice(num);
                    character.setHitDiceRemaining(num);
                    await Context.User.SendMessageAsync("You have updated your Hit Dice to " + num);
                }
            }
        }

        [Group("proficiency")]
        [Summary("Proficiency commands")]
        [Alias("p")]
        public class Proficiency : ModuleBase
        {

            [Group("add")]
            [Summary("Add proficiencies")]
            [Alias("a")]
            public class Add : ModuleBase
            {

                [Command("savingthrow")]
                [Summary("Add a saving throw")]
                [Alias("st")]
                public async Task addST([Remainder, Summary("stat type")] string stat)
                {
                    if (characters == null)
                    {
                        characters = new List<CharacterNode>();
                    }
                    CharacterNode old = findCharacter(Context.User);
                    if (old == null)
                    {
                        await Context.User.SendMessageAsync("You do not have an active character.");
                        return;
                    }
                    bool s = old.addSavingThrowProf(stat);
                    if (s)
                    {
                        await Context.User.SendMessageAsync("I added the proficiency for you.");
                    }
                    else
                    {
                        await Context.User.SendMessageAsync("That is not a valid proficiency.");
                    }
                }

                [Command("ability")]
                [Summary("Add an ability proficiency")]
                [Alias("a")]
                public async Task addAbility([Remainder, Summary("Ability")] string ability)
                {
                    if (characters == null)
                    {
                        characters = new List<CharacterNode>();
                    }
                    CharacterNode old = findCharacter(Context.User);
                    if (old == null)
                    {
                        await Context.User.SendMessageAsync("You do not have an active character.");
                        return;
                    }
                    bool s = old.addAbilityProficiency(ability);
                    if (s)
                    {
                        await Context.User.SendMessageAsync("I added the proficiency for you.");
                    }
                    else
                    {
                        await Context.User.SendMessageAsync("That is not a valid proficiency.");
                    }
                }

                [Command("equipment")]
                [Summary("Add an equipment proficiency")]
                [Alias("e")]
                public async Task addEquipment([Remainder, Summary("Equipment name")] string equipment)
                {
                    if (characters == null)
                    {
                        characters = new List<CharacterNode>();
                    }
                    CharacterNode old = findCharacter(Context.User);
                    if (old == null)
                    {
                        await Context.User.SendMessageAsync("You do not have an active character.");
                        return;
                    }
                    old.getWeaponArmorProf().Add(equipment);

                    await Context.User.SendMessageAsync("I added the proficiency for you.");
                }
            }

            [Group("remove")]
            [Summary("Remove proficiencies")]
            [Alias("r")]
            public class Remove : ModuleBase
            {
                [Command("savingthrow")]
                [Summary("Add a saving throw")]
                [Alias("st")]
                public async Task addST([Remainder, Summary("stat type")] string stat)
                {
                    if (characters == null)
                    {
                        characters = new List<CharacterNode>();
                    }
                    CharacterNode old = findCharacter(Context.User);
                    if (old == null)
                    {
                        await Context.User.SendMessageAsync("You do not have an active character.");
                        return;
                    }
                    bool s = old.removeSavingThrowProf(stat);
                    if (s)
                    {
                        await Context.User.SendMessageAsync("I added the proficiency for you.");
                    }
                    else
                    {
                        await Context.User.SendMessageAsync("That is not a valid proficiency.");
                    }
                }

                [Command("ability")]
                [Summary("Add an ability proficiency")]
                [Alias("a")]
                public async Task addAbility([Remainder, Summary("Ability")] string ability)
                {
                    if (characters == null)
                    {
                        characters = new List<CharacterNode>();
                    }
                    CharacterNode old = findCharacter(Context.User);
                    if (old == null)
                    {
                        await Context.User.SendMessageAsync("You do not have an active character.");
                        return;
                    }
                    bool s = old.removeAbilityProficiency(ability);
                    if (s)
                    {
                        await Context.User.SendMessageAsync("I added the proficiency for you.");
                    }
                    else
                    {
                        await Context.User.SendMessageAsync("That is not a valid proficiency.");
                    }
                }

                [Command("equipment")]
                [Summary("Add an equipment proficiency")]
                [Alias("e")]
                public async Task addEquipment([Remainder, Summary("Equipment name")] string equipment)
                {
                    if (characters == null)
                    {
                        characters = new List<CharacterNode>();
                    }
                    CharacterNode old = findCharacter(Context.User);
                    if (old == null)
                    {
                        await Context.User.SendMessageAsync("You do not have an active character.");
                        return;
                    }
                    if (old.getWeaponArmorProf().Contains(equipment))
                    {
                        old.getWeaponArmorProf().Remove(equipment);
                    }
                    await Context.User.SendMessageAsync("I added the proficiency for you.");
                }
            }
        }
    }
}