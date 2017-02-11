using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace JJerkBot.Modules
{
    public class HelloCommand : ICommand
    {
        public string Name => "hello";
        public string Description => "Greets people.";
        public string[] Alias => new []{ "hi", "hey" };
        public bool Hide => false;
        public Parameter[] Parameters => new []
        {
            new Parameter("name", ParameterType.Optional), 
        };

        public bool Check(Command command, User user, Channel channel) => true;

        public async Task Do(CommandEventArgs args)
        {
            var parameter = args.GetArg("name");
            if (string.IsNullOrEmpty(parameter))
                await args.Channel.SendMessage($"Hello {args.Message.User.Mention}");
            else
                await args.Channel.SendMessage($"Hello {parameter}");
        }
    }
}
