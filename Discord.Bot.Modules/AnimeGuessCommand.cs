//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Discord.API.Client;
//using Discord.Bot.Interfaces;
//using UnofficialAniListApiSharp.Api;
//using UnofficialAniListApiSharp.Api.Data;
//using UnofficialAniListApiSharp.Client;

//namespace Discord.Bot.Modules
//{
//    // TODO: rewrite this game
//    public class AnimeGuessCommand : IDiscordBotCommand, IMessageReceivedModule
//    {
//        enum GameState
//        {
//            Idle,
//            AwaitingPlayers,
//            AwaitingAnime,
//            Playing
//        }
        
//        private const string _host = "-host";
//        private const string _start = "-start";
//        private const string _join = "-join";
//        private const string _stop = "-stop";
//        private const string _pass = "-pass";

//        private const string _helpChooser =
//            "You have been chosen to pick an anime.\n" +
//            "To choose an anime first just write the name to me.\n" +
//            "Then, pick correct anime from the list i provide you.\n" +
//            "Example:\n" +
//            "You:\n" +
//            "Clannad\n\n" +
//            "Bot:\n" +
//            "---0---\n" +
//            "Title: Clannad Movie\n\n" +
//            "---1---\n" +
//            "Title: Clannad\n\n" +
//            "You:\n" +
//            "1\n\n" +
//            "Bot:\n" +
//            "You have chosen Clannad";

//        private const string _answer =
//            "Answer:\n" +
//            "\tTitle: {0}\n" +
//            "\tEnglish Title: {1}\n";

//        private const string _lost =
//            "No one guessed the anime.\n" +
//            _answer;

//        private const string _win =
//            "{2} guessed the anime.\n" +
//            _answer + "\n\n" +
//            "Current Scores:\n";

//        private const string _suggest =
//            "---{0}---\n" +
//            "Title: {1}\n" +
//            "English Title: {2}\n\n";

//        private const bool _hostCanGuess = false;
//        private const int _minPlayers = 0;
//        private const int _awaittime = 60;
//        private const int _maxScore = 5;

//        private readonly Dictionary<User, int> _players = new Dictionary<User, int>();
//        private readonly HashSet<User> _haspassed = new HashSet<User>();
//        private readonly Random _randomizer = new Random();
//        private readonly AnilistClient _client;

//        private GameState _state = GameState.Idle;
//        private bool _awaiting = true;

//        private User _choosingPlayer;
//        private User _hostingPlayer;
//        private AnimeBig _chosenanime;
//        private string _title;
//        private string _engtitle;

//        public AnimeGuessCommand(AnilistClient client)
//        {
//            _client = client;
//        }

//        public int? ArgumentCount => 1;
//        public string CommandName => "guess";
//        public string Help =>
//@"A anime guessing game.

//Arguments:
//    -host     Host a game.
//    -start    Start your hosted game.
//    -join     Join a hosted game.
//    -stop     Stop your hosted game.
//    -pass     Pass your guess.";

//        public string ModuleName => "Anime Guessing Game";
//        public string ModuleDescription => "This module receives channel and private messages, which it uses to play an anime guessing game";

//        public void Execute(string[] args, Server server, Channel channel, User user, Message message)
//        {
//            switch (args[0])
//            {
//                case _join:
//                    JoinGameCommand(channel, user);
//                    break;
//                case _stop:
//                    StopGameCommand(channel, user);
//                    break;
//                case _host:
//                    HostGameCommand(channel, user);
//                    break;
//                case _start:
//                    StartGameCommand(user);
//                    break;
//                case _pass:
//                    PassCommand(user, channel);
//                    break;
//            }
//        }

//        private void PassCommand(User user, Channel channel)
//        {
//            if (_haspassed.Contains(user))
//                return;

//            var msg = string.Format(user.NicknameMention + " passes this turn, and can't guess anymore.");

//            _haspassed.Add(user);

//            if (_players.All(item => item.Key == _choosingPlayer ||
//                                     _haspassed.Contains(item.Key)))
//            {
//                msg += "\n\n" + string.Format(_lost, _chosenanime.TitleRomaji, _chosenanime.TitleEnglish);
//                _awaiting = false;
//            }

//            channel.SendMessage(msg);
//        }

//        private void GuessAnime(Channel channel, User user, string message)
//        {
//            if (!_hostCanGuess)
//            {
//                if (user == _choosingPlayer || !_players.ContainsKey(user))
//                    return;
//            }

//            // filter out big letters, space and special characters
//            var strippedmsg = string.Join("",
//                message.ToLower().Where(c => (c >= 'a' && c <= 'z') || char.IsDigit(c)));

//            if ((string.IsNullOrEmpty(_title) || _title != strippedmsg) &&
//                (string.IsNullOrEmpty(_engtitle) || _engtitle != strippedmsg))
//                return;

//            _players[user]++;

//            var sendmsg = string.Format(_win, _chosenanime.TitleRomaji, _chosenanime.TitleEnglish, user.NicknameMention);

//            sendmsg += _players.Aggregate(sendmsg, (s, i) => i.Key.NicknameMention + ": " + i.Value + "\n");

//            channel.SendMessage(sendmsg);
//            _awaiting = false;
//        }

//        private void StartGameCommand(User user)
//        {
//            if (_state == GameState.AwaitingPlayers && user == _hostingPlayer)
//                _state = GameState.Playing;
//        }

//        private void HostGameCommand(Channel channel, User user)
//        {
//            if (_state != GameState.Idle)
//                return;

//            _players.Clear();
//            _choosingPlayer = null;
//            _chosenanime = null;
//            _players.Add(user, 0);
//            _hostingPlayer = user;

//            Task.Run(() =>
//            {
//                channel.SendMessage("Awaiting players!");
//                _state = GameState.AwaitingPlayers;

//                while (_state == GameState.AwaitingPlayers)
//                    Thread.Sleep(1000);

//                if (_players.Count >= _minPlayers)
//                    GameLoop(channel);
//                else
//                    channel.SendMessage("Not enough players joined.");
//            });
//        }

//        private void StopGameCommand(Channel channel, User user)
//        {
//            if (_state == GameState.Idle || user != _hostingPlayer)
//                return;

//            _state = GameState.Idle;
//            _awaiting = false;
//            channel.SendMessage("Game was stopped by host.");
//        }

//        private void JoinGameCommand(Channel channel, User user)
//        {
//            if (_state == GameState.Idle || _players.ContainsKey(user))
//                return;

//            _players.Add(user, 0);
//            channel.SendMessage(user.NicknameMention + " joined the game!");
//        }

//        private void GameLoop(Channel channel)
//        {
//            User winner;

//            while (!IsDone(out winner))
//            {
//                var chosen = _randomizer.Next(_players.Count);
//                _haspassed.Clear();
//                _chosenanime = null;
//                _choosingPlayer = _players.ElementAt(chosen).Key;

//                _choosingPlayer.SendMessage(_helpChooser);
//                Thread.Sleep(100);
//                channel.SendMessage(_choosingPlayer.NicknameMention + " is choosing an anime.");

//                _state = GameState.AwaitingAnime;
//                AwaitAnime(_awaittime);

//                if (_state == GameState.Idle)
//                    return;

//                if (_chosenanime != null)
//                {
//                    channel.SendMessage(
//                        "Anime has been chosen.\n" +
//                        "Genres: " + string.Join(", ", _chosenanime.Genres.Select(g => g.ToString())));

//                    _state = GameState.Playing;
//                    GuessTimer(_awaittime, channel);

//                    if (_awaiting)
//                        channel.SendMessage(string.Format(_lost, _chosenanime.TitleRomaji, _chosenanime.TitleEnglish));
//                }
//                else
//                {
//                    channel.SendMessage("Anime wasn't chosen.");
//                }
//            }

//            channel.SendMessage(winner.NicknameMention + " won the game!");
//            _state = GameState.Idle;
//        }

//        private bool IsDone(out User winner)
//        {
//            winner = null;

//            foreach (var item in _players)
//            {
//                if (item.Value < _maxScore)
//                    continue;

//                winner = item.Key;
//                return true;
//            }

//            return false;
//        }

//        private void AwaitAnime(int seconds)
//        {
//            const int last = 5;
//            var time = seconds;
//            var half = seconds/2;

//            _awaiting = true;

//            while (_awaiting)
//            {
//                if (time == 0)
//                    break;

//                if (time == half)
//                    _choosingPlayer.SendMessage(time + " seconds remaining!");

//                if (time <= last)
//                    _choosingPlayer.SendMessage(time.ToString());

//                Thread.Sleep(1000);
//                time -= 1;
//            }
//        }

//        private void GuessTimer(int seconds, Channel channel)
//        {
//            const int last = 5;
//            var time = seconds;
//            var secondHint = time - seconds/6;
//            var thirdHint = time - seconds/3;
//            var forthHint = seconds/2;
//            var fifthHint = seconds/4;

//            _awaiting = true;

//            while (_awaiting)
//            {
//                if (time == 0)
//                    break;

//                if (time == secondHint)
//                {
//                    channel.SendMessage($"{time} seconds remaining!\n" +
//                                        $"Genres: {string.Join(", ", _chosenanime.Genres.Select(g => g.ToString()))}\n" +
//                                        $"Episodes: {_chosenanime?.TotalEpisodes?.ToString() ?? "???"}\n" +
//                                        $"Type: {_chosenanime.Type}\n");
//                }

//                if (time == thirdHint)
//                {
//                    channel.SendMessage($"{time} seconds remaining!\n" +
//                                        $"Genres: {string.Join(", ", _chosenanime.Genres.Select(g => g.ToString()))}\n" +
//                                        $"Episodes: {_chosenanime.TotalEpisodes?.ToString() ?? "???"}\n" +
//                                        $"Type: {_chosenanime.Type}\n" +
//                                        $"Year: {_chosenanime.StartDate?.Remove(4) ?? "???"}");
//                }

//                if (time == forthHint)
//                {

//                    var desc = _chosenanime.Description
//                        .Replace(_chosenanime.TitleRomaji, "****")
//                        .Replace(_chosenanime.TitleEnglish, "****");

//                    if (desc.Length >= 200)
//                        desc = desc.Remove(197) + "...";

//                    channel.SendMessage($"{time} seconds remaining!\n" +
//                                        $"Genres: {string.Join(", ", _chosenanime.Genres.Select(g => g.ToString()))}\n" +
//                                        $"Episodes: {_chosenanime.TotalEpisodes?.ToString() ?? "???"}\n" +
//                                        $"Type: {_chosenanime.Type}\n" +
//                                        $"Description:\n" +
//                                        $"{desc}");
//                }

//                if (time == fifthHint)
//                {

//                    var desc = _chosenanime.Description
//                        .Replace(_chosenanime.TitleRomaji, "****")
//                        .Replace(_chosenanime.TitleEnglish, "****");

//                    if (desc.Length >= 200)
//                        desc = desc.Remove(197) + "...";

//                    channel.SendMessage($"{time} seconds remaining!\n" +
//                                        $"Genres: {string.Join(", ", _chosenanime.Genres.Select(g => g.ToString()))}\n" +
//                                        $"Episodes: {_chosenanime.TotalEpisodes?.ToString() ?? "???"}\n" +
//                                        $"Type: {_chosenanime.Type}\n" +
//                                        $"Description:\n" +
//                                        $"{desc}" +
//                                        $"Image: {_chosenanime.ImageUrlLge}\n");
//                }

//                if (time <= last)
//                    channel.SendMessage(time.ToString());

//                Thread.Sleep(1000);
//                time -= 1;
//            }
//        }

//        private Anime[] _tempEntries;
//        public void MessageReceived(object sender, MessageEventArgs messageEventArgs)
//        {
//            if (messageEventArgs.Message.Channel.IsPrivate)
//            {
//                var member = messageEventArgs.User;

//                // Only accept messages from the choosing player
//                if (_state != GameState.AwaitingAnime || member != _choosingPlayer)
//                    return;

//                var message = messageEventArgs.Message.Text.Trim(' ');
//                int choice;

//                // If bot receives a parseable number, assume they're trying to choose an anime
//                if (int.TryParse(message, out choice))
//                {
//                    // The choice was not valid, so return
//                    if (_tempEntries == null || choice >= _tempEntries.Length)
//                        return;

//                    _awaiting = false;
//                    _chosenanime = _client.Get<AnimeBig>(Category.Anime, _tempEntries[choice].Id);

//                    var title = _chosenanime.TitleRomaji;
//                    var engtitle = _chosenanime.TitleEnglish;

//                    // filter out big letters, space and special characters
//                    _title = string.Join("", title.ToLower().Where(c => (c >= 'a' && c <= 'z') || char.IsDigit(c)));
//                    _engtitle = string.Join("", engtitle.ToLower().Where(c => (c >= 'a' && c <= 'z') || char.IsDigit(c)));
//                    member.SendMessage("You have chosen " + _chosenanime.TitleRomaji);

//                    return;
//                }

//                _tempEntries = _client.Search<Anime>(Category.Anime, message);

//                if (_tempEntries != null)
//                {
//                    var suggests = "";

//                    // Provide the chooser options of what anime he wants the others to guess
//                    for (var i = 0; i < _tempEntries.Length; i++)
//                    {
//                        suggests += string.Format(_suggest, i, _tempEntries[i].TitleRomaji, _tempEntries[i].TitleEnglish);
//                    }

//                    member.SendMessage(suggests);
//                }
//                else
//                {
//                    member.SendMessage("Couldn't find any anime of the name " + message);
//                    _tempEntries = null;
//                }
//            }
//            else
//            {
//                GuessAnime(messageEventArgs.Channel, messageEventArgs.User, messageEventArgs.Message.Text);
//            }
//        }
//    }
//}
