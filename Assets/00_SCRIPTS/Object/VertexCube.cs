using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexCube : MonoBehaviour
{
	public GameObject obj;
	public Transform tran;
	[HideInInspector]
	public MeshAtlas atlas;

	public void OnSelect()
	{
		string str = atlas.PRINT_VERTEX;
		//str += obj.name.Substring(6);

		for (int i = 0; i < atlas.listVertex.Count; i++)
		{
			if (atlas.listVertex[i].tran.position == tran.position)
			{
				if (str != "") str += ",";
				str += i.ToString();
			}
		}
		atlas.PRINT_VERTEX = str;
	}
	
}
