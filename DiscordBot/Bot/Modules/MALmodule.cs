using Discord.Bot.BaseModules;
using DiscordSharp.Events;
using MyAnimeListSharp.Auth;
using MyAnimeListSharp.Core;
using MyAnimeListSharp.Facade;
using MyAnimeListSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Bot.Modules
{
    public class MALmodule : BaseMessageModule
    {
        readonly ICredentialContext _credential;
        readonly SearchMethods _search;
        readonly SearchResponseDeserializer<AnimeSearchResponse> _animeDeserializer = new SearchResponseDeserializer<AnimeSearchResponse>();
        readonly SearchResponseDeserializer<MangaSearchResponse> _mangoDeserializer = new SearchResponseDeserializer<MangaSearchResponse>();

        static readonly string _animeCommand = "#anime";
        static readonly string _mangoCommand = "#mango";
        static readonly string _animeMessage =
            "Title: {0}\n" +
            "English Title: {1}\n" +
            "Episodes: {2}\n" +
            "Link: http://myanimelist.net/anime/{3}\n" +
            "Synopsis: {4}\n";

        static readonly string _mangoMessage =
            "Title: {0}\n" +
            "English Title: {1}\n" +
            "Chapters: {2}\n" +
            "Volumes: {3}\n" +
            "Link: http://myanimelist.net/manga/{4}\n" +
            "Synopsis: {5}\n";

        public MALmodule(ICredentialContext credential)
        {
            _credential = credential;
            _search = new SearchMethods(_credential);
        }

        public override void MessageReceived(object sender, DiscordMessageEventArgs e)
        {
            var message = e.Message;
            var channel = e.Channel;
            string response;

            if (Search(message.Content, SearchType.Anime, out response))
            {
                var animes = _animeDeserializer.Deserialize(response);

                if (animes.Entries.Count != 0)
                {
                    var anime = animes.Entries.First();

                    channel.SendMessage(
                        string.Format(_animeMessage,
                            anime.Title,
                            anime.English,
                            anime.Episodes,
                            anime.Id,
                            anime.Synopsis));
                }

                return;
            }

            if (Search(message.Content, SearchType.Anime, out response))
            {
                var mangos = _mangoDeserializer.Deserialize(response);

                if (mangos.Entries.Count != 0)
                {
                    var mango = mangos.Entries.First();

                    channel.SendMessage(
                        string.Format(_mangoMessage,
                            mango.Title,
                            mango.English,
                            mango.Chapters,
                            mango.Volumes,
                            mango.Id,
                            mango.Synopsis));
                }

                return;
            }
        }

        enum SearchType { Mango, Anime }
        private bool Search(string message, SearchType type, out string response)
        {
            if (message.StartsWith(_animeCommand))
            {
                var arg = message.Remove(0, _animeCommand.Length).Trim(' ').Replace(' ', '_');
                response = _search.SearchAnime(arg);

                if (!string.IsNullOrEmpty(response))
                {
                    return true;
                }
            }

            response = null;
            return false;
        }
    }
}
