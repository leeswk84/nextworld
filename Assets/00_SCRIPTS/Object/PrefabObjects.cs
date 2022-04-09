using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabObjects : MonoBehaviour
{
	public GameObject obj;
	public GameObject[] objs;

	void Start ()
	{
		obj.SetActive(false);	
	}

	public void ApplyObjects()
	{
		/*
#if UNITY_EDITOR
		if (UnityEditor.EditorApplication.isPlaying == true)
		{	//플레이 중에는 수행하지 않도록...
			Debug.LogWarning("NOW PLAYING....");
			return;
		}
		for (int i = 0; i < objs.Length; i++)
		{
			//UnityEditor.PrefabUtility.PrefabInstanceUpdated(objs[i]);
			//GetCorrespondingObjectFromSource
			//UnityEditor.PrefabUtility.ReplacePrefab(objs[i], UnityEditor.PrefabUtility.GetPrefabParent(objs[i]), UnityEditor.ReplacePrefabOptions.ConnectToPrefab);
			UnityEditor.PrefabUtility.ReplacePrefab(objs[i], UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(objs[i]), UnityEditor.ReplacePrefabOptions.ConnectToPrefab);
			if (objs[i].GetComponent<Parts>() != null)
			{

			}
			UnityEditor.PrefabUtility.SaveAsPrefabAssetAndConnect(objs[i], "/Assets/03_PREFABS/", UnityEditor.InteractionMode.AutomatedAction);
		}
#endif
		*/
	}

}
