Shader "Starwars/UI/Blured Background"
{
	Properties
	{
		// _Radius ("Radius", Range(1, 255)) = 1
		[PerRendererData]  _MainTex ("Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)

		_StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15
	}
	Category
	{
		Tags
        {
			"Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

		Cull Off
        Lighting Off
        ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
        ZTest [unity_GUIZTestMode]

		CGINCLUDE
			sampler2D _BluredTex;
			sampler2D _MainTex;

			// half4 _BlurTex_TexelSize;
			half4 _MainTex_ST;
		ENDCG

		SubShader
		{
			// GrabPass{}
			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata_t
				{
					float4 vertex : POSITION;
					half2  uv     : TEXCOORD0;
					half4 color  : COLOR;
				};

				struct v2f
				{
					float4 vertex : POSITION;
					half4  uvgrab : TEXCOORD0;
					half2  uv     : TEXCOORD1;
					half4 color  : COLOR;
				};

				v2f vert(appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);

				#if UNITY_UV_STARTS_AT_TOP
					o.uvgrab.xy = (half2(o.vertex.x, o.vertex.y) + o.vertex.w) * 0.5;
				#else
					o.uvgrab.xy = (half2(o.vertex.x, o.vertex.y) + o.vertex.w) * 0.5;
				#endif
					o.uvgrab.zw = o.vertex.zw;

					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.color = v.color;
					return o;
				}

				half4 frag(v2f i) : SV_Target
				{
					half4 col;// = tex2D(_MainTex, i.uv);
					half3 blurBG = tex2Dproj(_BluredTex, UNITY_PROJ_COORD(i.uvgrab));
					// col.rgb = lerp(blurBG, i.color.rgb, i.color.a);
					col.rgb = blurBG * i.color.rgb;
					col.a = i.color.a;
					// col.rgb *= blurBG * 2;
					// col.rgb = blurBG;

					return col;

					// return half4(lerp(blurBG, col.rgb, i.color.a), col.a);
				}
				ENDCG
			}
		}
	}
}
