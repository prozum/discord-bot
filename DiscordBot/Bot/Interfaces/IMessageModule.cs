using DiscordSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Bot.Interfaces
{
    public interface IMessageModule
    {
        void URLMessageAutoUpdate(object sender, DiscordURLUpdateEventArgs e);

        void UnknownMessageTypeReceived(object sender, DiscordSharp.Events.UnknownMessageEventArgs e);

        void PrivateMessageReceived(object sender, DiscordPrivateMessageEventArgs e);

        void PrivateMessageDeleted(object sender, DiscordSharp.Events.DiscordPrivateMessageDeletedEventArgs e);

        void MessageReceived(object sender, DiscordSharp.Events.DiscordMessageEventArgs e);

        void MessageEdited(object sender, DiscordSharp.Events.DiscordMessageEditedEventArgs e);

        void MessageDeleted(object sender, DiscordSharp.Events.DiscordMessageDeletedEventArgs e);

        void MentionReceived(object sender, DiscordSharp.Events.DiscordMessageEventArgs e);
    }
}
