using AESharp;
using Discord.Bot.Modules;
using DiscordSharp.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Bot.Modules.AESharpExtensions
{
    public class SendMsgFunc : DiscordSysFunc
    {
        public SendMsgFunc(Scope scope, DiscordChannel channel) 
            : base("sendmsg", scope, channel)
        {
            ValidArguments = new List<ArgumentType>()
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
