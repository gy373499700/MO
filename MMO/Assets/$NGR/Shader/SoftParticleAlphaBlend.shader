Shader "SD/SoftParticle/AlphaBlend" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Color("Main Color", Color) = (0.5,0.5,0.5,0.5)
	}
		SubShader{
		Tags{ "Queue" = "Transparent+50" "RenderType" = "Transparent" }
		LOD 200

		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		Lighting Off
		ZWrite Off
		ZTest Less
		Fog{ Color(0,0,0,0) }
		pass {
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
			//#define SHADER_API_GLES
#include "UnityCG.cginc"

		sampler2D _MainTex;
		sampler2D _DepthTex;
		float4 _MainTex_ST;
		float4 _Color;
		float4 _FarCorner;

		struct v2f {
			float4 pos 		: SV_POSITION;
			float2 uv 		: TEXCOORD0;
			float4 proj_pos : TEXCOORD1;
			float4 viewpos 	: TEXCOORD2;
		};

		v2f vert(appdata_full v)
		{
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
			o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
			o.proj_pos = o.pos;
#if SHADER_API_GLES || SHADER_API_GLES3

#else
			//SHADER_API_METAL
			o.proj_pos.y *= -1;
#endif
			o.viewpos = mul(UNITY_MATRIX_MV, v.vertex);
			return o;
		}

		half4 frag(v2f i) : COLOR
		{
			float4 proj = i.proj_pos / i.proj_pos.w;
			float2 proj_uv = proj.xy*0.5f + 0.5f;
			float4 gbuffer = tex2D(_DepthTex, proj_uv);
			float2 XY_Depth = float2(1.0f, 0.003921568627451);
			float depth = dot(gbuffer.xy, XY_Depth);
			float new_depth = -i.viewpos.z / _FarCorner.z;
			float4 color = pow(tex2D(_MainTex, i.uv),2.2f)*_Color;
			float val = saturate((depth - new_depth)*20.0f);
			color.a = lerp(0, color.a, pow(val,1));
			//float emis = 2-saturate(dot(i.flength,i.flength));//1.0f - saturate(i.flength/MajorLightColor.w);
			return color;//*emis;//tex2D(_MainTex,i.uv)*lm*2.0f*_Color*(1+emis*2.0f);
		}
			ENDCG
	}
	}
		FallBack "Diffuse"
}
