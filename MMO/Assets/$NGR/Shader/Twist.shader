Shader "GDShader/Twist"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			
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
			float _Twist;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = float2(i.uv.x - 0.5f,i.uv.y - 0.5f);
				float angle = _Twist*0.1745f / (length(uv) + 0.5f);
				float sinv, cosv;
				sincos(angle, sinv, cosv);
				float2x2 mat = float2x2(cosv, -sinv, sinv, cosv);
				uv = mul(mat, uv) + 0.5f;
				fixed4 col = tex2D(_MainTex, uv); 
				return col;
			}
			ENDCG
		}
		Pass
			{
				CGPROGRAM
#pragma vertex vert
#pragma fragment frag


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
			float _Twist;
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
#if SHADER_API_GLES|| SHADER_API_GLES3
				//Mobile
#else
				//Editor Only 
				o.uv.y = 1 - o.uv.y;

#endif
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv;
				uv.x = sin(uv.x + _Twist);
				//uv=float2(frac(uv.x*10),uv.y );
				//uv.x = sin(uv.x  + _Twist);
				uv.y = sin(uv.y*10+ _Twist);//raid

				fixed4 col = tex2D(_MainTex,uv);

				return col;
			}
				ENDCG
			}
	}
}
