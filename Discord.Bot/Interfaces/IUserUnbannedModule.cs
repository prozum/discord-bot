namespace Discord.Bot.Interfaces
{
    public interface IUserUnbannedModule : IDiscordBotModule
    {
        void UserUnbanned(object sender, UserEventArgs e);
    }
}
