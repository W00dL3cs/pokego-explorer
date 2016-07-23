using PokemonGo.RocketAPI.Logging;
using System;

namespace PokemonGo.RocketAPI
{
    public static class LogColor
    {
        public static ConsoleColor Black = ConsoleColor.Black;
        public static ConsoleColor Blue = ConsoleColor.Blue;
        public static ConsoleColor Cyan = ConsoleColor.Cyan;
        public static ConsoleColor DarkBlue = ConsoleColor.DarkBlue;
        public static ConsoleColor DarkCyan = ConsoleColor.DarkCyan;
        public static ConsoleColor DarkGray = ConsoleColor.DarkGray;
        public static ConsoleColor DarkGreen = ConsoleColor.DarkGreen;
        public static ConsoleColor DarkMagenta = ConsoleColor.DarkMagenta;
        public static ConsoleColor DarkRed = ConsoleColor.DarkRed;
        public static ConsoleColor DarkYellow = ConsoleColor.DarkYellow;

        public static ConsoleColor Gray = ConsoleColor.Gray;
        public static ConsoleColor Green = ConsoleColor.Green;
        public static ConsoleColor Magenta = ConsoleColor.Magenta;
        public static ConsoleColor Error = ConsoleColor.Red;
        public static ConsoleColor White = ConsoleColor.White;
        public static ConsoleColor Yellow = ConsoleColor.Yellow;
    }

    /// <summary>
    /// Generic logger which can be used across the projects.
    /// Logger should be set to properly log.
    /// </summary>
    public static class Logger
	{
		private static ILogger logger;

		/// <summary>
		/// Set the logger. All future requests to <see cref="Write(string, LogLevel)"/> will use that logger, any old will be unset.
		/// </summary>
		/// <param name="logger"></param>
		public static void SetLogger(ILogger logger)
		{
			Logger.logger = logger;
		}

        /// <summary>
        /// write message to log window and file
        /// </summary>
        /// <param name="message">message text</param>
        public static void Normal(string message)
        {
            logger.Write(message);
        }

        /// <summary>
        /// Log a specific message to the logger setup by <see cref="SetLogger(ILogger)"/> .
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="level">Optional level to log. Default <see cref="LogLevel.Info"/>.</param>
        public static void Normal(ConsoleColor color, string message)
		{
			if (logger == null)
				return;
            ConsoleColor originalColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color;
            logger.Write(message);
            System.Console.ForegroundColor = originalColor;
        }

        /// <summary>
        /// Log a specific message to the logger setup by <see cref="SetLogger(ILogger)"/> .
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="level">Optional level to log. Default <see cref="LogLevel.Info"/>.</param>
        public static void Error(string message)
        {
            if (logger == null)
                return;
            ConsoleColor originalColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Red;
            logger.Write(message);
            System.Console.ForegroundColor = originalColor;
        }

    }

	public enum LogLevel
	{
		None = 0,
		Error = 1,
		Warning = 2,
		Info = 3,
		Debug = 4
	}
}