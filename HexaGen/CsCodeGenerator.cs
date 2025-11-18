namespace HexaGen
{
    using HexaGen.Core;
    using HexaGen.Core.CSharp;
    using HexaGen.Core.Logging;
    using HexaGen.CppAst.Diagnostics;
    using HexaGen.CppAst.Model.Declarations;
    using HexaGen.CppAst.Model.Interfaces;
    using HexaGen.CppAst.Model.Metadata;
    using HexaGen.CppAst.Model.Types;
    using HexaGen.CppAst.Parsing;
    using HexaGen.FunctionGeneration;
    using HexaGen.GenerationSteps;
    using HexaGen.Metadata;
    using HexaGen.Patching;
    using HexaGen.PreProcessSteps;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    public partial class CsCodeGenerator : BaseGenerator
    {
        protected FunctionGenerator funcGen = null!;
        protected PatchEngine patchEngine = new();
        private CsCodeGeneratorMetadata metadata = new();
        public readonly FunctionTableBuilder FunctionTableBuilder = new();
        private readonly List<GenerationStep> generationSteps = new();
        private Dictionary<string, string> wrappedPointers = null!;
        private List<CsCodeGeneratorMetadata> copyFromPending = [];

        public static CsCodeGenerator Create(string configPath)
        {
            return new(CsCodeGeneratorConfig.Load(configPath));
        }

        public CsCodeGenerator(CsCodeGeneratorConfig config) : base(config)
        {
        }

        public FunctionGenerator FunctionGenerator { get => funcGen; protected set => funcGen = value; }

        public PatchEngine PatchEngine => patchEngine;

        public IReadOnlyList<GenerationStep> GenerationSteps => generationSteps;

        public List<PreProcessStep> PreProcessSteps { get; } = new();

        public CLIGeneratorOptions? CLIOptions { get; set; }

        public T GetGenerationStep<T>() where T : GenerationStep
        {
            foreach (var step in GenerationSteps)
            {
                if (step is T t)
                {
                    return t;
                }
            }

            throw new InvalidOperationException($"Step of type '{typeof(T)}' was not found.");
        }

        public void AddGenerationStep(GenerationStep step)
        {
            generationSteps.Add(step);
        }

        public void AddGenerationStep<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>() where T : GenerationStep
        {
            var step = (GenerationStep)Activator.CreateInstance(typeof(T), this, config)!;
            generationSteps.Add(step);
        }

        public void OverwriteGenerationStep<TTarget>(GenerationStep newStep) where TTarget : GenerationStep
        {
            for (int i = 0; i < GenerationSteps.Count; i++)
            {
                GenerationStep step = GenerationSteps[i];
                if (step is TTarget)
                {
                    GenerationSteps[i] = newStep;
                }
            }
        }

        public bool Generate(string headerFile, string outputPath, List<string>? allowedHeaders = null)
        {
            return Generate([headerFile], outputPath, allowedHeaders);
        }

        public bool Generate(List<string> headerFiles, string outputPath, List<string>? allowedHeaders = null)
        {
            return Generate(PrepareSettings(), headerFiles, outputPath, allowedHeaders);
        }

        public bool Generate(CppParserOptions parserOptions, string headerFile, string outputPath, List<string>? allowedHeaders = null)
        {
            return Generate([headerFile], outputPath, allowedHeaders);
        }

        public bool Generate(CppParserOptions parserOptions, List<string> headerFiles, string outputPath, List<string>? allowedHeaders = null)
        {
            ConfigureCore();
            LogInfo($"Generating: {config.ApiName}");

            LogInfo("Parsing Headers...");
            var compilation = ParseFiles(parserOptions, headerFiles);

            return GenerateCore(compilation, headerFiles, outputPath, allowedHeaders);
        }

        protected virtual void ConfigureCore()
        {
            ConfigureGeneratorCore(PreProcessSteps, GenerationSteps, out funcGen);
            config.DefinedCppEnums = GetGenerationStep<EnumGenerationStep>().DefinedCppEnums;
            wrappedPointers = GetGenerationStep<TypeGenerationStep>().WrappedPointers;
            metadata = new();
            metadata.Settings = Settings;
            OnPostConfigure(config);
        }

        protected virtual void ConfigureGeneratorCore(List<PreProcessStep> preProcessSteps, List<GenerationStep> generationSteps, out FunctionGenerator funcGen)
        {
            funcGen = FunctionGenerator.CreateDefault(config);
            preProcessSteps.Add(new ConstantPreProcessStep(this, config));
            generationSteps.Add(new EnumGenerationStep(this, config));
            generationSteps.Add(new ConstantGenerationStep(this, config));
            generationSteps.Add(new HandleGenerationStep(this, config));
            generationSteps.Add(new TypeGenerationStep(this, config));
            generationSteps.Add(new FunctionGenerationStep(this, config));
            generationSteps.Add(new ExtensionGenerationStep(this, config));
            generationSteps.Add(new DelegateGenerationStep(this, config));
            OnConfigureGenerator();
        }

        protected virtual void OnConfigureGenerator()
        {
        }

        protected virtual CppParserOptions PrepareSettings()
        {
            var options = new CppParserOptions
            {
                ParseMacros = true,
                ParseComments = true,
                ParseSystemIncludes = true,

                ParseCommentAttribute = true,
                //ParseTokenAttributes = true,
                ParserKind = CppParserKind.Cpp,

                AutoSquashTypedef = false,
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

        protected virtual CppCompilation ParseFiles(CppParserOptions parserOptions, List<string> headerFiles)
        {
            return CppParser.ParseFiles(headerFiles, parserOptions);
        }

        public virtual bool GenerateCore(CppCompilation compilation, List<string> headerFiles, string outputPath, List<string>? allowedHeaders = null)
        {
            if (CLIOptions != null && CLIOptions.OutputDirectory != null)
            {
                outputPath = Path.Combine(CLIOptions.OutputDirectory, outputPath);
            }
            else
            {
            }

            if (Directory.Exists(outputPath)) Directory.Delete(outputPath, true);
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

            allowedHeaders ??= [];
            allowedHeaders.AddRange(headerFiles);

            FileSet files = new(allowedHeaders.Select(PathHelper.GetPath));

            foreach (var meta in copyFromPending)
            {
                foreach (var step in generationSteps)
                {
                    step.CopyFromMetadata(meta);
                }
            }   

            LogInfo($"Configuring Pre-Processing Steps...");
            foreach (var step in PreProcessSteps)
            {
                step.Configure(config);
            }

            LogInfo("Running Pre-Processing Steps...");

            ParseResult result = new(compilation);
            foreach (var step in PreProcessSteps)
            {
                step.PreProcess(files, compilation, config, metadata, result);
            }

            OnPrePatchCore(result, headerFiles);

            LogInfo($"Configuring Steps...");
            foreach (var step in GenerationSteps)
            {
                step.Configure(config);
            }

            foreach (var step in GenerationSteps)
            {
                if (step.Enabled)
                {
                    LogInfo($"Generating {step.Name}...");
                    step.Generate(files, result, outputPath, config, metadata);
                    step.CopyToMetadata(metadata);
                }
            }

            LogInfo("Applying Post-Patches...");
            patchEngine.ApplyPostPatches(metadata, outputPath, Directory.GetFiles(outputPath, "*.*", SearchOption.AllDirectories).ToList());

            return true;
        }

        protected virtual void OnPrePatchCore(ParseResult result, List<string> headerFiles)
        {
            LogInfo("Applying Pre-Patches...");
            patchEngine.ApplyPrePatches(config, AppDomain.CurrentDomain.BaseDirectory, headerFiles, result);
            OnPrePatch(result, headerFiles);
        }

        protected virtual void OnPrePatch(ParseResult result, List<string> headerFiles)
        {
        }

        public virtual void Reset()
        {
            foreach (var step in GenerationSteps)
            {
                step.Reset();
            }
        }

        public void CopyFrom(CsCodeGeneratorMetadata metadata)
        {
            copyFromPending.Add(metadata);
        }

        public void SaveMetadata(string path)
        {
            JsonSerializerSettings options = new() { Formatting = Formatting.Indented };
            var json = JsonConvert.SerializeObject(metadata, options);
            File.WriteAllText(path, json);
        }

        public void LoadMetadata(string path)
        {
            var json = File.ReadAllText(path);
            var metadata = JsonConvert.DeserializeObject<CsCodeGeneratorMetadata>(json) ?? new();
            CopyFrom(metadata);
        }

        public CsCodeGeneratorMetadata GetMetadata()
        {
            return metadata;
        }

        public static CppFunction FindFunction(CppCompilation compilation, string name)
        {
            for (int i = 0; i < compilation.Functions.Count; i++)
            {
                var function = compilation.Functions[i];
                if (function.Name == name)
                    return function;
            }

            throw new Exception($"Function '{name}' not found!");
        }

        public void PrepareArgs(CsFunctionVariation variation, CsType csReturnType)
        {
            if (wrappedPointers.TryGetValue(csReturnType.Name, out string? value))
            {
                csReturnType.Name = value;
            }

            for (int i = 0; i < variation.Parameters.Count; i++)
            {
                var cppParameter = variation.Parameters[i];
                if (wrappedPointers.TryGetValue(cppParameter.Type.Name, out string? v))
                {
                    cppParameter.Type.Name = v;
                    cppParameter.Type.Classify();
                }
            }
        }

        public virtual CsFunction CreateCsFunction(CppFunction cppFunction, CsFunctionKind kind, string csName, List<CsFunction> functions, out CsFunctionOverload overload)
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
                var paramCsTypeName = config.GetCsTypeName(cppParameter.Type);
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
                    overload.DefaultValues.Add(paramCsName, defaultValue!);
                }
            }

            function.Overloads.Add(overload);
            return function;
        }

        public virtual CsDelegate CreateCsDelegate<T>(T member, string csName, CppFunctionType functionType) where T : class, ICppDeclaration, ICppMember
        {
            config.WriteCsSummary(member.Comment, out string? comment);

            string returnCsName = config.GetCsReturnType(functionType.ReturnType);
            CppPrimitiveKind returnKind = functionType.ReturnType.GetPrimitiveKind();

            List<CsParameterInfo> parameters = [];

            for (int j = 0; j < functionType.Parameters.Count; j++)
            {
                var cppParameter = functionType.Parameters[j];
                var paramCsTypeName = config.GetCsTypeName(cppParameter.Type);
                var paramCsName = config.GetParameterName(j, cppParameter.Name);
                var direction = cppParameter.Type.GetDirection();
                var primKind = cppParameter.Type.GetPrimitiveKind();

                CsType csType = new(paramCsTypeName, primKind);

                CsParameterInfo csParameter = new(paramCsName, cppParameter.Type, csType, direction);
                csParameter.Attributes.Add($"[NativeName(NativeNameType.Param, \"{cppParameter.Name}\")]");
                csParameter.Attributes.Add($"[NativeName(NativeNameType.Type, \"{cppParameter.Type.GetDisplayName()}\")]");
                parameters.Add(csParameter);
            }

            List<string> attributes = [];

            if (config.GenerateMetadata)
            {
                attributes.Add($"[NativeName(NativeNameType.Delegate, \"{member.Name}\")]");
                attributes.Add($"[return: NativeName(NativeNameType.Type, \"{functionType.ReturnType.GetDisplayName()}\")]");
            }
            attributes.Add($"[UnmanagedFunctionPointer(CallingConvention.{functionType.CallingConvention.GetCallingConvention()})]");

            return new(member.Name, csName, new(returnCsName, returnKind), parameters, attributes, comment);
        }

        protected virtual string BuildFunctionSignature(CsFunctionVariation variation, bool useAttributes, bool useNames, WriteFunctionFlags flags)
        {
            int offset = flags == WriteFunctionFlags.None ? 0 : 1;
            StringBuilder sb = new();
            bool isFirst = true;

            if (flags == WriteFunctionFlags.Extension)
            {
                isFirst = false;
                var first = variation.Parameters[0];
                if (useNames)
                {
                    sb.Append($"this {first.Type} {first.Name}");
                }
                else
                {
                    sb.Append($"this {first.Type}");
                }
            }

            for (int i = offset; i < variation.Parameters.Count; i++)
            {
                var param = variation.Parameters[i];

                if (param.DefaultValue != null)
                    continue;

                if (!isFirst)
                    sb.Append(", ");

                if (useAttributes)
                {
                    sb.Append($"{string.Join(" ", param.Attributes)} ");
                }

                sb.Append($"{param.Type}");

                if (useNames)
                {
                    sb.Append($" {param.Name}");
                }

                isFirst = false;
            }

            return sb.ToString();
        }

        public virtual string BuildFunctionHeaderId(CsFunctionVariation variation, WriteFunctionFlags flags)
        {
            string signature = BuildFunctionSignature(variation, false, false, flags);
            return $"{variation.Name}({signature})";
        }

        public virtual string BuildFunctionHeader(CsFunctionVariation variation, CsType csReturnType, WriteFunctionFlags flags, bool generateMetadata)
        {
            string signature = BuildFunctionSignature(variation, generateMetadata, true, flags);
            return $"{csReturnType.Name} {variation.Name}({signature})";
        }

        public static void ClassifyParameters(CsFunctionOverload overload, CsFunctionVariation variation, CsType csReturnType, out bool firstParamReturn, out int offset, out bool hasManaged)
        {
            firstParamReturn = false;
            if (!csReturnType.IsString && csReturnType.Name != overload.ReturnType.Name)
            {
                firstParamReturn = true;
            }

            offset = firstParamReturn ? 1 : 0;
            hasManaged = false;
            for (int j = 0; j < variation.Parameters.Count - offset; j++)
            {
                var cppParameter = variation.Parameters[j + offset];

                if (cppParameter.DefaultValue == null)
                {
                    continue;
                }

                var paramCsDefault = cppParameter.DefaultValue;
                if (cppParameter.Type.IsString || paramCsDefault.StartsWith("\"") && paramCsDefault.EndsWith("\""))
                {
                    hasManaged = true;
                }
            }
        }
    }
}