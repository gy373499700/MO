Shader "GDShader/ScreenWater"
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
		Blend SrcAlpha OneMinusSrcAlpha   //subwater 用this   焦散用blend one one 去做
		//Blend One One
		ColorMask RGB
		Cull Off
		ZTest Always 
		ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag 
#pragma multi_compile  USE_Caustics 
#pragma multi_compile	USE_Dynamic_Fog

		
			#include "UnityCG.cginc"
#include "LightCommon.cg"
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 viewpos:TEXCOORD1; 
			};

			sampler2D	_MainTex;
			sampler2D _CausticsTex;
			sampler2D	_Depth;
			sampler2D	_NormalTex;
			sampler2D _FogNoiseTex;
			float4		farCorner;
			float4		_ViewUp;
			float4		invViewport_Radius;
			float refract_scale;
			float4x4 ViewToWorld;
			float WaterHeight;
			float4 ScreenPara;
			float4 _Color;
			float4 _DeepColor;
			float3 PlaneFogSpeed;
			float CausticsLerp;
			v2f vert (appdata v)
			{
				v2f o;
				//o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				//o.uv = v.texcoord;
				//o.corner = float4(v.texcoord*2-1,1,1)*farCorner;
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
			//	o.viewpos = mul(_WorldMatrix, float4(v.vertex.xyz, 1.0f));//*float3(1,1,-1);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{  
				float4 depth_normal = tex2D(_Depth,i.uv);
				float2 XY_Depth = float2(1.0f,0.003921568627451);
				float depth = dot(XY_Depth,depth_normal.xy);
				float3 vpos = i.viewpos*depth;
				float3 dir = normalize(vpos);
				float length = 1-distance(vpos,float3(0,0,0));


				float3 a = tex2D(_NormalTex, i.uv + 1*_Time.y*1);
				float3 b = tex2D(_NormalTex, i.uv + 1*_Time.y*1.0f);
				float3 nor = ((a + b) - 1)*0.5f;
				nor.z = sqrt(1 - max(0, dot(nor.xy, nor.xy)));
				nor = normalize(nor.xzy);


				float ViewCharPosY=ScreenPara.w;
				float4 wpos = mul(ViewToWorld, float4(vpos,1));


				float alpha =1-saturate((vpos.y-ViewCharPosY)*0.5 + 0.5);//水不能太深   水太深的情况要适当换下公式。
				float2 sample_uv = i.uv +nor.xz*0.03*refract_scale;
				float4 col = tex2D(_MainTex, sample_uv);

				if (wpos.y > WaterHeight+0.3f) 
				{
					if(ScreenPara.z == 1)//&&ViewCharPosY<vpos.y) //min and max height
					{//
						float fog =saturate( saturate((ScreenPara.x - abs(vpos.z))/(ScreenPara.x - ScreenPara.y))-0.5);//0-0.3
						return float4(col.xyz,fog)*_Color;//near0.5-0
					}
		
					return 0;
				}
				///float alpha =1-saturate(vpos.y*0.5 + 0.5);//只有下半屏幕渲折射
				///copmute caustics
				//int  size=4;//可以把这儿改成一张张图片传过来比较好
				//float time=floor(_Time.y*32);
				//float row=floor(time /size);
				//float colu=time-row*size;
				//float2 caustics_uv = abs(frac(wpos.xz*0.25))+half2(colu,-row)+nor.xz*0.03*refract_scale;
				//caustics_uv.x/=size;
				//caustics_uv.y/=size;
				//float4 colcau = tex2D(_CausticsTex, caustics_uv)*5;//1 caustics need blend one one pass,2 caustics need fade far from camera
				//if(colcau.r>0.3&&colcau.g>0.3&&colcau.g>0.3)
				//return float4(colcau.rgb,1);
				float4 colcau=float4(1,1,1,1);
#if USE_Caustics
				float2 caustics_uv = abs(frac(wpos.xz*0.25));//+nor.xz*0.08*refract_scale;
			    colcau = tex2D(_CausticsTex, caustics_uv);
#endif
				//证明了那采样方块的一横 是图tu的设置问题
				//float2 sample_uv = i.uv +nor.xz*0.03*refract_scale;
				//float4 col = tex2D(_MainTex, sample_uv);
				if (ScreenPara.z == 1||ScreenPara.z == 2)
					alpha = 1;
				if (alpha < 0.5)
					alpha = 0.5f;
				float fog =1- saturate((ScreenPara.x - abs(vpos.z))/(ScreenPara.x - ScreenPara.y));


				//水下动态雾
				float noise = 0;
#if USE_Dynamic_Fog
				float2 speed = _Time.y*PlaneFogSpeed;
				float2 plane_uv = i.uv;// abs(frac(wpos.xz*0.25));
				float4 nc = tex2D(_NormalTex, plane_uv + speed);
				noise = (nc.r-0.5 )*fog*2*_DeepColor.w;
#endif
				 
				float4 _finalCol = lerp(col*_Color, col*_Color*colcau, CausticsLerp);
				float4 finalCol= lerp(_finalCol, _DeepColor, _Color.w*fog*(1+ noise));

				return float4(finalCol.xyz, alpha);
			}
			ENDCG
		}
			////////////////////////////////////////////////////////////////////
			Pass
			{
				Blend SrcAlpha OneMinusSrcAlpha
				ColorMask RGB
				Cull Off
				ZTest Greater
				ZWrite Off
				CGPROGRAM
#pragma vertex vert
#pragma fragment frag


#include "UnityCG.cginc"
#include "LightCommon.cg"
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 viewpos:TEXCOORD1;
			};

			sampler2D	_MainTex;
			sampler2D	_Depth;
			sampler2D	_NormalTex;
			float4		farCorner;
			float4		_ViewUp;
			float4		invViewport_Radius;
			//float refract_scale;
			float4x4 ViewToWorld;
			float WaterHeight;
		//	float tatalInwater;
		//	float4 _Color;
		//	float4 _DeepColor;
			v2f vert(appdata v)
			{
				v2f o;
				//o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				//o.uv = v.texcoord;
				//o.corner = float4(v.texcoord*2-1,1,1)*farCorner;
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
				//	o.viewpos = mul(_WorldMatrix, float4(v.vertex.xyz, 1.0f));//*float3(1,1,-1);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target  
			{
				float4 depth_normal = tex2D(_Depth,i.uv);
				float2 XY_Depth = float2(1.0f,0.003921568627451);
				float depth = dot(XY_Depth,depth_normal.xy);
				float3 vpos = i.viewpos*depth;



				float3 a = tex2D(_NormalTex, i.uv + 1 * _Time.y * 1);
				float3 b = tex2D(_NormalTex, i.uv + 1 * _Time.y*1.0f);
				float3 nor = ((a + b) - 1)*0.5f;
				nor.z = sqrt(1 - max(0, dot(nor.xy, nor.xy)));
				nor = normalize(nor.xzy);



				float4 wpos = mul(ViewToWorld, float4(vpos,1));
				if (wpos.y > WaterHeight)
				{
					return 0;
				}

				float2 sample_uv = i.uv + nor.xz*0.03*1;
				float4 col = tex2D(_MainTex, sample_uv);
				return float4(col.xyz, 0.5);//forward
			}
				ENDCG
			}
	}
}
