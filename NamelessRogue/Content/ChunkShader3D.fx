﻿#include "Common.fx"

//shadow mapping
float4x4 xLightsWorldViewProjection;
float3 xLightPos;
float xLightPower;
float xAmbient;

Texture2D shadowMap : register(t1);

SamplerState ShadowMapSampler
{
	Filter = Point;
	MipFilter = Point;
	MagFilter = Point;
	MinFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};


//instance of billboard sprite data
struct VertexShaderBillboardInstanceInput
{
	float4 position : TEXCOORD1;
};


struct VSInTerrain
{
	//x is our height, y is our yaw and x is out pitch
    float3 vertexHeightYawPitch : TEXCOORD1;
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
	
    float4 textureColor = tileAtlas.Sample(textureSampler, PSIn.TextureCoordinate);
    Output.Color = textureColor;
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

struct TerrainShadowSceneVertexToPixel
{
    float4 Position : POSITION;
	float2 TexCoords : TEXCOORD;
    float4 Pos2DAsSeenByLight : TEXCOORD1;
    float3 Normal : TEXCOORD2;
    float4 Position3D : TEXCOORD3;
    
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


float mod(float x, float y)
{
    return x - y * floor(x / y);
}

float trueclamp(float x, float min, float max)
{
    if (x < min)
    {
        return min;
    }
    else if (x > max)
    {
        return max;
    }
    else
    {
        return x;
    }
}


float rowIndexEnd = 33;
float substractionCoef = 1;
float verticesPerRow = 36;

float4 calculateTerrainVertex(VSInTerrain input, uint vertexId)
{
    float4 position = (float4) 0;
    float rowIndex = mod(vertexId, verticesPerRow);
    float clampedIndex = clamp(rowIndex - substractionCoef, 0, rowIndexEnd);
    position.x = floor(clampedIndex / 2);
    position.y = mod(clampedIndex, 2);
	//new row
    float row = floor(vertexId / verticesPerRow);
    position.y += row;
    position.z = input.vertexHeightYawPitch.x;
    position.w = 1;
    return position;
}

SMapVertexToPixel TerrainShadowMapVertexShader(VSInTerrain input, uint vertexId : SV_VertexID)
{
    SMapVertexToPixel Output = (SMapVertexToPixel) 0;
	
    float4 position = calculateTerrainVertex(input, vertexId);
    float4 worldPosition = mul(position, xWorldMatrix);
    Output.Position = mul(worldPosition, xLightsWorldViewProjection);
    Output.Position2D = Output.Position;
    return Output;
}


TerrainShadowSceneVertexToPixel TerrainShadowedSceneVertexShader(VSInTerrain input, uint vertexId : SV_VertexID)
{
    TerrainShadowSceneVertexToPixel Output = (TerrainShadowSceneVertexToPixel) 0;
	
    float4 position = calculateTerrainVertex(input, vertexId);	
	float yaw = input.vertexHeightYawPitch.y;
    float pitch = input.vertexHeightYawPitch.z;
	float3 normal = (float3) 0;
    normal.x = cos(pitch) * sin(yaw);
    normal.y = sin(pitch);
    normal.z = cos(pitch) * cos(yaw);
	
    float4x4 worldViewProjection = mul(mul(xWorldMatrix, xView), xProjection);
	
    float4 posW = mul(position, xWorldMatrix);
    //float4 posV = mul(posW, xView);
    float4 pos = mul(position, worldViewProjection);
    Output.Position = pos; //    mul(position, xWorldViewProjection); //mul(mul(position, xView),xProjection);
    Output.Pos2DAsSeenByLight = mul(posW, xLightsWorldViewProjection);
    Output.Normal = normal;
    Output.Position3D = posW;
    float chunkSize = rowIndexEnd - 1;
    Output.TexCoords = float2((position.x) / chunkSize, (position.y) / chunkSize);
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


SScenePixelToFrame ShadowedScenePixelShader(SSceneVertexToPixel PSIn)
{
	SScenePixelToFrame Output = (SScenePixelToFrame)0;

	float2 ProjectedTexCoords;
	ProjectedTexCoords[0] = PSIn.Pos2DAsSeenByLight.x / PSIn.Pos2DAsSeenByLight.w / 2.0f + 0.5;
	ProjectedTexCoords[1] = -PSIn.Pos2DAsSeenByLight.y / PSIn.Pos2DAsSeenByLight.w / 2.0f + 0.5;

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

	float4 baseColor = tileAtlas.Sample(textureSampler, PSIn.TexCoords);
	Output.Color = baseColor * (diffuseLightingFactor + xAmbient);
	Output.Color.a = 1;
	return Output;
}

SScenePixelToFrame TerrainShadowedScenePixelShader(TerrainShadowSceneVertexToPixel PSIn)
{
    SScenePixelToFrame Output = (SScenePixelToFrame) 0;

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

    float2 texCoords = PSIn.Position.xy / 16;
    float4 baseColor = tileAtlas.Sample(textureSampler, PSIn.TexCoords);
    Output.Color = baseColor * (diffuseLightingFactor + xAmbient);
    Output.Color.a = 1;
    return Output;
}

SScenePixelToFrame SimplePixelShaderTerrain(TerrainShadowSceneVertexToPixel PSIn)
{
    SScenePixelToFrame Output = (SScenePixelToFrame) 0;
    float4 baseColor = tileAtlas.Sample(textureSampler, PSIn.TexCoords);
    Output.Color = baseColor;
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


technique TerrrainTextureTechShadowMap
{
    pass Pass0
    {
        VertexShader = compile vs_4_0 TerrainShadowMapVertexShader();
        PixelShader = compile ps_4_0 ShadowMapPixelShader();
    }

    pass Pass1
    {
        VertexShader = compile vs_4_0 TerrainShadowedSceneVertexShader();
        PixelShader = compile ps_4_0 TerrainShadowedScenePixelShader();
    }
}

technique TerrrainTextureTechSimple
{
    pass Pass0
    {
        VertexShader = compile vs_4_0 TerrainShadowedSceneVertexShader();
        PixelShader = compile ps_4_0 SimplePixelShaderTerrain();
    }
}


