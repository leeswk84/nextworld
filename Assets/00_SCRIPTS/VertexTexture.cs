using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexTexture : MonoBehaviour
{
	private Mesh filter_mesh;
	public MeshFilter filter;
	
	void Start()
	{
		filter_mesh = filter.mesh;
		Color[] cols = new Color[filter_mesh.vertices.Length];
		
		cols[0] = Color.green;
		cols[1] = Color.red;
		cols[2] = Color.blue;
		cols[3] = Color.black;
		
		cols[0] = cols[1] = cols[2] = cols[3] = Color.white;
		//cols[0] = cols[1] = cols[2] = cols[3] = Color.red;

		cols[3] = Color.gray;

		filter_mesh.colors = cols;
		
		Vector2[] buv = new Vector2[filter_mesh.uv.Length];
		buv[0] = new Vector2(0f, 0f);
		buv[1] = new Vector2(1f, 1f);
		buv[2] = new Vector2(1f, 0f);
		buv[3] = new Vector2(0f, 1f);
		
		Vector2[] uv = new Vector2[filter_mesh.uv.Length];
		Vector2[] uv2 = new Vector2[filter_mesh.uv.Length];
		Vector2[] uv3 = new Vector2[filter_mesh.uv.Length];
		Vector2[] uv4 = new Vector2[filter_mesh.uv.Length];

		for (int i = 0; i < uv.Length; i++)
		{
			uv[i] = new Vector2(buv[i].x * 0.5f, buv[i].y * 0.5f);
			uv2[i] = new Vector2(buv[i].x * 0.5f, buv[i].y * 0.5f + 0.5f);
			uv3[i] = new Vector2(buv[i].x * 0.5f, buv[i].y * 0.5f);
			uv4[i] = new Vector2(buv[i].x * 0.5f, buv[i].y * 0.5f + 0.5f);
		}
		filter_mesh.uv = uv;
		filter_mesh.uv2 = uv2;
		filter_mesh.uv3 = uv3;
		filter_mesh.uv4 = uv4;
	}
}
