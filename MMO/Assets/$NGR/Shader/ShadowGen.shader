Shader "Hidden/GDShader/ShadowGen" {
	Properties {
		//_MainTex ("Base (RGB)", 2D) = "white" {}
		_Cube("Env (RGB)", CUBE) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		//0
		pass{

			Stencil
			{
				Ref 4//64+4
				ReadMask 68  
				WriteMask 4 
				Comp Greater     
				Pass replace 
			}    
			ZTest	Greater 
			ZWrite	Off  
			Cull Front      
			ColorMask RGB
			Blend One One          
			CGPROGRAM 
			#pragma target 3.0
			#pragma vertex vert   
			#pragma fragment frag  
			#define SHADOW_QUILITY 0              
				    
			#include "NearShadow.cg"                           
			ENDCG   
		}     
	pass {    
		//1
		Stencil
		{
			Ref 4//64+4
			ReadMask 68
			WriteMask 4
			Comp Greater                           
			Pass replace
		}
			ZTest	Greater
			ZWrite	Off
			Cull Front
			ColorMask RGB
			Blend One One
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#define SHADOW_QUILITY 1 
			
			#include "NearShadow.cg"                                 
			              
			ENDCG
	}
	//2   
		//Far Shadow
		pass{

			Stencil
			{
				Ref 4//64+4
				ReadMask 68
				WriteMask 4
				Comp Greater
				Pass replace
			}
			ZTest	Greater 
			ZWrite	Off     
			Cull Off
			ColorMask RGB
			Blend One One
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#define SHADOW_QUILITY 2
			#include "NearShadow.cg"
			ENDCG

		}
		//3
		pass {

			Stencil
			{
				Ref 4//64+4
				ReadMask 68
				WriteMask 4
				Comp Greater   
				Pass replace
			}
				ZTest	Greater
				ZWrite	Off
				Cull Off
				ColorMask RGB 
				Blend One One
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert   
				#pragma fragment frag
				#define SHADOW_QUILITY 3     
				#define AMBIENT_SPECULAR 0 
				#include "NearShadow.cg"      

				                   
				ENDCG   

		}

		//4
		//No Shadow
		pass {
		  
			Stencil     
			{
				Ref 4//64+4                
				ReadMask 68         
				WriteMask 4
				Comp Greater
				Pass replace
			}
				ZTest	Greater
				ZWrite	Off
				Cull Off 
				ColorMask RGB                
				Blend One One              
				CGPROGRAM
				#pragma target 3.0         
				#pragma vertex vert 
				#pragma fragment frag   

				#include "NonShadow.cg"                                                       

				ENDCG        
		} 
		//5 clear shadow mask 0   
		pass {		
			ZTest	Greater           
				ZWrite	Off
				Cull Off
				ColorMask A
				CGPROGRAM
				#pragma target 3.0      
				#pragma vertex vert
				#pragma fragment frag_clear  
				#define SHADOW_QUILITY 0      
				
				#include "NonShadow.cg"                         
				ENDCG
		}
		//6 clear shadow mask 1
		pass {
			ZTest	Less
				ZWrite	Off
				Cull Off
				ColorMask A
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag_clear  
				#define SHADOW_QUILITY 0   
				
				#include "NonShadow.cg"                    
				ENDCG
		}
		//7 clear shadow mask all
		pass {
			ZTest	Always
				ZWrite	Off
				Cull Off
				ColorMask A
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag_clear  
				#define SHADOW_QUILITY 0 
				
				#include "NonShadow.cg"                    
				ENDCG
		}
		//8 calc shadow mask 0  rotate 12 sample
		pass {
				ZTest	Greater
				ZWrite	Off
				Cull Off
				ColorMask A
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag_shadow  
				#define SHADOW_QUILITY 1    
				
				#include "NonShadow.cg"                    
				ENDCG
		}
		//9 calc shadow mask 0  3x3 sample
		pass {
			ZTest	Greater
				ZWrite	Off
				Cull Off
				ColorMask A
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag_shadow  
				#define SHADOW_QUILITY 1    
				
				#include "NonShadow.cg"                    
				ENDCG
		}
		//10 calc shadow mask 0  2x2 sample
		pass {
			ZTest	Greater
				ZWrite	Off
				Cull Off
				ColorMask A
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag_shadow  
				#define SHADOW_QUILITY 2    
				
				#include "NonShadow.cg"                    
				ENDCG
		}
		//11 calc shadow mask 0 2 sample
		pass {
			ZTest	Greater
				ZWrite	Off
				Cull Off
				ColorMask A
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag_shadow  
				#define SHADOW_QUILITY 3    
				
				#include "NonShadow.cg"                    
				ENDCG
		}
		//12 calc shadow mask 1 rotate 12 sample
		pass {
			ZTest	Less
				ZWrite	Off
				Cull Off
				ColorMask A
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag_shadow  
				#define SHADOW_QUILITY 1    
				
				#include "NonShadow.cg"                    
				ENDCG
		}
		//13 calc shadow mask 1 3x3 sample
		pass {
			ZTest	Less
				ZWrite	Off
				Cull Off
				ColorMask A
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag_shadow  
				#define SHADOW_QUILITY 1    
				
				#include "NonShadow.cg"                    
				ENDCG
		}
		//14 calc shadow mask 1 2x2 sample
		pass {
			ZTest	Less
				ZWrite	Off
				Cull Off
				ColorMask A
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag_shadow  
				#define SHADOW_QUILITY 2    
				
				#include "NonShadow.cg"                    
				ENDCG
		}
		//15 calc shadow mask 1 2 sample
		pass {
			ZTest	Less
				ZWrite	Off
				Cull Off
				ColorMask A
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag_shadow  
				#define SHADOW_QUILITY 3   
				
				#include "NonShadow.cg"                    
				ENDCG
		}
		//16 calc shadow mask all rotate 12 sample
		pass {
			ZTest	Always
				ZWrite	Off
				Cull Off
				ColorMask A
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag_shadow  
				#define SHADOW_QUILITY 1    
				
				#include "NonShadow.cg"                    
				ENDCG
		}
		//17 calc shadow mask all 3x3 sample
		pass {
			ZTest	Always
				ZWrite	Off
				Cull Off
				ColorMask A
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag_shadow  
				#define SHADOW_QUILITY 1    
				
				#include "NonShadow.cg"                    
				ENDCG
		}
		//18 calc shadow mask all 2x2 sample
		pass {
			ZTest	Always
				ZWrite	Off
				Cull Off
				ColorMask A
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag_shadow  
				#define SHADOW_QUILITY 2   
				
				#include "NonShadow.cg"                    
				ENDCG
		}
		//19 calc shadow mask all 2 sample
		pass {
			ZTest	Always
				ZWrite	Off
				Cull Off
				ColorMask A
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag_shadow  
				#define SHADOW_QUILITY 3    
				
				#include "NonShadow.cg"                    
				ENDCG
		}
		//20 combine shadow mask and calc MainLight
		pass {
				ZTest	Greater
				ZWrite	Off
				Cull Off
				ColorMask RGB
				Blend One One   
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag_combine_shadow  
				  
				
				#include "NonShadow.cg"                             
				ENDCG
		}
	} 
	//FallBack "Diffuse"
}
