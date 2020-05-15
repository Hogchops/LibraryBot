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
        private int ac;
        private int hp;

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
        public int getAC() {
            return ac;
        }
        public int getHP() {
            return hp;
        }
        public void setAC(int ac) {
            this.ac = ac;
        }
        public void setHP(int hp) {
            this.hp = hp;
        }
        public int takeDamage(int damage) {
            hp -= damage;
            return hp;
        }
    }
}
