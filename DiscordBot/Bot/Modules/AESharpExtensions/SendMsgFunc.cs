using AESharp;
using Discord.Bot.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Bot.Modules.AESharpExtensions
{
    public class SendMsgFunc : DiscordSysFunc
    {
        public SendMsgFunc(Scope scope, DiscordBot bot) : base("sendmsg", scope, bot)
        {
            ValidArguments = new List<ArgumentType>()
            {
                ArgumentType.Expression,
                ArgumentType.Boolean
            };
        }

        public override Expression Call(List args)
        {
            var res = args[0].Evaluate().ToString();

            Bot.SendMessage(res, (args[1] as AESharp.Boolean).@bool);

            return new Text(res);
        }
    }
}
