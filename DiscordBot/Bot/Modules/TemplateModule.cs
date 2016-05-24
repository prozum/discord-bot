using Discord.Bot.BaseModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordSharp.Events;

namespace Discord.Bot.Modules
{
    public class TemplateModule : BaseMessageModule
    {
        // This is the command name that the module will be looking for
        static readonly string _commandName = "#hello ";

        public override void MessageReceived(object sender, DiscordMessageEventArgs e)
        {
            // Shortening the names of the event arguments, for more ease of use
            var message = e.Message;
            var channel = e.Channel;

            // Checks if the messages content starts with the desired command name of our module 
            if (message.Content.StartsWith(_commandName))
            {
                // Send "Hello World!" to the channel where the module recieved the message
                channel.SendMessage("Hello World!");
            }
        }
    }
}
