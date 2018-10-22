// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GDShader/GI_FinalGather"
{
	Properties
	{
		_Depth ("_Depth", 2D) = "white" {}
		_FinalResult ("_FinalResult", 2D) = "white" {}
		_RandomTex ("_RandomTex", 2D) = "white" {}
		//_Diffuse ("_Diffuse", 2D) = "white" {}
		}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			ZWrite Off
			//ColorMask A
			//Cull Front
			//Blend One One
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			//#pragma multi_compile_fog
			#include "LightCommon.cg"
			#include "UnityCG.cginc"

			float4 farCorner;
			
			
			
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 corner : TEXCOORD1;
			};

			v2f vert (appdata_img v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv = v.texcoord;
				o.corner = float4(v.texcoord*2-1,1,1)*farCorner;//0-1to-1 ,1 because of no drawmesh
				return o;
			}
			
			
			sampler2D _Depth;
			sampler2D _FinalResult;
			sampler2D _RandomTex;
			sampler2D _RandomTex2x2;
			sampler2D _Diffuse;
			
			float4 invViewport;
			float4 ProjVector;
			
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.uv;

				float4 temp = tex2D(_RandomTex,i.uv/256/invViewport.xy);//+float2(0,invViewport.w*0.01f));
				float  temp2x2 = tex2D(_RandomTex2x2,i.uv/2/invViewport.xy).x;

				float3   noise_dir   =  (temp.xyz*2-1);

				float4 depth_normal = tex2D(_Depth,uv);
				float spec	=	tex2D(_Diffuse,uv).w;
				float2 XY_Depth	=	float2(1.0f,0.003921568627451);
				float depth = dot(XY_Depth,depth_normal.xy);
				float3 normal = DecodeNormal(depth_normal.zw);


				float3 view_pos = i.corner*depth;

				float3 dst = normalize(normal+noise_dir.xyz*0.8f);

				float3 dstPos = dst*15+view_pos;// viewpos周围15单位范围的位置

				float4   newDirProjPos      =   (dstPos.xyzz*ProjVector);//special project
				newDirProjPos/=(newDirProjPos.w);
						
				float2 dstuv = (newDirProjPos.xy)*0.15+0.5;
				

				float2 uvoffset = (dstuv-uv);// + noise_dir.xy*0.2f;
				uvoffset*=0.125f;///8
				float2 start_uv = uv + uvoffset*temp2x2;
				float4 ret = 0;

				for(int i=0;i<8;i++)
				{
					float2 sample_uv = start_uv+uvoffset;
					float3 xyz = float3(sample_uv*2-1,1)*farCorner;
					float2 sample_depth = tex2D(_Depth,sample_uv).xy;
					float3 sample_pos = dot(sample_depth.xy,XY_Depth)*xyz;

					float3 dir = (sample_pos - view_pos);
					ret += max(dot(normalize(dir),normal),0)*tex2D(_FinalResult,sample_uv)/saturate(1-length(dir)/100);
				}
				//该版本为简化算法 算高光二次反射
				return ret*(normal.y*0.25+0.75);;


				
			}
			ENDCG
		}
	}
}
