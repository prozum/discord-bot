using System;
using System.IO;
using Discord.Bot.Interfaces;

namespace Discord.Bot.Modules
{
    public class WallpaperCommand : IDiscordBotCommand
    {
        private readonly string _path;
        private readonly Random _random = new Random();
        
        public WallpaperCommand(string path)
        {
            _path = path;
        }

        public int? ArgumentCount => 0;
        public string CommandName => "wallpaper";
        public string Help =>
@"Picks a random wallpaper from a folder the bot has acces to.

Arguments:
    Takes no arguments.";


        public void Execute(string[] args, Server server, Channel channel, User user, Message message)
        {
            var files = Directory.GetFiles(_path);
            channel.SendFile(files[_random.Next(0, files.Length)]).Wait();
        }
    }
}
