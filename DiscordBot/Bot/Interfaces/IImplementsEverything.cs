using DiscordSharp;
using System;

namespace Discord.Bot.Interfaces
{
    public interface IImplementsEverything
    {
        void VoiceStateUpdate(object sender, DiscordVoiceStateUpdateEventArgs e);

        void VoiceQueueEmpty(object sender, EventArgs e);

        void VoiceClientDebugMessageReceived(object sender, LoggerMessageReceivedArgs e);

        void VoiceClientConnected(object sender, EventArgs e);

        void UserUpdate(object sender, DiscordSharp.Events.DiscordUserUpdateEventArgs e);

        void UserTypingStart(object sender, DiscordTypingStartEventArgs e);

        void UserSpeaking(object sender, DiscordSharp.Events.DiscordVoiceUserSpeakingEventArgs e);

        void UserRemovedFromServer(object sender, DiscordSharp.Events.DiscordGuildMemberRemovedEventArgs e);

        void UserLeftVoiceChannel(object sender, DiscordLeftVoiceChannelEventArgs e);

        void UserAddedToServer(object sender, DiscordSharp.Events.DiscordGuildMemberAddEventArgs e);

        void URLMessageAutoUpdate(object sender, DiscordURLUpdateEventArgs e);

        void UnknownMessageTypeReceived(object sender, DiscordSharp.Events.UnknownMessageEventArgs e);

        void SocketOpened(object sender, EventArgs e);

        void SocketClosed(object sender, DiscordSocketClosedEventArgs e);

        void RoleUpdated(object sender, DiscordGuildRoleUpdateEventArgs e);

        void RoleDeleted(object sender, DiscordGuildRoleDeleteEventArgs e);

        void KeepAliveSent(object sender, DiscordKeepAliveSentEventArgs e);

        void GuildUpdated(object sender, DiscordSharp.Events.DiscordServerUpdateEventArgs e);

        void GuildMemberUpdated(object sender, DiscordGuildMemberUpdateEventArgs e);

        void GuildMemberBanned(object sender, DiscordGuildBanEventArgs e);

        void GuildDeleted(object sender, DiscordSharp.Events.DiscordGuildDeleteEventArgs e);

        void GuildCreated(object sender, DiscordSharp.Events.DiscordGuildCreateEventArgs e);

        void GuildAvailable(object sender, DiscordSharp.Events.DiscordGuildCreateEventArgs e);

        void Connected(object sender, DiscordConnectEventArgs e);

        void ChannelUpdated(object sender, DiscordSharp.Events.DiscordChannelUpdateEventArgs e);

        void ChannelDeleted(object sender, DiscordSharp.Events.DiscordChannelDeleteEventArgs e);

        void ChannelCreated(object sender, DiscordChannelCreateEventArgs e);

        void BanRemoved(object sender, DiscordBanRemovedEventArgs e);

        void AudioPacketReceived(object sender, DiscordAudioPacketEventArgs e);

        void PrivateMessageReceived(object sender, DiscordPrivateMessageEventArgs e);

        void PrivateMessageDeleted(object sender, DiscordSharp.Events.DiscordPrivateMessageDeletedEventArgs e);

        void PrivateChannelDeleted(object sender, DiscordSharp.Events.DiscordPrivateChannelDeleteEventArgs e);

        void PrivateChannelCreated(object sender, DiscordPrivateChannelEventArgs e);

        void PresenceUpdated(object sender, DiscordPresenceUpdateEventArgs e);

        void MessageReceived(object sender, DiscordSharp.Events.DiscordMessageEventArgs e);

        void MessageEdited(object sender, DiscordSharp.Events.DiscordMessageEditedEventArgs e);

        void MessageDeleted(object sender, DiscordSharp.Events.DiscordMessageDeletedEventArgs e);

        void MentionReceived(object sender, DiscordSharp.Events.DiscordMessageEventArgs e);
    }
}
