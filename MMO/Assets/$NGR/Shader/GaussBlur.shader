Shader "GDShader/GaussBlur"
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
			sampler2D _MaskTex;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			float _BlurSize;
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				float2 uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv = uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float gaussweight[3] = {0.4026,0.2442 / 2,0.0545 / 2};//模拟高斯公式
				float3 maskcol = tex2D(_MaskTex, i.uv);
				float3 col = tex2D(_MainTex, i.uv).rgb;

				/*if ((maskcol.r + maskcol.g + maskcol.b) < 1.5f)
					return float4(col, 1);
				if ((col.r + col.g + col.b) < 0.1f)
					return float4(col, 1);
				if(Luminance(col)<0.1f)
					return float4(col, 1);*/
				// sample the texture
				col = col.rgb*gaussweight[0];
				col += tex2D(_MainTex, i.uv + _BlurSize*float2(0, _MainTex_TexelSize.y)).rgb*gaussweight[1];
				col += tex2D(_MainTex, i.uv - _BlurSize*float2(0, _MainTex_TexelSize.y)).rgb*gaussweight[1];
				col += tex2D(_MainTex, i.uv + _BlurSize*float2(_MainTex_TexelSize.x, 0)).rgb*gaussweight[1];
				col += tex2D(_MainTex, i.uv - _BlurSize*float2(_MainTex_TexelSize.x, 0)).rgb*gaussweight[1];
				col += tex2D(_MainTex, i.uv + _BlurSize*float2(0, _MainTex_TexelSize.y * 2)).rgb*gaussweight[2];
				col += tex2D(_MainTex, i.uv - _BlurSize*float2(0, _MainTex_TexelSize.y * 2)).rgb*gaussweight[2];
				col += tex2D(_MainTex, i.uv + _BlurSize*float2(_MainTex_TexelSize.x * 2, 0)).rgb*gaussweight[2];
				col += tex2D(_MainTex, i.uv - _BlurSize*float2(_MainTex_TexelSize.x * 2, 0)).rgb*gaussweight[2];
				return float4(col,1);
			}
			fixed4 fragVertical(v2f i) : SV_Target
			{//计算bloom的 先算亮度  亮度过低的点不参与计算
				float gaussweight[3] = { 0.4026,0.2442 ,0.0545 };
				float3 maskcol = tex2D(_MaskTex, i.uv);
				float3 col = tex2D(_MainTex, i.uv).rgb;
				//if ((maskcol.r + maskcol.g + maskcol.b) < 0.1f)
				//	return float4(col, 1);
				//这个过滤会大幅降低模糊周围的效果
				//if (Luminance(col)<0.01f)
				//	return float4(col, 1);
				col = col.rgb*gaussweight[0];
				col += tex2D(_MainTex, i.uv + _BlurSize*float2(0, _MainTex_TexelSize.y)).rgb*gaussweight[1];
				col += tex2D(_MainTex, i.uv - _BlurSize*float2(0, _MainTex_TexelSize.y)).rgb*gaussweight[1];
				col += tex2D(_MainTex, i.uv + _BlurSize*float2(0, _MainTex_TexelSize.y * 2)).rgb*gaussweight[2];
				col += tex2D(_MainTex, i.uv - _BlurSize*float2(0, _MainTex_TexelSize.y * 2)).rgb*gaussweight[2];
				return float4(col,1);
			}
			fixed4 fragHorizontal(v2f i) : SV_Target
			{
				float gaussweight[3] = { 0.4026,0.2442 ,0.0545 };
				float3 maskcol = tex2D(_MaskTex, i.uv);
				float3 col = tex2D(_MainTex, i.uv).rgb;

				col += tex2D(_MainTex, i.uv + _BlurSize*float2(_MainTex_TexelSize.x, 0)).rgb*gaussweight[1];
				col += tex2D(_MainTex, i.uv - _BlurSize*float2(_MainTex_TexelSize.x, 0)).rgb*gaussweight[1];
				col += tex2D(_MainTex, i.uv + _BlurSize*float2(_MainTex_TexelSize.x * 2, 0)).rgb*gaussweight[2];
				col += tex2D(_MainTex, i.uv - _BlurSize*float2(_MainTex_TexelSize.x * 2, 0)).rgb*gaussweight[2];
				return float4(col,1);
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
				#pragma vertex vert
				#pragma fragment fragVertical
				#include "UnityCG.cginc"
				ENDCG
			}
			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment fragHorizontal
				#include "UnityCG.cginc"
				ENDCG
			}
	}

}


