namespace Discord.Bot.Interfaces
{
    public interface IMessageReceivedModule : IDiscordBotModule
    {
        void MessageReceived(object sender, MessageEventArgs messageEventArgs);
    }
}