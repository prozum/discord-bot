using Discord.Bot.Interfaces;
using DiscordSharp;
using DiscordSharp.Events;

namespace Discord.Bot.BaseModules
{
    public class BaseMessageModule : IMessageModule
    {
        public virtual void MentionReceived(object sender, DiscordMessageEventArgs e)
        { }

        public virtual void MessageDeleted(object sender, DiscordMessageDeletedEventArgs e)
        { }

        public virtual void MessageEdited(object sender, DiscordMessageEditedEventArgs e)
        { }

        public virtual void MessageReceived(object sender, DiscordMessageEventArgs e)
        { }

        public virtual void PrivateMessageDeleted(object sender, DiscordPrivateMessageDeletedEventArgs e)
        { }

        public virtual void PrivateMessageReceived(object sender, DiscordPrivateMessageEventArgs e)
        { }

        public virtual void UnknownMessageTypeReceived(object sender, UnknownMessageEventArgs e)
        { }

        public virtual void UrlMessageAutoUpdate(object sender, DiscordURLUpdateEventArgs e)
        { }
    }
}
