using AESharp;
using DiscordSharp.Objects;

namespace Discord.Bot.Modules.AESharpExtensions
{
    public abstract class DiscordSysFunc : SysFunc
    {
        public DiscordChannel Channel { get; set; }

        protected DiscordSysFunc(string name, Scope scope, DiscordChannel channel) 
            : base(name, scope)
        {
            Channel = channel;
        }
    }
}
