Shader "Hidden/DebugRT"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			ZWrite Off
			ZTest Always

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "LightCommon.cg"

			sampler2D _MainTex;
			float4	_FarCorner;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 viewpos  : TEXCOORD1;
			};

			v2f vert (appdata_full v)
			{
				v2f o;
				#if SHADER_API_GLES || SHADER_API_GLES3
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
			

			fixed4 frag (v2f IN) : SV_Target
			{
				float2 XY_Depth = float2(1.0f,0.003921568627451);		
				float4 depth_normal = tex2D(_MainTex,IN.uv);
				float view_depth = dot(depth_normal.xy,XY_Depth);
				float3 normal = DecodeNormal(depth_normal.zw);


				return normal.xyzz;
			}
			ENDCG
		}
		Pass
			{
				ZWrite Off
				ZTest Always

				CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#include "LightCommon.cg"

				sampler2D _MainTex;
			float4	_FarCorner;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 viewpos  : TEXCOORD1;
			};

			v2f vert(appdata_full v)
			{
				v2f o;
#if SHADER_API_GLES || SHADER_API_GLES3
				//Mobile
				o.pos = float4(v.vertex.x * 2,v.vertex.y * 2,0,1);
				o.uv = o.pos*0.5f + 0.5f;
				o.viewpos = float4(o.pos.xy,1.0f,1)*_FarCorner;
#else
				//Editor Only 
				o.pos = float4(v.vertex.x * 2,-v.vertex.y * 2,0,1);
				o.uv = o.pos*0.5f + 0.5f;
				o.uv.y = 1 - o.uv.y;
				o.viewpos = float4(o.pos.xy,1.0f,1)*_FarCorner;
				o.viewpos.y *= -1;
#endif


				return o;
			}


			fixed4 frag(v2f IN) : SV_Target
			{
				float2 XY_Depth = float2(1.0f,0.003921568627451);
				float4 depth_normal = tex2D(_MainTex,IN.uv);
				float view_depth = dot(depth_normal.xy,XY_Depth);
				float3 normal = DecodeNormal(depth_normal.zw);


				return view_depth;
			}
				ENDCG
			}
	}
}
