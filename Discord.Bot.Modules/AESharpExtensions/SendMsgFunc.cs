using AESharp;
using System.Collections.Generic;

namespace Discord.Bot.Modules.AESharpExtensions
{
    public class SendMsgFunc : DiscordSysFunc
    {
        public const string Name = "sendmsg";

        public SendMsgFunc(Scope scope, Channel channel) 
            : base(Name, scope, channel)
        {
            ValidArguments = new List<ArgumentType>
            {
                ArgumentType.Expression
            };
        }

        public override Expression Call(List args)
        {
            var res = args[0].Evaluate().ToString();
        
            Channel.SendMessage(res);

            return new Text(res);
        }
    }
}
