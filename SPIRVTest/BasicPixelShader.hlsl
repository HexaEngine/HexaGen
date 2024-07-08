// BasicPixelShader.hlsl

struct PS_INPUT
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR;
};

float4 PS(PS_INPUT input) : SV_TARGET
{
	return input.Color;
}