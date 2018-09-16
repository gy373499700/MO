Shader "Hidden/GDShader/SelfIllumGlow" {

	Properties{
		//_MainTex("_MainTex", 2D) = "white" {}
		//_Color("_Color", Color) = (0.5,0.5,0.5,0.5)
		//_SmoothBase("_SmoothBase",Range(0,1.0)) = 0.0
		//_Metal("_Metal",Range(0,1.0)) = 0.0
		//StencilRef("StencilRef",Int) = 0
		////CullMode("CullMode",Int) = 2
		//_PainterTex("_PainterTex", 2D) = "black" {}
		//_PaintColor0("_PaintColor0", Color) = (0.5,0.5,0.5,0.5)
		//_PaintColor1("_PaintColor1", Color) = (0.5,0.5,0.5,0.5)
		//_UVMove("_UVMove", Vector) = (0,0,0,0)
		//
		//_AnisoSpecTex("_AnisoSpecTex", 2D) = "gray"{}  
		//_AnisoSpecColor("_AnisoSpecColor", Color) = (1,1,1,1)  
		//_AnisoSpecularMultiplier("_AnisoSpecularMultiplier", float) = 1.0
		//_AnisoPrimaryShift("_AnisoPrimaryShift", float) = 0.5 
	}  
		SubShader{
		Tags{ "RenderTag" = "SelfIllum" }
		LOD 200
		pass {
			 
			ZTest LEqual
			ZWrite Off
			Blend One One
			Cull[CullMode]
			CGPROGRAM
			#pragma vertex gbuffer_vert
			#pragma fragment selfillum_frag 
				 
			//	int StencilRef;

			#define COLOR_PAINTER 1 
			#include "SelfIllum.cg"			                    

			ENDCG
	}
	}

	SubShader{
		Tags{ "RenderTag" = "Hair" }
		LOD 200
		pass {
		Stencil   
		{
			Ref[StencilRef]
			Comp always
			Pass replace        
			ZFail keep
		}  
			ZTest LEqual     
			
			Blend One One
			Cull[CullMode]
			CGPROGRAM
#pragma vertex aniso_vert 
#pragma fragment aniso_frag          
#define HAS_BUMP 1   
#include "HairAnisoSpec.cg"   
			ENDCG
	}  
	}
		FallBack "Diffuse"    
}