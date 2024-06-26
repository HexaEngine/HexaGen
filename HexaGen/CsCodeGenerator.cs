﻿namespace HexaGen
{
    using CppAst;
    using HexaGen.Core.CSharp;
    using HexaGen.Core.Logging;
    using System.Text.Json;

    public partial class CsCodeGenerator : BaseGenerator
    {
        private readonly FunctionGenerator funcGen;

        public CsCodeGenerator(CsCodeGeneratorSettings settings) : base(settings)
        {
            funcGen = new(settings);
        }

        protected virtual CppParserOptions PrepareSettings()
        {
            var options = new CppParserOptions
            {
                ParseMacros = true,
                ParseTokenAttributes = false,
                ParseCommentAttribute = false,
                ParseComments = true,
                ParseSystemIncludes = true,
                ParseAsCpp = true,
                AutoSquashTypedef = true,
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

            options.ConfigureForWindowsMsvc(CppTargetCpu.X86_64);
            options.AdditionalArguments.Add("-std=c++17");

            return options;
        }

        public virtual void Generate(string headerFile, string outputPath)
        {
            var options = PrepareSettings();

            var compilation = CppParser.ParseFile(headerFile, options);

            Generate(compilation, outputPath);
        }

        public virtual void Generate(List<string> headerFiles, string outputPath)
        {
            var options = PrepareSettings();

            var compilation = CppParser.ParseFiles(headerFiles, options);

            Generate(compilation, outputPath);
        }

        public virtual void Generate(CppCompilation compilation, string outputPath)
        {
            Directory.CreateDirectory(outputPath);
            // Print diagnostic messages
            for (int i = 0; i < compilation.Diagnostics.Messages.Count; i++)
            {
                CppDiagnosticMessage? message = compilation.Diagnostics.Messages[i];
                if (message.Type == CppLogMessageType.Error && settings.CppLogLevel <= LogSevertiy.Error)
                {
                    LogError(message.ToString());
                }
                if (message.Type == CppLogMessageType.Warning && settings.CppLogLevel <= LogSevertiy.Warning)
                {
                    LogWarn(message.ToString());
                }
                if (message.Type == CppLogMessageType.Info && settings.CppLogLevel <= LogSevertiy.Information)
                {
                    LogInfo(message.ToString());
                }
            }

            if (compilation.HasErrors)
            {
                return;
            }

            List<Task> tasks = new();

            if (settings.GenerateEnums)
            {
                Task taskEnums = new(() => GenerateEnums(compilation, outputPath));
                tasks.Add(taskEnums);
                taskEnums.Start();
            }

            if (settings.GenerateConstants)
            {
                Task taskConstants = new(() => GenerateConstants(compilation, outputPath));
                tasks.Add(taskConstants);
                taskConstants.Start();
            }

            if (settings.GenerateHandles)
            {
                Task taskHandles = new(() => GenerateHandles(compilation, outputPath));
                tasks.Add(taskHandles);
                taskHandles.RunSynchronously();
            }

            if (settings.GenerateTypes)
            {
                Task taskTypes = new(() => GenerateTypes(compilation, outputPath));
                tasks.Add(taskTypes);
                taskTypes.RunSynchronously();
            }

            if (settings.GenerateFunctions)
            {
                Task taskFuncs = new(() => GenerateFunctions(compilation, outputPath));
                tasks.Add(taskFuncs);
                taskFuncs.Start();
            }

            if (settings.GenerateExtensions)
            {
                Task taskExtensions = new(() => GenerateExtensions(compilation, outputPath));
                tasks.Add(taskExtensions);
                taskExtensions.Start();
            }

            if (settings.GenerateDelegates)
            {
                Task taskDelegates = new(() => GenerateDelegates(compilation, outputPath));
                tasks.Add(taskDelegates);
                taskDelegates.Start();
            }

            Task.WaitAll(tasks.ToArray());
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

        protected virtual CsFunction CreateCsFunction(CppFunction cppFunction, string csName, List<CsFunction> functions, out CsFunctionOverload overload)
        {
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
                function = new(csName, comment);
                functions.Add(function);
            }

            overload = new(cppFunction.Name, csName, function.Comment, "", false, false, false, new(returnCsName, returnKind));
            overload.Attributes.Add($"[NativeName(NativeNameType.Func, \"{cppFunction.Name}\")]");
            overload.Attributes.Add($"[return: NativeName(NativeNameType.Type, \"{cppFunction.ReturnType.GetDisplayName()}\")]");
            for (int j = 0; j < cppFunction.Parameters.Count; j++)
            {
                var cppParameter = cppFunction.Parameters[j];
                var paramCsTypeName = settings.GetCsTypeName(cppParameter.Type, false);
                var paramCsName = settings.GetParameterName(j, cppParameter.Name);
                var direction = cppParameter.Type.GetDirection();
                var kind = cppParameter.Type.GetPrimitiveKind();

                CsType csType = new(paramCsTypeName, kind);

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