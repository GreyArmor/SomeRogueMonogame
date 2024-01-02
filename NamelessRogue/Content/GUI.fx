
Texture2D DiffuseTexture : register(t0);
float4x4 xProjection;

struct VSIn
{
    float4 inPos : POSITION;
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

struct PixelToFrame
{
    float4 Color : COLOR0;
};


VertexToPixel GUI_VS(VSIn input)
{
    VertexToPixel output = (VertexToPixel) 0;
    output.Position = mul(input.inPos, xProjection);
    return output;
}

float4 GUI_PS(VertexToPixel PSIn) : SV_TARGET
{
    return float4(1, 0, 0, 1);
}

