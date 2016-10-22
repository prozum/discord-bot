namespace Discord.Bot.Interfaces
{
    public interface IMessageDeletedModule : IDiscordBotModule
    {
        void MessageDeleted(object sender, MessageEventArgs e);
    }
}
