using System;


namespace Cacti
{
    public static class Logger
    {
        public enum LogLevel
        {
            Silent,
            Minimal,
            Verbose,
            Debug
        }

        public static LogLevel Level { get; set; } = LogLevel.Minimal;

        public static void Log(string msg = "", LogLevel lvl = LogLevel.Minimal, bool newline = true)
        {
            if (Level == LogLevel.Silent)
                return;

            if (lvl <= Level)
                Console.Write(msg + ((newline) ? "\n" : ""));
        }
    }
}
