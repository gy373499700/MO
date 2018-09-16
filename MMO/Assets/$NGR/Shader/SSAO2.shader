Shader "Unlit/SSAO2"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
		CGINCLUDE
			#include "UnityCG.cginc" 
			#include "LightCommon.cg"
			#define SAMPLES_Num 16
			#define SAMPLE_NOISE///  
			sampler2D	_MainTex;
			sampler2D	_Depth;
			float4		_Depth_ST;
			sampler2D	_Sample2x2;
			sampler2D	_Diffuse;
			//sampler2D   _Random;
			samplerCUBE   _SkyTexture;
			float4		_SkyColor;
			//sampler2D   _LUT;
			float4		_AmbientColor;
			float4		_WorldUp;
			float4		_FarCorner;
			float4		_Params1;
			float _Debug;
			float4x4 ViewToWorld;
			sampler2D	_SSAO;
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 viewpos  : TEXCOORD1;
			};

			v2f vert (appdata_full v)
			{
				v2f o;

				#if SHADER_API_GLES || SHADER_API_GLES3
				//Mobile
				o.pos = float4(v.vertex.x*2,v.vertex.y*2,1,1);
				o.uv = o.pos*0.5f+0.5f;
				o.viewpos = float4(o.pos.xy,1.0f,1)*_FarCorner;//可以把depth直接塞进去，如果ps阶段不用depth的话。
				#else
				//Editor Only 
				o.pos = float4(v.vertex.x*2,v.vertex.y*2,1,1);
				o.uv = o.pos*0.5f+0.5f;
				o.uv.y = 1 - o.uv.y;
				o.viewpos = float4(o.pos.xy,1.0f,1)*_FarCorner;
				o.viewpos.y*=-1;
				#endif
				
				return o;
			}
		    inline half calcAO(half2 tcoord, half2 uvoffset, half3 p, half3 norm,half3 viewdir)
			{
				half2 t = tcoord + uvoffset;//采样点是UV坐标周围的点 对应的world空间点
				float2 XY_Depth = float2(1.0f,0.003921568627451);	
				float depth = dot(tex2D(_Depth, t).xy, XY_Depth);
				float3 diff = float3(t*2-1, 1)*_FarCorner*depth-p;//viewpos offset
				half3 v = normalize(diff);
				half d = length(diff) *0.11;// _Params1.w/100;//distance

				//return dot(viewdir, v);
			
				
				return max(0.0, dot(norm, v))* (1.0 / (1.0 + d)) ;
				//半球积分
			}
			float4 frag( v2f IN) : COLOR
			{
				
			    float2   uv=   IN.uv;
			    float2 XY_Depth = float2(1.0f,0.003921568627451);			  
			    float2 sampleuv = uv;	 
			    float4 depth_normal = tex2D(_Depth,sampleuv);	   
			    float view_depth = dot(depth_normal.xy,XY_Depth);//
			    float3 normal = DecodeNormal(depth_normal.zw);
			    float3 view_pos = IN.viewpos*view_depth;
			    float3 viewdir = normalize(view_pos);
			   
		    	const half2 CROSS[4] = { half2(1.0, 0.0), half2(-1.0, 0.0), half2(0.0, 1.0), half2(0.0, -1.0) };
	        	half eyeDepth =view_depth;// LinearEyeDepth(depth);
	        	half3 position =view_pos;// getWSPosition(uv, depth); // World space
	        	#if defined(SAMPLE_NOISE)
	        	half2 random = normalize(tex2D(_Sample2x2, _ScreenParams.xy * uv / _Params1.x).rg * 2.0 - 1.0);
	        	#endif
	        
	        	half radius =max(_Params1.y/100, 0.005); 
	        //	if(view_pos.z>30)
	        //		return view_depth;// Skip out of range pixels!!!!!!!!!!!!!!!!!!!!!!!!
	        	half ao = 0;



	        	// Sampling
	        	for (int j = 0; j < 4; j++)
	        	{
	        		half2 coord1;
	        
	        		#if defined(SAMPLE_NOISE)
	        		coord1 = reflect(CROSS[j], random) * radius;//this random important
	        		#else
	        		coord1 = CROSS[j] * radius;
	        		#endif
	        
	        		#if !SAMPLES_VERY_LOW
	        		half2 coord2 = coord1 * 0.707;
	        		coord2 = half2(coord2.x - coord2.y, coord2.x + coord2.y);
	        		#endif
	        
	        		#if SAMPLES_Num==20			// 20
	        		ao += calcAO(uv, coord1 * 0.20, position, normal,viewdir);
	        		ao += calcAO(uv, coord2 * 0.40, position, normal,viewdir);
	        		ao += calcAO(uv, coord1 * 0.60, position, normal,viewdir);
	        		ao += calcAO(uv, coord2 * 0.80, position, normal,viewdir);
	        		ao += calcAO(uv, coord1, position, normal,viewdir);
	        		#elif SAMPLES_Num==16			// 16
	        		ao += calcAO(uv, coord1 * 0.25, position, normal,viewdir);
	        		ao += calcAO(uv, coord2 * 0.50, position, normal,viewdir);
	        		ao += calcAO(uv, coord1 * 0.75, position, normal,viewdir);
	        		ao += calcAO(uv, coord2, position, normal,viewdir);
	        		#elif SAMPLES_Num==12		// 12
	        		ao += calcAO(uv, coord1 * 0.30, position, normal,viewdir);
	        		ao += calcAO(uv, coord2 * 0.60, position, normal,viewdir);
	        		ao += calcAO(uv, coord1 * 0.90, position, normal,viewdir);
	        		#elif SAMPLES_Num==8			// 8
	        		ao += calcAO(uv, coord1 * 0.30, position, normal,viewdir); 
	        		ao += calcAO(uv, coord2 * 0.80, position, normal,viewdir);
	        		#elif SAMPLES_Num==4		// 4				
	        		ao += calcAO(uv, coord1 * 0.50, position, normal,viewdir);
	        		#endif

	        	} 
	        	ao /= SAMPLES_Num;	

	        	float3 ret;
				//float3 ret =1-ao;
				
				if (_Debug == 1) 
				{//merge
					float3 ret = 1 - ao;
					return  half4(ret.xyz, 1);
				}
				else 
				{//perfect
					ret = 0.5-ao;
				}

	        	float4 diff = tex2D(_Diffuse, sampleuv);

   				ret*= diff.xyz*_AmbientColor;  				
				float3 WorldN= mul((float3x3)ViewToWorld, normal);
				float3 envDiff = texCUBE(_SkyTexture,WorldN); 
				ret *= envDiff*envDiff;


				return  half4(ret.xyz, 1); 
		    }
	
    	ENDCG  

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

	
			pass { //0
			Stencil  
			{ 
				Ref 64 
				ReadMask 64
				Comp Notequal  
				Pass keep   
			}
			ZTest Greater
			ZWrite Off 
			ColorMask RGB  
			Cull Off 
			Blend One One    
			CGPROGRAM
		
			#pragma target 3.0     
			#pragma vertex vert    
			#pragma fragment frag                                                     
			ENDCG         
		} 
	
			pass 
			{ //1
			Stencil  
			{ 
				Ref 64 
				ReadMask 64
				Comp Notequal  
				Pass keep   
			} 
			ZTest Greater
			ZWrite Off 
			ColorMask RGB  
			Cull Off 
			Blend One Zero    
			CGPROGRAM
			#pragma target 3.0     
			#pragma vertex vert    
			#pragma fragment frag                                                     
			ENDCG         
		} 
/////////////////////////////////////////////////////////
			//mergemode
			pass { //2
				Stencil
				{
					Ref 64
					ReadMask 64
					Comp Notequal
					Pass keep
				}

					ZTest Greater
					ZWrite Off
					ColorMask RGB
					Cull Off
					//Blend SrcAlpha OneMinusSrcAlpha
					CGPROGRAM
					#pragma target 3.0     
					#pragma vertex vert    
					#pragma fragment fragCombine     
				float4 fragCombine(v2f IN) : COLOR
				{ 
					float2   uv = IN.uv;
					float4 diff = tex2D(_Diffuse, uv);
					float4 ssao = tex2D(_SSAO, uv);
					return  half4(ssao.xyz*diff.xyz, 1);
				}
					ENDCG
			}
			pass
			{ //3
				Stencil
				{
					Ref 64
					ReadMask 64
					Comp Notequal
					Pass keep
				}
					ZTest Greater
					ZWrite Off
					ColorMask RGB
					Cull Off
					Blend One Zero
					CGPROGRAM
#pragma target 3.0     
#pragma vertex vert    
#pragma fragment frag
				
					ENDCG
			}
			pass
			{ //4
				Stencil
				{
					Ref 64
					ReadMask 64
					Comp Notequal
					Pass keep
				}
					ZTest Greater
					ZWrite Off
					ColorMask RGB
					Cull Off
					Blend One Zero

					CGPROGRAM
#pragma target 3.0     
#pragma vertex vert    
#pragma fragment fragBest
#define SAMPLER_NUM 16
#define ROTATE 1

				float4 invViewport_Radius;
#ifndef SAMPLER_NUM
#define SAMPLER_NUM 32
#endif
#if SAMPLER_NUM == 32
				static float radius = 10.0f;
				static float2 offset[32] = {
					float2(0.2599703f, -0.8976048f)*radius,
					float2(0.4422587f, -0.4750184f)*radius,
					float2(-0.1986221f, -0.6939777f)*radius,
					float2(-0.0124238f, -0.4680904f)*radius,
					float2(-0.4416051f, -0.3365665f)*radius,
					float2(0.3594103f, -0.1362432f)*radius,
					float2(-0.1736233f, -0.1094101f)*radius,
					float2(0.8127208f, -0.2710058f)*radius,
					float2(0.7109355f, -0.5696113f)*radius,
					float2(-0.04597443f, -0.9623359f)*radius,
					float2(-0.741334f, 0.04512462f)*radius,
					float2(-0.9083741f, -0.3378215f)*radius,
					float2(-0.7696987f, -0.597804f)*radius,
					float2(-0.3956201f, 0.1144934f)*radius,
					float2(0.3825082f, 0.2819604f)*radius,
					float2(0.6036237f, 0.03521964f)*radius,
					float2(0.7026271f, 0.4224456f)*radius,
					float2(0.8637612f, 0.1912058f)*radius,
					float2(0.08374725f, 0.1254498f)*radius,
					float2(0.5370411f, -0.8054448f)*radius,
					float2(-0.8713361f, 0.3171529f)*radius,
					float2(-0.551398f, 0.3643217f)*radius,
					float2(-0.473556f, -0.7749792f)*radius,
					float2(-0.499662f, 0.7189553f)*radius,
					float2(-0.7588091f, 0.5759102f)*radius,
					float2(-0.254542f, 0.3688764f)*radius,
					float2(-0.1405081f, 0.6183669f)*radius,
					float2(0.2668809f, 0.8706948f)*radius,
					float2(-0.0003728607f, 0.99127f)*radius,
					float2(0.06034204f, 0.4170518f)*radius,
					float2(0.4010086f, 0.5608956f)*radius,
					float2(0.6499857f, 0.7549948f)*radius,
				};
#elif SAMPLER_NUM == 16
				static float radius = 20.0f;
				const static float2 offset[16] = {
					float2(-0.3274248f, -0.8165722f),
					float2(0.2417469f, -0.3051864f),
					float2(0.2739029f, -0.8037025f),
					float2(-0.2944864f, -0.1231967f),
					float2(-0.7281096f, -0.1973342f),
					float2(-0.7600139f,  0.5833009f),
					float2(-0.0826142f,  0.2298674f),
					float2(-0.9805833f,  0.1633332f),
					float2(-0.7545381f, -0.6213331f),
					float2(0.6095112f,  0.5189977f),
					float2(-0.3558101f,  0.8667912f),
					float2(0.2464557f,  0.8337156f),
					float2(0.4317672f,  0.1220754f),
					float2(0.5942856f, -0.5409905f),
					float2(0.9252075f, -0.2078843f),
					float2(-0.5248916f,  0.2508877f)
				};
#elif SAMPLER_NUM == 8

				static float radius = 10.0f;
				static float2 offset[8] = {
#if ROTATE == 0
					float2(0.5276858f, -0.8309537f)*radius,
					float2(0.7190355f, 0.1213354f)*radius,
					float2(0.02684063f, -0.2622508f)*radius,
					float2(0.3505048f, 0.9361609f)*radius,
					float2(-0.6499152f, 0.5111607f)*radius,
					float2(-0.4145076f, -0.756127f)*radius,
					float2(-0.7485286f, -0.08379126f)*radius,
					float2(-0.07827432f, 0.5377133f)*radius,
#else
					float2(0.0,1.0)*radius*0.2f,
					float2(1.4,1.4)*radius*0.2f,
					float2(2.0,0.0)*radius*0.2f,
					float2(3.0,-3.0)*radius*0.2f,
					float2(0.0,-4)*radius*0.2f,
					float2(-5.0,-5.0)*radius*0.2f,
					float2(-6,0.0)*radius*0.2f,
					float2(-7.0,7.0)*radius*0.2f
#endif
				};
#endif
				const static fixed weight = 0.5f / SAMPLER_NUM;


				float4 fragBest(v2f IN) : COLOR
				{
					//float2 halfPixel   =   0.5f*invViewport_Radius.xy;
					float2   uv = IN.uv;

					float2 XY_Depth = float2(1.0f,0.003921568627451);

					float2 sampleuv = uv;

					float4 depth_normal = tex2D(_Depth,sampleuv);

					float view_depth = dot(depth_normal.xy,XY_Depth);
					float3 normal = DecodeNormal(depth_normal.zw);


					float3 view_pos = IN.viewpos*view_depth;
					float3 viewdir = normalize(view_pos);
					//view_pos += normal*0.1f;
					float2x2 rotate = float2x2(tex2D(_Sample2x2, sampleuv*0.25f / invViewport_Radius.xy));


					float4   tempUV;

					half occ = 0;

					tempUV = invViewport_Radius.xyxy*invViewport_Radius.z;

					tempUV.zw *= 2.0f;

					float4 UV[4];
					float4 baseuv = float4(IN.uv,IN.uv * 2 - 1);

					for (int i = 0; i<SAMPLER_NUM; i += 4) {


#if ROTATE == 1   //主要靠这个rotate来旋转顶点 显示模糊效果
						UV[0] = mul(rotate,offset[i]).xyxy*tempUV + baseuv;
						UV[1] = mul(rotate,offset[i + 1]).xyxy*tempUV + baseuv;
						UV[2] = mul(rotate,offset[i + 2]).xyxy*tempUV + baseuv;//uv坐标螺旋偏移
						UV[3] = mul(rotate,offset[i + 3]).xyxy*tempUV + baseuv;

#else
						UV[0] = offset[i].xyxy*tempUV + baseuv;
						UV[1] = offset[i + 1].xyxy*tempUV + baseuv;
						UV[2] = offset[i + 2].xyxy*tempUV + baseuv;
						UV[3] = offset[i + 3].xyxy*tempUV + baseuv;
#endif

						float4 sample_depth;
						sample_depth.x = dot(tex2D(_Depth, UV[0].xy).xy, XY_Depth);
						sample_depth.y = dot(tex2D(_Depth, UV[1].xy).xy, XY_Depth);
						sample_depth.z = dot(tex2D(_Depth, UV[2].xy).xy, XY_Depth);
						sample_depth.w = dot(tex2D(_Depth, UV[3].xy).xy, XY_Depth);

						UV[0].xyz = float3(UV[0].zw, 1)*_FarCorner*sample_depth.x - view_pos;//采样偏移后的uv坐标对应的viewpos点 作为采样点
						UV[1].xyz = float3(UV[1].zw, 1)*_FarCorner*sample_depth.y - view_pos;
						UV[2].xyz = float3(UV[2].zw, 1)*_FarCorner*sample_depth.z - view_pos;//这样写是为了简化指令数
						UV[3].xyz = float3(UV[3].zw, 1)*_FarCorner*sample_depth.w - view_pos;

						sample_depth.x = dot(normalize(UV[0].xyz), viewdir);
						sample_depth.y = dot(normalize(UV[1].xyz), viewdir);
						sample_depth.z = dot(normalize(UV[2].xyz), viewdir);
						sample_depth.w = dot(normalize(UV[3].xyz), viewdir);

						occ += dot(sample_depth, weight);

					}

					float3 ret = occ+1;
					return  half4(ret.xyz, 1);
					 
	

				}
					ENDCG
			}

}  

}