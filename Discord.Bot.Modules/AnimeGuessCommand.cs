using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord.API.Client;
using Discord.Bot.Interfaces;

namespace Discord.Bot.Modules
{
    // TODO: rewrite this game
    public class AnimeGuessCommand : IDiscordBotCommand, IMessageReceivedModule
    {
        class AnimeGuessGame
        {
            public class PlayerData
            {
                public bool CanGuess { get; set; }
                public int Score { get; set; }
            }
            
            private readonly Random _randomizer = new Random();
            //private readonly AnilistClient _client;

            private const bool _chooserCanGuess = false;
            private const int _minPlayers = 0;
            private const int _awaittime = 60;
            private const int _maxScore = 5;

            private bool _awaiting = true;
            private string _shortTitle;
            private string _shortEngTitle;
            
            public Dictionary<User, PlayerData> Players { get; } = new Dictionary<User, PlayerData>();
            //public AnimeBig ChosenAnime { get; private set; }
            public User Host { get; }
            public User Chooser { get; private set; }
            public GameState State { get; private set; } = GameState.AwaitingPlayers;

            public enum GameState
            {
                AwaitingPlayers,
                AwaitingAnime,
                Playing
            }

            public AnimeGuessGame(User host)
            {
                Host = host;
                //_client = client;

                Players.Add(host, new PlayerData());
            }

            public enum PassResult
            {
                PlayerPassed,
                AllPassed,
                PassFailed
            }

            public PassResult Pass(User user)
            {
                PlayerData playerData;
                if (!Players.TryGetValue(user, out playerData) || playerData.CanGuess)
                    return PassResult.PassFailed;

                playerData.CanGuess = false;

                if (Players.All(player => !player.Value.CanGuess))
                    return PassResult.AllPassed;

                return PassResult.PlayerPassed;
            }

            public bool Guess(User user, string guess)
            {
                PlayerData playerData;
                if ((!_chooserCanGuess && user == Chooser) || !Players.TryGetValue(user, out playerData))
                    return false;

                // filter out big letters, space and special characters
                guess = new string(guess.ToLower().Where(c => (c >= 'a' && c <= 'z') || char.IsDigit(c)).ToArray());

                if ((!string.IsNullOrEmpty(_shortTitle) || _shortTitle == guess) &&
                    (!string.IsNullOrEmpty(_shortEngTitle) || _shortEngTitle == guess))
                {
                    playerData.Score++;
                    _awaiting = false;

                    return true;
                }

                return false;
            }

            public bool Start(User user)
            {
                if (State == GameState.AwaitingPlayers && user == Host)
                {
                    State = GameState.Playing;
                    return true;
                }

                return false;
            }

            public bool Join(User user)
            {
                if (Players.ContainsKey(user))
                    return false;

                Players.Add(user, new PlayerData());
                return true;
            }

            // TODO: think about how animes a choosen. Should probably be related to peoples anime list profiles (maybe)
        }

        private const string _host = "-host";
        private const string _start = "-start";
        private const string _join = "-join";
        private const string _stop = "-stop";
        private const string _pass = "-pass";

        private const string _helpChooser =
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

        private const string _answer =
            "Answer:\n" +
            "\tTitle: {0}\n" +
            "\tEnglish Title: {1}\n";

        private const string _lost =
            "No one guessed the anime.\n" +
            _answer;

        private const string _win =
            "{2} guessed the anime.\n" +
            _answer + "\n\n" +
            "Current Scores:\n";

        private const string _suggest =
            "---{0}---\n" +
            "Title: {1}\n" +
            "English Title: {2}\n\n";

        private readonly Dictionary<ulong, AnimeGuessGame> _games = new Dictionary<ulong, AnimeGuessGame>();
        //private readonly AnilistClient _client;

        public AnimeGuessCommand(/*AnilistClient client*/)
        {
            //_client = client;
        }

        public int? ArgumentCount => 1;
        public string CommandName => "guess";
        public string Help =>
@"A anime guessing game.

Arguments:
    -host     Host a game.
    -start    Start your hosted game.
    -join     Join a hosted game.
    -stop     Stop your hosted game.
    -pass     Pass your guess.";

        public string ModuleName => "Anime Guessing Game";
        public string ModuleDescription => "This module receives channel and private messages, which it uses to play an anime guessing game";

        public void Execute(string[] args, Server server, Channel channel, User user, Message message)
        {
            switch (args[0])
            {
                case _join:
                    JoinGameCommand(channel, user);
                    break;
                case _stop:
                    StopGameCommand(channel, user);
                    break;
                case _host:
                    HostGame(channel, user);
                    break;
                case _start:
                    //StartGame(user);
                    break;
                case _pass:
                    PassTurn(user, channel);
                    break;
            }
        }

        private void PassTurn(User user, Channel channel)
        {
            AnimeGuessGame game;
            if (!_games.TryGetValue(channel.Id, out game))
                return;

            switch (game.Pass(user))
            {
                case AnimeGuessGame.PassResult.PlayerPassed:
                    channel.SendMessage($"{user.NicknameMention} passes this turn, and can't guess anymore.");
                    break;
                case AnimeGuessGame.PassResult.AllPassed:
                    var stringBuilder = new StringBuilder($"{user.NicknameMention} passes this turn, and can't guess anymore.");
                    stringBuilder.Append("No one guessed the anime.\n");
                    stringBuilder.Append("Answer:\n");
                    //stringBuilder.Append($"\tTitle: {game.ChosenAnime.TitleRomaji}\n");
                    //stringBuilder.Append($"\tEnglish Title: {game.ChosenAnime.TitleEnglish}\n");

                    channel.SendMessage(stringBuilder.ToString());
                    break;
                case AnimeGuessGame.PassResult.PassFailed:
                    break;
            }
        }

        private void GuessAnime(Channel channel, User user, string guess)
        {
            AnimeGuessGame game;
            if (!_games.TryGetValue(channel.Id, out game))
                return;

            if (game.Guess(user, guess))
            {
                var message = new StringBuilder($"{user.NicknameMention} guessed the anime.\n");
                message.Append("Answer:\n");
                //message.Append($"\tTitle: {game.ChosenAnime.TitleRomaji}\n");
                //message.Append($"\tEnglish Title: {game.ChosenAnime.TitleRomaji}\n\n\n");

                message.Append("Current Scores:\n");

                foreach (var player in game.Players)
                    message.Append($"{player.Key.NicknameMention}: {player.Value.Score}\n");

                channel.SendMessage(message.ToString());
            }
        }

        private void StartGame(User user, Channel channel)
        {
            AnimeGuessGame game;
            if (!_games.TryGetValue(channel.Id, out game))
                return;

            game.Start(user);
        }

        private void HostGame(Channel channel, User user)
        {
            if (_games.ContainsKey(channel.Id))
            {
                channel.SendMessage("A game is already running.");
                return;
            }

            //var game = new AnimeGuessGame(user, _client);
            //_games.Add(channel.Id, game);

            // TODO: more host logic
        }

        private void StopGameCommand(Channel channel, User user)
        {
            AnimeGuessGame game;
            if (!_games.TryGetValue(channel.Id, out game) || user != game.Host)
                return;

            _games.Remove(channel.Id);
            channel.SendMessage($"Game was stopped by {user.NicknameMention}.");
        }

        private void JoinGameCommand(Channel channel, User user)
        {
            AnimeGuessGame game;
            if (!_games.TryGetValue(channel.Id, out game) || user != game.Host)
                return;

            if (game.Join(user))
                channel.SendMessage($"{user.NicknameMention} joined the game!");
        }

        public void MessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            // TODO: think about how animes a choosen. Should probably be related to peoples anime list profiles (maybe)
        }
    }
}
