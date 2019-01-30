


struct VertexToPixel
{
	float4 Position     : POSITION;
	float4 Color        : COLOR0;
	float4 BackgroundColor        : COLOR1;
	float2 TextureCoordinate        : TEXCOORD;
};


struct PixelToFrame
{
	float4 Color        : COLOR0;
};

Texture2D tileAtlas;
SamplerState textureSampler : register(s0) = sampler_state
{
	Filter = Point;
	MipFilter = Point;
	MagFilter = Point;
	MinFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};


float4x4 xViewProjection;

VertexToPixel SimplestVertexShader(float4 inPos : POSITION, float4 inColor : COLOR0, float4 inBackgroundColor : COLOR1, float2 inTextureCoord : TEXCOORD)
{
	VertexToPixel Output = (VertexToPixel)0;

	Output.Position = mul(inPos, xViewProjection);
	Output.Color = inColor;
	Output.BackgroundColor = inBackgroundColor;
	Output.TextureCoordinate = inTextureCoord;

	return Output;
}


PixelToFrame SimplePixelShader(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;

	float4 textureColor = tileAtlas.Sample(textureSampler, PSIn.TextureCoordinate);

	Output.Color = textureColor * PSIn.Color;
	Output.Color.a = textureColor.a;
	return Output;
}


PixelToFrame BackgroundPixelShader(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;
	Output.Color = PSIn.BackgroundColor;
	return Output;
}


PixelToFrame TexturePixelShader(VertexToPixel PSIn)
{
	float4 textureColor = tileAtlas.Sample(textureSampler, PSIn.TextureCoordinate);
	PixelToFrame Output = (PixelToFrame)0;
	Output.Color = textureColor;
	return Output;
}

technique Point
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


technique TextureTecnique
{
	pass Pass0
	{
		VertexShader = compile vs_4_0 SimplestVertexShader();
		PixelShader = compile ps_4_0 TexturePixelShader();
	}
}