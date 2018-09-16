Shader "GDShader/DiffuseColorTemplateSelfIllum" {

	Properties {
		_MainTex ("_MainTex", 2D) = "white" {}
		_Color ("_Color", Color) = (0.5,0.5,0.5,0.5)
		_SmoothBase ("_SmoothBase",Range(0,1.0))=0.0
		_Metal ("_Metal",Range(0,1.0))=0.0
		StencilRef("StencilRef",Int) = 0
		CullMode("CullMode",Int)=2
		_PainterTex("_PainterTex", 2D) = "black" {}
		_PaintColor0("_PaintColor0", Color) = (0.5,0.5,0.5,0.5)
		_PaintColor1("_PaintColor1", Color) = (0.5,0.5,0.5,0.5)
		_UVMove("_UVMove", Vector) = (0,0,0,0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderTag"="SelfIllum"}
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

			//#define COLOR_PAINTER 1 
			#include "colorbuffer.cg"  

			ENDCG
		}
	} 
	FallBack "Diffuse"
}

