namespace HexaGen.Core.Logging
{
    public struct LogMessage
    {
        public LogSevertiy Severtiy;
        public string Message;

        public LogMessage(LogSevertiy severtiy, string message)
        {
            Severtiy = severtiy;
            Message = message;
        }

        private static string GetServertiyString(LogSevertiy severtiy)
        {
            return severtiy switch
            {
                LogSevertiy.Trace => "[Trc]",
                LogSevertiy.Debug => "[Dbg]",
                LogSevertiy.Information => "[Inf]",
                LogSevertiy.Warning => "[Wrn]",
                LogSevertiy.Error => "[Err]",
                LogSevertiy.Critical => "[Crt]",
                _ => throw new NotImplementedException(),
            };
        }

        public override readonly string ToString()
        {
            return $"{GetServertiyString(Severtiy)}\t{Message}";
        }
    }
}