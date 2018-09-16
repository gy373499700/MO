Shader "GDShader/Glow_BlendToTarget" {
	Properties {
		_GlowTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		pass{
			ColorMask RGB
			//Blend SrcAlpha OneMinusSrcAlpha
			//Blend One One
			ZTest Less
			ZWrite Off
			Cull	Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _GlowTex;
			float4 invViewport;
			float4 MainLightColor;
			float		_ZDepth;
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata_full v)
			{
				v2f o;
				#if SHADER_API_GLES|| SHADER_API_GLES3
				//Mobile
				o.pos = float4(v.vertex.x*2,v.vertex.y*2, _ZDepth,1);
				o.uv = o.pos*0.5f+0.5f;
				#else
				//Editor Only 
				o.pos = float4(v.vertex.x*2,v.vertex.y*2, _ZDepth,1);
				o.uv = o.pos*0.5f+0.5f;
				o.uv.y = 1-o.uv.y;
				#endif

				
				return o;
			}
			const static float2 sampleuv[7]
			=
			{
				float2(0,1),
				float2(0.7818315,0.6234897),
				float2(0.9749279,-0.2225209),
				float2(0.4338838,-0.9009688),
				float2(-0.4338838,-0.9009688),
				float2(-0.9749279,-0.2225209),
				float2(-0.7818316,0.6234897)
			};
			float4 frag( v2f IN) : COLOR
			{
			    float2   uv=   IN.uv;
			    
			    float4 origin = tex2D(_GlowTex,IN.uv);
			    
			    float4 color = 0;
			    for(int i=0;i<7;i++)
			    {
				
					color+=tex2D(_GlowTex,IN.uv+sampleuv[i]*invViewport.xy*invViewport.z*origin.w);
			    }
				float4 ret = color/7;
			    return ret;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
