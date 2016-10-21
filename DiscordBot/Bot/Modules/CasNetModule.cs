using System;
using System.Linq;
using AESharp;
using Discord.Bot.Modules.AESharpExtensions;
using Discord.Bot.BaseModules;
using DiscordSharp;
using DiscordSharp.Events;
using DiscordSharp.Objects;

namespace Discord.Bot.Modules
{
    public class CasNetModule : BaseCommandModule
    {
        private readonly Evaluator _evaluator = new Evaluator();
        
        public CasNetModule(DiscordBot bot) 
            : base(bot)
        { }

        public override string CommandName => "æsharp";
        public override string Help =>
@"Interprets Æ# code.

Arguments:
    Takes 1 or more arguments.
    All has to be valid Æ# code.
    Arguments will be interpreted in the order they where typed";

        public override void CommandCalled(string[] args, DiscordMember author, DiscordChannel channel, DiscordMessage message,
            DiscordMessageType messageType)
        {
            if (args.Length == 0)
                return;

            var scope = new Scope(_evaluator);
            _evaluator.SetVar("discord", scope);
            scope.SetVar("sendmsg", new SendMsgFunc(scope, channel));

            foreach (var arg in args)
            {
                _evaluator.Parse(arg);
                var res = _evaluator.Evaluate();

                if (res is Error)
                {
                    channel.SendMessage("Error!: " + res + "\n");
                    break;
                }
            }

        }
    }
}
