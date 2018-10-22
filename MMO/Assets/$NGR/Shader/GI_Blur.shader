// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GDShader/GI_Blur" {
	Properties {
		//_MainTex ("Base (RGB)", 2D) = "white" {}
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

			sampler2D _Sample2x2;

			float4 invViewport_Radius;

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
			float2(3.5,0.5),
			float2(3.5,-3.5),
			float2(-0.5,-3.5),
			};

			float4 frag( v2f IN) : COLOR
			{
			   //float2 twoPixel   =   2*invViewport_Radius.xy;
			   float2   uv=   IN.uv+ invViewport_Radius.xy*0.5f;

			   float4 blur_radius = tex2D(_MainTex,IN.uv);

			   float radius = 1.0f;//(1-blur_radius)*0.9+0.1f;//*0.9+0.1f ;
			   //float4 diff = tex2D(_Diffuse,IN.uv)+0.1f;

			   float4   rot   =  tex2D(_Sample2x2,IN.uv*0.5f/invViewport_Radius.xy)*2-1;
			   float2x2 rot_2x2 = float2x2(rot.xyzw);
			   float4 color = 0;
			   for(int i=0;i<4;i++)
			   {
					float2 uv_temp	=	mul(rot_2x2,offset[i])*invViewport_Radius+ IN.uv;
					float4 temp	= tex2D(_MainTex,uv_temp);
					color += temp;
			   }
			   return color*0.25;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
