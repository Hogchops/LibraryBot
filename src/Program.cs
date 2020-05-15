using System;
using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryBot
{

    public class Program
    {
        private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider services;

        static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

        public async Task Start()
        {
            client = new DiscordSocketClient();
            commands = new CommandService();

            string token = "NTUwNTAwNzc0MzY3NzIzNTQw.Xr72hw.eORx0zrHoEb20reqib0gJ3CXg_k";

            services = new ServiceCollection()
                    .BuildServiceProvider();

            await InstallCommands();

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            client.Log += log;

            await Task.Delay(-1);
        }

        public async Task InstallCommands()
        {
            // Hook the MessageReceived Event into our Command Handler
            client.MessageReceived += HandleCommand;
            // Discover all of the commands in this assembly and load them.
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {

            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            // Create a Command Context
            var context = new CommandContext(client, message);
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) return;
            // Execute the command. (result does not indicate a return value,
            // rather an object stating if the command executed successfully)
            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess)
                switch (result.ErrorReason)
                {
                    default:
                        await context.Channel.SendMessageAsync(result.ErrorReason);
                        break;
                    case "The server responded with error 403: Forbidden":
                        await context.Channel.SendMessageAsync("You lack sufficient permissions to activate this command.");
                        break;
                    case "Object reference not set to an instance of an object.":
                        await context.Channel.SendMessageAsync("Make sure to use **=ng** before other commands, or **!cc**.");
                        break;
                    case "The input text has too few parameters.":
                        await context.Channel.SendMessageAsync("The command you entered was missing inputs.");
                        break;
                    case "Failed to parse Int32.":
                        await context.Channel.SendMessageAsync("Your number is too large.");
                        break;
                }
        }

        private Task log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

    }
}