Shader "Hidden/GDShader/SSSSS" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Diffuse ("_Diffuse (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		pass{
			Stencil
			{
				Ref 8
				ReadMask 8
				Comp equal
				Pass keep
			}
			ZWrite Off
			ZTest Greater
			ColorMask RGB
			Blend One One
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "LightCommon.cg"
			#define SAMPLER_NUM 12

			sampler2D	_Depth;
			sampler2D	_Sample2x2;
			sampler2D	_Diffuse;
			sampler2D   _Random;
			sampler2D   _LastFrame;
			float4		_SSSColor;
			float4		_FarCorner;
			float4 		RimLightColorHigh;
			float4 		RimLightColorLow;
			
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 viewpos  : TEXCOORD1;
			};

			v2f vert (appdata_full v)
			{
				v2f o;
#if SHADER_API_GLES|| SHADER_API_GLES3
				//Mobile
				o.pos = float4(v.vertex.x*2,v.vertex.y*2,1,1);
				o.uv = o.pos*0.5f+0.5f;
				o.viewpos = float4(o.pos.xy,1.0f,1)*_FarCorner;
#else
				//Editor Only 
				o.pos = float4(v.vertex.x*2,-v.vertex.y*2,1,1);
				o.uv = o.pos*0.5f+0.5f;
				o.uv.y = 1-o.uv.y;
				o.viewpos = float4(o.pos.xy,1.0f,1)*_FarCorner;
				o.viewpos.y*=-1;
#endif
				
				return o;
			}


			float4 invViewport_Radius;
			



			const static float2 sample_kernel[12] = {//某点发散出去的螺旋点，用来找到改点周围任意不重复相交的点群，用来模拟光线发散出去后的内部散射

				float2(1.468021,-0.443222),
				float2(-1.277367,-1.922591),
				float2(-1.868983,2.259212),
				float2(3.203003,1.346417),
				float2(0.4142855,-3.941687),
				float2(-4.33534,0.8270095),
				float2(2.239452,4.283678),
				float2(3.736661,-3.659216),
				float2(-4.912866,-2.700863),
				float2(-1.240371,5.835525),
				float2(6.28885,-0.5280988),
				float2(-2.445607,-6.176906),
			};

			float4 frag( v2f IN) : COLOR
			{
			   float2 halfPixel   =   0.5f*invViewport_Radius.xy;
			   float2   uv=   IN.uv;//+halfPixel;
			   //float2   xy=   uv*temp.xy+temp.zw;
			   float4   rot   =  tex2D(_Sample2x2,IN.uv*0.25f/invViewport_Radius.xy)*2-1;
			   float4   rand   =  tex2D(_Random,IN.uv/256/invViewport_Radius.xy);
   
			   float2x2 rot_2x2 = float2x2(rot.xyzw);//用来把改点做一个旋转，使多次采样的点的扩散旋转都不重复

			   //float3 ViewDir   =   float3(viewPos.xy,1000);
			   float2 XY_Depth = float2(1.0f,0.003921568627451);

			   float2 sample_uv = uv+(rand.zw-0.5f)*invViewport_Radius.xy;//对uv进行一点点随机偏移

			   float4 depth_normal = tex2D(_Depth,uv);
			   float view_depth = dot(depth_normal.xy,XY_Depth);

			   float3 viewpos = IN.viewpos*view_depth;
			  
			   float3 normal = DecodeNormal(depth_normal.zw);
			   
			   //return float4(normal,0);
			   //normal = normalize(normal);

			   float fix = (1.0f-view_depth/invViewport_Radius.w)*20.0f;//*invViewport_Radius.z;//+0.1f;


   
			   float4   tempUV;
			   //float2   tempXY;
   
			   float4 occ=0;//计算周围点对 当前点的亮度贡献值
			   tempUV.zw   =   invViewport_Radius.xy*(0.75+0.5*rand.x)*invViewport_Radius.z;//采样半径
			   float2 off[4];
			   float4 UV[2];
			   float4 _DEPTH[2];
			   float4 dot_val = 0;
			   //[loop]
			   for(int i=0;i<SAMPLER_NUM;i+=4){
				  off[0] = sample_kernel[i];
				  off[1] = sample_kernel[i+1];  
				  off[2] = sample_kernel[i+2];
				  off[3] = sample_kernel[i+3];
				  UV[0].xy     =   mul(rot_2x2,off[0].xy);//*tempUV.zw+texCoord;
				  UV[0].zw     =   mul(rot_2x2,off[1].xy);////加旋转为了使相连的点的采样点整体旋转，保证各个点的采样点不会重复。
				  UV[1].xy     =   mul(rot_2x2,off[2].xy);//*tempUV.zw+texCoord;
				  UV[1].zw     =   mul(rot_2x2,off[3].xy);//做旋转使不重复
      
				  UV[0]*=tempUV.zwzw;
				  UV[1]*=tempUV.zwzw;
      
				  UV[0]+=sample_uv.xyxy;//随机偏移一点点
				  UV[1]+=sample_uv.xyxy;
      
				  //_DEPTH[0].xy = tex2D(_Depth,UV[0].xy).xy;
				  //_DEPTH[0].zw = tex2D(_Depth,UV[0].zw).xy;
				  //_DEPTH[1].xy = tex2D(_Depth,UV[1].xy).xy;
				  //_DEPTH[1].zw = tex2D(_Depth,UV[1].zw).xy;
      			  //
				  //dot_val.x =   dot(_DEPTH[0].xy,XY_Depth.xy);
				  //dot_val.y =   dot(_DEPTH[0].zw,XY_Depth.xy);
				  //dot_val.z =   dot(_DEPTH[1].xy,XY_Depth.xy);
				  //dot_val.w =   dot(_DEPTH[1].zw,XY_Depth.xy);
      
				  dot_val = 1.0f/ SAMPLER_NUM;// abs(1 / float4(off[0].z, off[1].z, off[2].z, off[3].z));

				  occ += dot_val.x*tex2D(_LastFrame,UV[0].xy);//和lastframe进行叠加，效果才会慢慢变好。
				  occ += dot_val.y*tex2D(_LastFrame,UV[0].zw);
				  occ += dot_val.z*tex2D(_LastFrame,UV[1].xy);
				  occ += dot_val.w*tex2D(_LastFrame,UV[1].zw);
			   }
			   float4 diff = tex2D(_Diffuse,IN.uv);

			   float4 color = occ*_SSSColor*diff;
			   return float4(color.xyz*2.0f, diff.w);//*_SSSColor;//pow(acos(-fAO)/3.14f,0.5f);///4;
			}
			ENDCG
		}
	} 
	//FallBack "Diffuse"
}
