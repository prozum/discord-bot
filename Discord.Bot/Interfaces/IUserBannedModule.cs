namespace Discord.Bot.Interfaces
{
    public interface IUserBannedModule : IDiscordBotModule
    {
        void UserBanned(object sender, UserEventArgs e);
    }
}
