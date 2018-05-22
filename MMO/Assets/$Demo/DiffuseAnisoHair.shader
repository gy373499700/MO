Shader "Unlit/DiffuseAnisoHair"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white" {}
		_Color("_Color", Color) = (0.5,0.5,0.5,0.5)
		_SmoothBase("_SmoothBase",Range(0,1.0)) = 0.0
		_Metal("_Metal",Range(0,1.0)) = 0.0
		StencilRef("StencilRef",Int) = 0
		//CullMode("CullMode",Int) = 2
		_PainterTex("_PainterTex", 2D) = "black" {}
		_PaintColor0("_PaintColor0", Color) = (0.5,0.5,0.5,0.5)
		_PaintColor1("_PaintColor1", Color) = (0.5,0.5,0.5,0.5)
		_UVMove("_UVMove", Vector) = (0,0,0,0)
		
		_AnisoSpecTex("_AnisoSpecTex", 2D) = "gray"{}  
		_AnisoSpecColor("_AnisoSpecColor", Color) = (1,1,1,1)  
		_AnisoSpecularMultiplier("_AnisoSpecularMultiplier", float) = 1.0
		_AnisoPrimaryShift("_AnisoPrimaryShift", float) = 0.5 
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM

			#pragma vertex aniso_vert
			#pragma fragment aniso_frag
			#include "UnityCG.cginc"


#include "UnityCG.cginc"
#include "AutoLight.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _AnisoSpecTex;
			float4 _AnisoSpecColor;
			float _AnisoSpecularMultiplier;
			float _AnisoPrimaryShift;
			float4 lightdir;
			float4 lightcolor;


		struct v2f {
			float4 pos				: SV_POSITION;
			float2 uv				: TEXCOORD0;
			float3 tangent_input	: TEXCOORD1;
			float3 viewDir			: TEXCOORD2;
			float3 normal			: TEXCOORD3;
		
		};

		half3 ShiftTangent(half3 T, half3 N, float shift)
		{
			half3 shiftedT = T + shift * N;
			return normalize(shiftedT);
		}

		float StrandSpecular(half3 T, half3 V, half3 L, float exponent)
		{
			half3 H = normalize(L + V);
			float dotTH = dot(T, H);
			float sinTH = sqrt(1 - dotTH * dotTH);
			float dirAtten = smoothstep(-1, 0, dotTH);
			return dirAtten * pow(sinTH, exponent);
		}

		inline fixed4 LightingHair(fixed3 lightDir, fixed3 viewDir, fixed atten,
			float3 normal, fixed SpecShift, float3 tangent_input,
			float alpha, fixed Specular, fixed SpecMask)
		{
			float4 c;
			c.a = alpha;
			float NdotL = saturate(dot(normal, lightDir));
			float shiftTex = SpecShift - 0.5;
			half3 T = -normalize(cross(normal, tangent_input));
			half3 t1 = ShiftTangent(T, normal, _AnisoPrimaryShift + shiftTex);
			half3 spec = _AnisoSpecColor * StrandSpecular(t1, viewDir, lightDir, _AnisoSpecularMultiplier) * (1 - SpecMask);

			c.rgb = spec * atten * 2 * lightcolor * NdotL;
			return c;
		}

		v2f aniso_vert(appdata_full v)
		{
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.tangent_input = v.tangent.xyz;
			o.viewDir = normalize(mul((float3x3)unity_ObjectToWorld, ObjSpaceViewDir(v.vertex)));
	
			o.normal = mul((float3x3)unity_ObjectToWorld,normalize(v.normal));
			return o;
		}

		float4 aniso_frag(v2f i) : COLOR0
		{
			float3 lightDirection = normalize(float3(lightdir.xyz));
			fixed4 specTex = tex2D(_AnisoSpecTex, i.uv);
			fixed atten = 1;
			//return float4(i.viewDir.xyz, 1);
			//return float4(i.tangent_input.xyz, 1);
			//return float4(specTex.g,specTex.g,specTex.g,specTex.g);
			//return float4(normalize(i.normal.xyz),1);
			//return float4(lightDirection.xyz,1);
			return LightingHair(lightDirection, i.viewDir, atten, i.normal
				, specTex.g, i.tangent_input, specTex.a
				, specTex.r, specTex.b);
		}
			ENDCG
		}
	}
}
