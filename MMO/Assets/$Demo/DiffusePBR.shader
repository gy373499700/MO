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

			//#include "LightCommon.cg"
			#include "UnityCG.cginc"
			#define PI 3.14159265f
			
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
				float4 wpos			: TEXCOOR4;
				float3 normal			: TEXCOOR3;
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
			#define MaterialFloat float
			float Square( float x )
			{
				return x*x;
			}
			MaterialFloat ClampedPow(MaterialFloat X,MaterialFloat Y)
			{
				return pow(max(abs(X),0.000001f),Y);
			}
			MaterialFloat PhongShadingPow(MaterialFloat X, MaterialFloat Y)
			{
				return ClampedPow(X, Y);
			}
			// [Blinn 1977, "Models of light reflection for computer synthesized pictures"]
			float D_Blinn( float Roughness, float NoH )
			{
				float m = Roughness * Roughness;
				float m2 = m * m;
				float n = 2 / m2 - 2;
				return (n+2)/ 2 * PhongShadingPow( NoH, n );		// 1 mad, 1 exp, 1 mul, 1 log
			}
			// Anisotropic GGX
			// [Burley 2012, "Physically-Based Shading at Disney"] can divide PI
			float D_GGXaniso( float RoughnessX, float RoughnessY, float3 N, float3 H, float4 tangent,float Roughness)
			{//各向异性指的是在不同方向光照效果产生的差异//disney效果更好
				float3 X = tangent;
				float3 Y = cross( normalize(N), normalize(tangent.xyz) ) * tangent.w;//binormal
				float NoH = saturate( dot(N, H) );
				float mx = RoughnessX * RoughnessX;
				float my = RoughnessY * RoughnessY;
				float XoH = dot( X, H );
				float YoH = dot( Y, H );
				float d = XoH*XoH / (mx*mx) + YoH*YoH / (my*my) + NoH*NoH;
				return Roughness / ( mx*my * d*d );//from disney论文
			}
			float D_ward( float RoughnessX, float RoughnessY, float3 N, float3 H, float4 tangent,float i,float Roughness)
			{//效果不好 
				float3 X = tangent;
				float3 Y = cross( normalize(N), normalize(tangent.xyz) ) * tangent.w;//binormal
				float NoH = saturate( dot(N, H) );
				float mx = RoughnessX * RoughnessX;
				float my = RoughnessY * RoughnessY;
				float XoH = dot( X, H );
				float YoH = dot( Y, H );
				float O=reflect(-i,N);
				float d = (XoH*XoH / (mx*mx) + YoH*YoH / (my*my)) /(NoH*NoH);
				float t=Roughness;///(mx*my*sqrt(dot(i,N)*dot(O,N)));
				return exp(-d)/t;//from note on ward brdf论文
			}
			// [Beckmann 1963, "The scattering of electromagnetic waves from rough surfaces"]
			float D_Beckmann( float Roughness, float NoH )
			{
				float m = Roughness * Roughness;
				float m2 = m * m;
				float NoH2 = NoH * NoH;
				return exp( (NoH2 - 1) / (m2 * NoH2) ) / ( m2 * NoH2 * NoH2 );
			}
			// [Burley 2012, "Physically-Based Shading at Disney"]

			half3 Diffuse_Lambert( half3 DiffuseColor )
			{
				return DiffuseColor / PI;
			}
			half3 SimplePointLightDiffuse( half3 DiffuseColor ,float inRoughness,float3 L,float3 N)
			{
				float fn = inRoughness*inRoughness;
				float f0   =   (1-fn)/(1+fn);
				return Diffuse_Lambert(DiffuseColor)*(dot(L, N)*0.5 + 0.5)*saturate(1-f0*f0);
			}
			float chiGGX(float v)
			{
			    return v > 0 ? 1 : 0;
			}

			float D_GGX_Distribution(float Roughness,float NoH)
			{//this is the same  with D_GGX,公式也完全一样
				float alpha=Roughness*Roughness;
			    float alpha2 = alpha * alpha;
			    float NoH2 = NoH * NoH;
			    float den = NoH2 * alpha2 + (1 - NoH2);
			    return (chiGGX(NoH) * alpha2) / ( PI * den * den );
			}
			float G_GGX_PartialGeometryTerm(float3 v, float3 n, float3 h, float Roughness)
			{
				float alpha=Roughness*Roughness;
			    float VoH = saturate(dot(v,h));
			    float chi = chiGGX( VoH / saturate(dot(v,n)) );
			    float VoH2 = VoH * VoH;
			    float tan2 = ( 1 - VoH2 ) / VoH2;
			    return (chi * 2) / ( 1 + sqrt( 1 + alpha * alpha * tan2 ) );
			}
			float D_GGX( float Roughness, float NoH)
			{
				float m = Roughness * Roughness;
				float m2 = m * m;
				float d = ( NoH * m2 - NoH ) * NoH + 1;	// 2 mad
				return m2 / ( d*d );//*LN;//dot(UnitL,N);					// 2 mul, 1 rcp
			}
			float G_Implicit() 
			{
				return 0.25;
			}
			// [Neumann et al. 1999, "Compact metallic reflectance models"]
			float G_Neumann( float NoV, float NoL )
			{
				return 1 / ( 4 * max( NoL, NoV ) );
			}

			// [Kelemen 2001, "A microfacet based coupled specular-matte brdf model with importance sampling"]
			float G_Kelemen( float3 L, float3 V )
			{
				return 1 / ( 2 + 2 * dot(L, V) );
			}

			// Tuned to match behavior of G_Smith
			// [Schlick 1994, "An Inexpensive BRDF Model for Physically-Based Rendering"]
			float G_Schlick( float Roughness, float NoV, float NoL )
			{
				float k = Square( Roughness ) * 0.5;
				float G_SchlickV = NoV * (1 - k) + k;
				float G_SchlickL = NoL * (1 - k) + k;
				return 0.25 / ( G_SchlickV * G_SchlickL );
			}

			// Smith term for GGX modified by Disney to be less "hot" for small roughness values
			// [Smith 1967, "Geometrical shadowing of a random rough surface"]
			// [Burley 2012, "Physically-Based Shading at Disney"]
			float G_Smith( float Roughness, float NoV, float NoL )
			{
				float a = Square( Roughness );
				float a2 = a*a;

				float G_SmithV = NoV + sqrt( NoV * (NoV - NoV * a2) + a2 );//+NL
				float G_SmithL = NoL + sqrt( NoL * (NoL - NoL * a2) + a2 );//+NV
				return 1.0f/( G_SmithV * G_SmithL );
			}
			float3 F_None( float3 SpecularColor )
			{
				return SpecularColor;
			}
			// [Schlick 1994, "An Inexpensive BRDF Model for Physically-Based Rendering"]
			// [Lagarde 2012, "Spherical Gaussian approximation for Blinn-Phong, Phong and Fresnel"]
			float3 F_Schlick( float3 SpecularColor, float VoH )
			{//high effective than original foumula
				// Anything less than 2% is physically impossible and is instead considered to be shadowing 
				return SpecularColor + (saturate( 50.0 * SpecularColor.g ) - SpecularColor ) * exp2( (-5.55473 * VoH - 6.98316) * VoH );//unreal 公式
				//SpecularColor is Fc
				//float Fc = exp2( (-5.55473 * VoH - 6.98316) * VoH );	// 1 mad, 1 mul, 1 exp
				//F_Schlick= Fc + (1 - Fc) * SpecularColor;					// 1 add, 3 mad
			}
	
			float F_Schlick2(float F0,float3 VH)
			{
                return  F0+(1-F0)*(pow(1.0 - VH,5.0));
			}
			float3 F_Fresnel( float3 SpecularColor, float VoH )
			{//这个公式消耗太高
				float3 SpecularColorSqrt = sqrt( clamp( float3(0, 0, 0), float3(0.99, 0.99, 0.99), SpecularColor ) );
				float3 n = ( 1 + SpecularColorSqrt ) / ( 1 - SpecularColorSqrt );
				float3 g = sqrt( n*n + VoH*VoH - 1 );
				return 0.5 * Square( (g - VoH) / (g + VoH) ) * ( 1 + Square( ((g+VoH)*VoH - 1) / ((g-VoH)*VoH + 1) ) );
			}
			half3 SimplePointLightSpecular( half3 SpecularColor,half inRoughness, float3 UnitL, float3 V, half3 N ,float4 tangent)
			{
				float Roughness = inRoughness;
				// TODO move outside tile loop
				Roughness = max( 0.08, Roughness );
				float3 H = normalize(V + UnitL);
				float NoH = saturate( dot(N, H) );
				float NoV = saturate( dot(N, V) );	
				float NoL = saturate( dot(N, UnitL) );	
				float VoH = saturate( dot(V, H) );	   			
				// Generalized microfacet specular
				//float D=D_GGXaniso(_RoughnessX, _RoughnessY,N,  H,  tangent,inRoughness);
				float  D =D_GGX( Roughness, NoH) / PI;//微平面分布函数 ，给定方向上的微平面的分数值
				float  G = G_Schlick(Roughness, NoV, NoL);
				//float  G = G_Implicit();//几何衰减系数G_Schlick(Roughness,NoV,NoL);//
				float3 F = F_Fresnel(SpecularColor, VoH);// F_None(SpecularColor);// F_Schlick( SpecularColor,VoH);//fresnel 反射系数，表示反射方向上的光强占原始光强的比率
				return (D * G) * F;// *dot(UnitL, N);//LN是ggx需要的,放在场景中让背面会变黑（物理正确），所以不加，让其背面亮一点

					//FGBufferData InGBufferData = ScreenSpaceData.GBuffer;
			}	
			void PBR(half inRoughness, half3 UnitL, half3 V, half3 N ,inout half3 diff,inout half3 spec,float4 tangent)
			{ 	

				diff	=	SimplePointLightDiffuse(diff,inRoughness,UnitL,N);
				spec	=	SimplePointLightSpecular(spec,inRoughness,UnitL,V,N,tangent);

			} 
			float CalcLerp2X2(float2 uv, float4 dot_val)
			{
				float2 lerp_val = frac(uv*1024.0f);
				float2 temp = lerp(dot_val.xy, dot_val.zw, lerp_val.y);

				return lerp(temp.x, temp.y, lerp_val.x);
			}	const static float2 kernel3x3[9] =
			{
				float2(-1,-1),
				float2(0,-1),
				float2(1,-1),

				float2(-1,0),
				float2(0,0),
				float2(1,0),

				float2(-1,1),
				float2(0,1),
				float2(1,1),
			};
			float CalcShadow3X3(float2 uv, float z, float2 XY_Depth, float3 invVP, sampler2D _ShadowDepth)
			{
				float _depth[9];
				for (int i = 0; i < 9; i++)
				{
					float2 temp = kernel3x3[i] * invVP.xyxy + uv.xyxy;
					float2 temp_depth = tex2D(_ShadowDepth, temp).xy;
					_depth[i] = dot(temp_depth, XY_Depth.xy) - z>0;
				}

				float ret = 0;
				ret += CalcLerp2X2(uv + kernel3x3[0] * invVP.xy, float4(_depth[0], _depth[1], _depth[3], _depth[4]));
				ret += CalcLerp2X2(uv + kernel3x3[1] * invVP.xy, float4(_depth[1], _depth[2], _depth[4], _depth[5]));
				ret += CalcLerp2X2(uv + kernel3x3[3] * invVP.xy, float4(_depth[3], _depth[4], _depth[6], _depth[7]));
				ret += CalcLerp2X2(uv + kernel3x3[4] * invVP.xy, float4(_depth[4], _depth[5], _depth[7], _depth[8]));
				return ret*0.25f;
			}

			float3 CalcLeafPos(float3 pos, float3 n)
			{
				float fsin = 0;
				float fcos = 0;
				sincos((_Time.y + pos.y)*3.14f*2, fsin, fcos);
				return pos + n*fsin*0.025f*2;// + fcos*float3(0,1,0)*0.01f;
			}
			v2f vert(appdata v)
			{
				v2f o;
				//float4 localpos = float4(CalcLeafPos(v.vertex.xyz, v.normal.xyz), 1);
				//o.vertex = mul(UNITY_MATRIX_MVP, localpos);
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);

				float4 veiw = mul(UNITY_MATRIX_MV, v.vertex);
				o.view_pos_nor.xyz =  mul(UNITY_MATRIX_MV, float4(v.normal, 0)).xyz;//view normal
				//o.view_pos_nor.xyz = mul(v.normal, (float3x3)UNITY_MATRIX_IT_MV).xyz;
				o.view_pos_nor.w = -veiw.z*_ProjectionParams.w;//view depth
				float3 view_dir = normalize(mul(UNITY_MATRIX_MV, v.vertex).xyz);//viewdir
				o.uv.zw = dot(normalize(o.view_pos_nor.xyz), -view_dir);//法线*视线（假装）
				o.view = view_dir;
				o.tangent = v.tangent;

				o.normal.xyz = normalize(mul(unity_ObjectToWorld, float4(v.normal, 0)).xyz);
				o.wpos = mul(unity_ObjectToWorld, v.vertex);
				float4 shadow_pos = mul(_ShadowView, o.wpos + float4(o.normal.xyz*0.07f, 0));
				float4 shadowproj = mul(_ShadowProj, shadow_pos);
				shadowproj /= shadowproj.w;
				if (abs(shadowproj.x)>1.0f || abs(shadowproj.y)>1.0f)
				{
					shadow_pos.z = 0.0f;
				}
#if SHADER_API_GLES|| SHADER_API_GLES3

#else
				o.view_pos_nor.x *= -1;
				o.view_pos_nor.z *= -1;//坐标系问题
				o.view.y *= -1;
				shadowproj.y *= -1;
#endif

				o.sview.xy = shadowproj.xy*0.5 + 0.5;
				o.sview.z = shadow_pos.z;
				o.sview.w = 0;

				return o;
			}
			fixed4 frag (v2f i) : SV_Target
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
             	float3 e =normalize(i.view);//viewspace
             	float3 diff = 1;
             	float metal =  col.w >= 0.497f;//__Metal
				float roughness =  saturate(1 - (col.w*2.0f) + metal);//1-_SmoothBase
             	float3 spec =(1 + metal * 3);//金属控制高光强度
             	 
     			 
             	PBR(roughness, -lightdir.xyz, e, normal, diff, spec,i.tangent);
             	//outspec=lightcolor*(col+metal)*outspec;
             	float3 ambient_spec = 0;   
                float3 R = reflect(-e,normal)*float3(1,1,-1); 
                R = mul(R,(float3x3)UNITY_MATRIX_V);
                float sqrt_roughness = (1-roughness);    
				float3 env = texCUBE(_EnvCube, R).xyz*sqrt_roughness*sqrt_roughness;// texCUBElod(_EnvCube, float4(R, (roughness)*5.0f)).xyz*sqrt_roughness*sqrt_roughness;
                ambient_spec = env*env*(metal+1)*ambientcolor;
				
	
				float2 shadowuv = i.sview.xy;
				float2 XY_DEPTH = float2(1.0f, 0.003921568627451)*invShadowViewport.w;
				float occ = CalcShadow3X3(shadowuv, i.sview.z, XY_DEPTH, invShadowViewport.xyz, _ShadowDepth);
             //  return float4(spec,1);
             	return float4(lightcolor.xyz*col.xyz*(diff*occ + spec) + ambient_spec, 1);//here1/2.2 willbe wrong

			}

			ENDCG 
		}
	}
}
