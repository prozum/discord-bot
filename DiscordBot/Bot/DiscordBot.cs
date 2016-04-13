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
using RestSharp;

namespace Discord.Bot
{
	public delegate void NewMessageHandler(DiscordBot bot, Message msg);

    public class DiscordBot : DiscordClient
    {
		public string BotName;

        public Channel Channel { get; private set; }
        public Guild Guild { get; private set; }
        public List<IBotModule> Modules { get; set; }
		public event NewMessageHandler NewMessage;

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
            Guild = GetGuilds().FirstOrDefault(g => g.Name == groupname);

            if (Guild == null)
                return false;

            var channels = GetGuildChannels(Guild);
            Channel = channels.FirstOrDefault(c => c.Name == channelname);

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
					Console.WriteLine("Got message:" + message.Content);
					if (message.Author.Username != BotName)
						NewMessage(this, message);
                }
		

                Thread.Sleep(500);
            }
        }

        public bool SendMessage(string str, bool tts = false)
        {
            if (Channel == null)
                return false;

            return SendMessage(Channel, str, tts);
        }

        public List<Message> GetMessages(string before = null, string after = null, int limit = -1)
        {
            if (Channel == null)
                return new List<Message>();

            return GetMessages(Channel, before, after, limit);
        }

        public List<Message> GetLatestMessages(int limit = -1)
        {
            if (Channel == null)
                return new List<Message>();


            return GetLatestMessages(Channel, limit);
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

        public void AddMessageModule(IMessageModule module)
        {
			NewMessage += module.MessageGotten;
        }

        public void RemoveMessageModule(IBotModule module)
        {
            Modules.Remove(module);
        }
    }
}
