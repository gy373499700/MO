Shader "Hidden/GDShader/ShadowDepthBA" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		pass{
			ColorMask BA
			ZTest Less
			Cull Off
			CGPROGRAM
			#define SHADOW_DEPTH 1	
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			#include "gbuffer.cg"



			ENDCG

		}
	} 
		SubShader{
		Tags{ "RenderType" = "OpaquePainter" }
		LOD 200
		pass {
		ColorMask BA
		ZTest Less
			Cull Off
			CGPROGRAM
#define SHADOW_DEPTH 1	
#pragma vertex gbuffer_vert
#pragma fragment gbuffer_frag
#include "gbuffer.cg"



			ENDCG

	}
	}
	SubShader{
		Tags{ "RenderType" = "OpaqueLeaf" }
		LOD 200
		pass {
		ColorMask BA
		ZTest Less
			Cull Off
			CGPROGRAM
			#define SHADOW_DEPTH 1	
			#define TREE_LEAF 1
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			#include "gbuffer.cg"
			ENDCG
		}
	}
	SubShader {
		Tags { "RenderType"="OpaqueDouble" }
		LOD 200
		pass{
		ColorMask BA
			ZTest Less
			Cull Off
			CGPROGRAM
			#define SHADOW_DEPTH 1	
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			#include "gbuffer.cg"
			ENDCG
		}
	} 
	SubShader {
		Tags { "RenderType"="OpaqueBump" }
		LOD 200
		pass{
		ColorMask BA
			ZTest Less
				Cull Off
			CGPROGRAM
			#define SHADOW_DEPTH 1	
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
		Tags{ "RenderType" = "OpaqueBumpPainter" }
		LOD 200
		pass {
		ColorMask BA
			ZTest Less
			Cull Off
			CGPROGRAM
			#define SHADOW_DEPTH 1	
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
		Tags { "RenderType"="OpaqueBumpDouble" }
		LOD 200
		pass{
		ColorMask BA
			ZTest Less
			Cull Off
			CGPROGRAM
			#define SHADOW_DEPTH 1	
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
		Tags { "RenderType"="AlphaTest" }
		LOD 200
		pass{
		ColorMask BA
			ZTest Less
				Cull Off
			CGPROGRAM
			#define SHADOW_DEPTH 1	
			#define ALPHA_TEST 1
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			//#define SHADER_API_GLES
			#include "gbuffer.cg"
			

			ENDCG
		}
	} 
	SubShader{
		Tags{ "RenderType" = "AlphaTestDisAppear" }
		LOD 200
		pass {
		ColorMask BA
		ZTest Less
			Cull Off
			CGPROGRAM
		#define SHADOW_DEPTH 1	
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
		Tags { "RenderType"="AlphaTestDouble" }
		LOD 200
		pass{
		ColorMask BA
			ZTest Less
			Cull Off
			CGPROGRAM
			#define SHADOW_DEPTH 1	
			#define ALPHA_TEST 1
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			//#define SHADER_API_GLES
			#include "gbuffer.cg"
			

			ENDCG
		}
	} 
	SubShader {
		Tags { "RenderType"="AlphaTestBlack" }
		LOD 200
		pass{
		ColorMask BA
			ZTest Less
			Cull Off
			CGPROGRAM
			#define SHADOW_DEPTH 1	
			#define ALPHA_TEST_BLACK 1
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			#include "gbuffer.cg"
			

			ENDCG
		}
	} 
	SubShader {
		Tags { "RenderType"="BumpAlphaTest" }
		LOD 200
		pass{
		ColorMask BA
			ZTest Less
			Cull Off
			CGPROGRAM
			#define SHADOW_DEPTH 1	
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
		Tags { "RenderType"="BumpOIT" }
		LOD 200
		pass{
		ColorMask BA
			ZTest Less
			Cull Off
			CGPROGRAM
			#define SHADOW_DEPTH 1	
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
		Tags { "RenderType"="OIT" }
		LOD 200
		pass{
		ColorMask BA
			ZTest Less
			Cull Off
			CGPROGRAM
			#define OIT 1
			#define SHADOW_DEPTH 1	

			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			//#define SHADER_API_GLES
			#include "gbuffer.cg"
			
			ENDCG
		}
	} 
	SubShader{
		Tags{ "RenderType" = "OITGrass" }
		LOD 200
		pass {
		ColorMask BA
		ZTest Less
			Cull Off
			CGPROGRAM
			#define SHADOW_DEPTH 1	
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
		Tags{ "RenderType" = "OpaqueT4M" }
		LOD 200
		pass {
		ColorMask BA
		ZTest Less
			Cull Off
		CGPROGRAM
		#define SHADOW_DEPTH 1	
		#pragma vertex gbuffer_vert
		#pragma fragment gbuffer_frag
		#include "gbuffer.cg"
		ENDCG
		}
	}
	SubShader{
		Tags{ "RenderType" = "OpaqueT4MAO" }
		LOD 200
		pass {
		ColorMask BA
		ZTest Less
			Cull Off
			CGPROGRAM
		#define SHADOW_DEPTH 1	
		#pragma vertex gbuffer_vert
		#pragma fragment gbuffer_frag
		#include "gbuffer.cg"
		ENDCG
			}
	}
	SubShader{
		Tags{ "RenderType" = "OpaqueFace" }
		LOD 200
		pass {
		ColorMask BA
		ZTest Less
			Cull Off
			CGPROGRAM
		#define SHADOW_DEPTH 1	
		#pragma vertex gbuffer_vert
		#pragma fragment gbuffer_frag
		#include "gbuffer.cg"
			ENDCG
		}
	}
	//SubShader{
	//	Tags{ "RenderType" = "OpaqueFaceInUI" }
	//	LOD 200
	//	pass {
	//	ZTest Less
	//		CGPROGRAM
	//	#define SHADOW_DEPTH 1	
	//	#pragma vertex gbuffer_vert
	//	#pragma fragment gbuffer_frag
	//	#include "gbuffer.cg"
	//		ENDCG
	//	}
	//}
	SubShader{
		Tags{ "RenderType" = "OpaqueHair"}
		LOD 200
		pass {
		ColorMask BA
		ZTest Less
			Cull Off
			CGPROGRAM
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			#define SHADOW_DEPTH 1	
						//	int StencilRef;
			#include "gbuffer.cg"
			ENDCG
		}
	}
	FallBack "Diffuse"
}
