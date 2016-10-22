namespace Discord.Bot.Interfaces
{
    public interface IMessageUpdatedModule : IDiscordBotModule
    {
        void MessageUpdated(object sender, MessageUpdatedEventArgs e);
    }
}
