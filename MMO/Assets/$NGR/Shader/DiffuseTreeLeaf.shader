Shader "GDShader/DiffuseTreeLeaf" {

	Properties {
		_MainTex ("_MainTex", 2D) = "white" {}
		_Color ("_Color", Color) = (0.5,0.5,0.5,0.5)
		_SmoothBase ("_SmoothBase",Range(0,1.0))=0.0
		_Metal ("_Metal",Range(0,1.0))=0.0
		StencilRef("StencilRef",Int) = 0
		CullMode("CullMode",Int)=2
		_GrassStrength("_GrassStrength",Range(0,2.0)) = 1.0
		_GrassFreq("_GrassFreq",Range(1.0,10.0)) = 3.0
	}
	SubShader {
		Tags { "RenderType"="OpaqueLeaf" "Queue"="Geometry"}
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

			//	int StencilRef;
			#define TREE_LEAF 1
			#include "colorbuffer.cg"
			ENDCG
		}
	} 
	FallBack "Diffuse"
}

