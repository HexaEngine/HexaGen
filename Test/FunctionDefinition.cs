namespace Test
{
    using System.Collections.Generic;

    internal class FunctionDefinition
    {
        public string Name { get; }
        public OverloadDefinition[] Overloads { get; }

        public FunctionDefinition(string name, OverloadDefinition[] overloads, EnumDefinition[] enums)
        {
            Name = name;
            Overloads = ExpandOverloadVariants(overloads, enums);
        }

        private static OverloadDefinition[] ExpandOverloadVariants(OverloadDefinition[] overloads, EnumDefinition[] enums)
        {
            List<OverloadDefinition> newDefinitions = new();

            foreach (OverloadDefinition overload in overloads)
            {
                bool hasVariants = false;
                int[] variantCounts = new int[overload.Parameters.Length];

                for (int i = 0; i < overload.Parameters.Length; i++)
                {
                    var parameter = overload.Parameters[i];
                    if (parameter.TypeVariants != null)
                    {
                        hasVariants = true;
                        variantCounts[i] = parameter.TypeVariants.Length + 1;
                    }
                    else
                    {
                        variantCounts[i] = 1;
                    }
                }

                if (hasVariants)
                {
                    int totalVariants = variantCounts[0];
                    for (int i = 1; i < variantCounts.Length; i++) totalVariants *= variantCounts[i];

                    for (int i = 0; i < totalVariants; i++)
                    {
                        TypeReference[] parameters = new TypeReference[overload.Parameters.Length];
                        int div = 1;

                        for (int j = 0; j < parameters.Length; j++)
                        {
                            int k = i / div % variantCounts[j];

                            parameters[j] = overload.Parameters[j].WithVariant(k, enums);

                            if (j > 0) div *= variantCounts[j];
                        }

                        newDefinitions.Add(overload.WithParameters(parameters));
                    }
                }
                else
                {
                    newDefinitions.Add(overload);
                }
            }

            return newDefinitions.ToArray();
        }
    }
}