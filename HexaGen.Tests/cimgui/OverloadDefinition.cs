namespace Test
{
    using System.Collections.Generic;

    internal class OverloadDefinition
    {
        public string ExportedName { get; }

        public string FriendlyName { get; }

        public TypeReference[] Parameters { get; }

        public Dictionary<string, string> DefaultValues { get; }

        public string ReturnType { get; }

        public string StructName { get; }

        public bool IsMemberFunction { get; }

        public string? Comment { get; }

        public bool IsConstructor { get; }

        public bool IsDestructor { get; }

        public OverloadDefinition(
            string exportedName,
            string friendlyName,
            TypeReference[] parameters,
            Dictionary<string, string> defaultValues,
            string returnType,
            string structName,
            string? comment,
            bool isConstructor,
            bool isDestructor)
        {
            ExportedName = exportedName;
            FriendlyName = friendlyName;
            Parameters = parameters;
            DefaultValues = defaultValues;
            ReturnType = returnType.Replace("const", string.Empty).Replace("inline", string.Empty).Trim();
            StructName = structName;
            IsMemberFunction = !string.IsNullOrEmpty(structName);
            Comment = comment;
            IsConstructor = isConstructor;
            IsDestructor = isDestructor;
        }

        public OverloadDefinition WithParameters(TypeReference[] parameters)
        {
            return new OverloadDefinition(ExportedName, FriendlyName, parameters, DefaultValues, ReturnType, StructName, Comment, IsConstructor, IsDestructor);
        }
    }
}