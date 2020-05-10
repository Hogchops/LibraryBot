using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using LibraryBot;

namespace LibraryBot
{
    public class GameObj
    {
        private List<UserNode> players;
        private List<MonsterNode> monsters;

        private int currentPlayer;
        private int currentMonster;
        private int turnNumber;
        private bool playerDone;
        private bool monsterDone;

        private bool collectInit;
        private bool collecting;
        private bool initReady;
        private int initiativesEntered;

        private IUser dm;
        private IMessageChannel channel;

        public GameObj(IUser dm, IMessageChannel channel)
        {
            this.dm = dm;
            this.channel = channel;
            players = new List<UserNode>();
            monsters = new List<MonsterNode>();

            currentPlayer = 0;
            currentMonster = 0;
            turnNumber = 0;
            playerDone = false;
            monsterDone = false;
            collectInit = false;
            initReady = false;
            collecting = true;
            initiativesEntered = 0;
        }

        public IUser getDm()
        {
            return dm;
        }

        public IMessageChannel getChannel()
        {
            return channel;
        }

        private int findPlayer(IUser usr)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (usr == players[i].getUser())
                {
                    return i;
                }
            }
            return -1;
        }

        public int setInitiative(IUser usr, int initiative)
        {
            if (!collectInit)
            {
                return 1;
            }
            int target = findPlayer(usr);

            if (target == -1)
            {
                return 2;
            }
            players[target].setInit(initiative);
            return 0;
        }

        public int addPlayer(IUser usr)
        {
            if (!collecting)
            {
                return 1;
            }
            int target = findPlayer(usr);
            if (target == -1)
            {
                players.Add(new UserNode(usr));
                return 0;
            }
            else
            {
                return 2;
            }
        }

        public string listPlayers()
        {
            string msg = "The current players are:\n";
            for (int i = 0; i < players.Count; i++)
            {
                msg += players[i].getUser().Username + "\n";
            }
            return msg;
        }

        public void donePlayers()
        {
            collecting = false;
        }

        public void resetPlayers()
        {
            players = new List<UserNode>();
            collecting = true;
        }

        public int getInitiatives()
        {
            if (collecting == true)
            {
                return 1;
            }
            collectInit = true;
            return 0;
        }

        public int enterInitiative(IUser usr, int initiative)
        {
            if (!collectInit)
            {
                return 1;
            }
            int target = findPlayer(usr);
            if (target == -1)
            {
                return 2;
            }
            players[target].setInit(initiative);
            initiativesEntered++;
            if (initiativesEntered == players.Count)
            {
                //Not an error, signal done
                return 3;
            }
            return 0;
        }

        public int doneInitiative()
        {
            if (collectInit == false)
            {
                return 1;
            }
            collectInit = false;
            initReady = true;
            for (int j = 0; j < players.Count; j++)
            {
                int highest = j;
                for (int i = j; i < players.Count; i++)
                {
                    if (players[i].getInit() > players[highest].getInit())
                    {
                        highest = i;
                    }
                }
                UserNode usr = players[j];
                players[j] = players[highest];
                players[highest] = usr;
            }
            return 0;
        }

        public void resetInitiatives()
        {
            initiativesEntered = 0;
            for (int i = 0; i < players.Count; i++)
            {
                players[i].setInit(0);
            }
            collectInit = false;
            currentMonster = 0;
            currentPlayer = 0;
            turnNumber = 0;
            playerDone = false;
            monsterDone = false;
        }

        public string listInitiatives()
        {
            if (initReady)
            {
                string msg = "Here is the turn order:\n";
                int i = 0;
                int j = 0;
                while (i < players.Count && j < monsters.Count)
                {
                    if (players[i].getInit() >= monsters[j].getInit())
                    {
                        msg += players[i].getUser().Mention + " has an initiative of **" + players[i].getInit() + "**.\n";
                        i++;
                    }
                    else
                    {
                        msg += monsters[j].getMonster() + " has an initiative of **" + monsters[j].getInit() + "**.\n";
                        j++;
                    }
                }
                while (i < players.Count)
                {
                    msg += players[i].getUser().Mention + " has an initiative of **" + players[i].getInit() + "**.\n";
                    i++;
                }
                while (j < monsters.Count)
                {
                    msg += monsters[j].getMonster() + " has an initiative of **" + monsters[j].getInit() + "**.\n";
                    j++;
                }
                return msg;
            }
            return "Initiatives have not been collected yet.";
        }

        public int addMonster(string monsterName, int initiative)
        {
            MonsterNode mstr = new MonsterNode(monsterName);
            mstr.setInit(initiative);
            for (int i = 0; i < monsters.Count; i++)
            {
                if (monsterName.Equals(monsters[i].getMonster()))
                {
                    return -1;
                }
            }
            monsters.Add(mstr);
            for (int j = 0; j < monsters.Count; j++)
            {
                int highest = j;
                for (int i = j; i < monsters.Count; i++)
                {
                    if (monsters[i].getInit() > monsters[highest].getInit())
                    {
                        highest = i;
                    }
                }
                MonsterNode m = monsters[j];
                monsters[j] = monsters[highest];
                monsters[highest] = m;
            }
            return 0;
        }

        public string listMonsters()
        {
            string msg = "Here are the monsters and their initiatives:\n";
            for (int i = 0; i < monsters.Count; i++)
            {
                msg += monsters[i].getMonster() + " has an initiative of **" + monsters[i].getInit() + "**.\n";
            }
            return msg;
        }

        public void resetMonsters()
        {
            monsters = new List<MonsterNode>();
            currentMonster = 0;
            currentPlayer = 0;
            turnNumber = 0;
            playerDone = false;
            monsterDone = false;
        }

        public int removeMonster(string monsterName)
        {
            int target = -1;
            for (int i = 0; i < monsters.Count; i++)
            {
                if (monsterName == monsters[i].getMonster())
                {
                    target = i;
                }
            }
            if (target == -1)
            {
                return -1;
            }
            monsters.RemoveAt(target);
            return 0;
        }

        public string nextPlayer()
        {
            string msg = "";
            if (monsters != null && monsters.Count > 0)
            {
                if (playerDone)
                {
                    //monster turn until turnCount is maxed
                    string mstr = monsters[currentMonster].getMonster();
                    msg = "Attention " + dm.Mention + "!\nIt is now " + mstr + "'s turn!";
                    currentMonster = (currentMonster + 1) % monsters.Count;
                    turnNumber = (turnNumber + 1) % (players.Count + monsters.Count);
                    if (turnNumber == 0)
                    {
                        playerDone = false;
                        monsterDone = false;
                    }
                }
                else if (monsterDone)
                {
                    // user turn
                    IUser usr = players[currentPlayer].getUser();
                    msg = "Attention " + usr.Mention + "!\nIt is now your turn!";
                    currentPlayer = (currentPlayer + 1) % players.Count;
                    turnNumber = (turnNumber + 1) % (players.Count + monsters.Count);
                    if (turnNumber == 0)
                    {
                        playerDone = false;
                        monsterDone = false;
                    }
                }
                else
                {
                    if (players[currentPlayer].getInit() >= monsters[currentMonster].getInit())
                    {
                        // user turn
                        IUser usr = players[currentPlayer].getUser();
                        msg = "Attention " + usr.Mention + "!\nIt is now your turn!";
                        currentPlayer = (currentPlayer + 1) % players.Count;
                        turnNumber = (turnNumber + 1) % (players.Count + monsters.Count);
                        if (currentPlayer == 0)
                        {
                            playerDone = true;
                        }
                        if (turnNumber == 0)
                        {
                            playerDone = false;
                            monsterDone = false;
                        }
                    }
                    else
                    {
                        //monster turn until turnCount is maxed
                        string mstr = monsters[currentMonster].getMonster();
                        msg = "Attention " + dm.Mention + "!\nIt is now " + mstr + "'s turn!";
                        currentMonster = (currentMonster + 1) % monsters.Count;
                        turnNumber = (turnNumber + 1) % (players.Count + monsters.Count);
                        if (currentMonster == 0)
                        {
                            monsterDone = true;
                        }
                        if (turnNumber == 0)
                        {
                            playerDone = false;
                            monsterDone = false;
                        }
                    }
                }
            }
            else
            {
                // user turn
                IUser usr = players[currentPlayer].getUser();
                msg = "Attention " + usr.Mention + "!\nIt is now your turn!";
                currentPlayer = (currentPlayer + 1) % players.Count;
            }
            return msg;
        }

        public void resetTurns()
        {
            currentMonster = 0;
            currentPlayer = 0;
            turnNumber = 0;
            playerDone = false;
            monsterDone = false;
        }

        public string alertPlayers()
        {
            string msg = "";
            if (collecting == true)
            {
                msg = "The player group is not done forming yet. Please enter **!p d** to stop collecting new players.";
            }
            else
            {
                for (int i = 0; i < players.Count; i++)
                {
                    msg += players[i].getUser().Mention + " ";
                }
            }
            return msg;
        }
    }
}
