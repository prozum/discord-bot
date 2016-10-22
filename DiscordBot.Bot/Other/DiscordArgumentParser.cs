using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;

namespace Discord.Bot.Other
{
    public class DiscordArgumentParser
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
                var startPos = _pos;

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
                            _pos = startPos;
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
                            _pos = startPos;
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

        private void SkipWhiteSpace()
        {
            while (char.IsWhiteSpace(Peek()))
                Eat();
        }

        private char Peek() => _pos < _str.Length ? _str[_pos] : '\0';
        private char Eat() => _str[_pos++];
    }
}
