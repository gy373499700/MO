Shader "GDShader/DiffuseBumpOIT" {

	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SpecTex ("Base (RGB)", 2D) = "black" {}
		_Color ("Main Color", Color) = (0.5,0.5,0.5,0.5)
		_SmoothBase ("_SmoothBase",Range(0,1.0))=0.0
		_Smoothness ("_Smoothness",Range(0,1.0))=0.0
		_Metal ("Metal",Range(0,1.0))=0.0
		_BumpScale("_BumpScale",Range(0,1.0))=1.0
	}
	SubShader {
		Tags { "RenderType"="BumpOIT" "Queue"="Geometry+50"}
		LOD 200
		pass{
			Stencil
			{
				Ref 0
				Comp always
				Pass replace
				ZFail keep
			}
			ZTest LEqual
			Cull Off
			//blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define OIT 1
			#define HAS_BUMP 1
			#include "colorbuffer.cg"
			ENDCG
		}
		
	} 
	FallBack "Diffuse"
}


