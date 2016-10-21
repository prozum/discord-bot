using System;
using System.IO;
using Discord.Bot.BaseModules;
using DiscordSharp;
using DiscordSharp.Events;
using DiscordSharp.Objects;

namespace Discord.Bot.Modules
{
    public class WallpaperModule : BaseCommandModule
    {
        private readonly string _path;
        private readonly Random _random = new Random();
        
        public WallpaperModule(DiscordBot bot, string path)
            : base(bot)
        {
            _path = path;
        }

        public override string CommandName => "#wallpaper";
        public override string Help =>
@"Picks a random wallpaper from a folder the bot has acces to.

Arguments:
    Takes no arguments.";

        public override void CommandCalled(string[] args, DiscordMember author, DiscordChannel channel, DiscordMessage message, 
            DiscordMessageType messageType)
        {
            var files = Directory.GetFiles(_path);
            Bot.Client.AttachFile(channel, "", files[_random.Next(0, files.Length)]);
        }
    }
}
