namespace Discord.Bot.Interfaces
{
    public interface ILeftServerModule : IDiscordBotModule
    {
        void LeftServer(object sender, ServerEventArgs e);
    }
}
