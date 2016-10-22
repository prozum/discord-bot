namespace Discord.Bot.Interfaces
{
    public interface IServerUpdatedModule : IDiscordBotModule
    {
        void ServerUpdated(object sender, ServerUpdatedEventArgs e);
    }
}
