namespace HexaGen
{
    using CppAst;
    using HexaGen.Core.CSharp;
    using System.Collections.Generic;

    public class ParseResult
    {
        public ParseResult(CppCompilation compilation)
        {
            Compilation = compilation;
        }

        public CppCompilation Compilation { get; set; }

        public Dictionary<string, List<FunctionAlias>> FunctionAliases { get; set; } = [];

        public List<FunctionAlias> AddFunctionAlias(FunctionAlias alias)
        {
            if (!FunctionAliases.TryGetValue(alias.ExportedName, out var aliases))
            {
                aliases = [];
                FunctionAliases.Add(alias.ExportedName, aliases);
            }

            aliases.Add(alias);
            return aliases;
        }

        public IEnumerable<FunctionAlias> EnumerateFunctionAliases(string name)
        {
            if (!FunctionAliases.TryGetValue(name, out var aliases))
            {
                yield break;
            }

            foreach (var alias in aliases)
            {
                yield return alias;
            }
        }
    }
}