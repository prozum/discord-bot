using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using Discord.Bot.BaseModules;
using DiscordSharp.Events;
using DiscordSharp.Objects;

namespace Discord.Bot.Modules
{
    public class BrainFuckModule : BaseMessageModule
    {
        static readonly string _commandName = "#Æ#";

        public override void MessageReceived(object sender, DiscordMessageEventArgs e)
        {
            var channel = e.Channel;
            var message = e.Message;

            try
            {
                if (message.Content.StartsWith(_commandName))
                {
                    Interpret(message.Content.Remove(0, _commandName.Count()), channel);
                }
            }
            catch (Exception ex)
            {
                channel.SendMessage("Error!\n" + ex.Message);
            }
        }

        private void Interpret(string str, DiscordChannel channel)
        {
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
                channel.SendMessage(output);
            }
        }
    }
}
