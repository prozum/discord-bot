using AESharp;
using DiscordSharp.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Bot.Modules.AESharpExtensions
{
    public abstract class DiscordSysFunc : SysFunc
    {
        public DiscordChannel Channel { get; set; }

        public DiscordSysFunc(string name, Scope scope, DiscordChannel channel) : base(name, scope)
        {
            Channel = channel;
        }
    }
}
