Shader "GDShader/GenNormal" {
	Properties {
		//_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		pass{
			//ColorMask R
			//Blend One One
			ZWrite Off
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#define SAMPLER_NUM 4

			sampler2D _Depth;
			float4 farCorner;
			float4 invViewport_Radius;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 corner : TEXCOORD1;
			};

			v2f vert (appdata_img v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;
				o.corner = float4(v.texcoord*2-1,1,1)*farCorner;
				return o;
			}


			
			

			static float2 sample_point[4]=
			{
				float2(-1,0),
				float2(1,0),
				float2(0,-1),
				float2(0,1),
			};


			float4 frag( v2f IN) : COLOR
			{
			   float2 halfPixel   =   0.5f*invViewport_Radius.xy;
			   float2   uv=   IN.uv;

			   float2 XY_Depth = float2(1.0f,0.003921568627451);//*invViewport_Radius.w;



			   float2 current = tex2D(_Depth,IN.uv).xy;
			   float current_depth = dot(current,XY_Depth);
			   float3 pos = IN.corner.xyz*current_depth;

			   float3 n_pos[4];
			   for(int i=0;i<4;i++)
			   {
					float2 n_uv = uv + invViewport_Radius.xy*sample_point[i];
					float2 n_gbuffer = tex2D(_Depth,n_uv).xy;
					float n_depth = dot(n_gbuffer,XY_Depth);
					n_pos[i].xyz	=	float3(n_uv*2-1,1)*farCorner*n_depth;

			   }
			   float3 x	=	pos-n_pos[0];//abs(n_pos[0].z - current_depth) < abs(n_pos[1].z - current_depth)?pos-n_pos[0].xyz:n_pos[1].xyz-pos;
			   float3 y =	pos-n_pos[2];//abs(n_pos[2].z - current_depth) < abs(n_pos[3].z - current_depth)?pos-n_pos[2].xyz:n_pos[3].xyz-pos;

			   return -normalize(cross((x),(y))).xyzz;
			}
			ENDCG
		}
	} 
	//FallBack "Diffuse"
}
