Shader "GDShader/Water" {                
	Properties {
		_Depth ("Depth", 2D) = "white" {}
		_LastFrame ("LastFrame", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)

		_GerstnerIntensity("Wave Intensity", Float) = 1.0
		_GSpeed("Wave Speed", Vector) = (1.2, 1.375, 1.1, 1.5)      
		_GAmplitude("Wave Amplitude", Vector) = (0.3, 0.35, 0.25, 0.25)
		_GFrequency("Wave Frequency", Vector) = (1.3, 1.35, 1.25, 1.25)
		_GSteepness("Wave Steepness", Vector) = (1.0, 1.0, 1.0, 1.0)               
		_GDirectionAB("Wave Direction", Vector) = (0.3, 0.85, 0.85, 0.25)	   
		_GDirectionCD("Wave Direction", Vector) = (0.1, 0.9, 0.5, 0.5)
			  
		_OceanParams0("CE Water Param", Vector) = (1.0, 1.0, 1.0, 1.0)
		_AnimGenParams("CE Water Speed", Float) = 1.0   
		_FlowDir("CE Water FlowDir", Vector) = (1.0, 1.0, 1.0, 1.0)    
	} 
	SubShader {		
		Tags { "DeferredLight"="Light" }      
		LOD 200
		//0
		pass{       
			Stencil
			{
				Ref 64
				ReadMask 64
				Comp Notequal
				Pass keep
			}
			ZTest LEqual    
			//ZWrite Off          
			ColorMask RGB     
		    Cull Off    
			CGPROGRAM
			#pragma target 3.0    
			#pragma vertex vert                
			#pragma fragment frag    
			#include "Water.cg"              
			ENDCG
		}

		//1
		pass {
			Stencil
			{
				Ref 64
				ReadMask 64
				Comp Notequal
				Pass keep
			}
			ZTest LEqual 
			//ZWrite Off 
			ColorMask RGB	

			  
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#define USE_MESH_NORMAL 1            
			#include "Water.cg"  
			ENDCG
		}

		//2
		pass {
			Stencil
			{
				Ref 64
				ReadMask 64
				Comp Notequal
				Pass keep
			}
			ZTest LEqual    
			//ZWrite Off
			ColorMask RGB
			Blend One OneMinusSrcAlpha              
				              
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex forward_vert
			#pragma fragment forward_frag                    
			#include "Water.cg"

			ENDCG
		}
		 
		//3
		pass {
			Stencil
			{
				Ref 64
				ReadMask 64
				Comp Notequal
				Pass keep
			}
				ZTest LEqual
				//ZWrite Off
				ColorMask RGB
				Blend One OneMinusSrcAlpha

				CGPROGRAM
				#pragma target 3.0
				#pragma vertex forward_vert
				#pragma fragment forward_frag
				#define USE_MESH_NORMAL 1
				#include "Water.cg"
				ENDCG
		}

		//4
		pass {
			Stencil
			{
				Ref 64
				ReadMask 64
				Comp Notequal
				Pass keep
			}
			ZTest LEqual
				ColorMask RGB
				    
				CGPROGRAM      
			#pragma target 3.0
			#pragma vertex vert        
			#pragma fragment frag     
			#define WATER_REFLECT 1    
			#include "Water.cg"     
				ENDCG      
		}
  
		//5
		pass {
			Stencil
			{
				Ref 64
				ReadMask 64
				Comp Notequal
				Pass keep
			}
			ZTest LEqual
			ColorMask RGB

				CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert    
			#pragma fragment frag     
			#define USE_MESH_NORMAL 1
			#define WATER_REFLECT 1                         
			#include "Water.cg" 
				ENDCG
		}

		//6
		pass {
			Stencil
			{
				Ref 64
				ReadMask 64
				Comp Notequal
				Pass keep
			}
			ZTest LEqual 
			ColorMask RGB

				CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert        
			#pragma fragment frag      
			#define USE_MESH_NORMAL 1  
			#define WATER_REFLECT 1    
			#define WATER_WAVE 1   
			#define GERSTNER_WAVE 1                 
			#include "Water.cg"                         
				ENDCG
		}

		//7
		pass {
			Stencil
			{
				Ref 64
				ReadMask 64
				Comp Notequal
				Pass keep
			}
			ZTest LEqual
			ColorMask RGB

				CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert            
			#pragma fragment frag     
			#define USE_MESH_NORMAL 1  
			#define WATER_WAVE 1  
			#include "Water.cg"     
				ENDCG
		}

		//8
		pass {
			Stencil
			{
				Ref 64
				ReadMask 64
				Comp Notequal
				Pass keep
			}
			ZTest LEqual
			ColorMask RGB

				CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert        
			#pragma fragment frag      
			#define WATER_REFLECT 1    
			#define WATER_WAVE 1  
			#include "Water.cg"                                    
				ENDCG
		}      
		  
		//9
		pass {
			Stencil
			{
				Ref 64
				ReadMask 64
				Comp Notequal
				Pass keep
			}
			ZTest LEqual
			ColorMask RGB

				CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert        
			#pragma fragment frag                                    
			#define WATER_WAVE 1  
			#include "Water.cg"     
				ENDCG
		}
	} 
	//FallBack "Diffuse" 
}
