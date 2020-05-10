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
    public class DiceCommands : ModuleBase
    {
		[Command("d4")]
		[Summary("Roll a d4")]
		public async Task D4([Remainder, Summary("Number of dice")] string num)
		{
			if (num.Equals(""))
			{
				num = "1";
			}
			int result = Int32.Parse(num);
			Random rnd = new Random();
			int sum = 0;
			string rolls = "";
			for (int i = 0; i < result; i++)
			{
				int roll = rnd.Next(1, 5);
				sum += roll;
				rolls += roll + ", ";
			}

			if (num.Equals("1"))
			{
				await Context.Channel.SendMessageAsync(Context.User.Mention + " rolled a **" + sum + "**.");
			}
			else
			{
				await Context.Channel.SendMessageAsync(rolls);
				await Context.Channel.SendMessageAsync(Context.User.Mention + " rolled a **" + sum + "**.");
			}
		}

		[Command("d4")]
		[Summary("Roll a d4")]
		public async Task D4()
		{
			Random rnd = new Random();
			int num = rnd.Next(1, 5);
			await Context.Channel.SendMessageAsync(Context.User.Mention + " rolled a **" + num + "**.");
		}

		[Command("d6")]
		[Summary("Roll a d6")]
		public async Task D6([Remainder, Summary("Number of dice")] string num)
		{
			if (num.Equals(""))
			{
				num = "1";
			}
			int result = Int32.Parse(num);
			Random rnd = new Random();
			int sum = 0;
			string rolls = "";
			for (int i = 0; i < result; i++)
			{
				int roll = rnd.Next(1, 7);
				sum += roll;
				rolls += roll + ", ";
			}

			if (num.Equals("1"))
			{
				await Context.Channel.SendMessageAsync(Context.User.Mention + " rolled a **" + sum + "**.");
			}
			else
			{
				await Context.Channel.SendMessageAsync(rolls);
				await Context.Channel.SendMessageAsync(Context.User.Mention + " rolled a **" + sum + "**.");
			}
		}

		[Command("d6")]
		[Summary("Roll a 6")]
		public async Task D6()
		{
			Random rnd = new Random();
			int num = rnd.Next(1, 7);
			await Context.Channel.SendMessageAsync(Context.User.Mention + " rolled a **" + num + "**.");
		}

		[Command("d8")]
		[Summary("Roll a d8")]
		public async Task D8([Remainder, Summary("Number of dice")] string num)
		{
			if (num.Equals(""))
			{
				num = "1";
			}
			int result = Int32.Parse(num);
			Random rnd = new Random();
			int sum = 0;
			string rolls = "";
			for (int i = 0; i < result; i++)
			{
				int roll = rnd.Next(1, 9);
				sum += roll;
				rolls += roll + ", ";
			}

			if (num.Equals("1"))
			{
				await Context.Channel.SendMessageAsync(Context.User.Mention + " rolled a **" + sum + "**.");
			}
			else
			{
				await Context.Channel.SendMessageAsync(rolls);
				await Context.Channel.SendMessageAsync(Context.User.Mention + " rolled a **" + sum + "**.");
			}
		}

		[Command("d8")]
		[Summary("Roll a 8")]
		public async Task D8()
		{
			Random rnd = new Random();
			int num = rnd.Next(1, 9);
			await Context.Channel.SendMessageAsync(Context.User.Mention + " rolled a **" + num + "**.");
		}

		[Command("d12")]
		[Summary("Roll a d12")]
		public async Task D12([Remainder, Summary("Number of dice")] string num)
		{
			if (num.Equals(""))
			{
				num = "1";
			}
			int result = Int32.Parse(num);
			Random rnd = new Random();
			int sum = 0;
			string rolls = "";
			for (int i = 0; i < result; i++)
			{
				int roll = rnd.Next(1, 13);
				sum += roll;
				rolls += roll + ", ";
			}

			if (num.Equals("1"))
			{
				await Context.Channel.SendMessageAsync(Context.User.Mention + " rolled a **" + sum + "**.");
			}
			else
			{
				await Context.Channel.SendMessageAsync(rolls);
				await Context.Channel.SendMessageAsync(Context.User.Mention + " rolled a **" + sum + "**.");
			}
		}

		[Command("d12")]
		[Summary("Roll a 12")]
		public async Task D12()
		{
			Random rnd = new Random();
			int num = rnd.Next(1, 13);
			await Context.Channel.SendMessageAsync(Context.User.Mention + " rolled a **" + num + "**.");
		}

		[Command("d20")]
		[Summary("Roll a d20")]
		public async Task D20([Remainder, Summary("Number of dice")] string num)
		{
			if (num.Equals(""))
			{
				num = "1";
			}
			int result = Int32.Parse(num);
			Random rnd = new Random();
			int sum = 0;
			string rolls = "";
			for (int i = 0; i < result; i++)
			{
				int roll = rnd.Next(1, 21);
				sum += roll;
				rolls += roll + ", ";
			}

			if (num.Equals("1"))
			{
				await Context.Channel.SendMessageAsync(Context.User.Mention + " rolled a **" + sum + "**.");
			}
			else
			{
				await Context.Channel.SendMessageAsync(rolls);
				await Context.Channel.SendMessageAsync(Context.User.Mention + " rolled a **" + sum + "**.");
			}
		}

		[Command("d20")]
		[Summary("Roll a d20")]
		public async Task D20()
		{
			Random rnd = new Random();
			int num = rnd.Next(1, 21);
			await Context.Channel.SendMessageAsync(Context.User.Mention + " rolled a **" + num + "**.");
		}

		[Command("d10")]
		[Summary("Roll a d10")]
		public async Task D10([Remainder, Summary("Number of dice")] string num)
		{
			if (num.Equals(""))
			{
				num = "1";
			}
			int result = Int32.Parse(num);
			Random rnd = new Random();
			int sum = 0;
			string rolls = "";
			for (int i = 0; i < result; i++)
			{
				int roll = rnd.Next(1, 11);
				sum += roll;
				rolls += roll + ", ";
			}

			if (num.Equals("1"))
			{
				await Context.Channel.SendMessageAsync(Context.User.Mention + " rolled a **" + sum + "**.");
			}
			else
			{
				await Context.Channel.SendMessageAsync(rolls);
				await Context.Channel.SendMessageAsync(Context.User.Mention + " rolled a **" + sum + "**.");
			}
		}

		[Command("d10")]
		[Summary("Roll a 10")]
		public async Task D10()
		{
			Random rnd = new Random();
			int num = rnd.Next(1, 11);
			await Context.Channel.SendMessageAsync(Context.User.Mention + " rolled a **" + num + "**.");
		}

		[Command("d100")]
		[Summary("Roll a d100")]
		public async Task D100([Remainder, Summary("Number of dice")] string num)
		{
			if (num.Equals(""))
			{
				num = "1";
			}
			int result = Int32.Parse(num);
			Random rnd = new Random();
			int sum = 0;
			string rolls = "";
			for (int i = 0; i < result; i++)
			{
				int roll = rnd.Next(1, 101);
				sum += roll;
				rolls += roll + ", ";
			}

			if (num.Equals("1"))
			{
				await Context.Channel.SendMessageAsync(Context.User.Mention + " rolled a **" + sum + "**.");
			}
			else
			{
				await Context.Channel.SendMessageAsync(rolls);
				await Context.Channel.SendMessageAsync(Context.User.Mention + " rolled a **" + sum + "**.");
			}
		}

		[Command("d100")]
		[Summary("Roll a 100")]
		public async Task D100()
		{
			Random rnd = new Random();
			int num = rnd.Next(1, 101);
			await Context.Channel.SendMessageAsync(Context.User.Mention + " rolled a **" + num + "**.");
		}
	}
}
