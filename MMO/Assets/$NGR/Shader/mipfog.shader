Shader "GDShader/MipFog" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		pass {
			Blend SrcAlpha OneMinusSrcAlpha
			//Blend One One
			ColorMask RGB
			ZTest Greater
			ZWrite Off
				Cull Off
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "LightCommon.cg"

		sampler2D	_MainTex;
		sampler2D	_Depth;
		sampler2D	_SkyTexture;
		float4		farCorner;
		float4		_ViewUp;
		float4		invViewport_Radius;
		float4		FogColor;
		float4		FogDistance;
		float4x4	ViewToWorld;
		float4		_AmbientColor;
		struct v2f {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			float4 viewpos  : TEXCOORD1;
		};

		v2f vert(appdata_full v)
		{
			v2f o;

#if SHADER_API_GLES || SHADER_API_GLES3
			//Mobile
			o.pos = float4(v.vertex.x * 2, v.vertex.y * 2, 1, 1);
			o.uv = o.pos*0.5f + 0.5f;
			o.viewpos = float4(o.pos.xy, 1.0f, 1)*farCorner;
#else
			//Editor Only 
			o.pos = float4(v.vertex.x * 2, v.vertex.y * 2, 1, 1);
			o.uv = o.pos*0.5f + 0.5f;
			o.uv.y = 1 - o.uv.y;
			o.viewpos = float4(o.pos.xy, 1.0f, 1)*farCorner;
			o.viewpos.y *= -1;
#endif


			return o;
		}
#define MATH_PI 3.14159265f
		float2 DirectionToUV(float3 dir)
		{
			float2 uv = float2(atan(dir.z / dir.x) * 0.5f, acos(dir.y)) / MATH_PI + float2(dir.x >= 0, 0)*0.5;
			uv.x -= floor(uv.x);
			uv.y = 1 - uv.y;
			return uv;
		}
		float4 frag(v2f IN) : COLOR
		{
			float4 depth_normal = tex2D(_Depth,IN.uv);
			float2 XY_Depth = float2(1.0f,0.003921568627451);
			float depth = dot(XY_Depth,depth_normal.xy);
			//float3 pos = IN.corner*depth;
			float3 dir = mul((float3x3)ViewToWorld, normalize(IN.viewpos.xyz));
			float2 sky_uv = DirectionToUV(dir);
			float alpha = saturate((depth*farCorner.z - FogDistance.x) / (FogDistance.y - FogDistance.x));
			float mipLevel = (1-(alpha))*9.0f;
			//float4 c = _AmbientColor*pow(tex2Dlod(_SkyTexture, float4(sky_uv, 0, mipLevel)),2.2f);
			float4 c = _AmbientColor*pow(tex2D(_SkyTexture, sky_uv), 2.2f);

			return float4(c.xyz, alpha);
		}
			ENDCG
	}
	}
		FallBack "Diffuse"
}
