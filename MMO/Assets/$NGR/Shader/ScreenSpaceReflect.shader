Shader "Hidden/GDShader/ScreenSpaceReflect" {
	Properties {
		//_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		pass{

			
			//Stencil
			//{
			//	Ref 1
			//	ReadMask 1
			//	Comp equal
			//	Pass keep
			//}
		//Stencil
		//{
		//	Ref 16
		//	ReadMask 16
		//	Comp equal
		//	Pass keep
		//}

			ZTest Greater
			ZWrite Off
			ColorMask RGB
			Cull Off
			//Cull Front
			Blend One One


			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "LightCommon.cg"
			#include "UnityCG.cginc"

			#define SAMPLE_NUM 8
#define HIGH_QUILITY  1
			
			sampler2D	_MainTex;
			sampler2D	_Sample2x2;
			sampler2D	_FinalDiffuse;
			samplerCUBE	_Specular;
			sampler2D	_Diffuse;
			float4x4	_PorjMatrix;

			float4		_FarCorner;
			float4		invViewport;
			float4		ProjVector;
			float4		ReflectColor;
			float4x4	ViewToWorld;
			float4		AmbientColor;
			
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
				o.pos = float4(v.vertex.x*2,v.vertex.y*2,1,1);//mesh的坐标是-0.5，0.5   直接*2就转换到project空间了。
				o.uv = o.pos*0.5f+0.5f;
				o.viewpos = float4(o.pos.xy,1.0f,1)*_FarCorner;
				#else
				//Editor Only 
				o.pos = float4(v.vertex.x*2,v.vertex.y*2,1,1);
			//	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = o.pos*0.5f+0.5f;
				o.uv.y = 1-o.uv.y;
				o.viewpos = float4(o.pos.xy,1.0f,1)*_FarCorner;
				o.viewpos.y*=-1;
				#endif


				return o;
			}

			float RayMatch(inout float3 start, float3 step, inout float2 uv,float2 XY_Depth,float thickness)
			{
				float hit = 0.0f;
				for (int i = 0; i<SAMPLE_NUM; i++)
				{
					if (hit <0.5f)
					{
						start += step;
						//view pos to proj uv
						float4   newDirProjPos = (start.xyzz*ProjVector);
						newDirProjPos /= (newDirProjPos.w);

						float2 tempuv = (newDirProjPos.xy)*0.5 + 0.5;
						float2 depth_temp = tex2D(_MainTex, tempuv).xy;
						float oldz = dot(depth_temp, XY_Depth);

						if (start.z > oldz && tempuv.y < 1.0f)
						{
							hit = 1.0f;
							uv = tempuv;
							break;
						}
					}
				}
				return hit;
			}
			
			
			float4 frag(v2f i) : COLOR
			{
				//return float4(1,0,0,0);
				float4 ret;

			float2 uv = i.uv;

			float4 _random = tex2D(_Sample2x2,i.uv / 256.0f / invViewport.xy);
			float step = _random.w;
			_random.xyz = _random.xyz * 2 - 1;

			float4 depth_normal = tex2D(_MainTex,uv);
			
			float2 XY_Depth = float2(1.0f,0.003921568627451);
			float depth = dot(XY_Depth,depth_normal.xy);
			float3 normal = DecodeNormal(depth_normal.zw);

			float3 view_pos = i.viewpos*depth;
			//view_pos.y*=-1;

			float4 diffuse = tex2D(_Diffuse,uv);
			
			float metal = 0;
			if (diffuse.w >= 0.497f)
			{
				metal = 1;
			}
			float roughness = diffuse.w*2.0f - metal;


			float3 dir = normalize(view_pos);

			float3 dstdir = normalize(reflect(dir, normal));// +_random.xyz*(1 - spec.w)*0.5);

			float abs_dot = abs(dot(dir, dstdir));
			float fScale = 1+ abs_dot *abs_dot *7;// min(5, length(invViewport.xy) *500.0f / length(offset));


			float3 refl_dir = dstdir;


			//float3   wrefl   =  refl_dir*2.5f/xy_len;///xy_len;
			float3   wrefl = refl_dir*fScale;// / length(offset);
			//_random.w = 0.5f;
			float3 start = view_pos.xyz + _random.w*wrefl;

			XY_Depth *= _FarCorner.z;


			float2 final_uv = uv;

			float hit = RayMatch(start, wrefl, final_uv, XY_Depth, fScale);

#if HIGH_QUILITY == 1
			if (hit > 0.5f)
			{
				start -= wrefl;
				float3   wrefl2 = wrefl / SAMPLE_NUM;
				hit = RayMatch(start, wrefl2, final_uv, XY_Depth, fScale);
			}
#else

#endif
			float3 world_dir = normalize(mul((float3x3)ViewToWorld, refl_dir));

			//float2 skyuv = Normal2Sphere(world_dir);
			//skyuv.y = 1 - skyuv.y;
			float4 sky = texCUBE(_Specular, world_dir);
			//sky*sky*AmbientColor
			float4 ret_color = lerp(sky*sky*AmbientColor, tex2D(_FinalDiffuse, final_uv), hit);
			float frenel = (1 - abs(dot(dir, normal.xyz)));
			frenel *= frenel;
			//frenel *= frenel;
			//return hit;
			//return tex2D(_FinalDiffuse, final_uv)*hit;
			return  ReflectColor*ret_color *(frenel);// *(1 + diffuse.w));// *diffuse;// *diffuse.w;



			}
			ENDCG
		}
	} 
	//FallBack "Diffuse"
}
