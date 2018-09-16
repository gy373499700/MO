Shader "Hidden/GDShader/RadialBlur" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		pass {
			ZTest Always
			ZWrite Off
			Cull Off
			ColorMask RGB
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			#define BLUR_NUMBER 10
			sampler2D	_MainTex;
			sampler2D   _Kernel2x2;
			float4		blur_param;
			float4		invViewport;
			   
			struct v2f {
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
			};

			v2f vert(appdata_img v)
			{
				v2f o;
				float2 center = blur_param.xy;
#if SHADER_API_GLES|| SHADER_API_GLES3

#else
				center.y *= -1;
#endif
				float w_h = invViewport.x / invViewport.y;
				o.pos = float4(v.vertex.xy*blur_param.z*float2(w_h,1)+ center,1,1);
				float blur_len = 1-saturate(abs(0.5f - length(v.vertex.xy))*2.0f);
				
				o.uv = float4(o.pos.xy*0.5f+0.5f, v.vertex.xy*blur_param.w*blur_len*invViewport.xy*0.5f);
#if SHADER_API_GLES|| SHADER_API_GLES3

#else
				o.uv.y = 1 - o.uv.y;
				o.uv.w *= -1;
#endif
				return o;
			}
			float4 frag(v2f IN) : COLOR
			{
				float4   uv = IN.uv;

				float4   step = tex2D(_Kernel2x2, uv.xy*0.5f / invViewport.xy);

				float2 baseuv = uv.xy - uv.zw*(BLUR_NUMBER*0.5f+ step.x);
				float4	ret = 0;
				for (int i = 0; i < BLUR_NUMBER; i++)
				{
					ret += tex2D(_MainTex, baseuv + uv.zw*i);
				}

				return  ret / BLUR_NUMBER;
			}
				ENDCG
		}
	}
		FallBack "Diffuse"
}

