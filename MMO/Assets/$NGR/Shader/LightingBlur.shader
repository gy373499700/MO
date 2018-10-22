// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GDShader/LightingBlur" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Diffuse ("Diffuse (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		pass{
			//ColorMask RGB
			//Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _Diffuse;

			float4 invViewport;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata_img v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			static float2 offset[4]={

			float2(-0.5,0.5),
			float2(-0.5,-0.5),
			float2(0.5,0.5),
			float2(0.5,-0.5),
			};

			float4 frag( v2f IN) : COLOR
			{
			   float2 twoPixel   =  0.5f*invViewport.xy;
			   float2   uv=   IN.uv;

			   float4 diff = tex2D(_Diffuse,IN.uv);

			   float4 color = 0;
			   for(int i=0;i<4;i++)
			   {
					float2 uv_temp	=	IN.uv+twoPixel*offset[i];
					float4 temp	= tex2D(_MainTex,uv_temp);
					color += temp;
			   }
			   return diff*color*0.25;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
