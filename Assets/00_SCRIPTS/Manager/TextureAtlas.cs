using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureAtlas : MonoBehaviour
{
	public Dictionary<string, int> dicAtlasWidth;
	private Dictionary<string, Vector2[]> dicBaseUV;

	private Vector2[] vecs2;
	private int num;
	private float degree;
	private int width;
	private int max;

	public void Init()
	{
		dicAtlasWidth = new Dictionary<string, int>();
		dicBaseUV = new Dictionary<string, Vector2[]>();

		//texture atlas 가로 갯수
		dicAtlasWidth.Add("buildTile", 4);
		dicAtlasWidth.Add("dot", 1);
	}

	public void SetTextureUV(ref MeshFilter filter, string tex, int value)
	{
		//-3 8
		//기본 UV가 있는지 여부.
		if (dicAtlasWidth == null) Init();

		if (dicAtlasWidth.ContainsKey(tex) == false)
		{
			Debug.LogError(tex + " 'dicAtlasWidth' not set");
			return;
		}
		width = dicAtlasWidth[tex];
		max = (width * width) -1;
		degree = 1f / dicAtlasWidth[tex];
		
		if (dicBaseUV.ContainsKey(filter.mesh.name) == false)
		{
			vecs2 = new Vector2[filter.mesh.uv.Length];
			for (num = 0; num < vecs2.Length; num++)
			{
				vecs2[num] = filter.mesh.uv[num];
			}
			dicBaseUV.Add(filter.mesh.name, vecs2);
		}

		vecs2 = new Vector2[filter.mesh.uv.Length];

		value--;
		for (num = 0; num < vecs2.Length; num++)
		{
			vecs2[num].x = dicBaseUV[filter.mesh.name][num].x * degree;
			vecs2[num].y = dicBaseUV[filter.mesh.name][num].y * degree;

			vecs2[num].x += ((value % width) * degree);
			vecs2[num].y += Mathf.FloorToInt((max - value) / width) * degree;
		}
		filter.mesh.uv = vecs2;
	}
}
