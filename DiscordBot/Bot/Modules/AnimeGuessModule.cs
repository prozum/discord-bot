using Discord.Bot.BaseModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordSharp.Events;
using System.Threading;
using DiscordSharp;
using DiscordSharp.Objects;
using MyAnimeListSharp.Core;
using MyAnimeListSharp.Auth;
using MyAnimeListSharp.Facade;
using MyAnimeListSharp.Util;

namespace Discord.Bot.Modules
{
    public class AnimeGuessModule : BaseMessageModule
    {
        enum GameState { Idle, AwaitingPlayers, AwaitingAnime, Playing }
        enum GameHint { Synopsis, Image }

        static readonly string _commandName = "#guessgame ";
        static readonly string _choose = "#choose ";
        static readonly string _host = "host";
        static readonly string _start = "start";
        static readonly string _stop = "stop";
        static readonly string _join = "join";

        static readonly string _helpChooser =
            "You have been chosen to pick an anime.\n" +
            "To choose an anime first just write the name to me.\n" +
            "Then, use the #choose command to pick correct anime from the list i provide you.\n" +
            "Example:\n" + 
            "You: n" + 
            "Clannad\n\n" +
            "Bot:\n" + 
            "---0---\n" +
            "Title: Clannad Movie\n\n" +
            "---1---\n" +
            "Title: Clannad\n\n" +
            "You:\n" + 
            "#choose 1\n\n" +
            "Bot:\n" + 
            "You have chosen Clannad";

        static readonly string _anwser =
            "Answer: {0}\n" +
            "http://myanimelist.net/anime/{1}";

        static readonly string _lost = 
            "No one guessed the anime.\n" +
            _anwser;

        static readonly string _win =
            "{2} guessed the anime.\n" +
            _anwser;

        static readonly uint _minPlayers = 2;
        static readonly uint _awaittime = 60;
        static readonly uint _maxScore = 5;

        readonly ICredentialContext _credential;
        readonly SearchMethods _search;

        GameState _state = GameState.Idle;
        GameHint _hint = GameHint.Synopsis;
        bool _awaiting = true;

        Dictionary<DiscordMember, uint> _players = new Dictionary<DiscordMember, uint>();
        DiscordMember _choosingPlayer = null;
        AnimeEntry _chosenanime = null;
        string _title = null;
        string _engtitle = null;

        Random _randomizer = new Random();

        public AnimeGuessModule(ICredentialContext credential)
        {
            _credential = credential;
            _search = new SearchMethods(_credential);
        }

        public override void MessageReceived(object sender, DiscordMessageEventArgs e)
        {
            switch (_state)
            {
                case GameState.Idle:
                    MessageReceivedIdle(e);
                    break;
                case GameState.AwaitingPlayers:
                    MessageReceivedAwaitingPlayers(e);
                    break;
                case GameState.Playing:
                    MessageReceivedPlaying(e);
                    break;
                default:
                    break;
            }
        }

        private void MessageReceivedIdle(DiscordMessageEventArgs e)
        {
            var message = e.Message.Content;
            var channel = e.Channel;

            if (message.StartsWith(_commandName))
            {
                var arg = message.Remove(0, _commandName.Length).Trim(' ');

                if (arg == _host)
                {
                    _players.Clear();
                    _choosingPlayer = null;
                    _chosenanime = null;
                    _hint = GameHint.Synopsis;
                    _players.Add(e.Author, 0);

                    Task.Run(() => 
                    {
                        channel.SendMessage("Awaiting players!");
                        _state = GameState.AwaitingPlayers;

                        while (_state == GameState.AwaitingPlayers)
                        {
                            Thread.Sleep(1000);
                        }

                        if (_state == GameState.Idle)
                        {
                            return;
                        }

                        if (_players.Count >= _minPlayers)
                        {
                            GameLoop(channel);
                        }
                        else
                        {
                            channel.SendMessage("Not enough players joined.");
                        }
                    });
                }
            }
        }
        private void MessageReceivedAwaitingPlayers(DiscordMessageEventArgs e)
        {
            var message = e.Message.Content;
            var channel = e.Channel;
            var author = e.Author;

            if (message.StartsWith(_commandName))
            {
                var arg = message.Remove(0, _commandName.Length).Trim(' ');

                if (arg == _join)
                {
                    if (!_players.ContainsKey(author))
                    {
                        _players.Add(author, 0);
                        channel.SendMessage(author.Username + " joined the game!");
                    }

                    return;
                }

                if (arg == _start)
                {
                    _state = GameState.Playing;
                    return;
                }

                if (arg == _stop)
                {
                    _state = GameState.Idle;
                    return;
                }
            }
        }

        private void MessageReceivedPlaying(DiscordMessageEventArgs e)
        {
            var author = e.Author;
            var channel = e.Channel;
            var message = e.Message.Content.Replace(" ", "").ToLower();

            if (author != _choosingPlayer && _players.ContainsKey(author) && (_title == message || _engtitle == message))
            {
                _players[author]++;
                channel.SendMessage(string.Format(_win, _chosenanime.Title, _chosenanime.Id, e.Author.Username));

                var msg = "Current Scores:\n";

                foreach (var item in _players)
                {
                    msg += item.Key.Username + ": " + item.Value + "\n";
                }

                channel.SendMessage(msg);
                _awaiting = false;
            }
        }

        List<AnimeEntry> _tempEntries = null;
        public override void PrivateMessageReceived(object sender, DiscordPrivateMessageEventArgs e)
        {
            var member = e.Author;

            // Only accept messages from the choosing player
            if (_state == GameState.AwaitingAnime && member == _choosingPlayer)
            {
                var message = e.Message;

                if (message.StartsWith(_choose))
                {
                    var arg = message.Remove(0, _choose.Length).Trim(' ');
                    int choice;
                            
                    if (_tempEntries != null && int.TryParse(arg, out choice) && choice < _tempEntries.Count)
                    {
                        _awaiting = false;
                        _chosenanime = _tempEntries[choice];
                        _title = _chosenanime.Title.Replace(" ", "").ToLower();
                        _engtitle = _chosenanime.English.Replace(" ", "").ToLower();
                        member.SendMessage("You have chosen " + _chosenanime.Title);
                    }

                    return;
                }

                var response = _search.SearchAnime(message.Trim(' ').Replace(' ', '_'));

                if (!string.IsNullOrEmpty(response))
                {
                    var suggests = "";
                    _tempEntries = new SearchResponseDeserializer<AnimeSearchResponse>().Deserialize(response).Entries;

                    // Provide the chooser options of what anime he wants the others to guess
                    for (int i = 0; i < _tempEntries.Count; i++)
                    {
                        suggests += string.Format("---" + i + "---\nTitle: {0}\n\n", _tempEntries[i].Title);
                    }

                    member.SendMessage(suggests);
                }
                else
                {
                    member.SendMessage("Couldn't find any anime of the name " + message);
                    _tempEntries = null;
                }
            }
        }

        private void GameLoop(DiscordChannel channel)
        {
            DiscordMember winner;

            while (!IsDone(out winner))
            {
                int chosen = _randomizer.Next(_players.Count);
                _choosingPlayer = _players.ElementAt(chosen).Key;

                _choosingPlayer.SendMessage(_helpChooser);
                channel.SendMessage(_choosingPlayer.Username + " is choosing an anime.");

                _state = GameState.AwaitingAnime;
                Await(_awaittime, str => { channel.SendMessage(str); _choosingPlayer.SendMessage(str); });

                if (_chosenanime != null)
                {
                    switch (_hint)
                    {
                        case GameHint.Synopsis:
                            var msg = _chosenanime.Synopsis.Replace("<br />", "")
                                                     .Replace("&#039;", "'")
                                                     .Replace("[/i]", "")
                                                     .Replace("[i]", "")
                                                     .Replace("&quot;", "\"")
                                                     .Replace("&mdash;", "-");

                            if (!string.IsNullOrEmpty(_chosenanime.Title))
                                msg = msg.Replace(_chosenanime.Title, "****");

                            if (!string.IsNullOrEmpty(_chosenanime.English))
                                msg = msg.Replace(_chosenanime.English, "****");

                            channel.SendMessage(
                                "Anime has been chosen.\n" +
                                "Synopsis:\n" +
                                msg.Remove(200) + "...");
                            break;
                        case GameHint.Image:
                            channel.SendMessage(
                                "Anime has been chosen.\n" +
                                "Image:\n" +
                                _chosenanime.Image);
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    _state = GameState.Playing;
                    Await(_awaittime, str => channel.SendMessage(str));

                    if (_awaiting)
                    {
                        channel.SendMessage(string.Format(_lost, _chosenanime.Title, _chosenanime.Id));
                    }
                }
                else
                {
                    channel.SendMessage("Anime wasn't chosen.");
                }
            }
            
            channel.SendMessage(winner.Username + " won the game!");
            _state = GameState.Idle;
        }

        private bool IsDone(out DiscordMember winner)
        {
            winner = null;

            foreach (var item in _players)
            {
                if (item.Value >= _maxScore)
                {
                    winner = item.Key;
                    return true;
                }
            }

            return false;
        }

        private void Await(uint seconds, Action<string> send)
        {
            var time = seconds;
            var half = seconds / 2;
            var last = 5;
            _awaiting = true;

            while (_awaiting)
            {
                if (time == 0)
                {
                    break;
                }

                if (time == half)
                {
                    send(time + " seconds remaining!");
                }

                if (time <= last)
                {
                    send(time.ToString());
                }

                Thread.Sleep(1000);
                time -= 1;
            }
        }
    }
}
