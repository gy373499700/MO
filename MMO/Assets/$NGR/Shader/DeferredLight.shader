Shader "GDShader/DeferredLight" {
	Properties {
		//_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "DeferredLight"="Light" }
		LOD 200
		//mark stencil
		pass{
			Stencil
			{
				Ref 128
				ReadMask 128
				WriteMask 128
				Comp always
				Pass replace
				Fail zero
				ZFail zero
			}
			ZTest Greater
			ZWrite Off
			Cull Front
			ColorMask 0
			CGPROGRAM	
			#pragma vertex vert
			#pragma fragment frag
			#include "LightCommon.cg"
			#include "UnityCG.cginc"
			
			sampler2D	_MainTex;
			float4		_MainTex_ST;
			float4		_LightPos = float4(0,0,0,1);
			float4		_Color;
			float4		_FarCorner;
			sampler2D	_Diffuse;

			
			struct v2f{
				float4 pos 			: SV_POSITION;
				float4 lightpos 	: TEXCOORD0;
				float4 proj_pos		: TEXCOORD1;
			};
			
			v2f vert(appdata_full v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP,float4(v.vertex.xyz*_LightPos.w*2,1.0f));//*_LightPos.w);//*_LightPos.w+float4(_LightPos.xyz,0));
	
				o.lightpos  =	_LightPos;
				o.proj_pos	=	o.pos;
#if UNITY_UV_STARTS_AT_TOP
				o.proj_pos.y *= -1;
#endif
				return o;
			}
			float4 frag(v2f i) : COLOR
			{
				return 0;
			}
			ENDCG
		}
		//lighting no shadow
		pass{
			Stencil
			{
				Ref 128
				ReadMask 128
				WriteMask 128
				Comp equal
				Pass zero
				Fail zero
				ZFail zero
			}
			ZTest Less
			ZWrite Off
			ColorMask RGBA
			Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define CAST_SHADOW 0
			#include "PointLight.cg"




			ENDCG
		}

		pass{

			ZTest Greater
			ZWrite Off
			Cull Front
			ColorMask RGBA
			Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define CAST_SHADOW 0
			#include "PointLight.cg"
			ENDCG
		}

		//lighting with shadow
		pass {
			Stencil
			{
				Ref 128
				ReadMask 128
				WriteMask 128
				Comp equal
				Pass zero
				Fail zero
				ZFail zero
			}
				ZTest Less
				ZWrite Off
				ColorMask RGBA
				Blend One One

				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				#define CAST_SHADOW 1
				#include "PointLight.cg"

				ENDCG
		}

		pass {

			ZTest Greater
				ZWrite Off
				Cull Front
				ColorMask RGBA
				Blend One One

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#define CAST_SHADOW 1
				#include "PointLight.cg"

				
				ENDCG
		}
		//lighting with dynamic shadow
		pass {
			Stencil
			{
				Ref 128
				ReadMask 128
				WriteMask 128
				Comp equal
				Pass zero
				Fail zero
				ZFail zero
			}
				ZTest Less
				ZWrite Off
				ColorMask RGBA
				Blend One One

				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				#define CAST_SHADOW 1
				#define DYNAMIC_SHADOW 1
				#include "PointLight.cg"
				ENDCG

		}

		pass {

			ZTest Greater
				ZWrite Off
				Cull Front
				ColorMask RGBA
				Blend One One

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#define CAST_SHADOW 1
				#define DYNAMIC_SHADOW 1
				#include "PointLight.cg"




				ENDCG
		}
	} 
	FallBack "Diffuse"
}
