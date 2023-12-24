#include "Common.fx"
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

    if (windFlag && position.y > 0)
    {
        float4 unitOneTWorld = mul(float4(1, 1, 0, 1), worldMatrixInstance);
        float windStrenght = 0.1;
        uint seed = 12355;
        float noiseValue = perlinNoise(float3(unitOneTWorld.x + windCoef.x, unitOneTWorld.y + windCoef.y, 0.5), 1, 6, 0.5, 2.0, seed);
	
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

PixelToFrame SimplePixelShaderTextured(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame) 0;

    float4 textureColor = tileAtlas.Sample(textureSampler, PSIn.TextureCoordinate);

    if (textureColor.a == 0)
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
