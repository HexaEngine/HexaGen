namespace HexaGen
{
    using HexaGen.Core.Logging;
    using System;
    using System.Collections.Generic;

    public delegate void LogEventHandler(LogSeverity severity, string message);

    public class LoggerBase
    {
        private readonly List<LogMessage> messages = new();

        public IReadOnlyList<LogMessage> Messages => messages;

        public event LogEventHandler? LogEvent;

        public LogSeverity LogLevel { get; set; } = LogSeverity.Information;

        public void DisplayMessages()
        {
            int warns = 0;
            int errors = 0;
            for (int i = 0; i < messages.Count; i++)
            {
                var msg = messages[i];
                switch (msg.Severtiy)
                {
                    case LogSeverity.Trace:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;

                    case LogSeverity.Debug:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;

                    case LogSeverity.Information:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;

                    case LogSeverity.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        warns++;
                        break;

                    case LogSeverity.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        errors++;
                        break;

                    case LogSeverity.Critical:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        errors++;
                        break;
                }
                Console.WriteLine(messages[i]);
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine();
            Console.Write($"summary: ");
            if (warns > 0)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else
                Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"warnings: {warns}, ");
            if (errors > 0)
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"errors: {errors}\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            messages.Clear();
        }

        public void Log(LogSeverity severtiy, string message)
        {
            if (severtiy < LogLevel) return;
            messages.Add(new LogMessage(severtiy, message));
            LogEvent?.Invoke(severtiy, message);
        }

        public void LogCritical(string message)
        {
            Log(LogSeverity.Critical, message);
        }

        public void LogDebug(string message)
        {
            Log(LogSeverity.Debug, message);
        }

        public void LogError(string message)
        {
            Log(LogSeverity.Error, message);
        }

        public void LogInfo(string message)
        {
            Log(LogSeverity.Information, message);
        }

        public void LogTrace(string message)
        {
            Log(LogSeverity.Trace, message);
        }

        public void LogWarn(string message)
        {
            Log(LogSeverity.Warning, message);
        }

        public void LogToConsole()
        {
            LogEvent += GeneratorLogEvent;
        }

        private static void GeneratorLogEvent(LogSeverity severity, string message)
        {
            switch (severity)
            {
                case LogSeverity.Trace:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;

                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;

                case LogSeverity.Information:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;

                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case LogSeverity.Critical:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
            }

            string type = severity switch
            {
                LogSeverity.Trace => "[Trc] ",
                LogSeverity.Debug => "[Dbg] ",
                LogSeverity.Information => "[Inf] ",
                LogSeverity.Warning => "[Wrn] ",
                LogSeverity.Error => "[Err] ",
                LogSeverity.Critical => "[Crt] ",
                _ => "[Unk] ",
            };

            Console.Write(type);
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}