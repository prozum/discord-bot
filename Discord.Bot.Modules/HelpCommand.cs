using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Bot.Interfaces;

namespace Discord.Bot.Modules
{
    public class HelpCommand : IDiscordBotCommand
    {
        private const string _commandListFormat =
@"```
Available commands:{0}
```";
        private const string _commandHelpFormat =
@"```
#{0}:
{1}
```";

        private readonly DiscordBot _bot;

        public HelpCommand(DiscordBot bot)
        {
            _bot = bot;
        }
        
        public int? ArgumentCount => null;
        public string CommandName => "help";
        public string Help =>
@"Prints a help message.

Arguments:
    0 arguments, to print a list of all commands.
    Provide help with a command name, and help will print the a detailed help message for that command.";

        public void Execute(string[] args, Server server, Channel channel, User user, Message message)
        {
            if (args.Length == 1)
            {
                var strBuilder = new StringBuilder();

                foreach (var command in _bot.Commands)
                {
                    strBuilder.Append("\n\t");
                    strBuilder.Append(_bot.CommandPrefix);
                    strBuilder.Append(command.Value.CommandName);
                }

                channel.SendMessage(string.Format(_commandListFormat, strBuilder));
            }
            else if (args.Length == 2)
            {
                IDiscordBotCommand command;

                if (_bot.Commands.TryGetValue(args[1], out command))
                    channel.SendMessage(string.Format(_commandHelpFormat, command.CommandName, command.Help));
            }
        }
    }
}
