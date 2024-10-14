namespace HexaGen.FunctionGeneration
{
    using HexaGen.Core.CSharp;

    /// <summary>
    /// Represents a generator step that produces variations of functions
    /// by applying default values to parameters.
    /// </summary>
    public class DefaultValueGenStep : FunctionGenStep
    {
        /// <summary>
        /// Generates variations of the specified function and variation
        /// by applying default values to parameters.
        /// </summary>
        /// <param name="function">The function to generate variations for.</param>
        /// <param name="variation">The variation to generate variations for.</param>
        public override void GenerateVariations(CsFunctionOverload function, CsFunctionVariation variation)
        {
            if (function.DefaultValues.Count == 0)
                return;

            for (int i = variation.Parameters.Count - 1; i >= 0; i--)
            {
                var param = variation.Parameters[i];

                if (!function.DefaultValues.TryGetValue(param.Name, out var defaultValue))
                {
                    continue;
                }

                TransformDefaultValue(function, variation, param, ref defaultValue);

                CsFunctionVariation defaultVariation = variation.ShallowClone();
                defaultVariation.GenericParameters.AddRange(variation.GenericParameters);
                for (int j = 0; j < variation.Parameters.Count; j++)
                {
                    var variationParameter = variation.Parameters[j];
                    if (param != variationParameter)
                    {
                        defaultVariation.Parameters.Add(variationParameter);
                    }
                    else
                    {
                        var cloned = variationParameter.Clone();
                        cloned.DefaultValue = defaultValue;
                        defaultVariation.Parameters.Add(cloned);
                    }
                }

                if (!function.TryAddVariation(defaultVariation))
                    continue;

                if (function.Kind != CsFunctionKind.Constructor)
                {
                    DoSubStep(function, defaultVariation);
                }
                else
                {
                    DoSubStep<DefaultValueGenStep>(function, defaultVariation);
                }
            }
        }

        /// <summary>
        /// Transforms the default value of a parameter before applying it to a variation.
        /// </summary>
        /// <param name="function">The function being processed.</param>
        /// <param name="variation">The variation being processed.</param>
        /// <param name="parameter">The parameter whose default value is being transformed.</param>
        /// <param name="defaultValue">The default value to transform.</param>
        protected virtual void TransformDefaultValue(CsFunctionOverload function, CsFunctionVariation variation, CsParameterInfo parameter, ref string defaultValue)
        {
        }
    }
}