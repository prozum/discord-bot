using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using System.Net;
using System.Threading;
using CSScriptLibrary;
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

            Console.WriteLine("Email:");
            var email = Console.ReadLine();

            Console.WriteLine("Password:");
            var password = Console.ReadLine();

            bot.Client.Login(email, password);
            
            if (bot.TryJoinChannelInGuild(@"/r/jimmicirclejerk", "general"))
            {
                bot.Run();
                Console.WriteLine("Bye!");
            }
            else
            {
                Console.WriteLine("Fail to join!");
            }

        }
    }
}
