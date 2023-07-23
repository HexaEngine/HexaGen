// See https://aka.ms/new-console-template for more information
using HexaGen;
using HexaGen.Core.Mapping;
using System.Reflection.Emit;
using Test;

CsCodeGeneratorSettings generatorSettings = CsCodeGeneratorSettings.Load("d3d11/generator.json");
CsComCodeGenerator generator1 = new(generatorSettings);
generator1.Generate(new List<string>() { "d3d11/d3d11.h", "d3d11/d3d11_1.h", "d3d11/d3d11.h", "d3d11/d3d11_2.h", "d3d11/d3d11_3.h", "d3d11/d3d11_4.h" }, "../../../../D3D11/Generated");
generator1.DisplayMessages();
return;
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
        settings.TypeMappings.Add(mapping);
    }

    string headerFile = "cimgui/cimgui.h";

    settings.DelegateMappings.Add(new("PlatformGetWindowPos", "Vector2*", "Vector2* pos, ImGuiViewport* viewport"));
    settings.DelegateMappings.Add(new("PlatformGetWindowSize", "Vector2*", "Vector2* size, ImGuiViewport* viewport"));

    CsCodeGenerator generator = new(settings);

    generator.Generate(headerFile, "../../../../ImGui/Generated");
    generator.DisplayMessages();
}
{
    CsCodeGeneratorSettings settings = CsCodeGeneratorSettings.Load("shaderc/generator.json");
    string headerFile = "shaderc/shaderc.h";

    CsCodeGenerator generator = new(settings);

    generator.Generate(headerFile, "../../../../Shaderc/Generated");
    generator.DisplayMessages();
}