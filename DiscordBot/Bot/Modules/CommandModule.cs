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
        public Dictionary<string, Action<string, DiscordBot>> Commands { get; set; }
        Regex commandregex = new Regex(@"^([!\w\d]*)(.*)");

        public CommandModule()
        {
            Commands = new Dictionary<string, Action<string, DiscordBot>>();
        }

        public void MessageGotten(DiscordBot bot, Message message)
        {
            Action<string, DiscordBot> action;
            var match = commandregex.Match(message.content);

            if (!match.Success)
                return;

            var command = match.Groups[0].Value;
            var arg = match.Groups[1].Value;

            if (Commands.TryGetValue(command, out action))
            {
                action(arg, bot);
            }
        }
    }
}
