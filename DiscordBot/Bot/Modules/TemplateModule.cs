using Discord.Bot.BaseModules;
using DiscordSharp;
using DiscordSharp.Events;
using DiscordSharp.Objects;

namespace Discord.Bot.Modules
{
    public class TemplateModule : BaseCommandModule
    {
        public TemplateModule(DiscordBot bot) 
            : base(bot)
        { }
        public override string CommandName => "hello";
        public override string Help =>
@"Greets the user who used the command.

Arguments:
    Takes no arguments.";

        public override void CommandCalled(string[] args, DiscordMember author, DiscordChannel channel, DiscordMessage message,
            DiscordMessageType messageType)
        {
            if (args.Length != 0)
                return;

            // Send "Hello World!" to the channel where the module recieved the message
            channel.SendMessage("Hello " + author.Username + "!");
        }
    }
}
