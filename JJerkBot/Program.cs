using System;
using System.Reflection;
using JJerkBot.Modules;
using JJerkBot.Properties;

namespace JJerkBot.Application
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new Bot('#');
            
            var iCommand = typeof(ICommand);
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var t in a.GetTypes())
                {
                    if (iCommand.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null)
                        bot.AddCommand(Activator.CreateInstance(t) as ICommand);
                }

            }

            // Rest of the commands, that do not have an empty constructor
            bot.AddCommand(new WallpaperCommand(Resources.WallAlphacodersKey));
            bot.AddCommand(new AnimeCommand(Resources.AnilistId, Resources.AnilistSecret));

            bot.Start(Resources.BotToken);
            Environment.Exit(0);
        }
    }
}
