namespace HexaGen.Language
{
    public enum AnalyserResult
    {
        /// <summary>
        /// Successfully analysed the tokens
        /// </summary>
        Success,

        /// <summary>
        /// Not recognised, by the analyser.
        /// </summary>
        Unrecognised,

        /// <summary>
        /// Has recognised the tokens, but errored.
        /// </summary>
        Error,
    }
}