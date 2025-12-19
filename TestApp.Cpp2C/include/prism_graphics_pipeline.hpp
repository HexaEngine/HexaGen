#pragma once

#include "prism_common.hpp"

HEXA_PRISM_NAMESPACE_BEGIN

struct GraphicsPipelineDesc
{
	PrismObj<ShaderSource> vertexShader;
	const char* vertexEntryPoint;
	PrismObj<ShaderSource> hullShader;
	const char* hullEntryPoint;
	PrismObj<ShaderSource> domainShader;
	const char* domainEntryPoint;
	PrismObj<ShaderSource> geometryShader;
	const char* geometryEntryPoint;
	PrismObj<ShaderSource> pixelShader;
	const char* pixelEntryPoint;
};

class GraphicsPipeline : public Pipeline
{
protected:
	GraphicsPipelineDesc desc;
public:
	GraphicsPipeline(const GraphicsPipelineDesc& desc) : desc(desc) {}

	const GraphicsPipelineDesc& GetDesc() const { return desc; }
};

struct RenderTargetBlendDescription
{
	bool isBlendEnabled = false;
	bool isLogicOpEnabled = false;

	Blend sourceBlend = Blend::One;
	Blend destinationBlend = Blend::Zero;
	BlendOperation blendOp = BlendOperation::Add;
	Blend sourceBlendAlpha = Blend::One;
	Blend destinationBlendAlpha = Blend::Zero;
	BlendOperation blendOpAlpha = BlendOperation::Add;

	LogicOperation logicOp = LogicOperation::Clear;
	ColorWriteEnable renderTargetWriteMask = ColorWriteEnable::All;

	constexpr RenderTargetBlendDescription() = default;
};

struct BlendDescription
{
	static constexpr size_t SimultaneousRenderTargetCount = 8;

	bool alphaToCoverageEnable = false;
	bool independentBlendEnable = false;
	std::array<RenderTargetBlendDescription, SimultaneousRenderTargetCount> renderTargets;

private:
	static constexpr bool IsBlendEnabled(const RenderTargetBlendDescription& renderTarget)
	{
		return renderTarget.blendOpAlpha != BlendOperation::Add
			|| renderTarget.sourceBlendAlpha != Blend::One
			|| renderTarget.destinationBlendAlpha != Blend::Zero
			|| renderTarget.blendOp != BlendOperation::Add
			|| renderTarget.sourceBlend != Blend::One
			|| renderTarget.destinationBlend != Blend::Zero;
	}

public:
	constexpr BlendDescription() = default;

	constexpr BlendDescription(Blend sourceBlend, Blend destinationBlend)
		: BlendDescription(sourceBlend, destinationBlend, sourceBlend, destinationBlend)
	{
	}

	constexpr BlendDescription(Blend sourceBlend, Blend destinationBlend, Blend srcBlendAlpha, Blend destBlendAlpha)
		: BlendDescription()
	{
		alphaToCoverageEnable = false;
		independentBlendEnable = false;

		for (int i = 0; i < SimultaneousRenderTargetCount; i++)
		{
			renderTargets[i].sourceBlend = sourceBlend;
			renderTargets[i].destinationBlend = destinationBlend;
			renderTargets[i].blendOp = BlendOperation::Add;
			renderTargets[i].sourceBlendAlpha = srcBlendAlpha;
			renderTargets[i].destinationBlendAlpha = destBlendAlpha;
			renderTargets[i].blendOpAlpha = BlendOperation::Add;
			renderTargets[i].renderTargetWriteMask = ColorWriteEnable::All;
			renderTargets[i].isBlendEnabled = IsBlendEnabled(renderTargets[i]);
			renderTargets[i].isLogicOpEnabled = false;
		}
	}

	constexpr BlendDescription(Blend sourceBlend, Blend destinationBlend, Blend srcBlendAlpha, Blend destBlendAlpha, BlendOperation blendOperation, BlendOperation blendOperationAlpha)
		: BlendDescription()
	{
		alphaToCoverageEnable = false;
		independentBlendEnable = false;

		for (size_t i = 0; i < SimultaneousRenderTargetCount; i++)
		{
			renderTargets[i].sourceBlend = sourceBlend;
			renderTargets[i].destinationBlend = destinationBlend;
			renderTargets[i].blendOp = blendOperation;
			renderTargets[i].sourceBlendAlpha = srcBlendAlpha;
			renderTargets[i].destinationBlendAlpha = destBlendAlpha;
			renderTargets[i].blendOpAlpha = blendOperationAlpha;
			renderTargets[i].renderTargetWriteMask = ColorWriteEnable::All;
			renderTargets[i].isBlendEnabled = IsBlendEnabled(renderTargets[i]);
			renderTargets[i].isLogicOpEnabled = false;
		}
	}

	constexpr BlendDescription(Blend sourceBlend, Blend destinationBlend, Blend srcBlendAlpha, Blend destBlendAlpha, BlendOperation blendOperation, BlendOperation blendOperationAlpha, LogicOperation logicOperation)
		: BlendDescription()
	{
		alphaToCoverageEnable = false;
		independentBlendEnable = false;

		for (size_t i = 0; i < SimultaneousRenderTargetCount; i++)
		{
			renderTargets[i].sourceBlend = sourceBlend;
			renderTargets[i].destinationBlend = destinationBlend;
			renderTargets[i].blendOp = blendOperation;
			renderTargets[i].sourceBlendAlpha = srcBlendAlpha;
			renderTargets[i].destinationBlendAlpha = destBlendAlpha;
			renderTargets[i].blendOpAlpha = blendOperationAlpha;
			renderTargets[i].logicOp = logicOperation;
			renderTargets[i].isLogicOpEnabled = true;
			renderTargets[i].renderTargetWriteMask = ColorWriteEnable::All;
			renderTargets[i].isBlendEnabled = IsBlendEnabled(renderTargets[i]);
		}
	}
};

namespace BlendDescriptions
{
	static constexpr auto Opaque = BlendDescription(Blend::One, Blend::Zero);
	static constexpr auto AlphaBlend = BlendDescription(Blend::One, Blend::InverseSourceAlpha);
	static constexpr auto Additive = BlendDescription(Blend::SourceAlpha, Blend::One);
	static constexpr auto NonPremultiplied = BlendDescription(Blend::SourceAlpha, Blend::InverseSourceAlpha);
}


struct RasterizerDescription
{
	static constexpr int DefaultDepthBias = 0;
	static constexpr float DefaultDepthBiasClamp = 0.0f;
	static constexpr float DefaultSlopeScaledDepthBias = 0.0f;

	FillMode fillMode = FillMode::Solid;
	CullMode cullMode = CullMode::Back;
	bool frontCounterClockwise = false;
	int depthBias = DefaultDepthBias;
	float depthBiasClamp = DefaultDepthBiasClamp;
	float slopeScaledDepthBias = DefaultSlopeScaledDepthBias;
	bool depthClipEnable = true;
	bool scissorEnable = false;
	bool multisampleEnable = true;
	bool antialiasedLineEnable = false;
	uint32_t forcedSampleCount = 0;
	ConservativeRasterizationMode conservativeRaster = ConservativeRasterizationMode::Off;

	constexpr RasterizerDescription() = default;

	constexpr RasterizerDescription(CullMode cullMode, FillMode fillMode)
		: fillMode(fillMode), cullMode(cullMode)
	{
	}

	constexpr RasterizerDescription(
		CullMode cullMode,
		FillMode fillMode,
		bool frontCounterClockwise,
		int depthBias,
		float depthBiasClamp,
		float slopeScaledDepthBias,
		bool depthClipEnable,
		bool scissorEnable,
		bool multisampleEnable,
		bool antialiasedLineEnable)
		: fillMode(fillMode),
		cullMode(cullMode),
		frontCounterClockwise(frontCounterClockwise),
		depthBias(depthBias),
		depthBiasClamp(depthBiasClamp),
		slopeScaledDepthBias(slopeScaledDepthBias),
		depthClipEnable(depthClipEnable),
		scissorEnable(scissorEnable),
		multisampleEnable(multisampleEnable),
		antialiasedLineEnable(antialiasedLineEnable)
	{
	}

	constexpr RasterizerDescription(
		CullMode cullMode,
		FillMode fillMode,
		bool frontCounterClockwise,
		int depthBias,
		float depthBiasClamp,
		float slopeScaledDepthBias,
		bool depthClipEnable,
		bool scissorEnable,
		bool multisampleEnable,
		bool antialiasedLineEnable,
		uint32_t forcedSampleCount,
		ConservativeRasterizationMode conservativeRasterization)
		: fillMode(fillMode),
		cullMode(cullMode),
		frontCounterClockwise(frontCounterClockwise),
		depthBias(depthBias),
		depthBiasClamp(depthBiasClamp),
		slopeScaledDepthBias(slopeScaledDepthBias),
		depthClipEnable(depthClipEnable),
		scissorEnable(scissorEnable),
		multisampleEnable(multisampleEnable),
		antialiasedLineEnable(antialiasedLineEnable),
		forcedSampleCount(forcedSampleCount),
		conservativeRaster(conservativeRasterization)
	{
	}
};

namespace RasterizerDescriptions
{
	static constexpr auto CullNone = RasterizerDescription(CullMode::None, FillMode::Solid);
	static constexpr auto CullFront = RasterizerDescription(CullMode::Front, FillMode::Solid);
	static constexpr auto CullBack = RasterizerDescription(CullMode::Back, FillMode::Solid);
	static constexpr auto CullBackScissors = RasterizerDescription(CullMode::Back, FillMode::Solid, false, 0, 0.0f, 0.0f, true, true, true, false);
	static constexpr auto Wireframe = RasterizerDescription(CullMode::None, FillMode::Wireframe);
	static constexpr auto CullNoneDepthBias = RasterizerDescription(CullMode::None, FillMode::Solid, false, -1, 0.0f, 1.0f, true, false, false, false);
	static constexpr auto CullFrontDepthBias = RasterizerDescription(CullMode::Front, FillMode::Solid, false, -1, 0.0f, 1.0f, true, false, false, false);
	static constexpr auto CullBackDepthBias = RasterizerDescription(CullMode::Back, FillMode::Solid, false, -1, 0.0f, 1.0f, true, false, false, false);
}

struct DepthStencilOperationDescription
{
	StencilOperation stencilFailOp = StencilOperation::Keep;
	StencilOperation stencilDepthFailOp = StencilOperation::Keep;
	StencilOperation stencilPassOp = StencilOperation::Keep;
	ComparisonFunc stencilFunc = ComparisonFunc::Always;

	constexpr DepthStencilOperationDescription() = default;

	constexpr DepthStencilOperationDescription(
		StencilOperation stencilFailOp,
		StencilOperation stencilDepthFailOp,
		StencilOperation stencilPassOp,
		ComparisonFunc stencilFunc)
		: stencilFailOp(stencilFailOp),
		stencilDepthFailOp(stencilDepthFailOp),
		stencilPassOp(stencilPassOp),
		stencilFunc(stencilFunc)
	{
	}
};

namespace DepthStencilOperationDescriptions
{
	static constexpr auto Default = DepthStencilOperationDescription(
		StencilOperation::Keep,
		StencilOperation::Keep,
		StencilOperation::Keep,
		ComparisonFunc::Always);

	static constexpr auto DefaultFront = DepthStencilOperationDescription(
		StencilOperation::Keep,
		StencilOperation::Increment,
		StencilOperation::Keep,
		ComparisonFunc::Always);

	static constexpr auto DefaultBack = DepthStencilOperationDescription(
		StencilOperation::Keep,
		StencilOperation::Decrement,
		StencilOperation::Keep,
		ComparisonFunc::Always);
}

struct DepthStencilDescription
{
	static constexpr uint8_t DefaultStencilReadMask = 255;
	static constexpr uint8_t DefaultStencilWriteMask = 255;

	bool depthEnable = true;
	DepthWriteMask depthWriteMask = DepthWriteMask::All;
	ComparisonFunc depthFunc = ComparisonFunc::LessEqual;
	bool stencilEnable = false;
	uint8_t stencilReadMask = DefaultStencilReadMask;
	uint8_t stencilWriteMask = DefaultStencilWriteMask;
	DepthStencilOperationDescription frontFace;
	DepthStencilOperationDescription backFace;

	constexpr DepthStencilDescription() = default;

	constexpr DepthStencilDescription(bool depthEnable, DepthWriteMask depthWriteMask, ComparisonFunc depthFunc = ComparisonFunc::LessEqual)
		: depthEnable(depthEnable),
		depthWriteMask(depthWriteMask),
		depthFunc(depthFunc),
		stencilEnable(false),
		stencilReadMask(DefaultStencilReadMask),
		stencilWriteMask(DefaultStencilWriteMask),
		frontFace(DepthStencilOperationDescriptions::Default),
		backFace(DepthStencilOperationDescriptions::Default)
	{
	}

	constexpr DepthStencilDescription(bool depthEnable, bool stencilEnable, DepthWriteMask depthWriteMask, ComparisonFunc depthFunc = ComparisonFunc::LessEqual)
		: depthEnable(depthEnable),
		depthWriteMask(depthWriteMask),
		depthFunc(depthFunc),
		stencilEnable(stencilEnable),
		stencilReadMask(DefaultStencilReadMask),
		stencilWriteMask(DefaultStencilWriteMask),
		frontFace(DepthStencilOperationDescriptions::DefaultFront),
		backFace(DepthStencilOperationDescriptions::DefaultBack)
	{
	}

	constexpr DepthStencilDescription(
		bool depthEnable,
		bool depthWriteEnable,
		ComparisonFunc depthFunc,
		bool stencilEnable,
		uint8_t stencilReadMask,
		uint8_t stencilWriteMask,
		StencilOperation frontStencilFailOp,
		StencilOperation frontStencilDepthFailOp,
		StencilOperation frontStencilPassOp,
		ComparisonFunc frontStencilFunc,
		StencilOperation backStencilFailOp,
		StencilOperation backStencilDepthFailOp,
		StencilOperation backStencilPassOp,
		ComparisonFunc backStencilFunc)
		: depthEnable(depthEnable),
		depthWriteMask(depthWriteEnable ? DepthWriteMask::All : DepthWriteMask::Zero),
		depthFunc(depthFunc),
		stencilEnable(stencilEnable),
		stencilReadMask(stencilReadMask),
		stencilWriteMask(stencilWriteMask),
		frontFace(frontStencilFailOp, frontStencilDepthFailOp, frontStencilPassOp, frontStencilFunc),
		backFace(backStencilFailOp, backStencilDepthFailOp, backStencilPassOp, backStencilFunc)
	{
	}
};

namespace DepthStencilDescriptions
{
	static constexpr auto None = DepthStencilDescription(false, DepthWriteMask::Zero);
	static constexpr auto Always = DepthStencilDescription(true, DepthWriteMask::All, ComparisonFunc::Always);
	static constexpr auto Default = DepthStencilDescription(true, DepthWriteMask::All);
	static constexpr auto DefaultLess = DepthStencilDescription(true, DepthWriteMask::All, ComparisonFunc::Less);
	static constexpr auto DefaultStencil = DepthStencilDescription(true, true, DepthWriteMask::All);
	static constexpr auto DepthRead = DepthStencilDescription(true, DepthWriteMask::Zero);
	static constexpr auto DepthReadEquals = DepthStencilDescription(true, DepthWriteMask::Zero, ComparisonFunc::Equal);
	static constexpr auto DepthReverseZ = DepthStencilDescription(true, DepthWriteMask::All, ComparisonFunc::GreaterEqual);
	static constexpr auto DepthReadReverseZ = DepthStencilDescription(true, DepthWriteMask::Zero, ComparisonFunc::GreaterEqual);
}

struct InputElementDescription
{	
	std::string semanticName;
	uint32_t semanticIndex;
	Format format;
	uint32_t slot;
	uint32_t alignedByteOffset;
	InputClassification classification;
	uint32_t instanceDataStepRate;

	static constexpr uint32_t AppendAligned = static_cast<uint32_t>(-1);
};

struct GraphicsPipelineStateDesc
{
	RasterizerDescription rasterizer = RasterizerDescriptions::CullBack;
	DepthStencilDescription depthStencil = DepthStencilDescriptions::Default;
	BlendDescription blend = BlendDescriptions::Opaque;
	Color blendFactor;
	uint32_t sampleMask = 0xFFFFFFFF;
	uint32_t stencilRef = 0;
	const InputElementDescription* inputElements = nullptr;
	uint32_t numInputElements = 0;
	PrimitiveTopology primitiveTopology = PrimitiveTopology::TriangleList;
	PipelineStateFlags flags = PipelineStateFlags::None;

	constexpr GraphicsPipelineStateDesc() = default;

	constexpr GraphicsPipelineStateDesc(
		const RasterizerDescription& rasterizer,
		const DepthStencilDescription& depthStencil,
		const BlendDescription& blend,
		PrimitiveTopology primitiveTopology,
		const Color& blendFactor = {},
		uint32_t sampleMask = 0xFFFFFFFF,
		uint32_t stencilRef = 0,
		const InputElementDescription* inputElements = nullptr,
		size_t numInputElements = 0,
		PipelineStateFlags flags = PipelineStateFlags::None)
		: rasterizer(rasterizer),
		depthStencil(depthStencil),
		blend(blend),
		primitiveTopology(primitiveTopology),
		blendFactor(blendFactor),
		sampleMask(sampleMask),
		stencilRef(stencilRef),
		inputElements(inputElements),
		numInputElements(numInputElements),
		flags(flags)
	{
	}
};

namespace GraphicsPipelineStateDescs
{
	static constexpr auto Default = GraphicsPipelineStateDesc(
		RasterizerDescriptions::CullBack,
		DepthStencilDescriptions::Default,
		BlendDescriptions::Opaque,
		PrimitiveTopology::TriangleList
	);

	static constexpr auto DefaultAlphaBlend = GraphicsPipelineStateDesc(
		RasterizerDescriptions::CullBack,
		DepthStencilDescriptions::Default,
		BlendDescriptions::AlphaBlend,
		PrimitiveTopology::TriangleList
	);

	static constexpr auto DefaultFullscreen = GraphicsPipelineStateDesc(
		RasterizerDescriptions::CullBack,
		DepthStencilDescriptions::None,
		BlendDescriptions::Opaque,
		PrimitiveTopology::TriangleStrip
	);

	static constexpr auto DefaultFullscreenScissors = GraphicsPipelineStateDesc(
		RasterizerDescriptions::CullBackScissors,
		DepthStencilDescriptions::None,
		BlendDescriptions::Opaque,
		PrimitiveTopology::TriangleStrip
	);

	static constexpr auto DefaultAdditiveFullscreen = GraphicsPipelineStateDesc(
		RasterizerDescriptions::CullBack,
		DepthStencilDescriptions::None,
		BlendDescriptions::Additive,
		PrimitiveTopology::TriangleStrip
	);

	static constexpr auto DefaultAlphaBlendFullscreen = GraphicsPipelineStateDesc(
		RasterizerDescriptions::CullBack,
		DepthStencilDescriptions::None,
		BlendDescriptions::AlphaBlend,
		PrimitiveTopology::TriangleStrip
	);
}

class GraphicsPipelineState : public PipelineState
{
protected:
	PrismObj<GraphicsPipeline> pipeline;
	GraphicsPipelineStateDesc desc;

public:
	GraphicsPipelineState(const PrismObj<GraphicsPipeline>& pipeline, const GraphicsPipelineStateDesc& desc)
		: pipeline(pipeline), desc(desc)
	{
	}

	const GraphicsPipelineStateDesc& GetDesc() const { return desc; }
	const PrismObj<GraphicsPipeline>& GetPipeline() const { return pipeline; }
};

HEXA_PRISM_NAMESPACE_END