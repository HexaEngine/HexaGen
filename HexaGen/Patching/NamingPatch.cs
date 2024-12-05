using CppAst;

namespace HexaGen.Patching
{
    public enum NamingPatchOptions
    {
        None,
        MultiplePrefixes = 1 << 0,
        OverwriteNames = 1 << 1,
        CaseInsensitive = 1 << 2
    }

    public enum NamingPatchMode
    {
        None = 0,
        Functions = 1 << 0,
        Structs = 1 << 1,
        Handles = 1 << 2,
        Enums = 1 << 3,
        All = Functions | Structs | Handles | Enums,
    }

    public class NamingPatch : PrePatch
    {
        private readonly string[] prefixes;
        private readonly NamingPatchOptions options;
        private readonly NamingPatchMode mode;

        public NamingPatch(string[] prefixes, NamingPatchOptions options, NamingPatchMode mode = NamingPatchMode.Functions)
        {
            this.prefixes = prefixes;
            this.options = options;
            this.mode = mode;
        }

        protected override void PatchEnum(CsCodeGeneratorConfig config, CppEnum cppEnum)
        {
            if ((mode & NamingPatchMode.Enums) == 0) return;

            var name = config.GetCsCleanName(cppEnum.Name);
            name = Process(name);
            if (!config.TryGetEnumMapping(cppEnum.Name, out var mapping))
            {
                mapping = new(cppEnum.Name, name, null);
                config.EnumMappings.Add(mapping);
                config.TypeMappings[cppEnum.Name] = name;
                config.TypeMappings.TryAdd(cppEnum.Name, name);
            }

            if ((options & NamingPatchOptions.OverwriteNames) != 0)
            {
                mapping.FriendlyName = name;
            }
            else
            {
                mapping.FriendlyName ??= name;
            }
        }

        protected override void PatchClass(CsCodeGeneratorConfig config, CppClass cppClass)
        {
            if ((mode & NamingPatchMode.Structs) == 0) return;

            var name = config.GetCsCleanName(cppClass.Name);
            name = Process(name);
            if (!config.TryGetTypeMapping(cppClass.Name, out var mapping))
            {
                mapping = new(cppClass.Name, name, null);
                config.ClassMappings.Add(mapping);
                config.TypeMappings.TryAdd(cppClass.Name, name);
            }

            if ((options & NamingPatchOptions.OverwriteNames) != 0)
            {
                mapping.FriendlyName = name;
            }
            else
            {
                mapping.FriendlyName ??= name;
            }
        }

        protected override void PatchTypedef(CsCodeGeneratorConfig config, CppTypedef cppTypedef)
        {
            if ((mode & NamingPatchMode.Handles) == 0) return;

            var name = config.GetCsTypeName(cppTypedef);
            name = Process(name);
            if (!config.TryGetHandleMapping(cppTypedef.Name, out var mapping))
            {
                mapping = new(cppTypedef.Name, name, null);
                config.HandleMappings.Add(mapping);
                config.TypeMappings.TryAdd(cppTypedef.Name, name);
            }

            if ((options & NamingPatchOptions.OverwriteNames) != 0)
            {
                mapping.FriendlyName = name;
            }
            else
            {
                mapping.FriendlyName ??= name;
            }
        }

        protected override void PatchFunction(CsCodeGeneratorConfig config, CppFunction cppFunction)
        {
            if ((mode & NamingPatchMode.Functions) == 0) return;

            var name = config.GetCsFunctionName(cppFunction.Name);
            name = Process(name);

            if (!config.TryGetFunctionMapping(name, out var mapping))
            {
                mapping = new(cppFunction.Name, name, null, [], []);
                config.FunctionMappings.Add(mapping);
            }

            if ((options & NamingPatchOptions.OverwriteNames) != 0)
            {
                mapping.FriendlyName = name;
            }
            else
            {
                mapping.FriendlyName ??= name;
            }
        }

        private unsafe string Process(string name)
        {
            bool changed = false;
            foreach (var prefix in prefixes)
            {
                if (name.StartsWith(prefix, (options & NamingPatchOptions.CaseInsensitive) != 0 ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                {
                    changed = true;
                    name = name[prefix.Length..];
                    if (!options.HasFlag(NamingPatchOptions.MultiplePrefixes))
                    {
                        break;
                    }
                }
            }

            if (changed)
            {
                if (name.Length > 0)
                {
                    fixed (char* p = name)
                    {
                        p[0] = char.ToUpper(p[0]);
                    }
                }
            }

            return name;
        }
    }
}