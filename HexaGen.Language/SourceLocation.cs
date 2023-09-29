namespace HexaGen.Language
{
    public readonly struct SourceLocation
    {
        /// <summary>
        /// Gets or sets the file associated with this location.
        /// </summary>
        public readonly string File;

        /// <summary>
        /// Gets or sets the char offset from the beginning of the file.
        /// </summary>
        public readonly int Offset;

        /// <summary>
        /// Gets or sets the line (start from 1) of this location.
        /// </summary>
        public readonly int Line;

        /// <summary>
        /// Gets or sets the column (start from 1) of this location.
        /// </summary>
        public readonly int Column;

        public SourceLocation(string file, int offset, int line, int column)
        {
            File = file;
            Offset = offset;
            Line = line;
            Column = column;
        }

        public override string ToString()
        {
            return $"{File} at line: {Line}, character: {Column}";
        }
    }
}