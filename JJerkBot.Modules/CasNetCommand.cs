using System;
using System.Threading.Tasks;
using AESharp;
using AESharp.Evaluator;
using Discord;
using Discord.Commands;
using JJerkBot.Modules.AESharpExtensions;

namespace JJerkBot.Modules
{
    public class CasNetCommand : ICommand
    {
        private readonly Evaluator _evaluator = new Evaluator();
        
        public string Name => "æsharp";
        public string Description => "Interprets Æ# code.";
        public string[] Alias => null;
        public bool Hide => false;
        public Parameter[] Parameters => new []
        {
            new Parameter("code", ParameterType.Unparsed), 
        };

        public bool Check(Command command, User user, Channel channel) => true;

        public async Task Do(CommandEventArgs args)
        {
            // TODO: Handle code block ´´´ code ´´´

            var scope = new Scope(_evaluator);
            _evaluator.SetVar("discord", scope);
            scope.SetVar(SendMsgFunc.Name, new SendMsgFunc(scope, args.Channel));
            
            _evaluator.Parse(args.GetArg("code"));
            var res = _evaluator.Evaluate();

            if (res is Error)
                await args.Channel.SendMessage("Error!: " + res + "\n");
        }
    }
}
