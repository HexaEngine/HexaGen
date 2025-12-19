using HexaGen.Core;
using HexaGen.Cpp2C.Metadata;
using HexaGen.CppAst.Model.Declarations;
using HexaGen.CppAst.Model.Interfaces;

namespace HexaGen.Cpp2C.GenerationSteps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EnumGenerationStep : GenerationStep
    {
        public EnumGenerationStep(Cpp2CCodeGenerator generator, Cpp2CGeneratorConfig config) : base(generator, config)
        {
        }

        public override string Name { get; } = "Enums";

        public override void Configure(Cpp2CGeneratorConfig config)
        {
        }

        public override void CopyToMetadata(Cpp2CGeneratorMetadata metadata)
        {
        }

        public override void CopyFromMetadata(Cpp2CGeneratorMetadata metadata)
        {
        }

        public override void Reset()
        {
        }

        public override void Generate(FileSet files, ParseResult result, string outputPath, Cpp2CGeneratorConfig config,
            Cpp2CGeneratorMetadata metadata)
        {
            var fileName = Path.Combine(outputPath, "include", "enums.h");
            using CodeWriter writer = new CodeWriter(fileName, "", null);
            WriteEnums(config.NamePrefix, writer, result.Compilation.Enums);

            foreach (var ns in result.Compilation.EnumerateNamespaces())
            {
                var name = ns.GetFullNamespace("::");
                writer.WriteLine($"// begin namespace {name}");
                WriteEnums(ns.GetFullNamespace("_"), writer, ns.Enums);
                writer.WriteLine($"// end namespace {name}");
            }
        }

        private void WriteEnums(string prefix, CodeWriter writer, IEnumerable<CppEnum> enums)
        {
            foreach (var enumClass in enums)
            {
                WriteEnum(prefix, writer, enumClass);
            }
        }

        private void WriteEnum(string prefix, ICodeWriter writer, CppEnum enumClass)
        {
            Dictionary<string, string> map = [];
            string enumName = $"{prefix}{enumClass.Name}";
            foreach (var item in enumClass.Items)
            {
                map[item.Name] = $"{enumName}_{item.Name}";
            }
            writer.BeginBlock("typedef enum");
            foreach (var item in enumClass.Items)
            {
                WriteEnumItem(writer, map[item.Name], item);
            }
            writer.EndBlock($"}} {enumName};");
        }

        private void WriteEnumItem(ICodeWriter writer, string enumName, CppEnumItem item)
        {
            writer.WriteLine($"{enumName} = {item.Value},");
        }
    }
}