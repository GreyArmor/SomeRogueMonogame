
float3 CameraPosition;
Texture tileAtlas;
float4x4 xView;
float4x4 xProjection;
float4x4 xViewProjection;
float4x4 xWorldViewProjection;
float4x4 xWorldMatrix;

//shadow mapping
float4x4 xLightsWorldViewProjection;
float3 xLightPos;
float xLightPower;
float xAmbient;

Texture2D shadowMap;

SamplerState ShadowMapSampler
{
	MinFilter = linear;
	MagFilter = Anisotropic;
	AddressU = Wrap;
	AddressV = Wrap;
};


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


PixelToFrame ColorPixelShader(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;
	Output.Color = PSIn.Color;
	Output.Color.a = 1;
	return Output;
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

technique ColorTech
{
	pass Pass0
	{
		VertexShader = compile vs_4_0 SimplestVertexShader();
		PixelShader = compile ps_4_0 ColorPixelShader();
	}
}

technique ColorTechInstanced
{
	pass Pass0
	{
		VertexShader = compile vs_4_0 SimplestVertexShaderInstanced();
		PixelShader = compile ps_4_0 ColorPixelShader();
	}
}


struct SMapVertexToPixel
{
	float4 Position     : POSITION;
	float4 Position2D    : TEXCOORD0;
};

struct SMapPixelToFrame
{
	float4 Color : COLOR0;
};

struct SSceneVertexToPixel
{
	float4 Position             : POSITION;
	float4 Pos2DAsSeenByLight    : TEXCOORD0;

	float2 TexCoords            : TEXCOORD1;
	float3 Normal                : TEXCOORD2;
	float4 Position3D            : TEXCOORD3;

	float4 Color        : COLOR0;

};

struct SScenePixelToFrame
{
	float4 Color : COLOR0;
};

SMapVertexToPixel ShadowMapVertexShader(float4 inPos : POSITION)
{
	SMapVertexToPixel Output = (SMapVertexToPixel)0;

	Output.Position = mul(inPos, xLightsWorldViewProjection);
	Output.Position2D = Output.Position;

	return Output;
}


SMapPixelToFrame ShadowMapPixelShader(SMapVertexToPixel PSIn)
{
	SMapPixelToFrame Output = (SMapPixelToFrame)0;

	Output.Color = PSIn.Position2D.z / PSIn.Position2D.w;

	return Output;
}

SSceneVertexToPixel ShadowedSceneVertexShader(float4 inPos : POSITION, float2 inTexCoords : TEXCOORD0, float3 inNormal : NORMAL, float4 Color : COLOR0)
{
	SSceneVertexToPixel Output = (SSceneVertexToPixel)0;

	Output.Position = mul(inPos, xWorldViewProjection);
	Output.Pos2DAsSeenByLight = mul(inPos, xLightsWorldViewProjection);
	Output.Normal = normalize(mul(inNormal, (float3x3)xWorldMatrix));
	Output.Position3D = mul(inPos, xWorldMatrix);
	Output.TexCoords = inTexCoords;
	Output.Color = Color;
	return Output;
}


float DotProduct(float3 lightPos, float3 pos3D, float3 normal)
{
	float3 lightDir = normalize(pos3D - lightPos);
	return dot(-lightDir, normal);
}

SScenePixelToFrame ShadowedScenePixelShader(SSceneVertexToPixel PSIn)
{
	SScenePixelToFrame Output = (SScenePixelToFrame)0;

	float2 ProjectedTexCoords;
	ProjectedTexCoords[0] = PSIn.Pos2DAsSeenByLight.x / PSIn.Pos2DAsSeenByLight.w / 2.0f + 0.5f;
	ProjectedTexCoords[1] = -PSIn.Pos2DAsSeenByLight.y / PSIn.Pos2DAsSeenByLight.w / 2.0f + 0.5f;

	float diffuseLightingFactor = 0;
	if ((saturate(ProjectedTexCoords).x == ProjectedTexCoords.x) && (saturate(ProjectedTexCoords).y == ProjectedTexCoords.y))
	{
		float4 shadow = shadowMap.Sample(ShadowMapSampler, ProjectedTexCoords);
		float depthStoredInShadowMap = shadow.r;
		float realDistance = PSIn.Pos2DAsSeenByLight.z / PSIn.Pos2DAsSeenByLight.w;
		if ((realDistance - 1.0f / 100.0f) <= depthStoredInShadowMap)
		{
			diffuseLightingFactor = DotProduct(xLightPos, PSIn.Position3D, PSIn.Normal);
			diffuseLightingFactor = saturate(diffuseLightingFactor);
			diffuseLightingFactor *= xLightPower;
		}
	}

	float4 baseColor = PSIn.Color;
	Output.Color = baseColor * (diffuseLightingFactor + xAmbient);
	Output.Color.a = 1;
	return Output;
}


technique ColorTechShadowMap
{
	pass Pass0
	{
		VertexShader = compile vs_4_0 ShadowMapVertexShader();
		PixelShader = compile ps_4_0 ShadowMapPixelShader();
	}

	pass Pass1
	{
		VertexShader = compile vs_4_0 ShadowedSceneVertexShader();
		PixelShader = compile ps_4_0 ShadowedScenePixelShader();
	}
}


