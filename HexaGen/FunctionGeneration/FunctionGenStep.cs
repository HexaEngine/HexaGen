namespace HexaGen.FunctionGeneration
{
    using HexaGen.Core.CSharp;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an abstract base class for function generation steps,
    /// providing methods to generate variations of functions and
    /// execute sub-steps.
    /// </summary>
    public abstract class FunctionGenStep
    {
        /// <summary>
        /// Gets or sets the list of function generation steps.
        /// </summary>
        public List<FunctionGenStep> GenSteps = []; // default to empty list, to avoid problems later with null checks

        /// <summary>
        /// Determines whether to skip the given function and variation.
        /// </summary>
        /// <param name="function">The function to check.</param>
        /// <param name="variation">The variation to check.</param>
        /// <returns>
        /// True if the function should be skipped; otherwise, false.
        /// </returns>
        public virtual bool ShouldSkip(CsFunctionOverload function, CsFunctionVariation variation)
        {
            return false;
        }

        /// <summary>
        /// Generates variations of the specified function and variation.
        /// Must be implemented by derived classes.
        /// </summary>
        /// <param name="function">The function to generate variations for.</param>
        /// <param name="variation">The variation to generate variations for.</param>
        public abstract void GenerateVariations(CsFunctionOverload function, CsFunctionVariation variation);

        /// <summary>
        /// Executes sub-steps of the specified type for the given function and variation.
        /// </summary>
        /// <typeparam name="T">The type of sub-step to execute.</typeparam>
        /// <param name="function">The function to process with sub-steps.</param>
        /// <param name="variation">The variation to process with sub-steps.</param>
        public void DoSubStep<T>(CsFunctionOverload function, CsFunctionVariation variation) where T : FunctionGenStep
        {
            foreach (var step in GenSteps)
            {
                if (step.ShouldSkip(function, variation)) continue;
                if (step is T t)
                {
                    t.GenerateVariations(function, variation);
                }
            }
        }

        /// <summary>
        /// Executes all sub-steps for the given function and variation.
        /// </summary>
        /// <param name="function">The function to process with sub-steps.</param>
        /// <param name="variation">The variation to process with sub-steps.</param>
        public void DoSubStep(CsFunctionOverload function, CsFunctionVariation variation)
        {
            foreach (var step in GenSteps)
            {
                if (step.ShouldSkip(function, variation)) continue;
                step.GenerateVariations(function, variation);
            }
        }
    }
}