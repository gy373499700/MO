Shader "GDShader/DeferredShadowLight" {
	Properties {
		//_MainTex ("Base (RGB)", 2D) = "white" {}
		_Cube("Env (RGB)", CUBE) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		//Off 0 
		pass{



			ZTest	Greater
			ZWrite	Off
			Cull Front 
			ColorMask RGB
			Blend One One
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			
			#include "ShadowLight.cg"

			
			ENDCG
		}
		//Low 1
		pass {

				ZTest	Greater
				ZWrite	Off
				Cull Front
				ColorMask RGB
				Blend One One
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag
				#define CAST_SHADOW 1
				#include "ShadowLight.cg"
				
				ENDCG
			}
		//Middle 2
		pass {


			ZTest	Greater
			ZWrite	Off
			Cull Front
			ColorMask RGB
			Blend One One
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#define CAST_SHADOW 1
			#include "ShadowLight.cg"


			ENDCG
		}
		//High 3
		pass {


			ZTest	Greater
			ZWrite	Off
			Cull Front
			ColorMask RGB
			Blend One One
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#define CAST_SHADOW 1
			#include "ShadowLight.cg"

			ENDCG
		}
		//mask stencil 4
		pass {

			Stencil
			{
				Ref 128
				ReadMask 128
				WriteMask 128
				Comp notequal
				Pass replace
				Fail zero
				ZFail zero
			}
			ZTest	Greater
			ZWrite	Off
			Cull Front
			ColorMask Off
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment mask_frag

			#include "ShadowLight.cg"


			ENDCG
		}
		

		//Off 5
		pass {
			Stencil
			{
				Ref 128
				ReadMask 128
				WriteMask 128
				Comp equal
				Pass zero
				Fail zero
				ZFail zero
			}

			ZTest	Less
			ZWrite	Off
			
			ColorMask RGB
			Blend One One
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			
			#include "ShadowLight.cg"

			ENDCG
		}
		//Low 6
		pass {
			Stencil
			{
				Ref 128
				ReadMask 128
				WriteMask 128
				Comp equal
				Pass zero
				Fail zero
				ZFail zero
			}
			ZTest	Less
			ZWrite	Off
			
			ColorMask RGB
			Blend One One
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#define CAST_SHADOW 1
			#include "ShadowLight.cg"

			ENDCG
		}
		//Middle 7
		pass {
			Stencil
			{
				Ref 128
				ReadMask 128
				WriteMask 128
				Comp equal
				Pass zero
				Fail zero
				ZFail zero
			}
				ZTest	Less
				ZWrite	Off
				
				ColorMask RGB
				Blend One One
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag
				#define CAST_SHADOW 1
				#include "ShadowLight.cg"

				ENDCG
		}
		//High 8
		pass {
			Stencil
			{
				Ref 128
				ReadMask 128
				WriteMask 128
				Comp equal
				Pass zero
				Fail zero
				ZFail zero
			}
				ZTest	Less
				ZWrite	Off
				
				ColorMask RGB
				Blend One One
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag
				#define CAST_SHADOW 1
				#include "ShadowLight.cg"





				ENDCG
		}
	} 
	//FallBack "Diffuse"
}
