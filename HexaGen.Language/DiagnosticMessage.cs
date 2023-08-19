namespace HexaGen.Language
{
    public struct DiagnosticMessage
    {
        public LogMessageType Type;
        public string Text;
        public SourceLocation Location;

        public DiagnosticMessage(LogMessageType type, string text, SourceLocation location)
        {
            Type = type;
            Text = text;
            Location = location;
        }

        public override string ToString()
        {
            return $"{Type}: {Text}, {Location}";
        }
    }
}