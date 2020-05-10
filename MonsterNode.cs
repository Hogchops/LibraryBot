using System;
using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryBot
{
    public class MonsterNode
    {
        private string monster;
        private int init;

        public MonsterNode(string monster)
        {
            this.monster = monster;
        }

        public void setInit(int init)
        {
            this.init = init;
        }

        public string getMonster()
        {
            return monster;
        }

        public int getInit()
        {
            return init;
        }
    }
}
