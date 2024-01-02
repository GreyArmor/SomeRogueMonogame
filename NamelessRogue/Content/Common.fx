
float3 CameraPosition;
float3 CameraUp;
float3 CameraRight;
Texture2D tileAtlas : register(t0);
float4x4 xView;
float4x4 xProjection;
float4x4 xViewProjection;
float4x4 xWorldViewProjection;
float4x4 xWorldMatrix;
float4x4 xBillboard;

float2 windCoef;
bool windFlag;;

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
    float4 Position : POSITION;
    float4 Color : COLOR0;
    float4 BackgroundColor : COLOR1;
    float2 TextureCoordinate : TEXCOORD;
    float3 Normal : NORMAL0;
    float3 WorldPos : TEXCOORD1;
};

//instance transform matrix
struct VertexShaderInstanceInput
{
    float4 row1 : TEXCOORD1;
    float4 row2 : TEXCOORD2;
    float4 row3 : TEXCOORD3;
    float4 row4 : TEXCOORD4;
};

struct PixelToFrame
{
    float4 Color : COLOR0;
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



// implementation of MurmurHash (https://sites.google.com/site/murmurhash/) for a 
// single unsigned integer.

uint hash(uint x, uint seed)
{
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

uint hash(float2 x, uint seed)
{
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


float2 gradientDirection(uint hash)
{
    switch (int(hash) & 3)
    { // look at the last two bits to pick a gradient direction
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

float interpolate(float value1, float value2, float value3, float value4, float2 t)
{
    return lerp(lerp(value1, value2, t.x), lerp(value3, value4, t.x), t.y);
}

float2 fade(float2 t)
{
    // 6t^5 - 15t^4 + 10t^3
    return t * t * t * (t * (t * 6.0 - 15.0) + 10.0);
}

float perlinNoise(float2 position, uint seed)
{
    float2 floorPosition = floor(position);
    float2 fractPosition = position - floorPosition;
    float2 cellCoordinates = float2(floorPosition);
    float value1 = dot(gradientDirection(hash(cellCoordinates, seed)), fractPosition);
    float value2 = dot(gradientDirection(hash((cellCoordinates + float2(1, 0)), seed)), fractPosition - float2(1.0, 0.0));
    float value3 = dot(gradientDirection(hash((cellCoordinates + float2(0, 1)), seed)), fractPosition - float2(0.0, 1.0));
    float value4 = dot(gradientDirection(hash((cellCoordinates + float2(1, 1)), seed)), fractPosition - float2(1.0, 1.0));
    return interpolate(value1, value2, value3, value4, fade(fractPosition));
}

float perlinNoise(float2 position, int frequency, int octaveCount, float persistence, float lacunarity, uint seed)
{
    float value = 0.0;
    float amplitude = 1.0;
    float currentFrequency = float(frequency);
    uint currentSeed = seed;
    for (int i = 0; i < octaveCount; i++)
    {
        currentSeed = hash(currentSeed, 0x0U); // create a new seed for each octave
        value += perlinNoise(position * currentFrequency, currentSeed) * amplitude;
        amplitude *= persistence;
        currentFrequency *= lacunarity;
    }
    return value;
}

float4x4 CreateMatrixFromRows(
	float4 row1, float4 row2, float4 row3, float4 row4)
{
    return float4x4(
		row1.x, row1.y, row1.z, row1.w,
		row2.x, row2.y, row2.z, row2.w,
		row3.x, row3.y, row3.z, row3.w,
		row4.x, row4.y, row4.z, row4.w);
}

float DotProduct(float3 lightPos, float3 pos3D, float3 normal)
{
    float3 lightDir = normalize(pos3D - lightPos);
    return dot(-lightDir, normal);
}

VertexToPixel emptyVs(VSIn input, VertexShaderInstanceInput instanceInput)
{
    VertexToPixel output = (VertexToPixel)0;

    return output;
}

PixelToFrame emptyPs(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame) 0;
    return Output;
}


technique StupidMonogameRules
{
    pass Pass0
    {
        VertexShader = compile vs_4_0 emptyVs();
        PixelShader = compile ps_4_0 emptyPs();
    }
}
