using Discord.API.Model;
using System;

namespace Discord.Bot.EventArguments
{
    public class MessageGottenEventArgs : EventArgs
    {
        public Message Message { get; set; }

        public MessageGottenEventArgs(Message message)
        {
            Message = message;
        }
    }
}
