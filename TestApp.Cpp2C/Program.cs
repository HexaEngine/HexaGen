
using HexaGen.Cpp2C;

var config = Cpp2CGeneratorConfig.Load("config.json");
Cpp2CCodeGenerator generator = new(config);
generator.Generate("include/test.hpp", "Output");