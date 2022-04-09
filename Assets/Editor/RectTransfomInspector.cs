/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(RectTransform), true)]
public class RectTransformInspector : Editor
{
	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI();
		if (target == null) return;
		//if (typeof(Editor).Assembly.GetType("UnityEditor.RectTransformEditor") == null) return;
		Editor tmp = CreateEditor(targets, typeof(Editor).Assembly.GetType("UnityEditor.RectTransformEditor"));
		if (tmp == null) return;
		tmp.OnInspectorGUI();
		
		EditorGUILayout.BeginHorizontal();
		GUI.backgroundColor = new Color(0.8f, 1, 0);

		if (GUILayout.Button("Position", EditorStyles.miniButtonLeft))
		{
			((RectTransform)target).localPosition = new Vector3(Mathf.RoundToInt(((RectTransform)target).localPosition.x),
																Mathf.RoundToInt(((RectTransform)target).localPosition.y),
																Mathf.RoundToInt(((RectTransform)target).localPosition.z));

			((RectTransform)target).anchoredPosition3D = new Vector3(Mathf.RoundToInt(((RectTransform)target).anchoredPosition3D.x),
																Mathf.RoundToInt(((RectTransform)target).anchoredPosition3D.y),
																Mathf.RoundToInt(((RectTransform)target).anchoredPosition3D.z));
			
			//((RectTransform)target).anchorMax = new Vector2(	Mathf.RoundToInt(((RectTransform)target).anchorMax.x),
			//													Mathf.RoundToInt(((RectTransform)target).anchorMax.y));

			//((RectTransform)target).anchorMin = new Vector2(Mathf.RoundToInt(((RectTransform)target).anchorMin.x),
			//													Mathf.RoundToInt(((RectTransform)target).anchorMin.y));
			
			
			((RectTransform)target).sizeDelta = new Vector2(Mathf.RoundToInt(((RectTransform)target).sizeDelta.x),
																Mathf.RoundToInt(((RectTransform)target).sizeDelta.y));

		}
		if (GUILayout.Button("Rotation", EditorStyles.miniButtonMid))
		{
			((RectTransform)target).localRotation = Quaternion.identity;
		}
		if (GUILayout.Button("Scale", EditorStyles.miniButtonRight))
		{
			((RectTransform)target).localScale = Vector3.one;
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("All", EditorStyles.miniButton))
		{
			((RectTransform)target).localPosition = new Vector3(Mathf.RoundToInt(((RectTransform)target).localPosition.x),
																Mathf.RoundToInt(((RectTransform)target).localPosition.y),
																Mathf.RoundToInt(((RectTransform)target).localPosition.z));

			((RectTransform)target).anchoredPosition3D = new Vector3(Mathf.RoundToInt(((RectTransform)target).anchoredPosition3D.x),
																Mathf.RoundToInt(((RectTransform)target).anchoredPosition3D.y),
																Mathf.RoundToInt(((RectTransform)target).anchoredPosition3D.z));
			
			//((RectTransform)target).anchorMax = new Vector2(	Mathf.RoundToInt(((RectTransform)target).anchorMax.x),
			//													Mathf.RoundToInt(((RectTransform)target).anchorMax.y));

			//((RectTransform)target).anchorMin = new Vector2(Mathf.RoundToInt(((RectTransform)target).anchorMin.x),
			//													Mathf.RoundToInt(((RectTransform)target).anchorMin.y));
			

			((RectTransform)target).sizeDelta = new Vector2(Mathf.RoundToInt(((RectTransform)target).sizeDelta.x),
																Mathf.RoundToInt(((RectTransform)target).sizeDelta.y));

			((RectTransform)target).localRotation = Quaternion.identity;
			((RectTransform)target).localScale = Vector3.one;
		}
		EditorGUILayout.EndHorizontal();
	}
}
*/