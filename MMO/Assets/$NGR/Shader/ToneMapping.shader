Shader "Hidden/GDShader/ToneMapping" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		pass{
			ZTest Always
			ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			

			sampler2D	_MainTex;
			float4		tonemapping_param;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata_img v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;
				return o;
			}
			float4 frag( v2f IN) : COLOR
			{
			   float2   uv=   IN.uv;

				float3 x = tex2D(_MainTex,uv).rgb;
				float C = tonemapping_param.y;
				x *= tonemapping_param.x;  // Hardcoded Exposure Adjustment
				
				if (tonemapping_param.z != 1) 
				{
					float luminance = Luminance(x);
					float3 luminanceCol = float3(luminance, luminance, luminance);

					x = lerp(luminanceCol, x, tonemapping_param.z);//饱和度调整
				}
				if (tonemapping_param.w != 1)
				{
					float3 avcol = (0.5, 0.5, 0.5);
					x = lerp(avcol, x, tonemapping_param.w);//锐化度调整
				}
				float3 t1 = x*x*6.2;
				float3 t2 = x*C;
				float3 retColor = (t1+t2)/(t1+4.1*t2+0.05) + (0.634*C - 0.247)*x;
				return float4(retColor,1);
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
