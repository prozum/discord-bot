using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace JJerkBot.Modules
{
    public class BrainFuckCommand : ICommand
    {
        public string Name => "brainfuck";
        public string Description => "Interprets Brainfuck code.";
        public string[] Alias => new [] { "bfuck", "bf" };
        public bool Hide => false;
        public Parameter[] Parameters => new[]
        {
            new Parameter("code", ParameterType.Unparsed),
        };

        public bool Check(Command command, User user, Channel channel) => true;

        public async Task Do(CommandEventArgs args)
        {
            var output = new StringBuilder();

            var ic = 0;
            // The memory pointer
            var mp = 0;
            // The instruction pointer 
            var ip = 0;
            // The array that acts as the interpreter's virtual memory
            var mem = new List<char>();
            // The string containing the comands to be executed
            var com = args.GetArg("code");
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
                        if (mem.Count <= mp)
                            mem.AddRange(new char[mp - mem.Count]);

                        mem[mp]++; break;
                    case '-':
                        if (mem.Count <= mp)
                            mem.AddRange(new char[mp - mem.Count]);

                        mem[mp]--; break;
                    case '.':
                        output.Append(mem[mp]); break;
                    case '[':
                        if (mem.Count <= mp)
                            mem.AddRange(new char[mp - mem.Count]);

                        if (mem[mp] == 0)
                            while (com[ip] != ']') ip++;

                        break;
                    case ']':
                        if (mem.Count <= mp)
                            mem.AddRange(new char[mp - mem.Count]);

                        if (mem[mp] != 0)
                            while (com[ip] != '[') ip--;

                        break;
                    default:
                        output.Clear();
                        output.Append($"Error! Unknown symbol: {c}");
                        ip = eof;
                        break;
                }

                // increment instruction mp
                ip++;
            }

            if (output.Length > 0)
                await args.Channel.SendMessage(output.ToString());
        }
    }
}
