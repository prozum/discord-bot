namespace Discord.Bot.Interfaces
{
    public interface IRoleUpdatedModule : IDiscordBotModule
    {
        void RoleUpdated(object sender, RoleUpdatedEventArgs e);
    }
}
