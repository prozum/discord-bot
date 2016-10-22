namespace Discord.Bot.Interfaces
{
    public interface IRoleCreatedModule : IDiscordBotModule
    {
        void RoleCreated(object sender, RoleEventArgs e);
    }
}
