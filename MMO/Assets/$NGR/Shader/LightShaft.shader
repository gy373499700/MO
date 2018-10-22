// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GDShader/LightShaft" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		pass{
			ZTest Off
			ZWrite Off
			//ColorMask A
			//Blend One One
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _Sample2x2;
			sampler2D _SunTex;
			float4 invViewport;
			float4 MainLight_UV;
			float _SunSize;
			#define SAMPLE_NUM 16

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
			   float2   uv=   IN.uv;

			   float   step =   tex2D(_Sample2x2, IN.uv*0.5f / invViewport.xy).x;
			     
			   float2 dir = MainLight_UV.xy - uv;
		
			   float dir_len = length(dir);

			   float2 xy = float2(1.0f, invViewport.y / invViewport.x);

			   float4 color = 0;
			   float2 stepoffset = dir/SAMPLE_NUM;//*length(invViewport.xy)*invViewport.z;
			   float2 baseuv = uv;// +stepoffset*step;
			   


		        float2 tempuv = baseuv;// +stepoffset*i;
		  		float4 c = tex2D(_MainTex,tempuv);
		  		float len = length((MainLight_UV.xy - tempuv) /xy);
		  		color += (c.r > 0.999f)/ len;
			   
				float4 colorsun = float4(0, 0, 0, 0);
				if (_SunSize > 0) 
				{
					float2 scale = 1 / invViewport.xy / 512 * _SunSize;
					float2 uvscale = uv*scale;
					float2 centeroffset = 0.5f - MainLight_UV.xy*scale;
					 colorsun = tex2D(_SunTex, uvscale + centeroffset);
				}
				//return colorsun.a*0.3;
		
				float sun = MainLight_UV.z*color / SAMPLE_NUM*0.1f;
				if (sun.r > 0.01)
					return sun +colorsun.a*0.5;
				else
					return sun;
			}
			ENDCG
		}

		pass{

			ZTest Less
			ZWrite Off
			ColorMask RGB
			Cull Off
			Blend One One
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _Sample2x2;
			float4 invViewport;
			float4 MainLight_UV;
			float4 MainLightColor;
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata_full v)
			{
				v2f o;
				#if SHADER_API_GLES|| SHADER_API_GLES3
				//Mobile
				o.pos = float4(v.vertex.x*2,v.vertex.y*2,0,1);
				o.uv = o.pos*0.5f+0.5f;
				#else
				//Editor Only 
				o.pos = float4(v.vertex.x*2,v.vertex.y*2,0,1);
				o.uv = o.pos*0.5f+0.5f;
				o.uv.y = 1-o.uv.y;
				#endif

				
				return o;
			}

			float4 frag( v2f IN) : COLOR
			{

			   float2   uv=   IN.uv;
			   float   step =   tex2D(_Sample2x2, IN.uv*0.5f / invViewport.xy).x;

			   float2 dir = (MainLight_UV.xy - uv);

			   float4 color = 0;
			   float2 stepoffset = dir/8;//*length(invViewport.xy)*invViewport.z;
			   float2 baseuv = uv +stepoffset*step;
			   float4 base = tex2D(_MainTex, baseuv);
			   for(int i=0;i<8;i++)
			   {
					float4 c = tex2D(_MainTex,baseuv+stepoffset*i);;
					color	+=	c;
			   }
			   return MainLightColor*(color*0.125f);// *invViewport.w;
			}
			ENDCG
		}
		pass {

			ZTest Always
				ZWrite Off
				ColorMask RGB
				Cull Off
				Blend One One
				CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

				sampler2D _MainTex;
			sampler2D _Sample2x2;
			float4 invViewport;
			float4 MainLight_UV;
			float4 MainLightColor;
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata_full v)
			{
				v2f o;
#if SHADER_API_GLES|| SHADER_API_GLES3
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

			float4 frag(v2f IN) : COLOR
			{

				float2   uv = IN.uv;
				float   step = tex2D(_Sample2x2,IN.uv*0.5f / invViewport.xy).x;

				float2 dir = (MainLight_UV.xy - uv);

				float4 color = 0;
				float2 stepoffset = dir / 8;//*length(invViewport.xy)*invViewport.z;
				float2 baseuv = uv + stepoffset*step;
				float4 base = tex2D(_MainTex, baseuv);

				return MainLightColor*( base);// *invViewport.w;
			}
				ENDCG
		}


		pass {
				ZTest Less
				ZWrite Off
				ColorMask RGB
				Cull Off
				Blend One One
				CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

				sampler2D _MainTex;

			//sampler2D _SunTex;
			float4 invViewport;
			float4 MainLight_UV;
			float4 MainLightColor;
			float _SunSize;
#define SAMPLE_NUM 16

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata_img v)
			{
				v2f o;
#if SHADER_API_GLES|| SHADER_API_GLES3
				//Mobile
				o.pos = float4(v.vertex.x * 2, v.vertex.y * 2, 0.99, 1);
				o.uv = o.pos*0.5f + 0.5f;
#else
				//Editor Only 
				o.pos = float4(v.vertex.x * 2, v.vertex.y * 2, 0.99, 1);
				o.uv = o.pos*0.5f + 0.5f;
				o.uv.y = 1 - o.uv.y;
#endif


				return o;  
			}

			float4 frag(v2f IN) : COLOR
			{
				float2   uv = IN.uv;

		

				float2 dir = MainLight_UV.xy - uv;
				float dir_len = length(dir);

				float2 xy = float2(1.0f, invViewport.y / invViewport.x);



				float2 baseuv = uv;// +stepoffset*step;



				float2 tempuv = baseuv;// +stepoffset*i;
				float4 c = tex2D(_MainTex,tempuv);
				float len = length((MainLight_UV.xy - tempuv) / xy);
				float4 color = (c.r > 0.999f) / len;



				float sun = MainLight_UV.z*color / SAMPLE_NUM*_SunSize;
				return sun*MainLightColor;
			}
				ENDCG
		}
	} 
	FallBack "Diffuse"
}
