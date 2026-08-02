using HexaGen.CppAst.Model.Templates;
using HexaGen.CppAst.Model.Types;

namespace HexaGen.Cpp2C.GenerationSteps
{
    using ClangSharp.Interop;
    using HexaGen.Core;
    using HexaGen.Cpp2C.GenerationSteps.ParameterWriters;
    using HexaGen.Cpp2C.Metadata;
    using HexaGen.CppAst.Model;
    using HexaGen.CppAst.Model.Declarations;
    using System;
    using System.Text;

    public class ClassGenerationStep : GenerationStep
    {
        private readonly HashSet<string> definedFunctions = [];
        private readonly List<CFunction> definedCFunctions = [];

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
            using var headerWriter = CodeWriter.CreateFile(filePathHeader, IncludeBuilder.Create().AddInclude("common.h").AddInclude("enums.h").Build(), null);
            using var cppWriter = CodeWriter.CreateFile(filePathCpp, IncludeBuilder.Create().AddSystemInclude("HexaGenRuntime.hpp").AddInclude(headerName).AddIncludes(config.Includes).Build(), null);

            TypeGraph graph = new();

            foreach (var c in compilation.Classes)
            {
                var node = graph.GetOrCreate(c);
            }
            foreach (var c in compilation.TemplateSpecializations)
            {
                var node = graph.GetOrCreate(c);
                node.Flags |= TypeNodeFlags.TemplateSpecialization;
            }

            foreach (var ns in compilation.EnumerateNamespaces())
            {
                foreach (var c in ns.Classes)
                {
                    var node = graph.GetOrCreate(c);
                }
                foreach (var c in ns.TemplateSpecializations)
                {
                    var node = graph.GetOrCreate(c);
                    node.Flags |= TypeNodeFlags.TemplateSpecialization;
                }
            }

            Stack<CppClass> stack = new();
            foreach (var n in graph.Nodes)
            {
                stack.Push(n.Type);
                while (stack.TryPop(out var c))
                {
                    foreach (var b in c.BaseTypes)
                    {
                        if (b.Type is CppClass cppClass)
                        {
                            graph.TryAddDependency(n, cppClass);
                        }
                    }
                    foreach (var sub in c.Classes)
                    {
                        stack.Push(sub);
                    }
                    foreach (var field in c.Fields)
                    {
                        var type = field.Type.TryFlattenByValue();
                        if (type != null)
                        {
                            graph.TryAddDependency(n, type);
                        }
                    }
                    foreach (var arg in c.TemplateSpecializedArguments)
                    {
                        var type = arg.ArgAsType.TryFlattenByValue();
                        if (type != null)
                        {
                            graph.TryAddDependency(n, type);
                        }
                    }
                }
            }

            var classes = graph.TopologicalSort();

            WriteClasses(classes, headerWriter, cppWriter);
        }

        private static void WriteCommon(string outputPath, Cpp2CGeneratorConfig config)
        {
            string filePathHeader = Path.Combine(outputPath, "include", "common.h");
            using var headerWriter = CodeWriter.CreateFile(filePathHeader, "", null);
            headerWriter.Write(templateHeader.Replace("{PREFIX}", config.NamePrefix));
        }

        private unsafe void WriteClasses(IEnumerable<TypeNode> classes, ICodeWriter headerWriter, ICodeWriter cppWriter)
        {
            foreach (var n in classes.Where(c => c.Type.TemplateKind != CppTemplateKind.TemplateClass))
            {
                var c = n.Type;
                var dissolver = config.FindTemplateDissolver(c);
                if (dissolver != null) continue;
                var typeName = config.GetCTypeName(c);
                headerWriter.WriteLine($"typedef {ClassKindToString(c.ClassKind)} {typeName} {typeName};");
            }
            headerWriter.WriteLine();

            foreach (var n in classes)
            {
                var c = n.Type;
                if ((n.Flags & TypeNodeFlags.TemplateSpecialization) != 0)
                {
                    var dissolver = config.FindTemplateDissolver(c);
                    if (dissolver != null) continue;
                    WriteCppClass(headerWriter, cppWriter, c);
                }
                else
                {
                    if (c.ClassKind != CppClassKind.Class && c.ClassKind != CppClassKind.Struct && c.ClassKind != CppClassKind.Union || c.TemplateKind == CppTemplateKind.TemplateClass || c.TemplateKind == CppTemplateKind.TemplateSpecializedClass)
                    {
                        continue;
                    }

                    if (c.SourceFile == null) continue;

                    WriteCppClass(headerWriter, cppWriter, c);
                }
            }

            foreach (var n in classes)
            {
                var c = n.Type;
                if (c.ClassKind != CppClassKind.Class && c.ClassKind != CppClassKind.Struct && c.ClassKind != CppClassKind.Union || c.TemplateKind == CppTemplateKind.TemplateClass)
                {
                    continue;
                }

                if (c.HasVirtualMembers())
                {
                    WriteClassFunctions(c, headerWriter, cppWriter);
                }
                else
                {
                    WriteFunctions(c, headerWriter, cppWriter);
                }
            }
        }

        private void WriteCppClass(ICodeWriter headerWriter, ICodeWriter cppWriter, CppClass c)
        {
            if (!c.HasVirtualMembers())
            {
                WritePOD(c, headerWriter, cppWriter);
                headerWriter.WriteLine();
            }
        }

        private HashSet<CppClass> FindInlineStructs(CppClass cppClass)
        {
            HashSet<CppClass> found = [];
            foreach (var field in cppClass.Fields)
            {
                var cursor = field.Cursor;
                var type = cursor.Type;

                var ownedTagDecl = type.OwnedTagDecl;
                if (ownedTagDecl != CXCursor.Null)
                {
                    foreach (var subClass in cppClass.Classes)
                    {
                        if (subClass.Cursor == ownedTagDecl)
                        {
                            found.Add(subClass);
                            break;
                        }
                    }
                }
            }

            return found;
        }

        private void WritePOD(CppClass cppClass, ICodeWriter headerWriter, ICodeWriter cppWriter, string? typeName = null)
        {
            var inlineStructFields = FindInlineStructs(cppClass);
            typeName ??= config.GetCTypeName(cppClass);

            foreach (var subClass in cppClass.Classes)
            {
                if (subClass.IsAnonymous)
                {
                    continue;
                }
                if (inlineStructFields.Contains(subClass))
                {
                    continue;
                }
                string nestedTypeName = $"{typeName}_{subClass.Name}";

                WritePOD(subClass, headerWriter, cppWriter, nestedTypeName);
                config.SubClasses.Add(subClass, nestedTypeName);
            }

            string typeKeyword = ClassKindToString(cppClass.ClassKind);

            headerWriter.BeginBlock($"{typeKeyword} {typeName}");

            if (cppClass.BaseTypes.Any())
            {
                Stack<(CppClass cls, bool close)> stack = new();
                stack.Push((cppClass, false));
                while (stack.Count > 0)
                {
                    var (cls, close) = stack.Pop();
                    if (close)
                    {
                        WriteFields(headerWriter, cls, cls.Fields, inlineStructFields);
                    }
                    else
                    {
                        stack.Push((cls, true));
                        foreach (var baseType in cls.BaseTypes)
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
                WriteFields(headerWriter, cppClass, cppClass.Fields, inlineStructFields);
            }

            headerWriter.EndBlock($"}};");
        }

        private void WriteClassFunctions(CppClass cppClass, ICodeWriter headerWriter, ICodeWriter cppWriter)
        {
            var typeName = config.GetCTypeName(cppClass);
            if (!cppClass.IsAbstract)
            {
                foreach (var ctor in cppClass.Constructors)
                {
                    WriteConstructor(cppClass, ctor, headerWriter, cppWriter, typeName);
                }
            }

            headerWriter.WriteLine($"{config.NamePrefix}API(void) {typeName}Destroy({typeName}* self);");

            WriteFunctions(cppClass, headerWriter, cppWriter, typeName);
        }

        private void WriteConstructor(CppClass cppClass, CppFunction ctor, ICodeWriter headerWriter, ICodeWriter cppWriter, string typeName)
        {
            if (ctor.Parameters.Count != 0) return;
            headerWriter.WriteLine($"{config.NamePrefix}API({typeName}*) {typeName}Create();");

            cppWriter.BeginBlock($"{config.NamePrefix}API_INTERNAL(::{typeName}*) {typeName}Create()");
            cppWriter.WriteLine($"return reinterpret_cast<::{typeName}*>(new {cppClass.FullName}());");
            cppWriter.EndBlock();
        }

        private void WriteFunctions(CppClass cppClass, ICodeWriter headerWriter, ICodeWriter cppWriter, string? typeName = null)
        {
            typeName ??= config.GetCTypeName(cppClass);
            for (int j = 0; j < cppClass.Functions.Count; j++)
            {
                var f = cppClass.Functions[j];
                if (f.Visibility != CppVisibility.Public)
                {
                    continue;
                }
                if (f.Cursor.IsOverloadedOperator)
                {
                    continue;
                }
                if (f.IsFunctionTemplate)
                {
                    continue;
                }

                WriteFunctionH(cppClass, f, headerWriter);
                WriteFunctionCpp(cppClass, typeName, f, cppWriter);
            }
        }

        private void WriteFields(ICodeWriter writer, CppClass parent, IEnumerable<CppField> fields, HashSet<CppClass>? inlineStructFields)
        {
            if (parent.TemplateKind == CppTemplateKind.TemplateSpecializedClass)
            {
                fields = parent.SpecializedTemplate!.Fields;
            }
            foreach (var field in fields)
            {
                if ((field.StorageQualifier & CppStorageQualifier.Static) != 0)
                {
                    continue;
                }

                var type = field.Type;

                if (type is CppClass cppClass)
                {
                    // Handle nested unions and anonymous structs
                    if (cppClass.IsAnonymous)
                    {
                        WriteInline(writer, cppClass, cppClass.Name);
                        continue;
                    }
                    if (inlineStructFields?.Contains(cppClass) ?? false)
                    {
                        WriteInline(writer, cppClass, fieldName: field.Name);
                        continue;
                    }
                }
                writer.WriteLine($"{config.GetCType(field.Type, field.Name, parent)};");
            }
        }

        public static string ClassKindToString(CppClassKind kind)
        {
            return kind switch
            {
                CppClassKind.Class => "struct",
                CppClassKind.Struct => "struct",
                CppClassKind.Union => "union",
                _ => throw new NotSupportedException(),
            };
        }

        private void WriteInline(ICodeWriter writer, CppClass cppClass, string? name = null, string? fieldName = null)
        {
            name ??= cppClass.Name;
            if (string.IsNullOrEmpty(name))
            {
                writer.BeginBlock($"{ClassKindToString(cppClass.ClassKind)}");
            }
            else
            {
                writer.BeginBlock($"{ClassKindToString(cppClass.ClassKind)} {name}");
            }

            WriteFields(writer, cppClass, cppClass.Fields, null);
            if (string.IsNullOrEmpty(name))
            {
                writer.EndBlock($"}};");
            }
            else
            {
                writer.EndBlock($"}} {fieldName};");
            }
        }

        private string GetCFunctionSignature(CppClass c, CppFunction f, bool globalNamespace)
        {
            StringBuilder sb = new();

            sb.Append($"{config.GetCType(c!, null, globalNamespace: globalNamespace)}* self");
            if (f.Parameters.Count > 0)
            {
                sb.Append(", ");
            }

            for (int i = 0; i < f.Parameters.Count; i++)
            {
                var param = f.Parameters[i];
                sb.Append($"{config.GetCType(param.Type, param.Name, c, globalNamespace: globalNamespace)}");
                if (i < f.Parameters.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            return sb.ToString();
        }

        private string GetCFunctionSignature(CppFunction f, bool globalNamespace)
        {
            StringBuilder sb = new();

            for (int i = 0; i < f.Parameters.Count; i++)
            {
                var param = f.Parameters[i];
                sb.Append($"{config.GetCType(param.Type, param.Name, null, globalNamespace: globalNamespace)}");
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

        private void WriteFunctionH(CppClass c, CppFunction f, ICodeWriter writer, string? name = null)
        {
            name ??= GetUniqueCFunctionName(c, f);
            string cSignature = GetCFunctionSignature(c, f, false);
            definedFunctions.Add(name);
            mapping.Add((c, f), name);

            definedCFunctions.Add(new(name, f.Name, f.Comment?.ToString()));
            string returnType = config.GetCType(f.ReturnType, null);
            writer.WriteLine($"{config.NamePrefix}API({returnType}) {name}({cSignature});");
        }

        private readonly List<ParameterWriter> parameterWriters = [new PointerToClassParameterWriter(), new ReferenceParameterWriter(), new OpaquePointerParameterWriter(), new EnumParameterWriter(), new FallbackParameterWriter()];

        private void WriteFunctionCpp(CppClass c, string typeName, CppFunction f, ICodeWriter writer)
        {
            var name = mapping[(c, f)];
            string cSignature = GetCFunctionSignature(c, f, true);

            string returnType = config.GetCType(f.ReturnType, null, globalNamespace: true);
            bool isVoid = returnType == "void";
            bool isRef = f.ReturnType.IsReferenceType();
            using (writer.PushBlock($"{config.NamePrefix}API_INTERNAL({returnType}) {name}({cSignature})"))
            {
                writer.WriteLine($"auto* ptr = reinterpret_cast<{c.FullName}*>(self);");
                using FunctionWriteContext context = new(f, writer);
                foreach (var param in f.Parameters)
                {
                    var cppType = param.Type;
                    CType cType = new(config.GetCType(param.Type, null, c, globalNamespace: true));
                    foreach (var pw in parameterWriters.OrderByDescending(pw => pw.Priority))
                    {
                        if (pw.CanWriteParameter(context, param, cppType, cType))
                        {
                            pw.WriteParameter(context, param, cppType, cType);
                            break;
                        }
                    }
                }

                if (!isVoid)
                {
                    if (isRef)
                    {
                        writer.Write("auto& ret = ");
                    }
                    else
                    {
                        writer.Write("auto ret = ");
                    }
                }
                writer.WriteLine($"ptr->{f.Name}({context.GetParamString()});");
                if (!isVoid)
                {
                    if (isRef)
                    {
                        writer.WriteLine($"return reinterpret_cast<{returnType}>(&ret);");
                    }
                    else if (f.ReturnType.IsPointerType())
                    {
                        writer.WriteLine($"return reinterpret_cast<{returnType}>(ret);");
                    }
                    else
                    {
                        writer.WriteLine($"return HexaGenRuntime::bit_cast<{returnType}>(ret);");
                    }
                }
            }
        }
    }
}