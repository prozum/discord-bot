using System;
using System.Linq;
using AESharp;
using Discord.Bot.Modules.AESharpExtensions;
using Discord.Bot.BaseModules;
using DiscordSharp.Events;
using DiscordSharp.Objects;

namespace Discord.Bot.Modules
{
    public class CasNetModule : BaseMessageModule
    {
        static readonly int _cordsize = 21;
        static readonly string _commandName = "#æsharp ";

        Evaluator evaluator = new Evaluator();

        public override void MessageReceived(object sender, DiscordMessageEventArgs e)
        {
            var channel = e.Channel;
            var message = e.Message;

            try
            {
                if (message.Content.StartsWith(_commandName))
                {
                    Interpret(message.Content.Remove(0, _commandName.Count()), channel);
                }
            }
            catch (Exception ex)
            {
                channel.SendMessage("Error!\n" + ex.Message);
            }
        }

        private void Interpret(string str, DiscordChannel channel)
        {
            string output = "";
            Expression res;

            var scope = new Scope(evaluator);
            evaluator.SetVar("discord", scope);
            scope.SetVar("sendmsg", new SendMsgFunc(scope, channel));

            evaluator.Parse(str);

            if ((res = evaluator.Evaluate()) is AESharp.Error)
                output += "Error!: " + res.ToString() + "\n";

            foreach (var effect in evaluator.SideEffects)
            {
                if (effect is PlotData)
                {
                    output += "```\n" + MakeAsciiPlot(effect as PlotData) + "\n```";
                }
            }

            channel.SendMessage(output);
        }

        private string MakeAsciiPlot(PlotData effect)
        {
            string res = "";

            for (int y = 0; y < _cordsize; y++)
            {
                for (int x = 0; x < _cordsize; x++)
                {
                    if (y - _cordsize / 2 == 0 || x - _cordsize / 2 == 0)
                    {
                        res += " . ";
                    }
                    else
                    {
                        bool notfound = true;
                        for (int i = 0; i < effect.x.Count; i++)
                        {
                            if (((int)effect.x[i].@decimal == x - _cordsize / 2 && (int)effect.y[i].@decimal == ((_cordsize - 1) - y) - _cordsize / 2))
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
