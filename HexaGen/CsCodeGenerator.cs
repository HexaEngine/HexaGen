namespace HexaGen
{
    using CppAst;
    using HexaGen.Core.CSharp;
    using HexaGen.Core.Logging;
    using HexaGen.Patching;
    using System.Text.Json;

    public partial class CsCodeGenerator : BaseGenerator
    {
        protected FunctionGenerator funcGen;
        protected PatchEngine patchEngine = new();

        public static CsCodeGenerator Create(string configPath)
        {
            return new(CsCodeGeneratorSettings.Load(configPath));
        }

        public CsCodeGenerator(CsCodeGeneratorSettings settings) : this(settings, new(settings))
        {
        }

        public CsCodeGenerator(CsCodeGeneratorSettings settings, FunctionGenerator functionGenerator) : base(settings)
        {
            funcGen = functionGenerator;
        }

        public FunctionGenerator FunctionGenerator { get => funcGen; protected set => funcGen = value; }

        public PatchEngine PatchEngine => patchEngine;

        protected virtual CppParserOptions PrepareSettings()
        {
            var options = new CppParserOptions
            {
                ParseMacros = true,
                ParseComments = true,
                ParseSystemIncludes = true,
                ParseAsCpp = true,
                AutoSquashTypedef = settings.AutoSquashTypedef,
            };

            for (int i = 0; i < settings.AdditionalArguments.Count; i++)
            {
                options.AdditionalArguments.Add(settings.AdditionalArguments[i]);
            }

            for (int i = 0; i < settings.IncludeFolders.Count; i++)
            {
                options.IncludeFolders.Add(settings.IncludeFolders[i]);
            }

            for (int i = 0; i < settings.SystemIncludeFolders.Count; i++)
            {
                options.SystemIncludeFolders.Add(settings.SystemIncludeFolders[i]);
            }

            for (int i = 0; i < settings.Defines.Count; i++)
            {
                options.Defines.Add(settings.Defines[i]);
            }

            // options.ConfigureForWindowsMsvc(CppTargetCpu.X86_64);
            //options.AdditionalArguments.Add("-std=c++17");

            return options;
        }

        public virtual bool Generate(string headerFile, string outputPath)
        {
            var options = PrepareSettings();

            var compilation = CppParser.ParseFile(headerFile, options);

            return Generate(compilation, [headerFile], outputPath);
        }

        public virtual bool Generate(List<string> headerFiles, string outputPath)
        {
            var options = PrepareSettings();

            var compilation = CppParser.ParseFiles(headerFiles, options);

            return Generate(compilation, headerFiles, outputPath);
        }

        public virtual bool Generate(CppCompilation compilation, List<string> headerFiles, string outputPath)
        {
            Directory.CreateDirectory(outputPath);
            // Print diagnostic messages
            for (int i = 0; i < compilation.Diagnostics.Messages.Count; i++)
            {
                CppDiagnosticMessage? message = compilation.Diagnostics.Messages[i];
                if (message.Type == CppLogMessageType.Error && settings.CppLogLevel <= LogSeverity.Error)
                {
                    LogError(message.ToString());
                }
                if (message.Type == CppLogMessageType.Warning && settings.CppLogLevel <= LogSeverity.Warning)
                {
                    LogWarn(message.ToString());
                }
                if (message.Type == CppLogMessageType.Info && settings.CppLogLevel <= LogSeverity.Information)
                {
                    LogInfo(message.ToString());
                }
            }

            if (compilation.HasErrors)
            {
                return false;
            }

            patchEngine.ApplyPrePatches(settings, AppDomain.CurrentDomain.BaseDirectory, headerFiles, compilation);

            settings.DefinedCppEnums = DefinedCppEnums;

            if (settings.GenerateEnums)
            {
                GenerateEnums(compilation, outputPath);
            }

            if (settings.GenerateConstants)
            {
                GenerateConstants(compilation, outputPath);
            }

            if (settings.GenerateHandles)
            {
                GenerateHandles(compilation, outputPath);
            }

            if (settings.GenerateTypes)
            {
                GenerateTypes(compilation, outputPath);
            }

            if (settings.GenerateFunctions)
            {
                GenerateFunctions(compilation, outputPath);
            }

            if (settings.GenerateExtensions)
            {
                GenerateExtensions(compilation, outputPath);
            }

            if (settings.GenerateDelegates)
            {
                GenerateDelegates(compilation, outputPath);
            }

            patchEngine.ApplyPostPatches(GetMetadata(), outputPath, Directory.GetFiles(outputPath, "*.*", SearchOption.AllDirectories).ToList());

            return true;
        }

        public virtual void Reset()
        {
            DefinedConstants.Clear();
            DefinedEnums.Clear();
            DefinedExtensions.Clear();
            DefinedFunctions.Clear();
            DefinedTypedefs.Clear();
            DefinedTypes.Clear();
            DefinedDelegates.Clear();
        }

        public virtual void CopyFrom(List<CsConstantMetadata> constants, List<CsEnumMetadata> enums, List<string> extensions, List<string> functions, List<string> typedefs, List<string> types, List<string> delegates)
        {
            for (int i = 0; i < constants.Count; i++)
            {
                LibDefinedConstants.Add(constants[i]);
            }
            for (int i = 0; i < enums.Count; i++)
            {
                LibDefinedEnums.Add(enums[i]);
            }
            for (int i = 0; i < extensions.Count; i++)
            {
                LibDefinedExtensions.Add(extensions[i]);
            }
            for (int i = 0; i < functions.Count; i++)
            {
                LibDefinedFunctions.Add(functions[i]);
            }
            for (int i = 0; i < typedefs.Count; i++)
            {
                LibDefinedTypedefs.Add(typedefs[i]);
            }
            for (int i = 0; i < types.Count; i++)
            {
                LibDefinedTypes.Add(types[i]);
            }
            for (int i = 0; i < delegates.Count; i++)
            {
                LibDefinedDelegates.Add(delegates[i]);
            }
        }

        public void CopyFrom(CsCodeGeneratorMetadata metadata)
        {
            var constants = metadata.DefinedConstants;
            var enums = metadata.DefinedEnums;
            var extensions = metadata.DefinedExtensions;
            var functions = metadata.DefinedFunctions;
            var typedefs = metadata.DefinedTypes;
            var types = metadata.DefinedTypes;
            var delegates = metadata.DefinedDelegates;
            for (int i = 0; i < constants.Count; i++)
            {
                LibDefinedConstants.Add(constants[i]);
            }
            foreach (var enumMeta in enums)
            {
                LibDefinedEnums.Add(enumMeta);
                DefinedCppEnums.Add(enumMeta.Identifier, enumMeta);
            }
            for (int i = 0; i < extensions.Count; i++)
            {
                LibDefinedExtensions.Add(extensions[i]);
            }
            for (int i = 0; i < functions.Count; i++)
            {
                LibDefinedFunctions.Add(functions[i]);
            }
            for (int i = 0; i < typedefs.Count; i++)
            {
                LibDefinedTypedefs.Add(typedefs[i]);
            }
            for (int i = 0; i < types.Count; i++)
            {
                LibDefinedTypes.Add(types[i]);
            }
            for (int i = 0; i < delegates.Count; i++)
            {
                LibDefinedDelegates.Add(delegates[i]);
            }
        }

        public void SaveMetadata(string path)
        {
            CsCodeGeneratorMetadata metadata = new();
            metadata.CopyFrom(this);
            JsonSerializerOptions options = new(JsonSerializerDefaults.General);
            options.WriteIndented = true;
            var json = JsonSerializer.Serialize(metadata, options);
            File.WriteAllText(path, json);
        }

        public void LoadMetadata(string path)
        {
            var json = File.ReadAllText(path);
            var metadata = JsonSerializer.Deserialize<CsCodeGeneratorMetadata>(json) ?? new();
            CopyFrom(metadata);
        }

        public CsCodeGeneratorMetadata GetMetadata()
        {
            CsCodeGeneratorMetadata metadata = new();
            metadata.CopyFrom(this);
            return metadata;
        }

        protected static CppFunction FindFunction(CppCompilation compilation, string name)
        {
            for (int i = 0; i < compilation.Functions.Count; i++)
            {
                var function = compilation.Functions[i];
                if (function.Name == name)
                    return function;
            }
            return null;
        }

        protected void PrepareArgs(CsFunctionVariation variation, CsType csReturnType)
        {
            if (WrappedPointers.TryGetValue(csReturnType.Name, out string? value))
            {
                csReturnType.Name = value;
            }

            for (int i = 0; i < variation.Parameters.Count; i++)
            {
                var cppParameter = variation.Parameters[i];
                if (WrappedPointers.TryGetValue(cppParameter.Type.Name, out string? v))
                {
                    cppParameter.Type.Name = v;
                    cppParameter.Type.Classify();
                }
            }
        }

        protected virtual CsFunction CreateCsFunction(CppFunction cppFunction, CsFunctionKind kind, string csName, List<CsFunction> functions, out CsFunctionOverload overload)
        {
            settings.TryGetFunctionMapping(cppFunction.Name, out var mapping);

            string returnCsName = settings.GetCsTypeName(cppFunction.ReturnType, false);
            CppPrimitiveKind returnKind = cppFunction.ReturnType.GetPrimitiveKind();

            CsFunction? function = null;
            for (int j = 0; j < functions.Count; j++)
            {
                if (functions[j].Name == csName)
                {
                    function = functions[j];
                    break;
                }
            }

            if (function == null)
            {
                settings.WriteCsSummary(cppFunction.Comment, out string? comment);
                if (mapping != null && mapping.Comment != null)
                {
                    comment = settings.WriteCsSummary(mapping.Comment);
                }
                function = new(csName, comment);
                functions.Add(function);
            }

            overload = new(cppFunction.Name, csName, function.Comment, "", kind, new(returnCsName, returnKind));
            overload.Attributes.Add($"[NativeName(NativeNameType.Func, \"{cppFunction.Name}\")]");
            overload.Attributes.Add($"[return: NativeName(NativeNameType.Type, \"{cppFunction.ReturnType.GetDisplayName()}\")]");
            for (int j = 0; j < cppFunction.Parameters.Count; j++)
            {
                var cppParameter = cppFunction.Parameters[j];
                var paramCsTypeName = settings.GetCsTypeName(cppParameter.Type, false);
                var paramCsName = settings.GetParameterName(j, cppParameter.Name);
                var direction = cppParameter.Type.GetDirection();
                var primKind = cppParameter.Type.GetPrimitiveKind();

                CsType csType = new(paramCsTypeName, primKind);

                CsParameterInfo csParameter = new(paramCsName, cppParameter.Type, csType, direction);
                csParameter.Attributes.Add($"[NativeName(NativeNameType.Param, \"{cppParameter.Name}\")]");
                csParameter.Attributes.Add($"[NativeName(NativeNameType.Type, \"{cppParameter.Type.GetDisplayName()}\")]");
                overload.Parameters.Add(csParameter);
                if (settings.TryGetDefaultValue(cppFunction.Name, cppParameter, false, out var defaultValue))
                {
                    overload.DefaultValues.Add(paramCsName, defaultValue);
                }
            }

            function.Overloads.Add(overload);
            return function;
        }
    }
}