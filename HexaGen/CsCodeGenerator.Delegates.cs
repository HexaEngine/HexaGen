namespace HexaGen
{
    using CppAst;
    using HexaGen.Core;
    using System.IO;

    public partial class CsCodeGenerator
    {
        protected readonly HashSet<string> LibDefinedDelegates = new();

        public readonly HashSet<string> DefinedDelegates = new();

        private readonly HashSet<string> CsNames = new();

        protected virtual List<string> SetupDelegateUsings()
        {
            List<string> usings =
            [
                "System", "System.Diagnostics", "System.Runtime.CompilerServices", "System.Runtime.InteropServices", "HexaGen.Runtime",
                .. config.Usings,
            ];
            return usings;
        }

        protected virtual bool FilterIgnoredType(GenContext context, CppClass cppClass)
        {
            if (config.AllowedTypes.Count != 0 && !config.AllowedTypes.Contains(cppClass.Name))
                return true;

            if (config.IgnoredTypes.Contains(cppClass.Name))
                return true;

            return false;
        }

        protected virtual bool FilterDelegate(GenContext context, ICppMember member)
        {
            if (config.AllowedDelegates.Count != 0 && !config.AllowedDelegates.Contains(member.Name))
                return true;
            if (config.IgnoredDelegates.Contains(member.Name))
                return true;

            if (LibDefinedDelegates.Contains(member.Name))
                return true;

            if (DefinedDelegates.Contains(member.Name))
            {
                LogWarn($"{context.FilePath}: {member.Name} delegate is already defined!");
                return true;
            }

            DefinedDelegates.Add(member.Name);

            return false;
        }

        protected virtual void GenerateDelegates(CppCompilation compilation, string outputPath)
        {
            string folder = Path.Combine(outputPath, "Delegates");
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
            Directory.CreateDirectory(folder);
            string filePath = Path.Combine(folder, "Delegates.cs");

            // Generate Delegates
            using var writer = new CsSplitCodeWriter(filePath, config.Namespace, SetupDelegateUsings(), config.HeaderInjector, 1);

            GenContext context = new(compilation, filePath, writer);

            // Print All classes, structs
            for (int i = 0; i < compilation.Classes.Count; i++)
            {
                CppClass? cppClass = compilation.Classes[i];

                if (FilterIgnoredType(context, cppClass))
                    continue;

                WriteClassDelegates(context, cppClass);

                writer.TrySplit();
            }

            for (int i = 0; i < compilation.Typedefs.Count; i++)
            {
                CppTypedef typedef = compilation.Typedefs[i];

                if (typedef.ElementType is CppPointerType pointerType && pointerType.ElementType is CppFunctionType functionType)
                {
                    WriteDelegate(context, typedef, functionType);
                }

                writer.TrySplit();
            }
        }

        protected virtual void WriteClassDelegates(GenContext context, CppClass cppClass, string? csName = null)
        {
            csName ??= config.GetDelegateName(cppClass.Name);

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
                    csSubName = config.GetDelegateName(subClass.Name);
                }

                WriteClassDelegates(context, subClass, csSubName);
            }

            for (int j = 0; j < cppClass.Fields.Count; j++)
            {
                CppField cppField = cppClass.Fields[j];

                if (cppField.Type is CppPointerType cppPointer && cppPointer.IsDelegate(out var functionType))
                {
                    WriteDelegate(context, cppField, functionType);
                }
                else if (cppField.Type is CppTypedef typedef && typedef.ElementType is CppPointerType pointerType && pointerType.ElementType is CppFunctionType cppFunctionType)
                {
                    WriteDelegate(context, cppField, cppFunctionType, false);
                }
            }
        }

        private void WriteDelegate<T>(GenContext context, T field, CppFunctionType functionType, bool isReadOnly = false) where T : class, ICppDeclaration, ICppMember
        {
            if (FilterDelegate(context, field))
            {
                return;
            }

            var writer = context.Writer;

            string csFieldName = config.GetFieldName(field.Name);
            string fieldPrefix = isReadOnly ? "readonly " : string.Empty;

            int i = 1;
            while (CsNames.Contains(csFieldName))
            {
                csFieldName += $"{i++}";
            }
            CsNames.Add(csFieldName);

            writer.WriteLine("#if NET5_0_OR_GREATER");
            WriteFinal(writer, field, functionType, csFieldName, fieldPrefix);
            writer.WriteLine("#else");
            WriteFinal(writer, field, functionType, csFieldName, fieldPrefix, compatibility: true);
            writer.WriteLine("#endif");
            writer.WriteLine();
        }

        private void WriteFinal<T>(ICodeWriter writer, T field, CppFunctionType functionType, string csFieldName, string fieldPrefix, bool compatibility = false) where T : class, ICppDeclaration, ICppMember
        {
            string signature = config.GetParameterSignature(functionType.Parameters, canUseOut: false, delegateType: true, compatibility: compatibility);
            string returnCsName = config.GetCsTypeName(functionType.ReturnType, false);
            returnCsName = returnCsName.Replace("bool", config.GetBoolType());

            if (functionType.ReturnType is CppTypedef typedef && typedef.ElementType.IsDelegate(out var cppFunction) && !returnCsName.Contains('*'))
            {
                if (cppFunction.Parameters.Count == 0)
                {
                    returnCsName = $"delegate*<{config.GetCsTypeName(cppFunction.ReturnType)}>";
                }
                else
                {
                    returnCsName = $"delegate*<{config.GetNamelessParameterSignature(cppFunction.Parameters, canUseOut: false, delegateType: true, compatibility)}, {config.GetCsTypeName(cppFunction.ReturnType)}>";
                }
            }

            if (compatibility && returnCsName.Contains('*'))
            {
                returnCsName = "nint";
            }

            if (config.TryGetDelegateMapping(csFieldName, out var mapping))
            {
                returnCsName = mapping.ReturnType;
                signature = mapping.Signature;
            }

            string header = $"{returnCsName} {csFieldName}({signature})";

            config.WriteCsSummary(field.Comment, writer);
            if (config.GenerateMetadata)
            {
                writer.WriteLine($"[NativeName(NativeNameType.Delegate, \"{field.Name}\")]");
            }
            writer.WriteLine($"[UnmanagedFunctionPointer(CallingConvention.{functionType.CallingConvention.GetCallingConvention()})]");
            writer.WriteLine($"public unsafe {fieldPrefix}delegate {header};");
            writer.WriteLine();
        }
    }
}