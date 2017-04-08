using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using RestSharp;
using RestSharp.Extensions;
using UnifiedAnime.Clients.Browsers.AniList;
using UnifiedAnime.Data.AniList;
using User = Discord.User;

namespace JJerkBot.Modules
{
    public class AnimeCommand : ICommand
    {
        private readonly AniListBrowser _browser;

        public AnimeCommand(string anilistClientId, string anilistClientSecret)
        {
            _browser = new AniListBrowser(anilistClientId, anilistClientSecret);
        }

        public string Name => "anime";
        public string Description => "";
        public string[] Alias => null;
        public bool Hide => false;
        public Parameter[] Parameters => new[]
        {
            new Parameter("term"),
        };

        public bool Check(Command command, User user, Channel channel) => true;

        public async Task Do(CommandEventArgs args)
        {
            var term = args.GetArg("term");
            if (string.IsNullOrEmpty(term))
                return;

            var response = _browser.SearchAnime(term);
            if (response.Item2.StatusCode == HttpStatusCode.Unauthorized)
            {
                _browser.RefreshCredentials();
                response = _browser.SearchAnime(term);
            }

            if (response.Item2.StatusCode == HttpStatusCode.OK && response.Item1?.Length > 0)
            {
                var anime = response.Item1[0];
                var animeMessage = new StringBuilder(
                    $"Title: {anime.TitleRomaji}\n" +
                    $"English Title: {anime.TitleEnglish}\n" +
                    $"Episodes: {anime.TotalEpisodes}\n" +
                    $"Type: {anime.Type}\n" +
                    $"Started Airing: {anime.StartDateFuzzy}\n" +
                    $"Finished Airing: {anime.EndDateFuzzy}\n" +
                    $"Genres: {string.Join(", ", anime.Genres)}\n" +
                    $"Score: {anime.AverageScore}\n" +
                    $"Url: https://anilist.co/anime/{anime.Id}\n");

                const int lastMessageIndex = 1999;
                if (animeMessage.Length > lastMessageIndex)
                {
                    const int removeStartIndex = lastMessageIndex - 3;
                    animeMessage.Remove(removeStartIndex, animeMessage.Length - removeStartIndex);
                    animeMessage.Append("...");
                }

                Debug.Assert(animeMessage.Length <= 2000);

                await args.Channel.SendMessage(animeMessage.ToString());
            }
        }
    }
}
