using System;
using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Text;

namespace LibraryBot
{
    public class UserNode
    {
        private IUser usr;
        private int init;
        private CharacterNode character;

        // Here are optional stats the players can use to keep track of if they want

        public UserNode(IUser usr)
        {
            this.usr = usr;
            character = null;
        }

        public void setInit(int init)
        {
            this.init = init;
        }

        public IUser getUser()
        {
            return usr;
        }

        public int getInit()
        {
            return init;
        }   

        public CharacterNode getCharacter() {
            return character;
        }

        public void createCharacter(string characterName) {
            character = new CharacterNode(characterName);
        }

        public void loadCharacter(string characterName) {
            character = new CharacterNode(characterName, usr);
        }

        public void setCharacter(CharacterNode character) {
            this.character = character;
        }
    }
}