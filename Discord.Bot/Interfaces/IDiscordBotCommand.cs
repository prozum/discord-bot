namespace Discord.Bot.Interfaces
{
    /// <summary>
    /// An interface for implementing commands to the discord bot
    /// </summary>
    public interface IDiscordBotCommand
    {
        /// <summary>
        /// The number of arguments this command takes.
        /// <remarks>
        /// This is used to filter out command calls that uses the wrong number of arguments. If this is null
        /// it indicates that this command accepts a non constant number of arguments, and the bot should therefor
        /// not make the simple argument count check.
        /// </remarks>
        /// </summary>
        int? ArgumentCount { get; }

        /// <summary>
        /// The name of the command.
        /// </summary>
        string CommandName { get; }

        /// <summary>
        /// A string descripting the command, and how it works.
        /// </summary>
        string Help { get; }

        /// <summary>
        /// The function for executing the command.
        /// </summary>
        /// <param name="args">The arguments provided to the command. This includes the command itself, so args[0] is CommandPrefix + CommandName.</param>
        /// <param name="server">The server the command was called in.</param>
        /// <param name="channel">The channel the command was called in.</param>
        /// <param name="user">The user who called the command.</param>
        /// <param name="message">The message that called the command.</param>
        void Execute(string[] args, Server server, Channel channel, User user, Message message);
    }
}