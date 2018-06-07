Shader "GDShader/DiffuseOITGrass" {

	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Main Color", Color) = (0.5,0.5,0.5,0.5)
		_SmoothBase ("_SmoothBase",Range(0,1.0))=0.0
		_Metal ("Metal",Range(0,1.0))=0.0
		_GrassStrength("_GrassStrength",Range(0,2.0)) = 1.0
		StencilRef("StencilRef",Int) = 0
		CullMode("CullMode",Int) = 2
	}
	SubShader {
		Tags { "RenderType"="OITGrass" "Queue"="Geometry+60"}
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
			Cull[CullMode]
			//Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature Wind_On 
			#define ALPHA_TEST 1
			//#define OIT 1
			#define FORCE_FIELD 1
			#define VERTEX_COLOR 1
			#pragma target 3.0
			#include "colorbuffer.cg"

			ENDCG
		}
		
	} 
	FallBack "Diffuse"
}


