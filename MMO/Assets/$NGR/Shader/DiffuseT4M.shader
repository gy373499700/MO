
Shader "GDShader/DiffuseT4M" {
	Properties{
	_Splat0("Layer 1", 2D) = "white" {}
	_Splat1("Layer 2", 2D) = "white" {}
	_Splat2("Layer 3", 2D) = "white" {}
	_Splat3("Layer 4", 2D) = "white" {}
	_Control("Control (RGBA)", 2D) = "white" {}
	_MainTex("Never Used", 2D) = "white" {}
	_Color("Main Color", Color) = (0.5,0.5,0.5,0.5)
	_SmoothBase("_SmoothBase",Range(0,1.0)) = 0.0
	_Metal("Metal",Range(0,1.0)) = 0.0
	}
		SubShader{
		Tags{ "RenderType" = "OpaqueT4M" "Queue" = "Geometry+55" }
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
			#define T4M_AO 0

			#include "T4M.cg"

			ENDCG
	}
	}
		FallBack "Diffuse"
}
	
