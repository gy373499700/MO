 Shader "Hidden/GDShader/Forward" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		pass{
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
			#include "forward.cg"  

			    
			ENDCG           
		}    
	} 
	SubShader{
		Tags{ "RenderType" = "OpaquePainter" }
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
			#define COLOR_PAINTER 1
			#include "forward.cg"



			ENDCG
	}
	}
	SubShader{
		Tags{ "RenderType" = "OpaqueLeaf" }
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
			#define TREE_LEAF 1
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			#include "forward.cg"


			ENDCG
		}
	}
	SubShader {
		Tags { "RenderType"="OpaqueBump" }
		LOD 200
		pass{
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
			#define HAS_BUMP 1
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			//#define SHADER_API_GLES
			#include "UnityCG.cginc"
			#include "forward.cg"


			ENDCG
		}
	} 
	SubShader{
		Tags{ "RenderType" = "OpaqueBumpPainter" }
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
			#define HAS_BUMP 1
			#define COLOR_PAINTER 1
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
						//#define SHADER_API_GLES
			#include "UnityCG.cginc"
			#include "forward.cg"


			  
			ENDCG
	}
	}
	SubShader {
		Tags { "RenderType"="AlphaTest" }
		LOD 200
		pass{
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
			#define ALPHA_TEST 1
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			//#define SHADER_API_GLES
			#include "forward.cg"
			

			ENDCG
		}
	} 
		SubShader{
		Tags{ "RenderType" = "AlphaTestDisAppear" }
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
		#define ALPHA_TEST 1
		#define ALPHA_DisAppear 1
		#pragma vertex gbuffer_vert
		#pragma fragment gbuffer_frag
					//#define SHADER_API_GLES
		#include "forward.cg"


			ENDCG
	}
	}
	SubShader {
		Tags { "RenderType"="BumpAlphaTest" }
		LOD 200
		pass{
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
			#define ALPHA_TEST 1
			#define HAS_BUMP 1
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			//#define SHADER_API_GLES
			#include "forward.cg"
			





			ENDCG
		}
	} 
	SubShader {
		Tags { "RenderType"="BumpOIT" }
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
			#include "forward.cg"
			



			ENDCG
		}
	} 
	SubShader {
		Tags { "RenderType"="OIT" }
		LOD 200
		pass{
			ZTest Less
			Cull Off
			CGPROGRAM
			#define OIT 1



			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			//#define SHADER_API_GLES
			#include "forward.cg"   
			
			ENDCG
		}
	} 
	SubShader{
		Tags{ "RenderType" = "OITGrass" }
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
			#define VERTEX_COLOR 1
			#pragma target 3.0

			#include "forward.cg"    




			ENDCG
		}
	}
	SubShader{
		Tags{ "RenderType" = "OpaqueT4M" "Queue" = "Geometry" }
		LOD 200
		pass {
		Stencil
		{
			Ref 0
			Comp always
			Pass replace
			ZFail keep
		}
		ZTest Less

		CGPROGRAM

		#pragma vertex mrt_vert
		#pragma fragment mrt_frag
		#define T4M_AO 0
		#include "T4MForward.cg"

		ENDCG
			}
	}
	SubShader{
	Tags{ "RenderType" = "OpaqueT4MAO" "Queue" = "Geometry" }
	LOD 200
	pass {
			Stencil
			{
				Ref 0
				Comp always
				Pass replace
				ZFail keep
			}
			ZTest Less

			CGPROGRAM

			#pragma vertex mrt_vert
			#pragma fragment mrt_frag
			#define T4M_AO 1
			#include "T4MForward.cg"






			ENDCG
		}
	}
	SubShader{
		Tags{ "RenderType" = "OpaqueT4MNormal" "Queue" = "Geometry" }
		LOD 200
		pass {
			Stencil
			{
				Ref 0
				Comp always
				Pass replace
				ZFail keep
			}
			ZTest Less

			CGPROGRAM

			#pragma vertex mrt_vert
			#pragma fragment mrt_frag

			#define T4M_AO 2
			#define T4M_NORMAL 1
			#include "T4MForward.cg" 



			ENDCG
		}
	}
	SubShader{
	Tags{ "RenderType" = "OpaqueFace" }
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

		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag

		#include "FaceForward.cg"


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
			#include "forward.cg"
	
	
	
	
	
			ENDCG
		}
	}

}
