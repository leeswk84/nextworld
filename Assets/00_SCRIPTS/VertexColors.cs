using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class VertexColors : MonoBehaviour 
{
	public Color col = Color.grey;

	private Mesh mesh;
	private Color[] cols;
	private int i;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(mesh == null) mesh = GetComponent<MeshFilter>().mesh;
		if(cols == null) cols = new Color[mesh.vertices.Length];

		i = 0;
		while (i < mesh.vertices.Length)
		{
			cols[i] = col;
			i++;
		}
		mesh.colors = cols;
		

	}
}
