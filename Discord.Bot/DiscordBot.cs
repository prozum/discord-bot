using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using Discord.Bot.Interfaces;

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

            var arguments = ParseArguments(messageText);
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
        
        private static string[] ParseArguments(string str)
        {
            var pos = 0;
            var args = new List<string>();

            // HACK: Waiting for those local functions...
            Func<char> Eat = () => str[pos++];
            Func<char> Peek = () => pos < str.Length ? str[pos] : '\0';
            Action SkipWhiteSpace = () =>
            {
                while (char.IsWhiteSpace(Peek()))
                    Eat();
            };

            while (Peek() != '\0')
            {
                var strBuilder = new StringBuilder();

                SkipWhiteSpace();
                var startPos = pos;

                switch (Peek())
                {
                    case '"':
                        Eat();

                        // for ", we parse until the next " is found
                        while (Peek() != '"' && Peek() != '\0')
                            strBuilder.Append(Eat());

                        // if we never found an end, we just reset and parse " as part of a word
                        if (Peek() != '"')
                        {
                            strBuilder.Clear();
                            pos = startPos;
                            goto default;
                        }

                        args.Add(strBuilder.ToString());

                        Eat();
                        break;
                    case '`':
                        // ` is used for code blocks in discord, and we will therefor want the content
                        // of that codeblock as an argument
                        var startCount = 0;
                        var endCount = 0;

                        for (; Peek() == '`' && startCount < 3; startCount++)
                            Eat();

                        while (Peek() != '`' && Peek() != '\0')
                            strBuilder.Append(Eat());

                        // if a code block starts with 1 `, it's single line. If it starts with 3 `, it's multi line
                        for (; Peek() == '`' && endCount < (startCount == 3 ? 3 : 1); endCount++)
                            Eat();

                        // if we never found an end, we just reset and parse ` as part of a word
                        if (endCount == 0)
                        {
                            strBuilder.Clear();
                            pos = startPos;
                            goto default;
                        }

                        // a code block only works if it has content
                        if (strBuilder.Length == 0)
                        {
                            // we put all the eaten ` into the string
                            for (var i = startCount + endCount; i > 0; i--)
                                strBuilder.Append('`');

                            // we go to default, since a code block with no content is shown as normal text with the `
                            goto default;
                        }

                        // if the ending ` is not 3, we wonna return all except 1 ` to the argument
                        if (endCount != 3)
                        {
                            for (startCount--; startCount > 0; startCount--)
                                strBuilder.Insert(0, '`');
                        }

                        args.Add(strBuilder.ToString());
                        break;
                    default:
                        while (!char.IsWhiteSpace(Peek()) && Peek() != '\0')
                            strBuilder.Append(Eat());

                        args.Add(strBuilder.ToString());
                        break;
                }

                SkipWhiteSpace();
            }

            return args.ToArray();
        }
    }
}
