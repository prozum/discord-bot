using System;
using System.Text;
using System.Threading.Tasks;
using AESharp;
using Discord;
using Discord.Commands;

namespace JJerkBot.Modules
{
    public class CasNetCommand : ICommand
    {
        private readonly Evaluator _evaluator = new Evaluator();

        public string Name => "æsharp";
        public string Description => "Interprets Æ# code.";
        public string[] Alias => null;
        public bool Hide => false;
        public Parameter[] Parameters => new[]
        {
            new Parameter("code", ParameterType.Unparsed),
        };

        public bool Check(Command command, User user, Channel channel) => true;

        public async Task Do(CommandEventArgs args)
        {
            // TODO: Handle code block ´´´ code ´´´
            _evaluator.Parse(args.GetArg("code"));
            var res = _evaluator.Evaluate();

            var output = new StringBuilder();
            if (res is Error)
            {
                output.Append($"Error!: {res}\n");
            }
            else
            {
                foreach (var effect in _evaluator.SideEffects)
                {
                    var printData = effect as PrintData;
                    if (printData != null)
                        output.Append(printData.msg);
                }
            }

            while (output.Length > 2000)
            {
                await args.Channel.SendMessage(output.ToString().Substring(0, 2000));
                output.Remove(0, 2000);
            }

            await args.Channel.SendMessage(output.ToString());
        }
    }
}
