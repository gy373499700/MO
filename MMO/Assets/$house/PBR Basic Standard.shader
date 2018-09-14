Shader "TOPX/Scene/PBR Basic Standard" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Base Color (RGB)", 2D) = "white" {}
	    _BumpMap("Normal Map", 2D) = "bump" {}
		_MetallicGlossMap ("Mix Map (R: Roughness, G: Metallic, B: AO)", 2D) = "gray" {}
		_EmissionMap ("Emission Map", 2D) = "black" {}
		[HDR]_EmissionColor ("Emission Color", Color) = (1,1,1,1)			
	}
	
	SubShader {
		Tags { "RenderType" = "Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard exclude_path:deferred exclude_path:prepass finalcolor:final fullforwardshadows vertex:vert nolppv
		#pragma target 3.0
		#include "HeightFogCommon.cginc"

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _MetallicGlossMap;
		sampler2D _EmissionMap;
		fixed4 _EmissionColor;
		fixed4 _Color;

		struct Input {
			float2 uv_MainTex;
			float4 fogCoord;
		};

		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			TOPX_TRANSFER_FOG(o, worldPos);
		}
		
		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 normalTex = tex2D(_BumpMap, IN.uv_MainTex);
			fixed3 mixMap = tex2D(_MetallicGlossMap, IN.uv_MainTex).rgb;
			fixed3 bump = UnpackNormal(normalTex);
			fixed3 emission = tex2D (_EmissionMap, IN.uv_MainTex).rgb * _EmissionColor.rgb;
			o.Albedo = c.rgb;
			o.Metallic = mixMap.g;
			o.Smoothness = 1.0 - mixMap.r;
			o.Occlusion = mixMap.b;
			o.Emission = emission;
			o.Normal = bump;
			o.Alpha = c.a;
		}

		void final(Input IN, SurfaceOutputStandard o, inout fixed4 color) {
			TOPX_APPLY_FOG(IN, color);
			TOPX_MOD_COLOR(color);
        }

		ENDCG
	}
	FallBack "Diffuse"
}
