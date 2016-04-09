using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.API.Model;
using System.Threading;
using System.Text.RegularExpressions;

namespace Discord.Bot.Modules
{
    public class BrainFuckModule : IMessageModule
    {
        Regex regex = new Regex(@"^```[\s]*#Brainfuck\n*([^`]*)```$");

        public void MessageGotten(DiscordBot bot, Message message)
        {
            var match = regex.Match(message.content);

            if (!match.Success)
                return;

            try
            {
                Interpret(match.Groups[1].Value, bot);
            }
            catch (Exception e)
            {
                bot.SendMessage(bot.Channel, e.Message);
                return;
            }
        }

        private void Interpret(string str, DiscordBot bot)
        {
            Message input = null;
            string output = "";

            int ic = 0;
            /** The memory pointer */
            int mp = 0;
            /** The instruction pointer */
            int ip = 0;
            /** The array that acts as the interpreter's virtual memory */
            char[] mem = new char[30000];

            /** The string containing the comands to be executed */
            char[] com = str.ToCharArray();

            int EOF = com.Length; //End Of File

            char c = '\0';

            while (ip < EOF && ic++ < 100000)
            {

                // Get the current command
                c = com[ip];

                // Act based on the current command and the brainfuck spec
                switch (c)
                {
                    case '>': mp++; break;
                    case '<': mp--; break;
                    case '+': mem[mp]++; break;
                    case '-': mem[mp]--; break;
                    case '.':
                        output += mem[mp];
                        break;
                    case ',':
                        while ((input = bot.GetLatestMessages(bot.Channel).LastOrDefault()) == null && input.content.Length != 0)
                            Thread.Sleep(5);

                        mem[mp] = input.content.Last();
                        break;
                    case '[':
                        if (mem[mp] == 0)
                        {
                            while (com[ip] != ']') ip++;
                        }
                        break;

                    case ']':
                        if (mem[mp] != 0)
                        {
                            while (com[ip] != '[') ip--;
                        }
                        break;
                }

                // increment instruction mp
                ip++;

            }

            if (output != "")
            {
                bot.SendMessage(bot.Channel, output);
            }
        }
    }
}
