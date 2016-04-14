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
        Regex addcommandregex = new Regex("^\\s*\"([\\w\\d]*)\"\\s*([\\s\\S]*)");

        public CommandModule()
        {
            Commands = new Dictionary<string, string>()
            {
                { "add", "" },
                { "commands", "" }
            };
        }

        public void MessageGotten(DiscordBot bot, Message message)
        {
            var match = commandregex.Match(message.Content);

            if (!match.Success)
                return;

            var command = match.Groups[1].Value;
            var arg = match.Groups[2].Value;
            string str;

            if (Commands.TryGetValue(command, out str))
            {
                if (command == "add")
                {
                    var addmatch = addcommandregex.Match(arg);
                    if (addmatch.Success)
                    {
                        Commands[addmatch.Groups[1].Value] = addmatch.Groups[2].Value;
                        bot.SendMessage(addmatch.Groups[1].Value + " was added!");
                        bot.Backup();
                    }
                    else
                    {
                        bot.SendMessage("Error! Invalid name for command!");
                    }
                }
                else if (command == "commands")
                {
                    var matcharg = match.Groups[2].Value.Trim(' ', '\n');
                    var commandlist = "";
                    foreach (var item in Commands)
                    {
                        if (item.Key.Contains(matcharg))
                            commandlist += item.Key + "\n";
                    }

                    if (matcharg != "")
                    {
                        bot.SendMessage("All commands containing \"" + matcharg + "\":\n" + commandlist);
                    } 
                    else
                    {
                        bot.SendMessage("All commands:\n" + commandlist);
                    }
                }
                else
				{
                    str = str.Replace("##arg##", arg);
                    foreach (var module in bot.Modules)
                    {
                        if (module is ICommandable)
                        {
                            if ((module as ICommandable).TryRunCommand(str, bot))
                                return;
                        }
                    }

                    bot.SendMessage("Warning. Command !" + command + " wasn't preformed by any modules in the bot.");
                }
            }
            else
            {
                bot.SendMessage("Error! Command !" + command + " does not exist!");
            }
        }
    }
}
