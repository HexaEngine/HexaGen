namespace HexaGen.Core.Mapping
{
    using CppAst;
    using HexaGen.CppAst.Model.Declarations;

    public class FunctionMapping
    {
        public FunctionMapping(string exportedName, string friendlyName, string? comment, Dictionary<string, string> defaults, List<Dictionary<string, string>> customVariations, List<ParameterMapping>? parameters = null)
        {
            ExportedName = exportedName;
            FriendlyName = friendlyName;
            Comment = comment;
            Defaults = defaults;
            CustomVariations = customVariations;
            Parameters = parameters;
        }

        public string ExportedName { get; set; }

        public string? FriendlyName { get; set; }

        public string? Comment { get; set; }

        public Dictionary<string, string> Defaults { get; set; }

        public List<Dictionary<string, string>> CustomVariations { get; set; }

        public List<ParameterMapping>? Parameters { get; set; }

        public void CreateDefaultMappingParameters(CppFunction function)
        {
            Parameters ??= new(function.Parameters.Count);
            Parameters.Clear();
            foreach (var param in function.Parameters)
            {
                ParameterMapping mapping = new(param.Name, null, false);
                Parameters.Add(mapping);
            }
        }
    }

    public class ParameterMapping
    {
        public ParameterMapping(string exportedName, string? friendlyName, bool useOut)
        {
            ExportedName = exportedName;
            FriendlyName = friendlyName;
            UseOut = useOut;
        }

        public string ExportedName { get; set; }

        public string? FriendlyName { get; set; }

        public bool UseOut { get; set; }
    }
}