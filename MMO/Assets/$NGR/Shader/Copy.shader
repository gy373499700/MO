// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GDShader/Copy" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		pass{
			ZTest Always
			ZWrite Off
			//Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
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
			float4 frag( v2f IN) : COLOR
			{
			   float2 halfPixel   =   0.5f*invViewport_Radius.xy;
			   float2   uv=   IN.uv;

			   float4 color = pow(tex2D(_MainTex,uv),1.0f/invViewport_Radius.z);
	
			   
			   return color;
			}
			ENDCG
		}
		pass {
			ZTest Always
			ZWrite Off
			ColorMask RGB
			//Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 invViewport_Radius;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata_img v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}
			float4 frag(v2f IN) : COLOR
			{

				float2   uv = IN.uv;

				float4 color = pow(tex2D(_MainTex,uv),1.0f / invViewport_Radius.z);


				return color;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
