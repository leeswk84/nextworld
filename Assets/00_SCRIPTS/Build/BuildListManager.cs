using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildListManager : MonoBehaviour
{
	public List<Build> listPrefab;
	private Dictionary<int, List<Build>> dic;
	private Build tmp;

	public void Init()
	{
#if UNITY_EDITOR
		listPrefab = new List<Build>();
		//유니티에서 실행했을 경우 폴더에 있는 내용 list에 적용 해서 prefab applay
		string[] strs = System.IO.Directory.GetFiles(Application.dataPath + "/03_PREFABS/Builds/");
		Build tmp;
		for (int i = 0; i < strs.Length; i++)
		{
			if (strs[i].Contains(".meta")) continue;
			strs[i] = strs[i].Substring(strs[i].IndexOf("Assets/03_PREFABS/"));
			strs[i].Replace("\\","/");

			tmp = UnityEditor.AssetDatabase.LoadAssetAtPath(strs[i], typeof(Build)) as Build;
			if (tmp != null) listPrefab.Add(tmp);
			//Debug.Log(strs[i]);	
		}
		//GetCorrespondingObjectFromSource
		//UnityEditor.PrefabUtility.ReplacePrefab(gameObject, UnityEditor.PrefabUtility.GetPrefabParent(gameObject), UnityEditor.ReplacePrefabOptions.ConnectToPrefab);

		//프리팹 설정 내용
		//UnityEditor.PrefabUtility.ReplacePrefab(gameObject, UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(gameObject), UnityEditor.ReplacePrefabOptions.ConnectToPrefab);
#endif
	}
	
	public Build GetBuild(int value)
	{
		if (value < 0) return null;
		if (listPrefab.Count <= value) return null;
		tmp = GameObject.Instantiate(listPrefab[value]).GetComponent<Build>();
		tmp.prefab = listPrefab[value].obj;

		return tmp;
	}

	public void HideBuild()
	{

	}

}
