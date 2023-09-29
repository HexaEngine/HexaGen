namespace HexaGen.Core
{
    using System;
    using System.Buffers.Binary;
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Compression;
    using System.Runtime.InteropServices;
    using System.Text;

    public class CharCaseInsensitiveEqualityComparer : IEqualityComparer<char>
    {
        public static readonly CharCaseInsensitiveEqualityComparer Default = new();

        public bool Equals(char x, char y)
        {
            return char.ToLower(x) == char.ToLower(y);
        }

        public int GetHashCode([DisallowNull] char obj)
        {
            return char.ToLower(obj).GetHashCode();
        }
    }

    public class WordList
    {
        private readonly TrieStringSet words = new(CharCaseInsensitiveEqualityComparer.Default);

        public static readonly WordList EN_EN = new("en_EN");

        public WordList()
        {
        }

        public WordList(string countryCode)
        {
            ReadTxt(countryCode + ".txt");
        }

        public static unsafe void CreateFromTxt(string source, string destination)
        {
            var lines = File.ReadAllLines(source);
            var fs = File.Create(destination);
            fs.Position = 4;

            byte* buffer = (byte*)Marshal.AllocHGlobal(4096);
            int bufferSize = 4096;
            var span = new Span<byte>(buffer, bufferSize);

            DeflateStream stream = new(fs, CompressionLevel.Optimal, true);

            HashSet<string> words = new(lines.Length);

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if (words.Contains(line) || line.Length == 1)
                {
                    continue;
                }

                words.Add(line);

                int size = Encoding.Unicode.GetByteCount(line);
                if (size + 4 > bufferSize)
                {
                    bufferSize = (size + 4) * 2;
                    buffer = (byte*)Marshal.ReAllocHGlobal((nint)buffer, bufferSize);
                    span = new Span<byte>(buffer, bufferSize);
                }
                BinaryPrimitives.WriteInt32LittleEndian(span, size);
                var written = Encoding.Unicode.GetBytes(line, span[4..]);
                stream.Write(span[..(written + 4)]);
            }

            stream.Close();

            fs.Position = 0;
            BinaryPrimitives.WriteInt32LittleEndian(span, words.Count);
            fs.Write(span[..4]);

            Marshal.FreeHGlobal((nint)buffer);
            fs.Close();
        }

        public unsafe void Read(string path)
        {
            var fs = File.OpenRead(path);

            byte* buffer = (byte*)Marshal.AllocHGlobal(4096);
            int bufferSize = 4096;
            var span = new Span<byte>(buffer, bufferSize);

            fs.Read(span[..4]);
            var count = BinaryPrimitives.ReadInt32LittleEndian(span);

            DeflateStream stream = new(fs, CompressionMode.Decompress);
            for (int i = 0; i < count; i++)
            {
                stream.Read(span[..4]);
                var size = BinaryPrimitives.ReadInt32LittleEndian(span);
                if (size + 4 > bufferSize)
                {
                    bufferSize = (size + 4) * 2;
                    buffer = (byte*)Marshal.ReAllocHGlobal((nint)buffer, bufferSize);
                    span = new Span<byte>(buffer, bufferSize);
                }
                stream.Read(span[..size]);
                var word = Encoding.Unicode.GetString(span[..size]);
                if (!words.Contains(word))
                {
                    words.Add(word);
                }
            }

            Marshal.FreeHGlobal((nint)buffer);

            stream.Close();
            fs.Close();
        }

        public unsafe void ReadTxt(string path)
        {
            var lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (!words.Contains(line))
                {
                    words.Add(line);
                }
            }
        }

        public unsafe void Write(string path)
        {
            /*
            var fs = File.Create(path);

            byte* buffer = (byte*)Marshal.AllocHGlobal(4096);
            int bufferSize = 4096;
            var span = new Span<byte>(buffer, bufferSize);

            BinaryPrimitives.WriteInt32LittleEndian(span, words.Count);
            fs.Write(span[..4]);

            DeflateStream stream = new(fs, CompressionLevel.Optimal);

            foreach (var word in words)
            {
                var line = word.Key;
                int size = Encoding.Unicode.GetByteCount(line);
                if (size + 4 > bufferSize)
                {
                    bufferSize = (size + 4) * 2;
                    buffer = (byte*)Marshal.ReAllocHGlobal((nint)buffer, bufferSize);
                    span = new Span<byte>(buffer, bufferSize);
                }
                BinaryPrimitives.WriteInt32LittleEndian(span, size);
                var written = Encoding.Unicode.GetBytes(line, span[4..]);
                stream.Write(span[..(written + 4)]);
            }

            Marshal.FreeHGlobal((nint)buffer);
            stream.Close();
            fs.Close();*/
        }

        public unsafe void WriteTxt(string path)
        {
            /*
            var fs = File.Create(path);

            byte* buffer = (byte*)Marshal.AllocHGlobal(4096);
            int bufferSize = 4096;
            var span = new Span<byte>(buffer, bufferSize);

            foreach (var word in words)
            {
                var line = word.Key;
                int size = Encoding.Unicode.GetByteCount(line);
                if (size + 4 > bufferSize)
                {
                    bufferSize = (size + 4) * 2;
                    buffer = (byte*)Marshal.ReAllocHGlobal((nint)buffer, bufferSize);
                    span = new Span<byte>(buffer, bufferSize);
                }
                var written = Encoding.Unicode.GetBytes(line, span);
                written += Encoding.Unicode.GetBytes(Environment.NewLine, span[written..]);
                fs.Write(span[..written]);
            }

            Marshal.FreeHGlobal((nint)buffer);
            fs.Close();*/
        }

        public static WordList ReadFrom(string path)
        {
            WordList list = new();
            list.Read(path);
            return list;
        }

        public static WordList ReadFromTxt(string path)
        {
            WordList list = new();
            list.ReadTxt(path);

            return list;
        }

        public ReadOnlySpan<char> FindMostMatching(ReadOnlySpan<char> text)
        {
            return words.FindLargestMatch(text);
        }

        public string[] SplitWords(string text)
        {
            List<string> words = new();

            int index = 0;
            while (index < text.Length)
            {
                var word = this.words.FindLargestMatch(text.AsSpan(index));
                if (word.Length == 0)
                {
                    index++;
                    continue;
                }

                words.Add(word.ToString());
                index += word.Length;
            }

            return words.ToArray();
        }
    }

    public class WordListCollection
    {
    }
}