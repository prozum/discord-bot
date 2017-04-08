using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Globalization;
using System.Diagnostics;

namespace JJerkBot.Modules
{
    public class TryYourAnimeNameCommand : ICommand
    {
        private static readonly Dictionary<char, string> _translater = new Dictionary<char, string>()
        {
            { 'a', "ka" },  { 'b', "tu" },  { 'c', "mi" },
            { 'd', "te" },  { 'e', "ku" },  { 'f', "lu" },
            { 'g', "ji" },  { 'h', "ri" },  { 'i', "ki" },
            { 'j', "zu" },  { 'k', "me" },  { 'l', "ta" },
            { 'm', "rin" }, { 'n', "to" },  { 'o', "mo" },
            { 'p', "no" },  { 'q', "ke" },  { 'r', "shi" },
            { 's', "ari" }, { 't', "chi" }, { 'u', "do" },
            { 'v', "ru" },  { 'w', "mei" }, { 'x', "na" },
            { 'y', "fu" },  { 'z', "zi" },
        };

        public string Name => "tryYourAnimeName";
        public string Description => "Translates everything given to this command to \"Japanese\". Based on https://s-media-cache-ak0.pinimg.com/736x/c6/f6/73/c6f673b555b345ff6286a2805f4e0efc.jpg";
        public string[] Alias => new []{ "tyan", "animeName", "tryAnime" };
        public bool Hide => false;
        public Parameter[] Parameters => new[]
        {
            new Parameter("sentence", ParameterType.Unparsed)
        };

        public bool Check(Command command, User user, Channel channel) => true;

        public async Task Do(CommandEventArgs args)
        {
            var sentence = args.GetArg("sentence");
            var builder = new StringBuilder();

            foreach (var chr in sentence)
            {
                var small = char.ToLower(chr);

                if (_translater.TryGetValue(small, out string translated))
                {
                    if (builder.Length + translated.Length <= 2000)
                    {
                        if (!char.IsLower(chr))
                            translated = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(translated);

                        builder.Append(translated);

                    }
                    else
                        break;
                }
                else if (builder.Length < 2000)
                    builder.Append(chr);
                else
                    break;
            }

            Debug.Assert(builder.Length <= 2000);

            await args.Channel.SendMessage(builder.ToString());
        }
    }
}
