Shader "Hidden/GDShader/ShadowMask" {
	Properties {
		//_MainTex ("Base (RGB)", 2D) = "white" {}
		_Cube("Env (RGB)", CUBE) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		pass{


			ZTest	Always
			ZWrite	Off
			ColorMask RGB
			Blend One One
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "LightCommon.cg"
			

			sampler2D _Depth;
			sampler2D _ShadowAO;
			sampler2D _Diffuse;

			float4 farCorner;
			float4 invViewport_Radius;
			float4 lightdir;
			float4 lightcolor;
			float4 ambientcolor;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata_img v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			const static float2 uv_offset[4]=
			{
				float2(-1.5,1.5),
				float2(-1.5,-0.5),
				float2(0.5,1.5),
				float2(0.5,-0.5)
			};


			float4 frag( v2f IN) : COLOR
			{
				
				float2 proj_uv = IN.uv;

				float4 viewdir	=	float4(proj_uv*2-1,1,1)*farCorner;

			   float2 halfPixel   =   0.5f*invViewport_Radius.xy;
			   float2   uv=   proj_uv+halfPixel;

			   float2 XY_Depth = float2(1.0f,0.003921568627451);


			   float4 depth = tex2D(_Depth,proj_uv);
	
			   //return depth;
			   float view_depth = dot(depth.xy,XY_Depth);
			   float3 normal = DecodeNormal(depth.zw);


			   float4 viewspace_pos = viewdir*view_depth;
			  
			   float4 shadow_ao =0;
		
			   for(int idx=0;idx<4;idx++)
			   {
			   	shadow_ao += tex2D(_ShadowAO,proj_uv+invViewport_Radius.xy*uv_offset[idx]);
			   }
			   shadow_ao*=0.25f;
			   return shadow_ao;
			  
			  //float4 spec = tex2D(_ShadowAO,proj_uv);

			 //  float4 col = tex2D(_Diffuse,proj_uv);
			 //  float metal = col.w >0.497f;
			 //  float inRoughness = 1-col.w*2 + metal;
			 //  float fn = inRoughness*inRoughness;
				//float f0   =   (1-fn)/(1+fn);
				//f0*=f0;

			   //return (saturate(1-f0)+0.04f)*shadow_ao.w*ambientcolor + shadow_ao;
			}
			ENDCG
		}
	} 
	//FallBack "Diffuse"
}
