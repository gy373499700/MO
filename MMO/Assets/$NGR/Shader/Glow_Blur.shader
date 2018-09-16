Shader "GDShader/Glow_Blur" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		pass {
			ColorMask RGB
			//Blend One One
			ZTest Less
			ZWrite Off
			Cull	Off
			CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
		#include "Quad.cg"
		
				sampler2D _MainTex;
				float4 invViewport;
				float4 frag(v2f IN) : COLOR
				{
					float4 c = tex2D(_MainTex,IN.uv);
					return c*c.a;
				}
					ENDCG
			}
		pass{
			ColorMask RGB
			//Blend One One
			ZTest Off
			ZWrite Off
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			
			#include "Quad.cg"
			sampler2D _MainTex;
			float4 invViewport;
			
			const static float2 samples[6] = {
			   float2(-5, -5),
			   float2(-3, -3),
			   float2(-1, -1),
			   float2(1, 1),
			   float2(3, 3),
			   float2(5, 4)
			};

			float4 frag( v2f IN) : COLOR
			{
			   float2   uv=   IN.uv;

			   float2 half_offset = 0.5f*invViewport.xy;

			   float4 ret = 0;
			   float weight = 0;
			   for(int i=0;i<6;i++)
			   {
					float2 uv_offset =	samples[i];
					float2 uv2 = uv_offset*invViewport.xy*invViewport.zw;// *invViewport.z;
					float4 color = tex2D(_MainTex,IN.uv+uv2);
					ret	+=	color;//*uv_offset.y;
					//weight;//+=uv_offset.y;
			   }
			   return ret/6;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
