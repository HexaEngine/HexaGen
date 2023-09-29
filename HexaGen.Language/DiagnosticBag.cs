namespace HexaGen.Language
{
    using System;
    using System.Text;

    public class DiagnosticBag
    {
        private readonly List<DiagnosticMessage> messages = new();

        public IReadOnlyList<DiagnosticMessage> Messages => messages;

        public bool HasErrors { get; private set; }

        public void Info(string message, SourceLocation? location = null)
        {
            LogMessage(LogMessageType.Information, message, location);
        }

        public void Warning(string message, SourceLocation? location = null)
        {
            LogMessage(LogMessageType.Warning, message, location);
        }

        public void Error(string message, SourceLocation? location = null)
        {
            LogMessage(LogMessageType.Error, message, location);
        }

        public void Log(DiagnosticMessage message)
        {
            if (message.Type == LogMessageType.Error)
            {
                HasErrors = true;
            }

            messages.Add(message);
        }

        public void CopyTo(DiagnosticBag dest)
        {
            if (dest == null) throw new ArgumentNullException(nameof(dest));
            foreach (var diagnosticMessage in Messages)
            {
                dest.Log(diagnosticMessage);
            }
        }

        protected void LogMessage(LogMessageType type, string message, SourceLocation? location = null)
        {
            // Try to recover a proper location
            var locationResolved = location ?? new SourceLocation(); // In case we have an unexpected BuilderException, use this location instead
            Log(new DiagnosticMessage(type, message, locationResolved));
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var diagnostics = new StringBuilder();

            foreach (var message in Messages)
            {
                diagnostics.AppendLine(message.ToString());
            }

            return diagnostics.ToString();
        }
    }
}