using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Discord.Bot.Interfaces;
using Discord.Bot.Other;

namespace Discord.Bot
{
    public class DiscordBot
    {
        public Dictionary<string, IDiscordBotCommand> Commands { get; } = new Dictionary<string, IDiscordBotCommand>();
        public List<IDiscordBotModule> Modules { get; } = new List<IDiscordBotModule>();
        public DiscordClient Client { get; } = new DiscordClient();
        public string CommandPrefix { get; }

        public DiscordBot(string commandPrefix)
        {
            CommandPrefix = commandPrefix;
            Client.MessageReceived += Client_MessageReceived;
        }

        private void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            var messageText = e.Message.Text;

            if (!messageText.StartsWith(CommandPrefix))
                return;

            var arguments = new DiscordArgumentParser().ParseArguments(messageText);
            IDiscordBotCommand command;

            if (Commands.TryGetValue(arguments[0].Remove(0, 1), out command))
            {
                try
                {
                    if (command.ArgumentCount != null && command.ArgumentCount != arguments.Length - 1)
                        return;

                    command.Execute(arguments, e.Server, e.Channel, e.User, e.Message);
                }
                catch (Exception ex)
                {
                    e.Channel.SendMessage($"```\nException was thrown while executing: {CommandPrefix}{command.CommandName} {string.Join(" ", arguments)}!\n\n\n{ex.Message}\n```");
                }
            }
        }

        public void Start(string token)
        {
            try
            {
                Console.WriteLine("Connecting client...");
                Client.ExecuteAndWait(async () => {
                    await Client.Connect(token, TokenType.Bot);
                    Console.WriteLine("Client connected!");
                });
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong!\n" + e.Message + "\nPress any key to close this window.");
            }
        }

        public void AddCommand(IDiscordBotCommand command)
        {
            Commands.Add(command.CommandName, command);

            if (command is IDiscordBotModule)
                AddModule((IDiscordBotModule)command);
        }

        public void AddModule(IDiscordBotModule module)
        {
            // HACK: C# 7 is gonna be great
            if (module is IMessageReceivedModule)
                Client.MessageReceived += ((IMessageReceivedModule)module).MessageReceived;
            if (module is IJoinedServerModule)
                Client.JoinedServer += ((IJoinedServerModule)module).JoinedServer;
            if (module is ILeftServerModule)
                Client.LeftServer += ((ILeftServerModule)module).LeftServer;
            if (module is IMessageDeletedModule)
                Client.MessageDeleted += ((IMessageDeletedModule)module).MessageDeleted;
            if (module is IMessageUpdatedModule)
                Client.MessageUpdated += ((IMessageUpdatedModule)module).MessageUpdated;
            if (module is IProfileUpdatedModule)
                Client.ProfileUpdated += ((IProfileUpdatedModule)module).ProfileUpdated;
            if (module is IReadyModule)
                Client.Ready += ((IReadyModule)module).Ready;
            if (module is IRoleCreatedModule)
                Client.RoleCreated += ((IRoleCreatedModule)module).RoleCreated;
            if (module is IRoleUpdatedModule)
                Client.RoleUpdated += ((IRoleUpdatedModule)module).RoleUpdated;
            if (module is IChannelCreatedModule)
                Client.ChannelCreated += ((IChannelCreatedModule)module).ChannelCreated;
            if (module is IChannelDestroyedModule)
                Client.ChannelDestroyed += ((IChannelDestroyedModule)module).ChannelDestroyed;
            if (module is IUserUpdatedModule)
                Client.UserUpdated += ((IUserUpdatedModule)module).UserUpdated;
            if (module is IUserUnbannedModule)
                Client.UserUnbanned += ((IUserUnbannedModule)module).UserUnbanned;
            if (module is IUserLeftModule)
                Client.UserLeft += ((IUserLeftModule)module).UserLeft;
            if (module is IUserJoinedModule)
                Client.UserJoined += ((IUserJoinedModule)module).UserJoined;
            if (module is IUserIsTypingModule)
                Client.UserIsTyping += ((IUserIsTypingModule)module).UserIsTyping;
            if (module is IUserBannedModule)
                Client.UserBanned += ((IUserBannedModule)module).UserBanned;
            if (module is IServerUpdatedModule)
                Client.ServerUpdated += ((IServerUpdatedModule)module).ServerUpdated;
            if (module is IServerUnavailableModule)
                Client.ServerUnavailable += ((IServerUnavailableModule)module).ServerUnavailable;
            if (module is IServerAvailableModule)
                Client.ServerAvailable += ((IServerAvailableModule)module).ServerAvailable;
            if (module is IChannelUpdatedModule)
                Client.ChannelUpdated += ((IChannelUpdatedModule)module).ChannelUpdated;

            Modules.Add(module);
        }
    }
}
