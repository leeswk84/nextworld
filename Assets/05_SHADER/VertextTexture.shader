Shader "Mobile/VertextTexture"
{
	Properties
	{
		_BaseTexture("Base Texture", 2D) = "white" {}
		_BaseTexture2("Base Texture2", 2D) = "white" {}
		_BaseTexture3("Base Texture3", 2D) = "white" {}
		_BaseTexture4("Base Texture4", 2D) = "white" {}
		_TextureColor("TextureColor (Color)", 2D) = "white" {}
		_TextureTile("TileWidthCount", Range(1, 10)) = 2
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		//LOD 200
		Fog{ Mode Off }
		
		CGPROGRAM
		#pragma surface surf Lambert
		#pragma exclude_renderers flash

		sampler2D _BaseTexture;
		sampler2D _BaseTexture2;
		sampler2D _BaseTexture3;
		sampler2D _BaseTexture4;
		sampler2D _TextureColor;
		float _TextureTile;

		half4 t0;
		half4 t1;
		half4 t2;
		half4 t3;
		half4 tc;
		half4 cum;
		fixed fac;

		struct Input 
		{
			float2 uv_BaseTexture;
			float2 uv2_BaseTexture2;
			float2 uv3_BaseTexture3;
			float2 uv4_BaseTexture4;
			float4 color : COLOR;
		};

		void surf(Input IN, inout SurfaceOutput o) 
		{
			t0 = tex2D(_BaseTexture, IN.uv_BaseTexture);
			t1 = tex2D(_BaseTexture2, IN.uv2_BaseTexture2);
			t2 = tex2D(_BaseTexture3, IN.uv3_BaseTexture3);
			t3 = tex2D(_BaseTexture4, IN.uv4_BaseTexture4);

			tc = tex2D(_TextureColor, IN.uv_BaseTexture / (1 / _TextureTile));

			cum = t1 * tc.r + t2 * tc.g + t3 * tc.b;
			fac = tc.r + tc.g + tc.b;

			if (fac != 0) cum /= fac;
			cum = lerp(t0, cum, fac);
			
			o.Albedo = cum.rgb * IN.color;
		}								
		ENDCG
	}
	//FallBack "Diffuse"
	FallBack "Mobile/Diffuse"
}
