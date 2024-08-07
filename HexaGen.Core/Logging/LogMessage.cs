namespace HexaGen.Core.Logging
{
    public struct LogMessage
    {
        public LogSeverity Severtiy;
        public string Message;

        public LogMessage(LogSeverity severtiy, string message)
        {
            Severtiy = severtiy;
            Message = message;
        }

        private static string GetServertiyString(LogSeverity severtiy)
        {
            return severtiy switch
            {
                LogSeverity.Trace => "[Trc]",
                LogSeverity.Debug => "[Dbg]",
                LogSeverity.Information => "[Inf]",
                LogSeverity.Warning => "[Wrn]",
                LogSeverity.Error => "[Err]",
                LogSeverity.Critical => "[Crt]",
                _ => throw new NotImplementedException(),
            };
        }

        public override readonly string ToString()
        {
            return $"{GetServertiyString(Severtiy)}\t{Message}";
        }
    }
}