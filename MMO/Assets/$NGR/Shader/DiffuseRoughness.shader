Shader "Hidden/GDShader/DiffuseRoughness" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		pass{
			ZTest Equal
			ZWrite Off
			Cull[CullMode]
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

//#define LIGHTMAP_ON 1


			#include "colorbuffer.cg"
			ENDCG
		}
	} 
	SubShader{
		Tags{ "RenderType" = "OpaquePainter" }
		LOD 200
		pass {
			ZTest Equal
			ZWrite Off
			Cull[CullMode]
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#define COLOR_PAINTER 1
			#include "colorbuffer.cg"
				ENDCG
		}
	}
	SubShader{
		Tags{ "RenderType" = "OpaqueLeaf" }
		LOD 200
		pass {
		ZTest Equal
			ZWrite Off
			Cull[CullMode]
			CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		
		#define TREE_LEAF 1
		
		#include "colorbuffer.cg"
			ENDCG
	}
	}
	SubShader {
		Tags { "RenderType"="OpaqueBump" }
		LOD 200
		pass{
			ZTest Equal
			ZWrite Off
			Cull[CullMode]
			CGPROGRAM
			#define HAS_BUMP 1
			#pragma vertex vert
			#pragma fragment frag
			//#define SHADER_API_GLES
//#define LIGHTMAP_ON 1
			#include "UnityCG.cginc"
			#include "colorbuffer.cg"



			ENDCG
		}
	} 
	SubShader{
		Tags{ "RenderType" = "OpaqueBumpPainter" }
		LOD 200
		pass {
		ZTest Equal
			ZWrite Off
			Cull[CullMode]
			CGPROGRAM
			#define HAS_BUMP 1
			#define COLOR_PAINTER 1
			#pragma vertex vert
			#pragma fragment frag
			//#define SHADER_API_GLES
			//#define LIGHTMAP_ON 1
			#include "UnityCG.cginc"
			#include "colorbuffer.cg"


			ENDCG
	}
	}
	SubShader {
		Tags { "RenderType"="AlphaTest" }
		LOD 200
		pass{
			ZTest Equal
			ZWrite Off
			Cull[CullMode]
			CGPROGRAM
			//#define ALPHA_TEST 1
			#pragma vertex vert
			#pragma fragment frag
			//#define SHADER_API_GLES
			#include "colorbuffer.cg"
			

			ENDCG
		}
	} 
		SubShader{
		Tags{ "RenderType" = "AlphaTestDisAppear" }
		LOD 200
		pass {
		ZTest Equal
		ZWrite Off
		Cull[CullMode]
		CGPROGRAM
		//#define ALPHA_TEST 1
		//#define ALPHA_DisAppear 1
		#pragma vertex vert
		#pragma fragment frag
					//#define SHADER_API_GLES
		#include "colorbuffer.cg"


			ENDCG
		}
	}

	SubShader {
		Tags { "RenderType"="AlphaTestBlack" }
		LOD 200
		pass{
			ZTest Equal
			ZWrite Off
			Cull Off
			CGPROGRAM
			//#define ALPHA_TEST_BLACK 1
			#pragma vertex vert
			#pragma fragment frag
			#include "colorbuffer.cg"
			

			ENDCG
		}
	} 
	SubShader {
		Tags { "RenderType"="BumpAlphaTest" }
		LOD 200
		pass{
			ZTest Equal
			ZWrite Off
			Cull Off
			CGPROGRAM
			//#define ALPHA_TEST 1
			#define HAS_BUMP 1
			#pragma vertex vert
			#pragma fragment frag
			//#define SHADER_API_GLES
			#include "colorbuffer.cg"
			

			ENDCG
		}
	} 
	SubShader {
		Tags { "RenderType"="BumpOIT" }
		LOD 200
		pass{
			ZTest Equal
			ZWrite Off
			Cull Off
			CGPROGRAM
			//#define OIT 1
			#define HAS_BUMP 1

			#pragma vertex vert
			#pragma fragment frag
			//#define SHADER_API_GLES
			#include "colorbuffer.cg"
			
			ENDCG
		}
	} 
	SubShader {
		Tags { "RenderType"="OIT" }
		LOD 200
		pass{
			ZTest Equal
			ZWrite Off
			Cull Off
			CGPROGRAM
			//#define OIT 1


			#pragma vertex vert
			#pragma fragment frag
			//#define SHADER_API_GLES
			#include "colorbuffer.cg"
			
			ENDCG
		}
	} 
	SubShader{
		Tags{ "RenderType" = "OITGrass" }
		LOD 200
		pass {
			ZTest Equal
			ZWrite Off
			Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			//#define ALPHA_TEST 1
			//#define OIT 1
			#define FORCE_FIELD 1
			#define VERTEX_COLOR 1
			#pragma target 3.0


			#include "colorbuffer.cg"


			ENDCG
		}
	}
	SubShader{
		Tags{ "RenderType" = "OpaqueT4M" }
		LOD 200
		pass {
			ZTest Equal
			ZWrite Off
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
//#define LIGHTMAP_ON 1

			#include "T4M.cg"
		ENDCG
		}
	}
	SubShader{
		Tags{ "RenderType" = "OpaqueT4MAO" }
		LOD 200
		pass {
			ZTest Equal
			ZWrite Off
			CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
//#define LIGHTMAP_ON 1









		#define T4M_AO 1
		#include "T4M.cg"
		ENDCG

			}
	}
	SubShader{
		Tags{ "RenderType" = "OpaqueT4MNormal" }
		LOD 200
		pass {
		ZTest Equal
			ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#define	T4M_AO 2
			#define T4M_NORMAL 1
			#include "T4M.cg"
			ENDCG
	}
	}
	SubShader{
		Tags{ "RenderType" = "OpaqueFace" }
		LOD 200
		pass {
			ZTest Equal
			ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "DiffuseFace.cg"
			ENDCG
		}
	}
	SubShader{
		Tags{ "RenderType" = "OpaqueHair" "Queue" = "Geometry" }
		LOD 200
		pass {

			ZTest LEqual
			Cull[CullMode]
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define HAIR_ON 1
			//	int StencilRef;
			#include "colorbuffer.cg"
			ENDCG
		}
	}

	FallBack "Diffuse"
}
