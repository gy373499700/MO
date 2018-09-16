Shader "GDShader/DeferredShadowLightVolume" {
	Properties {
		//_MainTex ("Base (RGB)", 2D) = "white" {}
		_Cube("Env (RGB)", CUBE) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		//Write Depth
		pass{
			ZTest	Less
			ZWrite	Off
			Cull Front
			ColorMask RG



			CGPROGRAM

			#pragma vertex vert
			#pragma fragment depth_frag
			
			#include "ShadowLightVolume.cg"
			
			ENDCG
		}
		
		//Eye InSide
		pass {


			ZTest	Always
			ZWrite	Off
			Cull Front
			ColorMask RGB
			Blend One One
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment volume_frag
			#define EYE_IN_SHADOW 1
			#include "ShadowLightVolume.cg"


			ENDCG
		}
		

		//Eye OutSide
		pass {

			ZTest	Less
			ZWrite	Off
			Cull Off
			ColorMask RGB
			Blend One One
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment volume_frag
			
			#include "ShadowLightVolume.cg"
			ENDCG
		}
		//Eye InSide
		pass {


			ZTest	Always
				ZWrite	Off
				Cull Front
				ColorMask RGB
				Blend One One
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment volume_frag
				#define EYE_IN_SHADOW 1
				#define CAST_SHADOW 1
				#include "ShadowLightVolume.cg"
				
				ENDCG
		}


		//Eye OutSide
		pass {

			ZTest	Less
				ZWrite	Off
				//Cull Front
				ColorMask RGB
				Blend One One
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment volume_frag
				#define CAST_SHADOW 1
				#include "ShadowLightVolume.cg"



























				ENDCG
		}
	} 
	//FallBack "Diffuse"
}
