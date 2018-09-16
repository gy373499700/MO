Shader "GDShader/DiffuseAlphaDisappearTemplate" {

	Properties {
		_MainTex ("_MainTex", 2D) = "white" {}
		_NoiseTex("_NoiseTex", 2D) = "white" {}
		_Color ("_Color", Color) = (0.5,0.5,0.5,0.5)
		_SmoothBase ("_SmoothBase",Range(0,1.0))=0.0
		_Metal ("_Metal",Range(0,1.0))=0.0
		StencilRef("StencilRef",Int) = 0
		CullMode("CullMode",Int)=2
		_Disappear("_Disappear",Range(0,1.0))=1.0
		_DisappearColor("_DisappearColor", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="AlphaTestDisAppear" "Queue"="Geometry+61"}
		LOD 200
		pass{
			Stencil
			{
				Ref[StencilRef]
				Comp always
				Pass replace
				ZFail keep
			}
			ZTest LEqual
			Cull [CullMode]
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
		
			#define ALPHA_TEST 1
			#define ALPHA_DisAppear 1
			#include "colorbuffer.cg"
		
		
			ENDCG
		}
	} 
	FallBack "Diffuse"
}

