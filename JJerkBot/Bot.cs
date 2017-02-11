using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using Discord.Logging;

namespace JJerkBot
{
    public class Bot
    {
        private readonly DiscordClient _client = new DiscordClient();
        private readonly CommandService _commandService;

        public Bot(char commandPrefix)
        {
            _client.UsingCommands(x =>
            {
                x.PrefixChar = commandPrefix;
                x.HelpMode = HelpMode.Public;
            });

            _commandService = _client.GetService<CommandService>();
        }
        
        public void Start(string token)
        {
            try
            {
                // TODO: Use logger
                Console.WriteLine("Connecting client...");
                _client.ExecuteAndWait(async () => {
                    await _client.Connect(token, TokenType.Bot);
                    Console.WriteLine("Client connected!");
                });
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong!\n" + e.Message + "\nPress any key to close this window.");
            }
        }

        public void AddCommand(ICommand command)
        {
            if (command.Name == null)
                throw new ArgumentNullException(nameof(command.Name));

            var builder = _commandService
                .CreateCommand(command.Name)
                .AddCheck(command.Check);

            if (command.Alias != null)
                builder = builder.Alias(command.Alias);

            if (command.Description != null)
                builder = builder.Description(command.Description);

            if (command.Parameters != null)
            {
                foreach (var parameter in command.Parameters)
                    if (parameter != null)
                        builder = builder.Parameter(parameter.Name, parameter.Type);
            }

            if (command.Hide)
                builder = builder.Hide();

            builder.Do(command.Do);
        }
        
        // TODO: Not used code, but i think it is a solid implementation of a simple function that splits a string into arguments
        private static string[] ParseArguments(string str)
        {
            var pos = 0;
            var args = new List<string>();
            var argumentBuilder = new StringBuilder();

            // local helper functions
            char Eat() => str[pos++];
            char Peek() => pos < str.Length ? str[pos] : '\0';
            void SkipWhiteSpace()
            {
                while (char.IsWhiteSpace(Peek()))
                    Eat();
            }

            void ParseIdentifier()
            {
                while (!char.IsWhiteSpace(Peek()) && Peek() != '\0')
                    argumentBuilder.Append(Eat());
            }

            while (Peek() != '\0')
            {
                SkipWhiteSpace();
                var startPos = pos;

                switch (Peek())
                {
                    case '"':
                        Eat();

                        // for ", we parse until the next " is found
                        while (Peek() != '"' && Peek() != '\0')
                            argumentBuilder.Append(Eat());

                        // if we never found an end, we just reset and parse " as part of a word
                        if (Peek() != '"')
                        {
                            argumentBuilder.Clear();
                            pos = startPos;
                            ParseIdentifier();
                        }
                        
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
                            argumentBuilder.Append(Eat());

                        // if a code block starts with 1 `, it's single line. If it starts with 3 `, it's multi line
                        for (; Peek() == '`' && endCount < (startCount == 3 ? 3 : 1); endCount++)
                            Eat();

                        // if we never found an end, we just reset and parse ` as part of a word
                        if (endCount == 0)
                        {
                            argumentBuilder.Clear();
                            pos = startPos;
                            ParseIdentifier();
                        }

                        // a code block only works if it has content
                        if (argumentBuilder.Length == 0)
                        {
                            // we put all the eaten ` into the string
                            for (var i = startCount + endCount; i > 0; i--)
                                argumentBuilder.Append('`');

                            // we go to default, since a code block with no content is shown as normal text with the `
                            ParseIdentifier();
                        }

                        // if the ending ` is not 3, we wonna return all except 1 ` to the argument
                        if (endCount != 3)
                        {
                            for (startCount--; startCount > 0; startCount--)
                                argumentBuilder.Insert(0, '`');
                        }
                        break;
                    default:
                        ParseIdentifier();
                        break;
                }

                args.Add(argumentBuilder.ToString());
                argumentBuilder.Clear();
            }

            return args.ToArray();
        }
    }
}
