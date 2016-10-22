namespace Discord.Bot.Interfaces
{
    public interface IChannelUpdatedModule : IDiscordBotModule
    {
        void ChannelUpdated(object sender, ChannelUpdatedEventArgs e);
    }
}
