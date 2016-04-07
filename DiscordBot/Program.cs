using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using RestSharp;
using Newtonsoft.Json;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;
using Discord.Bot;
using Discord.API.Model;
using Discord.Bot.Modules;

namespace Discord
{
    public class Program
    {
        static void Main(string[] args)
        {
            var bot = new DiscordBot();
            var helloworld = new CommandModule();
            bot.AddModule(helloworld);
            bot.AddModule(new BrainFuckModule());
            bot.AddModule(new CasNetModule());
            helloworld.Commands.Add("!helloworld", (str, bt) => { bt.Client.SendMessage(bt.Channel, "Hello World!", true); });

            var email = ConfigurationManager.AppSettings.Get("EMail");
            var password = ConfigurationManager.AppSettings.Get("Password");

            bot.Client.Login(email, password);
			if (bot.TryJoinChannelInGuild(ConfigurationManager.AppSettings.Get("Guild"), ConfigurationManager.AppSettings.Get("Channel")))
            
            {
				Console.WriteLine("Join succeded!");
                bot.Run();
                Console.WriteLine("Bye!");
            }
            else
            {
                Console.WriteLine("Join failed!");
            }

        }
    }
}
