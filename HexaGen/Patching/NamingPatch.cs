using CppAst;

namespace HexaGen.Patching
{
    public enum NamingPatchOptions
    {
        None,
        MultiplePrefixes = 1 << 0,
        OverwriteNames = 1 << 1,
    }

    public class NamingPatch : PrePatch
    {
        private readonly string[] prefixes;
        private readonly NamingPatchOptions options;

        public NamingPatch(string[] prefixes, NamingPatchOptions options)
        {
            this.prefixes = prefixes;
            this.options = options;
        }

        protected override void PatchFunction(CsCodeGeneratorConfig config, CppFunction cppFunction)
        {
            var name = config.GetCsFunctionName(cppFunction.Name);

            foreach (var prefix in prefixes)
            {
                if (name.StartsWith(prefix))
                {
                    name = name[prefix.Length..];
                    if (!options.HasFlag(NamingPatchOptions.MultiplePrefixes))
                    {
                        break;
                    }
                }
            }

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
    }
}