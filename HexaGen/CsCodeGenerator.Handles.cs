namespace HexaGen
{
    using CppAst;
    using System.Collections.Generic;
    using System.IO;

    public partial class CsCodeGenerator
    {
        protected readonly HashSet<string> LibDefinedTypedefs = new();
        public readonly HashSet<string> DefinedTypedefs = new();

        protected virtual List<string> SetupHandleUsings()
        {
            List<string> usings = new() { "System", "System.diagnostics", "System.Runtime.InteropServices", "HexaGen.Runtime" };
            usings.AddRange(settings.Usings);
            return usings;
        }

        protected virtual bool FilterHandle(GenContext context, CppTypedef typedef)
        {
            if (settings.AllowedTypedefs.Count != 0 && !settings.AllowedTypedefs.Contains(typedef.Name))
                return true;
            if (settings.IgnoredTypedefs.Contains(typedef.Name))
                return true;
            if (LibDefinedTypedefs.Contains(typedef.Name))
                return true;

            if (DefinedTypedefs.Contains(typedef.Name))
            {
                LogWarn($"{context.FilePath}: {typedef} is already defined!");
                return true;
            }

            DefinedTypedefs.Add(typedef.Name);

            if (typedef.ElementType is CppPointerType pointerType && pointerType.ElementType is not CppFunctionType)
            {
                return false;
            }

            return true;
        }

        protected virtual void GenerateHandles(CppCompilation compilation, string outputPath)
        {
            string filePath = Path.Combine(outputPath, "Handles.cs");

            // Generate Functions
            using var writer = new CodeWriter(filePath, settings.Namespace, SetupHandleUsings());
            GenContext context = new(compilation, filePath, writer);

            for (int i = 0; i < compilation.Typedefs.Count; i++)
            {
                WriteHandle(context, compilation.Typedefs[i], true);
            }
        }

        protected virtual void WriteHandle(GenContext context, CppTypedef typedef, bool isDispatchable)
        {
            if (FilterHandle(context, typedef))
                return;

            var writer = context.Writer;
            var csName = settings.GetCsCleanName(typedef.Name);

            LogInfo("defined handle " + csName);
            typedef.Comment.WriteCsSummary(writer);
            writer.WriteLine($"[NativeName(NativeNameType.Typedef, \"{typedef.Name}\")]");
            writer.WriteLine($"[DebuggerDisplay(\"{{DebuggerDisplay,nq}}\")]");
            using (writer.PushBlock($"public readonly partial struct {csName} : IEquatable<{csName}>"))
            {
                string handleType = isDispatchable ? "nint" : "ulong";
                string nullValue = "0";

                writer.WriteLine($"public {csName}({handleType} handle) {{ Handle = handle; }}");
                writer.WriteLine($"public {handleType} Handle {{ get; }}");
                writer.WriteLine($"public bool IsNull => Handle == 0;");

                writer.WriteLine($"public static {csName} Null => new {csName}({nullValue});");
                writer.WriteLine($"public static implicit operator {csName}({handleType} handle) => new {csName}(handle);");
                writer.WriteLine($"public static bool operator ==({csName} left, {csName} right) => left.Handle == right.Handle;");
                writer.WriteLine($"public static bool operator !=({csName} left, {csName} right) => left.Handle != right.Handle;");
                writer.WriteLine($"public static bool operator ==({csName} left, {handleType} right) => left.Handle == right;");
                writer.WriteLine($"public static bool operator !=({csName} left, {handleType} right) => left.Handle != right;");
                writer.WriteLine($"public bool Equals({csName} other) => Handle == other.Handle;");
                writer.WriteLine("/// <inheritdoc/>");
                writer.WriteLine($"public override bool Equals(object obj) => obj is {csName} handle && Equals(handle);");
                writer.WriteLine("/// <inheritdoc/>");
                writer.WriteLine($"public override int GetHashCode() => Handle.GetHashCode();");
                writer.WriteLine($"private string DebuggerDisplay => string.Format(\"{csName} [0x{{0}}]\", Handle.ToString(\"X\"));");
            }
            writer.WriteLine();
        }
    }
}