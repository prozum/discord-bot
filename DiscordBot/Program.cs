using Discord.Bot;
using Discord.Bot.Modules;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;

namespace Discord
{
    public class Program
    {
        static void Main(string[] args)
        {
            var bot = new DiscordBot(ConfigurationManager.AppSettings.Get("BotToken"));
            
            bot.AddModule(new BrainFuckModule());
            bot.AddModule(new CasNetModule());
			bot.AddModule(new BashModule());
            
            bot.Start();
            while (Console.ReadLine() != "stop") { }

            Environment.Exit(0);
        }
    }
}
