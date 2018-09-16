Shader "GDShader/DiffuseSSS" {

	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Main Color", Color) = (0.5,0.5,0.5,0.5)
		_SmoothBase ("_SmoothBase",Range(0,1.0))=0.0
		_Metal ("Metal",Range(0,1.0))=0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue"="Geometry"}
		LOD 200
		pass{
			Stencil
			{
				Ref 24
				Comp always
				Pass replace
				ZFail keep
			}
			ZTest LEqual
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "colorbuffer.cg"
			ENDCG
		}
	} 
	FallBack "Diffuse"
}

