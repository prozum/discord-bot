using Discord.Bot.BaseModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordSharp.Events;

namespace Discord.Bot.Modules
{
    public class AnimeGuessModule : BaseMessageModule
    {
        bool _gameIsOn = false;

        public override void MessageReceived(object sender, DiscordMessageEventArgs e)
        {
            base.MessageReceived(sender, e);
        }
    }
}
