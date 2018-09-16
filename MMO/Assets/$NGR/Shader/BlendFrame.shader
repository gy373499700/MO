Shader "GDShader/BlendFrame" {
	Properties {
		//_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		pass{
			//Blend One One
			ZTest Always
			ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _LastFrame;
			sampler2D _CurrentFrame;
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

			   float2   uv=   IN.uv;

			   float4 color0 = tex2D(_LastFrame,uv);
			   float4 color1 = tex2D(_CurrentFrame,uv);
	
			   
			   return (color0+color1)*0.5f;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
