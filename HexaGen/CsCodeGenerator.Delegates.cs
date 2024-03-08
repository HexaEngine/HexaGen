namespace HexaGen
{
    using CppAst;
    using System.IO;

    public partial class CsCodeGenerator
    {
        protected readonly HashSet<string> LibDefinedDelegates = new();

        public readonly HashSet<string> DefinedDelegates = new();

        protected virtual List<string> SetupDelegateUsings()
        {
            List<string> usings = new() { "System", "System.Diagnostics", "System.Runtime.CompilerServices", "System.Runtime.InteropServices", "HexaGen.Runtime" };
            usings.AddRange(settings.Usings);
            return usings;
        }

        protected virtual bool FilterIgnoredType(GenContext context, CppClass cppClass)
        {
            if (settings.AllowedTypes.Count != 0 && !settings.AllowedTypes.Contains(cppClass.Name))
                return true;

            if (settings.IgnoredTypes.Contains(cppClass.Name))
                return true;

            return false;
        }

        protected virtual bool FilterDelegate(GenContext context, ICppMember member)
        {
            if (settings.AllowedDelegates.Count != 0 && !settings.AllowedDelegates.Contains(member.Name))
                return true;
            if (settings.IgnoredDelegates.Contains(member.Name))
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
            string filePath = Path.Combine(outputPath, "Delegates.cs");

            // Generate Delegates
            using var writer = new CsCodeWriter(filePath, settings.Namespace, SetupDelegateUsings());

            GenContext context = new(compilation, filePath, writer);

            // Print All classes, structs
            for (int i = 0; i < compilation.Classes.Count; i++)
            {
                CppClass? cppClass = compilation.Classes[i];

                if (FilterIgnoredType(context, cppClass))
                    continue;

                WriteClassDelegates(context, cppClass);
            }

            for (int i = 0; i < compilation.Typedefs.Count; i++)
            {
                CppTypedef typedef = compilation.Typedefs[i];

                if (typedef.ElementType is CppPointerType pointerType && pointerType.ElementType is CppFunctionType functionType)
                {
                    WriteDelegate(context, typedef, functionType);
                }
            }
        }

        protected virtual void WriteClassDelegates(GenContext context, CppClass cppClass, string? csName = null)
        {
            csName ??= settings.GetDelegateName(cppClass.Name);

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
                    csSubName = settings.GetDelegateName(subClass.Name);
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

        protected virtual void WriteDelegate<T>(GenContext context, T field, CppFunctionType functionType, bool isReadOnly = false) where T : class, ICppDeclaration, ICppMember
        {
            if (FilterDelegate(context, field))
                return;

            var writer = context.Writer;
            string csFieldName = settings.GetDelegateName(field.Name);
            string fieldPrefix = isReadOnly ? "readonly " : string.Empty;
            string signature = settings.GetParameterSignature(functionType.Parameters, false, settings.GenerateMetadata);
            string returnCsName = settings.GetCsTypeName(functionType.ReturnType, false);
            returnCsName = returnCsName.Replace("bool", settings.GetBoolType());

            if (settings.TryGetDelegateMapping(csFieldName, out var mapping))
            {
                returnCsName = mapping.ReturnType;
                signature = mapping.Signature;
            }

            string header = $"{returnCsName} {csFieldName}({signature})";
            LogInfo("defined delegate " + header);
            settings.WriteCsSummary(field.Comment, writer);
            if (settings.GenerateMetadata)
            {
                writer.WriteLine($"[NativeName(NativeNameType.Delegate, \"{field.Name}\")]");
                writer.WriteLine($"[return: NativeName(NativeNameType.Type, \"{functionType.ReturnType.GetDisplayName()}\")]");
            }
            writer.WriteLine($"[UnmanagedFunctionPointer(CallingConvention.{functionType.CallingConvention.GetCallingConvention()})]");
            writer.WriteLine($"public unsafe {fieldPrefix}delegate {header};");
            writer.WriteLine();
        }
    }
}