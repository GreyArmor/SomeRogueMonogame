
float3 CameraPosition;
Texture2D tileAtlas;
float4x4 xView;
float4x4 xProjection;
float4x4 xViewProjection;
float4x4 xWorldViewProjection;
float4x4 xWorldMatrix;

float4x4 xTextureCoordinatesDistanceScaling;

float3 SunLightDirection;
float4 SunLightColor;
float SunLightIntensity;

//instance transform matrix
struct VertexShaderInstanceInput
{
	float4 row1 : TEXCOORD1;
	float4 row2 : TEXCOORD2;
	float4 row3 : TEXCOORD3;
	float4 row4 : TEXCOORD4;
};

float4x4 CreateMatrixFromRows(
	float4 row1, float4 row2, float4 row3, float4 row4)
{
	return float4x4(
		row1.x, row1.y, row1.z, row1.w,
		row2.x, row2.y, row2.z, row2.w,
		row3.x, row3.y, row3.z, row3.w,
		row4.x, row4.y, row4.z, row4.w);
}

struct VSIn
{
	float4 inPos : POSITION;
	float4 inColor : COLOR0;
	float4 inBackgroundColor : COLOR1;
	float2 inTextureCoord : TEXCOORD;
	float3 normal : NORMAL;
};

struct VertexToPixel
{
	float4 Position     : POSITION;
	float4 Color        : COLOR0;
	float4 BackgroundColor        : COLOR1;
	float2 TextureCoordinate        : TEXCOORD;
	float3 Normal : NORMAL0;
	float3 WorldPos : TEXCOORD1;
};


struct PixelToFrame
{
	float4 Color        : COLOR0;
};


SamplerState textureSampler : register(s0) = sampler_state
{
	Filter = Point;
	MipFilter = Point;
	MagFilter = Point;
	MinFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};


VertexToPixel SimplestVertexShader(VSIn input)
{
	VertexToPixel Output = (VertexToPixel)0;

	Output.Position = mul(input.inPos, xWorldViewProjection);

	Output.Color = input.inColor;
	Output.BackgroundColor = input.inBackgroundColor;
	Output.TextureCoordinate = input.inTextureCoord;
	Output.Normal = input.normal;
	//no world matrix yet, so just pass the point
	Output.WorldPos = input.inPos.xyz;
	return Output;
}


PixelToFrame SimplePixelShader(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;
	Output.Color = float4(0,0,0,1);
	return Output;
}


float lengthSquared(float3 v1)
{
	return v1.x * v1.x + v1.y * v1.y + v1.z * v1.z;
}

PixelToFrame BackgroundPixelShader(VertexToPixel input)
{

	PixelToFrame Output = (PixelToFrame)0;
	Output.Color = input.BackgroundColor;
	return Output;
}


PixelToFrame ColorPixelShader(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;
	Output.Color = PSIn.Color;
	Output.Color.a = 1;
	return Output;
}

technique Wireframe
{
	pass Pass0
	{
		VertexShader = compile vs_4_0 SimplestVertexShader();
		PixelShader = compile ps_4_0 SimplePixelShader();
	}
}

technique Background
{
	pass Pass0
	{
		VertexShader = compile vs_4_0 SimplestVertexShader();
		PixelShader = compile ps_4_0 BackgroundPixelShader();
	}
}


technique ColorTech
{
	pass Pass0
	{
		VertexShader = compile vs_4_0 SimplestVertexShader();
		PixelShader = compile ps_4_0 ColorPixelShader();
	}
}


VertexToPixel SimplestVertexShaderInstanced(VSIn input, VertexShaderInstanceInput instanceInput)
{
	VertexToPixel output;

	float4x4 worldMatrixInstance = CreateMatrixFromRows(
		instanceInput.row1,
		instanceInput.row2,
		instanceInput.row3,
		instanceInput.row4);

	float4 worldPosition = mul(input.inPos, worldMatrixInstance);
	float4 viewPosition = mul(worldPosition, xView);

	output.Color = input.inColor;
	output.BackgroundColor = input.inBackgroundColor;
	output.TextureCoordinate = input.inTextureCoord;
	output.Normal = input.normal;

	float4 finalPosition = mul(viewPosition, xProjection);
	output.Position = finalPosition;
	output.WorldPos = finalPosition.xyz;

	return output;
}

technique ColorTechInstanced
{
	pass Pass0
	{
		VertexShader = compile vs_4_0 SimplestVertexShaderInstanced();
		PixelShader = compile ps_4_0 ColorPixelShader();
	}
}


