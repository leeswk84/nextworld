using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VertexCube))]
public class VertexCubeEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		VertexCube myScript = (VertexCube)target;
		if (GUILayout.Button("SELECT"))
		{
			myScript.OnSelect();
		}
	}
}
