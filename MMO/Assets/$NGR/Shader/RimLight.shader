Shader "Hidden/GDShader/RimLight" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Diffuse ("_Diffuse (RGB)", 2D) = "white" {}
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
			}//标记哪些材质不会渲染边缘光*/
			ZWrite Off
			ColorMask RGB
			Blend One One
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "LightCommon.cg"

			sampler2D	_Depth;
			sampler2D   _LastFrame;
			sampler2D   _Diffuse;
			float4		invViewport;
			float4		_FarCorner;
			float4 		RimLightColor;
			
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 viewpos  : TEXCOORD1;
			};

			v2f vert (appdata_full v)
			{
				v2f o;
#if SHADER_API_GLES|| SHADER_API_GLES3
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


			
			



			float4 frag( v2f IN) : COLOR
			{
			   float2 halfPixel   =   0.5f*invViewport.xy;
			   float2   uv=   IN.uv;//+halfPixel;


			   //float3 ViewDir   =   float3(viewPos.xy,1000);
			   float2 XY_Depth = float2(1.0f,0.003921568627451);


			   float4 depth_normal = tex2D(_Depth,uv);
			   float4 c = tex2D(_Diffuse, depth_normal.zw);

			   float view_depth = dot(depth_normal.xy,XY_Depth);
			   
			   float3 viewpos = IN.viewpos*view_depth;
			   
			   float3 normal = DecodeNormal(depth_normal.zw);

			   return RimLightColor *c;// *max(0, dot(normal, normalize(viewpos))) + invViewport.w);
			   
			   
			}
			ENDCG
		}
	} 
	//FallBack "Diffuse"
}
