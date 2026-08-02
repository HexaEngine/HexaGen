namespace HexaGen.Core.Profiling
{
    /// <summary>
    /// A utility class for rendering flame graph-style visualizations using ImGui.
    /// </summary>
    public static unsafe class ImGuiWidgetFlameGraph
    {
        /// <summary>
        /// A delegate that provides values for the flame graph rendering.
        /// </summary>
        /// <param name="start">Pointer to the start value.</param>
        /// <param name="end">Pointer to the end value.</param>
        /// <param name="level">Pointer to the level value.</param>
        /// <param name="caption">Pointer to the caption.</param>
        /// <param name="data">The data source.</param>
        /// <param name="idx">The index of the data.</param>
        public delegate void ValuesGetter(float* start, float* end, byte* level, byte** caption, void* data, int idx);
    }
}