namespace HexaGen.Core.CSharp
{
    public class CsGenericParameterInfo
    {
        public CsGenericParameterInfo(string name, string constrain)
        {
            Name = name;
            Constrain = constrain;
        }

        public string Name { get; set; }

        public string Constrain { get; set; }
    }
}