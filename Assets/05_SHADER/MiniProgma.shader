Shader "Mobile/MiniProgma" 
{
	Properties 
	{
		_Pattern ("Pattern", 2D) = "gray" {}
		_PatternColor ("Pattern Color", Color) = (1,1,1,1)

		_Shininess ("Shininess", Range (0.01, 1)) = 0.7
		_ShiniessColor ("ShininessColor", Color) = (1,1,1,1)
		
		_BaseColor ("Base Color", Color) = (1,1,1,1)
		_BodyColor ("Body Color", Color) = (1,1,1,1)
		_Body ("Body", 2D) = "white" {}

	}

	SubShader 
	{
		Pass
		{
			Material
			{
				Diffuse [_BaseColor]
				Ambient [_BaseColor]
				Shininess [_Shininess]
				Specular [_ShiniessColor]
			}
			
			Lighting On
			SeparateSpecular On
			//Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse

			Fog { Mode Off }
			
			SetTexture [_Body] 
			{ 
				constantColor [_BodyColor] 
				combine texture * constant
			}

			SetTexture [_Pattern] 
			{ 
				constantColor [_PatternColor] 
				combine constant lerp(texture) previous
			}
			SetTexture[_Body]
			{
				combine previous * primary
			}
		}
	} 
	FallBack "VertexLit"
}