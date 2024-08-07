namespace HexaGen
{
    using ClangSharp;
    using CppAst;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class CsHandleMetadata
    {
        public CsHandleMetadata(string name, CppTypedef cppType, string? comment, bool isDispatchable)
        {
            Name = name;
            CppType = cppType;
            Comment = comment;
            IsDispatchable = isDispatchable;
        }

        public string Name { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public CppTypedef CppType { get; set; }

        public string? Comment { get; set; }

        public bool IsDispatchable { get; set; }
    }

    public partial class CsCodeGenerator
    {
        protected readonly HashSet<string> LibDefinedTypedefs = new();
        public readonly HashSet<string> DefinedTypedefs = new();

        protected virtual List<string> SetupHandleUsings()
        {
            List<string> usings = ["System", "System.Diagnostics", "System.Runtime.InteropServices", "HexaGen.Runtime", .. settings.Usings];
            return usings;
        }

        protected virtual bool FilterHandle(GenContext? context, CsHandleMetadata csHandle)
        {
            var typedef = csHandle.CppType;
            if (settings.AllowedTypedefs.Count != 0 && !settings.AllowedTypedefs.Contains(typedef.Name))
                return true;

            if (settings.IgnoredTypedefs.Contains(typedef.Name))
                return true;

            if (LibDefinedTypedefs.Contains(typedef.Name))
                return true;

            if (DefinedTypedefs.Contains(csHandle.Name))
            {
                LogWarn($"{context?.FilePath}: {typedef} is already defined!");
                return true;
            }

            DefinedTypedefs.Add(csHandle.Name);

            if (typedef.ElementType is CppPointerType pointerType && pointerType.ElementType is not CppFunctionType)
            {
                return false;
            }

            return true;
        }

        protected virtual void GenerateHandles(CppCompilation compilation, string outputPath)
        {
            string folder = Path.Combine(outputPath, "Handles");
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
            Directory.CreateDirectory(folder);

            if (settings.OneFilePerType)
            {
                for (int i = 0; i < compilation.Typedefs.Count; i++)
                {
                    var typedef = compilation.Typedefs[i];
                    var handle = ParseHandle(typedef);
                    if (FilterHandle(null, handle))
                        continue;

                    string filePath = Path.Combine(folder, $"{handle.Name}.cs");
                    using var writer = new CsCodeWriter(filePath, settings.Namespace, SetupHandleUsings(), settings.HeaderInjector);
                    GenContext context = new(compilation, filePath, writer);
                    WriteHandle(context, handle);
                }
            }
            else
            {
                string filePath = Path.Combine(folder, "Handles.cs");
                using var writer = new CsSplitCodeWriter(filePath, settings.Namespace, SetupHandleUsings(), settings.HeaderInjector, 1);
                GenContext context = new(compilation, filePath, writer);

                for (int i = 0; i < compilation.Typedefs.Count; i++)
                {
                    var typedef = compilation.Typedefs[i];
                    var handle = ParseHandle(typedef);
                    if (FilterHandle(context, handle))
                        continue;
                    WriteHandle(context, handle);
                    if (i + 1 != compilation.Typedefs.Count)
                    {
                        writer.WriteLine();
                    }
                }
            }
        }

        protected virtual CsHandleMetadata ParseHandle(CppTypedef typedef)
        {
            var csName = settings.GetCsCleanName(typedef.Name);
            CsHandleMetadata metadata = new(csName, typedef, null, true);

            settings.TryGetHandleMapping(typedef.Name, out var mapping);

            metadata.Comment = settings.WriteCsSummary(typedef.Comment);
            if (mapping != null)
            {
                if (mapping.Comment != null)
                {
                    metadata.Comment = settings.WriteCsSummary(mapping.Comment);
                }
                if (mapping.FriendlyName != null)
                {
                    metadata.Name = mapping.FriendlyName;
                }
            }

            return metadata;
        }

        protected virtual void WriteHandle(GenContext context, CsHandleMetadata csHandle)
        {
            var writer = context.Writer;

            LogInfo("defined handle " + (string?)csHandle.Name);

            writer.WriteLines(csHandle.Comment);
            if (settings.GenerateMetadata)
            {
                writer.WriteLine($"[NativeName(NativeNameType.Typedef, \"{csHandle.CppType.Name}\")]");
            }
            writer.WriteLine("#if NET5_0_OR_GREATER");
            writer.WriteLine($"[DebuggerDisplay(\"{{DebuggerDisplay,nq}}\")]");
            writer.WriteLine("#endif");
            using (writer.PushBlock($"public readonly partial struct {csHandle.Name} : IEquatable<{csHandle.Name}>"))
            {
                string handleType = csHandle.IsDispatchable ? "nint" : "ulong";
                string nullValue = "0";

                writer.WriteLine($"public {csHandle.Name}({handleType} handle) {{ Handle = handle; }}");
                writer.WriteLine($"public {handleType} Handle {{ get; }}");
                writer.WriteLine($"public bool IsNull => Handle == 0;");

                writer.WriteLine($"public static {csHandle.Name} Null => new {csHandle.Name}({nullValue});");
                writer.WriteLine($"public static implicit operator {csHandle.Name}({handleType} handle) => new {csHandle.Name}(handle);");
                writer.WriteLine($"public static bool operator ==({csHandle.Name} left, {csHandle.Name} right) => left.Handle == right.Handle;");
                writer.WriteLine($"public static bool operator !=({csHandle.Name} left, {csHandle.Name} right) => left.Handle != right.Handle;");
                writer.WriteLine($"public static bool operator ==({csHandle.Name} left, {handleType} right) => left.Handle == right;");
                writer.WriteLine($"public static bool operator !=({csHandle.Name} left, {handleType} right) => left.Handle != right;");
                writer.WriteLine($"public bool Equals({csHandle.Name} other) => Handle == other.Handle;");
                writer.WriteLine("/// <inheritdoc/>");
                writer.WriteLine($"public override bool Equals(object obj) => obj is {csHandle.Name} handle && Equals(handle);");
                writer.WriteLine("/// <inheritdoc/>");
                writer.WriteLine($"public override int GetHashCode() => Handle.GetHashCode();");
                writer.WriteLine("#if NET5_0_OR_GREATER");
                writer.WriteLine($"private string DebuggerDisplay => string.Format(\"{csHandle.Name} [0x{{0}}]\", Handle.ToString(\"X\"));");
                writer.WriteLine("#endif");
            }
        }
    }
}