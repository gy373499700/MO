Shader "GDShader/SSSSS_HightLight" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Diffuse ("_Diffuse (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		pass{
			Stencil
			{
				Ref 0
				ReadMask 5
				Comp Equal
				Pass keep
			}
			ZTest Greater
			ZWrite Off
			ColorMask RGB
			Blend One One
			Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#define LIGHT_COUNT 1
			#include "PointLightSpecular.cg"
			ENDCG
		}
	pass {
		Stencil
		{
			Ref 0
			ReadMask 5
			Comp Equal
			Pass keep
		}
			ZTest Greater
			ZWrite Off
			ColorMask RGB
			Blend One One
			Cull Off
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 3.0
#define LIGHT_COUNT 2
#include "PointLightSpecular.cg"
			ENDCG
	}
	pass {
		Stencil
		{
			Ref 0
			ReadMask 5
			Comp Equal
			Pass keep
		}
			ZTest Greater
			ZWrite Off
			ColorMask RGB
			Blend One One
			Cull Off
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 3.0
#define LIGHT_COUNT 3
#include "PointLightSpecular.cg"
			ENDCG
	}
	pass {
		Stencil
		{
			Ref 0
			ReadMask 5
			Comp Equal
			Pass keep
		}
			ZTest Greater
			ZWrite Off
			ColorMask RGB
			Blend One One
			Cull Off
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 3.0
#define LIGHT_COUNT 4
#include "PointLightSpecular.cg"
			ENDCG
	}
	pass {
		Stencil
		{
			Ref 0
			ReadMask 5
			Comp Equal
			Pass keep
		}
			ZTest Greater
			ZWrite Off
			ColorMask RGB
			Blend One One
			Cull Off
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 3.0
#define LIGHT_COUNT 5
#include "PointLightSpecular.cg"
			ENDCG
	}
	pass {
		Stencil
		{
			Ref 0
			ReadMask 5
			Comp Equal
			Pass keep
		}
			ZTest Greater
			ZWrite Off
			ColorMask RGB
			Blend One One
			Cull Off
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 3.0
#define LIGHT_COUNT 6
#include "PointLightSpecular.cg"
			ENDCG
	}
		pass {
			Stencil
			{
				Ref 1
				ReadMask 1
				Comp Equal
				Pass keep
			}
				ZTest Greater
				ZWrite Off
				ColorMask RGB
				Blend One One
				Cull Off
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0
				#define HAIR_ON 1
				#include "PointLightSpecular.cg"



				ENDCG
		}
	} 
	//FallBack "Diffuse"
}
