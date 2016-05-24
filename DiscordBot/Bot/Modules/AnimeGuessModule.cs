﻿using Discord.Bot.BaseModules;
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
        static readonly string _start = "start";
        static readonly string _join = "join";

        static readonly string _helpChooser =
            "You have been chosen to pick an anime.\n" +
            "To choose an anime first just write the name to me.\n" +
            "Then, use the #choose command to pick correct anime from the list i provide you.\n" +
            "Example:\n" + 
            "You: Clannad\n\n" +
            "Bot: ---0---\n" +
            "     Title: Clannad Movie\n\n" +
            "     ---1---\n" +
            "     Title: Clannad\n\n" +
            "You: #choose 1\n\n" +
            "Bot: You have chosen Clannad";

        static readonly string _anwser =
            "Answer: {0}\n" +
            "http://myanimelist.net/anime/{1}";

        static readonly string _lost = 
            "No one guessed the anime.\n" +
            _anwser;

        static readonly string _win =
            "{2} guessed the anime.\n" +
            _anwser;


        readonly ICredentialContext _credential;
        readonly SearchMethods _search;

        GameState _state = GameState.Idle;
        GameHint _hint = GameHint.Synopsis;
        int _awaittime = 60;
        bool _awaiting = true;

        List<DiscordMember> _players = new List<DiscordMember>();
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

                if (arg == _start)
                {
                    _players.Clear();
                    _choosingPlayer = null;
                    _chosenanime = null;
                    _hint = GameHint.Synopsis;

                    Task.Run(() => GameLoop(channel));
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
                    if (!_players.Contains(author))
                    {
                        _players.Add(author);
                        channel.SendMessage(author.Username + " joined the game!");
                    }
                }
            }
        }

        private void MessageReceivedPlaying(DiscordMessageEventArgs e)
        {
            var author = e.Author;
            var channel = e.Channel;
            var message = new string(e.Message.Content.SkipWhile(chr => chr == ' ').ToArray()).ToLower(); ;

            if (_players.Contains(author) && (_title == message || _engtitle == message))
            {
                channel.SendMessage(string.Format(_win, _chosenanime.Title, _chosenanime.Id, e.Author.Username));
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
                        _title = new string(_chosenanime.Title.SkipWhile(chr => chr == ' ').ToArray()).ToLower();
                        _engtitle = new string(_chosenanime.English.SkipWhile(chr => chr == ' ').ToArray()).ToLower();
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
            channel.SendMessage("Awaiting players!");
            _state = GameState.AwaitingPlayers;
            Await(_awaittime, str => channel.SendMessage(str));


            if (_players.Count > 0)
            {
                int index = 0;
                int chosen = _randomizer.Next(_players.Count);
                _choosingPlayer = _players.FirstOrDefault(pair => index++ == chosen);

                _choosingPlayer.SendMessage(_helpChooser);
                channel.SendMessage(_choosingPlayer.Username + " is choosing an anime.");

                _state = GameState.AwaitingAnime;
                Await(_awaittime, str => { channel.SendMessage(str); _choosingPlayer.SendMessage(str); });

                if (_chosenanime != null)
                {
                    switch (_hint)
                    {
                        case GameHint.Synopsis:
                            channel.SendMessage(_chosenanime.Synopsis);
                            break;
                        case GameHint.Image:
                            channel.SendMessage(_chosenanime.Image);
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    _state = GameState.Playing;
                    Await(_awaittime * 2, str => channel.SendMessage(str));

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
            else
            {
                channel.SendMessage("Not enough players joined.");
            }

            _state = GameState.Idle;
        }

        private void Await(int seconds, Action<string> send)
        {
            var time = seconds;
            var half = seconds / 2;
            var last = 5;
            _awaiting = true;

            while (_awaiting)
            {
                if (time <= 0)
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
