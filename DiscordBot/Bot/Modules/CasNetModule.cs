using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.API.Model;
using System.Text.RegularExpressions;
using AESharp;

namespace Discord.Bot.Modules
{
    public class CasNetModule : IMessageModule
    {
        Evaluator evalor = new Evaluator();
        Regex regex = new Regex(@"^```\s*\n*\s*#Æ#\n*([^`]*)```$");

        public void MessageGotten(DiscordBot bot, Message message)
        {
            var match = regex.Match(message.content);

            if (!match.Success)
                return;

            var group = match.Groups[1].Value;
            Interpret(group, bot);
        }

        private void Interpret(string str, DiscordBot bot)
        {
            Expression res;
            evalor.Parse(str);

            if ((res = evalor.Evaluate()) is AESharp.Error)
            {
                Task.Run(() => { bot.Client.SendMessage(bot.Channel, "Error!: " + res.ToString()); });
            }
            else
            {
                foreach (var effect in evalor.SideEffects)
                {
                    if (effect is PrintData)
                    {
                        Task.Run (() => { bot.Client.SendMessage(bot.Channel, (effect as PrintData).msg); });
                    }
                }
            }

        }

    }
}
