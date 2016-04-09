using AESharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Bot.Modules.AESharpExtensions
{
    public class GetMsgFunc : DiscordSysFunc
    {
        public GetMsgFunc(Scope scope, DiscordBot bot) : base("getmsg", scope, bot)
        {
            ValidArguments = new List<ArgumentType>()
            {
                ArgumentType.Integer
            };
        }

        public override Expression Call(List args)
        {
            var msgs = Bot.GetMessages(limit: (int)(args[0] as Integer).@int);
            var res = new List();

            foreach (var item in msgs)
            {
                res.Items.Add(new Text(item.Content));
            }

            return res;
        }
    }
}
