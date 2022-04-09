using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PrefabObjects))]
public class PrefabObjectsEditor : Editor
{
	/*
	static PrefabObjectsEditor()
	{
		Debug.Log("aaa");
		EditorApplication.playmodeStateChanged += OnChangePlay;
	}

	private static void OnChangePlay()
	{
		Debug.Log(EditorApplication.isPlaying);
	}
	*/

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		PrefabObjects myScript = (PrefabObjects)target;
		/*
		if (GUILayout.Button("APPLY PREFABS"))
		{
			myScript.ApplyObjects();
		}
		*/
	}
}
