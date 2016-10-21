using System;
using System.Collections.Generic;
using System.Text;
using DiscordSharp;
using DiscordSharp.Events;
using DiscordSharp.Objects;

namespace Discord.Bot.BaseModules
{
    public abstract class BaseCommandModule : BaseMessageModule
    {
        public const string CommandPrefix = "#";
        public const string HelpCommandName = "help";
        public const string HelpCommand = CommandPrefix + HelpCommandName;

        public DiscordBot Bot { get; }
        public abstract string CommandName { get; }
        public string Command => CommandPrefix + CommandName;
        public abstract string Help { get; }

        protected BaseCommandModule(DiscordBot bot)
        {
            Bot = bot;
        }

        public abstract void CommandCalled(string[] args, DiscordMember author,
            DiscordChannel channel, DiscordMessage message, DiscordMessageType messageType);

        public override void MentionReceived(object sender, DiscordMessageEventArgs e)
        {
            TryCallCommand(e.MessageText, e.Author, e.Channel, e.Message, e.MessageType);
        }

        protected bool TryCallCommand(string messageText, DiscordMember author, DiscordChannel channel, DiscordMessage message, DiscordMessageType messageType)
        {            
            // if the string is larger than the command length, then after the command there have to be a whitespace,
            // since after the command, the arguments for that command will start
            if (messageText.Length > Command.Length && !char.IsWhiteSpace(messageText[Command.Length]))
                return false;
            if (!messageText.StartsWith(Command))
                return false;

            var arguments = new ArgumentParser().ParseArguments(messageText.Remove(0, Command.Length));

            try
            {
                CommandCalled(arguments, author, channel, message, messageType);
            }
            catch (Exception ex)
            {
                channel.SendMessage($"```\nException was thrown while executing: {Command} {string.Join(" ", arguments)}!\n\n\n{ex.Message}\n```");
            }

            return true;
        }

        private class ArgumentParser
        {
            private string _str;
            private int _pos;

            public string[] ParseArguments(string str)
            {
                var args = new List<string>();

                _str = str;
                _pos = 0;

                while (Peek() != '\0')
                {
                    var strBuilder = new StringBuilder();

                    SkipWhiteSpace();

                    switch (Peek())
                    {
                        case '"':
                            Eat();

                            // for ", we parse until the next " is found
                            while (Peek() != '"')
                                strBuilder.Append(Eat());

                            Eat();

                            args.Add(strBuilder.ToString());
                            break;
                        case '`':
                            // ` is used for code blocks in discord, and we will therefor want the content
                            // of that codeblock as an argument
                            var startCount = 0;
                            var endCount = 0;

                            for (; Peek() == '`' && startCount < 3; startCount++)
                                Eat();

                            while (Peek() != '`')
                                strBuilder.Append(Eat());

                            // if a code block starts with 1 `, it's single line. If it starts with 3 `, it's multi line
                            for (; Peek() == '`' && endCount < (startCount == 3 ? 3 : 1); endCount++)
                                Eat();

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
                            while (!char.IsWhiteSpace(Peek()))
                                strBuilder.Append(Eat());

                            args.Add(strBuilder.ToString());
                            break;
                    }
                }

                return args.ToArray();
            }

            private void SkipWhiteSpace()
            {
                while (char.IsWhiteSpace(Peek()))
                    Eat();
            }

            private char Peek() => _pos < _str.Length ? _str[_pos] : '\0';
            private char Eat() => _pos < _str.Length ? _str[_pos++] : '\0';
        }
    }
}