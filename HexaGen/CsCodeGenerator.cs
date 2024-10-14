namespace HexaGen
{
    using CppAst;
    using HexaGen.Core.CSharp;
    using HexaGen.Core.Logging;
    using HexaGen.FunctionGeneration;
    using HexaGen.Metadata;
    using HexaGen.Patching;
    using System.Text.Json;

    public partial class CsCodeGenerator : BaseGenerator
    {
        protected FunctionGenerator funcGen;
        protected PatchEngine patchEngine = new();
        protected ConfigComposer configComposer = new();
        private CsCodeGeneratorMetadata metadata = new();

        public static CsCodeGenerator Create(string configPath)
        {
            return new(CsCodeGeneratorConfig.Load(configPath));
        }

        public CsCodeGenerator(CsCodeGeneratorConfig config) : this(config, FunctionGenerator.CreateDefault(config))
        {
        }

        public CsCodeGenerator(CsCodeGeneratorConfig config, FunctionGenerator functionGenerator) : base(config)
        {
            funcGen = functionGenerator;
        }

        public FunctionGenerator FunctionGenerator { get => funcGen; protected set => funcGen = value; }

        public PatchEngine PatchEngine => patchEngine;

        public ConfigComposer ConfigComposer => configComposer;

        protected virtual CppParserOptions PrepareSettings()
        {
            var options = new CppParserOptions
            {
                ParseMacros = true,
                ParseComments = true,
                ParseSystemIncludes = true,
                ParseAsCpp = true,
                AutoSquashTypedef = config.AutoSquashTypedef,
            };

            for (int i = 0; i < config.AdditionalArguments.Count; i++)
            {
                options.AdditionalArguments.Add(config.AdditionalArguments[i]);
            }

            for (int i = 0; i < config.IncludeFolders.Count; i++)
            {
                options.IncludeFolders.Add(config.IncludeFolders[i]);
            }

            for (int i = 0; i < config.SystemIncludeFolders.Count; i++)
            {
                options.SystemIncludeFolders.Add(config.SystemIncludeFolders[i]);
            }

            for (int i = 0; i < config.Defines.Count; i++)
            {
                options.Defines.Add(config.Defines[i]);
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
            metadata = new();
            Directory.CreateDirectory(outputPath);
            // Print diagnostic messages
            for (int i = 0; i < compilation.Diagnostics.Messages.Count; i++)
            {
                CppDiagnosticMessage? message = compilation.Diagnostics.Messages[i];
                if (message.Type == CppLogMessageType.Error && config.CppLogLevel <= LogSeverity.Error)
                {
                    LogError(message.ToString());
                }
                if (message.Type == CppLogMessageType.Warning && config.CppLogLevel <= LogSeverity.Warning)
                {
                    LogWarn(message.ToString());
                }
                if (message.Type == CppLogMessageType.Info && config.CppLogLevel <= LogSeverity.Information)
                {
                    LogInfo(message.ToString());
                }
            }

            if (compilation.HasErrors)
            {
                return false;
            }

            configComposer.LogEvent += Log;
            configComposer.Compose(config);
            configComposer.LogEvent -= Log;
            patchEngine.ApplyPrePatches(config, AppDomain.CurrentDomain.BaseDirectory, headerFiles, compilation);

            config.DefinedCppEnums = DefinedCppEnums;

            if (config.GenerateEnums)
            {
                GenerateEnums(compilation, outputPath);
            }

            if (config.GenerateConstants)
            {
                GenerateConstants(compilation, outputPath);
            }

            if (config.GenerateHandles)
            {
                GenerateHandles(compilation, outputPath);
            }

            if (config.GenerateTypes)
            {
                GenerateTypes(compilation, outputPath);
            }

            if (config.GenerateFunctions)
            {
                GenerateFunctions(compilation, outputPath);
            }

            if (config.GenerateExtensions)
            {
                GenerateExtensions(compilation, outputPath);
            }

            if (config.GenerateDelegates)
            {
                GenerateDelegates(compilation, outputPath);
            }

            metadata.CopyFrom(this);

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
            var wrappedPointers = metadata.WrappedPointers;
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
            foreach (var item in wrappedPointers)
            {
                WrappedPointers[item.Key] = item.Value;
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
            config.TryGetFunctionMapping(cppFunction.Name, out var mapping);

            string returnCsName = config.GetCsReturnType(cppFunction.ReturnType);
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
                config.WriteCsSummary(cppFunction.Comment, out string? comment);
                if (mapping != null && mapping.Comment != null)
                {
                    comment = config.WriteCsSummary(mapping.Comment);
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
                var paramCsTypeName = config.GetCsTypeName(cppParameter.Type, false);
                var paramCsName = config.GetParameterName(j, cppParameter.Name);
                var direction = cppParameter.Type.GetDirection();
                var primKind = cppParameter.Type.GetPrimitiveKind();

                CsType csType = new(paramCsTypeName, primKind);

                CsParameterInfo csParameter = new(paramCsName, cppParameter.Type, csType, direction);
                csParameter.Attributes.Add($"[NativeName(NativeNameType.Param, \"{cppParameter.Name}\")]");
                csParameter.Attributes.Add($"[NativeName(NativeNameType.Type, \"{cppParameter.Type.GetDisplayName()}\")]");
                overload.Parameters.Add(csParameter);
                if (config.TryGetDefaultValue(cppFunction.Name, cppParameter, false, out var defaultValue))
                {
                    overload.DefaultValues.Add(paramCsName, defaultValue);
                }
            }

            function.Overloads.Add(overload);
            return function;
        }
    }
}