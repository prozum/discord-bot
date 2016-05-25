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
        const int _cordSize = 21;
        const string _commandName = "#æsharp ";

        readonly Evaluator _evaluator = new Evaluator();

        public override void MessageReceived(object sender, DiscordMessageEventArgs e)
        {
            var channel = e.Channel;
            var message = e.Message;

            try
            {
                if (message.Content.StartsWith(_commandName))
                    Interpret(message.Content.Remove(0, _commandName.Length), channel);
            }
            catch (Exception ex)
            {
                channel.SendMessage("Error!\n" + ex.Message);
            }
        }

        private void Interpret(string str, DiscordChannel channel)
        {
            var output = "";
            Expression res;

            var scope = new Scope(_evaluator);
            _evaluator.SetVar("discord", scope);
            scope.SetVar("sendmsg", new SendMsgFunc(scope, channel));

            _evaluator.Parse(str);

            if ((res = _evaluator.Evaluate()) is Error)
                output += "Error!: " + res + "\n";
           
            foreach (var effect in _evaluator.SideEffects.OfType<PlotData>())
            {
                output += "```\n" + MakeAsciiPlot(effect) + "\n```";
            }

            channel.SendMessage(output);
        }

        static string MakeAsciiPlot(PlotData effect)
        {
            var res = "";

            for (var y = 0; y < _cordSize; y++)
            {
                for (var x = 0; x < _cordSize; x++)
                {
                    if (y - _cordSize / 2 == 0 || x - _cordSize / 2 == 0)
                    {
                        res += " . ";
                    }
                    else
                    {
                        var notfound = true;
                        for (var i = 0; i < effect.x.Count; i++)
                        {
                            if ((int) effect.x[i].@decimal != x - _cordSize/2 ||
                                (int) effect.y[i].@decimal != _cordSize - 1 - y - _cordSize/2)
                                continue;

                            res += " . ";
                            notfound = false;
                            break;
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
