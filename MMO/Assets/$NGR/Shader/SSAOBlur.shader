Shader "Hidden/GDShader/SSAOBlur" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Diffuse ("_Diffuse (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		
		pass {
			ZTest Always
			ZWrite Off
				//Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 invViewport_Radius;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata_img v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;
				return o;
			}
			const static float2 uv_offset[4] =
			{
				float2(-1.5,1.5),
				float2(-1.5,-0.5),
				float2(0.5,1.5),
				float2(0.5,-0.5)
			};
			float4 frag(v2f IN) : COLOR
			{
				float2 halfPixel = 0.5f*invViewport_Radius.xy;
				float2   uv = IN.uv;

				float4 ao = 0;
				for (int i = 0; i < 4; i++)
				{
					ao += tex2D(_MainTex, uv + uv_offset[i] * invViewport_Radius.xy);
				}

				return ao*0.25f;
			}
			ENDCG
		}

	pass {
			Stencil
			{
				Ref 4
				ReadMask 4
			
				Comp NotEqual
				Pass keep
			}
			ZTest Greater
			ZWrite Off
			ColorMask RGB
			Cull Off
			Blend One One

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "LightCommon.cg"
			#define SAMPLER_NUM 4

			


			sampler2D	_Diffuse;
			sampler2D   _AO;
			float4		invViewport_Radius;
			float4		_AmbientColor;
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				//float4 viewpos  : TEXCOORD1;
			};

			v2f vert(appdata_full v)
			{
				v2f o;
				#if SHADER_API_GLES|| SHADER_API_GLES3
					//Mobile
					o.pos = float4(v.vertex.x * 2,v.vertex.y * 2,1,1);
					o.uv = o.pos*0.5f + 0.5f;
					//o.viewpos = float4(o.pos.xy,1.0f,1)*_FarCorner;
				#else
					//Editor Only 
					o.pos = float4(v.vertex.x * 2,v.vertex.y * 2,1,1);
					o.uv = o.pos*0.5f + 0.5f;
					o.uv.y = 1 - o.uv.y;
					//o.viewpos = float4(o.pos.xy,1.0f,1)*_FarCorner;
					//o.viewpos.y *= -1;
				#endif

				return o;
			}

			float4 frag(v2f IN) : COLOR
			{
				//float4 micro = 0;
				//#if SHADER_API_GLES
				//micro.x = 1;
				//#endif
				//#if SHADER_API_GLES3
				//micro.y = 1;
				//#endif
				//#if SHADER_API_MOBILE
				//micro.z = 1;
				//#endif
				//return micro;
				float2 uv = IN.uv;
				float4 color = tex2D(_AO, uv);
				return color*_AmbientColor*tex2D(_Diffuse, uv);
			}
			ENDCG
		}
	} 
	//FallBack "Diffuse"
}
