
// implementation of MurmurHash (https://sites.google.com/site/murmurhash/) for a 
// single unsigned integer.

uint hash(uint x, uint seed) {
    const uint m = 0x5bd1e995U;
    uint hash = seed;
    // process input
    uint k = x;
    k *= m;
    k ^= k >> 24;
    k *= m;
    hash *= m;
    hash ^= k;
    // some final mixing
    hash ^= hash >> 13;
    hash *= m;
    hash ^= hash >> 15;
    return hash;
}

// implementation of MurmurHash (https://sites.google.com/site/murmurhash/) for a  
// 2-dimensional unsigned integer input vector.

uint hash(float2 x, uint seed){
    const uint m = 0x5bd1e995U;
    uint hash = seed;
    // process first vector element
    uint k = x.x; 
    k *= m;
    k ^= k >> 24;
    k *= m;
    hash *= m;
    hash ^= k;
    // process second vector element
    k = x.y; 
    k *= m;
    k ^= k >> 24;
    k *= m;
    hash *= m;
    hash ^= k;
	// some final mixing
    hash ^= hash >> 13;
    hash *= m;
    hash ^= hash >> 15;
    return hash;
}


float2 gradientDirection(uint hash) {
    switch (int(hash) & 3) { // look at the last two bits to pick a gradient direction
    case 0:
        return float2(1.0, 1.0);
    case 1:
        return float2(-1.0, 1.0);
    case 2:
        return float2(1.0, -1.0);
    case 3:
        return float2(-1.0, -1.0);
    }
}

float interpolate(float value1, float value2, float value3, float value4, float2 t) {
    return lerp(lerp(value1, value2, t.x), lerp(value3, value4, t.x), t.y);
}

float2 fade(float2 t) {
    // 6t^5 - 15t^4 + 10t^3
	return t * t * t * (t * (t * 6.0 - 15.0) + 10.0);
}

float perlinNoise(float2 position, uint seed) {
    float2 floorPosition = floor(position);
    float2 fractPosition = position - floorPosition;
    float2 cellCoordinates = float2(floorPosition);
    float value1 = dot(gradientDirection(hash(cellCoordinates, seed)), fractPosition);
    float value2 = dot(gradientDirection(hash((cellCoordinates + float2(1, 0)), seed)), fractPosition - float2(1.0, 0.0));
    float value3 = dot(gradientDirection(hash((cellCoordinates + float2(0, 1)), seed)), fractPosition - float2(0.0, 1.0));
    float value4 = dot(gradientDirection(hash((cellCoordinates + float2(1, 1)), seed)), fractPosition - float2(1.0, 1.0));
    return interpolate(value1, value2, value3, value4, fade(fractPosition));
}

float perlinNoise(float2 position, int frequency, int octaveCount, float persistence, float lacunarity, uint seed) {
    float value = 0.0;
    float amplitude = 1.0;
    float currentFrequency = float(frequency);
    uint currentSeed = seed;
    for (int i = 0; i < octaveCount; i++) {
        currentSeed = hash(currentSeed, 0x0U); // create a new seed for each octave
        value += perlinNoise(position * currentFrequency, currentSeed) * amplitude;
        amplitude *= persistence;
        currentFrequency *= lacunarity;
    }
    return value;
}


//////
//////
//////
//////
//////


float3 CameraPosition;
float3 CameraUp;
float3 CameraRight;
Texture2D tileAtlas;
float4x4 xView;
float4x4 xProjection;
float4x4 xViewProjection;
float4x4 xWorldViewProjection;
float4x4 xWorldMatrix;
float4x4 xBillboard;

//shadow mapping
float4x4 xLightsWorldViewProjection;
float3 xLightPos;
float xLightPower;
float xAmbient;

float2 windCoef;
bool windFlag;



Texture2D shadowMap;

SamplerState ShadowMapSampler
{
	Filter = Point;
	MipFilter = Point;
	MagFilter = Point;
	MinFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};


//instance transform matrix
struct VertexShaderInstanceInput
{
	float4 row1 : TEXCOORD1;
	float4 row2 : TEXCOORD2;
	float4 row3 : TEXCOORD3;
	float4 row4 : TEXCOORD4;
};

//instance of billboard sprite data
struct VertexShaderBillboardInstanceInput
{
	float4 position : TEXCOORD1;
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

struct VSInTerrain
{
	//x is our height, y is our yaw and x is out pitch
	float3 vertexHeightYawPitch : POSITION;
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

VertexToPixel BillboardVertexShaderInstanced(VSIn input, VertexShaderInstanceInput instanceInput)
{
	VertexToPixel output;	
	output.Color = input.inColor;
	output.BackgroundColor = input.inBackgroundColor;
	output.TextureCoordinate = input.inTextureCoord;
	output.Normal = input.normal;
	

	float4x4 worldMatrixInstance = CreateMatrixFromRows(
		instanceInput.row1,
		instanceInput.row2,
		instanceInput.row3,
		instanceInput.row4);

	float3 position = input.inPos;

	if(windFlag && position.y > 0)
	{
		float4 unitOneTWorld = mul(float4(1,1,0,1), worldMatrixInstance);
		float windStrenght = 0.1;
		uint seed = 12355;
		float noiseValue = perlinNoise(float3(unitOneTWorld.x + windCoef.x, unitOneTWorld.y + windCoef.y , 0.5), 1, 6, 0.5, 2.0, seed); 
	
		position.x += noiseValue;
	}

	

	float4 billboard = mul(float4(position.x, position.y, 0, 1), xBillboard);

	float4 world = mul(billboard, worldMatrixInstance);

	float4 viewPosition = mul(world, xView);

	float4 pos = mul(viewPosition, xProjection);

	output.Position = pos;
	output.WorldPos = pos.xyz;

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


PixelToFrame SimplePixelShaderTextured(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;

	float4 textureColor = tileAtlas.Sample(textureSampler, PSIn.TextureCoordinate);

	if(textureColor.a == 0)
	{
		discard;
	}

	Output.Color = textureColor * PSIn.Color;
	Output.Color.a = textureColor.a;
	return Output;
}


technique TextureTechInstanced
{
	pass Pass0
	{
		VertexShader = compile vs_4_0 BillboardVertexShaderInstanced();
		PixelShader = compile ps_4_0 SimplePixelShaderTextured();
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



int chunkVerticesCount;
SMapVertexToPixel TerrainShadowMapVertexShader(VSInTerrain input, uint vertexId : SV_VertexID)
{
    SMapVertexToPixel Output = (SMapVertexToPixel) 0;

    return Output;
}

float mod(float x, float y)
{
    return x - y * floor(x / y);
}


float rowIndexEnd = 33;
float verticesPerRow = 36;
float substractionCoef = 1;
TerrainShadowSceneVertexToPixel TerrainShadowedSceneVertexShader(VSInTerrain input, uint vertexId : SV_VertexID)
{
    TerrainShadowSceneVertexToPixel Output = (TerrainShadowSceneVertexToPixel) 0;
    //float vertexIdfloat = vertexId;
    float rowIndex = mod(vertexId, verticesPerRow);
    float clampedIndex = clamp(rowIndex - substractionCoef, 0, rowIndexEnd);
	
    float4 position = (float4) 0;
  	    
    position.y = floor(clampedIndex / 2);
    position.x = mod(clampedIndex, 2); 
	
	//new row
    float row = floor(vertexId / rowIndexEnd);
    position.x += row;
	
    position.z = 0;
    position.z = input.vertexHeightYawPitch.x;

    position.w = 1;	
	
	float yaw = input.vertexHeightYawPitch.y;
    float pitch = input.vertexHeightYawPitch.z;
	float3 normal = (float3) 0;
    normal.x = 0;//    cos(pitch) * sin(yaw);
    normal.y = 0; //sin(pitch);
    normal.z = 1; // cos(pitch) * cos(yaw);
	
	
    //float4 viewPosition = mul(position, xWorldViewProjection);

    float4 pos = mul(position, xWorldViewProjection);

    Output.Position = pos; //    mul(position, xWorldViewProjection); //mul(mul(position, xView),xProjection);
    Output.Pos2DAsSeenByLight = mul(pos, xLightsWorldViewProjection);
    Output.Normal = normalize(mul(normal, (float3x3) xWorldMatrix));
    Output.Position3D = mul(pos, xWorldMatrix);
    Output.TexCoords = float2(position.y / 32.0, position.x / 32.0);
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

    //float2 ProjectedTexCoords;
    //ProjectedTexCoords[0] = PSIn.Pos2DAsSeenByLight.x / PSIn.Pos2DAsSeenByLight.w / 2.0f + 0.5f;
    //ProjectedTexCoords[1] = -PSIn.Pos2DAsSeenByLight.y / PSIn.Pos2DAsSeenByLight.w / 2.0f + 0.5f;

    //float diffuseLightingFactor = 0;
    //if ((saturate(ProjectedTexCoords).x == ProjectedTexCoords.x) && (saturate(ProjectedTexCoords).y == ProjectedTexCoords.y))
    //{
    //    float4 shadow = shadowMap.Sample(ShadowMapSampler, ProjectedTexCoords);
    //    float depthStoredInShadowMap = shadow.r;
    //    float realDistance = PSIn.Pos2DAsSeenByLight.z / PSIn.Pos2DAsSeenByLight.w;
    //    if ((realDistance - 1.0f / 100.0f) <= depthStoredInShadowMap)
    //    {
    //        diffuseLightingFactor = DotProduct(xLightPos, PSIn.Position3D, PSIn.Normal);
    //        diffuseLightingFactor = saturate(diffuseLightingFactor);
    //        diffuseLightingFactor *= xLightPower;
    //    }
    //}

    float2 texCoords = PSIn.Position.xy / 16;
    float4 baseColor = tileAtlas.Sample(textureSampler, texCoords);
  //  Output.Color = baseColor * (diffuseLightingFactor + xAmbient);
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


