using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Linq;

namespace JJerkBot.Modules
{
    public class WallpaperCommand : ICommand
    {
        private readonly RestClient _client = new RestClient("https://wall.alphacoders.com/api2.0/get.php");
        private readonly Random _random = new Random();
        private readonly string _auth;

        public WallpaperCommand(string auth)
        {
            _auth = auth;
        }

        public string Name => "wallpaper";
        public string Description => "Searched for a term on https://wall.alphacoders.com and returns a random result from the results.";
        public string[] Alias => null;
        public bool Hide => false;
        public Parameter[] Parameters => new []
        {
            new Parameter("term")
        };

        public bool Check(Command command, User user, Channel channel) => true;

        public async Task Do(CommandEventArgs args)
        {
            var term = args.GetArg("term");
            if (string.IsNullOrEmpty(term))
                return;

            var request = new RestRequest();
            request.AddParameter("auth", _auth);
            request.AddParameter("method", "search");
            request.AddParameter("term", term);

            var result = _client.Get<WallpaperResponse>(request);
            var wallpapers = result.Data.Wallpapers;

            if (wallpapers != null && wallpapers.Count != 0)
                await args.Channel.SendMessage(
                    $"Powered By Wallpaper Abyss: `https://wall.alphacoders.com`\n{wallpapers[_random.Next(wallpapers.Count)].UrlImage ?? "No Results"}"
                );
        }

        private class WallpaperResponse
        {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("wallpapers")]
            public List<Wallpaper> Wallpapers { get; set; }

            [JsonProperty("total_match")]
            public int TotalMatch { get; set; }
        }

        private class Wallpaper
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("width")]
            public int Width { get; set; }

            [JsonProperty("height")]
            public int Height { get; set; }

            [JsonProperty("file_type")]
            public string FileType { get; set; }

            [JsonProperty("file_size")]
            public int FileSize { get; set; }

            [JsonProperty("url_image")]
            public string UrlImage { get; set; }

            [JsonProperty("url_thumb")]
            public string UrlThumb { get; set; }

            [JsonProperty("url_page")]
            public string UrlPage { get; set; }
        }
    }
}
