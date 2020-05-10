using System;
using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryBot
{
    public class UserNode
    {
        private IUser usr;
        private int init;

        public UserNode(IUser usr)
        {
            this.usr = usr;
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
    }
}
