// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'


Shader "GDShader/UIModelLight" {

	Properties{
		_Color("Diffuse Color", Color) = (1,1,1,1)
		_SpecColor("Specular Color", Color) = (1, 1, 1, 1)
		_Shininess("Shininess", Float) = 0.35
		_MainTex("Base (RGB)", 2D) = "white" {}	
		_SpecTex("SpecTex", 2D) = "white" {}
		_LightDirection("Light", Vector) = (1,1.6,-1.7,1)
	}

	SubShader{
		pass
		{
			Tags {"RenderType" = "Qpaque"}
				LOD 200
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			//#include "LightCommon.cg"
			//#include "Lighting.cginc"
			#pragma target 3.0
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;

			uniform float4 _LightColor0;

			sampler2D _MainTex;
			sampler2D _SpecTex;
			float4 _MainTex_ST;
			float _Fresnel;
			float4 _MainTint;

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float2 uv 		: TEXCOORD0;
				float3 normal : TEXCOORD1;
			};

			vertexOutput vert(appdata_full input)
			{
				vertexOutput output;
				
				float3 normalDirection = normalize(mul((float3x3)unity_ObjectToWorld, input.normal.xyz));

				output.pos = UnityObjectToClipPos(input.vertex);
				output.normal = normalDirection;
				output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);

				return output;
			}

			float3 Light(vertexOutput input, float3 lightDirection, float3 col)
			{
				float3 normalDirection = input.normal;

				float3 diffuseReflection = col*max(0.0, dot(normalDirection, lightDirection));

				float3 specularReflection;
				if (dot(normalDirection, lightDirection) < 0.0)
				{
					specularReflection = col*float3(0, 0, 0);
				}
				else
				{
					specularReflection = col*pow(max(0.0, dot(reflect(-lightDirection, normalDirection), float3(0, 0, -1))), _Shininess);
				}

				float3 light = specularReflection  + diffuseReflection + float3(1,1,1);

				return light;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				float3 light = Light(input, normalize(float3(-1, -1, -1)), float3(121 / 255.0,168.0 / 255.0,208.0 / 255.0))*0.2;
				light += Light(input, normalize(float3(1, 1, -1)), float3(1, 231.0 / 255.0, 210.0 / 255.0))*0.45;

				float4 c = pow(tex2D(_MainTex,input.uv),2.2)*_Color;
				return pow(c*float4(light, 1), 1/2.2)*1.5;
			}

				ENDCG
		}
	}
	FallBack "Diffuse"
}
