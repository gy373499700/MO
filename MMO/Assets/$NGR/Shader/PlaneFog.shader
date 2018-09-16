Shader "Unlit/PlaneFog"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{ 
			ZTest Greater
			ZWrite Off
			ColorMask RGB
			Blend  SrcAlpha OneMinusSrcAlpha
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
				float4 viewpos : TEXCOORD1;
			};
			sampler2D	_MainTex;
			sampler2D	_Depth;
			float4		farCorner;
			float4		FogColor;
			sampler2D	_Noice;
			float3 PlaneFogSpeed;
			float4x4 ViewToWorld;
			float4  _PlaneFogParam;//     [Tooltip("地面雾效,x 雾效强度，y为噪声强度，z云的起始高度,w云的结束高度")]
			v2f vert (appdata v)
			{

				v2f o;
#if SHADER_API_GLES|| SHADER_API_GLES3
				//Mobile
				o.pos = float4(v.vertex.x * 2, v.vertex.y * 2, 1, 1);
				o.uv = o.pos*0.5f + 0.5f;
				o.viewpos = float4(o.pos.xy, 1.0f, 1)*farCorner;
#else
				//Editor Only 
				o.pos = float4(v.vertex.x * 2, v.vertex.y * 2, 1, 1);
				o.uv = o.pos*0.5f + 0.5f;
				o.uv.y = 1 - o.uv.y;
				o.viewpos = float4(o.pos.xy, 1.0f, 1)*farCorner;
				o.viewpos.y *= -1;
#endif
				return o;
			} 
			
			fixed4 frag (v2f i) : SV_Target
			{ 
				float4 depth_normal = tex2D(_Depth,i.uv);
				float2 XY_Depth = float2(1.0f,0.003921568627451);
				float depth = dot(XY_Depth,depth_normal.xy);
				float3 pos = i.viewpos*depth;
				float3 wpos = mul((float3x3)ViewToWorld, pos);
			//	if(wpos.y>11)	return FogColor;
				float density = _PlaneFogParam.x*(_PlaneFogParam.w - wpos.y) / (_PlaneFogParam.w - _PlaneFogParam.z);
				float noise = 0;

				float2 speed = _Time.y*PlaneFogSpeed;
				float2 plane_uv = i.uv;// abs(frac(wpos.xz*0.25));
				float4 nc = tex2D(_Noice, plane_uv + speed);
				noise = (nc.r)*_PlaneFogParam.y*density;
				clip(density);
				return  float4(FogColor.xyz, FogColor.w*density*(1+ noise));
			}
			ENDCG
		}
	}
}
