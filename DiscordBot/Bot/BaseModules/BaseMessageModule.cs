﻿using Discord.Bot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public virtual void URLMessageAutoUpdate(object sender, DiscordURLUpdateEventArgs e)
        { }
    }
}