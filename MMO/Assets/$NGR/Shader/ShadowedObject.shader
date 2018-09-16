


Shader "GDShader/TransparentTemplate" {

	Properties {
		_MainTex ("_MainTex", 2D) = "white" {}
		_Color ("_Color", Color) = (0.5,0.5,0.5,0.5)
		_SmoothBase ("_SmoothBase",Range(0,1.0))=0.0
		_Metal ("_Metal",Range(0,1.0))=0.0
		_SpecColor("_SpecColor", Color) = (0.5,0.5,0.5,0.5)
		_Emissive("_SpecColor", Color) = (0,0,0,0)
		CullMode("CullMode",Int)=2
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 200
		pass {
			ZTest Less
			ColorMask Off
				Cull[CullMode]
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag_null
								//	int StencilRef;
				#include "shadowedobject.cg"

				ENDCG
		}
		pass{

			ZTest Equal
			
			Blend SrcAlpha OneMinusSrcAlpha
			Cull [CullMode]
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//	int StencilRef;
			#include "shadowedobject.cg"

			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}


