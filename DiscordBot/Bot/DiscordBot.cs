using DiscordSharp;
using Discord.Bot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Discord.Bot.BaseModules;

namespace Discord.Bot
{
    public class DiscordBot
    {
        public List<BaseCommandModule> Commands { get; } = new List<BaseCommandModule>();
        public DiscordClient Client { get; }

        public DiscordBot(string token)
        {
            Client = new DiscordClient(token, true);
        }

        public void Start()
        {
            try
            {
                Console.WriteLine("Sending login request...");
                Client.SendLoginRequest();

                Console.WriteLine("Connecting client in separate thread...");
                Client.Connect();

                Console.WriteLine("Client connected!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong!\n" + e.Message + "\nPress any key to close this window.");
            }
        }

        public void AddCommand(BaseCommandModule command)
        {
            // we need to make sure we don't have dublicate commands
            if (Commands.Any(com => com.CommandName == command.CommandName))
                throw new NotImplementedException();

            Commands.Add(command);
            AddModule(command);
        }

        public void AddModule(IMessageModule module)
        {
            Client.MentionReceived             += module.MentionReceived;
            Client.MessageDeleted              += module.MessageDeleted;
            Client.MessageEdited               += module.MessageEdited;
            Client.MessageReceived             += module.MessageReceived;
            Client.PrivateMessageDeleted       += module.PrivateMessageDeleted;
            Client.PrivateMessageReceived      += module.PrivateMessageReceived;
            Client.UnknownMessageTypeReceived  += module.UnknownMessageTypeReceived;
            Client.URLMessageAutoUpdate        += module.UrlMessageAutoUpdate;
        }

        public void AddModule(IImplementsEverything module)
        {
            Client.AudioPacketReceived             += module.AudioPacketReceived;
            Client.BanRemoved                      += module.BanRemoved;
            Client.ChannelCreated                  += module.ChannelCreated;
            Client.ChannelDeleted                  += module.ChannelDeleted;
            Client.ChannelUpdated                  += module.ChannelUpdated;
            Client.Connected                       += module.Connected;
            Client.GuildAvailable                  += module.GuildAvailable;
            Client.GuildCreated                    += module.GuildCreated;
            Client.GuildDeleted                    += module.GuildDeleted;
            Client.GuildMemberBanned               += module.GuildMemberBanned;
            Client.GuildMemberUpdated              += module.GuildMemberUpdated;
            Client.GuildUpdated                    += module.GuildUpdated;
            Client.KeepAliveSent                   += module.KeepAliveSent;
            Client.MentionReceived                 += module.MentionReceived;
            Client.MessageDeleted                  += module.MessageDeleted;
            Client.MessageEdited                   += module.MessageEdited;
            Client.MessageReceived                 += module.MessageReceived;
            Client.PresenceUpdated                 += module.PresenceUpdated;
            Client.PrivateChannelCreated           += module.PrivateChannelCreated;
            Client.PrivateChannelDeleted           += module.PrivateChannelDeleted;
            Client.PrivateMessageDeleted           += module.PrivateMessageDeleted;
            Client.PrivateMessageReceived          += module.PrivateMessageReceived;
            Client.RoleDeleted                     += module.RoleDeleted;
            Client.RoleUpdated                     += module.RoleUpdated;
            Client.SocketClosed                    += module.SocketClosed;
            Client.SocketOpened                    += module.SocketOpened;
            Client.UnknownMessageTypeReceived      += module.UnknownMessageTypeReceived;
            Client.URLMessageAutoUpdate            += module.UrlMessageAutoUpdate;
            Client.UserAddedToServer               += module.UserAddedToServer;
            Client.UserLeftVoiceChannel            += module.UserLeftVoiceChannel;
            Client.UserRemovedFromServer           += module.UserRemovedFromServer;
            Client.UserSpeaking                    += module.UserSpeaking;
            Client.UserTypingStart                 += module.UserTypingStart;
            Client.UserUpdate                      += module.UserUpdate;
            Client.VoiceClientConnected            += module.VoiceClientConnected;
            Client.VoiceClientDebugMessageReceived += module.VoiceClientDebugMessageReceived;
            Client.VoiceQueueEmpty                 += module.VoiceQueueEmpty;
            Client.VoiceStateUpdate                += module.VoiceStateUpdate;
        }
    }
}
