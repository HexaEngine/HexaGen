namespace HexaGen.Cpp2C
{
    using CppAst;
    using System.Text;

    public class Function
    {
        public Function(string exportedName, string friendlyName, string? comment)
        {
            ExportedName = exportedName;
            FriendlyName = friendlyName;
            Comment = comment;
        }

        public string ExportedName { get; set; }

        public string FriendlyName { get; set; }

        public string? Comment { get; set; }
    }

    public class Metadata
    {
        public List<Function> Functions { get; } = new();
    }

    public partial class Cpp2CCodeGenerator
    {
        private readonly HashSet<string> definedFunctions = [];
        private readonly Dictionary<(CppClass, CppFunction), string> mapping = [];

        private static readonly string templateHeader = @"
#include <stdio.h>
#include <stdint.h>
#if defined _WIN32 || defined __CYGWIN__
    #define API __declspec(dllexport)
#else
    #ifdef __GNUC__
        #define API  __attribute__((__visibility__(""default"")))
    #else
        #define API
    #endif
#endif

#if defined __cplusplus
    #define EXTERN extern ""C""
#else
    #include <stdarg.h>
    #include <stdbool.h>
    #define EXTERN extern
#endif

#define _API EXTERN API
";

        private static readonly string templateCpp = @"
#include ""Classes.h""
";

        public void GenerateClasses(CppCompilation compilation, string outputPath)
        {
            string filePathHeader = Path.Combine(outputPath, "Classes.h");
            string filePathCpp = Path.Combine(outputPath, "Classes.cpp");
            using var headerWriter = new CodeWriter(filePathHeader, templateHeader, null);
            using var cppWriter = new CodeWriter(filePathCpp, templateCpp, null);

            for (int i = 0; i < compilation.Classes.Count; i++)
            {
                var c = compilation.Classes[i];

                if (c.ClassKind != CppClassKind.Class)
                {
                    continue;
                }

                for (int j = 0; j < c.Functions.Count; j++)
                {
                    var f = c.Functions[j];
                    if (f.Visibility != CppVisibility.Public)
                    {
                        continue;
                    }

                    WriteFunctionH(c, f, headerWriter);
                    WriteFunctionCpp(c, f, cppWriter);
                }
            }
        }

        private static string GetCFunctionSignature(CppClass c, CppFunction f)
        {
            StringBuilder sb = new();

            sb.Append($"{c.Name}* self");
            if (f.Parameters.Count > 0)
            {
                sb.Append(", ");
            }

            for (int i = 0; i < f.Parameters.Count; i++)
            {
                var param = f.Parameters[i];
                sb.Append($"{param.Type} {param.Name}");
                if (i < f.Parameters.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            return sb.ToString();
        }

        private static string GetCppFunctionSignature(CppFunction f)
        {
            StringBuilder sb = new();

            for (int i = 0; i < f.Parameters.Count; i++)
            {
                var param = f.Parameters[i];
                sb.Append($"{param.Type} {param.Name}");
                if (i < f.Parameters.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            return sb.ToString();
        }

        private static string GetCppFunctionSignatureTypeless(CppFunction f)
        {
            StringBuilder sb = new();

            for (int i = 0; i < f.Parameters.Count; i++)
            {
                var param = f.Parameters[i];
                sb.Append($"{param.Name}");
                if (i < f.Parameters.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            return sb.ToString();
        }

        private string GetUniqueCFunctionName(CppClass c, CppFunction f)
        {
            int iter = 0;
            string cName = $"{c.Name}_{f.Name}";
            string currentName = cName;
            while (definedFunctions.Contains(currentName))
            {
                currentName = $"{cName}{iter++}";
            }
            return currentName;
        }

        private void WriteFunctionH(CppClass c, CppFunction f, CodeWriter writer)
        {
            string name = GetUniqueCFunctionName(c, f);
            string cSignature = GetCFunctionSignature(c, f);
            definedFunctions.Add(name);
            mapping.Add((c, f), name);

            metadata.Functions.Add(new(name, f.Name, f.Comment?.ToString()));

            writer.WriteLine($"_API {f.ReturnType} {name}({cSignature});");
        }

        private void WriteFunctionCpp(CppClass c, CppFunction f, CodeWriter writer)
        {
            var name = mapping[(c, f)];
            string cSignature = GetCFunctionSignature(c, f);
            string signature = GetCppFunctionSignatureTypeless(f);

            using (writer.PushBlock($"{f.ReturnType} {name}({cSignature})"))
            {
                writer.WriteLine($"self->{f.Name}({signature});");
            }
        }
    }
}