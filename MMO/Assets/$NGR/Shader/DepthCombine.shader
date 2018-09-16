Shader "Hidden/GDShader/DepthCombine" {
	Properties{
		_StaticDepth("_StaticDepth", 2D) = "white" {}
		_DynamicDepth("_DynamicDepth", 2D) = "white" {}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		//0 Copy Static Depth
		pass {
			ZWrite Off
			ZTest LEqual
			Cull Off
			ColorMask RG
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_clear
			#include "DepthCombine.cg"
			ENDCG

		}
		//1 Combine Static And Dynamic Depth
		pass {
			ZWrite Off
			ZTest Greater
			ColorMask RG
			Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "DepthCombine.cg" 
			ENDCG

		}
	}
}
