using System;
using System.Collections.Generic;
using System.Linq;
using Discord.Bot.BaseModules;
using DiscordSharp;
using DiscordSharp.Events;
using DiscordSharp.Objects;
using UnofficialAniListApiSharp.Api;
using UnofficialAniListApiSharp.Api.Data;
using UnofficialAniListApiSharp.Client;

namespace Discord.Bot.Modules
{
    public class AnilistModule : BaseCommandModule
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

        private const string _random = "-random";
        private const string _anime = "-anime";
        private const string _manga = "-manga";
        private const string _char = "-char";
        private const string _staff = "-staff";
        private const string _studio = "-studio";
        
        private readonly AnilistClient _client;

        public override string CommandName => "anilist";
        public override string Help =>
@"#anlist <option>

Arguments:
    -random <anime|manga>   Get a random thing in a category.
    -anime <term>        Search for anime.
    -manga <term>        Search for manga.
    -char <term>         Search for character.
    -staff <term>        Search for staff.
    -studio <term>       Search for studio.";

        public AnilistModule(DiscordBot bot, AnilistClient client)
            : base(bot)
        {
            _client = client;
        }

        public override void CommandCalled(string[] args, DiscordMember author, DiscordChannel channel, DiscordMessage message,
            DiscordMessageType messageType)
        {
            if (args.Length != 2)
                return;

            switch (args[0])
            {
                case _random:
                    switch (args[1])
                    {
                        case "anime":
                            var anime = GetRandom<Anime>(Category.Anime);
                            SendAnime(_client.Get<AnimeBig>(Category.Anime, anime.Id), channel);
                            break;
                        case "manga":
                            var manga = GetRandom<Manga>(Category.Manga);
                            SendManga(_client.Get<MangaBig>(Category.Manga, manga.Id), channel);
                            break;
                    }

                    break;

                case _anime:
                    var animes = _client.Search<Anime>(Category.Anime, string.Join(" ", args));

                    if (animes != null && animes.Any())
                        SendAnime(_client.Get<AnimeBig>(Category.Anime, animes.First().Id), channel);

                    break;

                case _manga:
                    var mangas = _client.Search<Manga>(Category.Manga, string.Join(" ", args));

                    if (mangas != null && mangas.Any())
                        SendManga(_client.Get<MangaBig>(Category.Manga, mangas.First().Id), channel);

                    break;

                case _char:
                    var characters = _client.Search<Character>(Category.Character, string.Join(" ", args));

                    if (characters != null && characters.Any())
                        SendCharacter(_client.Get<CharacterBig>(Category.Character, characters.First().Id), channel);

                    break;

                case _staff:
                    var staffs = _client.Search<Staff>(Category.Staff, string.Join(" ", args));

                    if (staffs != null && staffs.Any())
                        SendStaff(_client.Get<StaffBig>(Category.Staff, staffs.First().Id), channel);

                    break;

                case _studio:
                    var studios = _client.Search<Studio>(Category.Studio, string.Join(" ", args));

                    if (studios != null && studios.Any())
                        SendStudio(studios.First(), channel);

                    break;
            }
        }

        private object[] FilterNulls(params object[] objs)
        {
            return objs.Select(o => o ?? "").ToArray();
        }

        private string Format(string format, params object[] objs)
        {
            var str = string.Format(format, FilterNulls(objs));
            
            if (str.Length > 1999)
                return str.Remove(1996) + "...";

            return str;
        }

        private void SendStudio(Studio studio, DiscordChannel channel)
        {
            channel.SendMessage(
                Format(_studioMsg,
                    studio.StudioName,
                    studio.StudioWiki));
        }

        private void SendStaff(StaffBig staff, DiscordChannel channel)
        {
            channel.SendMessage(
                Format(_staffMsg,
                    staff.NameFirst ?? "" + (staff.NameLast ?? ""), 
                    staff.Language, 
                    staff.ImageUrlLge, 
                    staff.Info));
        }

        private void SendCharacter(CharacterBig character, DiscordChannel channel)
        {
            channel.SendMessage(
                Format(_charMsg,
                    character.NameFirst ?? "" + (character.NameLast ?? ""), 
                    character.NameAlt, character.Role, 
                    character.ImageUrlLge, 
                    character.Info));
        }

        private void SendAnime(AnimeBig anime, DiscordChannel channel)
        {
            channel.SendMessage(
                Format(_animeMsg,
                    anime.TitleRomaji, 
                    anime.TitleEnglish, 
                    anime.Duration, 
                    anime.TotalEpisodes, 
                    anime.StartDate?.Remove(4) ?? "???",
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
                    anime.Description));
        }

        private void SendManga(MangaBig manga, DiscordChannel channel)
        {
            channel.SendMessage(
                Format(_mangaMsg,
                    manga.TitleRomaji, 
                    manga.TitleEnglish, 
                    manga.TotalChapters, 
                    manga.TotalVolumes, 
                    manga.StartDate?.Remove(4) ?? "???",
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
                    manga.Description));
        }

        private static readonly string[] _seasons = {"winter", "spring", "summer", "fall"};
        private T GetRandom<T>(Category category) where T : AnilistObject
        {
            var randomizer = new Random();
            T[] res;
            
            do
            {
                res = _client.Browse<T>(category, 
                    "year=" + randomizer.Next(2000, DateTime.Now.Year + 1), 
                    "season=" + _seasons[randomizer.Next(0, 4)], 
                    "full_page=true");
            } while (res == null || res.Length == 0);
            
            return res[randomizer.Next(0, res.Length)];
        }
    }
}
