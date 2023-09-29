namespace HexaGen.Core.CSharp
{
    using System.Collections.Generic;

    public class CsFunctionOverload : ICsFunction, ICloneable<CsFunctionOverload>
    {
        public CsFunctionOverload(string exportedName, string name, string? comment, Dictionary<string, string> defaultValues, string structName, bool isMember, bool isConstructor, bool isDestructor, CsType returnType, List<CsParameterInfo> parameters, List<CsFunctionVariation> variations, List<string> modifiers, List<string> attributes)
        {
            ExportedName = exportedName;
            Name = name;
            Comment = comment;
            DefaultValues = defaultValues;
            StructName = structName;
            IsMember = isMember;
            IsConstructor = isConstructor;
            IsDestructor = isDestructor;
            ReturnType = returnType;
            Parameters = parameters;
            Variations = variations;
            Modifiers = modifiers;
            Attributes = attributes;
        }

        public CsFunctionOverload(string exportedName, string name, string? comment, string structName, bool isMember, bool isConstructor, bool isDestructor, CsType returnType)
        {
            ExportedName = exportedName;
            Name = name;
            Comment = comment;
            DefaultValues = new();
            StructName = structName;
            IsMember = isMember;
            IsConstructor = isConstructor;
            IsDestructor = isDestructor;
            ReturnType = returnType;
            Parameters = new();
            Variations = new();
            Modifiers = new();
            Attributes = new();
        }

        public string ExportedName { get; set; }

        public string Name { get; set; }

        public string? Comment { get; set; }

        public Dictionary<string, string> DefaultValues { get; }

        public string StructName { get; set; }

        public bool IsMember { get; set; }

        public bool IsConstructor { get; set; }

        public bool IsDestructor { get; set; }

        public CsType ReturnType { get; set; }

        public List<CsParameterInfo> Parameters { get; set; }

        public List<CsFunctionVariation> Variations { get; set; }

        public List<string> Modifiers { get; set; }

        public List<string> Attributes { get; set; }

        public bool HasVariation(CsFunctionVariation variation)
        {
            for (int i = 0; i < Variations.Count; i++)
            {
                var iation = Variations[i];
                if (variation.Parameters.Count != iation.Parameters.Count)
                    continue;
                if (variation.Name != iation.Name)
                    continue;

                bool skip = false;
                for (int j = 0; j < iation.Parameters.Count; j++)
                {
                    if (variation.Parameters[j].Type.Name != iation.Parameters[j].Type.Name || variation.Parameters[j].DefaultValue != iation.Parameters[j].DefaultValue)
                    {
                        skip = true;
                        break;
                    }
                }

                if (skip)
                    continue;

                return true;
            }

            return false;
        }

        public string BuildSignature()
        {
            return string.Join(", ", Parameters.Select(p => $"{string.Join(" ", p.Attributes)} {p.Type.Name} {p.Name}"));
        }

        public string BuildSignatureNameless()
        {
            return string.Join(", ", Parameters.Select(p => $"{p.Name}"));
        }

        public string BuildSignatureNamelessForCOM(string comObject, IGeneratorSettings settings)
        {
            return $"{comObject}*{(Parameters.Count > 0 ? ", " : string.Empty)}{string.Join(", ", Parameters.Select(x => $"{(x.Type.IsBool ? settings.GetBoolType() : x.Type.Name)}"))}";
        }

        public override string ToString()
        {
            return BuildSignature();
        }

        public bool HasParameter(CsParameterInfo cppParameter)
        {
            for (int i = 0; i < Parameters.Count; i++)
            {
                if (Parameters[i].Name == cppParameter.Name)
                    return true;
            }
            return false;
        }

        public CsFunctionVariation CreateVariationWith()
        {
            return new(ExportedName, Name, StructName, IsMember, IsConstructor, IsDestructor, ReturnType);
        }

        public CsFunctionOverload Clone()
        {
            return new CsFunctionOverload(ExportedName, Name, Comment, DefaultValues.Clone(), StructName, IsMember, IsConstructor, IsDestructor, ReturnType.Clone(), Parameters.CloneValues(), Variations.CloneValues(), Modifiers.Clone(), Attributes.Clone());
        }
    }
}