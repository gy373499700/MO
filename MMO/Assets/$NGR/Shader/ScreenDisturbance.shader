Shader "GDShader/ScreenDisturbance" {
	Properties {

		//_Depth ("Depth", 2D) = "white" {}
		_MainTex ("_MainTex", 2D) = "white" {}
		_Color ("_Color", Color) = (1,1,1,1)
		_RefractScale("_RefractScale", Range(0,1.0))=1.0
	}
	SubShader {
		Tags { "Queue" = "Geometry+50" "DeferredLight"="ScreenDisturbance" }
		LOD 200
		pass{

			//Stencil
			//{
			//	Ref 4
			//	Comp equal
			//	Pass keep
			//}

			//ZTest Greater
			ZWrite Off
			//ColorMask RGB
			//Cull Front
			//Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#define SHADER_API_GLES
			#include "UnityCG.cginc"
			
			sampler2D	_DepthTex;
			sampler2D	_ColorTex;
			sampler2D   _MainTex;
			float4		_MainTex_ST;
			float4		_Color;
			float4		_FarCorner;
			float		_RefractScale;
			
			struct v2f{
				float4 pos 			: SV_POSITION;
				
				float4 uv		: TEXCOORD0;
				float4 normal		: TEXCOORD1;
				float3 viewpos		: TEXCOORD2;
			};
			
			v2f vert(appdata_full v)
			{
				v2f o;
				o.pos		=	mul(UNITY_MATRIX_MVP,float4(v.vertex.xyz,1.0f));
				o.normal.xyz	=	mul((float3x3)UNITY_MATRIX_MV,v.normal);
				o.viewpos	=	mul(UNITY_MATRIX_MV,float4(v.vertex.xyz,1.0f));
				o.uv	=	float4(TRANSFORM_TEX(v.texcoord,_MainTex),o.pos.xy);
				#if SHADER_API_GLES|| SHADER_API_GLES3
				o.uv.w *= 1;
				#else
				o.uv.w *= -1;
				#endif
				o.normal.w = o.pos.w;
				return o;
			}
			
			float3 GetViewPos(float2 uv,float3 viewdir)
			{
				float2 XY_Depth	=	float2(1.0f,0.003921568627451);
				float4 depth_normal = tex2D(_DepthTex,uv);
				float depth = dot(XY_Depth,depth_normal.xy);
				return depth*viewdir*_FarCorner;
			
			}
			float4 frag(v2f i) : COLOR
			{
				float4 ret;
				float2 pos = i.uv.zw/i.normal.w;

				float2 uv = pos.xy*0.5+0.5;

				float3 depthpos = GetViewPos(uv,float3(pos.xy,1));

				float len = distance(depthpos,i.viewpos*float3(1,1,-1));

				//float3 ldir = normalize(normalize(i.viewpos)+i.normal);
				//float3 refract_dir = refract(float3(0,0,-1),ldir,1.1f);//_Param.y);

				float4 normal_color = tex2D(_MainTex,i.uv);
				normal_color.xy = normal_color.xy*2-1;
				float4 color = tex2D(_ColorTex,uv+(normal_color.xy)*_RefractScale*normal_color.z*0.1f);

				color *= lerp(1,_Color,normal_color.z);//lerp(1,_Color,saturate(len*len*0.5f));

				return float4(color.xyz,normal_color.z);
			}
			ENDCG
		}
	} 
	//FallBack "Diffuse"
}
