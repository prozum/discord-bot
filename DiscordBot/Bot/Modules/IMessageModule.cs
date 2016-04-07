using Discord.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Bot.Modules
{
    public interface IMessageModule : IBotModule
    {
        void MessageGotten(DiscordBot bot, Message message);
    }
}
