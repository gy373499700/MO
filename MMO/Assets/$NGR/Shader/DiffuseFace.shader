Shader "GDShader/DiffuseFace"
{
	Properties
	{
		_MainTex ("_MainTex", 2D) = "white" {}
		_EyeTex("_EyeTex", 2D) = "white" {}
		_MouseTex("_MouseTex", 2D) = "white" {}
		_Color("_Color", Color) = (0.5,0.5,0.5,0.5)
		_SmoothBase("_SmoothBase",Range(0,1.0)) = 0.0
		_Metal("_Metal",Range(0,1.0)) = 0.0
		StencilRef("StencilRef",Int) = 0
		_LeftEyeTex_Src("_LeftEyeTex_Src", Vector) = (0.5,0.5,0.5,0.5)
		_RightEyeTex_Src("_RightEyeTex_Src", Vector) = (0.5,0.5,0.5,0.5)
		_MouseTex_Src("_MouseTex_Src", Vector) = (0.5,0.5,0.5,0.5)
		_LeftEyeTex_ST_Dest("_LeftEyeTex_ST_Dest", Vector) = (0.5,0.5,0.5,0.5)
		//_RightEyeTex_ST_Dest("_RightEyeTex_ST_Dest", Vector) = (0.5,0.5,0.5,0.5)
		_MouseTex_ST_Dest("_MouseTex_ST_Dest", Vector) = (0.5,0.5,0.5,0.5)
	}
	SubShader
	{
		Tags { "RenderType"="OpaqueFace" "Tag" = "OtherChar" }
		LOD 100

		Pass
		{
			Stencil
			{
				Ref [StencilRef]
				Comp always
				Pass replace
				ZFail keep
			}
			ZTest LEqual

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			//#pragma multi_compile_fog


			#include "DiffuseFace.cg"




			
			ENDCG
		}
	}
}
