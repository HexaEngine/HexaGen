using HexaGen.CppAst.Model.Templates;
using HexaGen.CppAst.Model.Types;

namespace HexaGen.Cpp2C.GenerationSteps
{
    using HexaGen.Core;
    using HexaGen.Cpp2C.Metadata;
    using HexaGen.CppAst.Model;
    using HexaGen.CppAst.Model.Declarations;
    using System;
    using System.Text;

    public class ClassGenerationStep : GenerationStep
    {
        private readonly HashSet<string> definedFunctions = [];
        private readonly HashSet<string> definedTypes = [];
        private List<CFunction> definedCFunctions = [];

        private readonly Dictionary<(CppClass, CppFunction), string> mapping = [];

        private static readonly string templateHeader = @"
#ifndef {PREFIX}COMMON_H
#define {PREFIX}COMMON_H

#include <stdio.h>
#include <stdint.h>
/* Calling convention */

#if defined(_WIN32) || defined(_WIN64)
#define {PREFIX}CALL __cdecl
#elif defined(__GNUC__) || defined(__clang__)
#define {PREFIX}CALL __attribute__((__cdecl__))
#else
#define {PREFIX}CALL
#endif

/* API export/import */
#if defined(_WIN32) || defined(_WIN64)
#ifdef {PREFIX}BUILD_SHARED
#define {PREFIX}EXPORT __declspec(dllexport)
#else
#define {PREFIX}EXPORT __declspec(dllimport)
#endif
#elif defined(__GNUC__) || defined(__clang__)
#ifdef {PREFIX}BUILD_SHARED
#define {PREFIX}EXPORT __attribute__((visibility(""default"")))
#else
#define {PREFIX}EXPORT
#endif
#else
#define {PREFIX}EXPORT
#endif

#if defined __cplusplus
#define {PREFIX}EXTERN extern ""C""
#else
#include <stdarg.h>
#include <stdbool.h>
#define {PREFIX}EXTERN extern
#endif

#define {PREFIX}API(type) {PREFIX}EXTERN {PREFIX}EXPORT type {PREFIX}CALL
#define {PREFIX}API_INTERNAL(type) {PREFIX}EXTERN type {PREFIX}CALL

#endif
";

        private static readonly string templateCpp = @"
#include ""Classes.h""
";

        public ClassGenerationStep(Cpp2CCodeGenerator generator, Cpp2CGeneratorConfig config) : base(generator, config)
        {
        }

        public override string Name { get; } = "Class Generation Step";

        public override void Configure(Cpp2CGeneratorConfig config)
        {
        }

        public override void CopyFromMetadata(Cpp2CGeneratorMetadata metadata)
        {
        }

        public override void CopyToMetadata(Cpp2CGeneratorMetadata metadata)
        {
        }

        public override void Reset()
        {
        }

        public override void Generate(FileSet files, ParseResult result, string outputPath, Cpp2CGeneratorConfig config, Cpp2CGeneratorMetadata metadata)
        {
            var compilation = result.Compilation;
            WriteCommon(outputPath, config);

            string headerName = $"Classes.h";
            string filePathHeader = Path.Combine(outputPath, "include", headerName);
            string filePathCpp = Path.Combine(outputPath, "src", $"Classes.cpp");
            using var headerWriter = new CodeWriter(filePathHeader, IncludeBuilder.Create().AddInclude("common.h").Build(), null);
            using var cppWriter = new CodeWriter(filePathCpp, IncludeBuilder.Create().AddInclude(headerName).Build(), null);

            WriteClasses(compilation.Classes, headerWriter, cppWriter);
            foreach (var ns in compilation.EnumerateNamespaces())
            {
                WriteClasses(ns.Classes, headerWriter, cppWriter);
            }
        }

        private static void WriteCommon(string outputPath, Cpp2CGeneratorConfig config)
        {
            string filePathHeader = Path.Combine(outputPath, "include", "common.h");
            using var headerWriter = new CodeWriter(filePathHeader, "", null);
            headerWriter.Write(templateHeader.Replace("{PREFIX}", config.NamePrefix));
        }

        private void WriteClasses(IEnumerable<CppClass> classes, ICodeWriter headerWriter, ICodeWriter cppWriter)
        {
            foreach (var c in classes)
            {
                if (c.ClassKind != CppClassKind.Class && c.ClassKind != CppClassKind.Struct)
                {
                    continue;
                }

                if (c.TemplateKind == CppTemplateKind.TemplateClass)
                {
                    continue;
                }

                if (c.SourceFile == null) continue;

                if (c.HasVirtualMembers())
                {
                    WriteClass(c, headerWriter, cppWriter);
                }
                else
                {
                    WriteDto(c, headerWriter, cppWriter);
                }
            }
        }

        private void WriteDto(CppClass cppClass, ICodeWriter headerWriter, ICodeWriter cppWriter)
        {
            headerWriter.BeginBlock($"typedef struct");
            if (cppClass.BaseTypes.Any())
            {
                Stack<(CppClass cls, bool close)> stack = new();
                stack.Push((cppClass, false));
                while (stack.Count > 0)
                {
                    var pair = stack.Pop();
                    if (pair.close)
                    {
                        WriteFields(headerWriter, pair.cls.Fields);
                    }
                    else
                    {
                        stack.Push((pair.cls, true));
                        foreach (var baseType in pair.cls.BaseTypes)
                        {
                            if (baseType.Type is CppClass baseClass)
                            {
                                stack.Push((baseClass, false));
                            }
                        }
                    }
                }
            }
            else
            {
                WriteFields(headerWriter, cppClass.Fields);
            }

            headerWriter.EndBlock($"}} {config.GetCTypeName(cppClass)};");
        }

        private void WriteClass(CppClass cppClass, ICodeWriter headerWriter, ICodeWriter cppWriter)
        {
            var typeName = config.GetCTypeName(cppClass);
            headerWriter.WriteLine($"typedef struct {typeName} {typeName};");
            headerWriter.WriteLine();
            headerWriter.WriteLine($"{config.NamePrefix}API({typeName}*) {typeName}Create();");
            headerWriter.WriteLine($"{config.NamePrefix}API(void) {typeName}Destroy({typeName}* self);");

            cppWriter.WriteLine($"{config.NamePrefix}API_INTERNAL({typeName}*) {typeName}Create()");

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

        private void WriteFields(ICodeWriter writer, IEnumerable<CppField> fields)
        {
            foreach (var field in fields)
            {
                if ((field.StorageQualifier & CppStorageQualifier.Static) != 0)
                {
                    continue;
                }
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
            string cName = $"{config.NamePrefix}{c.Name}_{f.Name}";
            string currentName = cName;
            while (definedFunctions.Contains(currentName))
            {
                currentName = $"{cName}{iter++}";
            }
            return currentName;
        }

        private void WriteFunctionH(CppClass c, CppFunction f, ICodeWriter writer)
        {
            string name = GetUniqueCFunctionName(c, f);
            string cSignature = GetCFunctionSignature(c, f);
            definedFunctions.Add(name);
            mapping.Add((c, f), name);

            definedCFunctions.Add(new(name, f.Name, f.Comment?.ToString()));

            writer.WriteLine($"{config.NamePrefix}API({f.ReturnType}) {name}({cSignature});");
        }

        private void WriteFunctionCpp(CppClass c, string typeName, CppFunction f, ICodeWriter writer)
        {
            var name = mapping[(c, f)];
            string cSignature = GetCFunctionSignature(c, f);
            string signature = GetCppFunctionSignatureTypeless(f);

            using (writer.PushBlock($"{config.NamePrefix}API_INTERNAL({f.ReturnType}) {name}({cSignature})"))
            {
                writer.WriteLine($"auto* ptr = reinterpret_cast<{c.Name}*>(self);");
                writer.WriteLine($"return ptr->{f.Name}({signature});");
            }
        }
    }
}