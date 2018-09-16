Shader "Hidden/GDShader/GBuffer" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue" = "Geometry"  }
		LOD 200
		pass{
			ZTest Less
			Cull[CullMode]
			Stencil
			{
				Ref[StencilRef]
				Comp always
				Pass replace
				ZFail keep
			}
			CGPROGRAM
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			#include "gbuffer.cg"



			ENDCG
		}
	} 
		SubShader{
		Tags{ "RenderType" = "OpaquePainter" "Queue" = "Geometry" }
		LOD 200
		pass {
		ZTest Less
			Cull[CullMode]
			Stencil
		{
			Ref[StencilRef]
			Comp always
			Pass replace
			ZFail keep
		}
			CGPROGRAM
#pragma vertex gbuffer_vert
#pragma fragment gbuffer_frag
#include "gbuffer.cg"



			ENDCG
	}
	}
	SubShader{
		Tags{ "RenderType" = "OpaqueLeaf" "Queue" = "Geometry" }
		LOD 200
		pass {
		ZTest Less
			Cull[CullMode]
			Stencil
		{
			Ref[StencilRef]
			Comp always
			Pass replace
			ZFail keep
		}
			CGPROGRAM
			#define TREE_LEAF 1
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			#include "gbuffer.cg"
				ENDCG
		}
	}
	SubShader {
		Tags { "RenderType"="OpaqueBump" "Queue" = "Geometry+10" }
		LOD 200
		pass{
			ZTest Less
			Cull[CullMode]
			Stencil
			{
				Ref[StencilRef]
				Comp always
				Pass replace
				ZFail keep
			}
			CGPROGRAM
			#define HAS_BUMP 1
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			//#define SHADER_API_GLES
			#include "UnityCG.cginc"
			#include "gbuffer.cg"



			ENDCG
		}
	} 
	SubShader{
		Tags{ "RenderType" = "OpaqueBumpPainter" "Queue" = "Geometry+10" }
		LOD 200
		pass {
		ZTest Less
			Cull[CullMode]
			Stencil
			{
				Ref[StencilRef]
				Comp always
				Pass replace
				ZFail keep
			}
			CGPROGRAM
			#define HAS_BUMP 1
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
						//#define SHADER_API_GLES
			#include "UnityCG.cginc"
			#include "gbuffer.cg"



			ENDCG
	}
	}
	SubShader {
		Tags { "RenderType"="AlphaTest" "Queue" = "Geometry+60" }
		LOD 200
		pass{
			ZTest Less
			Cull[CullMode]
			Stencil
			{
				Ref[StencilRef]
				Comp always
				Pass replace
				ZFail keep
			}
			CGPROGRAM
			#define ALPHA_TEST 1
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			//#define SHADER_API_GLES
			#include "gbuffer.cg"
			

			ENDCG
		}
	} 
		SubShader{
		Tags{ "RenderType" = "AlphaTestDisAppear" "Queue" = "Geometry+61" }
		LOD 200
		pass {
		ZTest Less
		Cull[CullMode]
		Stencil
		{
			Ref[StencilRef]
			Comp always
			Pass replace
			ZFail keep
		}
		Stencil
		{
			Ref[StencilRef]
			Comp always
			Pass replace
			ZFail keep
		}
			CGPROGRAM
		#define ALPHA_TEST 1
		#define ALPHA_DisAppear 1
		#pragma vertex gbuffer_vert
		#pragma fragment gbuffer_frag
					//#define SHADER_API_GLES
		#include "gbuffer.cg"


			ENDCG
		}
	}

	SubShader {
		Tags { "RenderType"="BumpAlphaTest" "Queue" = "Geometry+62" }
		LOD 200
		pass{
			ZTest Less
			Cull Off
			CGPROGRAM
			#define ALPHA_TEST 1
			#define HAS_BUMP 1
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			//#define SHADER_API_GLES
			#include "gbuffer.cg"
			

			ENDCG
		}
	} 
	SubShader {
		Tags { "RenderType"="BumpOIT" "Queue" = "Geometry+64" }
		LOD 200
		pass{
			ZTest Less
			Cull Off
			CGPROGRAM
			#define OIT 1
			#define HAS_BUMP 1

			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			//#define SHADER_API_GLES
			#include "gbuffer.cg"
			
			ENDCG
		}
	} 
	SubShader {
		Tags { "RenderType"="OIT" "Queue" = "Geometry+63" }
		LOD 200
		pass{
			ZTest Less
			Cull Off
			CGPROGRAM
			#define OIT 1


			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			//#define SHADER_API_GLES
			#include "gbuffer.cg"
			
			ENDCG
		}
	} 
	SubShader{
		Tags{ "RenderType" = "OITGrass" "Queue" = "Geometry+65" }
		LOD 200
		pass {
		Stencil
		{
			Ref[StencilRef]
			Comp always
			Pass replace
			ZFail keep
		}
		ZTest Less
			Cull[CullMode]
			CGPROGRAM
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag

			#define ALPHA_TEST 1
			//#define OIT 1
			#define FORCE_FIELD 1
			#pragma target 3.0


			#include "gbuffer.cg"


			ENDCG
		}
	}
	SubShader{
		Tags{ "RenderType" = "OpaqueT4M"  "Queue" = "Geometry+55" }
		LOD 200
		pass {
		ZTest Less
		Stencil
		{
			Ref 0
			Comp always
			Pass replace
			ZFail keep
		}
		CGPROGRAM
		#pragma vertex gbuffer_vert
		#pragma fragment gbuffer_frag
		#include "gbuffer.cg"
		ENDCG
		}
	}
	SubShader{
		Tags{ "RenderType" = "OpaqueT4MAO" "Queue" = "Geometry+56" }
		LOD 200
		pass {
		ZTest Less
		Stencil
		{
			Ref 0
			Comp always
			Pass replace
			ZFail keep
		}
			CGPROGRAM
		#pragma vertex gbuffer_vert
		#pragma fragment gbuffer_frag
		#include "gbuffer.cg"
		ENDCG
			}
	}
		SubShader{
		Tags{ "RenderType" = "OpaqueT4MNormal" "Queue" = "Geometry+56" }
		LOD 200
		pass {
		ZTest Less
			Stencil
		{
			Ref 0
			Comp always
			Pass replace
			ZFail keep
		}
			CGPROGRAM
		#pragma vertex mrt_vert
		#pragma fragment gbuffer_frag

		#define	T4M_AO 2
		#define T4M_NORMAL 1
		#include "T4M.cg"



					ENDCG
			}
	}
	SubShader{
		Tags{ "RenderType" = "OpaqueFace" "Queue" = "Geometry+50" }
		LOD 200
		pass {
		ZTest Less
		Stencil
		{
			Ref[StencilRef]
			Comp always
			Pass replace
			ZFail keep
		}
			CGPROGRAM
		#pragma vertex gbuffer_vert
		#pragma fragment gbuffer_frag
		#include "gbuffer.cg"
			ENDCG
		}
	}
	SubShader{
		Tags{ "RenderType" = "OpaqueHair" "Queue" = "Geometry" }
		LOD 200
		pass {
			Stencil
			{
				Ref 1
				Comp always
				Pass replace
				ZFail keep
			}

			ZTest LEqual
			Cull[CullMode]
			CGPROGRAM
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			#define HAIR_ON 1
			#include "gbuffer.cg"

			ENDCG
		}
	}
	FallBack "Diffuse"
}
