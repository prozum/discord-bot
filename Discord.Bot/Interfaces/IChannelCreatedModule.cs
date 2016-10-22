namespace Discord.Bot.Interfaces
{
    public interface IChannelCreatedModule : IDiscordBotModule
    {
        void ChannelCreated(object sender, ChannelEventArgs e);
    }
}
