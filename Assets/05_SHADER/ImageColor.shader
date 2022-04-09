Shader "Mobile/ImageColor" {
	Properties {
		_Shininess ("Shininess", Range (0.01, 1)) = 0.7
		_ShiniessColor ("ShininessColor", Color) = (1,1,1,1)
		
		_BaseColor ("Base Color", Color) = (1,1,1,1)
		_BodyColor ("Body Color", Color) = (1,1,1,1)
		_Body ("Body", 2D) = "white" {}
		
		_PatternColor ("Pattern Color", Color) = (1,1,1,1)
		_Pattern ("Pattern", 2D) = "gray" {}

		_ColorImage("ColorImage", 2D) = "gray" {}

		_Eye ("Eye", 2D) = "gray" {}
	}

	SubShader 
	{
		Tags{ "RenderType" = "Opaque" }
		CGPROGRAM
		#pragma surface surf Lambert
		struct Input
		{
			float2 uv_Body;
			float2 uv_Eye;
		};
		sampler2D _Body;
		sampler2D _Eye;
		void surf(Input IN, inout SurfaceOutput o)
		{
			o.Albedo = tex2D(_Body, IN.uv_Body).rgb;
		}
		ENDCG
		
	} 
	FallBack "Diffuse"
}