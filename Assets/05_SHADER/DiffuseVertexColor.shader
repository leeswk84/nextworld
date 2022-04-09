Shader "Mobile/Diffuse Vertex Color" 
{	
	Properties 
	{
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		
		
		//Lighting On
		//SeparateSpecular On
		Fog { Mode Off }

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

		sampler2D _MainTex;

		struct Input 
		{
			fixed2 uv_MainTex;
			fixed3 vertexColor;
		};

		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
			o.vertexColor = v.color; 
		}

		void surf (Input IN, inout SurfaceOutput  o) 
		{
			o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * IN.vertexColor;
			//fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			//o.Albedo = c.rgb * IN.vertexColor; 
			//o.Alpha = c.a;
		}
		ENDCG
	}

	//FallBack "VertexLit"
	FallBack "Mobile/Diffuse"
}