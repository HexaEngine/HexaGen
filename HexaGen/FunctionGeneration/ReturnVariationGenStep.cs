namespace HexaGen.FunctionGeneration
{
    using HexaGen.Core.CSharp;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a generator step that produces variations of functions
    /// based on return type and parameter conditions.
    /// </summary>
    public class ReturnVariationGenStep : FunctionGenStep
    {
        /// <summary>
        /// Determines whether to skip the given function and variation.
        /// </summary>
        /// <param name="function">The function to check.</param>
        /// <param name="variation">The variation to check.</param>
        /// <returns>
        /// True if the function should be skipped; otherwise, false.
        /// </returns>
        public override bool ShouldSkip(CsFunctionOverload function, CsFunctionVariation variation)
        {
            return function.Kind == CsFunctionKind.Member;
        }

        /// <summary>
        /// Gets the set of allowed parameter names.
        /// </summary>
        public virtual HashSet<string> AllowedParameterNames { get; } = ["pOut"];

        /// <summary>
        /// Determines whether the specified parameter is allowed.
        /// </summary>
        /// <param name="parameter">The parameter to check.</param>
        /// <returns>
        /// True if the parameter is allowed; otherwise, false.
        /// </returns>
        protected virtual bool IsParameterAllowed(CsParameterInfo parameter)
        {
            return AllowedParameterNames.Contains(parameter.Name) && parameter.Type.IsPointer;
        }

        /// <summary>
        /// Generates variations of the specified function and variation.
        /// </summary>
        /// <param name="function">The function to generate variations for.</param>
        /// <param name="variation">The variation to generate variations for.</param>
        public override void GenerateVariations(CsFunctionOverload function, CsFunctionVariation variation)
        {
            if (variation.ReturnType.IsVoid && !variation.ReturnType.IsPointer && variation.Parameters.Count > 0)
            {
                var param = variation.Parameters[0];
                if (IsParameterAllowed(param))
                {
                    CsFunctionVariation returnVariation = variation.ShallowClone();
                    CreateVariation(function, variation, param, returnVariation);
                }
            }
        }

        /// <summary>
        /// Creates a new variation of the function with updated return type
        /// based on the specified parameter.
        /// </summary>
        /// <param name="function">The original function.</param>
        /// <param name="variation">The original variation.</param>
        /// <param name="parameter">The parameter to base the new variation on.</param>
        /// <param name="returnVariation">The new function variation to be updated.</param>
        protected virtual void CreateVariation(CsFunctionOverload function, CsFunctionVariation variation, CsParameterInfo parameter, CsFunctionVariation returnVariation)
        {
            returnVariation.GenericParameters.AddRange(variation.GenericParameters);
            returnVariation.Parameters = variation.Parameters.Skip(1).ToList();
            if (function.TryUpdateVariation(variation, returnVariation))
            {
                returnVariation.ReturnType = new(parameter.Type.Name[..^1], parameter.Type.PrimitiveType);
            }
        }
    }
}