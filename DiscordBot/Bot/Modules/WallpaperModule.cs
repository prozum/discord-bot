using System;
using System.IO;
using Discord.Bot.BaseModules;
using DiscordSharp;
using DiscordSharp.Events;

namespace Discord.Bot.Modules
{
    public class WallpaperModule : BaseMessageModule
    {
        const string _commandName = "#wallpaper";
        readonly string _path;
        readonly Random _random = new Random();
        
        public WallpaperModule(string path)
        {
            _path = path;
        }

        public override void MessageReceived(object sender, DiscordMessageEventArgs e)
        {
            var client = sender as DiscordClient;
            var channel = e.Channel;
            var message = e.MessageText;

            if (message.StartsWith(_commandName))
            {
                var files = Directory.GetFiles(_path);
                client.AttachFile(channel, "", files[_random.Next(0, files.Length)]);
            }
        }
    }
}
