// BasicVertexShader.hlsl

struct VS_INPUT
{
	float4 Position : POSITION;
	float4 Color : COLOR;
};

struct PS_INPUT
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR;
};

PS_INPUT VS(VS_INPUT input)
{
	PS_INPUT output;
	output.Position = input.Position;
	output.Color = input.Color;
	return output;
}