namespace HexaGen
{
    using HexaGen.Core.CSharp;
    using HexaGen.CppAst.Model.Types;

    public class CsSubClass
    {
        public CppType CppType { get; set; }

        public CsType Type { get; set; }

        public string Name { get; set; }

        public string CppFieldName { get; set; }

        public string FieldName { get; set; }

        public CsSubClass(CppType type, string name, string cppFieldName, string fieldName)
        {
            Type = new(name, name, false, false, false, false, false, false, false, false, false, false, CsStringType.None, CsPrimitiveType.Unknown);
            CppType = type;
            Name = name;
            CppFieldName = cppFieldName;
            FieldName = fieldName;
        }
    }
}