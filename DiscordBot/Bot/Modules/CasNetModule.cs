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
        static readonly int cordsize = 21;

        Evaluator evalor = new Evaluator();
        Regex regex = new Regex(@"^(?:(?:{\s*([\s\S]*)}\s*)|\s*)```[^\w\d]*#Æ#([^`]*)```$");

        public void MessageGotten(DiscordBot bot, Message message)
        {
            var match = regex.Match(message.content);

            if (!match.Success)
                return;

            try
            {
                Interpret(match.Groups[2].Value, match.Groups[1].Value, bot);
            }
            catch (Exception e)
            {
                bot.SendMessage(bot.Channel, "Error!\n" + e.Message);
            }
        }

        private void Interpret(string str, string args, DiscordBot bot)
        {
            string output = "";
            Expression res;

            var scope = new Scope(evalor);
            evalor.SetVar("discord", scope);

            scope.SetVar("args", Evaluator.Eval(args));

            evalor.Parse(str);

            if ((res = evalor.Evaluate()) is AESharp.Error)
                output += "Error!: " + res.ToString() + "\n";

            foreach (var effect in evalor.SideEffects)
            {
                if (effect is PrintData)
                {
                    output += (effect as PrintData).msg + "\n";
                }
                else if (effect is PlotData)
                {
                    output += MakeAsciiPlot(effect as PlotData);
                }
            }

            bot.SendMessage(bot.Channel, output);
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
