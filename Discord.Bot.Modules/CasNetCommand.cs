using AESharp;
using Discord.Bot.Interfaces;
using Discord.Bot.Modules.AESharpExtensions;

namespace Discord.Bot.Modules
{
    public class CasNetCommand : IDiscordBotCommand
    {
        private readonly Evaluator _evaluator = new Evaluator();

        public int? ArgumentCount => null;
        public string CommandName => "æsharp";
        public string Help =>
@"Interprets Æ# code.

Arguments:
    Takes 1 or more arguments.
    All has to be valid Æ# code.
    Arguments will be interpreted in the order they where typed";

        public void Execute(string[] args, Server server, Channel channel, User user, Message message)
        {
            if (args.Length <= 1)
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
