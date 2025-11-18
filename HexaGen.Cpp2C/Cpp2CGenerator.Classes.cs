namespace HexaGen.Cpp2C
{
    using HexaGen.CppAst.Model;
    using HexaGen.CppAst.Model.Declarations;
    using HexaGen.CppAst.Model.Metadata;
    using System.Text;

    public partial class Cpp2CCodeGenerator
    {
        private readonly HashSet<string> definedFunctions = [];
        private readonly HashSet<string> definedTypes = [];

        private readonly Dictionary<(CppClass, CppFunction), string> mapping = [];

        private static readonly string templateHeader = @"
#ifndef {PREFIX}_COMMON_H
#define {PREFIX}_COMMON_H

#include <stdio.h>
#include <stdint.h>
/* Calling convention */

#if defined(_WIN32) || defined(_WIN64)
#define {PREFIX}_CALL __cdecl
#elif defined(__GNUC__) || defined(__clang__)
#define {PREFIX}_CALL __attribute__((__cdecl__))
#else
#define {PREFIX}_CALL
#endif

/* API export/import */
#if defined(_WIN32) || defined(_WIN64)
#ifdef {PREFIX}_BUILD_SHARED
#define {PREFIX}_EXPORT __declspec(dllexport)
#else
#define {PREFIX}_EXPORT __declspec(dllimport)
#endif
#elif defined(__GNUC__) || defined(__clang__)
#ifdef {PREFIX}_BUILD_SHARED
#define {PREFIX}_EXPORT __attribute__((visibility(""default"")))
#else
#define {PREFIX}_EXPORT
#endif
#else
#define {PREFIX}_EXPORT
#endif

#if defined __cplusplus
#define {PREFIX}_EXTERN extern ""C""
#else
#include <stdarg.h>
#include <stdbool.h>
#define {PREFIX}_EXTERN extern
#endif

#define {PREFIX}_API(type) {PREFIX}_EXTERN {PREFIX}_EXPORT type {PREFIX}_CALL
#define {PREFIX}_API_INTERNAL(type) {PREFIX}_EXTERN type {PREFIX}_CALL

#endif
";

        private static readonly string templateCpp = @"
#include ""Classes.h""
";

        public void GenerateClasses(CppCompilation compilation, string outputPath)
        {
            string filePathHeader = Path.Combine(outputPath, "include", "common.h");
            using var headerWriter = new CodeWriter(filePathHeader, "", null);
            headerWriter.Write(templateHeader.Replace("{PREFIX}", config.NamePrefix));

            for (int i = 0; i < compilation.Classes.Count; i++)
            {
                var c = compilation.Classes[i];

                if (c.ClassKind != CppClassKind.Class)
                {
                    continue;
                }

                WriteClass(c, outputPath);
            }
        }


        private void WriteClass(CppClass cppClass, string outputPath)
        {
            string headerName = $"{cppClass.Name}.h";
            string filePathHeader = Path.Combine(outputPath, "include", headerName);
            string filePathCpp = Path.Combine(outputPath, "src", $"{cppClass.Name}.cpp");
            using var headerWriter = new CodeWriter(filePathHeader, IncludeBuilder.Create().AddInclude("common.h").Build(), null);
            using var cppWriter = new CodeWriter(filePathCpp, IncludeBuilder.Create().AddInclude(headerName).AddInclude(Path.GetRelativePath(outputPath, cppClass.SourceFile)).Build(), null);

            var typeName = config.GetCTypeName(cppClass);
            headerWriter.WriteLine($"typedef struct {typeName} {typeName};");
            headerWriter.WriteLine();
            headerWriter.WriteLine($"{config.NamePrefix}_API({typeName}*) {typeName}_Create();");
            headerWriter.WriteLine($"{config.NamePrefix}_API(void) {typeName}_Destroy({typeName}* self);");

            cppWriter.WriteLine($"{config.NamePrefix}_API_INTERNAL({typeName}*) {typeName}_Create()");

            for (int j = 0; j < cppClass.Functions.Count; j++)
            {
                var f = cppClass.Functions[j];
                if (f.Visibility != CppVisibility.Public)
                {
                    continue;
                }


                WriteFunctionH(cppClass, f, headerWriter);
                WriteFunctionCpp(cppClass, typeName, f, cppWriter);
            }
        }

        private void WriteFields(CodeWriter writer, IEnumerable<CppField> fields)
        {
            foreach (var field in fields)
            {
                writer.WriteLine($"{config.GetCType(field.Type)} {field.Name};");
            }
        }

        private string GetCFunctionSignature(CppClass c, CppFunction f)
        {
            StringBuilder sb = new();

            sb.Append($"{config.GetCType(c)}* self");
            if (f.Parameters.Count > 0)
            {
                sb.Append(", ");
            }

            for (int i = 0; i < f.Parameters.Count; i++)
            {
                var param = f.Parameters[i];
                sb.Append($"{config.GetCType(param.Type)} {param.Name}");
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
            string cName = $"{config.NamePrefix}_{c.Name}_{f.Name}";
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

            writer.WriteLine($"{config.NamePrefix}_API({f.ReturnType}) {name}({cSignature});");
        }

        private void WriteFunctionCpp(CppClass c, string typeName, CppFunction f, CodeWriter writer)
        {
            var name = mapping[(c, f)];
            string cSignature = GetCFunctionSignature(c, f);
            string signature = GetCppFunctionSignatureTypeless(f);

            using (writer.PushBlock($"{config.NamePrefix}_API_INTERNAL({f.ReturnType}) {name}({cSignature})"))
            {
                writer.WriteLine($"auto* ptr = reinterpret_cast<{c.Name}*>(self);");
                writer.WriteLine($"return ptr->{f.Name}({signature});");
            }
        }
    }
}