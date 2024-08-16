namespace HexaGen.Core
{
    using System.Text;

    public class FileNameHelper
    {
        private static readonly Dictionary<char, string> replacements = new()
        {
            { '*', "Star" },
            { ':', "Colon" },
            { '<', "LessThan" },
            { '>', "GreaterThan" },
            { '|', "Pipe" },
            { '?', "QuestionMark" },
            { '"', "Quote" },
            { '/', "Slash" },
            { '\\', "Backslash" }
        };

        private static readonly string[] reservedNames = [
            "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4",
            "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2",
            "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
        ];

        private static readonly char[] invalidChars = Path.GetInvalidFileNameChars();

        public static string SanitizeFileName(string fileName)
        {
            var sb = new StringBuilder(fileName.Length);

            // Replace characters based on the dictionary
            foreach (char c in fileName)
            {
                if (replacements.TryGetValue(c, out string? replacement))
                {
                    sb.Append(replacement);
                }
                else if (!invalidChars.Contains(c))
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append('_'); // Replace invalid characters with '_'
                }
            }

            fileName = sb.ToString();

            // Handle reserved names (Windows-specific)
            foreach (string reservedName in reservedNames)
            {
                if (string.Equals(fileName, reservedName, StringComparison.OrdinalIgnoreCase))
                {
                    fileName = $"_{fileName}";
                    break;
                }
            }

            // Limit the length to 255 characters
            return fileName.Length > 255 ? fileName[..255] : fileName;
        }
    }
}