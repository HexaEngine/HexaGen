namespace HexaGen.Core.CSharp
{
    using System;
    using System.Collections.Generic;

    public class CsFunctionVariation : ICsFunction
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

        public string BuildSignatureIdentifier()
        {
            return $"{ReturnType.Name} {Name}{(IsGeneric ? $"<{BuildGenericSignature()}>" : string.Empty)}({BuildSignature(false, false)}) {BuildGenericConstraint()}";
        }

        public string BuildSignatureIdentifierForCOM()
        {
            return $"{ReturnType.Name} {Name}{(IsGeneric ? $"<{BuildGenericSignature()}>" : string.Empty)}({BuildSignature(false, false)}) {BuildGenericConstraint()}";
        }

        public string BuildExtensionSignatureIdentifierForCOM(string comObject)
        {
            return $"{ReturnType.Name} {Name}{(IsGeneric ? $"<{BuildGenericSignature()}>" : string.Empty)}({BuildExtensionSignatureForCOM(comObject, false, false)}) {BuildGenericConstraint()}";
        }

        public string BuildFullExtensionSignatureForCOM(string comObject)
        {
            return $"{ReturnType.Name} {Name}{(IsGeneric ? $"<{BuildGenericSignature()}>" : string.Empty)}({BuildExtensionSignatureForCOM(comObject)}) {BuildGenericConstraint()}";
        }

        public string BuildFullSignatureForCOM()
        {
            return $"{ReturnType.Name} {Name}{(IsGeneric ? $"<{BuildGenericSignature()}>" : string.Empty)}({BuildSignature()}) {BuildGenericConstraint()}";
        }

        public string BuildSignature(bool useAttributes = true, bool useNames = true)
        {
            return string.Join(", ", Parameters.Select(x => $"{(useAttributes ? string.Join(" ", x.Attributes) : string.Empty)} {x.Type} {(useNames ? x.Name : string.Empty)}"));
        }

        public string BuildExtensionSignatureForCOM(string comObject, bool useAttributes = true, bool useNames = true)
        {
            return string.Join(", ", Parameters.Select(x => $"{(useAttributes ? string.Join(" ", x.Attributes) : string.Empty)} {x.Type} {(useNames ? x.Name : string.Empty)}").Reverse().Append(useNames ? $"this ComPtr<{comObject}> comObj" : $"this ComPtr<{comObject}>").Reverse());
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
                if (Parameters[i].Name == cppParameter.Name)
                    return true;
            }
            return false;
        }
    }
}