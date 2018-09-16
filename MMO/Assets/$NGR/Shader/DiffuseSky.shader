Shader "GDShader/DiffuseSky" {

	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Main Color", Color) = (0.5,0.5,0.5,0.5)
	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue"="Geometry+20"}
		LOD 200
		pass{
			Stencil
			{
				Ref 4
				Comp always
				Pass replace
				//ZFail keep
			}
			ZTest LEqual
			Cull Off
			ColorMask RGB
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#define SHADER_API_GLES
			#include "UnityCG.cginc"
			
			sampler2D	_MainTex;
			float4		_MainTex_ST;

			float4		_Color;
			float4		_FarCorner;
			float4x4	_InvViewMatrix;
			float _DirY;
			
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 viewpos  : TEXCOORD1;
			};

			v2f vert (appdata_full v)
			{
				v2f o;

				#if SHADER_API_GLES || SHADER_API_GLES3
				//Mobile
				o.pos = float4(v.vertex.x*2,v.vertex.y*2,1,1);
				o.uv = o.pos*0.5f+0.5f;
				o.viewpos = float4(o.pos.xy,1.0f,1)*_FarCorner;
				#else
				//Editor Only 
				o.pos = float4(v.vertex.x*2,v.vertex.y*2, 1,1);
				o.uv = o.pos*0.5f+0.5f;
				o.uv.y = 1-o.uv.y;
				o.viewpos = float4(o.pos.xy,1.0f,1)*_FarCorner;
				o.viewpos.y*=-1;
				#endif


				return o;
			}
			#define MATH_PI 3.14159265f
			float2 DirectionToUV(float3 dir)
			{
				float2 uv = float2(atan(dir.z / dir.x) * 0.5f,acos(dir.y))/MATH_PI +float2( dir.x>=0,0)*0.5;
				uv.x-=floor(uv.x);
				uv.y = 1-uv.y;
				return uv;
			}
			float4 frag(v2f i) : COLOR
			{
				float4 ret;

				float3 dir = normalize(i.viewpos.xyz);
				float3 world_dir = mul((float3x3)_InvViewMatrix,dir);
				world_dir.y *= _DirY;
				float2 sample_uv = DirectionToUV(world_dir);

				ret.xyz   =   pow(tex2D(_MainTex,sample_uv),2.2f)*_Color.xyz;
				ret.w   =   _Color.w;
			
				return ret;
			}
			ENDCG
		}
		pass {
			Stencil
			{
				Ref 4
				Comp always
				Pass replace
				//ZFail keep
			}
				ZTest LEqual
				Cull Off
				ColorMask RGB
				CGPROGRAM
#pragma vertex vert
#pragma fragment frag
				//#define SHADER_API_GLES
#include "UnityCG.cginc"

			samplerCUBE	_SkyCubeTex;

			float4		_Color;
			float4		_FarCorner;
			float4x4	_InvViewMatrix;
			float _DirY;

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
				o.pos = float4(v.vertex.x * 2,v.vertex.y * 2,1,1);
				o.uv = o.pos*0.5f + 0.5f;
				o.viewpos = float4(o.pos.xy,1.0f,1)*_FarCorner;
#else
				//Editor Only 
				o.pos = float4(v.vertex.x * 2,v.vertex.y * 2, 1,1);
				o.uv = o.pos*0.5f + 0.5f;
				o.uv.y = 1 - o.uv.y;
				o.viewpos = float4(o.pos.xy,1.0f,1)*_FarCorner;
				o.viewpos.y *= -1;
#endif


				return o;
			}

			float4 frag(v2f i) : COLOR
			{
				float4 ret;

			float3 dir = normalize(i.viewpos.xyz);
			float3 world_dir = mul((float3x3)_InvViewMatrix,dir);
			world_dir.y *= _DirY;

			ret.xyz = pow(texCUBE(_SkyCubeTex, world_dir),2.2f)*_Color.xyz;
			ret.w = _Color.w;

			return ret;
			}
				ENDCG
		}
	} 
	FallBack "Diffuse"
}

