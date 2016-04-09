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

            if (File.Exists("bot.json"))
            {
                using (var file = File.OpenText("bot.json"))
                {
                    var settings = new JsonSerializerSettings();
                    settings.TypeNameHandling = TypeNameHandling.Auto;
                    bot = JsonConvert.DeserializeObject<DiscordBot>(file.ReadToEnd(), settings);
                }
            }
            else
            {
                bot = new DiscordBot();
                var helloworld = new CommandModule();
                bot.AddModule(helloworld);
                bot.AddModule(new BrainFuckModule());
                bot.AddModule(new CasNetModule());
            }

            var email = ConfigurationManager.AppSettings.Get("EMail");
            var password = ConfigurationManager.AppSettings.Get("Password");

            bot.Login(email, password);
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
