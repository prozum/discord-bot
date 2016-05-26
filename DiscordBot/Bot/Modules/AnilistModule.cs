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
                var arg = args.Dequeue();

                switch (arg)
                {
                    case _help:
                        channel.SendMessage(_helpMsg);
                        break;
                    case _random:
                        RandomCommand(args, channel);
                        break;
                    case _anime:
                        var animes = Search<Anime>(string.Join(" ", args), Category.Anime);

                        if (animes != null && animes.Any())
                            SendAnime(Get<AnimeBig>(Category.Anime, animes.First().Id), channel);
                        break;
                    case _manga:
                        var mangas = Search<Manga>(string.Join(" ", args), Category.Manga);

                        if (mangas != null && mangas.Any())
                            SendManga(Get<MangaBig>(Category.Manga, mangas.First().Id), channel);
                        break;
                    case _char:
                        var characters = Search<Character>(string.Join(" ", args), Category.Character);

                        if (characters != null && characters.Any())
                            SendCharacter(Get<CharacterBig>(Category.Character, characters.First().Id), channel);
                        break;
                    case _staff:
                        var staffs = Search<Staff>(string.Join(" ", args), Category.Staff);

                        if (staffs != null && staffs.Any())
                            SendStaff(Get<StaffBig>(Category.Staff, staffs.First().Id), channel);
                        break;
                    case _studio:
                        var studios = Search<Studio>(string.Join(" ", args), Category.Studio);

                        if (studios != null && studios.Any())
                            SendStudio(studios.First(), channel);
                        break;
                }
            }
            else
            {
                channel.SendMessage(_helpMsg);
            }
        }

        int GetFitness(string str1, string str2)
        {
            str1 = string.Join("", str1.ToLower().Where(c => (c >= 'a' && c <= 'z') || char.IsDigit(c)));
            str2 = string.Join("", str2.ToLower().Where(c => (c >= 'a' && c <= 'z') || char.IsDigit(c)));
            return Enumerable.Range(0, Math.Min(str1.Length, str2.Length)).Count(i => str1[i] == str2[i]);
        }

        void GetAndSend(int id, Category category, DiscordChannel channel)
        {
            switch (category)
            {
                case Category.Anime:
                    SendAnime(Get<AnimeBig>(category, id), channel);
                    return;
                case Category.Manga:
                    SendManga(Get<MangaBig>(category, id), channel);
                    return;
                case Category.Character:
                    SendCharacter(Get<CharacterBig>(category, id), channel);
                    return;
                case Category.Staff:
                    SendStaff(Get<StaffBig>(category, id), channel);
                    return;
                case Category.Studio:
                    SendStudio(Get<Studio>(category, id), channel);
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), category, null);
            }
        }

        Category GetCategory(string str)
        {
            switch (str)
            {
                case _anime:
                    return Category.Anime;
                case _manga:
                    return Category.Manga;
                case _char:
                    return Category.Character;
                case _staff:
                    return Category.Staff;
                case _studio:
                    return Category.Studio;
                default:
                    return (Category) (-1);
            }
        }

        object[] FilterNulls(params object[] objs)
        {
            return objs.Select(o => o ?? "").ToArray();
        }

        string EnusureMaxLength(string str)
        {
            if (str.Length > 1999)
                return str.Remove(1996) + "...";

            return str;
        }

        void SendStudio(Studio studio, DiscordChannel channel)
        {
            channel.SendMessage(EnusureMaxLength(
                string.Format(
                    _studioMsg,
                    FilterNulls(
                        studio.StudioName, 
                        studio.StudioWiki))));
        }

        void SendStaff(StaffBig staff, DiscordChannel channel)
        {
            channel.SendMessage(EnusureMaxLength(
                string.Format(
                    _staffMsg,
                    FilterNulls(
                        staff.NameFirst ?? "" + (staff.NameLast ?? ""), 
                        staff.Language, 
                        staff.ImageUrlLge, 
                        staff.Info))));
        }

        void SendCharacter(CharacterBig character, DiscordChannel channel)
        {
            channel.SendMessage(EnusureMaxLength(
                string.Format(
                    _charMsg,
                    FilterNulls(
                        character.NameFirst ?? "" + (character.NameLast ?? ""), 
                        character.NameAlt, character.Role, 
                        character.ImageUrlLge, 
                        character.Info))));
        }

        void SendAnime(AnimeBig anime, DiscordChannel channel)
        {
            channel.SendMessage(EnusureMaxLength(
                string.Format(
                    _animeMsg,
                    FilterNulls(
                        anime.TitleRomaji, 
                        anime.TitleEnglish, 
                        anime.Duration, 
                        anime.TotalEpisodes, 
                        anime.Type, 
                        anime.Id, 
                        anime.ImageUrlLge, 
                        anime.Genres?.Aggregate(", ", (g, g1) => g1.ToString()) ?? "", 
                        anime.Popularity, 
                        anime.AverageScore, 
                        anime.ListStats != null ? 
                            string.Format(
                                _animeStats, 
                                anime.ListStats.Completed, 
                                anime.ListStats.Watching, 
                                anime.ListStats.Dropped, 
                                anime.ListStats.OnHold, 
                                anime.ListStats.PlanToWatch) : "", 
                        anime.Description))));
        }

        void SendManga(MangaBig manga, DiscordChannel channel)
        {
            channel.SendMessage(EnusureMaxLength(
                string.Format(
                    _mangaMsg,
                    FilterNulls(
                        manga.TitleRomaji, 
                        manga.TitleEnglish, 
                        manga.TotalChapters, 
                        manga.TotalVolumes, 
                        manga.Type, 
                        manga.Id, 
                        manga.ImageUrlLge, 
                        manga.Genres?.Aggregate(", ", (res, g1) => g1.ToString()) ?? "", 
                        manga.Popularity, 
                        manga.AverageScore, 
                        manga.ListStats != null ? 
                            string.Format(
                                _mangaStats, 
                                manga.ListStats.Completed, 
                                manga.ListStats.Reading, 
                                manga.ListStats.Dropped, 
                                manga.ListStats.OnHold, 
                                manga.ListStats.PlanToRead) : "", 
                        manga.Description))));
        }

        void RandomCommand(Queue<string> args, DiscordChannel channel)
        {
            if (args.Count == 0)
                return;

            switch (args.Dequeue())
            {
                case "anime":
                    var anime = GetRandom<Anime>(Category.Anime);
                    SendAnime(Get<AnimeBig>(Category.Anime, anime.Id), channel);
                    break;
                case "manga":
                    var manga = GetRandom<Manga>(Category.Manga);
                    SendManga(Get<MangaBig>(Category.Manga, manga.Id), channel);
                    break;
            }
        }

        static readonly string[] _seasons = {"winter", "spring", "summer", "fall"};
        T GetRandom<T>(Category category) where T : AnilistObject
        {
            var randomizer = new Random();
            T[] res;
            
            do
            {
                if (!CheckAuth())
                    return null;

                res = Anilist.BrowseAndDeserialize<T>(_auth, category, 
                    "year=" + randomizer.Next(1951, DateTime.Now.Year + 1), 
                    "season=" + _seasons[randomizer.Next(0, 4)], 
                    "full_page=true");
            } while (res == null || res.Length == 0);
            
            return res[randomizer.Next(0, res.Length)];
        }

        T[] Search<T>(string name, Category category) where T : AnilistObject
        {
            if (!CheckAuth())
                return null;

            return Anilist.SearchAndDeserialize<T>(_auth, category, name);
        }

        T Get<T>(Category category, int id) where T : AnilistObject
        {
            if (!CheckAuth())
                return null;

            return Anilist.GetAndDeserialize<T>(_auth, category, id);
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
