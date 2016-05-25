using System;
using System.Collections.Generic;
using System.Linq;
using Discord.Bot.BaseModules;
using DiscordSharp.Events;
using DiscordSharp.Objects;
using UnofficialAniListApiSharp.Api;
using UnofficialAniListApiSharp.Api.Data;

namespace Discord.Bot.Modules
{
    public class AnilistModule : BaseMessageModule
    {
        const string _commandName = "#anilist ";

        const string _help = "-help";
        const string _random = "-random";
        const string _anime = "-anime";
        const string _manga = "-manga";
        const string _char = "-char";
        const string _staff = "-staff";
        const string _studio = "-studio";

        const string _helpMsg =
            "```" +
            "#anlist <option>\n" +
            "Options:\n" +
            "\t-help                Display available options\n" +
            "\t-random <category>   Get a random thing in a category\n" +
            "\t-anime <term>        Search for anime\n" +
            "\t-manga <term>        Search for manga\n" +
            "\t-char <term>         Search for character\n" +
            "\t-staff <term>        Search for staff\n" +
            "\t-studio <term>       Search for studio\n" +
            "```";

        const string _animeMsg =
            "Title: {0}\n" +
            "English Title: {1}\n" +
            "Duration: {2}\n" +
            "Episodes: {3}\n" +
            "Type: {4}\n" +
            "Link: https://anilist.co/anime/{5}\n" +
            "Image: {6}\n" +
            "Genres: {7}\n" +
            "Popularity: {8}\n" +
            "Score: {9}\n" +
            "{10}\n" +
            "Description:\n{11}\n";

        const string _mangaMsg =
            "Title: {0}\n" +
            "English Title: {1}\n" +
            "Chapters: {2}\n" +
            "Volumes: {3}\n" +
            "Type: {4}\n" +
            "Link: https://anilist.co/anime/{5}\n" +
            "Image: {6}\n" +
            "Genres: {7}\n" +
            "Popularity: {8}\n" +
            "Score: {9}\n" +
            "{10}\n" +
            "Description:\n{11}\n";

        const string _mangaStats =
            "Stats:\n" +
            "\tCompleted: {0}\n" +
            "\tReading: {1}\n" +
            "\tDropped: {2}\n" +
            "\tOn Hold: {3}\n" +
            "\tPlan To Watch: {4}\n";

        const string _animeStats =
            "Stats:\n" +
            "\tCompleted: {0}\n" +
            "\tWatching: {1}\n" +
            "\tDropped: {2}\n" +
            "\tOn Hold: {3}\n" +
            "\tPlan To Watch: {4}\n";

        const string _charMsg =
            "Name: {0}\n" +
            "Alt Name: {1}\n" +
            "Role: {2}\n" +
            "Image: {3}\n\n" +
            "Info:\n{4}\n";

        const string _staffMsg =
            "Name: {0}\n" +
            "Language: {1}\n" +
            "Image: {2}\n\n" +
            "Info:\n{3}\n";

        const string _studioMsg =
            "Name: {0}\n" +
            "Wiki: {1}\n";


        readonly string _clientId;
        readonly string _clientSecret;

        Authentication _auth;
        DateTime _nextUpdate;
        

        public AnilistModule(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public override void MessageReceived(object sender, DiscordMessageEventArgs e)
        {
            var channel = e.Channel;
            var message = e.MessageText;

            if (!message.StartsWith(_commandName))
                return;

            var args = new Queue<string>(message
                .Remove(0, _commandName.Length)
                .Trim(' ')
                .Split(' ')
                .SkipWhile(s => s == ""));

            if (args.Any())
            {
                switch (args.Dequeue())
                {
                    case _help:
                        channel.SendMessage(_helpMsg);
                        break;
                    case _random:
                        RandomCommand(args, channel);
                        break;
                    case _anime:
                        var anime = SearchAndGet<AnimeBig>(args, Category.Anime);

                        if (anime != null)
                            SendAnime(anime, channel);
                        break;
                    case _manga:
                        var manga = SearchAndGet<MangaBig>(args, Category.Manga);

                        if (manga != null)
                            SendManga(manga, channel);
                        break;
                    case _char:
                        var character = SearchAndGet<CharacterBig>(args, Category.Character);

                        if (character != null)
                            SendCharacter(character, channel);
                        break;
                    case _staff:
                        var staff = SearchAndGet<StaffBig>(args, Category.Staff);

                        if (staff != null)
                            SendStaff(staff, channel);
                        break;
                    case _studio:
                        var studio = SearchAndGet<Studio>(args, Category.Studio);

                        if (studio != null)
                            SendStudio(studio, channel);
                        break;
                }
            }
            else
            {
                channel.SendMessage(_helpMsg);
            }
        }

        void SendStudio(Studio studio, DiscordChannel channel)
        {
            channel.SendMessage(
                string.Format(_studioMsg,
                    studio.studio_name ?? "",
                    studio.studio_wiki ?? ""));
        }

        void SendStaff(StaffBig staff, DiscordChannel channel)
        {
            channel.SendMessage(
                string.Format(_staffMsg,
                    staff.name_first ?? "" + (staff.name_last ?? ""),
                    staff.language ?? "",
                    staff.image_url_lge ?? "",
                    staff.info ?? ""));
        }

        void SendCharacter(CharacterBig character, DiscordChannel channel)
        {
            channel.SendMessage(
                string.Format(_charMsg,
                    character.name_first ?? "" + (character.name_last ?? ""),
                    character.name_alt ?? "",
                    character.role ?? "",
                    character.image_url_lge ?? "",
                    character.info ?? ""));
        }

        void SendAnime(AnimeBig anime, DiscordChannel channel)
        {
            channel.SendMessage(
                string.Format(_animeMsg,
                    anime.title_romaji ?? "",
                    anime.title_english ?? "",
                    anime.duration,
                    anime.total_episodes,
                    anime.type ?? "",
                    anime.id,
                    anime.image_url_lge ?? "",
                    anime.genres?.Aggregate("", (s, s1) => s1) ?? "",
                    anime.popularity,
                    anime.average_score ?? "",
                    anime.list_stats != null ? 
                        string.Format(_animeStats,
                            anime.list_stats.completed,
                            anime.list_stats.watching,
                            anime.list_stats.dropped,
                            anime.list_stats.on_hold,
                            anime.list_stats.plan_to_watch) : "",
                    anime.description ?? ""));
        }

        void SendManga(MangaBig manga, DiscordChannel channel)
        {
            channel.SendMessage(
                string.Format(_mangaMsg,
                        manga.title_romaji ?? "",
                        manga.title_english ?? "",
                        manga.total_chapters,
                        manga.total_volumes,
                        manga.type ?? "",
                        manga.id,
                        manga.image_url_lge ?? "",
                        manga.genres?.Aggregate("", (s, s1) => s1) ?? "",
                        manga.popularity,
                        manga.average_score ?? "",
                        manga.list_stats != null ?
                            string.Format(_mangaStats,
                                manga.list_stats.completed,
                                manga.list_stats.reading,
                                manga.list_stats.dropped,
                                manga.list_stats.on_hold,
                                manga.list_stats.plan_to_watch) : "",
                        manga.description ?? ""));
        }

        void RandomCommand(Queue<string> args, DiscordChannel channel)
        {
            if (args.Count == 0)
                return;
            
            switch (args.Dequeue())
            {
                case "anime":
                    AnimeBig anime;

                    if (!TryGetRandom(Category.Anime, 30000, out anime))
                        return;

                    SendAnime(anime, channel);
                    break;
                case "manga":
                    MangaBig manga;

                    if (!TryGetRandom(Category.Manga, 60000, out manga))
                        return;

                    SendManga(manga, channel);
                    break;
                case "char":
                    CharacterBig character;

                    if (!TryGetRandom(Category.Character, 100000, out character))
                        return;

                    SendCharacter(character, channel);
                    break;
                case "studio":
                    Studio studio;

                    if (!TryGetRandom(Category.Studio, 3000, out studio))
                        return;

                    SendStudio(studio, channel);
                    break;
                case "staff":
                    StaffBig staff;

                    if (!TryGetRandom(Category.Staff, 40000, out staff))
                        return;

                    SendStaff(staff, channel);
                    break;
            }
        }

        bool TryGetRandom<T>(Category category, int max, out T res) where T : AnilistObject
        {
            var randomizer = new Random();
            res = null;

            do
            {
                Console.WriteLine("Trying to get Random " + category);
                if (!CheckAuth())
                    return false;

                var index = randomizer.Next(max);
                res = Anilist.GetAndDeserialize<T>(_auth, category, index);
            } while (res == null);

            return true;
        }

        T SearchAndGet<T>(Queue<string> args, Category category) where T : AnilistObject
        {
            if (!CheckAuth() || !args.Any())
                return null;

            var response = Anilist.SearchAndDeserialize<T>(_auth, category, args.Dequeue());

            if (response == null || !response.Any())
                return null;

            return Anilist.GetAndDeserialize<T>(_auth, category, response.First().id);
        }

        bool CheckAuth()
        {
            if (_auth == null || _nextUpdate < DateTime.Now)
            {
                _nextUpdate = DateTime.Now + new TimeSpan(1, 0, 0);
                _auth = Anilist.GrantClientCredentials(_clientId, _clientSecret);
            }

            return _auth != null;
        }
    }
}
