using System;

namespace Discord.Bot.Interfaces
{
    public interface IReadyModule : IDiscordBotModule
    {
        void Ready(object sender, EventArgs e);
    }
}
