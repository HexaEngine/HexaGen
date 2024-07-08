namespace SPIRVTest
{
    using Hexa.NET.Shaderc;
    using Hexa.NET.SPIRVReflect;
    using HexaGen.Runtime;

    internal class Program
    {
        private static unsafe void Main(string[] args)
        {
            ShadercCompileOptions options = Shaderc.ShadercCompileOptionsInitialize();

            options.SetSourceLanguage(ShadercSourceLanguage.Hlsl);
            options.SetTargetSpirv(ShadercSpirvVersion.Version13);
            options.SetTargetEnv(ShadercTargetEnv.Vulkan, (uint)ShadercEnvVersion.Vulkan11);

            ShadercCompiler compiler = Shaderc.ShadercCompilerInitialize();

            string text = File.ReadAllText("BasicVertexShader.hlsl");

            ShadercCompilationResult result = compiler.CompileIntoSpv(text, (nuint)text.Length, ShadercShaderKind.VertexShader, "BasicVertexShader.hlsl", "VS", options);

            if (result.GetCompilationStatus() != ShadercCompilationStatus.Success)
            {
                Console.WriteLine(result.GetErrorMessageS());
                throw new Exception();
            }

            options.Release();
            compiler.Release();

            SpvReflectShaderModule module = default;
            SPIRV.SpvReflectCreateShaderModule(result.GetLength(), result.GetBytes(), ref module);

            result.Release();

            for (int i = 0; i < module.InputVariableCount; i++)
            {
                var inputVar = module.InputVariables[i];
                Console.WriteLine(inputVar->Format);
                Console.WriteLine(Utils.DecodeStringUTF8(inputVar->Name));
            }

            SPIRV.SpvReflectDestroyShaderModule(&module);
        }
    }
}