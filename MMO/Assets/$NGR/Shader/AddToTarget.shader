Shader "GDShader/AddToTarget" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Diffuse ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		pass{
			ZWrite Off
			Blend One One
			ColorMask RGB
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _Diffuse;
			sampler2D _AO;

			float4 invViewport_Radius;

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
			   float2 halfPixel   =   invViewport_Radius.xy;
			   float2   uv=   IN.uv;//+halfPixel;

			   float4 color = tex2D(_MainTex,uv+halfPixel);
				float4 diffuse = tex2D(_Diffuse	 ,uv);
				float4 ao_val = tex2D(_AO,uv);

				float metal = diffuse.w >0.497f;
			   float inRoughness = 1-diffuse.w*2 + metal;
			   float fn = inRoughness*inRoughness;
				float f0   =   (1-fn)/(1+fn);
				f0*=f0;
	
			   return (saturate(1-f0)+0.04)*color*invViewport_Radius.z*diffuse*ao_val.w;//*2;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
