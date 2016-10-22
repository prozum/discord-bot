namespace Discord.Bot.Interfaces
{
    public interface IUserLeftModule : IDiscordBotModule
    {
        void UserLeft(object sender, UserEventArgs e);
    }
}
