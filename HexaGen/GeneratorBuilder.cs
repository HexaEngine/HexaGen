namespace HexaGen
{
    using CommandLine;
    using HexaGen.Metadata;
    using HexaGen.Patching;
    using Microsoft.Extensions.Options;
    using System.Diagnostics.CodeAnalysis;

    public readonly ref struct MacroBuilder
    {
        private readonly List<string> defines;
        private readonly string? prefix;

        public MacroBuilder(string? prefix = null)
        {
            defines = [];
            this.prefix = prefix;
        }

        public MacroBuilder(List<string> defines, string? prefix = null)
        {
            this.defines = defines;
            this.prefix = prefix;
        }

        public readonly MacroBuilder AddMacro(string value)
        {
            defines.Add(prefix + value);
            return this;
        }

        public readonly MacroBuilder WithPrefix(string prefix)
        {
            return new(defines, prefix);
        }

        public delegate string? SelectorFormat<T>(T option, string? value, bool condition);

        public readonly Selector<T> WithSelector<T>(T current, SelectorFormat<T> format, Func<T, T, bool> selector)
        {
            return new(this, current, format, selector);
        }

        public readonly Selector<T> WithSelector<T>(T current, SelectorFormat<T> format)
        {
            return new(this, current, format, EqualityComparer<T>.Default.Equals);
        }

        public readonly ref struct Selector<T>
        {
            private readonly MacroBuilder builder;
            private readonly T current;
            private readonly SelectorFormat<T> format;
            private readonly Func<T, T, bool> selector;

            public Selector(MacroBuilder builder, T current, SelectorFormat<T> format, Func<T, T, bool> selector)
            {
                this.builder = builder;
                this.current = current;
                this.format = format;
                this.selector = selector;
            }

            private readonly void AddMacro(T conditional, bool condition, string? value)
            {
                string? macro = format(conditional, value, condition);
                if (macro == null) return;
                builder.AddMacro(macro);
            }

            public readonly Selector<T> Option(T conditional, string? value = null)
            {
                AddMacro(conditional, selector(current, conditional), value);
                return this;
            }

            public readonly Selector<T> Option(T conditional, ReadOnlySpan<string> values)
            {
                bool condition = selector(current, conditional);
                foreach (var value in values)
                {
                    AddMacro(conditional, condition, value);
                }
                return this;
            }
        }
    }

    public class GeneratorBuilder
    {
        private CsCodeGeneratorConfig config = null!;
        private CsCodeGenerator generator = null!;
        private CLIGeneratorOptions? options;
        private readonly List<IPrePatch> prePatches = [];
        private readonly List<IPostPatch> postPatches = [];
        private readonly List<GenEventHandler<CsCodeGenerator, CsCodeGeneratorConfig>> onPostConfigureCallbacks = [];

        public static GeneratorBuilder Create()
        {
            GeneratorBuilder generator = new();
            return generator;
        }

        public static GeneratorBuilder Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(string configPath) where T : CsCodeGenerator
        {
            GeneratorBuilder generator = new();
            generator.Setup<T>(configPath);
            return generator;
        }

        public static GeneratorBuilder Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(CsCodeGeneratorConfig config) where T : CsCodeGenerator
        {
            GeneratorBuilder generator = new();
            generator.Setup<T>(config);
            return generator;
        }

        public GeneratorBuilder Setup<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(string configPath) where T : CsCodeGenerator
        {
            return Setup<T>(CsCodeGeneratorConfig.Load(configPath));
        }

        public GeneratorBuilder Setup<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(CsCodeGeneratorConfig config) where T : CsCodeGenerator
        {
            if (generator != null)
            {
                onPostConfigureCallbacks.Clear();
                generator.PostConfigure -= OnPostConfigure;
            }
            this.config = config;
            var type = typeof(T);
            var ctor = type.GetConstructor([typeof(CsCodeGeneratorConfig)]);
            generator = (T)ctor!.Invoke([config]);
            generator.LogToConsole();
            foreach (var patch in prePatches)
            {
                generator.PatchEngine.RegisterPrePatch(patch);
            }
            foreach (var patch in postPatches)
            {
                generator.PatchEngine.RegisterPostPatch(patch);
            }

            generator.PostConfigure += OnPostConfigure;
            return this;
        }

        public GeneratorBuilder WithArgs(string[] args)
        {
            options = Parser.Default.ParseArguments<CLIGeneratorOptions>(args).Value;
            if (generator != null)
            {
                generator.CLIOptions = options;
            }

            return this;
        }

        private void OnPostConfigure(CsCodeGenerator sender, CsCodeGeneratorConfig args)
        {
            foreach (var callback in onPostConfigureCallbacks)
            {
                callback(sender, args);
            }
        }

        public GeneratorBuilder WithGlobalPrePatch(IPrePatch patch)
        {
            prePatches.Add(patch);
            generator?.PatchEngine.RegisterPrePatch(patch);
            return this;
        }

        public GeneratorBuilder WithGlobalPostPatch(IPostPatch patch)
        {
            postPatches.Add(patch);
            generator?.PatchEngine.RegisterPostPatch(patch);
            return this;
        }

        public GeneratorBuilder WithPrePatch(IPrePatch patch)
        {
            generator.PatchEngine.RegisterPrePatch(patch);
            return this;
        }

        public GeneratorBuilder WithPostPatch(IPostPatch patch)
        {
            generator.PatchEngine.RegisterPostPatch(patch);
            return this;
        }

        public GeneratorBuilder OnPostConfigure(GenEventHandler<CsCodeGenerator, CsCodeGeneratorConfig> onPostConfigure)
        {
            onPostConfigureCallbacks.Add(onPostConfigure);
            return this;
        }

        /// <summary>
        /// Generates code from the specified source files.
        /// </summary>
        /// <param name="sources">A list of source file paths to process.</param>
        /// <param name="output">The output directory for the generated code.</param>
        /// <param name="allowedHeaders">
        /// (Optional) A list of allowed header files. <b>Note:</b> This parameter is not yet supported by HexaGen.Legacy builds and will be ignored.
        /// </param>
        /// <returns>The current <see cref="GeneratorBuilder"/> instance.</returns>
        public GeneratorBuilder Generate(List<string> sources, string output, List<string>? allowedHeaders = null)
        {
            generator.Generate(sources, output, allowedHeaders);
            return this;
        }

        /// <summary>
        /// Generates code from the specified source file.
        /// </summary>
        /// <param name="source">The source file path to process.</param>
        /// <param name="output">The output directory for the generated code.</param>
        /// <param name="allowedHeaders">
        /// (Optional) A list of allowed header files. <b>Note:</b> This parameter is not yet supported by HexaGen.Legacy builds and will be ignored.
        /// </param>
        /// <returns>The current <see cref="GeneratorBuilder"/> instance.</returns>
        public GeneratorBuilder Generate(string source, string output, List<string>? allowedHeaders = null)
        {
            generator.Generate(source, output, allowedHeaders);
            return this;
        }

        public GeneratorBuilder GetMetadata(out CsCodeGeneratorMetadata metadata)
        {
            metadata = generator.GetMetadata();
            return this;
        }

        public GeneratorBuilder GetConfig(out CsCodeGeneratorConfig config)
        {
            config = this.config;
            return this;
        }

        public GeneratorBuilder AlterGenerator(Action<CsCodeGenerator> action)
        {
            action(generator);
            return this;
        }

        public GeneratorBuilder AlterConfig(Action<CsCodeGeneratorConfig> action)
        {
            action(config);
            return this;
        }

        public delegate void MacroBuilderCallback(MacroBuilder builder);

        public GeneratorBuilder WithMacros(MacroBuilderCallback action) => WithMacros(null, action);

        public GeneratorBuilder WithMacros(string? prefix, MacroBuilderCallback action)
        {
            action(new MacroBuilder(config.Defines, prefix));
            return this;
        }

        public GeneratorBuilder MergeConfig(CsCodeGeneratorConfig baseConfig, MergeOptions options)
        {
            config.Merge(baseConfig, options);
            return this;
        }

        public GeneratorBuilder CopyFromMetadata(CsCodeGeneratorMetadata? metadata)
        {
            if (metadata == null) return this;
            generator.CopyFrom(metadata);
            return this;
        }

        public GeneratorBuilder WithFunctionTableEntires(CsCodeGeneratorMetadata? metadata)
        {
            if (metadata == null) return this;
            return WithFunctionTableEntires(metadata.FunctionTable.Entries);
        }

        public GeneratorBuilder WithFunctionTableEntires(List<CsFunctionTableEntry> entries)
        {
            config.FunctionTableEntries = entries;
            return this;
        }
    }
}