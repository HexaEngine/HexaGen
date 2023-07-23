namespace HexaGen
{
    using CppAst;
    using HexaGen;
    using System.IO;
    using System.Linq;

    public partial class CsCodeGenerator
    {
        private readonly HashSet<string> LibDefinedDelegates = new();

        public readonly HashSet<string> DefinedDelegates = new();

        private void GenerateDelegates(CppCompilation compilation, string outputPath)
        {
            string filePath = Path.Combine(outputPath, "Delegates.cs");
            string[] usings = { "System", "System.Diagnostics", "System.Runtime.CompilerServices", "System.Runtime.InteropServices", "HexaGen.Runtime" };

            // Generate Delegates
            using var writer = new CodeWriter(filePath, settings.Namespace, usings.Concat(settings.Usings).ToArray());

            // Print All classes, structs
            for (int i = 0; i < compilation.Classes.Count; i++)
            {
                CppClass? cppClass = compilation.Classes[i];
                if (settings.AllowedTypes.Count != 0 && !settings.AllowedTypes.Contains(cppClass.Name))
                    continue;

                if (settings.IgnoredTypes.Contains(cppClass.Name))
                    continue;

                string csName = settings.GetCsCleanName(cppClass.Name);
                WriteClassDelegates(filePath, writer, compilation, cppClass, csName);
            }

            for (int i = 0; i < compilation.Typedefs.Count; i++)
            {
                CppTypedef typedef = compilation.Typedefs[i];
                if (settings.AllowedDelegates.Count != 0 && !settings.AllowedDelegates.Contains(typedef.Name))
                    continue;
                if (settings.IgnoredDelegates.Contains(typedef.Name))
                    continue;

                if (typedef.ElementType is CppPointerType pointerType && pointerType.ElementType is CppFunctionType functionType)
                {
                    if (LibDefinedDelegates.Contains(typedef.Name))
                        continue;

                    if (DefinedDelegates.Contains(typedef.Name))
                    {
                        LogWarn($"{filePath}: {typedef} delegate is already defined!");
                        continue;
                    }
                    var csName = settings.GetCsCleanName(typedef.Name);
                    DefinedDelegates.Add(csName);
                    WriteDelegate(writer, typedef, functionType, csName);
                }
            }
        }

        public void WriteDelegate(CodeWriter writer, CppTypedef typedef, CppFunctionType type, string csName)
        {
            string returnCsName = settings.GetCsTypeName(type.ReturnType, false);
            string signature = settings.GetParameterSignature(type.Parameters, false);

            if (settings.TryGetDelegateMapping(csName, out var mapping))
            {
                returnCsName = mapping.ReturnType;
                signature = mapping.Signature;
            }
            string header = $"{returnCsName} {csName}({signature})";
            LogInfo("defined delegate " + header);
            typedef.Comment.WriteCsSummary(writer);
            writer.WriteLine($"[UnmanagedFunctionPointer(CallingConvention.{type.CallingConvention.GetCallingConvention()})]");
            writer.WriteLine($"public unsafe delegate {header};");
            writer.WriteLine();
        }

        public void WriteClassDelegates(string filePath, CodeWriter writer, CppCompilation compilation, CppClass cppClass, string csName)
        {
            if (cppClass.ClassKind == CppClassKind.Class || cppClass.Name.EndsWith("_T") || csName == "void")
            {
                return;
            }

            for (int j = 0; j < cppClass.Classes.Count; j++)
            {
                var subClass = cppClass.Classes[j];
                string csSubName;
                if (string.IsNullOrEmpty(subClass.Name))
                {
                    string label = cppClass.Classes.Count == 1 ? "" : j.ToString();
                    csSubName = csName + "Union" + label;
                }
                else
                {
                    csSubName = settings.GetCsCleanName(subClass.Name);
                }

                WriteClassDelegates(filePath, writer, compilation, subClass, csSubName);
            }

            for (int j = 0; j < cppClass.Fields.Count; j++)
            {
                CppField cppField = cppClass.Fields[j];

                if (cppField.Type is CppPointerType cppPointer && cppPointer.IsDelegate(out var functionType))
                {
                    string csFieldName = settings.NormalizeFieldName(cppField.Name);

                    if (LibDefinedDelegates.Contains(csFieldName))
                        continue;

                    if (DefinedDelegates.Contains(csFieldName))
                    {
                        LogWarn($"{filePath}: {csFieldName} delegate is already defined!");
                        continue;
                    }

                    DefinedDelegates.Add(csFieldName);

                    string returnCsName = settings.GetCsTypeName(functionType.ReturnType, false);
                    string signature = settings.GetParameterSignature(functionType.Parameters, false);
                    returnCsName = returnCsName.Replace("bool", "byte");

                    if (settings.TryGetDelegateMapping(csFieldName, out var mapping))
                    {
                        returnCsName = mapping.ReturnType;
                        signature = mapping.Signature;
                    }

                    string header = $"{returnCsName} {csFieldName}({signature})";
                    LogInfo("defined delegate " + header);
                    cppField.Comment.WriteCsSummary(writer);
                    writer.WriteLine($"[UnmanagedFunctionPointer(CallingConvention.{functionType.CallingConvention.GetCallingConvention()})]");
                    writer.WriteLine($"public unsafe delegate {header};");
                    writer.WriteLine();
                }
                else
                {
                    WriteDelegate(filePath, writer, cppField, false);
                }
            }
        }

        private void WriteDelegate(string filePath, CodeWriter writer, CppField field, bool isReadOnly = false)
        {
            string csFieldName = settings.NormalizeFieldName(field.Name);

            string fieldPrefix = isReadOnly ? "readonly " : string.Empty;

            if (field.Type is CppTypedef typedef &&
                typedef.ElementType is CppPointerType pointerType &&
                pointerType.ElementType is CppFunctionType functionType)
            {
                if (LibDefinedDelegates.Contains(csFieldName))
                    return;

                if (DefinedDelegates.Contains(csFieldName))
                {
                    LogWarn($"{filePath}: {csFieldName}, delegate is already defined!");
                    return;
                }

                DefinedDelegates.Add(csFieldName);

                string signature = settings.GetParameterSignature(functionType.Parameters, false);
                string returnCsName = settings.GetCsTypeName(functionType.ReturnType, false);
                returnCsName = returnCsName.Replace("bool", "byte");

                if (settings.TryGetDelegateMapping(csFieldName, out var mapping))
                {
                    returnCsName = mapping.ReturnType;
                    signature = mapping.Signature;
                }

                string header = $"{returnCsName} {csFieldName}({signature})";
                LogInfo("defined delegate " + header);
                field.Comment.WriteCsSummary(writer);
                writer.WriteLine($"[UnmanagedFunctionPointer(CallingConvention.{functionType.CallingConvention.GetCallingConvention()})]");
                writer.WriteLine($"public unsafe {fieldPrefix}delegate {header};");
                writer.WriteLine();
            }
        }
    }
}