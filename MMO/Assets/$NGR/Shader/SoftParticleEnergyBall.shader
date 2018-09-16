Shader "SD/SoftParticle/EnergyBall_Add"
{
	Properties 
	{
		_MainTex("_MainTex", 2D) = "white" {}
		_Color("Main_Texture_Color", Color) = (1,1,1,1)
		_Blend_Texture("Blend_Texture", 2D) = "white" {}
		_Color02("Blend_Texture_Color", Color) = (1,1,1,1)
		_Blend_Texture01("Blend_Texture01", 2D) = "black" {}
		_Speed("Main_Texutre_Speed", Float) = 1
		_Speed01("Blend_Texture_Speed", Float) = 1
		_Lighten("Lighten", Float) = 1
		_SoftDistance("Lighten", Range(1,100.0)) = 50.0
	}
	
	SubShader 
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="False"
			"RenderType"="Transparent"

		}

		
		Cull Back
		ZWrite Off
		ZTest LEqual
		Blend One	One
		ColorMask RGBA
		FOG{Color(0,0,0,0)}


		pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#define SHADER_API_GLES
			#include "UnityCG.cginc"
			
			
			
			
			

			sampler2D _MainTex;
			float4 _Color;
			float4 _MainTex_ST;
			sampler2D	_Blend_Texture;
			float4 _Blend_Texture_ST;
			float4 _Color02;
			sampler2D _Blend_Texture01;
			float4 _Blend_Texture01_ST;
			float _Speed;
			float _Speed01;
			float _Speed02;
			float	_Lighten;
			sampler2D _DepthTex;
			float4 _FarCorner;
			float _SoftDistance;
			struct v2f{
				float4 pos 		: SV_POSITION;
				float4 uv 		: TEXCOORD0;
				float2 uv1		: TEXCOORD1;
				float4 proj_pos : TEXCOORD2;
				float4 viewpos	: TEXCOORD3;
			};
		
			v2f vert(appdata_full v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.uv.y	-=	_Speed*_Time.x;
				o.uv.zw	=	TRANSFORM_TEX(v.texcoord,_Blend_Texture);
				o.uv.w	-=	_Speed01*_Time.x;
				o.uv1	=	TRANSFORM_TEX(v.texcoord,_Blend_Texture01);
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
				float val = saturate((depth - new_depth)*_SoftDistance);

				float4 c0 = tex2D(_MainTex,i.uv.xy);
				float4 c1 = tex2D(_Blend_Texture, i.uv.zw);
				float4 c2 = tex2D(_Blend_Texture01, i.uv1);

				c0.xyz =	pow(c0.xyz, 2.2f);
				c1.xyz =	pow(c1.xyz, 2.2f);
				c2.xyz =	pow(c2.xyz, 2.2f);
				float4 	c	=	(c0*_Color*c1*_Color02*c2)*_Lighten;//+bcolor1*i.retColor;
				//float alpha = lerp(0, 1, val);
				return	float4(c.xyz*val, 1);//*emis;//tex2D(_MainTex,i.uv)*lm*2.0f*_Color*(1+emis*2.0f);
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}