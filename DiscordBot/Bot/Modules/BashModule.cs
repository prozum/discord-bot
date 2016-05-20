using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Discord.Bot.BaseModules;
using DiscordSharp.Events;
using DiscordSharp;
using DiscordSharp.Objects;

namespace Discord.Bot.Modules
{
	public class BashModule : BaseMessageModule
    {
		static readonly Regex regex = new Regex(@"#!([^`]*)\n([^`]*)");
		string helpmessage = "Interpreters:\n" +
							 "\tbash     Bourne-again shell\n" +
							 "\tzsh      Z shell\n" +
							 "\tpython   Python 2\n" +
						     "\tpython3  Python 3\n" +
			                 "\truby     Ruby\n" +
							 "\tnode     Nodejs (Javascript)\n" +
							 "\tphp      PHP\n" +
							 "\tperl     Perl\n" +
			                 "\tcsharp   C#\n" +
			                 "\tcling    C/C++\n";

        public override void MessageReceived(object sender, DiscordMessageEventArgs e)
        {
            var channel = e.Channel;
            var message = e.Message;

            if (message.Content.StartsWith("#!help"))
            {
                channel.SendMessage(helpmessage);
                return;
            }

            var match = regex.Match(message.Content);

            if (!match.Success)
                return;

            Interpret(match.Groups[1].Value, match.Groups[2].Value, channel);
        }

        private void Interpret(string interpreter, string str, DiscordChannel channel)
		{
			var filename = "/tmp/" + Guid.NewGuid().ToString();
			System.IO.File.WriteAllText(filename, "#!/usr/bin/env " + interpreter + "\n" + str);

			Process proc = new System.Diagnostics.Process();
			proc.StartInfo.WorkingDirectory = "/home/sandbox";
			proc.StartInfo.FileName = "/bin/bash";
			proc.StartInfo.Arguments = "-c \"chmod +x " + filename + "; sudo -u sandbox " + filename + "\"";
			proc.StartInfo.UseShellExecute = false; 
			proc.StartInfo.RedirectStandardOutput = true;
			proc.StartInfo.RedirectStandardError = true;
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

