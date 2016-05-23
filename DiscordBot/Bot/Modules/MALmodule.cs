using Discord.Bot.BaseModules;
using DiscordSharp.Events;
using MyAnimeListSharp.Auth;
using MyAnimeListSharp.Facade;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Bot.Modules
{
    public class MALmodule : BaseMessageModule
    {
        readonly ICredentialContext _credential;
        readonly SearchMethods _search;

        static readonly string _commandName = "#anime";
        static readonly string _animeMessage =
            "Title: {0}\n" +
            "English Title: {1}\n" +
            "Episodes: {2}\n" +
            "Link: http://myanimelist.net/anime/{3}\n" +
            "Image: {4}" +
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

            if (message.Content.StartsWith(_commandName))
            {
                var arg = message.Content.Remove(0, _commandName.Length);
                var reponse = _search.SearchAnimeDeserialized(arg);
                
                if (reponse.Entries.Count != 0)
                {
                    var anime = reponse.Entries.First();

                    channel.SendMessage(
                        string.Format(_animeMessage,
                            anime.Title,
                            anime.English,
                            anime.Episodes,
                            anime.Id,
                            anime.Image,
                            anime.Synopsis));
                }
            }
        }
    }
}
