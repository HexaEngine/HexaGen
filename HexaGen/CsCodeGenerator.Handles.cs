namespace HexaGen
{
    using CppAst;
    using HexaGen;
    using HexaGen.Core.CSharp;
    using System.IO;

    public partial class CsCodeGenerator
    {
        private readonly HashSet<string> LibDefinedTypedefs = new();

        public readonly HashSet<string> DefinedTypedefs = new();

        protected virtual void GenerateHandles(CppCompilation compilation, string outputPath)
        {
            string filePath = Path.Combine(outputPath, "Handles.cs");
            string[] usings = { "System", "System.Diagnostics", "System.Runtime.InteropServices", "HexaGen.Runtime" };
            // Generate Functions
            using var writer = new CodeWriter(filePath, settings.Namespace, usings.Concat(settings.Usings).ToArray());

            for (int i = 0; i < compilation.Typedefs.Count; i++)
            {
                CppTypedef typedef = compilation.Typedefs[i];
                if (settings.AllowedTypedefs.Count != 0 && !settings.AllowedTypedefs.Contains(typedef.Name))
                    continue;
                if (settings.IgnoredTypedefs.Contains(typedef.Name))
                    continue;
                if (LibDefinedTypedefs.Contains(typedef.Name))
                    continue;

                if (DefinedTypedefs.Contains(typedef.Name))
                {
                    LogWarn($"{filePath}: {typedef} is already defined!");
                    continue;
                }

                DefinedTypedefs.Add(typedef.Name);

                if (typedef.ElementType is CppPointerType pointerType && pointerType.ElementType is not CppFunctionType)
                {
                    var isDispatchable = true;
                    var csName = settings.GetCsCleanName(typedef.Name);
                    WriteHandle(writer, typedef, csName, isDispatchable);
                }
            }
        }

        private void WriteHandle(CodeWriter writer, CppTypedef typedef, string csName, bool isDispatchable)
        {
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