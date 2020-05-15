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
        private static string[] stats = { "str", "dex", "con", "int", "wis", "cha", "strength", "dexterity", "constitution", "intelligence", "wisdom", "charisma" };

        public CharacterNode(string characterName)
        {
            this.characterName = characterName;
            abilityProfs = new bool[18];
            savingThrowProfs = new bool[6];
            weaponArmorProfs = new ArrayList();
        }
        public CharacterNode(IUser user, string characterName)
        {
            this.user = user;
            this.characterName = characterName;
            abilityProfs = new bool[18];
            savingThrowProfs = new bool[6];
            weaponArmorProfs = new ArrayList();
        }
        public CharacterNode(string filename, IUser user)
        {
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
                int weaponArmorCount = Int32.Parse(lines[lineNumber++]);
                if (weaponArmorCount > 0)
                {
                    for (int i = 0; i < weaponArmorCount; i++)
                    {
                        weaponArmorProfs.Add(lines[lineNumber++]);
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

                    for (int i = 0; i < 18; i++)
                    {
                        line = new UTF8Encoding(true).GetBytes(abilityProfs[i] + "\n");
                        fs.Write(line, 0, line.Length);
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        line = new UTF8Encoding(true).GetBytes(savingThrowProfs[i] + "\n");
                        fs.Write(line, 0, line.Length);
                    }

                    line = new UTF8Encoding(true).GetBytes(weaponArmorProfs.Count + "\n");
                    fs.Write(line, 0, line.Length);

                    for (int i = 0; i < weaponArmorProfs.Count; i++)
                    {
                        line = new UTF8Encoding(true).GetBytes(weaponArmorProfs[i] + "\n");
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
            string msg = "**" + characterName + "**\n";
            msg += "Level " + level + " " + race + " " + dndClass + "\n";
            msg += "STR DEX CON INT WIS CHA\n";
            msg += str + " " + dex + " " + con + " " + intel + " " + wis + " " + chr + "\n";
            msg += "HP: " + hp + "\n";
            msg += "Hit Dice: " + hitDiceRemaining + "\n";
            msg += "AC: " + ac + "\n";
            msg += "\nProficiencies:\n";
            msg += "\nSaving throws:\n";
            if (savingThrowProfs[0])
            {
                int num = determineModifier(getStr()) + determineProficiency();
                msg += "**STR:** " + num;
            }
            else
            {
                msg += "STR: " + determineModifier(getStr());
            }
            msg += ", ";
            if (savingThrowProfs[1])
            {
                int num = determineModifier(getDex()) + determineProficiency();
                msg += "**DEX:** " + num;
            }
            else
            {
                msg += "DEX: " + determineModifier(getDex());
            }
            msg += ", ";
            if (savingThrowProfs[2])
            {
                int num = determineModifier(getCon()) + determineProficiency();
                msg += "**CON:** " + num;
            }
            else
            {
                msg += "CON: " + determineModifier(getCon());
            }
            msg += ", ";
            if (savingThrowProfs[3])
            {
                int num = determineModifier(getIntel()) + determineProficiency();
                msg += "**INT:** " + num;
            }
            else
            {
                msg += "INT: " + determineModifier(getIntel());
            }
            msg += ", ";
            if (savingThrowProfs[4])
            {
                int num = determineModifier(getWis()) + determineProficiency();
                msg += "**WIS:** " + num;
            }
            else
            {
                msg += "WIS: " + determineModifier(getWis());
            }
            msg += ", ";
            if (savingThrowProfs[5])
            {
                int num = determineModifier(getChr()) + determineProficiency();
                msg += "**CHA:** " + num;
            }
            else
            {
                msg += "CHA: " + determineModifier(getChr());
            }

            msg += "\n\nAbilities:\n";
            for (int i = 0; i < abilityProfs.Length; i++)
            {
                if (abilityProfs[i])
                {
                    int num = determineModifierOfAbility(getAbility(i)) + determineProficiency();
                    msg += "**" + getAbility(i) + ":** " + num;
                }
                else
                {
                    msg += getAbility(i) + ": " + determineModifierOfAbility(getAbility(i));
                }
                msg += "\n";
            }

            msg += "\nEquipment proficiencies:\n";
            for (int i = 0; i < weaponArmorProfs.Count; i++)
            {
                msg += weaponArmorProfs[i] + ", ";
            }
            msg += "\n\nProficiency bonus: " + determineProficiency();

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
        public bool removeSavingThrowProf(string savingthrow) {
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
        public bool removeAbilityProficiency(string ability) {
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
    }
}