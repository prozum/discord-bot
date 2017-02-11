using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace JJerkBot
{
    public class Parameter
    {
        public string Name { get; }
        public ParameterType Type { get; }

        public Parameter(string name, ParameterType type = ParameterType.Required)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type;
        }
    }

    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        string[] Alias { get; }
        bool Hide { get; }
        Parameter[] Parameters { get; }

        bool Check(Command command, User user, Channel channel);
        Task Do(CommandEventArgs args);
    }
}
