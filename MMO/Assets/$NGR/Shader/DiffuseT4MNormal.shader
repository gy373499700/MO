
Shader "GDShader/DiffuseT4MNormal" {
	Properties{
	_Splat0("Layer 1", 2D) = "white" {}
	_Normal0("_Normal 1", 2D) = "white" {}
	_Color0("Layer Color1", Color) = (0.5,0.5,0.5,0.5)
	_Splat1("Layer 2", 2D) = "white" {}
	_Normal1("_Normal 2", 2D) = "white" {}
	_Color1("Layer Color2", Color) = (0.5,0.5,0.5,0.5)
	_Splat2("Layer 3", 2D) = "white" {}
	_Normal2("_Normal 3", 2D) = "white" {}
	_Color2("Layer Color3", Color) = (0.5,0.5,0.5,0.5)
	_Splat3("Layer 4", 2D) = "white" {}
	_Normal3("_Normal 4", 2D) = "white" {}
	_Color3("Layer Color4", Color) = (0.5,0.5,0.5,0.5)

	_Control("Control (RGBA)", 2D) = "white" {}
	_ColorControl("AO_Wetness", 2D) = "white" {}
	_Color("Main Color", Color) = (0.5,0.5,0.5,0.5)
	_SmoothBase("_SmoothBase",Range(0,1.0)) = 0.0
	_Metal("Metal",Range(0,1.0)) = 0.0
	}
		SubShader{
		Tags{ "RenderType" = "OpaqueT4MNormal" "Queue" = "Geometry+55" }
		LOD 200
		pass {
		Stencil
		{
			Ref 0
			Comp always
			Pass replace
			ZFail keep
		}
			ZTest LEqual

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define T4M_AO 2
			#define T4M_NORMAL 1
			#define DEBUG_AO 1
			#include "T4M.cg"





			ENDCG
	}
	}
		FallBack "Diffuse"
}
	
