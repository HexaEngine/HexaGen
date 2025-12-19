#pragma once

#include "prism_common.hpp"

HEXA_PRISM_NAMESPACE_BEGIN

struct ComputePipelineDesc
{
	PrismObj<ShaderSource> computeShader;
	const char* computeEntryPoint;
};

class ComputePipeline : public Pipeline
{
protected:
	ComputePipelineDesc desc;
public:
	ComputePipeline(const ComputePipelineDesc& desc) : desc(desc) {}
	const ComputePipelineDesc& GetDesc() const { return desc; }
};

struct ComputePipelineStateDesc
{
	PipelineStateFlags flags = PipelineStateFlags::None;

	constexpr ComputePipelineStateDesc() = default;
	constexpr ComputePipelineStateDesc(PipelineStateFlags flags) : flags(flags)
	{
	}
};

class ComputePipelineState : public PipelineState
{
protected:
	PrismObj<ComputePipeline> pipeline;
	ComputePipelineStateDesc desc;

public:
	ComputePipelineState(const PrismObj<ComputePipeline>& pipeline, const ComputePipelineStateDesc& desc)
		: pipeline(pipeline), desc(desc)
	{
	}

	const ComputePipelineStateDesc& GetDesc() const { return desc; }
	const PrismObj<ComputePipeline>& GetPipeline() const { return pipeline; }
};

HEXA_PRISM_NAMESPACE_END