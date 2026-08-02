namespace HexaGen.Cpp2C.GenerationSteps
{
    using HexaGen.CppAst.Model.Declarations;

    public class TypeNode
    {
        public readonly int Id;
        public readonly CppClass Type;
        public readonly HashSet<TypeNode> Dependencies = [];
        public TypeNodeFlags Flags;

        public TypeNode(int id, CppClass type)
        {
            Id = id;
            Type = type;
        }

        public void AddDependency(TypeNode type)
        {
            Dependencies.Add(type);
        }

        public override string ToString()
        {
            return Type.ToString();
        }
    }
}