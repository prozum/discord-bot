﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Bot.Modules
{
    public interface ICommandable
    {
        bool TryRunCommand(string command, DiscordBot bot);
    }
}
