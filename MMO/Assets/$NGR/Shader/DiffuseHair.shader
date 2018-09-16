Shader "GDShader/DiffuseHair" {

	Properties {
		_MainTex ("_MainTex", 2D) = "white" {}
		_Color ("_Color", Color) = (0.5,0.5,0.5,0.5)
		_SmoothBase ("_SmoothBase",Range(0,1.0))=0.0
		_Metal ("_Metal",Range(0,1.0))=0.0
		//StencilRef("StencilRef",Int) = 0
		CullMode("CullMode",Int)=2
		_HairDetail("_HairDetail", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="OpaqueHair" "Queue"="Geometry"}
		LOD 200
		pass{
			Stencil
			{
				Ref 1
				Comp always
				Pass replace
				ZFail keep
			}
			ZTest LEqual
			Cull [CullMode]
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define HAIR_ON 1
			//	int StencilRef;
			#include "colorbuffer.cg" 
			ENDCG
		}
	} 
	FallBack "Diffuse"
}

