Shader "Hidden/GDShader/ActorWhiting" {

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		pass{
			Stencil
			{
				Ref 2
				ReadMask 2
				Comp Equal
				Pass keep
			}
			ZTest	Always
			ZWrite Off
			Cull Off
			ColorMask RGB
				Blend One One
				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				float4 _Color;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata_full v)
			{
				v2f o;
				#if SHADER_API_GLES|| SHADER_API_GLES3
				//Mobile
				o.pos = float4(v.vertex.x*2,v.vertex.y*2,1,1);
				o.uv = o.pos*0.5f+0.5f;
				#else
				//Editor Only 
				o.pos = float4(v.vertex.x*2,v.vertex.y*2,1,1);
				o.uv = o.pos*0.5f+0.5f;
				o.uv.y = 1-o.uv.y;
				#endif

				
				return o;
			}

			float4 frag( v2f IN) : COLOR
			{
				return _Color;
			}
			ENDCG
		}
	} 
	//FallBack "Diffuse"
}
