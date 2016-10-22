namespace Discord.Bot.Interfaces
{
    public interface IServerUnavailableModule : IDiscordBotModule
    {
        void ServerUnavailable(object sender, ServerEventArgs e);
    }
}
