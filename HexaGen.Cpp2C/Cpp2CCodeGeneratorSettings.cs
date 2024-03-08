namespace HexaGen.Cpp2C
{
    using HexaGen.Core.Logging;

    public class Cpp2CCodeGeneratorSettings
    {
        /// <summary>
        /// The log level of the generator. (Default <see cref="LogSevertiy.Warning"/>)
        /// </summary>
        public LogSevertiy LogLevel { get; set; } = LogSevertiy.Warning;

        /// <summary>
        /// The log level of the Clang Compiler. (Default <see cref="LogSevertiy.Error"/>)
        /// </summary>
        public LogSevertiy CppLogLevel { get; set; } = LogSevertiy.Error;

        /// <summary>
        /// List of the include folders. (Default: Empty)
        /// </summary>
        public List<string> IncludeFolders { get; set; } = new();

        /// <summary>
        /// List of the system include folders. (Default: Empty)
        /// </summary>
        public List<string> SystemIncludeFolders { get; set; } = new();

        /// <summary>
        /// List of macros passed to CppAst. (Default: Empty)
        /// </summary>
        public List<string> Defines { get; set; } = new();

        /// <summary>
        /// List of the additional arguments passed directly to the C++ Clang compiler. (Default: Empty)
        /// </summary>
        public List<string> AdditionalArguments { get; set; } = new();
    }
}