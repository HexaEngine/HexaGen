namespace HexaGen.Core.CSharp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    public class CsFunctionVariation : ICsFunction, ICloneable<CsFunctionVariation>, IHasIdentifier
    {
        [JsonConstructor]
        public CsFunctionVariation(string identifier, string exportedName, string name, string structName, CsFunctionKind kind, CsType returnType, List<CsParameterInfo> parameters, List<CsGenericParameterInfo> genericParameters, List<string> modifiers, List<string> attributes)
        {
            Identifier = identifier;
            ExportedName = exportedName;
            Name = name;

            StructName = structName;
            Kind = kind;
            ReturnType = returnType;
            Parameters = parameters;
            GenericParameters = genericParameters;
            Modifiers = modifiers;
            Attributes = attributes;
        }

        public CsFunctionVariation(string identifier, string exportedName, string name, string structName, CsFunctionKind kind, CsType returnType)
        {
            Identifier = identifier;
            ExportedName = exportedName;
            Name = name;
            StructName = structName;
            Kind = kind;
            ReturnType = returnType;
            Parameters = new();
            GenericParameters = new();
            Modifiers = new();
            Attributes = new();
        }

        public string Identifier { get; set; }

        public string ExportedName { get; set; }

        public string Name { get; set; }

        public string StructName { get; set; }

        public CsFunctionKind Kind { get; set; }

        public bool IsGeneric => GenericParameters.Count > 0;

        public CsType ReturnType { get; set; }

        public List<CsParameterInfo> Parameters { get; set; }

        public List<CsGenericParameterInfo> GenericParameters { get; set; }

        public List<string> Modifiers { get; set; }

        public List<string> Attributes { get; set; }

        #region IDs

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

        public string BuildFunctionHeaderId(WriteFunctionFlags flags)
        {
            string signature = BuildFunctionSignature(this, false, false, flags);
            return Identifier = $"{Name}({signature})";
        }

        public string BuildFunctionHeaderId(string alias, WriteFunctionFlags flags)
        {
            string signature = BuildFunctionSignature(this, false, false, flags);
            return Identifier = $"{alias}({signature})";
        }

        public string BuildFunctionHeader(CsType csReturnType, WriteFunctionFlags flags, bool generateMetadata)
        {
            string signature = BuildFunctionSignature(this, generateMetadata, true, flags);
            if (IsGeneric)
            {
                return Identifier = $"{csReturnType.Name} {Name}<{BuildGenericSignature()}>({signature}) {BuildGenericConstraint()}";
            }
            else
            {
                return Identifier = $"{csReturnType.Name} {Name}({signature})";
            }
        }

        public string BuildFunctionHeader(string alias, CsType csReturnType, WriteFunctionFlags flags, bool generateMetadata)
        {
            string signature = BuildFunctionSignature(this, generateMetadata, true, flags);
            if (IsGeneric)
            {
                return Identifier = $"{csReturnType.Name} {alias}<{BuildGenericSignature()}>({signature}) {BuildGenericConstraint()}";
            }
            else
            {
                return Identifier = $"{csReturnType.Name} {alias}({signature})";
            }
        }

        public string BuildFunctionOverload(WriteFunctionFlags flags)
        {
            string signature = BuildFunctionOverload(this, flags);
            if (IsGeneric)
            {
                return $"{Name}<{BuildGenericSignature()}>({signature})";
            }
            else
            {
                return $"{Name}({signature})";
            }
        }

        protected virtual string BuildFunctionOverload(CsFunctionVariation variation, WriteFunctionFlags flags)
        {
            int offset = flags == WriteFunctionFlags.None ? 0 : 1;
            StringBuilder sb = new();
            bool isFirst = true;

            if (flags == WriteFunctionFlags.Extension)
            {
                isFirst = false;
                var first = variation.Parameters[0];
                sb.Append("this");

                sb.Append($" {first.Name}");
            }

            for (int i = offset; i < variation.Parameters.Count; i++)
            {
                bool written = false;
                var param = variation.Parameters[i];

                if (param.DefaultValue != null)
                    continue;

                if (!isFirst)
                    sb.Append(", ");

                if (param.Type.IsRef)
                {
                    sb.Append("ref");
                    written = true;
                }

                if (param.Type.IsOut)
                {
                    sb.Append("out");
                    written = true;
                }

                if (written) sb.Append(' ');
                sb.Append(param.Name);

                isFirst = false;
            }

            return sb.ToString();
        }

        public string BuildConstructorSignatureIdentifier()
        {
            return Identifier = $"{StructName}({BuildConstructorSignature(false, false, false)})";
        }

        public string BuildSignatureIdentifierForCOM()
        {
            return Identifier = $"{ReturnType.Name} {Name}{(IsGeneric ? $"<{BuildGenericSignature()}>" : string.Empty)}({BuildSignature(false, false)})";
        }

        public string BuildExtensionSignatureIdentifierForCOM(string comObject)
        {
            return Identifier = $"{ReturnType.Name} {Name}{(IsGeneric ? $"<{BuildGenericSignature()}>" : string.Empty)}({BuildExtensionSignatureForCOM(comObject, false, false)})";
        }

        #endregion IDs

        public string BuildFullConstructorSignature(bool generateMetadata)
        {
            return $"{StructName}({BuildConstructorSignature(generateMetadata)})";
        }

        public string BuildFullSignature()
        {
            return $"{ReturnType.Name} {Name}{(IsGeneric ? $"<{BuildGenericSignature()}>" : string.Empty)}({BuildSignature()}) {BuildGenericConstraint()}";
        }

        public string BuildFullExtensionSignature(string type, string name)
        {
            return $"{ReturnType.Name} {Name}{(IsGeneric ? $"<{BuildGenericSignature()}>" : string.Empty)}({BuildExtensionSignature(type, name)}) {BuildGenericConstraint()}";
        }

        public string BuildFullSignatureForCOM(bool generateMetadata)
        {
            return $"{ReturnType.Name} {Name}{(IsGeneric ? $"<{BuildGenericSignature()}>" : string.Empty)}({BuildSignature(generateMetadata)}) {BuildGenericConstraint()}";
        }

        public string BuildFullExtensionSignatureForCOM(string comObject, bool generateMetadata)
        {
            return $"{ReturnType.Name} {Name}{(IsGeneric ? $"<{BuildGenericSignature()}>" : string.Empty)}({BuildExtensionSignatureForCOM(comObject, generateMetadata)}) {BuildGenericConstraint()}";
        }

        public string BuildSignature(bool useAttributes = true, bool useNames = true)
        {
            StringBuilder sb = new();
            bool isFirst = true;
            for (int i = 0; i < Parameters.Count; i++)
            {
                var param = Parameters[i];
                var writeAttr = useAttributes && param.Attributes.Count > 0;

                if (param.DefaultValue != null)
                    continue;

                if (!isFirst)
                    sb.Append(", ");

                sb.Append($"{(writeAttr ? string.Join(" ", param.Attributes) + " " : string.Empty)}{param.Type}{(useNames ? " " + param.Name : string.Empty)}");
                isFirst = false;
            }

            return sb.ToString();
        }

        public string BuildConstructorSignature(bool useAttributes = true, bool useNames = true, bool useDefaults = true)
        {
            StringBuilder sb = new();
            bool isFirst = true;

            for (int i = 0; i < Parameters.Count; i++)
            {
                var param = Parameters[i];
                var writeAttr = useAttributes && param.Attributes.Count > 0;
                var writeDefault = useDefaults && param.DefaultValue != null;

                if (!isFirst)
                    sb.Append(", ");

                sb.Append($"{(writeAttr ? string.Join(" ", param.Attributes) + " " : string.Empty)}{param.Type}{(useNames ? " " + param.Name : string.Empty)}{(writeDefault ? $" = {param.DefaultValue}" : string.Empty)}");
                isFirst = false;
            }

            return sb.ToString();
        }

        public string BuildExtensionSignature(string type, string? name, bool useAttributes = true, bool useNames = true)
        {
            StringBuilder sb = new();
            sb.Append(useNames ? $"this {type} {name ?? string.Empty}" : $"this {type}");
            for (int i = 0; i < Parameters.Count; i++)
            {
                var param = Parameters[i];
                var writeAttr = useAttributes && param.Attributes.Count > 0;

                if (param.DefaultValue != null)
                    continue;

                sb.Append($", {(writeAttr ? string.Join(" ", param.Attributes) + " " : string.Empty)}{param.Type}{(useNames ? " " + param.Name : string.Empty)}");
            }

            return sb.ToString();
        }

        public string BuildExtensionSignatureForCOM(string comObject, bool useAttributes = true, bool useNames = true)
        {
            StringBuilder sb = new();
            sb.Append(useNames ? $"this ComPtr<{comObject}> comObj" : $"this ComPtr<{comObject}>");
            for (int i = 0; i < Parameters.Count; i++)
            {
                var param = Parameters[i];
                var writeAttr = useAttributes && param.Attributes.Count > 0;

                if (param.DefaultValue != null)
                    continue;

                sb.Append($", {(writeAttr ? string.Join(" ", param.Attributes) + " " : string.Empty)}{param.Type}{(useNames ? " " + param.Name : string.Empty)}");
            }

            return sb.ToString();
        }

        public string BuildGenericSignature()
        {
            return string.Join(", ", GenericParameters.Select(p => p.Name));
        }

        public string BuildGenericConstraint()
        {
            return string.Join(" ", GenericParameters.Select(p => p.Constrain));
        }

        public override string ToString()
        {
            return BuildSignature();
        }

        public bool HasParameter(CsParameterInfo cppParameter)
        {
            for (int i = 0; i < Parameters.Count; i++)
            {
                if (Parameters[i].Name == cppParameter.Name && Parameters[i].DefaultValue == cppParameter.DefaultValue)
                    return true;
            }
            return false;
        }

        public CsParameterInfo? GetParameter(string name)
        {
            for (int i = 0; i < Parameters.Count; i++)
            {
                var p = Parameters[i];
                if (p.Name == name)
                    return p;
            }
            return null;
        }

        public bool TryGetParameter(string name, [NotNullWhen(true)] out CsParameterInfo? parameter)
        {
            parameter = GetParameter(name);
            return parameter != null;
        }

        public CsFunctionVariation ShallowClone()
        {
            return new CsFunctionVariation(Identifier, ExportedName, Name, StructName, Kind, ReturnType.Clone());
        }

        public CsFunctionVariation Clone()
        {
            return new CsFunctionVariation(Identifier, ExportedName, Name, StructName, Kind, ReturnType.Clone(), Parameters.CloneValues(), GenericParameters.CloneValues(), Modifiers.Clone(), Attributes.Clone());
        }

        public static implicit operator ValueVariation(CsFunctionVariation variation)
        {
            return new ValueVariation(variation.Name, variation.Parameters);
        }
    }
}