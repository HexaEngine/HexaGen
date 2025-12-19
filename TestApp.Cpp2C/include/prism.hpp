#pragma once
#include "common.hpp"
#include "prism_base.hpp"
#include "prism_common.hpp"
#include "prism_graphics_pipeline.hpp"
#include "prism_compute_pipeline.hpp"

HEXA_PRISM_NAMESPACE_BEGIN

	enum class BackendType
	{
		D3D11,
		D3D12,
		Vulkan,
	};

	class Resource : public DeviceChild
	{
	};

	struct SubresourceData
	{
		const void* data;
		uint32_t rowPitch;
		uint32_t slicePitch;
	};

	struct MappedSubresource
	{
		void* data;
		uint32_t rowPitch;
		uint32_t depthPitch;
	};

	enum class MapType : uint8_t
	{
		Read = 1,
		Write = 2,
		ReadWrite = 3,
		WriteDiscard = 4,
		WriteNoOverwrite = 5,
	};

	enum class MapFlags : uint32_t
	{
		None = 0,
		DoNotWait = 0x100000,
	};

	enum class BufferType
	{
		Default,
		ConstantBuffer,
		VertexBuffer,
		IndexBuffer,
	};

	struct BufferDesc
	{
		BufferType type;
		uint32_t widthInBytes;
		uint32_t structureStride;
		CpuAccessFlags cpuAccessFlags;
		GpuAccessFlags gpuAccessFlags;
	};

	class Buffer : public Resource
	{
	protected:
		BufferDesc desc;

		Buffer(const BufferDesc& desc) : desc(desc)
		{
		}

	public:
		const BufferDesc& GetDesc() const { return desc; }
	};

	struct SampleDesc
	{
		uint32_t count;
		uint32_t quality;
	};

	struct Texture1DDesc
	{
		GpuAccessFlags gpuAccessFlags;
		CpuAccessFlags cpuAccessFlags;
		Format format;
		uint32_t width;
		uint32_t arraySize;
		uint32_t mipLevels;
		ResourceMiscFlags miscFlags;
	};

	class Texture1D : public Resource
	{
	protected:
		Texture1DDesc desc;

		Texture1D(const Texture1DDesc& desc) : desc(desc)
		{
		}

	public:
		const Texture1DDesc& GetDesc() const { return desc; }
	};

	struct Texture2DDesc
	{
		GpuAccessFlags gpuAccessFlags;
		CpuAccessFlags cpuAccessFlags;
		Format format;
		uint32_t width;
		uint32_t height;
		uint32_t arraySize;
		uint32_t mipLevels;
		SampleDesc sampleDesc;
		ResourceMiscFlags miscFlags;
	};

	class Texture2D : public Resource
	{
	protected:
		Texture2DDesc desc;

		Texture2D(const Texture2DDesc& desc) : desc(desc)
		{
		}

	public:
		const Texture2DDesc& GetDesc() const { return desc; }
	};

	struct Texture3DDesc
	{
		GpuAccessFlags gpuAccessFlags;
		CpuAccessFlags cpuAccessFlags;
		Format format;
		uint32_t width;
		uint32_t height;
		uint32_t depth;
		uint32_t mipLevels;
		ResourceMiscFlags miscFlags;
	};

	class Texture3D : public Resource
	{
	protected:
		Texture3DDesc desc;

		Texture3D(const Texture3DDesc& desc) : desc(desc)
		{
		}

	public:
		const Texture3DDesc& GetDesc() const { return desc; }
	};

	class ResourceView : public DeviceChild
	{
	};

	// RenderTargetView structures
	enum class RenderTargetViewDimension
	{
		Buffer,
		Texture1D,
		Texture1DArray,
		Texture2D,
		Texture2DArray,
		Texture2DMS,
		Texture2DMSArray,
		Texture3D,
	};

	struct BufferRTV
	{
		union
		{
			uint32_t firstElement;
			uint32_t elementOffset;
		};

		union
		{
			uint32_t numElements;
			uint32_t elementWidth;
		};
	};

	struct Tex1DRTV
	{
		uint32_t mipSlice;
	};

	struct Tex1DArrayRTV
	{
		uint32_t mipSlice;
		uint32_t firstArraySlice;
		uint32_t arraySize;
	};

	struct Tex2DRTV
	{
		uint32_t mipSlice;
	};

	struct Tex2DArrayRTV
	{
		uint32_t mipSlice;
		uint32_t firstArraySlice;
		uint32_t arraySize;
	};

	struct Tex2DMSRTV
	{
		uint32_t unusedField_NothingToDefine;
	};

	struct Tex2DMSArrayRTV
	{
		uint32_t firstArraySlice;
		uint32_t arraySize;
	};

	struct Tex3DRTV
	{
		uint32_t mipSlice;
		uint32_t firstWSlice;
		uint32_t wSize;
	};

	struct RenderTargetViewDesc
	{
		RenderTargetViewDimension dimension;
		Format format;

		union
		{
			BufferRTV buffer;
			Tex1DRTV texture1D;
			Tex1DArrayRTV texture1DArray;
			Tex2DRTV texture2D;
			Tex2DArrayRTV texture2DArray;
			Tex2DMSRTV texture2DMS;
			Tex2DMSArrayRTV texture2DMSArray;
			Tex3DRTV texture3D;
		};
	};

	class RenderTargetView : public ResourceView
	{
	protected:
		RenderTargetViewDesc desc;

		explicit RenderTargetView(const RenderTargetViewDesc& desc) : desc(desc)
		{
		}

	public:
		const RenderTargetViewDesc& GetDesc() const { return desc; }
	};

	// ShaderResourceView structures
	enum class ShaderResourceViewDimension
	{
		Unknown,
		Buffer,
		Texture1D,
		Texture1DArray,
		Texture2D,
		Texture2DArray,
		Texture2DMS,
		Texture2DMSArray,
		Texture3D,
		TextureCube,
		TextureCubeArray,
		BufferEx,
	};

	struct BufferSRV
	{
		union
		{
			uint32_t firstElement;
			uint32_t elementOffset;
		};

		union
		{
			uint32_t numElements;
			uint32_t elementWidth;
		};
	};

	struct Tex1DSRV
	{
		uint32_t mostDetailedMip;
		uint32_t mipLevels;
	};

	struct Tex1DArraySRV
	{
		uint32_t mostDetailedMip;
		uint32_t mipLevels;
		uint32_t firstArraySlice;
		uint32_t arraySize;
	};

	struct Tex2DSRV
	{
		uint32_t mostDetailedMip;
		uint32_t mipLevels;
	};

	struct Tex2DArraySRV
	{
		uint32_t mostDetailedMip;
		uint32_t mipLevels;
		uint32_t firstArraySlice;
		uint32_t arraySize;
	};

	struct Tex2DMSSRV
	{
		uint32_t unusedField_NothingToDefine;
	};

	struct Tex2DMSArraySRV
	{
		uint32_t firstArraySlice;
		uint32_t arraySize;
	};

	struct Tex3DSRV
	{
		uint32_t mostDetailedMip;
		uint32_t mipLevels;
	};

	struct TexCubeSRV
	{
		uint32_t mostDetailedMip;
		uint32_t mipLevels;
	};

	struct TexCubeArraySRV
	{
		uint32_t mostDetailedMip;
		uint32_t mipLevels;
		uint32_t first2DArrayFace;
		uint32_t numCubes;
	};

	struct BufferExSRV
	{
		uint32_t firstElement;
		uint32_t numElements;
		uint32_t flags;
	};

	struct ShaderResourceViewDesc
	{
		ShaderResourceViewDimension dimension;
		Format format;

		union
		{
			BufferSRV buffer;
			Tex1DSRV texture1D;
			Tex1DArraySRV texture1DArray;
			Tex2DSRV texture2D;
			Tex2DArraySRV texture2DArray;
			Tex2DMSSRV texture2DMS;
			Tex2DMSArraySRV texture2DMSArray;
			Tex3DSRV texture3D;
			TexCubeSRV textureCube;
			TexCubeArraySRV textureCubeArray;
			BufferExSRV bufferEx;
		};
	};

	class ShaderResourceView : public ResourceView
	{
	protected:
		ShaderResourceViewDesc desc;

		explicit ShaderResourceView(const ShaderResourceViewDesc& desc) : desc(desc)
		{
		}

	public:
		const ShaderResourceViewDesc& GetDesc() const { return desc; }
	};

	// DepthStencilView structures
	enum class DepthStencilViewDimension
	{
		Unknown,
		Texture1D,
		Texture1DArray,
		Texture2D,
		Texture2DArray,
		Texture2DMS,
		Texture2DMSArray,
	};

	enum class DepthStencilViewFlags
	{
		None = 0,
		ReadOnlyDepth = 1 << 0,
		ReadOnlyStencil = 1 << 1,
	};

	struct Tex1DDSV
	{
		uint32_t mipSlice;
	};

	struct Tex1DArrayDSV
	{
		uint32_t mipSlice;
		uint32_t firstArraySlice;
		uint32_t arraySize;
	};

	struct Tex2DDSV
	{
		uint32_t mipSlice;
	};

	struct Tex2DArrayDSV
	{
		uint32_t mipSlice;
		uint32_t firstArraySlice;
		uint32_t arraySize;
	};

	struct Tex2DMSDSV
	{
		uint32_t unusedField_NothingToDefine;
	};

	struct Tex2DMSArrayDSV
	{
		uint32_t firstArraySlice;
		uint32_t arraySize;
	};

	struct DepthStencilViewDesc
	{
		DepthStencilViewDimension dimension;
		Format format;
		DepthStencilViewFlags flags;

		union
		{
			Tex1DDSV texture1D;
			Tex1DArrayDSV texture1DArray;
			Tex2DDSV texture2D;
			Tex2DArrayDSV texture2DArray;
			Tex2DMSDSV texture2DMS;
			Tex2DMSArrayDSV texture2DMSArray;
		};
	};

	class DepthStencilView : public ResourceView
	{
	protected:
		DepthStencilViewDesc desc;

		explicit DepthStencilView(const DepthStencilViewDesc& desc) : desc(desc)
		{
		}

	public:
		const DepthStencilViewDesc& GetDesc() const { return desc; }
	};

	// UnorderedAccessView structures
	enum class UnorderedAccessViewDimension
	{
		Unknown,
		Buffer,
		Texture1D,
		Texture1DArray,
		Texture2D,
		Texture2DArray,
		Texture3D,
	};

	struct BufferUAV
	{
		uint32_t firstElement;
		uint32_t numElements;
		uint32_t flags;
	};

	struct Tex1DUAV
	{
		uint32_t mipSlice;
	};

	struct Tex1DArrayUAV
	{
		uint32_t mipSlice;
		uint32_t firstArraySlice;
		uint32_t arraySize;
	};

	struct Tex2DUAV
	{
		uint32_t mipSlice;
	};

	struct Tex2DArrayUAV
	{
		uint32_t mipSlice;
		uint32_t firstArraySlice;
		uint32_t arraySize;
	};

	struct Tex3DUAV
	{
		uint32_t mipSlice;
		uint32_t firstWSlice;
		uint32_t wSize;
	};

	struct UnorderedAccessViewDesc
	{
		UnorderedAccessViewDimension dimension;
		Format format;

		union
		{
			BufferUAV buffer;
			Tex1DUAV texture1D;
			Tex1DArrayUAV texture1DArray;
			Tex2DUAV texture2D;
			Tex2DArrayUAV texture2DArray;
			Tex3DUAV texture3D;
		};
	};

	class UnorderedAccessView : public ResourceView
	{
	protected:
		UnorderedAccessViewDesc desc;

		explicit UnorderedAccessView(const UnorderedAccessViewDesc& desc) : desc(desc)
		{
		}

	public:
		const UnorderedAccessViewDesc& GetDesc() const { return desc; }
	};

	struct SamplerDesc
	{
		Filter filter;
		TextureAddressMode addressU;
		TextureAddressMode addressV;
		TextureAddressMode addressW;
		float mipLODBias;
		uint32_t maxAnisotropy;
		ComparisonFunc comparisonFunc;
		Color borderColor;
		float minLOD;
		float maxLOD;
	};

	class SamplerState : public DeviceChild
	{
	protected:
		SamplerDesc desc;

		explicit SamplerState(const SamplerDesc& desc) : desc(desc)
		{
		}

	public:
		const SamplerDesc& GetDesc() const { return desc; }
	};

	struct SwapChainDesc
	{
		uint32_t width;
		uint32_t height;
		Format format;
		bool stereo;
		SampleDesc sampleDesc;
		Usage bufferUsage;
		uint32_t bufferCount;
		Scaling scaling;
		SwapEffect swapEffect;
		AlphaMode alphaMode;
		SwapChainFlags flags;
	};

	struct Rational
	{
		uint32_t numerator;
		uint32_t denominator;
	};

	struct SwapChainFullscreenDesc 
	{
		Rational refreshRate;
		ScanlineOrder scanlineOrdering;
		Scaling scaling;
		bool windowed;
	};

	class SwapChain : public PrismObject
	{
	protected:
		SwapChainDesc desc;
		SwapChainFullscreenDesc fullscreenDesc;

	public:
		SwapChain(const SwapChainDesc& desc, const SwapChainFullscreenDesc& fullscreenDesc) 
			: desc(desc), fullscreenDesc(fullscreenDesc)
		{
		}

		const SwapChainDesc& GetDesc() const { return desc; }
		const SwapChainFullscreenDesc& GetFullscreenDesc() const { return fullscreenDesc; }

		virtual void ResizeBuffers(uint32_t bufferCount, uint32_t width, uint32_t height, Format newFormat, SwapChainFlags swapChainFlags) = 0;

		virtual PrismObj<Texture2D> GetBuffer(size_t index) = 0;
		virtual void Present(uint32_t interval, PresentFlags flags) = 0;
	};

	enum class QueryType
	{
		Event,
		Occlusion,
		Timestamp,
		TimestampDisjoint,
		PipelineStatistics,
		OcclusionPredicate,
		SOStatistics,
		SOOverflowPredicate,
		SOStatisticsStream0,
		SOOverflowPredicateStream0,
		SOStatisticsStream1,
		SOOverflowPredicateStream1,
		SOStatisticsStream2,
		SOOverflowPredicateStream2,
		SOStatisticsStream3,
		SOOverflowPredicateStream3,
	};

	enum class ContextType
	{
		All = 0,
		Graphics = 1,
		Compute = 2,
		Copy = 3,
		Video = 4,
	};

	enum class QueryMiscFlags
	{
		None,
		PredicateHint = 1,
	};

	struct QueryDesc
	{
		QueryType type;
		ContextType contextType = ContextType::All;
		QueryMiscFlags miscFlags = QueryMiscFlags::None;
	};

	class Query : public DeviceChild
	{
		QueryDesc desc;
	public:
		Query(const QueryDesc& desc) : desc(desc)
		{
		}

		const QueryDesc& GetDesc() const { return desc; }
	};

	enum class QueryGetDataFlags
	{
		None,
		DoNotFlush = 1,
	};

	class CommandList : public DeviceChild
	{
	public:
		virtual CommandListType GetType() const noexcept = 0;
		virtual void Begin() = 0;
		virtual void End() = 0;
		virtual void SetGraphicsPipelineState(GraphicsPipelineState* state) = 0;
		virtual void SetComputePipelineState(ComputePipelineState* state) = 0;
		virtual void SetVertexBuffer(uint32_t slot, Buffer* buffer, uint32_t stride, uint32_t offset) = 0;
		virtual void SetIndexBuffer(Buffer* buffer, Format format, uint32_t offset) = 0;
		virtual void SetRenderTarget(RenderTargetView* rtv, DepthStencilView* dsv) = 0;
		virtual void SetRenderTargetsAndUnorderedAccessViews(uint32_t count, RenderTargetView** views, DepthStencilView* depthStencilView, uint32_t uavSlot, uint32_t uavCount, UnorderedAccessView** uavs, uint32_t* pUavInitialCount) = 0;
		virtual void SetViewport(const Viewport& viewport) = 0;
		virtual void SetViewports(uint32_t viewportCount, const Viewport* viewports) = 0;
		virtual void SetPrimitiveTopology(PrimitiveTopology topology) = 0;
		virtual void SetScissorRects(const Rect* rects, uint32_t rectCount) = 0;
		virtual void DrawInstanced(uint32_t vertexCount, uint32_t instanceCount, uint32_t vertexOffset, uint32_t instanceOffset) = 0;
		virtual void DrawIndexedInstanced(uint32_t indexCount, uint32_t instanceCount, uint32_t indexOffset, int32_t vertexOffset, uint32_t instanceOffset) = 0;
		virtual void DrawIndexedInstancedIndirect(Buffer* bufferForArgs, uint32_t alignedByteOffsetForArgs) = 0;
		virtual void DrawInstancedIndirect(Buffer* bufferForArgs, uint32_t alignedByteOffsetForArgs) = 0;
		virtual void Dispatch(uint32_t threadGroupCountX, uint32_t threadGroupCountY, uint32_t threadGroupCountZ) = 0;
		virtual void DispatchIndirect(Buffer* dispatchArgs, uint32_t offset) = 0;
		virtual void ExecuteCommandList(CommandList* commandList) = 0;
		virtual void ClearRenderTargetView(RenderTargetView* rtv, const Color& color) = 0;
		virtual void ClearDepthStencilView(DepthStencilView* dsv, DepthStencilViewClearFlags flags, float depth, char stencil) = 0;
		virtual void ClearUnorderedAccessViewUint(UnorderedAccessView* uav, uint32_t r, uint32_t g, uint32_t b, uint32_t a) = 0;
		virtual void ClearView(ResourceView* view, const Color& color, const Rect& rect) = 0;
		virtual void CopyResource(Resource* dstResource, Resource* srcResource) = 0;
		virtual void GenerateMips(ShaderResourceView* srv) = 0;
		virtual void ClearState() = 0;
		virtual void Flush() = 0;
		virtual MappedSubresource Map(Resource* resource, uint32_t subresource, MapType mapType, MapFlags mapFlags) = 0;
		virtual void Unmap(Resource* resource, uint32_t subresource) = 0;
		virtual void BeginQuery(Query* query) = 0;
		virtual void EndQuery(Query* query) = 0;
		virtual bool QueryGetData(Query* query, void* data, uint32_t size, QueryGetDataFlags flags = QueryGetDataFlags::None) = 0;

		virtual void BeginEvent(const char* name) = 0;
		virtual void EndEvent() = 0;

		template<typename T>
		void Write(Resource* resource, const T& data, uint32_t offset = 0)
		{
			MappedSubresource mapped = Map(resource, 0, MapType::WriteDiscard, MapFlags::None);
			std::memcpy(static_cast<uint8_t*>(mapped.data) + offset, &data, sizeof(T));
			Unmap(resource, 0);
		}

		template<typename T>
		void WriteArray(Resource* resource, const T* data, const uint32_t count = 0, uint32_t offset = 0)
		{
			MappedSubresource mapped = Map(resource, 0, MapType::WriteDiscard, MapFlags::None);
			std::memcpy(static_cast<uint8_t*>(mapped.data) + offset, data, sizeof(T) * count);
			Unmap(resource, 0);
		}
	};

	class GraphicsDevice : public PrismObject
	{
	public:
		static PrismObj<GraphicsDevice> Create();
		virtual CommandList* GetImmediateCommandList() = 0;
		virtual PrismObj<Buffer> CreateBuffer(const BufferDesc& desc, const SubresourceData* initialData = nullptr) = 0;
		virtual PrismObj<Texture1D> CreateTexture1D(const Texture1DDesc& desc) = 0;
		virtual PrismObj<Texture2D> CreateTexture2D(const Texture2DDesc& desc) = 0;
		virtual PrismObj<Texture3D> CreateTexture3D(const Texture3DDesc& desc) = 0;
		virtual PrismObj<RenderTargetView> CreateRenderTargetView(Resource* resource, const RenderTargetViewDesc& desc) = 0;
		virtual PrismObj<ShaderResourceView> CreateShaderResourceView(Resource* resource, const ShaderResourceViewDesc& desc) = 0;
		virtual PrismObj<DepthStencilView> CreateDepthStencilView(Resource* resource, const DepthStencilViewDesc& desc) = 0;
		virtual PrismObj<UnorderedAccessView> CreateUnorderedAccessView(Resource* resource, const UnorderedAccessViewDesc& desc) = 0;
		virtual PrismObj<SamplerState> CreateSamplerState(const SamplerDesc& desc) = 0;
		virtual PrismObj<CommandList> CreateCommandList() = 0;
		virtual PrismObj<GraphicsPipeline> CreateGraphicsPipeline(const GraphicsPipelineDesc& desc) = 0;
		virtual PrismObj<GraphicsPipelineState> CreateGraphicsPipelineState(GraphicsPipeline* pipeline, const GraphicsPipelineStateDesc& desc) = 0;
		virtual PrismObj<ComputePipeline> CreateComputePipeline(const ComputePipelineDesc& desc) = 0;
		virtual PrismObj<ComputePipelineState> CreateComputePipelineState(ComputePipeline* pipeline, const ComputePipelineStateDesc& desc) = 0;
		virtual PrismObj<SwapChain> CreateSwapChain(void* windowHandle, const SwapChainDesc& desc, const SwapChainFullscreenDesc& fullscreenDesc) = 0;
		virtual PrismObj<SwapChain> CreateSwapChain(void* windowHandle) = 0;
		virtual PrismObj<Query> CreateQuery(const QueryDesc& desc) = 0;
	};

HEXA_PRISM_NAMESPACE_END
