// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/DiffusePBR"
{//old ds cooktorrrance
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Metal("_Metal",Range(0,1.0)) = 1.0
		_SmoothBase("_SmoothBase",Range(0,1.0)) = 0.0

		_Color("_Color", Color) = (0.5,0.5,0.5,0.5)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{ 
	
			ZTest Less
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile Shadow_Off Shadow_Low Shadow_Mid Shadow_High
			#include "CommonBRDF.cg"
			#include "UnityCG.cginc"

			
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 tangent : TANGENT;
				float2 uv2 : TEXCOORD3;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 vertex 			: SV_POSITION;
				float4 uv 			: TEXCOORD0;
				float4 view_pos_nor	: TEXCOORD1;
				float4 tangent		: TEXCOORD2;
				float3 view	: TEXCOORD3;
				float4 sview	: TEXCOORD4;
			//	float4 wpos			: TEXCOOR4;
			//	float3 normal			: TEXCOOR3;
			};
			sampler2D _ShadowDepth;
			float4 invShadowViewport;
			float4x4 _ShadowView;
			float4x4 _ShadowProj;

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float _SmoothBase;
			float _Metal;
			float4 lightdir;
			float4 lightcolor;
			float4 ambientcolor;
			samplerCUBE _EnvCube;
			sampler2D _Depth;
			float _RoughnessX;
			float _RoughnessY;
			

			v2f vert(appdata v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);

				float4 veiw = mul(UNITY_MATRIX_MV, v.vertex);

				o.view_pos_nor.xyz = normalize(mul(UNITY_MATRIX_MV, float4(v.normal, 0)).xyz);
			//	o.view_pos_nor.xyz = mul(v.normal, (float3x3)UNITY_MATRIX_IT_MV).xyz;
				o.view_pos_nor.w = -veiw.z*_ProjectionParams.w;//view depth
				float3 view_dir = normalize(mul(UNITY_MATRIX_MV, v.vertex).xyz);//viewdir
				o.uv.zw = dot(normalize(o.view_pos_nor.xyz), -view_dir);//法线*视线（假装）
				o.view = view_dir;
				o.tangent = float4((normalize(mul((float3x3)UNITY_MATRIX_MV, v.tangent.xyz))), v.tangent.w);
				float3 normal = normalize(mul(unity_ObjectToWorld, float4(v.normal, 0)).xyz);
				float4 wpos = mul(unity_ObjectToWorld, v.vertex);
				float4 shadow_pos = mul(_ShadowView, wpos + float4(normal.xyz*0.07f, 0));
				float4 shadowproj = mul(_ShadowProj, shadow_pos);
				shadowproj /= shadowproj.w;
				if (abs(shadowproj.x)>1.0f || abs(shadowproj.y)>1.0f)
				{
					shadow_pos.z = 0.0f;
				}
#if SHADER_API_GLES|| SHADER_API_GLES3

#else
				o.view_pos_nor.z *= -1;//坐标系问题
				o.view.z *= -1;
				shadowproj.y *= -1;
#endif
				
				o.sview.xy = shadowproj.xy*0.5 + 0.5;
				o.sview.z = shadow_pos.z;
				o.sview.w = 0;

				return o;
			}
			struct PS_OUTPUT
			{
				float4 diffuse_roughness	: COLOR0;
				float4 depth_normal			: COLOR1;

			};
			PS_OUTPUT frag (v2f i) 
			{
				fixed4 col = tex2D(_MainTex, i.uv.xy);	
			
				float4 retc;  
				retc.xyz=  pow(col.xyz,2.2f)*_Color.xyz*1;//进行伽马矫正  最后输出要power(c, 1/2.2)
				retc.w     =   saturate(_SmoothBase+max(0,1-abs(i.uv.z))*0.2f);//w存粗糙度
				retc.w *=0.495f; 
				if(_Metal>0.5f) 
				{
					retc.w +=0.5f; 
				}
				//float diffuse_roughness=retc.w;
				//float3 normal = normalize(i.view_pos_nor.xyz);//viewspace
				//float2 enormal =	EncodeNormal(normalize(i.view_pos_nor.xyz));//在我们的encodenormal里边有坐标转换，EncodeViewNormalStereo这个没有
				//i.view_pos_nor.z=-i.view_pos_nor.z;//因为camera是左手坐标系，unity的obj是右手坐标系。
				float3 normal=normalize(i.view_pos_nor.xyz);//DecodeNormal(enormal);//most important normal 

				//return float4(normal.xy,0,1); 
               	col =retc;// tex2D(_Diffuse, proj_uv);
             	float3 e =normalize(-i.view);//viewspace
             	float3 diff = 1;
             	float metal =  col.w >= 0.497f;//__Metal
				float roughness =   saturate(1 - (col.w*2.0f) + metal);//1-_SmoothBase
             	float3 spec =(1 + metal * 3);//金属控制高光强度
             	 
     			 
             	PBR(roughness, _Metal, -lightdir.xyz, e, normal, diff, spec,i.tangent);
             	//outspec=lightcolor*(col+metal)*outspec;
             	float3 ambient_spec = 0;   
                float3 R = reflect(-e,normal)*float3(1,1,-1); 
                R = mul(R,(float3x3)UNITY_MATRIX_V);
                float sqrt_roughness = (1-roughness);    
				float3 env = texCUBE(_EnvCube, R).xyz*sqrt_roughness*sqrt_roughness;// texCUBElod(_EnvCube, float4(R, (roughness)*5.0f)).xyz*sqrt_roughness*sqrt_roughness;
                ambient_spec = env*env*(metal+1)*ambientcolor;
				ambient_spec *= _Metal;//metal
				float occ = 1;
#if Shadow_High
				float2 shadowuv = i.sview.xy;
				float2 XY_DEPTH = float2(1.0f, 0.003921568627451)*invShadowViewport.w;
				 occ =  max(0.2, CalcShadow3X3(shadowuv, i.sview.z, XY_DEPTH, invShadowViewport.xyz, _ShadowDepth));
#elif Shadow_Mid
				float2 shadowuv = i.sview.xy;
				float2 XY_DEPTH = float2(1.0f, 0.003921568627451)*invShadowViewport.w;
				occ = max(0.2, CalcShadow2(shadowuv, i.sview.z, XY_DEPTH, invShadowViewport.xyz, _ShadowDepth));
#elif Shadow_Low
				float2 shadowuv = i.sview.xy;
				float2 XY_DEPTH = float2(1.0f, 0.003921568627451)*invShadowViewport.w;
				occ = max(0.2, CalcShadow(shadowuv, i.sview.z, XY_DEPTH, invShadowViewport.xyz, _ShadowDepth));
#endif
			
				float4 color = float4(lightcolor.xyz*col.xyz*(diff*occ + spec) + ambient_spec, 1);//here1/2.2 willbe wrong

				PS_OUTPUT output;
				color.w = col.w;
				output.diffuse_roughness = color;
				float4 ret;
				ret.xy = EncodeDepth(i.view_pos_nor.w);
				ret.zw = EncodeNormal(normalize(normal));
			
				output.depth_normal = ret;
				return output;
			}

			ENDCG 
		}
	}
}
