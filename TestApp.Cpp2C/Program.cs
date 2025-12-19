
using HexaGen.Cpp2C;
using HexaGen.Cpp2C.GenerationSteps;

var config = Cpp2CGeneratorConfig.Load("config.json");
Cpp2CCodeGenerator generator = new(config);
generator.AddGenerationStep<EnumGenerationStep>();
generator.AddGenerationStep<ClassGenerationStep>();
generator.Generate("include/prism.hpp", "Output");