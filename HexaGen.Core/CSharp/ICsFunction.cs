namespace HexaGen.Core.CSharp
{
    using System.Collections.Generic;

    public interface ICsFunction
    {
        string ExportedName { get; set; }
        public CsFunctionKind Kind { get; set; }
        string Name { get; set; }
        List<CsParameterInfo> Parameters { get; set; }
        CsType ReturnType { get; set; }
        string StructName { get; set; }

        bool HasParameter(CsParameterInfo cppParameter);

        string ToString();
    }
}