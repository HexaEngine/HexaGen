#pragma once
#include "prism_base.hpp"

HEXA_PRISM_NAMESPACE_BEGIN
	class Buffer;
	class SamplerState;
	class ShaderResourceView;
	class UnorderedAccessView;
	class RenderTargetView;

	enum class Format : uint8_t
	{
		Unknown = 0,
		R32G32B32A32Typeless = 1,
		R32G32B32A32Float = 2,
		R32G32B32A32UInt = 3,
		R32G32B32A32SInt = 4,
		R32G32B32Typeless = 5,
		R32G32B32Float = 6,
		R32G32B32UInt = 7,
		R32G32B32SInt = 8,
		R16G16B16A16Typeless = 9,
		R16G16B16A16Float = 10,
		R16G16B16A16UNorm = 11,
		R16G16B16A16UInt = 12,
		R16G16B16A16SNorm = 13,
		R16G16B16A16Sint = 14,
		R32G32Typeless = 0xF,
		R32G32Float = 0x10,
		R32G32UInt = 17,
		R32G32SInt = 18,
		R32G8X24Typeless = 19,
		D32FloatS8X24UInt = 20,
		R32FloatX8X24Typeless = 21,
		X32TypelessG8X24UInt = 22,
		R10G10B10A2Typeless = 23,
		R10G10B10A2UNorm = 24,
		R10G10B10A2UInt = 25,
		R11G11B10Float = 26,
		R8G8B8A8Typeless = 27,
		R8G8B8A8UNorm = 28,
		R8G8B8A8UNormSRGB = 29,
		R8G8B8A8UInt = 30,
		R8G8B8A8SNorm = 0x1F,
		R8G8B8A8SInt = 0x20,
		R16G16Typeless = 33,
		R16G16Float = 34,
		R16G16UNorm = 35,
		R16G16UInt = 36,
		R16G16SNorm = 37,
		R16G16Sint = 38,
		R32Typeless = 39,
		D32Float = 40,
		R32Float = 41,
		R32UInt = 42,
		R32SInt = 43,
		R24G8Typeless = 44,
		D24UNormS8UInt = 45,
		R24UNormX8Typeless = 46,
		X24TypelessG8UInt = 47,
		R8G8Typeless = 48,
		R8G8UNorm = 49,
		R8G8UInt = 50,
		R8G8SNorm = 51,
		R8G8Sint = 52,
		R16Typeless = 53,
		D16UNorm = 55,
		R16UNorm = 56,
		R16UInt = 57,
		R16SNorm = 58,
		R16Sint = 59,
		R8Typeless = 60,
		R8UNorm = 61,
		R8UInt = 62,
		R8SNorm = 0x3F,
		R8SInt = 0x40,
		A8UNorm = 65,
		R1UNorm = 66,
		R9G9B9E5SharedExp = 67,
		R8G8B8G8UNorm = 68,
		G8R8G8B8UNorm = 69,
		BC1Typeless = 70,
		BC1UNorm = 71,
		BC1UNormSRGB = 72,
		BC2Typeless = 73,
		BC2UNorm = 74,
		BC2UNormSRGB = 75,
		BC3Typeless = 76,
		BC3UNorm = 77,
		BC3UNormSRGB = 78,
		BC4Typeless = 79,
		BC4UNorm = 80,
		BC4SNorm = 81,
		BC5Typeless = 82,
		BC5UNorm = 83,
		BC5SNorm = 84,
		B5G6R5UNorm = 85,
		B5G5R5A1UNorm = 86,
		B8G8R8A8UNorm = 87,
		B8G8R8X8UNorm = 88,
		R10G10B10XRBiasA2UNorm = 89,
		B8G8R8A8Typeless = 90,
		B8G8R8A8UNormSRGB = 91,
		B8G8R8X8Typeless = 92,
		B8G8R8X8UNormSRGB = 93,
		BC6HTypeless = 94,
		BC6HUF16 = 95,
		BC6HSF16 = 96,
		BC7Typeless = 97,
		BC7UNorm = 98,
		BC7UNormSRGB = 99,
		AYUV = 100,
		Y410 = 101,
		Y416 = 102,
		NV12 = 103,
		P010 = 104,
		P016 = 105,
		Opaque420 = 106,
		YUY2 = 107,
		Y210 = 108,
		Y216 = 109,
		NV11 = 110,
		AI44 = 111,
		IA44 = 112,
		P8 = 113,
		A8P8 = 114,
		B4G4R4A4UNorm = 115,
		P208 = 130,
		V208 = 131,
		V408 = 132,
		SamplerFeedbackMinMipOpaque = 189,
		SamplerFeedbackMipRegionUsedOpaque = 190,
	};

	enum class CpuAccessFlags : uint8_t
	{
		None = 0,
		Read = 1 << 0,
		Write = 1 << 1,
		All = Read | Write
	};

	enum class GpuAccessFlags : uint8_t
	{
		None = 0,
		Read = 1 << 0,
		Write = 1 << 1,
		UA = 1 << 2,
		DepthStencil = 1 << 3,
		Immutable = 1 << 4,
		RW = Read | Write,
		All = Read | Write | UA
	};

	enum class Filter : uint32_t
	{
		MinMagMipPoint = 0,
		MinMagPointMipLinear = 1,
		MinPointMagLinearMipPoint = 4,
		MinPointMagMipLinear = 5,
		MinLinearMagMipPoint = 16,
		MinLinearMagPointMipLinear = 17,
		MinMagLinearMipPoint = 20,
		MinMagMipLinear = 21,
		Anisotropic = 85,
		ComparisonMinMagMipPoint = 128,
		ComparisonMinMagPointMipLinear = 129,
		ComparisonMinPointMagLinearMipPoint = 132,
		ComparisonMinPointMagMipLinear = 133,
		ComparisonMinLinearMagMipPoint = 144,
		ComparisonMinLinearMagPointMipLinear = 145,
		ComparisonMinMagLinearMipPoint = 148,
		ComparisonMinMagMipLinear = 149,
		ComparisonAnisotropic = 213,
		MinimumMinMagMipPoint = 256,
		MinimumMinMagPointMipLinear = 257,
		MinimumMinPointMagLinearMipPoint = 260,
		MinimumMinPointMagMipLinear = 261,
		MinimumMinLinearMagMipPoint = 272,
		MinimumMinLinearMagPointMipLinear = 273,
		MinimumMinMagLinearMipPoint = 276,
		MinimumMinMagMipLinear = 277,
		MinimumAnisotropic = 341,
		MaximumMinMagMipPoint = 384,
		MaximumMinMagPointMipLinear = 385,
		MaximumMinPointMagLinearMipPoint = 388,
		MaximumMinPointMagMipLinear = 389,
		MaximumMinLinearMagMipPoint = 400,
		MaximumMinLinearMagPointMipLinear = 401,
		MaximumMinMagLinearMipPoint = 404,
		MaximumMinMagMipLinear = 405,
		MaximumAnisotropic = 469
	};

	enum class TextureAddressMode : uint8_t
	{
		Wrap = 1,
		Mirror = 2,
		Clamp = 3,
		Border = 4,
		MirrorOnce = 5
	};

	enum class ComparisonFunc : uint8_t
	{
		Never = 1,
		Less = 2,
		Equal = 3,
		LessEqual = 4,
		Greater = 5,
		NotEqual = 6,
		GreaterEqual = 7,
		Always = 8
	};

	enum class ResourceMiscFlags : uint8_t
	{
		None = 0,
		TextureCube = 1 << 0,
	};

	enum class ShaderStage : uint8_t
	{
		Vertex,
		Hull,
		Domain,
		Geometry,
		Pixel,
		Compute,
	};

	enum class ShaderParameterType : uint8_t
	{
		SRV,
		UAV,
		CBV,
		Sampler,
	};

	enum class Blend : uint8_t
	{
		Zero = 1,
		One = 2,
		SourceColor = 3,
		InverseSourceColor = 4,
		SourceAlpha = 5,
		InverseSourceAlpha = 6,
		DestinationAlpha = 7,
		InverseDestinationAlpha = 8,
		DestinationColor = 9,
		InverseDestinationColor = 10,
		SourceAlphaSaturate = 11,
		BlendFactor = 14,
		InverseBlendFactor = 15,
		Source1Color = 16,
		InverseSource1Color = 17,
		Source1Alpha = 18,
		InverseSource1Alpha = 19
	};

	enum class BlendOperation : uint8_t
	{
		Add = 1,
		Subtract = 2,
		ReverseSubtract = 3,
		Min = 4,
		Max = 5
	};

	enum class LogicOperation : uint8_t
	{
		Clear = 0,
		Set = 1,
		Copy = 2,
		CopyInverted = 3,
		Noop = 4,
		Invert = 5,
		And = 6,
		Nand = 7,
		Or = 8,
		Nor = 9,
		Xor = 10,
		Equiv = 11,
		AndReverse = 12,
		AndInverted = 13,
		OrReverse = 14,
		OrInverted = 15
	};

	enum class ColorWriteEnable : uint8_t
	{
		None = 0,
		Red = 1,
		Green = 2,
		Blue = 4,
		Alpha = 8,
		All = 15,
	};

	enum class FillMode : uint8_t
	{
		Wireframe = 2,
		Solid = 3
	};

	enum class CullMode : uint8_t
	{
		None = 1,
		Front = 2,
		Back = 3
	};

	enum class ConservativeRasterizationMode : uint8_t
	{
		Off = 0,
		On = 1
	};

	enum class DepthWriteMask : uint8_t
	{
		Zero = 0,
		All = 1
	};

	enum class StencilOperation : uint8_t
	{
		Keep = 1,
		Zero = 2,
		Replace = 3,
		IncrementSaturate = 4,
		DecrementSaturate = 5,
		Invert = 6,
		Increment = 7,
		Decrement = 8
	};

	enum class PrimitiveTopology : uint8_t
	{
		Undefined = 0,
		PointList = 1,
		LineList = 2,
		LineStrip = 3,
		TriangleList = 4,
		TriangleStrip = 5,
		LineListAdjacency = 10,
		LineStripAdjacency = 11,
		TriangleListAdjacency = 12,
		TriangleStripAdjacency = 13,
		PatchListWith1ControlPoints = 33,
		PatchListWith2ControlPoints = 34,
		PatchListWith3ControlPoints = 35,
		PatchListWith4ControlPoints = 36,
		PatchListWith5ControlPoints = 37,
		PatchListWith6ControlPoints = 38,
		PatchListWith7ControlPoints = 39,
		PatchListWith8ControlPoints = 40,
		PatchListWith9ControlPoints = 41,
		PatchListWith10ControlPoints = 42,
		PatchListWith11ControlPoints = 43,
		PatchListWith12ControlPoints = 44,
		PatchListWith13ControlPoints = 45,
		PatchListWith14ControlPoints = 46,
		PatchListWith15ControlPoints = 47,
		PatchListWith16ControlPoints = 48,
		PatchListWith17ControlPoints = 49,
		PatchListWith18ControlPoints = 50,
		PatchListWith19ControlPoints = 51,
		PatchListWith20ControlPoints = 52,
		PatchListWith21ControlPoints = 53,
		PatchListWith22ControlPoints = 54,
		PatchListWith23ControlPoints = 55,
		PatchListWith24ControlPoints = 56,
		PatchListWith25ControlPoints = 57,
		PatchListWith26ControlPoints = 58,
		PatchListWith27ControlPoints = 59,
		PatchListWith28ControlPoints = 60,
		PatchListWith29ControlPoints = 61,
		PatchListWith30ControlPoints = 62,
		PatchListWith31ControlPoints = 63,
		PatchListWith32ControlPoints = 64
	};

	enum class InputClassification : uint8_t
	{
		PerVertexData = 0,
		PerInstanceData = 1
	};

	enum class Usage : uint8_t
	{
		None = 0,
		BackBuffer = 1 << 0,
		DiscardOnPresent = 1 << 1,
		ReadOnly = 1 << 2,
		RenderTargetOutput = 1 << 3,
		ShaderInput = 1 << 4,
		Shared = 1 << 5,
		UnorderedAccess = 1 << 6
	};

	enum class SwapEffect : uint8_t
	{
		Discard = 0,
		Sequential = 1,
		FlipSequential = 3,
		FlipDiscard = 4
	};

	enum class SwapChainFlags : uint16_t
	{
		NonPreRotated = 1,
		AllowModeSwitch = 2,
		GDICompatible = 4,
		RestrictedContent = 8,
		RestrictedSharedResourceDriver = 16,
		DisplayOnly = 32,
		FrameLatencyWaitableObject = 64,
		ForegroundLayer = 128,
		FullscreenVideo = 256,
		YuvVideo = 512,
		HwProtected = 1024,
		AllowTearing = 2048,
		RestrictedToAllHolographicDisplays = 4096
	};

	enum class Scaling : uint8_t
	{
		Stretch = 0,
		None = 1,
		AspectRatioStretch = 2
	};

	enum class AlphaMode : uint8_t
	{
		Unspecified = 0,
		Premultiplied = 1,
		Straight = 2,
		Ignore = 3,
	};

	enum class ScanlineOrder : uint8_t
	{
		Unspecified = 0,
		Progressive = 1,
		UpperFieldFirst = 2,
		LowerFieldFirst = 3
	};

	enum class PresentFlags : uint8_t
	{
		None,
		DoNotWait = 1 << 0,
		AllowTearing = 1 << 1,
	};

	enum class DepthStencilViewClearFlags : uint8_t
	{
		None = 0,
		Depth = 1 << 0,
		Stencil = 1 << 1,
		All = Depth | Stencil,
	};

	enum class CommandListType : uint8_t
	{
		Immediate = 0,
		Deferred = 1,
	};

	enum class PipelineStateFlags : uint8_t
	{
		None = 0,
		ReflectVariables = 1 << 0,
	};


	struct Viewport
	{
		float x = 0, y = 0;
		float width = 0, height = 0;
		float minDepth = 0, maxDepth = 1.0f;

		constexpr Viewport() = default;

		constexpr Viewport(float x, float y, float width, float height, float minDepth = 0.0f, float maxDepth = 1.0f)
			: x(x), y(y), width(width), height(height), minDepth(minDepth), maxDepth(maxDepth)
		{
		}

		constexpr Viewport(float width, float height)
			: x(0.0f), y(0.0f), width(width), height(height), minDepth(0.0f), maxDepth(1.0f)
		{
		}

		constexpr Viewport(int width, int height)
			: x(0.0f), y(0.0f), width(static_cast<float>(width)), height(static_cast<float>(height)), minDepth(0.0f),
			  maxDepth(1.0f)
		{
		}
	};

	struct Color
	{
		float r, g, b, a;
	};

	struct Rect
	{
		int32_t left, top, right, bottom;
	};

	class ShaderSource : public PrismObject
	{
	public:
		virtual const char* GetIdentifier() = 0;
		virtual void GetData(uint8_t*& data, size_t& dataLength) = 0;
	};

	class TextShaderSource : public ShaderSource
	{
		std::string identifier;
		std::string text;

	public:
		TextShaderSource(const std::string& identifier, const std::string& text) : identifier(identifier), text(text)
		{
		}

		const char* GetIdentifier() override
		{
			return identifier.c_str();
		}

		void GetData(uint8_t*& data, size_t& dataLength) override
		{
			data = reinterpret_cast<uint8_t*>(text.data());
			dataLength = text.size();
		}
	};

	class Blob : public PrismObject
	{
		uint8_t* data;
		size_t length;
		bool owns;

	public:
		Blob() : data(nullptr), length(0), owns(false)
		{
		}

		Blob(uint8_t* bytecode, size_t length, bool owns, bool copy = false) : data(bytecode), length(length),
		                                                                       owns(owns)
		{
			if (copy && length > 0)
			{
				data = static_cast<uint8_t*>(PrismAlloc(length));
				PrismMemoryCopy(data, bytecode, length);
				this->owns = true;
			}
		}

		~Blob() override
		{
			if (owns)
			{
				PrismFree(data);
			}
		}


		uint8_t* GetData() const { return data; }
		size_t GetLength() const { return length; }
	};

	class Pipeline : public PrismObject
	{
	};

	struct BindingValuePair
	{
		const char* name;
		ShaderStage stage;
		ShaderParameterType type;
		void* value;
	};

	class ResourceBindingList
	{
	public:
		using iterator = BindingValuePair*;
		using iterator_pair = std::pair<iterator, iterator>;

		virtual ~ResourceBindingList() = default;

		virtual Pipeline* GetPipeline() const = 0;

		virtual void SetCBV(const char* name, Buffer* buffer) = 0;
		virtual void SetSampler(const char* name, SamplerState* sampler) = 0;
		virtual void SetSRV(const char* name, ShaderResourceView* view) = 0;
		virtual void SetUAV(const char* name, UnorderedAccessView* view,
		                    uint32_t initialCount = static_cast<uint32_t>(-1)) = 0;

		virtual void SetCBV(const char* name, ShaderStage stage, Buffer* buffer) = 0;
		virtual void SetSampler(const char* name, ShaderStage stage, SamplerState* sampler) = 0;
		virtual void SetSRV(const char* name, ShaderStage stage, ShaderResourceView* view) = 0;
		virtual void SetUAV(const char* name, ShaderStage stage, UnorderedAccessView* view,
		                    uint32_t initialCount = static_cast<uint32_t>(-1)) = 0;

		virtual iterator_pair GetSRVs() = 0;
		virtual iterator_pair GetCBVs() = 0;
		virtual iterator_pair GetUAVs() = 0;
		virtual iterator_pair GetSamplers() = 0;
	};

	class PipelineState : public PrismObject
	{
	public:
		virtual ResourceBindingList& GetBindings() = 0;
	};

HEXA_PRISM_NAMESPACE_END
