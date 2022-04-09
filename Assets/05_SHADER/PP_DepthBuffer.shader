Shader "Custom/PP_DepthBuffer"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	_DepthDistance("DepthDistance", float) = 25
		_BlurIntensity("_BlurIntensity", Range(2, 10)) = 4
	}
		SubShader
	{
		Cull Off
		ZWrite Off
		ZTest Always

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}

	sampler2D _MainTex;
	float4 _MainTex_TexelSize;

	sampler2D_float _CameraDepthTexture; //! 카메라로부터 뎁스텍스처를 받아옴
	half _DepthDistance;
	half _BlurIntensity;

	fixed4 frag(v2f i) : SV_Target
	{
		float4 fMainTex = tex2D(_MainTex, i.uv);

		_BlurIntensity = ceil(_BlurIntensity);				//! 텍스처 블러
		float4 fBlurTex = 0;
		for (int u = -_BlurIntensity; u <= _BlurIntensity; u++)
		{
			for (int v = -_BlurIntensity; v <= _BlurIntensity; v++)
			{
				float2 o = float2(u, v) * _MainTex_TexelSize.xy;
				fBlurTex += tex2D(_MainTex, i.uv + o);
			}
		}
		fBlurTex *= 1.0f / pow((_BlurIntensity * 2.0f), 2);

		float fDepthData = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(float4(i.uv, 1, 1))).r;	//! 텝스텍스처 샘플러에서 텍셀정보를 가져옴
		float fSceneZ = LinearEyeDepth(fDepthData);		//! DepthData를 0~1 Linear데이터로 변환(0 = 카메라, 1 = 먼거리)
		float fCalc_Depth = 1 - saturate(fSceneZ / _DepthDistance);		//! 거리 값 조절용 계산
																		//return fCalc_Depth;
		return lerp(fBlurTex, fMainTex, saturate(fCalc_Depth));		//! 뎁스데이터를 이용해 일반 텍스처랑 블러 텍스처 lerp
	}
		ENDCG
	}
	}
}
