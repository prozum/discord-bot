using System;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Discord.Bot.BaseModules;
using DiscordSharp;
using DiscordSharp.Events;
using DiscordSharp.Objects;

namespace Discord.Bot.Modules
{
	public class BashModule : BaseCommandModule
    {
        public BashModule(DiscordBot bot) 
            : base(bot)
        { }

        public override string CommandName => "bash";
        public override string Help =>
@"Can interpret the following code:
    bash     Bourne-again shell 
    zsh      Z shell
    python   Python 2
    python3  Python 3 
    ruby     Ruby
    node     Nodejs (Javascript) 
    php      PHP
    perl     Perl
    csharp   C#
    cling    C/C++

Arguments:
    2 arguments.
    The first argument should be one of the things stated above.
    The second argument should be the code to interpret";

        public override void CommandCalled(string[] args, DiscordMember author, DiscordChannel channel, DiscordMessage message,
            DiscordMessageType messageType)
        {
            var filename = $"/tmp/{Guid.NewGuid()}";
            File.WriteAllText(filename, $"#!/usr/bin/env {args[0]}\n{args[1]}");

            var proc = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = "/home/sandbox",
                    FileName = "/bin/bash",
                    Arguments = $"-c \"chmod +x {filename}; sudo -u sandbox {filename}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            //proc.StartInfo.RedirectStandardInput = true;
            proc.OutputDataReceived += (s, e) => channel.SendMessage(e.Data);
            proc.ErrorDataReceived += (s, e) => channel.SendMessage(e.Data);
            /*bot.NewMessage += (DiscordBot _, Message msg) => { 
				if (!proc.HasExited)
					proc.StandardInput.Write(msg.Content);
			};*/

            proc.Start();
            proc.BeginErrorReadLine();
            proc.BeginOutputReadLine();

            //proc.WaitForExit(10000);
        }
    }
}

