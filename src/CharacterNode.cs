using System;
using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Text;
using System.Collections;

namespace LibraryBot
{
    public class CharacterNode
    {
        private string characterName;
        private int ac;
        private int hp;
        private int maxHP;
        private int str;
        private int con;
        private int dex;
        private int intel;
        private int wis;
        private int chr;
        private string dndClass;
        private string race;
        private int level;
        private int hitDice;
        private int hitDiceRemaining;
        private IUser user;
        private bool[] abilityProfs;
        private bool[] savingThrowProfs;
        private ArrayList weaponArmorProfs;
        private int[] resistances;
        private static string[] stats = { "str", "dex", "con", "int", "wis", "cha", "strength", "dexterity", "constitution", "intelligence", "wisdom", "charisma" };
        private ArrayList conditions;
        private string primaryStat;
        private bool spellCaster;
        private int hitDie;
        private ArrayList spellList;

        public CharacterNode(string characterName)
        {
            this.characterName = characterName;
            abilityProfs = new bool[18];
            savingThrowProfs = new bool[6];
            weaponArmorProfs = new ArrayList();
            resistances = new int[13];
            conditions = new ArrayList();
            str = dex = con = intel = wis = chr = 10;
            dndClass = "no class";
            race = "no race";
            primaryStat = "no stat";
            spellList = new ArrayList();
        }
        public CharacterNode(IUser user, string characterName)
        {
            this.user = user;
            this.characterName = characterName;
            abilityProfs = new bool[18];
            savingThrowProfs = new bool[6];
            weaponArmorProfs = new ArrayList();
            resistances = new int[13];
            conditions = new ArrayList();
            str = dex = con = intel = wis = chr = 10;
            dndClass = "no class";
            race = "no race";
            primaryStat = "no stat";
            spellList = new ArrayList();
        }
        public CharacterNode(string filename, IUser user)
        {
            spellList = new ArrayList();
            conditions = new ArrayList();
            openCharacter(filename, user);
        }
        public int openCharacter(string filename, IUser usr)
        {
            string pathToFile = "/Users/hog/Characters/" + filename;
            string path = Path.GetFullPath(pathToFile);
            abilityProfs = new bool[18];
            for (int i = 0; i < 18; i++)
            {
                abilityProfs[i] = false;
            }
            savingThrowProfs = new bool[6];
            for (int i = 0; i < 6; i++)
            {
                savingThrowProfs[i] = false;
            }
            weaponArmorProfs = new ArrayList();
            resistances = new int[13];
            try
            {
                string[] lines = System.IO.File.ReadAllLines(path);
                int lineNumber = 1;
                if (!usr.Id.ToString().Equals(lines[0].ToString()))
                {
                    return 2;
                }
                user = usr;
                characterName = lines[lineNumber++];
                str = Int32.Parse(lines[lineNumber++]);
                dex = Int32.Parse(lines[lineNumber++]);
                con = Int32.Parse(lines[lineNumber++]);
                intel = Int32.Parse(lines[lineNumber++]);
                wis = Int32.Parse(lines[lineNumber++]);
                chr = Int32.Parse(lines[lineNumber++]);
                ac = Int32.Parse(lines[lineNumber++]);
                maxHP = Int32.Parse(lines[lineNumber++]);
                hp = maxHP;
                dndClass = lines[lineNumber++];
                race = lines[lineNumber++];
                level = Int32.Parse(lines[lineNumber++]);
                hitDice = Int32.Parse(lines[lineNumber++]);
                hitDiceRemaining = hitDice;
                for (int i = 0; i < 18; i++)
                {
                    abilityProfs[i] = Boolean.Parse(lines[lineNumber++]);
                }
                for (int i = 0; i < 6; i++)
                {
                    savingThrowProfs[i] = Boolean.Parse(lines[lineNumber++]);
                }
                for (int i = 0; i < resistances.Length; i++)
                {
                    resistances[i] = Int32.Parse(lines[lineNumber++]);
                }
                spellCaster = Boolean.Parse(lines[lineNumber++]);
                primaryStat = lines[lineNumber++];
                hitDie = Int32.Parse(lines[lineNumber++]);
                int weaponArmorCount = Int32.Parse(lines[lineNumber++]);
                if (weaponArmorCount > 0)
                {
                    for (int i = 0; i < weaponArmorCount; i++)
                    {
                        weaponArmorProfs.Add(lines[lineNumber++]);
                    }
                }
                int spellListCount = Int32.Parse(lines[lineNumber++]);
                if(spellListCount > 0) {
                    for(int i = 0; i < spellListCount; i++) {
                        spellList.Add(lines[lineNumber++]);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                return 3;
            }
            return 0;
        }
        public int saveToFile(IUser user, string filename)
        {
            string pathToFile = "/Users/hog/Characters/" + filename;
            if (File.Exists(pathToFile))
            {
                int error = deleteCharacter(filename, user);
                if (error != 0)
                {
                    return 2;
                }
            }
            try
            {
                using (FileStream fs = File.Create(pathToFile))
                {
                    Byte[] line = new UTF8Encoding(true).GetBytes(user.Id + "\n");
                    fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes(characterName + "\n");
                    fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes(str + "\n");
                    fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes(dex + "\n");
                    fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes(con + "\n");
                    fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes(intel + "\n");
                    fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes(wis + "\n");
                    fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes(chr + "\n");
                    fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes(ac + "\n");
                    fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes(maxHP + "\n");
                    fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes(dndClass + "\n");
                    fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes(race + "\n");
                    fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes(level + "\n");
                    fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes(hitDice + "\n");
                    fs.Write(line, 0, line.Length);

                    for (int i = 0; i < abilityProfs.Length; i++)
                    {
                        line = new UTF8Encoding(true).GetBytes(abilityProfs[i] + "\n");
                        fs.Write(line, 0, line.Length);
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        line = new UTF8Encoding(true).GetBytes(savingThrowProfs[i] + "\n");
                        fs.Write(line, 0, line.Length);
                    }

                    for (int i = 0; i < resistances.Length; i++)
                    {
                        line = new UTF8Encoding(true).GetBytes(resistances[i] + "\n");
                        fs.Write(line, 0, line.Length);
                    }

                    line = new UTF8Encoding(true).GetBytes(spellCaster + "\n");
                    fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes(primaryStat + "\n");
                    fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes(hitDie + "\n");
                    fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes(weaponArmorProfs.Count + "\n");
                    fs.Write(line, 0, line.Length);

                    for (int i = 0; i < weaponArmorProfs.Count; i++)
                    {
                        line = new UTF8Encoding(true).GetBytes(weaponArmorProfs[i] + "\n");
                        fs.Write(line, 0, line.Length);
                    }

                    line = new UTF8Encoding(true).GetBytes(spellList.Count + "\n");
                    fs.Write(line, 0, line.Length);

                    for(int i = 0; i < spellList.Count; i++) {
                        line = new UTF8Encoding(true).GetBytes(spellList[i].ToString() + "\n");
                        fs.Write(line, 0, line.Length);
                    }
                }
                return 0;
            }
            catch
            {
                return 1;
            }
        }
        public int deleteCharacter(string filename, IUser usr)
        {
            string pathToFile = "/Users/hog/Characters/" + filename;
            string path = Path.GetFullPath(pathToFile);
            try
            {
                string[] lines = System.IO.File.ReadAllLines(path);
                if (!usr.Id.ToString().Equals(lines[0].ToString()))
                {
                    return 2;
                }
            }
            catch (FileNotFoundException)
            {
                return 3;
            }
            System.IO.File.Delete(path);
            return 0;
        }

        public string toString()
        {
            string msg = "```\n";
            int n = characterName.Length + 10;
            string[] stats = { "Strength", "Dexterity", "Constitution", "Intelligence", "Wisdom", "Charisma" };
            msg += String.Format("{0, -" + n + "} {1, -30} {2, -20} {3, -15} {4, -15}\n\n", characterName, "Level " + level + " " + race + " " + dndClass, hp + "/" + maxHP + " Hitpoints", "AC: " + ac, "Proficiency Bonus: " + determineProficiency());
            msg += String.Format("{0, -15} {1, -15} {2, -15} {3, -15} {4, -15} {5, -15}\n", stats[0], stats[1], stats[2], stats[3], stats[4], stats[5]);
            string[] stMod = new string[6];
            for (int i = 0; i < 6; i++)
            {
                if (determineModifier(determineStat(i)) >= 0)
                {
                    stMod[i] = "+" + determineModifier(determineStat(i));
                }
                else
                {
                    stMod[i] = "" + determineModifier(determineStat(i));
                }
            }
            msg += String.Format("{0, -15} {1, -15} {2, -15} {3, -15} {4, -15} {5, -15}\n", stMod[0] + "", stMod[1] + "", stMod[2] + "", stMod[3] + "", stMod[4] + "", stMod[5] + "");
            msg += "Hit Dice: " + hitDiceRemaining + "\n";
            msg += "AC: " + ac + "\n";
            msg += "\nSaving throw modifiers:\n";
            string[] stMods = new string[6];
            for (int i = 0; i < 6; i++)
            {
                int num = 0;
                if (savingThrowProfs[i])
                {
                    stats[i] = "+ " + stats[i];
                    num = determineModifier(determineStat(i)) + determineProficiency();
                }
                else
                {
                    num = determineModifier(determineStat(i));
                }
                if (num >= 0)
                {
                    stMods[i] = "+" + num;
                }
                else
                {
                    stMods[i] = "" + num;
                }
            }
            msg += String.Format("{0, -15} {1, -15} {2, -15} {3, -15} {4, -15} {5, -15}\n", stats[0], stats[1], stats[2], stats[3], stats[4], stats[5]);
            msg += String.Format("{0, -15} {1, -15} {2, -15} {3, -15} {4, -15} {5, -15}", stMods[0], stMods[1], stMods[2], stMods[3], stMods[4], stMods[5]);
            msg += "\n\nAbilities:\n";
            for (int i = 0; i < abilityProfs.Length; i++)
            {
                if (i == getAbility("intimidation") && dndClass.ToLower().Trim().Equals("lich"))
                {
                    int num = determineModifier(getIntel()) + determineProficiency();
                    if (num >= 0)
                    {
                        msg += String.Format("{0, -20} {1, -5}", "+ " + getAbility(i) + ":", "+" + num);
                    }
                    else
                    {
                        msg += String.Format("{0, -20} {1, -5}", "+ " + getAbility(i) + ":", num);
                    }
                }
                else
                {
                    if (abilityProfs[i])
                    {
                        int num = determineModifierOfAbility(getAbility(i)) + determineProficiency();
                        if (num >= 0)
                        {
                            msg += String.Format("{0, -20} {1, -5}", "+ " + getAbility(i) + ":", "+" + num);
                        }
                        else
                        {
                            msg += String.Format("{0, -20} {1, -5}", "+ " + getAbility(i) + ":", num);
                        }
                    }
                    else
                    {
                        int num = determineModifierOfAbility(getAbility(i));
                        if (num >= 0)
                        {
                            msg += String.Format("{0, -20} {1, -5}", getAbility(i) + ":", "+" + num);
                        }
                        else
                        {
                            msg += String.Format("{0, -20} {1, -5}", getAbility(i) + ":", num);
                        }
                    }
                }
                msg += "\n";
            }

            msg += "\nEquipment proficiencies:\n";
            for (int i = 0; i < weaponArmorProfs.Count; i++)
            {
                msg += weaponArmorProfs[i] + ", ";
            }
            msg += "\nResistances:\n";
            string[] resist = new string[7];
            msg += String.Format("{0, -15} {1, -15} {2, -15} {3, -15} {4, -15} {5, -15} {6, -15}\n", "Acid", "Bludgeoning", "Cold", "Fire", "Force", "Lightning", "Necrotic");
            for (int i = 0; i < 7; i++)
            {
                if (resistances[i] == 1)
                {
                    resist[i] = "Resistant";
                }
                else if (resistances[i] == -1)
                {
                    resist[i] = "Vulnerable";
                }
                else if (resistances[i] == 2)
                {
                    resist[i] += "Immune";
                }
            }
            msg += String.Format("{0, -15} {1, -15} {2, -15} {3, -15} {4, -15} {5, -15} {6, -15}\n", resist[0], resist[1], resist[2], resist[3], resist[4], resist[5], resist[6]);
            msg += String.Format("{0, -15} {1, -15} {2, -15} {3, -15} {4, -15} {5, -15}\n", "Piercing", "Poison", "Psychic", "Radiant", "Slashing", "Thunder");
            string[] resist2 = new string[6];
            for (int i = 7; i < 13; i++)
            {
                if (resistances[i] == 1)
                {
                    resist2[i - 7] = "Resistant";
                }
                else if (resistances[i] == -1)
                {
                    resist2[i - 7] = "Vulnerable";
                }
                else if (resistances[i] == 2)
                {
                    resist2[i - 7] = "Immune";
                }
            }
            msg += String.Format("{0, -15} {1, -15} {2, -15} {3, -15} {4, -15} {5, -15}\n", resist2[0], resist2[1], resist2[2], resist2[3], resist2[4], resist2[5]);

            if (spellCaster)
            {
                int num = 0;
                if (primaryStat == "str")
                {
                    num = determineProficiency() + determineModifier(getStr());
                }
                else if (primaryStat == "dex")
                {
                    num = determineProficiency() + determineModifier(getWis());
                }
                else if (primaryStat == "con")
                {
                    num = determineProficiency() + determineModifier(getCon());
                }
                else if (primaryStat == "int")
                {
                    num = determineProficiency() + determineModifier(getIntel());
                }
                else if (primaryStat == "wis")
                {
                    num = determineProficiency() + determineModifier(getWis());
                }
                else if (primaryStat == "cha")
                {
                    num = determineProficiency() + determineModifier(getChr());
                }
                msg += String.Format("{0, -30} {1, -30}\n", "Spell attack modifier: " + num, "Spell save DC: " + (int)(8 + num));
            }

            int number = 0;
            if (primaryStat == "str")
            {
                number = determineProficiency() + determineModifier(getStr());
            }
            else if (primaryStat == "dex")
            {
                number = determineProficiency() + determineModifier(getDex());
            }
            else if (dndClass == "monk")
            {
                number = determineProficiency() + determineModifier(getWis());
            } else {
                number = determineModifier(getStr());
            }
            msg += String.Format("{0, -30}", "Attack modifier: " + number);

            if(spellCaster) {
                msg += "\n\nSpell List:\n";
                for(int i = 0; i < spellList.Count; i++) {
                    msg += spellList[i] + ", ";
                }
            }
            
            msg += "\n\nConditions: ";
            for (int i = 0; i < conditions.Count; i++)
            {
                msg += conditions[i];
            }
            msg += "```";
            return msg;
        }
        public int rollStr(int die, int num, bool proficient)
        {
            Random rnd = new Random();
            int sum = 0;
            for (int i = 0; i < num; i++)
            {
                sum += rnd.Next(1, die + 1);
            }
            if (proficient)
            {
                return sum + str + determineProficiency();
            }
            else
            {
                return sum + str;
            }
        }
        public int rollDex(int die, int num, bool proficient)
        {
            Random rnd = new Random();
            int sum = 0;
            for (int i = 0; i < num; i++)
            {
                sum += rnd.Next(1, die + 1);
            }
            if (proficient)
            {
                return sum + dex + determineProficiency();
            }
            else
            {
                return sum + dex;
            }
        }
        public int rollCon(int die, int num, bool proficient)
        {
            Random rnd = new Random();
            int sum = 0;
            for (int i = 0; i < num; i++)
            {
                sum += rnd.Next(1, die + 1);
            }
            if (proficient)
            {
                return sum + con + determineProficiency();
            }
            else
            {
                return sum + con;
            }
        }
        public int rollIntel(int die, int num, bool proficient)
        {
            Random rnd = new Random();
            int sum = 0;
            for (int i = 0; i < num; i++)
            {
                sum += rnd.Next(1, die + 1);
            }
            if (proficient)
            {
                return sum + intel + determineProficiency();
            }
            else
            {
                return sum + intel;
            }
        }
        public int rollWis(int die, int num, bool proficient)
        {
            Random rnd = new Random();
            int sum = 0;
            for (int i = 0; i < num; i++)
            {
                sum += rnd.Next(1, die + 1);
            }
            if (proficient)
            {
                return sum + wis + determineProficiency();
            }
            else
            {
                return sum + wis;
            }
        }
        public int rollChr(int die, int num, bool proficient)
        {
            Random rnd = new Random();
            int sum = 0;
            for (int i = 0; i < num; i++)
            {
                sum += rnd.Next(1, die + 1);
            }
            if (proficient)
            {
                return sum + chr + determineProficiency();
            }
            else
            {
                return sum + chr;
            }
        }
        public string viewStats()
        {
            return "";
        }
        public int determineProficiency()
        {
            double value = level / 4.0;
            return (int)Math.Ceiling(value) + 1;
        }
        public int determineModifier(int num)
        {
            double val = (num - 10) / 2;
            return (int)Math.Floor(val);
        }
        public void setStr(int str)
        {
            this.str = str;
        }
        public void setDex(int dex)
        {
            this.dex = dex;
        }
        public void setCon(int con)
        {
            this.con = con;
        }
        public void setIntel(int intel)
        {
            this.intel = intel;
        }
        public void setWis(int wis)
        {
            this.wis = wis;
        }
        public void setChr(int chr)
        {
            this.chr = chr;
        }
        public void setAc(int ac)
        {
            this.ac = ac;
        }
        public void setHp(int hp)
        {
            this.hp = hp;
        }
        public void setMaxHP(int maxHP)
        {
            this.maxHP = maxHP;
        }
        public void setHitDice(int hitDice)
        {
            this.hitDice = hitDice;
        }
        public void setHitDiceRemaining(int hitDiceRemaining)
        {
            this.hitDiceRemaining = hitDiceRemaining;
        }
        public void setCharacterName(string characterName)
        {
            this.characterName = characterName;
        }
        public void setDndClass(string dndClass)
        {
            this.dndClass = dndClass;
            switch (dndClass.ToLower().Trim())
            {
                case "barbarian":
                    spellCaster = false;
                    primaryStat = "str";
                    savingThrowProfs[0] = true;
                    savingThrowProfs[2] = true;
                    hitDie = 12;
                    weaponArmorProfs.Add("Light Armor");
                    weaponArmorProfs.Add("Medium Armor");
                    weaponArmorProfs.Add("Shields");
                    weaponArmorProfs.Add("Simple Weapons");
                    weaponArmorProfs.Add("Martial Weapons");
                    break;
                case "bard":
                    spellCaster = true;
                    primaryStat = "cha";
                    savingThrowProfs[1] = true;
                    savingThrowProfs[5] = true;
                    hitDie = 8;
                    weaponArmorProfs.Add("Light Armor");
                    weaponArmorProfs.Add("Simple Weapons");
                    weaponArmorProfs.Add("Hand Crossbows");
                    weaponArmorProfs.Add("Longswords");
                    weaponArmorProfs.Add("Rapiers");
                    weaponArmorProfs.Add("Shortswords");
                    weaponArmorProfs.Add("Three musical instruments");
                    break;
                case "cleric":
                    spellCaster = true;
                    primaryStat = "wis";
                    savingThrowProfs[4] = true;
                    savingThrowProfs[5] = true;
                    hitDie = 8;
                    weaponArmorProfs.Add("Light Armor");
                    weaponArmorProfs.Add("Medium Armor");
                    weaponArmorProfs.Add("Shields");
                    weaponArmorProfs.Add("Simple Weapons");
                    break;
                case "druid":
                    spellCaster = true;
                    primaryStat = "wis";
                    savingThrowProfs[3] = true;
                    savingThrowProfs[4] = true;
                    hitDie = 8;
                    weaponArmorProfs.Add("Light Armor");
                    weaponArmorProfs.Add("Medium Armor");
                    weaponArmorProfs.Add("Shields");
                    weaponArmorProfs.Add("Clubs");
                    weaponArmorProfs.Add("Daggers");
                    weaponArmorProfs.Add("Javelins");
                    weaponArmorProfs.Add("Maces");
                    weaponArmorProfs.Add("Quarterstaffs");
                    weaponArmorProfs.Add("Scimitars");
                    weaponArmorProfs.Add("Sickles");
                    weaponArmorProfs.Add("Slings");
                    weaponArmorProfs.Add("Spears");
                    weaponArmorProfs.Add("Herbalism Kit");
                    break;
                case "fighter":
                    spellCaster = false;
                    primaryStat = "str";
                    savingThrowProfs[0] = true;
                    savingThrowProfs[2] = true;
                    hitDie = 10;
                    weaponArmorProfs.Add("All Armor");
                    weaponArmorProfs.Add("Shields");
                    weaponArmorProfs.Add("Simple Weapons");
                    weaponArmorProfs.Add("Martial Weapons");
                    break;
                case "monk":
                    spellCaster = false;
                    primaryStat = "dex";
                    savingThrowProfs[0] = true;
                    savingThrowProfs[4] = true;
                    hitDie = 8;
                    weaponArmorProfs.Add("Simple Weapons");
                    weaponArmorProfs.Add("Shortswords");
                    weaponArmorProfs.Add("One Musical Instrument");
                    weaponArmorProfs.Add("One type of artisan's tools");
                    break;
                case "paladin":
                    spellCaster = true;
                    primaryStat = "str";
                    savingThrowProfs[4] = true;
                    savingThrowProfs[5] = true;
                    hitDie = 10;
                    weaponArmorProfs.Add("All Armor");
                    weaponArmorProfs.Add("Shields");
                    weaponArmorProfs.Add("Simple Weapons");
                    weaponArmorProfs.Add("Martial Weapons");
                    break;
                case "ranger":
                    spellCaster = false;
                    primaryStat = "dex";
                    savingThrowProfs[0] = true;
                    savingThrowProfs[1] = true;
                    hitDie = 10;
                    weaponArmorProfs.Add("Light Armor");
                    weaponArmorProfs.Add("Medium Armor");
                    weaponArmorProfs.Add("Shields");
                    weaponArmorProfs.Add("Simple Weapons");
                    weaponArmorProfs.Add("Martial Weapons");
                    break;
                case "rogue":
                    spellCaster = false;
                    primaryStat = "dex";
                    savingThrowProfs[1] = true;
                    savingThrowProfs[3] = true;
                    hitDie = 8;
                    weaponArmorProfs.Add("Light Armor");
                    weaponArmorProfs.Add("Simple Weapons");
                    weaponArmorProfs.Add("Hand Crossbows");
                    weaponArmorProfs.Add("Longswords");
                    weaponArmorProfs.Add("Rapiers");
                    weaponArmorProfs.Add("Shortswords");
                    weaponArmorProfs.Add("Theives' Tools");
                    break;
                case "sorceror":
                    spellCaster = true;
                    primaryStat = "cha";
                    savingThrowProfs[2] = true;
                    savingThrowProfs[5] = true;
                    hitDie = 6;
                    weaponArmorProfs.Add("Daggers");
                    weaponArmorProfs.Add("Darts");
                    weaponArmorProfs.Add("Slings");
                    weaponArmorProfs.Add("Quarterstaffs");
                    weaponArmorProfs.Add("Light Crossbows");
                    break;
                case "warlock":
                    spellCaster = true;
                    primaryStat = "cha";
                    savingThrowProfs[4] = true;
                    savingThrowProfs[5] = true;
                    hitDie = 8;
                    weaponArmorProfs.Add("Light Armor");
                    weaponArmorProfs.Add("Simple Weapons");
                    break;
                case "wizard":
                    spellCaster = true;
                    primaryStat = "int";
                    savingThrowProfs[3] = true;
                    savingThrowProfs[4] = true;
                    hitDie = 6;
                    weaponArmorProfs.Add("Daggers");
                    weaponArmorProfs.Add("Darts");
                    weaponArmorProfs.Add("Slings");
                    weaponArmorProfs.Add("Quarterstaffs");
                    weaponArmorProfs.Add("Light Crossbows");
                    break;
                case "lich":
                    spellCaster = true;
                    primaryStat = "int";
                    savingThrowProfs[2] = true;
                    savingThrowProfs[3] = true;
                    hitDie = 6;
                    weaponArmorProfs.Add("None");
                    break;
                case "vampire":
                    spellCaster = false;
                    primaryStat = "str";
                    savingThrowProfs[1] = true;
                    savingThrowProfs[2] = true;
                    hitDie = 10;
                    weaponArmorProfs.Add("Simple weapons");
                    weaponArmorProfs.Add("Martial weapons");
                    weaponArmorProfs.Add("Medium armor");
                    break;
                default:
                    break;
            }
        }
        public void setRace(string race)
        {
            this.race = race;
        }
        public void setLevel(int level)
        {
            this.level = level;
        }
        public void setAbilityProf(bool[] a)
        {
            abilityProfs = a;
        }
        public void setSavingThrowProf(bool[] a)
        {
            savingThrowProfs = a;
        }
        public void setWeaponArmorProf(ArrayList a)
        {
            weaponArmorProfs = a;
        }
        public int getStr()
        {
            return str;
        }
        public int getDex()
        {
            return dex;
        }
        public int getCon()
        {
            return con;
        }
        public int getIntel()
        {
            return intel;
        }
        public int getWis()
        {
            return wis;
        }
        public int getChr()
        {
            return chr;
        }
        public int getAc()
        {
            return ac;
        }
        public int getHp()
        {
            return hp;
        }
        public int getMaxHp()
        {
            return maxHP;
        }
        public int getHitDice()
        {
            return hitDice;
        }
        public int getHitDiceRemaining()
        {
            return hitDiceRemaining;
        }
        public string getCharacterName()
        {
            return characterName;
        }
        public string getDndClass()
        {
            return dndClass;
        }
        public string getRace()
        {
            return race;
        }
        public int getLevel()
        {
            return level;
        }
        public IUser GetUser()
        {
            return user;
        }
        public bool[] getAbilityProf()
        {
            return abilityProfs;
        }
        public bool[] getSavingThrowProf()
        {
            return savingThrowProfs;
        }
        public ArrayList getWeaponArmorProf()
        {
            return weaponArmorProfs;
        }
        public bool determineValidAbility(string ability)
        {
            switch (ability.ToLower().Trim())
            {
                case "acrobatics":
                    return true;

                case "animal handling":
                    return true;

                case "arcana":
                    return true;

                case "athletics":
                    return true;

                case "deception":
                    return true;

                case "history":
                    return true;

                case "insight":
                    return true;

                case "intimidation":
                    return true;

                case "investigation":
                    return true;

                case "medicine":
                    return true;

                case "nature":
                    return true;

                case "perception":
                    return true;

                case "performance":
                    return true;

                case "persuasion":
                    return true;

                case "religion":
                    return true;

                case "sleight of hand":
                    return true;

                case "stealth":
                    return true;

                case "survival":
                    return true;

                default:
                    return false;
            }
        }
        public string determineStatOfAbility(string ability)
        {
            switch (ability.ToLower().Trim())
            {
                case "acrobatics":
                    return "dex";

                case "animal handling":
                    return "wis";

                case "arcana":
                    return "int";

                case "athletics":
                    return "str";

                case "deception":
                    return "cha";

                case "history":
                    return "int";

                case "insight":
                    return "wis";

                case "intimidation":
                    return "cha";

                case "investigation":
                    return "int";

                case "medicine":
                    return "wis";

                case "nature":
                    return "wis";

                case "perception":
                    return "wis";

                case "performance":
                    return "cha";

                case "persuasion":
                    return "cha";

                case "religion":
                    return "int";

                case "sleight of hand":
                    return "dex";

                case "stealth":
                    return "dex";

                case "survival":
                    return "wis";

                default:
                    return null;
            }
        }
        public bool addAbilityProficiency(string ability)
        {
            switch (ability.ToLower().Trim())
            {
                case "acrobatics":
                    abilityProfs[0] = true;
                    return true;

                case "animal handling":
                    abilityProfs[1] = true;
                    return true;

                case "arcana":
                    abilityProfs[2] = true;
                    return true;

                case "athletics":
                    abilityProfs[3] = true;
                    return true;

                case "deception":
                    abilityProfs[4] = true;
                    return true;

                case "history":
                    abilityProfs[5] = true;
                    return true;

                case "insight":
                    abilityProfs[6] = true;
                    return true;

                case "intimidation":
                    abilityProfs[7] = true;
                    return true;

                case "investigation":
                    abilityProfs[8] = true;
                    return true;

                case "medicine":
                    abilityProfs[9] = true;
                    return true;

                case "nature":
                    abilityProfs[10] = true;
                    return true;

                case "perception":
                    abilityProfs[11] = true;
                    return true;

                case "performance":
                    abilityProfs[12] = true;
                    return true;

                case "persuasion":
                    abilityProfs[13] = true;
                    return true;

                case "religion":
                    abilityProfs[14] = true;
                    return true;

                case "sleight of hand":
                    abilityProfs[15] = true;
                    return true;

                case "stealth":
                    abilityProfs[16] = true;
                    return true;

                case "survival":
                    abilityProfs[17] = true;
                    return true;

                default:
                    return false;
            }
        }
        public int determineModifierOfAbility(string ability)
        {
            if (determineValidAbility(ability))
            {
                string stat = determineStatOfAbility(ability);
                switch (stat)
                {
                    case "str":
                        return determineModifier(getStr());

                    case "dex":
                        return determineModifier(getWis());

                    case "con":
                        return determineModifier(getCon());

                    case "int":
                        return determineModifier(getIntel());

                    case "wis":
                        return determineModifier(getWis());

                    case "cha":
                        return determineModifier(getChr());

                    default:
                        return -100;
                }
            }
            return -100;
        }
        public bool determineValidStat(string savingthrow)
        {
            for (int i = 0; i < stats.Length; i++)
            {
                if (stats[i].Equals(savingthrow.ToLower().Trim()))
                {
                    return true;
                }
            }
            return false;
        }
        public string getAbility(int index)
        {
            switch (index)
            {
                case 0:
                    return "acrobatics";

                case 1:
                    return "animal handling";

                case 2:
                    return "arcana";

                case 3:
                    return "athletics";

                case 4:
                    return "deception";

                case 5:
                    return "history";

                case 6:
                    return "insight";

                case 7:
                    return "intimidation";

                case 8:
                    return "investigation";

                case 9:
                    return "medicine";

                case 10:
                    return "nature";

                case 11:
                    return "perception";

                case 12:
                    return "performance";

                case 13:
                    return "persuasion";

                case 14:
                    return "religion";

                case 15:
                    return "sleight of hand";

                case 16:
                    return "stealth";

                case 17:
                    return "survival";

                default:
                    return "";
            }
        }
        public bool addSavingThrowProf(string savingthrow)
        {
            switch (savingthrow.ToLower().Trim())
            {
                case "str":
                    savingThrowProfs[0] = true;
                    return true;

                case "strength":
                    savingThrowProfs[0] = true;
                    return true;

                case "dex":
                    savingThrowProfs[1] = true;
                    return true;

                case "dexterity":
                    savingThrowProfs[1] = true;
                    return true;

                case "con":
                    savingThrowProfs[2] = true;
                    return true;

                case "constitution":
                    savingThrowProfs[2] = true;
                    return true;

                case "int":
                    savingThrowProfs[3] = true;
                    return true;

                case "intelligence":
                    savingThrowProfs[3] = true;
                    return true;

                case "wis":
                    savingThrowProfs[4] = true;
                    return true;

                case "wisdom":
                    savingThrowProfs[4] = true;
                    return true;

                case "cha":
                    savingThrowProfs[5] = true;
                    return true;

                case "charisma":
                    savingThrowProfs[5] = true;
                    return true;

                default:
                    return false;
            }
        }
        public bool removeSavingThrowProf(string savingthrow)
        {
            switch (savingthrow.ToLower().Trim())
            {
                case "str":
                    savingThrowProfs[0] = false;
                    return true;

                case "strength":
                    savingThrowProfs[0] = false;
                    return true;

                case "dex":
                    savingThrowProfs[1] = false;
                    return true;

                case "dexterity":
                    savingThrowProfs[1] = false;
                    return true;

                case "con":
                    savingThrowProfs[2] = false;
                    return true;

                case "constitution":
                    savingThrowProfs[2] = false;
                    return true;

                case "int":
                    savingThrowProfs[3] = false;
                    return true;

                case "intelligence":
                    savingThrowProfs[3] = false;
                    return true;

                case "wis":
                    savingThrowProfs[4] = false;
                    return true;

                case "wisdom":
                    savingThrowProfs[4] = false;
                    return true;

                case "cha":
                    savingThrowProfs[5] = false;
                    return true;

                case "charisma":
                    savingThrowProfs[5] = false;
                    return true;

                default:
                    return false;
            }
        }
        public bool removeAbilityProficiency(string ability)
        {
            switch (ability.ToLower().Trim())
            {
                case "acrobatics":
                    abilityProfs[0] = false;
                    return true;

                case "animal handling":
                    abilityProfs[1] = false;
                    return true;

                case "arcana":
                    abilityProfs[2] = false;
                    return true;

                case "athletics":
                    abilityProfs[3] = false;
                    return true;

                case "deception":
                    abilityProfs[4] = false;
                    return true;

                case "history":
                    abilityProfs[5] = false;
                    return true;

                case "insight":
                    abilityProfs[6] = false;
                    return true;

                case "intimidation":
                    abilityProfs[7] = false;
                    return true;

                case "investigation":
                    abilityProfs[8] = false;
                    return true;

                case "medicine":
                    abilityProfs[9] = false;
                    return true;

                case "nature":
                    abilityProfs[10] = false;
                    return true;

                case "perception":
                    abilityProfs[11] = false;
                    return true;

                case "performance":
                    abilityProfs[12] = false;
                    return true;

                case "persuasion":
                    abilityProfs[13] = false;
                    return true;

                case "religion":
                    abilityProfs[14] = false;
                    return true;

                case "sleight of hand":
                    abilityProfs[15] = false;
                    return true;

                case "stealth":
                    abilityProfs[16] = false;
                    return true;

                case "survival":
                    abilityProfs[17] = false;
                    return true;

                default:
                    return false;
            }
        }
        public string damageTypes(int i)
        {
            switch (i)
            {
                case 0:
                    return "acid";
                case 1:
                    return "bludgeoning";
                case 2:
                    return "cold";
                case 3:
                    return "fire";
                case 4:
                    return "force";
                case 5:
                    return "lightning";
                case 6:
                    return "necrotic";
                case 7:
                    return "piercing";
                case 8:
                    return "poison";
                case 9:
                    return "psychic";
                case 10:
                    return "radiant";
                case 11:
                    return "slashing";
                case 12:
                    return "thunder";
                default:
                    return "";
            }
        }
        public int damageTypes(string damageType)
        {
            switch (damageType.ToLower().Trim())
            {
                case "acid":
                    return 0;
                case "bludgeoning":
                    return 1;
                case "cold":
                    return 2;
                case "fire":
                    return 3;
                case "force":
                    return 4;
                case "lightning":
                    return 5;
                case "necrotic":
                    return 6;
                case "piercing":
                    return 7;
                case "poison":
                    return 8;
                case "psychic":
                    return 9;
                case "radiant":
                    return 10;
                case "slashing":
                    return 11;
                case "thunder":
                    return 12;
                default:
                    return -1;
            }
        }
        public void addResistance(string damageType)
        {
            int index = damageTypes(damageType);
            if (index != -1)
            {
                resistances[index] = 1;
            }
        }
        public void addVulnerability(string damageType)
        {
            int index = damageTypes(damageType);
            if (index != -1)
            {
                resistances[index] = -1;
            }
        }
        public void addImmunity(string damageType)
        {
            int index = damageTypes(damageType);
            if (index != -1)
            {
                resistances[index] = 2;
            }
        }
        public void removeResistance(string damageType)
        {
            int index = damageTypes(damageType);
            if (index != -1)
            {
                if (resistances[index] == 1)
                {
                    resistances[index] = 0;
                }
            }
        }
        public void removeImmunity(string damageType)
        {
            int index = damageTypes(damageType);
            if (index != -1)
            {
                if (resistances[index] == 2)
                {
                    resistances[index] = 0;
                }
            }
        }
        public void removeVulnerability(string damageType)
        {
            int index = damageTypes(damageType);
            if (index != -1)
            {
                if (resistances[index] == -1)
                {
                    resistances[index] = 0;
                }
            }
        }
        public int getCondition(string condition)
        {
            switch (condition.ToLower().Trim())
            {
                case "blinded":
                    return 0;
                case "charmed":
                    return 1;
                case "deafened":
                    return 2;
                case "fatigued":
                    return 3;
                case "frightened":
                    return 4;
                case "grappled":
                    return 5;
                case "incapacitated":
                    return 6;
                case "invisible":
                    return 7;
                case "paralyzed":
                    return 8;
                case "petrified":
                    return 9;
                case "poisoned":
                    return 10;
                case "prone":
                    return 11;
                case "restrained":
                    return 12;
                case "stunned":
                    return 13;
                case "unconscious":
                    return 14;
                case "exhaustion":
                    return 15;
                default:
                    return -1;
            }
        }
        public string getCondition(int index)
        {
            switch (index)
            {
                case 0:
                    return "blinded";
                case 1:
                    return "charmed";
                case 2:
                    return "deafened";
                case 3:
                    return "fatigued";
                case 4:
                    return "frightened";
                case 5:
                    return "grappled";
                case 6:
                    return "incapacitated";
                case 7:
                    return "invisible";
                case 8:
                    return "paralyzed";
                case 9:
                    return "petrified";
                case 10:
                    return "poisoned";
                case 11:
                    return "prone";
                case 12:
                    return "restrained";
                case 13:
                    return "stunned";
                case 14:
                    return "unconscious";
                case 15:
                    return "exhaustion";
                default:
                    return "";
            }
        }
        public int addCondition(string condition)
        {
            int index = getCondition(condition);
            if (index != -1)
            {
                conditions.Add(condition);
                return 0;
            }
            return 1;
        }
        public int removeCondition(string condition)
        {
            int index = getCondition(condition);
            if (index != -1)
            {
                conditions.Remove(condition);
                return 0;
            }
            return 1;
        }
        public int determineStat(int num)
        {
            switch (num)
            {
                case 0:
                    return str;
                case 1:
                    return dex;
                case 2:
                    return con;
                case 3:
                    return intel;
                case 4:
                    return wis;
                case 5:
                    return chr;
                default:
                    return -1;
            }
        }
        public int getAbility(string ability)
        {
            switch (ability)
            {
                case "acrobatics":
                    return 0;

                case "animal handling":
                    return 1;

                case "arcana":
                    return 2;

                case "athletics":
                    return 3;

                case "deception":
                    return 4;

                case "history":
                    return 5;

                case "insight":
                    return 6;

                case "intimidation":
                    return 7;

                case "investigation":
                    return 8;

                case "medicine":
                    return 9;

                case "nature":
                    return 10;

                case "perception":
                    return 11;

                case "performance":
                    return 12;

                case "persuasion":
                    return 13;

                case "religion":
                    return 14;

                case "sleight of hand":
                    return 15;

                case "stealth":
                    return 16;

                case "survival":
                    return 17;

                default:
                    return -1;
            }
        }
        public void setPrimaryStat(string stat)
        {
            if (determineValidStat(stat))
            {
                primaryStat = stat;
            }
        }
        public void setSpellcaster(bool spellCaster)
        {
            this.spellCaster = spellCaster;
        }
        public void setHitDie(int die)
        {
            this.hitDie = die;
        }
        public void addSpell(string spell) {
            if(!spellList.Contains(spell)) {
                spellList.Add(spell);
            }
        }
        public void removeSpell(string spell) {
            if(spellList.Contains(spell)) {
                spellList.Remove(spell);
            }
        }
    }
}