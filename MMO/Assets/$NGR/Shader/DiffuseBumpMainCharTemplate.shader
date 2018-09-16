Shader "GDShader/DiffuseBumpMainCharTemplate" {
	Properties {
		_MainTex ("_MainTex", 2D) = "white" {}
		_SpecTex ("_SpecTex", 2D) = "black" {}
		_Color ("_Color", Color) = (1,1,1,1)
		_SmoothBase ("_SmoothBase",Range(0,1.0))=0.0
		_Smoothness ("_Smoothness",Range(0,1.0))=0.0
		_Metal ("_Metal",Range(0,1.0))=0.0
		_BumpScale("_BumpScale",Range(0,1.0))=1.0
		StencilRef("StencilRef",Int) = 0
		CullMode("CullMode",Int) = 2

	}
	SubShader {
		Tags { "RenderType"="OpaqueBump" "Tag" = "MainChar" "MainCharTag"="MainChar" }
		LOD 200
		
		pass{
			Stencil
			{
				Ref [StencilRef]
				Comp always
				Pass replace
				ZFail keep
			}
			ZTest LEqual
			Cull [CullMode]
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define HAS_BUMP 1
			#include "colorbuffer.cg"
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
