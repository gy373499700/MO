// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Unlit/VertexShaft"
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
		{ // 顶点偏移的体积光
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest Less
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal:NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 wpos: TEXCOORD2;
			};
			sampler2D _Sample2x2;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float3 _LightPos;
			float _Offset;
			float _LightDis;//model to light
			float _LightDensity;
			float2 _UV;
			float4 invViewport;
			v2f vert (appdata v)
			{
				v2f o;
		
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.wpos= mul((float3x3)unity_ObjectToWorld, v.vertex.xyz);
				float3 opos = mul((float3x3)unity_WorldToObject, _LightPos);
				float3 lightDir = normalize(opos-v.vertex.xyz);
				float factor = dot(lightDir, v.normal);
				float factorvalue = factor > 0 ? 0 : 1; 

				v.vertex.xyz += v.normal*0.1f;
				v.vertex.xyz -= lightDir*factorvalue*_Offset;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);

				return o; 
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float dis = distance(i.wpos,_LightPos); 
			
				float alpha =1- saturate((dis- _LightDis)/ _Offset);
				alpha = pow(alpha, _LightDensity);
				float   step = tex2D(_Sample2x2, i.uv*0.5f / invViewport.xy).x;
				fixed4 col = tex2D(_MainTex, step*float2( i.uv.x/256*768* _UV.x, i.uv.y*_UV.y ));
				return float4(col.xyz*_Color, alpha*col.a);
			}
			ENDCG
		}

		  
		     

	
////////////////////////////////////////////////////////////////////////////////////////

			Pass
			{ //1 绘制灯光 算遮挡
					Blend One One
					//	Blend SrcAlpha OneMinusSrcAlpha 
					ColorMask RGB
					Cull Off
					ZTest Lequal
					ZWrite Off
					CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					#include "UnityCG.cginc"
				sampler2D _MainTex;
			float4 _MainTex_ST;

			float4 _mColor;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;

			};
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float4 color = _mColor;

				return color;

			}
					ENDCG
			}
			Pass
			{ //2 绘制灯光 不管遮挡
				Blend One One
					//	Blend SrcAlpha OneMinusSrcAlpha 
					ColorMask RGB
					Cull Off
					ZTest Off
					ZWrite Off
					CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc" 
				sampler2D _MainTex;
			float4 _MainTex_ST;

			float4 _mColor;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				//float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;

			};
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float4 color = _mColor;

				return color;

			}
					ENDCG
			}
			Pass
			{ //3 拉伸偏移
					Blend One One
				
					ColorMask RGBA
					Cull Off
					ZTest Always
					ZWrite Off
					CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#define BLUR_NUMBER 30
				
				sampler2D _MainTex;
				sampler2D _Sample2x2;
				float4 _MainTex_ST;
				float4 _MainUV;
				float2 _LightParam;
				float4 invViewport;
				float2 _CenterOffset;
				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f  
				{
					float2 uv : TEXCOORD0;
					float4 pos : SV_POSITION;
				};
				v2f vert(appdata v)
				{
					v2f o;
				
#if SHADER_API_GLES|| SHADER_API_GLES3
					//Mobile
					o.pos = float4(v.vertex.x * 2, v.vertex.y * 2, 1, 1);
					o.uv = o.pos*0.5f + 0.5f;
					//o.viewpos = float4(o.pos.xy, 1.0f, 1)*farCorner;
#else
					//Editor Only 
					o.pos = float4(v.vertex.x * 2, v.vertex.y * 2, 1, 1);
					o.uv = o.pos*0.5f + 0.5f;
					o.uv.y = 1 - o.uv.y;
					//o.viewpos = float4(o.pos.xy, 1.0f, 1)*farCorner;
					//o.viewpos.y *= -1;
#endif
					return o;
				}

				float4 frag(v2f IN) : COLOR
				{
					float2   uv = IN.uv;
					float   step =  tex2D(_Sample2x2, IN.uv*0.5f / invViewport.xy).x;
					//return step;
					float2 dir = (_MainUV.xy - uv+ _CenterOffset);

					float4 color = 0;
					float2 stepoffset = _LightParam.x* dir / BLUR_NUMBER ;//*length(invViewport.xy)*invViewport.z;
					float2 baseuv = uv + stepoffset*step;
					float4 base = tex2D(_MainTex, baseuv);
					for (int i = 0; i<BLUR_NUMBER; i++)
					{
						float4 c = tex2D(_MainTex,baseuv + stepoffset*i);;
						color += c;
					}
					return (color/ BLUR_NUMBER)*_LightParam.y;// *invViewport.w;
				}

					ENDCG
			}

			Pass
			{ //4 blend体积光
					Blend One One
					//Blend SrcAlpha OneMinusSrcAlpha
					ColorMask RGBA
					Cull Off
					ZTest Always
					ZWrite Off
					CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"


				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _MainUVOffset;
				sampler2D _DepthTex;
				float _depth;
				float4 farCorner;
				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 pos : SV_POSITION;
				//	float4 viewpos : TEXCOORD1;
				};
				v2f vert(appdata v)
				{
					v2f o;

#if SHADER_API_GLES|| SHADER_API_GLES3
					//Mobile
					o.pos = float4(v.vertex.x * 2, v.vertex.y * 2, 1, 1);
					o.uv = o.pos*0.5f + 0.5f;
					//o.viewpos = float4(o.pos.xy, 1.0f, 1)*farCorner;
#else
					//Editor Only 
					o.pos = float4(v.vertex.x * 2, v.vertex.y * 2, 1, 1);
					o.uv = o.pos*0.5f + 0.5f;
					o.uv.y = 1 - o.uv.y;
				//	o.viewpos = float4(o.pos.xy, 1.0f, 1)*farCorner;
				//	o.viewpos.y *= -1;
#endif
					return o;
				}

				float4 frag(v2f i) : COLOR
				{ 
					float4 depth_normal = tex2D(_DepthTex,i.uv);
					float2 XY_Depth = float2(1.0f,0.003921568627451);
					float depth = dot(XY_Depth,depth_normal.xy);
				//	float3 vpos = i.viewpos*depth;

					float ret =   depth > _depth; 
					float2 uv = i.uv + _MainUVOffset;
					float up = uv.y < 1;
					float down = uv.y > 0;
					float right = uv.x < 1;
					float left = uv.x > 0;
					float4 base = tex2D(_MainTex, uv);
					ret =  ret*up*down*right*left;
					return float4(base.xyz*ret,1);// *invViewport.w;
				}

					ENDCG
				}

				
	}
}
