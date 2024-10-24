namespace HexaGen.Core.CSharp
{
    using HexaGen.Core.Collections;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public enum CsFunctionKind
    {
        Default,
        Constructor,
        Destructor,
        Operator,
        Member,
        Extension,
    }

    public class CsFunctionOverload : ICsFunction, ICloneable<CsFunctionOverload>
    {
        [JsonConstructor]
        public CsFunctionOverload(string exportedName, string name, string? comment, Dictionary<string, string> defaultValues, string structName, CsFunctionKind kind, CsType returnType, List<CsParameterInfo> parameters, List<CsFunctionVariation> variations, List<string> modifiers, List<string> attributes)
        {
            ExportedName = exportedName;
            Name = name;
            Comment = comment;
            DefaultValues = defaultValues;
            StructName = structName;
            Kind = kind;
            ReturnType = returnType;
            Parameters = parameters;
            Variations = new(variations);
            Modifiers = modifiers;
            Attributes = attributes;
        }

        public CsFunctionOverload(string exportedName, string name, string? comment, string structName, CsFunctionKind kind, CsType returnType)
        {
            ExportedName = exportedName;
            Name = name;
            Comment = comment;
            DefaultValues = new();
            StructName = structName;
            Kind = kind;
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

        public CsFunctionKind Kind { get; set; }

        public CsType ReturnType { get; set; }

        public List<CsParameterInfo> Parameters { get; set; }

        public ConcurrentList<CsFunctionVariation> Variations { get; set; }

        public List<string> Modifiers { get; set; }

        public List<string> Attributes { get; set; }

        public bool HasVariation(CsFunctionVariation variation)
        {
            lock (Variations.SyncObject)
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
        }

        public bool TryAddVariation(CsFunctionVariation variation)
        {
            lock (Variations.SyncObject)
            {
                if (!HasVariation(variation))
                {
                    Variations.Add(variation);
                    return true;
                }
            }
            return false;
        }

        public bool TryUpdateVariation(CsFunctionVariation oldVariation, CsFunctionVariation newVariation)
        {
            lock (Variations.SyncObject)
            {
                if (!HasVariation(newVariation))
                {
                    Variations.Add(newVariation);
                    Variations.Remove(oldVariation);
                    return true;
                }
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

        public string BuildSignatureNamelessForCOM(string comObject, IGeneratorConfig settings)
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
            return new(null!, ExportedName, Name, StructName, Kind, ReturnType);
        }

        public CsFunctionOverload Clone()
        {
            return new CsFunctionOverload(ExportedName, Name, Comment, DefaultValues.Clone(), StructName, Kind, ReturnType.Clone(), Parameters.CloneValues(), Variations.CloneValues(), Modifiers.Clone(), Attributes.Clone());
        }
    }
}