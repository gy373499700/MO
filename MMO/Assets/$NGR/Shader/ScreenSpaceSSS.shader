Shader "Hidden/GDShader/ScreenSpaceSSS" {
	Properties {
		//_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		pass{

			
			Stencil
			{
				Ref 16
				ReadMask 16
				Comp equal
				Pass keep
			}

			ZTest Less
			ZWrite Off
			ColorMask RGB
			//Cull Front
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "LightCommon.cg"
			#include "UnityCG.cginc"

			#define SAMPLE_NUM 16
			
			sampler2D	_MainTex;
			sampler2D	_FinalDiffuse;
			sampler2D   _Diffuse;
			float4		_FarCorner;
			float4		LightDir;
			float4		LightColor;
			
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 viewpos  : TEXCOORD1;
			};

			v2f vert (appdata_full v)
			{
				v2f o;

				#if defined(SHADER_API_MOBILE)
				//Mobile
				o.pos = float4(v.vertex.x*2,v.vertex.y*2,0,1);
				o.uv = o.pos*0.5f+0.5f;
				o.viewpos = float4(o.pos.xy,1.0f,1)*_FarCorner;
				#else
				//Editor Only 
				o.pos = float4(v.vertex.x*2,-v.vertex.y*2,0,1);
				o.uv = o.pos*0.5f+0.5f;
				o.uv.y = 1-o.uv.y;
				o.viewpos = float4(o.pos.xy,1.0f,1)*_FarCorner;
				o.viewpos.y*=-1;
				#endif


				return o;
			}
			
			
			float4 frag(v2f i) : COLOR
			{
			//return float4(1,0,0,0);
				float4 ret;

				float2 uv = i.uv;

				//float4 _random   =  tex2D(_Sample2x2,i.uv/256.0f/invViewport.xy);
				//float step = _random.w;
				//_random.xyz = _random.xyz*2-1;

				float4 depth_normal = tex2D(_MainTex,uv);
				
				float2 XY_Depth	=	float2(1.0f,0.003921568627451);
				float depth = dot(XY_Depth,depth_normal.xy);
				float3 normal = DecodeNormal(depth_normal.zw);
				float3 view_pos = i.viewpos*depth;
				float3 L = -LightDir.xyz;
				//view_pos.y*=-1;

				float4 diff = tex2D(_Diffuse, uv);
				float4 sss = tex2D(_FinalDiffuse, uv);

				float current_thickness = (1-sss.w);
				float max_thickness = frac(diff.w*2.0f);
				float shadow = 1 - saturate(current_thickness / max_thickness);
				float4 ret_color = (max(0, dot(normal, L))*0.5+0.5f)* pow(shadow, 4)*LightColor*diff;
				return float4(ret_color.xyz, max_thickness);// *diff;// / (3.5 + 100 * pow(diff.r - 0.33, 4));
				//return  shadow*shadow*(max(0, dot(normal, L))*(1- max_thickness) + max_thickness)*diff;
				
				
				
			}
			ENDCG
		}
	} 
	//FallBack "Diffuse"
}
