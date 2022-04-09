// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Mobile/VertexColors" 
{
	Properties {}

	SubShader 
	{
		Pass
		{
			//Tags {  "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct vertexInput 
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 vertCol : COLOR;
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.vertCol = v.color;
				return o;
			}

			float4 frag(vertexOutput i) : COLOR
			{
				return i.vertCol;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
