Shader "GDShader/Bloom"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100
		CGINCLUDE
		#include "UnityCG.cginc"
		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float2 uv : TEXCOORD0;
			float4 vertex : SV_POSITION;
		};

		sampler2D _MainTex;
		float4 _MainTex_ST;
		float4 _MainTex_TexelSize;
		float _Luminance;
		sampler2D _BloomTex;
		float _Intensity;
		v2f vert(appdata v)
		{ 
			v2f o;
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			return o;
		}

		fixed4 frag(v2f i) : SV_Target  
		{
			// sample the texture
			fixed4 col = tex2D(_MainTex, i.uv);
			//float light = Luminance(col);
			//float c = clamp(light - _Luminance, 0, 1);
			//return c*col;
			return max(col- _Luminance,0)*_Intensity;

		}
		v2f vertBloom(appdata v)
		{
			v2f o;
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			return o;
		}

		fixed4 fragBloom(v2f i) : SV_Target
		{
			// sample the texture
			float4 col = tex2D(_MainTex, i.uv) +tex2D(_BloomTex, i.uv);
			return col;

		}

		ENDCG
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			ENDCG
		}  
		Pass   
		{
			CGPROGRAM
			#pragma vertex vertBloom
			#pragma fragment fragBloom
			#include "UnityCG.cginc"
			ENDCG
		}    
		//UsePass "GDShader/GaussBlur/GaussBlur"
	}
	Fallback off
}
