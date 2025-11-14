namespace HexaGen.Cpp2C
{
    using HexaGen.Core.Logging;
    using System.ComponentModel;

    public partial class Cpp2CCodeGeneratorConfig
    {
        [DefaultValue(null)]
        public BaseConfig? BaseConfig { get; set; }

        /// <summary>
        /// The log level of the generator. (Default <see cref="LogSeverity.Warning"/>)
        /// </summary>
        public LogSeverity LogLevel { get; set; } = LogSeverity.Warning;

        /// <summary>
        /// The log level of the Clang Compiler. (Default <see cref="LogSeverity.Error"/>)
        /// </summary>
        public LogSeverity CppLogLevel { get; set; } = LogSeverity.Error;

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