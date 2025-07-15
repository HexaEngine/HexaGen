namespace HexaGen.Core.CSharp
{
    using Newtonsoft.Json;

    public class CsGenericParameterInfo : ICloneable<CsGenericParameterInfo>
    {
        [JsonConstructor]
        public CsGenericParameterInfo(string name, string constrain)
        {
            Name = name;
            Constrain = constrain;
        }

        public string Name { get; set; }

        public string Constrain { get; set; }

        public CsGenericParameterInfo Clone()
        {
            return new CsGenericParameterInfo(Name, Constrain);
        }
    }
}