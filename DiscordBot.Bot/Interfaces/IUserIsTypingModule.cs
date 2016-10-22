namespace Discord.Bot.Interfaces
{
    public interface IUserIsTypingModule : IDiscordBotModule
    {
        void UserIsTyping(object sender, ChannelUserEventArgs e);
    }
}
