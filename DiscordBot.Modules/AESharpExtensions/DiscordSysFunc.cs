using AESharp;

namespace Discord.Bot.Modules.AESharpExtensions
{
    public abstract class DiscordSysFunc : SysFunc
    {
        public Channel Channel { get; set; }

        protected DiscordSysFunc(string name, Scope scope, Channel channel) 
            : base(name, scope)
        {
            Channel = channel;
        }
    }
}
