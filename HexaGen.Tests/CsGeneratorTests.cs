namespace HexaGen.Tests
{
    using CppAst;
    using HexaGen.Core.Logging;
    using HexaGen.Core.Mapping;
    using System;
    using System.Text;
    using Test;

    public class CsGeneratorTests
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
            generator1.Generate(Directory.GetFiles("xaudio2", "*.h").ToList(), "../../../../Hexa.NET.XAudio2/Generated");
            EvaluateResult(generator1);
            Assert.Pass();
        }

        [Test]
        public void X3DAudio()
        {
            CsCodeGeneratorSettings generatorSettings = CsCodeGeneratorSettings.Load("xd3audio/generator.json");
            CsComCodeGenerator generator1 = new(generatorSettings);
            generator1.Generate("xd3audio/main.h", "../../../../Hexa.NET.X3DAudio/Generated");
            EvaluateResult(generator1);
            Assert.Pass();
        }

        [Test]
        public void D3DCommon()
        {
            CsCodeGeneratorSettings generatorSettings = CsCodeGeneratorSettings.Load("d3dcommon/generator.json");
            CsComCodeGenerator generator1 = new(generatorSettings);
            generator1.Generate("d3dcommon/d3dcommon.h", "../../../../Hexa.NET.D3DCommon/Generated");
            EvaluateResult(generator1);
            Assert.Pass();
        }

        [Test]
        public void D3DCompiler()
        {
            CsCodeGeneratorSettings generatorSettings = CsCodeGeneratorSettings.Load("d3dcompiler/generator.json");
            CsComCodeGenerator generator1 = new(generatorSettings);
            generator1.Generate("d3dcompiler/d3dcompiler.h", "../../../../Hexa.NET.D3DCompiler/Generated");
            EvaluateResult(generator1);
            Assert.Pass();
        }

        [Test]
        public void DXC()
        {
            CsCodeGeneratorSettings generatorSettings = CsCodeGeneratorSettings.Load("dxc/generator.json");
            CsComCodeGenerator generator1 = new(generatorSettings);
            generator1.Generate("dxc/main.h", "../../../../Hexa.NET.DXC/Generated");
            EvaluateResult(generator1);
            Assert.Pass();
        }

        [Test]
        public void DXGI()
        {
            CsCodeGeneratorSettings generatorSettings = CsCodeGeneratorSettings.Load("dxgi/generator.json");
            CsComCodeGenerator generator1 = new(generatorSettings);
            generator1.Generate(Directory.GetFiles("dxgi", "*.h").ToList(), "../../../../Hexa.NET.DXGI/Generated");
            EvaluateResult(generator1);
            Assert.Pass();
        }

        [Test]
        public void D3D11()
        {
            CsCodeGeneratorSettings generatorSettings = CsCodeGeneratorSettings.Load("d3d11/generator.json");
            CsComCodeGenerator generator1 = new(generatorSettings);
            generator1.Generate(Directory.GetFiles("d3d11", "*.h").ToList(), "../../../../Hexa.NET.D3D11/Generated");
            EvaluateResult(generator1);
            Assert.Pass();
        }

        [Test]
        public void D3D12()
        {
            CsCodeGeneratorSettings generatorSettings = CsCodeGeneratorSettings.Load("d3d12/generator.json");
            CsComCodeGenerator generator1 = new(generatorSettings);
            generator1.Generate(Directory.GetFiles("d3d12", "*.h").ToList(), "../../../../Hexa.NET.D3D12/Generated");
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

            generator.Generate(headerFile, "../../../../Hexa.NET.ImGui/Generated");
            EvaluateResult(generator);
            Assert.Pass();
        }

        [Test]
        public void Shaderc()
        {
            CsCodeGeneratorSettings settings = CsCodeGeneratorSettings.Load("shaderc/generator.json");
            string headerFile = "shaderc/shaderc.h";

            CsCodeGenerator generator = new(settings);

            generator.Generate(headerFile, "../../../../Hexa.NET.Shaderc/Generated");
            EvaluateResult(generator);
            Assert.Pass();
        }

        [Test]
        public void SPIRVCross()
        {
            CsCodeGeneratorSettings settings = CsCodeGeneratorSettings.Load("spirvcross/generator.json");
            string headerFile = "spirvcross/spirv_cross_c.h";

            CsCodeGenerator generator = new(settings);

            generator.Generate(headerFile, "../../../../Hexa.NET.SPIRVCross/Generated");
            EvaluateResult(generator);
            Assert.Pass();
        }

        [Test]
        public void SPIRVReflect()
        {
            CsCodeGeneratorSettings settings = CsCodeGeneratorSettings.Load("spirvreflect/generator.json");
            string headerFile = "spirvreflect/spirv_reflect.h";

            CsCodeGenerator generator = new(settings);

            generator.Generate(headerFile, "../../../../Hexa.NET.SPIRVReflect/Generated");
            EvaluateResult(generator);
            Assert.Pass();
        }

        [Test]
        public void SDL2()
        {
            CsCodeGeneratorSettings settings = CsCodeGeneratorSettings.Load("sdl2/generator.json");
            settings.CustomEnumItemMapper = SDL2EnumMapper;
            CsCodeGenerator generator = new(settings);
            generator.Generate(new List<string>() { "sdl2/main.h" }, "../../../../Hexa.NET.SDL2/Generated");
            generator.SaveMetadata("sdl2.json");
            EvaluateResult(generator);
            Assert.Pass();
        }

        private void SDL2EnumMapper(CppEnum cppEnum, CppEnumItem cppEnumItem, CsEnumMetadata csEnum, CsEnumItemMetadata csEnumItem)
        {
        }

        [Test]
        public void OpenAL()
        {
            CsCodeGeneratorSettings settings = CsCodeGeneratorSettings.Load("openal/generator.json");
            string headerFile = "openal/main.h";

            CsCodeGenerator generator = new(settings);

            generator.Generate(headerFile, "../../../../Hexa.NET.OpenAL/Generated");
            EvaluateResult(generator);
            Assert.Pass();
        }

        [Test]
        public void OpenGL()
        {
            CsCodeGeneratorSettings settings = CsCodeGeneratorSettings.Load("opengl/generator.json");
            string headerFile = "opengl/main.h";

            CsCodeGenerator generator = new(settings);

            generator.Generate(headerFile, "../../../../Hexa.NET.OpenGL/Generated");
            generator.SaveMetadata("opengl.json");
            EvaluateResult(generator);
            Assert.Pass();
        }

        [Test]
        public void OpenGLExt()
        {
            CsCodeGeneratorSettings settings = CsCodeGeneratorSettings.Load("opengl/glext/generator.json");
            string headerFile = "opengl/glext/main.h";

            CsCodeGenerator generator = new(settings);

            generator.LoadMetadata("opengl.json");
            generator.Generate(headerFile, "../../../../Hexa.NET.OpenGL/Extensions/GLExt/Generated");
            EvaluateResult(generator);
            Assert.Pass();
        }

        [Test]
        public void PhysX()
        {
            CsCodeGeneratorSettings settings = CsCodeGeneratorSettings.Load("physx/generator.json");
            string headerFile = "physx/physx_generated.hpp";

            CsCodeGenerator generator = new(settings);

            generator.Generate(headerFile, "../../../../Hexa.NET.PhysX/Generated");
            EvaluateResult(generator);
            Assert.Pass();
        }

        [Test]
        public void FreeType()
        {
            CsCodeGeneratorSettings settings = CsCodeGeneratorSettings.Load("freetype/generator.json");

            EnumMapping enumMappingFTGlyphFormat = new("FT_Glyph_Format_", null, null);
            enumMappingFTGlyphFormat.ItemMappings.Add(new("FT_GLYPH_FORMAT_NONE", null, null, "0"));
            enumMappingFTGlyphFormat.ItemMappings.Add(new("FT_GLYPH_FORMAT_COMPOSITE", null, null, "1668246896"));
            enumMappingFTGlyphFormat.ItemMappings.Add(new("FT_GLYPH_FORMAT_BITMAP", null, null, "1651078259"));
            enumMappingFTGlyphFormat.ItemMappings.Add(new("FT_GLYPH_FORMAT_OUTLINE", null, null, "186998492"));
            enumMappingFTGlyphFormat.ItemMappings.Add(new("FT_GLYPH_FORMAT_PLOTTER", null, null, "1886154612"));
            enumMappingFTGlyphFormat.ItemMappings.Add(new("FT_GLYPH_FORMAT_SVG", null, null, "1398163232"));

            settings.EnumMappings.Add(enumMappingFTGlyphFormat);

            EnumMapping enumMappingFTEncoding = new("FT_Encoding_", null, null);
            enumMappingFTEncoding.ItemMappings.Add(new("FT_ENCODING_NONE", null, null, "0"));
            enumMappingFTEncoding.ItemMappings.Add(new("FT_ENCODING_MS_SYMBOL", null, null, "1937337698"));
            enumMappingFTEncoding.ItemMappings.Add(new("FT_ENCODING_UNICODE", null, null, "1970170211"));
            enumMappingFTEncoding.ItemMappings.Add(new("FT_ENCODING_SJIS", null, null, "1936353651"));
            enumMappingFTEncoding.ItemMappings.Add(new("FT_ENCODING_PRC", null, null, "1734484000"));
            enumMappingFTEncoding.ItemMappings.Add(new("FT_ENCODING_BIG5", null, null, "1651074869"));
            enumMappingFTEncoding.ItemMappings.Add(new("FT_ENCODING_WANSUNG", null, null, "2002873971"));
            enumMappingFTEncoding.ItemMappings.Add(new("FT_ENCODING_JOHAB", null, null, "1785686113"));

            enumMappingFTEncoding.ItemMappings.Add(new("FT_ENCODING_ADOBE_STANDARD", null, null, "1094995778"));
            enumMappingFTEncoding.ItemMappings.Add(new("FT_ENCODING_ADOBE_EXPERT", null, null, "1094992453"));
            enumMappingFTEncoding.ItemMappings.Add(new("FT_ENCODING_ADOBE_CUSTOM", null, null, "1094992451"));
            enumMappingFTEncoding.ItemMappings.Add(new("FT_ENCODING_ADOBE_LATIN_1", null, null, "1818326065"));
            enumMappingFTEncoding.ItemMappings.Add(new("FT_ENCODING_OLD_LATIN_2", null, null, "1818326066"));
            enumMappingFTEncoding.ItemMappings.Add(new("FT_ENCODING_APPLE_ROMAN", null, null, "1634889070"));

            settings.EnumMappings.Add(enumMappingFTEncoding);

            string headerFile = "freetype/main.h";

            CsCodeGenerator generator = new(settings);

            generator.Generate(headerFile, "../../../../Hexa.NET.FreeType/Generated");
            EvaluateResult(generator);
            Assert.Pass();
        }

        [Test]
        public void Vulkan()
        {
            CsCodeGeneratorSettings settings = CsCodeGeneratorSettings.Load("vulkan/generator.json");

            settings.CustomEnumItemMapper = VulkanCustomEnumItemMapper;

            string headerFile = "vulkan/main.h";

            CsCodeGenerator generator = new(settings);

            generator.Generate(headerFile, "../../../../Hexa.NET.Vulkan/Generated");
            EvaluateResult(generator);
            Assert.Pass();
        }

        private Dictionary<string, string> valueMap = [];

        private HashSet<string> vulkanFlags = [
             "VkMemoryPropertyFlags",
 "VkMemoryHeapFlags",
 "VkInstanceCreateFlags",
 "VkSampleCountFlags",
 "VkFormatFeatureFlags",
 "VkAccessFlags",
 "VkImageAspectFlags",
 "VkDeviceCreateFlags",
 "VkDeviceQueueCreateFlags",
 "VkQueueFlags",
 "VkPipelineStageFlags",
 "VkSparseMemoryBindFlags",
 "VkSparseImageFormatFlags",
 "VkFenceCreateFlags",
 "VkSemaphoreCreateFlags",
 "VkEventCreateFlags",
 "VkQueryPoolCreateFlags",
 "VkQueryPipelineStatisticFlags",
 "VkBufferCreateFlags",
 "VkBufferUsageFlags",
 "VkBufferViewCreateFlags",
 "VkImageCreateFlags",
 "VkImageUsageFlags",
 "VkImageViewCreateFlags",
 "VkShaderModuleCreateFlags",
 "VkPipelineCacheCreateFlags",
 "VkPipelineShaderStageCreateFlags",
 "VkPipelineCreateFlags",
 "VkPipelineLayoutCreateFlags",
 "VkSamplerCreateFlags",
 "VkDescriptorPoolCreateFlags",
 "VkDescriptorSetLayoutCreateFlags",
 "VkAttachmentDescriptionFlags",
 "VkDependencyFlags",
 "VkSubpassDescriptionFlags",
 "VkRenderPassCreateFlags",
 "VkCommandPoolCreateFlags",
 "VkCommandPoolResetFlags",
 "VkCommandBufferUsageFlags",
 "VkQueryControlFlags",
 "VkQueryResultFlags",
 "VkCommandBufferResetFlags",
 "VkStencilFaceFlags",
 "VkCullModeFlags",
 "VkDescriptorBindingFlags",
 "VkResolveModeFlags",
 "VkFramebufferCreateFlags",
 "VkSemaphoreWaitFlags",
 "VkToolPurposeFlags",
 "VkPipelineVertexInputStateCreateFlags",
 "VkPipelineInputAssemblyStateCreateFlags",
 "VkPipelineTessellationStateCreateFlags",
 "VkPipelineViewportStateCreateFlags",
 "VkPipelineRasterizationStateCreateFlags",
 "VkPipelineMultisampleStateCreateFlags",
 "VkPipelineDepthStencilStateCreateFlags",
 "VkColorComponentFlags",
 "VkPipelineColorBlendStateCreateFlags",
 "VkPipelineDynamicStateCreateFlags",
 "VkShaderStageFlags",
 "VkSubgroupFeatureFlags",
 "VkMemoryAllocateFlags",
 "VkDescriptorUpdateTemplateCreateFlags",
 "VkExternalMemoryFeatureFlags",
 "VkExternalMemoryHandleTypeFlags",
 "VkExternalFenceHandleTypeFlags",
 "VkExternalFenceFeatureFlags",
 "VkExternalSemaphoreHandleTypeFlags",
 "VkExternalSemaphoreFeatureFlags",
 "VkPipelineCreationFeedbackFlags",
 "VkPrivateDataSlotCreateFlags",
 "VkPipelineStageFlags2",
 "VkAccessFlags2",
 "VkSubmitFlags",
 "VkRenderingFlags",
 "VkFormatFeatureFlags2",
 "VkSwapchainCreateFlagsKHR",
 "VkDeviceGroupPresentModeFlagsKHR",
 "VkDisplayModeCreateFlagsKHR",
 "VkDisplayPlaneAlphaFlagsKHR",
 "VkSurfaceTransformFlagsKHR",
 "VkDisplaySurfaceCreateFlagsKHR",
 "VkVideoCodecOperationFlagsKHR",
 "VkVideoChromaSubsamplingFlagsKHR",
 "VkVideoCapabilityFlagsKHR",
 "VkVideoSessionCreateFlagsKHR",
 "VkVideoSessionParametersCreateFlagsKHR",
 "VkVideoBeginCodingFlagsKHR",
 "VkVideoEndCodingFlagsKHR",
 "VkVideoCodingControlFlagsKHR",
 "VkVideoDecodeCapabilityFlagsKHR",
 "VkVideoDecodeUsageFlagsKHR",
 "VkVideoDecodeFlagsKHR",
 "VkSemaphoreImportFlags",
 "VkPerformanceCounterDescriptionFlagsKHR",
 "VkAcquireProfilingLockFlagsKHR",
 "VkOpticalFlowGridSizeFlagsNV",
 "VkOpticalFlowExecuteFlagsNV",
 "VkShaderCreateFlagsEXT",
 "VkShaderCorePropertiesFlagsAMD",
 "VkAccelerationStructureCreateFlagsKHR",
 "VkBuildAccelerationStructureFlagsKHR",
 "VkDebugReportFlagsEXT",
 "VkPipelineCoverageReductionStateCreateFlagsNV",
 "VkHeadlessSurfaceCreateFlagsEXT",
 "VkDescriptorPoolResetFlags",
 "VkPresentGravityFlagsEXT",
 "VkVideoComponentBitDepthFlagsKHR",
 "VkExternalMemoryFeatureFlagsNV",
 "VkIndirectStateFlagsNV",
 "VkDebugUtilsMessageTypeFlagsEXT",
 "VkDeviceDiagnosticsConfigFlagsNV",
 "VkDebugUtilsMessengerCallbackDataFlagsEXT",
 "VkDebugUtilsMessengerCreateFlagsEXT",
 "VkPipelineDiscardRectangleStateCreateFlagsEXT",
 "VkBuildAccelerationStructureFlagsNV",
 "VkPipelineRasterizationConservativeStateCreateFlagsEXT",
 "VkGraphicsPipelineLibraryFlagsEXT",
 "VkAccelerationStructureMotionInfoFlagsNV",
 "VkGeometryInstanceFlagsKHR",
 "VkMemoryDecompressionMethodFlagsNV",
 "VkDebugUtilsMessageSeverityFlagsEXT",
 "VkAccelerationStructureMotionInstanceFlagsNV",
 "VkImageCompressionFlagsEXT",
 "VkImageCompressionFixedRateFlagsEXT",
 "VkMicromapCreateFlagsEXT",
 "VkGeometryFlagsKHR",
 "VkDeviceAddressBindingFlagsEXT",
 "VkPipelineCompilerControlFlagsAMD",
 "VkPipelineCoverageToColorStateCreateFlagsNV",
 "VkPipelineCoverageModulationStateCreateFlagsNV",
 "VkDirectDriverLoadingFlagsLUNARG",
 "VkOpticalFlowSessionCreateFlagsNV",
 "VkMemoryMapFlags",
 "VkPeerMemoryFeatureFlags",
 "VkCommandPoolTrimFlags",
 "VkExternalMemoryHandleTypeFlagsNV",
 "VkCompositeAlphaFlagsKHR",
 "VkIndirectCommandsLayoutUsageFlagsNV",
 "VkDeviceMemoryReportFlagsEXT",
 "VkMemoryUnmapFlagsKHR",
 "VkPresentScalingFlagsEXT",
 "VkBuildMicromapFlagsEXT",
 "VkPipelineRasterizationStateStreamCreateFlagsEXT",
 "VkConditionalRenderingFlagsEXT",
 "VkSurfaceCounterFlagsEXT",
 "VkValidationCacheCreateFlagsEXT",
 "VkPipelineViewportSwizzleStateCreateFlagsNV",
 "VkAccelerationStructureTypeNV",
 "VkOpticalFlowUsageFlagsNV",
 "VkPipelineRasterizationDepthClipStateCreateFlagsEXT",
 "VkFenceImportFlags"
            ];

        private void VulkanCustomEnumItemMapper(CppEnum cppEnum, CppEnumItem cppEnumItem, CsEnumMetadata csEnum, CsEnumItemMetadata csEnumItem)
        {
            if (cppEnum.Name == "VkFormat")
            {
                if (valueMap.TryGetValue(csEnumItem.Value, out var newValue))
                {
                    csEnumItem.Value = newValue;
                }

                ReadOnlySpan<char> itemName = cppEnumItem.Name.AsSpan(10);

                string newName = itemName.ToString();
                valueMap[csEnumItem.Name] = newName;
                csEnumItem.Name = newName;
            }

            if (vulkanFlags.Contains(cppEnum.Name))
            {
                csEnum.BaseType = "uint";
            }
        }

        [Test]
        public void VMA()
        {
            CsCodeGeneratorSettings settings = CsCodeGeneratorSettings.Load("vma/generator.json");

            string headerFile = "vma/main.h";

            CsCodeGenerator generator = new(settings);

            generator.Generate(headerFile, "../../../../Hexa.NET.VMA/Generated");
            EvaluateResult(generator);
            Assert.Pass();
        }

        [Test]
        public void Daxa()
        {
            CsCodeGeneratorSettings settings = CsCodeGeneratorSettings.Load("daxa/generator.json");

            string headerFile = "daxa/daxa.h";

            CsCodeGenerator generator = new(settings);

            generator.Generate(headerFile, "../../../../Hexa.NET.Daxa/Generated");
            EvaluateResult(generator);
            Assert.Pass();
        }
    }
}