namespace Discord.Bot.Interfaces
{
    public interface IJoinedServerModule : IDiscordBotModule
    {
        void JoinedServer(object sender, ServerEventArgs e);
    }
}
