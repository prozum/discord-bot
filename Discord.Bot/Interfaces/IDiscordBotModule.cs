namespace Discord.Bot.Interfaces
{
    public interface IDiscordBotModule
    {
        string ModuleName { get; }
        string ModuleDescription { get; }
    }
}
