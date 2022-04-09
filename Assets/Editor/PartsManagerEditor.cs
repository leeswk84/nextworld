using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PartsManager))]
public class PartsManagerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		if (GUILayout.Button("프리팹 설정", GUILayout.Height(20)))
		{
			PartsManager mgr = (PartsManager)target;
			string[] guids = AssetDatabase.FindAssets("", new string[] { "Assets/03_PREFABS/Parts" });
			string[] paths = System.Array.ConvertAll<string, string>(guids, AssetDatabase.GUIDToAssetPath);
			mgr.listParts.Clear();
			for (int i = 0; i < paths.Length; i++)
			{
				//Debug.Log(paths[i]);
				mgr.listParts.Add(AssetDatabase.LoadAssetAtPath<Parts>(paths[i]));
			}

			//Debug.Log(paths.Length);
			//Parts[] arrParts = AssetDatabase.LoadAllAssetsAtPath("Assets/03_PREFABS/Parts", typeof(Parts));
			//Debug.Log(arrParts.Length);
			
		}
	}
}
