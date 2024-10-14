namespace HexaGen.FunctionGeneration
{
    using HexaGen.Core.CSharp;
    using System.Linq;

    /// <summary>
    /// Represents a generator step that produces variations of functions
    /// with string pointer return types.
    /// </summary>
    public class StringReturnGenStep : FunctionGenStep
    {
        /// <summary>
        /// Determines whether the specified return type is allowed.
        /// </summary>
        /// <param name="function">The function to check.</param>
        /// <param name="variation">The variation to check.</param>
        /// <param name="returnType">The return type to check.</param>
        /// <returns>
        /// True if the return type is allowed; otherwise, false.
        /// </returns>
        protected virtual bool AllowReturnType(CsFunctionOverload function, CsFunctionVariation variation, CsType returnType)
        {
            return returnType.IsPointer && (returnType.PrimitiveType == CsPrimitiveType.Byte || returnType.PrimitiveType == CsPrimitiveType.Char);
        }

        /// <summary>
        /// Generates variations of the specified function and variation
        /// with string pointer return types.
        /// </summary>
        /// <param name="function">The function to generate variations for.</param>
        /// <param name="variation">The variation to generate variations for.</param>
        public override void GenerateVariations(CsFunctionOverload function, CsFunctionVariation variation)
        {
            if (AllowReturnType(function, variation, variation.ReturnType))
            {
                CsFunctionVariation returnVariation = variation.ShallowClone();
                CreateVariation(function, variation, returnVariation);
            }
        }

        /// <summary>
        /// Creates a new variation of the function with an updated return type of string.
        /// </summary>
        /// <param name="function">The original function.</param>
        /// <param name="variation">The original variation.</param>
        /// <param name="returnVariation">The new function variation to be updated.</param>
        protected virtual void CreateVariation(CsFunctionOverload function, CsFunctionVariation variation, CsFunctionVariation returnVariation)
        {
            returnVariation.Name += "S";
            returnVariation.GenericParameters.AddRange(variation.GenericParameters);
            returnVariation.Parameters = variation.Parameters.ToList();
            if (function.TryAddVariation(returnVariation))
            {
                returnVariation.ReturnType = new("string", variation.ReturnType.PrimitiveType);
            }
        }
    }
}