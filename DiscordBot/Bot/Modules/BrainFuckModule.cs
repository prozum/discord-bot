using System;
using Discord.Bot.BaseModules;
using DiscordSharp;
using DiscordSharp.Events;
using DiscordSharp.Objects;

namespace Discord.Bot.Modules
{
    public class BrainFuckModule : BaseCommandModule
    {
        public BrainFuckModule(DiscordBot bot) 
            : base(bot)
        { }

        public override string CommandName => "bfuck";
        public override string Help =>
@"Interprets Brainfuck code.

Arguments:
    Takes 1 argument.
    Has to be valid Brainfuck code.";

        public override void CommandCalled(string[] args, DiscordMember author, DiscordChannel channel, DiscordMessage message,
            DiscordMessageType messageType)
        {
            if (args.Length != 1)
                return;
            
            var output = "";

            var ic = 0;
            // The memory pointer
            var mp = 0;
            // The instruction pointer 
            var ip = 0;
            // The array that acts as the interpreter's virtual memory
            var mem = new char[30000];
            // The string containing the comands to be executed
            var com = args[0];
            //End Of File
            var eof = com.Length;


            while (ip < eof && ic++ < 100000)
            {
                // Get the current command
                var c = com[ip];

                // Act based on the current command and the brainfuck spec
                switch (c)
                {
                    case '>':
                        mp++; break;
                    case '<':
                        mp--; break;
                    case '+':
                        mem[mp]++; break;
                    case '-':
                        mem[mp]--; break;
                    case '.':
                        output += mem[mp]; break;
                    case '[':
                        if (mem[mp] == 0)
                            while (com[ip] != ']') ip++;

                        break;
                    case ']':
                        if (mem[mp] != 0)
                            while (com[ip] != '[') ip--;

                        break;
                    default:
                        output = "Error! Unknown symbol: " + c;
                        ip = eof;
                        break;
                }

                // increment instruction mp
                ip++;

            }

            if (output != "")
                channel.SendMessage(output);
        }
    }
}
