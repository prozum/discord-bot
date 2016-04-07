using Discord.Bot.EventArguments;
using Discord.Bot.Modules;
using Discord.API;
using Discord.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Bot
{
    public class DiscordBot
    {
        public DiscordClient Client { get; set; }
        public Channel Channel { get; private set; }
        public Guild Guild { get; private set; }

        List<IBotModule> modules = new List<IBotModule>();

        public DiscordBot()
        {
            Client = new DiscordClient();
        }

        public DiscordBot(DiscordClient client)
        {
            Client = client;
        }

        public bool TryJoinChannelInGuild(string groupname, string channelname)
        {
            Guild = Client.GetGuilds().FirstOrDefault(g => g.name == groupname);

            if (Guild == null)
                return false;

            var channels = Client.GetGuildChannels(Guild);
            Channel = channels.FirstOrDefault(c => c.name == channelname);
            
            return Channel != null;
        }

        public void Run()
        {
            while (Channel != null)
            {
                var messages = Client.GetLatestMessages(Channel, limit: 5);

                foreach (var message in messages)
                {
                    foreach (var module in modules)
                    {
                        if (module is IMessageModule)
                            (module as IMessageModule).MessageGotten(this, message);
                    }
                }

                Thread.Sleep(50);
            }
        }

        public void AddModule(IBotModule module)
        {
            modules.Add(module);
        }

        public void RemoveModule(IBotModule module)
        {
            modules.Remove(module);
        }
    }
}
