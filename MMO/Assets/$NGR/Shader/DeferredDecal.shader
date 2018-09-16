Shader "GDShader/DeferredDecal" {
	Properties {
		//_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Decal" }
		LOD 200
		pass{
			ZTest Greater
			ZWrite Off
			ColorMask RGB
			Cull Front
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#define OUTPUT_DIFFUSE 1
			#pragma vertex vert
			#pragma fragment frag
			//#define SHADER_API_GLES
			#include "Decal.cg"
			  


			ENDCG
		}
		pass{
			ZTest Greater
			ZWrite Off
			ColorMask A
			Cull Front
			Blend One One


			CGPROGRAM
			#define OUTPUT_ROUGHNESS 1
			#pragma vertex vert
			#pragma fragment frag
			//#define SHADER_API_GLES
			#include "Decal.cg"
			


			ENDCG
		}
	} 
	FallBack "Diffuse"
}
