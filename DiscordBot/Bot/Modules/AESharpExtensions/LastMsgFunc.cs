using AESharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Bot.Modules.AESharpExtensions
{
    public class LastMsgFunc : DiscordSysFunc
    {
        public LastMsgFunc(Scope scope, DiscordBot bot) : base("lastmsg", scope, bot)
        {
            ValidArguments = new List<ArgumentType>();
        }

        public override Expression Call(List args)
        {
            var msgs = Bot.GetMessages(limit: 2);
            return new Text(msgs[1].Content);
        }
    }
}
