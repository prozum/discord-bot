using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Discord.Bot.Interfaces;
using UnifiedAnime.Clients;
using UnifiedAnime.Clients.Browsers.HummingBirdV1;
using UnifiedAnime.Data.Common;

namespace Discord.Bot.Modules
{
    public class AnimeCommand : IDiscordBotCommand
    {
        private const string _animeMsg =
            "Title: {0}\n" +
            "English Title: {1}\n" +
            "Duration: {2}\n" +
            "Episodes: {3}\n" +
            "Year: {4}\n" +
            "Type: {5}\n" +
            "Link: https://anilist.co/anime/{6}\n" +
            "Image: {7}\n" +
            "Genres: {8}\n" +
            "Popularity: {9}\n" +
            "Score: {10}\n" +
            "{11}\n" +
            "Description:\n{12}\n";

        private const string _mangaMsg =
            "Title: {0}\n" +
            "English Title: {1}\n" +
            "Chapters: {2}\n" +
            "Volumes: {3}\n" +
            "Year: {4}\n" +
            "Type: {5}\n" +
            "Link: https://anilist.co/anime/{6}\n" +
            "Image: {7}\n" +
            "Genres: {8}\n" +
            "Popularity: {9}\n" +
            "Score: {10}\n" +
            "{11}\n" +
            "Description:\n{12}\n";

        private const string _mangaStats =
            "Stats:\n" +
            "\tCompleted: {0}\n" +
            "\tReading: {1}\n" +
            "\tDropped: {2}\n" +
            "\tOn Hold: {3}\n" +
            "\tPlan To Watch: {4}\n";

        private const string _animeStats =
            "Stats:\n" +
            "\tCompleted: {0}\n" +
            "\tWatching: {1}\n" +
            "\tDropped: {2}\n" +
            "\tOn Hold: {3}\n" +
            "\tPlan To Watch: {4}\n";

        private const string _charMsg =
            "Name: {0}\n" +
            "Alt Name: {1}\n" +
            "Role: {2}\n" +
            "Image: {3}\n\n" +
            "Info:\n{4}\n";

        private const string _staffMsg =
            "Name: {0}\n" +
            "Language: {1}\n" +
            "Image: {2}\n\n" +
            "Info:\n{3}\n";

        private const string _studioMsg =
            "Name: {0}\n" +
            "Wiki: {1}\n";

        //private const string _user = "-user";
        private const string _anime = "-anime";
        //private const string _manga = "-manga";
        //private const string _char = "-char";
        //private const string _staff = "-staff";
        //private const string _studio = "-studio";

        private readonly HummingBirdV1Browser _browser = new HummingBirdV1Browser();

        public int? ArgumentCount => 2;
        public string CommandName => "hb";
        public string Help =>
@"#hb <option>

Arguments:
    -anime <term>        Search for anime.";

        public void Execute(string[] args, Server server, Channel channel, User user, Message message)
        {
            switch (args[1])
            {
                case _anime:
                    var result = _browser.GetSearchAnime(args[2]);

                    if (result.Status == ResponseStatus.Success && result.Data.Length > 0)
                    {
                        var anime = result.Data[0];
                        var animeMessage = new StringBuilder(
                            $"Title: {anime.Title}\n" +
                            $"English Title: {anime.AlternateTitle}\n" +
                            $"Episodes: {anime.EpisodeCount}\n" +
                            $"Episode Length: {anime.EpisodeLength}\n" +
                            $"Type: {anime.ShowType}\n" +
                            $"Status: {anime.Status}\n" +
                            $"Age Rating: {anime.AgeRating}\n" +
                            $"Started Airing: {anime.StartedAiring}\n" +
                            $"Finished Airing: {anime.FinishedAiring}\n" +
                            $"Genres: {string.Join(", ", anime.Genres.Select(i => i.Name))}\n" +
                            $"Score: {anime.CommunityRating}\n" +
                            $"Url: {anime.Url}\n" +
                            $"Synopsis: {anime.Synopsis}");

                        
                        

                        const int lastMessageIndex = 1999;
                        if (animeMessage.Length > lastMessageIndex)
                        {
                            var removeStartIndex = lastMessageIndex - 3;
                            animeMessage.Remove(removeStartIndex, animeMessage.Length - removeStartIndex);
                            animeMessage.Append("...");
                        }

                        Debug.Assert(animeMessage.Length <= 1999);

                        channel.SendMessage(animeMessage.ToString());
                    }
                    break;
            }
        }
    }
}
