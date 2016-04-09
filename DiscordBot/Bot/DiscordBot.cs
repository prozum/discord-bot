﻿using Discord.Bot.EventArguments;
using Discord.Bot.Modules;
using Discord.API;
using Discord.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Discord.Bot
{

    public class DiscordBot : DiscordClient
    {
        public Channel Channel { get; private set; }
        public Guild Guild { get; private set; }
        public List<IBotModule> Modules { get; set; }

        bool running;

        public DiscordBot()
            : this(new List<IBotModule>())
        { }

        public DiscordBot(List<IBotModule> modules)
        {
            Modules = modules;
        }

        public bool TryJoinChannelInGuild(string groupname, string channelname)
        {
            Guild = GetGuilds().FirstOrDefault(g => g.name == groupname);

            if (Guild == null)
                return false;

            var channels = GetGuildChannels(Guild);
            Channel = channels.FirstOrDefault(c => c.name == channelname);

            return Channel != null;
        }

        public void Run()
        {
            GetLatestMessages(Channel, limit: 1);
            running = true;

            while (running && Channel != null)
            {
                var messages = GetLatestMessages(Channel, limit: 5);

                foreach (var message in messages)
                {
                    RunMessageModules(message);
                }

                Thread.Sleep(100);
            }
        }

        public void Backup()
        {
            var settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Auto;

            using (var file = File.CreateText("bot.json"))
            {
                file.Write(JsonConvert.SerializeObject(this, Formatting.Indented, settings));
            }
        }

        public void Stop()
        {
            running = false;
        }

        public void RunMessageModules(Message message)
        {
            foreach (var module in Modules)
            {
                if (module is IMessageModule)
                    (module as IMessageModule).MessageGotten(this, message);
            }
        }

        public void AddModule(IBotModule module)
        {
            Modules.Add(module);
        }

        public void RemoveModule(IBotModule module)
        {
            Modules.Remove(module);
        }
    }
}
