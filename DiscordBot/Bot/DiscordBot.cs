using Discord.Bot.Modules;
using System.Collections.Generic;
using DiscordSharp;
using Discord.Bot.Interfaces;
using System;

namespace Discord.Bot
{
    public class DiscordBot
    {
        DiscordClient _client;

        public DiscordBot(string token)
        {
            _client = new DiscordClient(token, true);
        }

        public void Start()
        {
            try
            {
                Console.WriteLine("Sending login request...");
                _client.SendLoginRequest();

                Console.WriteLine("Connecting client in separate thread...");
                _client.Connect();

                Console.WriteLine("Client connected!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong!\n" + e.Message + "\nPress any key to close this window.");
            }
        }

        public void AddModule(IMessageModule module)
        {
            _client.MentionReceived             += module.MentionReceived;
            _client.MessageDeleted              += module.MessageDeleted;
            _client.MessageEdited               += module.MessageEdited;
            _client.MessageReceived             += module.MessageReceived;
            _client.PrivateMessageDeleted       += module.PrivateMessageDeleted;
            _client.PrivateMessageReceived      += module.PrivateMessageReceived;
            _client.UnknownMessageTypeReceived  += module.UnknownMessageTypeReceived;
            _client.URLMessageAutoUpdate        += module.URLMessageAutoUpdate;
        }

        public void AddModule(IImplementsEverything module)
        {
            _client.AudioPacketReceived             += module.AudioPacketReceived;
            _client.BanRemoved                      += module.BanRemoved;
            _client.ChannelCreated                  += module.ChannelCreated;
            _client.ChannelDeleted                  += module.ChannelDeleted;
            _client.ChannelUpdated                  += module.ChannelUpdated;
            _client.Connected                       += module.Connected;
            _client.GuildAvailable                  += module.GuildAvailable;
            _client.GuildCreated                    += module.GuildCreated;
            _client.GuildDeleted                    += module.GuildDeleted;
            _client.GuildMemberBanned               += module.GuildMemberBanned;
            _client.GuildMemberUpdated              += module.GuildMemberUpdated;
            _client.GuildUpdated                    += module.GuildUpdated;
            _client.KeepAliveSent                   += module.KeepAliveSent;
            _client.MentionReceived                 += module.MentionReceived;
            _client.MessageDeleted                  += module.MessageDeleted;
            _client.MessageEdited                   += module.MessageEdited;
            _client.MessageReceived                 += module.MessageReceived;
            _client.PresenceUpdated                 += module.PresenceUpdated;
            _client.PrivateChannelCreated           += module.PrivateChannelCreated;
            _client.PrivateChannelDeleted           += module.PrivateChannelDeleted;
            _client.PrivateMessageDeleted           += module.PrivateMessageDeleted;
            _client.PrivateMessageReceived          += module.PrivateMessageReceived;
            _client.RoleDeleted                     += module.RoleDeleted;
            _client.RoleUpdated                     += module.RoleUpdated;
            _client.SocketClosed                    += module.SocketClosed;
            _client.SocketOpened                    += module.SocketOpened;
            _client.UnknownMessageTypeReceived      += module.UnknownMessageTypeReceived;
            _client.URLMessageAutoUpdate            += module.URLMessageAutoUpdate;
            _client.UserAddedToServer               += module.UserAddedToServer;
            _client.UserLeftVoiceChannel            += module.UserLeftVoiceChannel;
            _client.UserRemovedFromServer           += module.UserRemovedFromServer;
            _client.UserSpeaking                    += module.UserSpeaking;
            _client.UserTypingStart                 += module.UserTypingStart;
            _client.UserUpdate                      += module.UserUpdate;
            _client.VoiceClientConnected            += module.VoiceClientConnected;
            _client.VoiceClientDebugMessageReceived += module.VoiceClientDebugMessageReceived;
            _client.VoiceQueueEmpty                 += module.VoiceQueueEmpty;
            _client.VoiceStateUpdate                += module.VoiceStateUpdate;
        }
    }
}
