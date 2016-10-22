namespace Discord.Bot.Interfaces
{
    public interface IUserUpdatedModule : IDiscordBotModule
    {
        void UserUpdated(object sender, UserUpdatedEventArgs e);
    }
}
