namespace Discord.Bot.Interfaces
{
    public interface IDiscordBotCommand
    {
        int? ArgumentCount { get; }
        string CommandName { get; }
        string Help { get; }

        void Execute(string[] args, Server server, Channel channel, User user, Message message);
    }
}