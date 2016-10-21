using Discord.Bot;
using Discord.Bot.Modules;
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

            bot.AddModule(new TemplateModule(bot));
            bot.AddModule(new BrainFuckModule(bot));
            bot.AddModule(new CasNetModule(bot));
            bot.AddModule(new BashModule(bot));
            bot.AddModule(new AnilistModule(bot, client));
            bot.AddModule(new AnimeGuessModule(bot, client));
            bot.AddModule(new WallpaperModule(bot, ConfigurationManager.AppSettings.Get("WallpaperFolder")));

            bot.Start();
            while (Console.ReadLine() != "stop") { }

            Environment.Exit(0);
        }
    }
}
