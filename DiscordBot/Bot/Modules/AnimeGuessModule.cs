using Discord.Bot.BaseModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordSharp.Events;
using System.Threading;
using DiscordSharp;
using DiscordSharp.Objects;
using UnofficialAniListApiSharp.Api;
using UnofficialAniListApiSharp.Api.Data;
using UnofficialAniListApiSharp.Client;

namespace Discord.Bot.Modules
{
    public class AnimeGuessModule : BaseMessageModule
    {
        enum GameState { Idle, AwaitingPlayers, AwaitingAnime, Playing }

        const string _commandName = "#guess ";
        const string _help = "-help";
        const string _host = "-host";
        const string _start = "-start";
        const string _join = "-join";
        const string _stop = "-stop";
        const string _pass = "-pass";

        const string _helpStr =
            "#guess \"command\"\n" +
            "\t-help     Get help\n" +
            "\t-host     Host a game\n" +
            "\t-start    Start your hosted game\n" +
            "\t-join     Join a hosted game\n" +
            "\t-stop     Stop your hosted game\n" +
            "\t-pass     Pass your guess\n";

        const string _helpChooser =
            "You have been chosen to pick an anime.\n" +
            "To choose an anime first just write the name to me.\n" +
            "Then, pick correct anime from the list i provide you.\n" +
            "Example:\n" + 
            "You:\n" + 
            "Clannad\n\n" +
            "Bot:\n" + 
            "---0---\n" +
            "Title: Clannad Movie\n\n" +
            "---1---\n" +
            "Title: Clannad\n\n" +
            "You:\n" + 
            "1\n\n" +
            "Bot:\n" + 
            "You have chosen Clannad";

        const string _answer =
            "Answer:\n" +
            "\tTitle: {0}\n" +
            "\tEnglish Title: {1}\n";

        const string _lost = 
            "No one guessed the anime.\n" +
            _answer;

        const string _win =
            "{2} guessed the anime.\n" +
            _answer + "\n\n" +
            "Current Scores:\n";

        const string _suggest =
            "---{0}---\n" +
            "Title: {1}\n" +
            "English Title: {2}\n\n";

        const bool _hostCanGuess = false;
        const uint _minPlayers = 0;
        const uint _awaittime = 60;
        const uint _maxScore = 5;

        readonly Dictionary<DiscordMember, uint> _players = new Dictionary<DiscordMember, uint>();
        readonly HashSet<DiscordMember> _haspassed = new HashSet<DiscordMember>();
        readonly Random _randomizer = new Random();
        readonly AnilistClient _client;

        GameState _state = GameState.Idle;
        bool _awaiting = true;

        DiscordMember _choosingPlayer;
        DiscordMember _hostingPlayer;
        AnimeBig _chosenanime;
        string _title;
        string _engtitle;


        public AnimeGuessModule(AnilistClient client)
        {
            _client = client;
        }

        public override void MessageReceived(object sender, DiscordMessageEventArgs e)
        {
            var author = e.Author;
            var channel = e.Channel;
            var message = e.MessageText;

            if (message.StartsWith(_commandName))
            {
                var arg = e.MessageText.Remove(0, _commandName.Length).Trim(' ');

                switch (arg)
                {
                    case _join:
                        JoinGameCommand(channel, author);
                        break;
                    case _help:
                        HelpCommand(channel);
                        break;
                    case _stop:
                        StopGameCommand(channel, author);
                        break;
                    case _host:
                        HostGameCommand(channel, author);
                        break;
                    case _start:
                        StartGameCommand(author);
                        break;
                    case _pass:
                        PassCommand(author, channel);
                        break;
                }
            }
            else
            {
                GuessAnime(channel, author, message);
            }
        }

        void PassCommand(DiscordMember author, DiscordChannel channel)
        {
            if (_haspassed.Contains(author))
                return;

            var msg = string.Format(author.Username + " passes this turn, and can't guess anymore.");

            _haspassed.Add(author);

            if (_players.All(item => item.Key == _choosingPlayer || 
                             _haspassed.Contains(item.Key)))
            {
                msg += "\n\n" + string.Format(_lost, _chosenanime.TitleRomaji, _chosenanime.TitleEnglish);
                _awaiting = false;
            }

            channel.SendMessage(msg);
        }

        void GuessAnime(DiscordChannel channel, DiscordMember author, string message)
        {
            if (!_hostCanGuess)
            {
                if (author == _choosingPlayer || !_players.ContainsKey(author))
                    return;
            }
            
            // filter out big letters, space and special characters
            var strippedmsg = string.Join("", 
                message.ToLower().Where(c => (c >= 'a' && c <= 'z') || char.IsDigit(c)));

            if ((string.IsNullOrEmpty(_title) || _title != strippedmsg) &&
                (string.IsNullOrEmpty(_engtitle) || _engtitle != strippedmsg))
                return;

            _players[author]++;

            var sendmsg = string.Format(_win, _chosenanime.TitleRomaji, _chosenanime.TitleEnglish, author.Username);

            sendmsg += _players.Aggregate(sendmsg, (s, i) => i.Key.Username + ": " + i.Value + "\n");

            channel.SendMessage(sendmsg);
            _awaiting = false;
        }

        void StartGameCommand(DiscordMember author)
        {
            if (_state == GameState.AwaitingPlayers && author == _hostingPlayer)
                _state = GameState.Playing;
        }

        void HostGameCommand(DiscordChannel channel, DiscordMember author)
        {
            if (_state != GameState.Idle)
                return;

            _players.Clear();
            _choosingPlayer = null;
            _chosenanime = null;
            _players.Add(author, 0);
            _hostingPlayer = author;

            Task.Run(() =>
            {
                channel.SendMessage("Awaiting players!");
                _state = GameState.AwaitingPlayers;

                while (_state == GameState.AwaitingPlayers)
                    Thread.Sleep(1000);

                if (_players.Count >= _minPlayers)
                    GameLoop(channel);
                else
                    channel.SendMessage("Not enough players joined.");
            });
        }

        void StopGameCommand(DiscordChannel channel, DiscordMember author)
        {
            if (_state == GameState.Idle || author != _hostingPlayer)
                return;

            _state = GameState.Idle;
            _awaiting = false;
            channel.SendMessage("Game was stopped by host.");
        }

        void HelpCommand(DiscordChannel channel)
        {
            channel.SendMessage(_helpStr);
        }

        void JoinGameCommand(DiscordChannel channel, DiscordMember author)
        {
            if (_state == GameState.Idle || _players.ContainsKey(author))
                return;

            _players.Add(author, 0);
            channel.SendMessage(author.Username + " joined the game!");
        }

        Anime[] _tempEntries;
        public override void PrivateMessageReceived(object sender, DiscordPrivateMessageEventArgs e)
        {
            var member = e.Author;

            // Only accept messages from the choosing player
            if (_state != GameState.AwaitingAnime || member != _choosingPlayer)
                return;
            var message = e.Message.Trim(' ');
            int choice;

            // If bot receives a parseable number, assume they're trying to choose an anime
            if (int.TryParse(message, out choice))
            {
                // The choice was not valid, so return
                if (_tempEntries == null || choice >= _tempEntries.Length)
                    return;

                _awaiting = false;
                _chosenanime = _client.Get<AnimeBig>(Category.Anime, _tempEntries[choice].Id);

                var title = _chosenanime.TitleRomaji;
                var engtitle = _chosenanime.TitleEnglish;

                // filter out big letters, space and special characters
                _title = string.Join("", title.ToLower().Where(c => (c >= 'a' && c <= 'z') || char.IsDigit(c)));
                _engtitle = string.Join("", engtitle.ToLower().Where(c => (c >= 'a' && c <= 'z') || char.IsDigit(c)));
                member.SendMessage("You have chosen " + _chosenanime.TitleRomaji);

                return;
            }

            _tempEntries = _client.Search<Anime>(Category.Anime, message);

            if (_tempEntries != null)
            {
                var suggests = "";
                
                // Provide the chooser options of what anime he wants the others to guess
                for (var i = 0; i < _tempEntries.Length; i++)
                {
                    suggests += string.Format(_suggest, i, _tempEntries[i].TitleRomaji, _tempEntries[i].TitleEnglish);
                }

                member.SendMessage(suggests);
            }
            else
            {
                member.SendMessage("Couldn't find any anime of the name " + message);
                _tempEntries = null;
            }
        }

        void GameLoop(DiscordChannel channel)
        {
            DiscordMember winner;

            while (!IsDone(out winner))
            {
                var chosen = _randomizer.Next(_players.Count);
                _haspassed.Clear();
                _chosenanime = null;
                _choosingPlayer = _players.ElementAt(chosen).Key;

                _choosingPlayer.SendMessage(_helpChooser);
                Thread.Sleep(100);
                channel.SendMessage(_choosingPlayer.Username + " is choosing an anime.");

                _state = GameState.AwaitingAnime;
                AwaitAnime(_awaittime);

                if (_state == GameState.Idle)
                    return;

                if (_chosenanime != null)
                {
                    channel.SendMessage(
                        "Anime has been chosen.\n" +
                        "Genres: " + string.Join(", ", _chosenanime.Genres.Select(g => g.ToString())));

                    _state = GameState.Playing;
                    GuessTimer(_awaittime, channel);

                    if (_awaiting)
                        channel.SendMessage(string.Format(_lost, _chosenanime.TitleRomaji, _chosenanime.TitleEnglish, _chosenanime.Id));
                }
                else
                {
                    channel.SendMessage("Anime wasn't chosen.");
                }
            }
            
            channel.SendMessage(winner.Username + " won the game!");
            _state = GameState.Idle;
        }

        bool IsDone(out DiscordMember winner)
        {
            winner = null;

            foreach (var item in _players)
            {
                if (item.Value < _maxScore)
                    continue;

                winner = item.Key;
                return true;
            }

            return false;
        }

        void AwaitAnime(uint seconds)
        {
            const int last = 5;
            var time = seconds;
            var half = seconds / 2;

            _awaiting = true;

            while (_awaiting)
            {
                if (time == 0)
                    break;
                
                if (time == half)
                    _choosingPlayer.SendMessage(time + " seconds remaining!");

                if (time <= last)
                    _choosingPlayer.SendMessage(time.ToString());

                Thread.Sleep(1000);
                time -= 1;
            }
        }

        void GuessTimer(uint seconds, DiscordChannel channel)
        {
            const int last = 5;
            var time = seconds;
            var secondHint = time - seconds / 6;
            var thirdHint = time - seconds / 3;
            var forthHint = seconds / 2;
            var fifthHint = seconds / 4;

            _awaiting = true;

            while (_awaiting)
            {
                if (time == 0)
                    break;

                if (time == secondHint)
                {
                    channel.SendMessage($"{time} seconds remaining!\n" +
                                        $"Genres: {string.Join(", ", _chosenanime.Genres.Select(g => g.ToString()))}\n" + 
                                        $"Episodes: {_chosenanime?.TotalEpisodes?.ToString() ?? "???"}\n" +
                                        $"Type: {_chosenanime.Type}\n");
                }

                if (time == thirdHint)
                {
                    channel.SendMessage($"{time} seconds remaining!\n" +
                                        $"Genres: {string.Join(", ", _chosenanime.Genres.Select(g => g.ToString()))}\n" +
                                        $"Episodes: {_chosenanime.TotalEpisodes?.ToString() ?? "???"}\n" +
                                        $"Type: {_chosenanime.Type}\n" +
                                        $"Year: {_chosenanime.StartDate?.Remove(4) ?? "???" }");
                }

                if (time == forthHint)
                {

                    var desc = _chosenanime.Description
                        .Replace(_chosenanime.TitleRomaji, "****")
                        .Replace(_chosenanime.TitleEnglish, "****");

                    if (desc.Length >= 200)
                        desc = desc.Remove(197) + "...";

                    channel.SendMessage($"{time} seconds remaining!\n" +
                                        $"Genres: {string.Join(", ", _chosenanime.Genres.Select(g => g.ToString()))}\n" +
                                        $"Episodes: {_chosenanime.TotalEpisodes?.ToString() ?? "???"}\n" +
                                        $"Type: {_chosenanime.Type}\n" +
                                        $"Description:\n" +
                                        $"{desc}");
                }

                if (time == fifthHint)
                {

                    var desc = _chosenanime.Description
                        .Replace(_chosenanime.TitleRomaji, "****")
                        .Replace(_chosenanime.TitleEnglish, "****");

                    if (desc.Length >= 200)
                        desc = desc.Remove(197) + "...";

                    channel.SendMessage($"{time} seconds remaining!\n" +
                                        $"Genres: {string.Join(", ", _chosenanime.Genres.Select(g => g.ToString()))}\n" +
                                        $"Episodes: {_chosenanime.TotalEpisodes?.ToString() ?? "???"}\n" +
                                        $"Type: {_chosenanime.Type}\n" +
                                        $"Description:\n" +
                                        $"{desc}" +
                                        $"Image: {_chosenanime.ImageUrlLge}\n");
                }

                if (time <= last)
                    channel.SendMessage(time.ToString());

                Thread.Sleep(1000);
                time -= 1;
            }
        }
    }
}
