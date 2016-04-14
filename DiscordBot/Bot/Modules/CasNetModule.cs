using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.API.Model;
using System.Text.RegularExpressions;
using AESharp;
using Discord.Bot.Modules.AESharpExtensions;

namespace Discord.Bot.Modules
{
    public class CasNetModule : IMessageModule, ICommandable
    {
        static readonly int cordsize = 21;
        static readonly Regex regex = new Regex(@"^```[^\w\d]*#Æ#([^`]*)```$");

        Evaluator evaluator = new Evaluator();

        public void MessageGotten(DiscordBot bot, Message message)
        {
            var match = regex.Match(message.Content);

            if (!match.Success)
                return;

            try
            {
                Interpret(match.Groups[1].Value, bot);
            }
            catch (Exception e)
            {
                bot.SendMessage("Error!\n" + e.Message);
            }
        }

        public bool TryRunCommand(string command, DiscordBot bot)
        {
            var match = regex.Match(command);

            if (!match.Success)
                return false;

            try
            {
                Interpret(match.Groups[1].Value, bot);
                return true;
            }
            catch (Exception e)
            {
                bot.SendMessage("Error!\n" + e.Message);
                return false;
            }
        }

        private void Interpret(string str, DiscordBot bot)
        {
            string output = "";
            Expression res;

            var scope = new Scope(evaluator);
            evaluator.SetVar("discord", scope);
            scope.SetVar("sendmsg", new SendMsgFunc(scope, bot));
            scope.SetVar("getmsgs", new GetMsgFunc(scope, bot));
            scope.SetVar("lastmsg", new LastMsgFunc(scope, bot));

            evaluator.Parse(str);

            if ((res = evaluator.Evaluate()) is AESharp.Error)
                output += "Error!: " + res.ToString() + "\n";

            foreach (var effect in evaluator.SideEffects)
            {
                if (effect is PlotData)
                {
                    output += MakeAsciiPlot(effect as PlotData);
                }
            }

            bot.SendMessage(output);
        }

        private string MakeAsciiPlot(PlotData effect)
        {
            string res = "";

            for (int y = 0; y < cordsize; y++)
            {
                for (int x = 0; x < cordsize; x++)
                {
                    if (y - cordsize / 2 == 0 || x - cordsize / 2 == 0)
                    {
                        res += " . ";
                    }
                    else
                    {
                        bool notfound = true;
                        for (int i = 0; i < effect.x.Count; i++)
                        {
                            if (((int)effect.x[i].@decimal == x - cordsize / 2 && (int)effect.y[i].@decimal == ((cordsize - 1) - y) - cordsize / 2))
                            {
                                res += " . ";
                                notfound = false;
                                break;
                            }
                        }

                        if (notfound)
                            res += "   ";
                    }
                }

                res += "\n";
            }

            return res;
        }
    }
}
