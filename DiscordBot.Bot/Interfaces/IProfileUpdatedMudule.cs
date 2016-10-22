namespace Discord.Bot.Interfaces
{
    public interface IProfileUpdatedModule : IDiscordBotModule
    {
        void ProfileUpdated(object sender, ProfileUpdatedEventArgs e);
    }
}
