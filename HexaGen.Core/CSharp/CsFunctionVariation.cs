namespace HexaGen.Core.CSharp
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    public class CsFunctionVariation : ICsFunction, ICloneable<CsFunctionVariation>
    {
        public CsFunctionVariation(string exportedName, string name, string structName, bool isMember, bool isConstructor, bool isDestructor, CsType returnType, List<CsParameterInfo> parameters, List<CsGenericParameterInfo> genericParameters, List<string> modifiers, List<string> attributes)
        {
            ExportedName = exportedName;
            Name = name;

            StructName = structName;
            IsMember = isMember;
            IsConstructor = isConstructor;
            IsDestructor = isDestructor;
            ReturnType = returnType;
            Parameters = parameters;
            GenericParameters = genericParameters;
            Modifiers = modifiers;
            Attributes = attributes;
        }

        public CsFunctionVariation(string exportedName, string name, string structName, bool isMember, bool isConstructor, bool isDestructor, CsType returnType)
        {
            ExportedName = exportedName;
            Name = name;
            StructName = structName;
            IsMember = isMember;
            IsConstructor = isConstructor;
            IsDestructor = isDestructor;
            ReturnType = returnType;
            Parameters = new();
            GenericParameters = new();
            Modifiers = new();
            Attributes = new();
        }

        public string ExportedName { get; set; }

        public string Name { get; set; }

        public string StructName { get; set; }

        public bool IsMember { get; set; }

        public bool IsConstructor { get; set; }

        public bool IsDestructor { get; set; }

        public bool IsGeneric => GenericParameters.Count > 0;

        public CsType ReturnType { get; set; }

        public List<CsParameterInfo> Parameters { get; set; }

        public List<CsGenericParameterInfo> GenericParameters { get; set; }

        public List<string> Modifiers { get; set; }

        public List<string> Attributes { get; set; }

        #region IDs

        public string BuildSignatureIdentifier()
        {
            return $"{Name}{(IsGeneric ? $"<{BuildGenericSignature()}>" : string.Empty)}({BuildSignature(false, false)})";
        }

        public string BuildConstructorSignatureIdentifier()
        {
            return $"{StructName}({BuildConstructorSignature(false, false, false)})";
        }

        public string BuildExtensionSignatureIdentifier(string type)
        {
            return $"{Name}{(IsGeneric ? $"<{BuildGenericSignature()}>" : string.Empty)}({BuildExtensionSignature(type, null, false, false)})";
        }

        public string BuildSignatureIdentifierForCOM()
        {
            return $"{ReturnType.Name} {Name}{(IsGeneric ? $"<{BuildGenericSignature()}>" : string.Empty)}({BuildSignature(false, false)})";
        }

        public string BuildExtensionSignatureIdentifierForCOM(string comObject)
        {
            return $"{ReturnType.Name} {Name}{(IsGeneric ? $"<{BuildGenericSignature()}>" : string.Empty)}({BuildExtensionSignatureForCOM(comObject, false, false)})";
        }

        #endregion IDs

        public string BuildFullConstructorSignature()
        {
            return $"{StructName}({BuildConstructorSignature()})";
        }

        public string BuildFullSignature()
        {
            return $"{ReturnType.Name} {Name}{(IsGeneric ? $"<{BuildGenericSignature()}>" : string.Empty)}({BuildSignature()}) {BuildGenericConstraint()}";
        }

        public string BuildFullExtensionSignature(string type, string name)
        {
            return $"{ReturnType.Name} {Name}{(IsGeneric ? $"<{BuildGenericSignature()}>" : string.Empty)}({BuildExtensionSignature(type, name)}) {BuildGenericConstraint()}";
        }

        public string BuildFullSignatureForCOM()
        {
            return $"{ReturnType.Name} {Name}{(IsGeneric ? $"<{BuildGenericSignature()}>" : string.Empty)}({BuildSignature()}) {BuildGenericConstraint()}";
        }

        public string BuildFullExtensionSignatureForCOM(string comObject)
        {
            return $"{ReturnType.Name} {Name}{(IsGeneric ? $"<{BuildGenericSignature()}>" : string.Empty)}({BuildExtensionSignatureForCOM(comObject)}) {BuildGenericConstraint()}";
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

        public CsFunctionVariation Clone()
        {
            return new CsFunctionVariation(ExportedName, Name, StructName, IsMember, IsConstructor, IsDestructor, ReturnType.Clone(), Parameters.CloneValues(), GenericParameters.CloneValues(), Modifiers.Clone(), Attributes.Clone());
        }
    }
}