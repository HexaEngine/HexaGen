namespace HexaGen
{
    using HexaGen.Core.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class BaseGenerator
    {
        protected readonly CsCodeGeneratorSettings settings;
        private readonly List<LogMessage> messages = new();

        public BaseGenerator(CsCodeGeneratorSettings settings)
        {
            this.settings = settings;
            settings.TypeMappings.Add("HRESULT", "HResult");
        }

        public IReadOnlyList<LogMessage> Messages => messages;

        public void Log(LogSevertiy severtiy, string message)
        {
            if (severtiy >= settings.LogLevel)
                messages.Add(new LogMessage(severtiy, message));
        }

        public void LogTrace(string message)
        {
            Log(LogSevertiy.Trace, message);
        }

        public void LogDebug(string message)
        {
            Log(LogSevertiy.Debug, message);
        }

        public void LogInfo(string message)
        {
            Log(LogSevertiy.Information, message);
        }

        public void LogWarn(string message)
        {
            Log(LogSevertiy.Warning, message);
        }

        public void LogError(string message)
        {
            Log(LogSevertiy.Error, message);
        }

        public void LogCritical(string message)
        {
            Log(LogSevertiy.Critical, message);
        }

        public void DisplayMessages()
        {
            int warns = 0;
            int errors = 0;
            for (int i = 0; i < messages.Count; i++)
            {
                var msg = messages[i];
                switch (msg.Severtiy)
                {
                    case LogSevertiy.Trace:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;

                    case LogSevertiy.Debug:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;

                    case LogSevertiy.Information:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;

                    case LogSevertiy.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        warns++;
                        break;

                    case LogSevertiy.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        errors++;
                        break;

                    case LogSevertiy.Critical:
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
    }
}