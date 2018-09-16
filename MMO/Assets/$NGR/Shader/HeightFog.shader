Shader "GDShader/HeightFog" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		pass{
			Blend SrcAlpha OneMinusSrcAlpha
			//Blend One One 
			ColorMask RGB 
			Cull Off
			ZTest Off
			//	ZTest Off
			ZWrite Off  
				      
			CGPROGRAM  
			#pragma vertex vert
			#pragma fragment frag
			#include "heightfog.cg"     
			ENDCG       
		}
		pass { 
		Blend SrcAlpha OneMinusSrcAlpha         
			//Blend One One
			ColorMask RGB   
			Cull Off      
			ZTest Greater     
			//	ZTest Off   
			ZWrite Off           

			CGPROGRAM  
			#pragma vertex vert    
			#pragma fragment frag 
			#include "heightfog.cg"    
			ENDCG  
		}    
		pass {   
			Blend SrcAlpha OneMinusSrcAlpha   
				//Blend One One 
				ColorMask RGB
				Cull Off
				ZTest Off
				//	ZTest Off
				ZWrite Off

				CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#define USE_Dynamic_Fog 1  
#include "heightfog.cg" 
				ENDCG
		}          
		pass {
			Blend SrcAlpha OneMinusSrcAlpha
				//Blend One One
				ColorMask RGB
				Cull Off
				ZTest Greater
				//	ZTest Off   
				ZWrite Off

				CGPROGRAM
#pragma vertex vert    
#pragma fragment frag 
#define USE_Dynamic_Fog 1  
#include "heightfog.cg" 
				ENDCG
		}

	} 
	FallBack "Diffuse"
}
