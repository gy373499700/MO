// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/GDShader/DOF" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		pass{
			ZTest Off
			ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D	_MainTex;
			sampler2D	_Depth;
			sampler2D	_Sample2x2;
			float4		farCorner;
			float4		invViewport_Radius;

			const static float2 samples[4]={
			
			float2(-0.5,1.5),
			float2(-2.5,0.5),
			float2(-0.5,-0.5),
			float2(1.5,0.5),
			};

			//const static float2 samples[6] = {
			//   float2(-0.326212, -0.405805),
			//   float2(-0.840144, -0.073580),
			//   float2(-0.695914,  0.457137),
			//   float2(-0.203345,  0.620716),
			//   float2( 0.962340, -0.194983),
			//   float2( 0.473434, -0.480026)
			//
			//};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 corner:TEXCOORD1;
			};

			v2f vert (appdata_img v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv = v.texcoord;
				o.corner = float4(v.texcoord*2-1,1,1)*farCorner;
				return o;
			}
			float4 frag( v2f IN) : COLOR
			{
			   float2 halfPixel   =   0.5f*invViewport_Radius.xy;
			   float2   uv=   IN.uv+halfPixel;

			   float4 depth_normal = tex2D(_Depth,IN.uv);
			   float2 XY_Depth	=	float2(1.0f,0.003921568627451);
			   float depth = dot(XY_Depth,depth_normal.xy);
			   float z = IN.corner.z*depth;
			   float blur =	saturate(max(0,z-farCorner.w)/10);
	

			   float2 uvScale = invViewport_Radius.xy*blur;

			   float4   rot   =  tex2D(_Sample2x2,IN.uv*0.5f/invViewport_Radius.xy)*2-1;
			   float2x2 rot_2x2 = float2x2(rot.xyzw);
			   float4 color = 0;
			   for(int i=0;i<4;i++)
			   {
					//float3 off = offset[i];
					float2 uv_temp	=	mul(rot_2x2,samples[i])*uvScale+ IN.uv;
					float4 temp	= tex2D(_MainTex,uv_temp);
					color += temp;//*off.z;
			   }
			   return color/4;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
