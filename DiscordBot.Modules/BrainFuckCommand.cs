using Discord.Bot.Interfaces;

namespace Discord.Bot.Modules
{
    public class BrainFuckCommand : IDiscordBotCommand
    {
        public int? ArgumentCount => 1;
        public string CommandName => "bfuck";
        public string Help =>
@"Interprets Brainfuck code.

Arguments:
    Takes 1 argument.
    Has to be valid Brainfuck code.";

        public void Execute(string[] args, Server server, Channel channel, User user, Message message)
        {
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
