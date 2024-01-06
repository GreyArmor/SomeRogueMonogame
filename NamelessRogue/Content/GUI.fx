﻿
Texture2D FontTexture : register(t0);
float4x4 xProjection;
SamplerState textureSampler : register(s0);

struct PS_INPUT
{
    float4 pos : SV_POSITION;
    float4 col : COLOR0;
    float2 uv : TEXCOORD0;
};


float4 PS(PS_INPUT input) : SV_Target
{
    float4 out_col = input.col * FontTexture.Sample(textureSampler, input.uv);
    return out_col;
  //  return float4(1, 1, 1, 1);
}

struct VS_INPUT
{
    float2 pos : POSITION0;
    float2 uv : TEXCOORD0;
    float4 col : COLOR0;
};

PS_INPUT VS(VS_INPUT input)
{
    PS_INPUT output;
    //
    output.pos = mul(xProjection, float4(input.pos.x, input.pos.y, 0.f, 1.f));
    output.col = input.col;
    output.uv = input.uv;
    return output;
}

