Shader "GDShader/Transparency" {
	Properties{
	}
		SubShader{
		Tags{ "RenderType" = "OpaqueBump" }
		LOD 200
		pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag			
		struct v2f 
		{
			float4 pos : SV_POSITION;
		};

		v2f vert(appdata_full v)
		{
			v2f o;
#if SHADER_API_GLES|| SHADER_API_GLES3
			//Mobile
			o.pos = float4(v.vertex.x * 2,v.vertex.y * 2,0,1);
#else
			//Editor Only 
			o.pos = float4(v.vertex.x * 2,-v.vertex.y * 2,0,1);
#endif
			return o;
		}

		float4 frag(v2f IN) : COLOR
		{
			return float4(0,0,0,1);
		}
			ENDCG
		}
	}
}