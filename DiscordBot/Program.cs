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
            DiscordBot bot;
            
            /*
            if (File.Exists("bot.json"))
            {
                using (var file = File.OpenText("bot.json"))
                {
                    var settings = new JsonSerializerSettings();
                    settings.TypeNameHandling = TypeNameHandling.Auto;
                    bot = JsonConvert.DeserializeObject<DiscordBot>(file.ReadToEnd(), settings);
                }
            }
            else */
            {
                bot = new DiscordBot();
				bot.AddMessageModule(new CommandModule());
                bot.AddMessageModule(new BrainFuckModule());
                bot.AddMessageModule(new CasNetModule());
				bot.AddMessageModule(new BashModule());
            }

			bot.BotName = "bot";
			bot.Login(ConfigurationManager.AppSettings.Get("EMail"), ConfigurationManager.AppSettings.Get("Password"));
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
