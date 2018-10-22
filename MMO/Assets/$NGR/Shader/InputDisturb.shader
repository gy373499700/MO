Shader "GDShader/InputDisturb" {
	Properties{

		//_Depth ("Depth", 2D) = "white" {}
		_MainTex("_MainTex", 2D) = "white" {}
		_Color("_Color", Color) = (1,1,1,1)
		_RefractScale("_RefractScale", Range(0,1.0)) = 1.0
	}
		SubShader{
		Tags{ "Queue" = "Geometry+50" "DeferredLight" = "ScreenDisturbance" }
		LOD 200
		pass {
			ZTest	Always
			ZWrite	Off
			Cull Off
			ColorMask RGB
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
			//#define SHADER_API_GLES
#include "UnityCG.cginc"



		struct v2f {
			float4 pos : SV_POSITION;
		};  

		v2f vert(appdata_full v)
		{
			v2f o;

#if SHADER_API_GLES || SHADER_API_GLES3
			//Mobile
			o.pos = float4(v.vertex.x * 2-1, v.vertex.y * 2+1, 1, 1);//try it is right?
#else
			//Editor Only 
			o.pos = float4(v.vertex.x * 2-1, -v.vertex.y * 2+1, 1, 1);
#endif


			return o;
		}

	
		float4 frag(v2f i) : COLOR
		{
			return float4(1,1,1,1);
		}
			ENDCG
		}
		
		
		pass {
			ZTest	Always
				ZWrite	Off
				Cull Off
				ColorMask RGB
				CGPROGRAM
#pragma vertex vert
#pragma fragment frag
				//#define SHADER_API_GLES
#include "UnityCG.cginc"


			sampler2D	_ColorTex;
			sampler2D   _MainTex;
			float4		_MainTex_ST;
			float		_RefractScale;
			float		_PixelSize;
			sampler2D	_MaskTex;
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			//	float4 viewpos  : TEXCOORD1;
			};

			v2f vert(appdata_full v)
			{
				v2f o;

#if SHADER_API_GLES || SHADER_API_GLES3
				//Mobile
				o.pos = float4(v.vertex.x * 2, v.vertex.y * 2, 1, 1);
				o.uv = o.pos*0.5f + 0.5f;
#else
				//Editor Only 
				o.pos = float4(v.vertex.x * 2, v.vertex.y * 2, 1, 1);
				o.uv = o.pos*0.5f + 0.5f;
				o.uv.y = 1 - o.uv.y;

#endif
				return o;
			}

		
			float4 frag(v2f i) : COLOR
			{
				float4 ret;
				float2 uv = i.uv;
				float dis = _PixelSize;
			
				float4 mask1 = tex2D(_MaskTex, uv+float2(dis,0));
				float4 mask2 = tex2D(_MaskTex, uv + float2(0, dis));
				float4 mask3 = tex2D(_MaskTex, uv + float2(-dis, 0));
				float4 mask4 = tex2D(_MaskTex, uv + float2(0, -dis));
				float4 mask5 = tex2D(_MaskTex, uv + float2(dis *2, dis *2));
				float4 mask6 = tex2D(_MaskTex, uv + float2(-dis * 2,-dis * 2));
				float4 mask7 = tex2D(_MaskTex, uv + float2(dis * 2, -dis * 2));
				float4 mask8 = tex2D(_MaskTex, uv + float2(-dis * 2, dis * 2));//采样点矩阵决定了水波纹的大概形状
				float4 len = (mask1 + mask2 + mask3 + mask4+ mask5 + mask6 + mask7 + mask8) / 8;

				float4 normal_color = tex2D(_ColorTex, uv*len*_RefractScale*frac(_Time.y*2));
				normal_color.xy = normal_color.xy * 2 - 1;
				float4 color = tex2D(_MainTex, uv + (normal_color.xy)*len*0.1f);
				//float4 color = tex2D(_MainTex, uv + len);
				return float4(color.xyz, 1);
			/*
			float len = distance(i.uv, _UV);
			//clip((0.2f - len));
			len = max(0, (0.2f - len));
			float4 normal_color = tex2D(_ColorTex,uv*len*_RefractScale*frac(_Time.y / 2));
			normal_color.xy = normal_color.xy * 2 - 1;
			float4 color = tex2D(_MainTex, uv + (normal_color.xy)*len*0.1f);
			return float4(color.xyz   ,1); */
			}
				ENDCG
		}
	}
	

		//FallBack "Diffuse"
}
