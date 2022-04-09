Shader "Mobile/Ambient Diffuse Detail" 
{
	Properties 
	{
		_Shininess ("Shininess", Range (0.01, 1)) = 0.7
		_ShiniessColor ("ShininessColor", Color) = (1,1,1,1)
		
		_BaseColor ("Base Color", Color) = (1,1,1,1)
		_Body ("Body", 2D) = "white" {}

		//_BodyColor ("Body Color", Color) = (1,1,1,1)
		//_PatternColor ("Pattern Color", Color) = (1,1,1,1)
		//_Pattern ("Pattern", 2D) = "gray" {}
		//_Eye ("Eye", 2D) = "gray" {}
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

			Fog { Mode Off }
			
			SetTexture [_Body] 
			{ 
				constantColor [_BodyColor] 
				combine texture * primary
			}
			

			/*
			SetTexture [_BaseColor] 
			{ 
				constantColor [_BaseColor] 
				combine texture * constant
			}
			SetTexture [_BaseColor] { combine previous * primary}
			
			SetTexture [_Pattern] 
			{ 
				constantColor [_PatternColor] 
				combine constant lerp(texture) previous
			}
			
			SetTexture [_Pattern] { combine previous * primary}
			
			SetTexture [_Eye] 
			{ 
				combine texture lerp(texture) previous
			}
			
			SetTexture [_Eye] { combine previous * primary}
			*/
		}
	} 
	FallBack "VertexLit"
}