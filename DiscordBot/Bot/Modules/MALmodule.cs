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

        static readonly string _animeCommand = "#anime ";
        static readonly string _mangoCommand = "#mango ";
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

            if (TrySearchAndSend<AnimeEntry, AnimeSearchResponse>(message.Content, _animeCommand, anime =>
                {
                    channel.SendMessage(
                        string.Format(_animeMessage,
                            anime.Title,
                            anime.English,
                            anime.Episodes,
                            anime.Id,
                            anime.Synopsis));
                })) { }
            else if (TrySearchAndSend<MangaEntry, MangaSearchResponse>(message.Content, _mangoCommand, mango =>
                {
                    channel.SendMessage(
                        string.Format(_mangoMessage,
                            mango.Title,
                            mango.English,
                            mango.Chapters,
                            mango.Volumes,
                            mango.Id,
                            mango.Synopsis));
                })) { }
        }

        private bool TrySearchAndSend<T, U>(string message, string command, Action<T> sender) where T : EntryBase where U : class, ISearchResponse<T>
        {
            if (message.StartsWith(command))
            {
                var arg = message.Remove(0, command.Length).Trim(' ').Replace(' ', '_');
                var response = _search.SearchAnime(arg);

                if (!string.IsNullOrEmpty(response))
                {
                    var entries = new SearchResponseDeserializer<U>().Deserialize(response).Entries;

                    if (entries.Count == 1)
                    {
                        sender(entries.First());
                    }
                    else if (entries.Count > 1)
                    {
                        arg = new string(arg.ToLower().SkipWhile(chr => chr == ' ').ToArray());

                        if (!TrySend<T>(arg, entries, sender, en => en.Title, (s1, s2) => s1 != s2))
                        {
                            if (!TrySend<T>(arg, entries, sender, en => en.English, (s1, s2) => s1 != s2))
                            {
                                if (!TrySend<T>(arg, entries, sender, en => en.Synonyms, (s1, s2) => !s1.Contains(s2)))
                                {
                                    sender(entries.First());
                                }
                            }
                        }
                    }

                    return true;
                }
            }
            
            return false;
        }

        private bool TrySend<T>(string name, List<T> entries, Action<T> sender, Func<T, string> comparewith, Func<string, string, bool> comparer) where T : EntryBase
        {
            var found = entries.SkipWhile(en =>
            {
                var title = new string(comparewith(en).ToLower().SkipWhile(chr => chr == ' ').ToArray());
                return comparer(title, name);
            });

            if (found.Count() > 0)
            {
                sender(found.First());
                return true;
            }

            return false;
        }
        
    }
}
