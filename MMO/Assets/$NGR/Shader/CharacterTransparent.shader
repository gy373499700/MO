Shader "GDShader/CharacterTransparent" {
	Properties{
		_MainTex("_MainTex", 2D) = "white" {}
		_SpecTex("_SpecTex", 2D) = "black" {}
		_Color("_Color", Color) = (1,1,1,1)
		_SmoothBase("_SmoothBase",Range(0,1.0)) = 0.0
		_Smoothness("_Smoothness",Range(0,1.0)) = 0.0
		_Metal("_Metal",Range(0,1.0)) = 0.0
		_BumpScale("_BumpScale",Range(0,1.0)) = 1.0
		StencilRef("StencilRef",Int) = 0
		CullMode("CullMode",Int) = 2

	}
		SubShader{
		Tags{ "RenderType" = "OpaqueBump"  "Tag"="MainChar" "MainCharTag" = "MainChar" }
		LOD 200
		Pass
		{
			Stencil
			{
				Ref 64
				Comp always
				Pass replace
				ZFail keep
			}
			//Blend SrcAlpha One
			Zwrite Off
			ZTest LEqual//reater
			Cull Off
			ColorMask Off

			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "CharacterTransparent.cg"
			ENDCG
		}
		Pass
		{
			Stencil
			{
				Ref 64
				Comp NotEqual
				Pass keep
				ZFail keep
			}
			Blend One One
			Zwrite Off
			ZTest GEqual//reater
			Cull Off

			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
			// make fog work

#include "UnityCG.cginc"
#include "CharacterTransparent.cg"
			ENDCG
		}

	}
		SubShader{
			Tags{ "RenderType" = "OpaqueBump"  "Tag" = "OtherChar" }
			LOD 200
			Pass
		{
			Stencil
		{
			Ref 64
			Comp always
			Pass replace
			ZFail keep
		}
			//Blend SrcAlpha One
			Zwrite Off
			ZTest LEqual//reater
			Cull Off
			ColorMask Off

			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "CharacterTransparent.cg"
			ENDCG
		}


		}
		FallBack "Diffuse"
}
