namespace Discord.Bot.Interfaces
{
    public interface IServerAvailableModule : IDiscordBotModule
    {
        void ServerAvailable(object sender, ServerEventArgs e);
    }
}
