namespace HexaGen.Tests
{
    using HexaGen.Core.Logging;
    using HexaGen.Core.Mapping;
    using Test;

    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        private static void EvaluateResult(BaseGenerator generator)
        {
            var messages = generator.Messages;
            int warns = 0;
            int errors = 0;
            for (int i = 0; i < messages.Count; i++)
            {
                var msg = messages[i];
                switch (msg.Severtiy)
                {
                    case LogSevertiy.Warning:
                        Assert.Warn(messages[i].Message);
                        warns++;
                        break;

                    case LogSevertiy.Error:
                        Assert.Warn(messages[i].Message);
                        errors++;
                        break;

                    case LogSevertiy.Critical:
                        Assert.Warn(messages[i].Message);
                        errors++;
                        break;
                }
            }

            if (errors > 0)
                Assert.Fail();
        }

        [Test]
        public void XAudio2()
        {
            CsCodeGeneratorSettings generatorSettings = CsCodeGeneratorSettings.Load("xaudio2/generator.json");
            CsComCodeGenerator generator1 = new(generatorSettings);
            Directory.CreateDirectory("../../../../XAudio2/Generated");
            generator1.Generate(Directory.GetFiles("xaudio2", "*.h").ToList(), "../../../../XAudio2/Generated");
            EvaluateResult(generator1);
            Assert.Pass();
        }

        [Test]
        public void X3DAudio()
        {
            CsCodeGeneratorSettings generatorSettings = CsCodeGeneratorSettings.Load("xd3audio/generator.json");
            CsComCodeGenerator generator1 = new(generatorSettings);
            Directory.CreateDirectory("../../../../X3DAudio/Generated");
            generator1.Generate("xd3audio/main.h", "../../../../X3DAudio/Generated");
            EvaluateResult(generator1);
            Assert.Pass();
        }

        [Test]
        public void D3DCommon()
        {
            CsCodeGeneratorSettings generatorSettings = CsCodeGeneratorSettings.Load("d3dcommon/generator.json");
            CsComCodeGenerator generator1 = new(generatorSettings);
            generator1.Generate("d3dcommon/d3dcommon.h", "../../../../D3DCommon/Generated");
            EvaluateResult(generator1);
            Assert.Pass();
        }

        [Test]
        public void D3DCompiler()
        {
            CsCodeGeneratorSettings generatorSettings = CsCodeGeneratorSettings.Load("d3dcompiler/generator.json");
            CsComCodeGenerator generator1 = new(generatorSettings);
            generator1.Generate("d3dcompiler/d3dcompiler.h", "../../../../D3DCompiler/Generated");
            EvaluateResult(generator1);
            Assert.Pass();
        }

        [Test]
        public void DXC()
        {
            CsCodeGeneratorSettings generatorSettings = CsCodeGeneratorSettings.Load("dxc/generator.json");
            CsComCodeGenerator generator1 = new(generatorSettings);
            generator1.Generate("dxc/main.h", "../../../../DXC/Generated");
            EvaluateResult(generator1);
            Assert.Pass();
        }

        [Test]
        public void DXGI()
        {
            CsCodeGeneratorSettings generatorSettings = CsCodeGeneratorSettings.Load("dxgi/generator.json");
            CsComCodeGenerator generator1 = new(generatorSettings);
            generator1.Generate(Directory.GetFiles("dxgi", "*.h").ToList(), "../../../../DXGI/Generated");
            EvaluateResult(generator1);
            Assert.Pass();
        }

        [Test]
        public void D3D11()
        {
            CsCodeGeneratorSettings generatorSettings = CsCodeGeneratorSettings.Load("d3d11/generator.json");
            CsComCodeGenerator generator1 = new(generatorSettings);
            generator1.Generate(Directory.GetFiles("d3d11", "*.h").ToList(), "../../../../D3D11/Generated");
            EvaluateResult(generator1);
            Assert.Pass();
        }

        [Test]
        public void D3D12()
        {
            CsCodeGeneratorSettings generatorSettings = CsCodeGeneratorSettings.Load("d3d12/generator.json");
            CsComCodeGenerator generator1 = new(generatorSettings);
            generator1.Generate(Directory.GetFiles("d3d12", "*.h").ToList(), "../../../../D3D12/Generated");
            EvaluateResult(generator1);
            Assert.Pass();
        }

        [Test]
        public void CImGui()
        {
            CsCodeGeneratorSettings settings = CsCodeGeneratorSettings.Load("cimgui/generator.json");
            ImguiDefinitions imguiDefinitions = new("cimgui");

            for (int i = 0; i < imguiDefinitions.Functions.Length; i++)
            {
                var functionDefinition = imguiDefinitions.Functions[i];
                for (int j = 0; j < functionDefinition.Overloads.Length; j++)
                {
                    var overload = functionDefinition.Overloads[j];
                    settings.FunctionMappings.Add(new(overload.ExportedName, overload.FriendlyName, overload.Comment, overload.DefaultValues, new()));

                    if (overload.IsMemberFunction && !overload.IsConstructor)
                    {
                        if (settings.KnownMemberFunctions.TryGetValue(overload.StructName, out var knownFunctions))
                        {
                            if (!knownFunctions.Contains(overload.ExportedName))
                                knownFunctions.Add(overload.ExportedName);
                        }
                        else
                        {
                            settings.KnownMemberFunctions.Add(overload.StructName, new List<string>() { overload.ExportedName });
                        }
                    }
                    else if (overload.IsConstructor)
                    {
                        if (settings.KnownConstructors.TryGetValue(overload.StructName, out var knownConstructors))
                        {
                            if (!knownConstructors.Contains(overload.ExportedName))
                                knownConstructors.Add(overload.ExportedName);
                        }
                        else
                        {
                            settings.KnownConstructors.Add(overload.StructName, new List<string>() { overload.ExportedName });
                        }
                    }
                }
            }

            for (int i = 0; i < imguiDefinitions.Enums.Length; i++)
            {
                var enumDefinition = imguiDefinitions.Enums[i];
                EnumMapping mapping = new(enumDefinition.Name, null, enumDefinition.Comment);
                for (int j = 0; j < enumDefinition.Members.Length; j++)
                {
                    var member = enumDefinition.Members[j];
                    EnumItemMapping valueMapping = new(member.Name, null, member.Comment, null);
                    mapping.ItemMappings.Add(valueMapping);
                }
                settings.EnumMappings.Add(mapping);
            }

            for (int i = 0; i < imguiDefinitions.Types.Length; i++)
            {
                var typeDefiniton = imguiDefinitions.Types[i];
                TypeMapping mapping = new(typeDefiniton.Name, null, typeDefiniton.Comment);
                for (int j = 0; j < typeDefiniton.Fields.Length; j++)
                {
                    var field = typeDefiniton.Fields[j];
                    TypeFieldMapping fieldMapping = new(field.Name, null, field.Comment);
                    mapping.FieldMappings.Add(fieldMapping);
                }
                settings.ClassMappings.Add(mapping);
            }

            string headerFile = "cimgui/cimgui.h";

            settings.DelegateMappings.Add(new("PlatformGetWindowPos", "Vector2*", "Vector2* pos, ImGuiViewport* viewport"));
            settings.DelegateMappings.Add(new("PlatformGetWindowSize", "Vector2*", "Vector2* size, ImGuiViewport* viewport"));

            CsCodeGenerator generator = new(settings);

            generator.Generate(headerFile, "../../../../ImGui/Generated");
            EvaluateResult(generator);
            Assert.Pass();
        }

        [Test]
        public void Shaderc()
        {
            CsCodeGeneratorSettings settings = CsCodeGeneratorSettings.Load("shaderc/generator.json");
            string headerFile = "shaderc/shaderc.h";

            CsCodeGenerator generator = new(settings);

            generator.Generate(headerFile, "../../../../Shaderc/Generated");
            EvaluateResult(generator);
            Assert.Pass();
        }

        [Test]
        public void SDL2()
        {
            CsCodeGeneratorSettings settings = CsCodeGeneratorSettings.Load("sdl2/generator.json");
            string headerFile = "sdl2/SDL.h";

            CsCodeGenerator generator = new(settings);

            generator.Generate(headerFile, "../../../../SDL2/Generated");
            EvaluateResult(generator);
            Assert.Pass();
        }
    }
}