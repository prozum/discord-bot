using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.API.Model;
using System.Text.RegularExpressions;

namespace Discord.Bot.Modules
{
    public class CommandModule : IMessageModule
    {
        public Dictionary<string, string> Commands { get; set; }

        Regex commandregex = new Regex(@"^!([\w\d]*)(?:([\s\S]*)| )$");
        Regex addregex = new Regex("^!add \"([\\w\\d]*)\"([\\s\\S]*)$");

        public CommandModule()
        {
            Commands = new Dictionary<string, string>();
        }

        public void MessageGotten(DiscordBot bot, Message message)
        {
            var isadd = addregex.Match(message.content);

            if (isadd.Success)
            {
                Commands[isadd.Groups[1].Value] = isadd.Groups[2].Value;
                bot.SendMessage(bot.Channel, isadd.Groups[1].Value + " was added!");
                bot.Backup();
                return;
            }

            var match = commandregex.Match(message.content);

            if (!match.Success)
                return;

            var command = match.Groups[1].Value;
            string str;

            if (Commands.TryGetValue(command, out str))
            {
                bot.RunMessageModules(new Message() { content = "{\n" + match.Groups[2].Value + "}\n" + str });
            }
            else
            {
                bot.SendMessage(bot.Channel, "Error! Command !" + command + " does not exist!");
            }
        }
    }
}
