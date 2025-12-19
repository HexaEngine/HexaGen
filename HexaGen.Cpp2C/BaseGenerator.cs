namespace HexaGen.Cpp2C
{
    using HexaGen.Core.Logging;

    public class BaseGenerator
    {
        protected readonly Cpp2CGeneratorConfig config;
        private readonly List<LogMessage> messages = new();

        public BaseGenerator(Cpp2CGeneratorConfig settings)
        {
            this.config = settings;
        }

        public IReadOnlyList<LogMessage> Messages => messages;

        public void Log(LogSeverity severtiy, string message)
        {
            if (severtiy < config.LogLevel) return;
            LogMessage msg = new(severtiy, message);
            messages.Add(msg);
            WriteMessage(msg);
        }

        public void LogTrace(string message)
        {
            Log(LogSeverity.Trace, message);
        }

        public void LogDebug(string message)
        {
            Log(LogSeverity.Debug, message);
        }

        public void LogInfo(string message)
        {
            Log(LogSeverity.Information, message);
        }

        public void LogWarn(string message)
        {
            Log(LogSeverity.Warning, message);
        }

        public void LogError(string message)
        {
            Log(LogSeverity.Error, message);
        }

        public void LogCritical(string message)
        {
            Log(LogSeverity.Critical, message);
        }

        public void DisplayMessages()
        {
            int warns = 0;
            int errors = 0;
            for (int i = 0; i < messages.Count; i++)
            {
                var msg = messages[i];
                WriteMessage(msg);
                if (msg.Severtiy == LogSeverity.Critical || msg.Severtiy == LogSeverity.Error) ++errors;
                else if (msg.Severtiy == LogSeverity.Warning) ++warns;
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

        private void WriteMessage(in LogMessage msg)
        {
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
                    break;

                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case LogSeverity.Critical:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
            }
            Console.WriteLine(msg.Message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}