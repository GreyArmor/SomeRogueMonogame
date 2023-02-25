
float3 CameraPosition;
Texture2D tileAtlas;
float4x4 xViewProjection;

float4x4 xTextureCoordinatesDistanceScaling;

float3 SunLightDirection;
float4 SunLightColor;
float SunLightIntensity;

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


VertexToPixel SimplestVertexShader(float4 inPos : POSITION, float4 inColor : COLOR0, float4 inBackgroundColor : COLOR1, float2 inTextureCoord : TEXCOORD, float3 normal : NORMAL)
{
	VertexToPixel Output = (VertexToPixel)0;

	Output.Position = mul(inPos, xViewProjection);

	Output.Color = inColor;
	Output.BackgroundColor = inBackgroundColor;
	Output.TextureCoordinate = inTextureCoord;
	Output.Normal = normal;
	//no world matrix yet, so just pass the point
	Output.WorldPos = inPos;
	return Output;
}


PixelToFrame SimplePixelShader(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;
	Output.Color = float4(0,0,0,1);
	return Output;
}


float4 CalcDiffuseLight(float3 normal, float3 lightDirection, float4 lightColor, float lightIntensity)
{
	return saturate(dot(normal, -lightDirection)) * lightIntensity * lightColor;
}

float4 CalcSpecularLight(float3 normal, float3 lightDirection, float3 cameraDirection, float4 lightColor, float lightIntensity)
{
	//float4 specular = SpecularIntensity * SpecularColor * max(pow(dotProduct, Shininess), 0) * length(input.Color);

	float3 halfVector = normalize(-lightDirection + -cameraDirection);
	float specular = saturate(dot(halfVector, normal));

	//I have all models be the same reflectance
	float specularPower = 20;

	return lightIntensity * lightColor * pow(abs(specular), specularPower);
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

	/*
	PixelToFrame Output = (PixelToFrame)0;
	//float3 lightToPoint = v3GlobalLight - PSIn.Position;
	//float light = abs(dot(lightToPoint, PSIn.Normal));
	float4 baseColor = input.BackgroundColor;
	float4 diffuseLight = float4(0, 0, 0, 0);
	float4 specularLight = float4(0, 0, 0, 0);

	//calculate our viewDirection
	float3 pos = input.WorldPos;
	float3 cameraDirection = normalize(pos - CameraPosition);

	//calculate our sunlight
	diffuseLight += CalcDiffuseLight(input.Normal, SunLightDirection, SunLightColor, SunLightIntensity);
	specularLight += CalcSpecularLight(input.Normal, SunLightDirection, cameraDirection, SunLightColor, SunLightIntensity);

	//calculate our pointLights
	//[loop]
	//for (int i = 0; i < MaxLightsRendered; i++)
	//{
	//	float3 PointLightDirection = input.WorldPos - PointLightPosition[i];

	//	float DistanceSq = lengthSquared(PointLightDirection);

	//	float radius = PointLightRadius[i];

	//	[branch]
	//	if (DistanceSq < abs(radius * radius))
	//	{
	//		float Distance = sqrt(DistanceSq);

	//		//normalize
	//		PointLightDirection /= Distance;

	//		float du = Distance / (1 - DistanceSq / (radius * radius - 1));

	//		float denom = du / abs(radius) + 1;

	//		//The attenuation is the falloff of the light depending on distance basically
	//		float attenuation = 1 / (denom * denom);

	//		diffuseLight += CalcDiffuseLight(input.Normal, PointLightDirection, PointLightColor[i], PointLightIntensity[i]) * attenuation;

	//		specularLight += CalcSpecularLight(input.Normal, PointLightDirection, cameraDirection, PointLightColor[i], PointLightIntensity[i]) * attenuation;
	//	}
	//}

	Output.Color = (baseColor * diffuseLight) + baseColor * SunLightColor * SunLightIntensity;
	Output.Color.a = 1;
	return Output;
	*/
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


