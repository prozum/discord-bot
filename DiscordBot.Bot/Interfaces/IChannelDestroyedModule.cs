namespace Discord.Bot.Interfaces
{
    public interface IChannelDestroyedModule : IDiscordBotModule
    {
        void ChannelDestroyed(object sender, ChannelEventArgs e);
    }
}
