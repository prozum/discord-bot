
using Discord.Bot;
using Discord.Bot.Modules;
using MyAnimeListSharp.Auth;
using System;
using System.Configuration;

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
            bot.AddModule(new TemplateModule());
            bot.AddModule(new MALmodule(new CredentialContext()
            {
                UserName = ConfigurationManager.AppSettings.Get("MALUserName"),
                Password = ConfigurationManager.AppSettings.Get("MALPassword")
            }));

            bot.Start();
            while (Console.ReadLine() != "stop") { }

            Environment.Exit(0);
        }
    }
}
