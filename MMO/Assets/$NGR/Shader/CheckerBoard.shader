Shader "Hidden/GDShader/CheckerBoard" {

		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		pass {
			ZTest Always
			ColorMask Off
			Cull Off
			CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag  

#include "CheckerBoard.cg"                         


			ENDCG
				}
	}
}