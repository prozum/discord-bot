using System;
using System.Configuration;
using Discord.Bot.Modules;
//using UnofficialAniListApiSharp.Client;

namespace Discord.Bot.Application
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new DiscordBot("#");
            //var client = new AnilistClient(
            //    ConfigurationManager.AppSettings.Get("AniListClientID"),
            //    ConfigurationManager.AppSettings.Get("AniListClientSecret"));
            //bot.AddCommand(new AnilistCommand(client));
            //bot.AddCommand(new AnimeGuessCommand(client));

            bot.AddCommand(new HelpCommand(bot));
            bot.AddCommand(new TemplateCommand());
            bot.AddCommand(new BrainFuckCommand());
            bot.AddCommand(new CasNetCommand());
            bot.AddCommand(new BashCommand());
            bot.AddCommand(new WallpaperCommand(ConfigurationManager.AppSettings.Get("WallpaperFolder")));

            bot.Start(ConfigurationManager.AppSettings.Get("BotToken"));
            Environment.Exit(0);
        }
    }
}
