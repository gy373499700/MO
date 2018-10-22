// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/ScreenRain"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 100

		Pass
	{
	

		Cull Off ZWrite Off ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag


#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 pos : SV_POSITION;
		//float4 wpos : TEXCOORD1;
		float4 proj_pos : TEXCOORD2;
	};
	sampler2D	_RainTexture;
	float4 _RainTexture_ST;
	sampler2D	_Depth; 
	float4 _UVData;
	float3 _UVParam;


	v2f vert(appdata v)
	{

		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _RainTexture);
		o.proj_pos = o.pos;
#if SHADER_API_GLES || SHADER_API_GLES3

#else
		//SHADER_API_METAL
		o.proj_pos.y *= -1;
#endif
 
		return o;
	}
	float4 CalculateRainLayer(float2 screenUV, float sceneViewDepth, float2 layerUV)
	{
		float2 rainAndDepth = tex2D(_RainTexture, layerUV).gb;
		float layerDistance = rainAndDepth.g * _UVParam.y + _UVParam.x;
		//if (forcedDistance >= 0.f)
		//	layerDistance = forcedDistance;
		float depthScale = saturate((sceneViewDepth - layerDistance) * 2.0f);
		float	output = rainAndDepth.r * depthScale;
		return output;
	}
	fixed4 frag(v2f i) : SV_Target
	{ 
		float4 pos = i.proj_pos / i.proj_pos.w;
		float2 screenUV = pos.xy*0.5 + 0.5;
		float4 depth_normal = tex2D(_Depth, screenUV);
		float2 XY_Depth = float2(1.0f,0.003921568627451);
		float linearViewDepth = dot(XY_Depth,depth_normal.xy);
		float2 uv0 = i.uv * _UVData.xy + _UVData.zw * _Time.x;
		float4 rain0 = CalculateRainLayer(screenUV, linearViewDepth, uv0);
		float Alpha = (0.5f - abs(i.uv.y - 0.5f))*0.5;
		return float4(1, 1, 1, rain0.a*_UVParam.z*Alpha);
	
	}
		ENDCG
	}
	}
}
