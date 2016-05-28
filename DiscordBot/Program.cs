using Discord.Bot;
using Discord.Bot.Modules;
using MyAnimeListSharp.Auth;
using System;
using System.Configuration;
using UnofficialAniListApiSharp.Client;

namespace Discord
{
    public class Program
    {
        static void Main(string[] args)
        {
            var bot = new DiscordBot(ConfigurationManager.AppSettings.Get("BotToken"));
            var client = new AnilistClient(
                ConfigurationManager.AppSettings.Get("AniListClientID"),
                ConfigurationManager.AppSettings.Get("AniListClientSecret"));

            bot.AddModule(new BrainFuckModule());
            bot.AddModule(new CasNetModule());
			bot.AddModule(new BashModule());
            bot.AddModule(new TemplateModule());
            bot.AddModule(new AnilistModule(client));
            bot.AddModule(new AnimeGuessModule(client));
            bot.AddModule(new WallpaperModule(ConfigurationManager.AppSettings.Get("WallpaperFolder")));

            bot.Start();
            while (Console.ReadLine() != "stop") { }

            Environment.Exit(0);
        }
    }
}
