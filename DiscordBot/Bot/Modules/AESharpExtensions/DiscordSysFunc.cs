using AESharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Bot.Modules.AESharpExtensions
{
    public abstract class DiscordSysFunc : SysFunc
    {
        public DiscordBot Bot { get; set; }

        public DiscordSysFunc(string name, Scope scope, DiscordBot bot) : base(name, scope)
        {
            Bot = bot;
        }
    }
}
