Shader "GDShader/Glow_DownSample" {
	Properties {
		_FinalTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		pass{
			ColorMask RGB
			//Blend One One
			ZTest Less
			ZWrite Off
			Cull	Off
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _FinalTex;
			float4 invViewport;
			sampler2D _DepthTex;
			float4 _FarCorner;
			float		_ZDepth;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata_full v)
			{
				v2f o;
				#if SHADER_API_GLES || SHADER_API_GLES3
				//Mobile
				o.pos = float4(v.vertex.x*2,v.vertex.y*2, _ZDepth,1);
				o.uv = o.pos*0.5f+0.5f;
				#else
				//Editor Only 
				o.pos = float4(v.vertex.x*2,v.vertex.y*2, _ZDepth,1);
				o.uv = o.pos*0.5f+0.5f;
				o.uv.y = 1-o.uv.y;
				#endif

				
				return o;
			}
			float4 frag( v2f IN) : COLOR
			{
			   return tex2D(_FinalTex,IN.uv);
			}
			ENDCG
		}
		pass{
			//ColorMask RGB
			//Blend One One
			ZTest Less
			ZWrite Off
			Cull	Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _FinalTex;
			float4 invViewport;
			sampler2D _DepthTex;
			float4 _FarCorner;
			float		_ZDepth;
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata_full v)
			{
				v2f o;
				#if SHADER_API_GLES|| SHADER_API_GLES3
				//Mobile
				o.pos = float4(v.vertex.x*2,v.vertex.y*2, _ZDepth,1);
				o.uv = o.pos*0.5f+0.5f;
				#else
				//Editor Only 
				o.pos = float4(v.vertex.x*2,v.vertex.y*2, _ZDepth,1);
				o.uv = o.pos*0.5f+0.5f;
				o.uv.y = 1-o.uv.y;
				#endif

				
				return o;
			}

			const static float2 sampleuv[8]=
			{
				float2(0,1),
				//float2(0.4067367,0.9135454),
				float2(0.7431448,0.6691306),
				//float2(0.9510565,0.309017),
				float2(0.9945219,-0.1045285),
				//float2(0.8660254,-0.5000001),
				float2(0.5877852,-0.8090171),
				//float2(0.2079116,-0.9781476),
				float2(-0.2079118,-0.9781476),
				//float2(-0.5877853,-0.8090168),
				float2(-0.8660255,-0.5),
				//float2(-0.994522,-0.1045284),
				float2(-0.9510565,0.3090172),
				//float2(-0.7431448,0.6691307),
				float2(-0.4067365,0.9135455)
			};

			float4 frag( v2f IN) : COLOR
			{
			   float2   uv=   IN.uv;

			   float4 depth_normal = tex2D(_DepthTex,IN.uv);
			   float2 XY_Depth	=	float2(1.0f,0.003921568627451);
			   float depth = dot(XY_Depth,depth_normal.xy);
			   float z = _FarCorner.z*depth;
			   float blur =	min(8,max(0, invViewport.z*(z-_FarCorner.w)/30.0f));
			   //return blur;//
			   float4 color =0;

			   for(int i=0;i<8;i++)
			   {
					float4 c = tex2D(_FinalTex,IN.uv+sampleuv[i]*invViewport.xy*blur);
					color+=c;
					//glowcolor+=c*c.w;
			   }
			   
			   return float4(color.xyz/8.0f, depth>0.999f);
			   //return 1;
			}
			ENDCG
		}

		pass {
			//ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest Less
				ZWrite Off
				Cull	Off
				CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

			sampler2D _FinalTex;
			float4 invViewport;
			sampler2D _DepthTex;
			float4 _FarCorner;
			float		_ZDepth;
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata_full v)
			{
				v2f o;
#if SHADER_API_GLES|| SHADER_API_GLES3
				//Mobile
				o.pos = float4(v.vertex.x * 2, v.vertex.y * 2, _ZDepth, 1);
				o.uv = o.pos*0.5f + 0.5f;
#else
				//Editor Only 
				o.pos = float4(v.vertex.x * 2, v.vertex.y * 2, _ZDepth, 1);
				o.uv = o.pos*0.5f + 0.5f;
				o.uv.y = 1 - o.uv.y;
#endif


				return o;
			}

			const static float2 sampleuv[8] =
			{
				float2(0,1),
				float2(0.7431448,0.6691306),
				float2(0.9945219,-0.1045285),
				float2(0.5877852,-0.8090171),
				float2(-0.2079118,-0.9781476),
				float2(-0.8660255,-0.5),
				float2(-0.9510565,0.3090172),
				float2(-0.4067365,0.9135455)
			};

			float4 frag(v2f IN) : COLOR
			{
				float2   uv = IN.uv;


				float blur = 4.0f;
				//return blur;//
				float4 ret = 0;

				for (int i = 0; i<8; i++)
				{
					ret += tex2D(_FinalTex,IN.uv + sampleuv[i] * invViewport.xy*blur);
				}
				ret *=0.125f;
				float4 c = tex2D(_FinalTex, IN.uv);
				return float4(c.xyz, 1 - ret.w); 
				//return 1;
			}
				ENDCG
		}
	} 
	FallBack "Diffuse"
}
