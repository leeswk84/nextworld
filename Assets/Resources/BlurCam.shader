Shader "Hidden/Starwars/BlurCam"
{
	Properties
	{
		_MainTex ("", 2D) = "white" {}
		_BaseTex ("", 2D) = "white" {}
	}

	CGINCLUDE
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		sampler2D _BaseTex;
		half2 _MainTex_TexelSize;
		half _SampleScale;

		struct appdata
		{
			float4 vertex : POSITION;
			half2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float4 vertex : SV_POSITION;
			half2 uv : TEXCOORD0;
		};

		v2f vert (appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv;
			return o;
		}

		half4 DownsampleFilter(half2 uv)
		{
			half4 d = _MainTex_TexelSize.xyxy * half4(-1, -1, +1, +1);

			half3 s;
			s  = tex2D(_MainTex, uv + d.xy);
			s += tex2D(_MainTex, uv + d.zy);
			s += tex2D(_MainTex, uv + d.xw);
			s += tex2D(_MainTex, uv + d.zw);

			return half4(s * 0.25, 1.0);
		}

		half4 UpsampleFilter(half2 uv)
		{
			half4 d = _MainTex_TexelSize.xyxy * half4(-1, -1, +1, +1) * (_SampleScale);// * 0.5);

			half3 s;
			s  = tex2D(_MainTex, uv + d.xy);
			s += tex2D(_MainTex, uv + d.zy);
			s += tex2D(_MainTex, uv + d.xw);
			s += tex2D(_MainTex, uv + d.zw);

			return half4(s * 0.25, 1.0);
		}

		half4 fragDown (v2f i) : SV_Target
		{
			return DownsampleFilter(i.uv);
		}

		half4 fragUp (v2f i) : SV_Target
		{
			return UpsampleFilter(i.uv);
		}

		half4 fragDownLinear (v2f i) : SV_Target
		{
			return pow(DownsampleFilter(i.uv), 1 / 2.2);
		}

		half4 fragUpLinear (v2f i) : SV_Target
		{
			return pow(UpsampleFilter(i.uv), 1 / 2.2);
		}
	ENDCG
		
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragDown
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragUp
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragDownLinear
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragUpLinear
			ENDCG
		}
	}
}
