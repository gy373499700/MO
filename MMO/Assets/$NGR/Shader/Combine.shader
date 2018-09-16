Shader "GDShader/Combine" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		pass{
			//Blend One One
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"


			sampler2D _MainTex;
			//float4 _Depth_ST;
			sampler2D _Diffuse;
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

			float4 ambientColor;
			float4 lightColor;
			
			float4 frag( v2f IN) : COLOR
			{
			   float2 halfPixel   =   0.5f*invViewport_Radius.xy;
			   float2   uv=   IN.uv+halfPixel;

			   float4 ao_shadow = tex2D(_MainTex,IN.uv);
			   float4 diff = tex2D(_Diffuse,IN.uv);
			   
			   return ao_shadow;//*diff;//pow(acos(-fAO)/3.14f,0.5f);///4;
			}
			ENDCG
		}
	} 
	//FallBack "Diffuse"
}
