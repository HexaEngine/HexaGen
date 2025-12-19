namespace HexaGen.Core.CSharp
{
    using System.Collections.Generic;

    public readonly struct ValueVariation : IEquatable<ValueVariation>
    {
        private readonly string name;
        private readonly IList<CsParameterInfo> parameters;

        public ValueVariation(string name, IList<CsParameterInfo> parameters)
        {
            this.name = name;
            this.parameters = parameters;
        }

        public readonly string Name => name;

        public readonly IList<CsParameterInfo> Parameters => parameters;

        public override readonly bool Equals(object? obj)
        {
            return obj is ValueVariation variation && Equals(variation);
        }

        public readonly bool Equals(ValueVariation other)
        {
            if (other.parameters.Count != parameters.Count) return false;
            if (other.name != name) return false;
            for (int i = 0; i < parameters.Count; i++)
            {
                if (!other.parameters[i].Conflicts(parameters[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override readonly int GetHashCode()
        {
            HashCode code = new();
            code.Add(name);
            foreach (var parameter in parameters)
            {
                code.Add(parameter.Type.GetConflictHashCode());
            }
            return code.ToHashCode();
        }

        public static bool operator ==(ValueVariation left, ValueVariation right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ValueVariation left, ValueVariation right)
        {
            return !(left == right);
        }
    }
}