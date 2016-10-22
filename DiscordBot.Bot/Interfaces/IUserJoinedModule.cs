namespace Discord.Bot.Interfaces
{
    public interface IUserJoinedModule : IDiscordBotModule
    {
        void UserJoined(object sender, UserEventArgs e);
    }
}
