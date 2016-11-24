using Discord.Bot.Interfaces;

namespace Discord.Bot.Modules
{
    public class HelloCommand : IDiscordBotCommand
    {
        public int? ArgumentCount => 0;
        public string CommandName => "hello";
        public string Help =>
@"Greets the user who used the command.

Arguments:
    Takes no arguments.";

        public void Execute(string[] args, Server server, Channel channel, User user, Message message)
        {
            channel.SendMessage("Hello " + user.NicknameMention + "!");
        }
    }
}
