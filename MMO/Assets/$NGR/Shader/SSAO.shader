Shader "Hidden/GDShader/SSAO" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Diffuse ("_Diffuse (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		pass{//0
		Stencil  
		{ 
			Ref 64
			ReadMask 64
			Comp Notequal
			Pass keep  
		}
			ZTest Greater 
			ZWrite Off 
			ColorMask RGB
			Cull Off
			Blend One One  
			CGPROGRAM   
			#pragma target 3.0
			#pragma vertex vert 
			#pragma fragment frag     
			#define AO_QUILITY  3     

			#include "AO.cg"                    

				     
			ENDCG  
		}

		pass { //1 
			Stencil     
			{
				Ref 64
				ReadMask 64
				Comp Notequal
				Pass keep
			}
			ZTest Greater
			ZWrite Off
			ColorMask RGB  
			Cull Off 
			Blend One One   
			CGPROGRAM
			#pragma target 3.0 
			#pragma vertex vert 
			#pragma fragment frag
			#define AO_QUILITY 2      
				 
			#include "AO.cg"
			ENDCG
		}
		pass {//2
			Stencil
			{
				Ref 64
				ReadMask 64
				Comp Notequal
				Pass keep
			}
			ZTest Greater
				ZWrite Off
				ColorMask RGB
				Cull Off
				Blend One One
				CGPROGRAM
#pragma target 3.0  
#pragma vertex vert    
#pragma fragment frag
#define AO_QUILITY 1

#include "AO.cg"
				ENDCG 
		}  
		pass {//3
			Stencil
			{
				Ref 64
				ReadMask 64
				Comp Notequal
				Pass keep
			}
			ZTest Greater
				ZWrite Off
				ColorMask RGB
				Cull Off
				Blend One One
				CGPROGRAM  
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag     
#define AO_QUILITY 0  

#include "AO.cg"
				ENDCG
		}
		pass {//4
			Stencil
			{
				Ref 64
				ReadMask 64
				Comp Notequal
				Pass keep
			}
			ZTest Greater
			ZWrite Off
				//Blend One One
				//ColorMask RGB
				Cull Off
				Blend One One
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag_noao 

				#include "AO.cg"

			ENDCG
						
		}
		//5 checkerboard rendering 0
		pass {
			ZTest Less
				ZWrite Off
				ColorMask RGB
				Cull Off
				Blend One Zero 
				CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag    
#define AO_QUILITY 5   

#include "AO.cg"
				ENDCG
		}
		//6 checkerboard rendering 1
		pass {
			ZTest Greater
				ZWrite Off
				ColorMask RGB
				Cull Off
				Blend One Zero
				CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag    
#define AO_QUILITY 5  

#include "AO.cg"
				ENDCG
		}
		
		//7 combine ao to backbuffer
		pass {

			ZTest Greater
				ZWrite Off
				//Blend One One
				ColorMask RGB
				Cull Off
				Blend One One
				CGPROGRAM    
				#pragma target 3.0  
				#pragma vertex vert
				#pragma fragment frag_ao
				
				#include "AO.cg"    

				ENDCG

		}
		//8 checkerboard rendering  when camera move
		pass {
			ZTest Always
				ZWrite Off
				ColorMask RGB 
				Cull Off
				Blend One Zero
				CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag    
#define AO_QUILITY 5  

#include "AO.cg" 
				ENDCG  
		} 

			pass { //9 for debug  
			Stencil  
			{ 
				Ref 64 
				ReadMask 64
				Comp Notequal  
				Pass keep   
			}
			ZTest Greater
			ZWrite Off 
			ColorMask RGB  
			Cull Off 
			Blend One Zero   
			CGPROGRAM
			#pragma target 3.0      
			#pragma vertex vert    
			#pragma fragment frag    
			#define AO_QUILITY 1              
			#define AO_Debug 1    
			#include "AO.cg"           
			ENDCG         
		} 
	} 
	//FallBack "Diffuse"
}
