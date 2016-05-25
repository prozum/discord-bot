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
        readonly SearchMethods _search;

        const string _animeCommand = "#anime ";
        const string _mangoCommand = "#mango ";
        const string _animeMessage =
            "Title: {0}\n" +
            "English Title: {1}\n" +
            "Episodes: {2}\n" +
            "Link: http://myanimelist.net/anime/{3}\n" +
            "Synopsis: {4}\n";

        const string _mangoMessage =
            "Title: {0}\n" +
            "English Title: {1}\n" +
            "Chapters: {2}\n" +
            "Volumes: {3}\n" +
            "Link: http://myanimelist.net/manga/{4}\n" +
            "Synopsis: {5}\n";

        public MALmodule(ICredentialContext credential)
        {
            _search = new SearchMethods(credential);
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

        bool TrySearchAndSend<T, TU>(string message, string command, Action<T> sender) where T : EntryBase where TU : class, ISearchResponse<T>
        {
            if (!message.StartsWith(command))
                return false;

            var arg = message.Remove(0, command.Length).Trim(' ').Replace(' ', '_');
            var response = _search.SearchAnime(arg);

            if (string.IsNullOrEmpty(response))
                return false;

            var entries = new SearchResponseDeserializer<TU>().Deserialize(response).Entries;

            if (entries.Count == 1)
            {
                sender(entries.First());
            }
            else if (entries.Count > 1)
            {
                arg = arg.ToLower().Replace(" ", "");

                if (TrySend(arg, entries, sender, en => en.Title, (s1, s2) => s1 != s2))
                    return true;

                if (TrySend(arg, entries, sender, en => en.English, (s1, s2) => s1 != s2))
                    return true;

                if (!TrySend(arg, entries, sender, en => en.Synonyms, (s1, s2) => !s1.Contains(s2)))
                    sender(entries.First());
            }

            return true;
        }

        static bool TrySend<T>(string name, IEnumerable<T> entries, Action<T> sender, Func<T, string> comparewith, Func<string, string, bool> comparer) where T : EntryBase
        {
            var found = entries.SkipWhile(en => comparer(comparewith(en).ToLower().Replace(" ", ""), name));

            if (!found.Any())
                return false;

            sender(found.First());
            return true;
        }
        
    }
}
