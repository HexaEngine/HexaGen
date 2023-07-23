namespace HexaGen
{
	using CppAst;
	using HexaGen.Core.Mapping;
	using System.Collections.Generic;
	using System.Linq;

    public partial class CsComCodeGenerator
    {
        private readonly HashSet<string> LibDefinedTypes = new();

        public readonly HashSet<string> DefinedTypes = new();

        private void GenerateTypes(CppCompilation compilation, string outputPath)
        {
            // Print All classes, structs
            string filePath = Path.Combine(outputPath, "Structures.cs");
            string[] usings = { "System", "System.Diagnostics", "System.Runtime.CompilerServices", "System.Runtime.InteropServices", "HexaGen.Runtime", "HexaGen.Runtime.COM" };

            // Generate Structures
            using var writer = new CodeWriter(filePath, settings.Namespace, usings.Concat(settings.Usings).ToArray());

            // Print All classes, structs
            for (int i = 0; i < compilation.Classes.Count; i++)
            {
                CppClass? cppClass = compilation.Classes[i];
                if (settings.AllowedTypes.Count != 0 && !settings.AllowedTypes.Contains(cppClass.Name))
                    continue;
                if (settings.IgnoredTypes.Contains(cppClass.Name))
                    continue;
                if (LibDefinedTypes.Contains(cppClass.Name))
                    continue;

                if (DefinedTypes.Contains(cppClass.Name))
                {
                    LogWarn($"{filePath}: {cppClass} is already defined!");
                    continue;
                }

                DefinedTypes.Add(cppClass.Name);

                string csName = settings.GetCsCleanName(cppClass.Name);

                var mapping = settings.GetTypeMapping(cppClass.Name);

                csName = mapping?.FriendlyName ?? csName;

                WriteClass(writer, cppClass, mapping, csName);
            }
        }

        private void WriteClass(CodeWriter writer, CppClass cppClass, TypeMapping? mapping, string csName)
        {
            bool isCOM = TryGetGUID(cppClass.Name, out var guid);

            if (isCOM)
            {
                writer.WriteLine($"[Guid(\"{guid}\")]");
            }

            using (writer.PushBlock($"public partial struct {csName}"))
            {
                if (isCOM)
                {
                    writer.WriteLine("public unsafe void** VtlbPtr;");

                    for (int i = 0; i < cppClass.Functions.Count; i++)
                    {
                        var func = cppClass.Functions[i];
                        var functionName = settings.GetCsCleanName(func.Name);
                        WriteCOMFunction(func, i, functionName);
                    }
                }
                else
                {
                }
            }
        }

        private void WriteCOMFunction(CppFunction function, int index, string csName)
        {
        }
    }
}