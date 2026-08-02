using HexaGen.Core;
using HexaGen.Cpp2C;
using HexaGen.Cpp2C.GenerationSteps;
using HexaGen.CppAst.Model.Declarations;

var config = Cpp2CGeneratorConfig.Load("config.json");
config.TemplateDissolvers.Add(new PrismObjDissolver());
Cpp2CCodeGenerator generator = new(config);

generator.AddGenerationStep<EnumGenerationStep>();
generator.AddGenerationStep<ClassGenerationStep>();

generator.Generate("include/prism.hpp", "Output/Gen");

internal class PrismObjDissolver : TemplateDissolver
{
    public override bool CanDissolve(Cpp2CGeneratorConfig config, CppClass type) => type.SpecializedTemplate?.Name == "PrismObj";

    public override CType GetDissolvedType(Cpp2CGeneratorConfig config, CppClass type)
    {
        return new(config.GetCType(type.TemplateSpecializedArguments[0].ArgAsType!, null) + "*");
    }

    public override string WriteDissolveFunc(Cpp2CGeneratorConfig config, ICodeWriter writer, CppClass cppType, CType cType)
    {
        writer.WriteLine("template <typename T>");
        writer.PushBlock("static inline T* DissolvePrismObj(PrismObj<T>& ptr)");
        writer.WriteLine("return ptr.Detach()");

        return "DissolvePrismObj";
    }
}