using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BONE
{
	None = -1,
	Part = 0,
	Footsteps,
	Peivis,
	Spine,

	LThigh,
	LCalf,
	LFoot,
	LToe,
	LToeNub,

	RThigh,
	RCalf,
	RFoot,
	RToe,
	RToeNub,

	Spine1,
	Neck,
	Head,
	HeadNub,

	LClavicle,
	LUpperArm,
	LForeArm,
	LHand,
	LFinger0,
	LFinger0Nub,
	LFinger1,
	LFinger1Nub,

	RClavicle,
	RUpperArm,
	RForeArm,
	RHand,
	RFinger0,
	RFinger0Nub,
	RFinger1,
	RFinger1Nub,

	Max,
}

public class PartsManager : MonoBehaviour
{
	public List<Parts> listParts;
	public Dictionary<BONE, List<Parts>> dicParts;

	public Material matOutline;
	public PartsOutLine prefabOutLine;
	//private Parts tmpPart;

	private int num;

	private BONE prefabbone;

	public void Init()
	{
		/*
#if UNITY_EDITOR
		listParts = new List<Parts>();
		//유니티에서 실행했을 경우 폴더에 있는 내용 list에 적용 해서 prefab applay
		string[] strs = System.IO.Directory.GetFiles(Application.dataPath + "/03_PREFABS/Parts/");
		Parts tmp;
		for (int i = 0; i < strs.Length; i++)
		{
			if (strs[i].Contains(".meta")) continue;
			strs[i] = strs[i].Substring(strs[i].IndexOf("Assets/03_PREFABS/"));
			strs[i].Replace("\\", "/");

			tmp = UnityEditor.AssetDatabase.LoadAssetAtPath(strs[i], typeof(Parts)) as Parts;
			if (tmp != null) listParts.Add(tmp);
			//Debug.Log(strs[i]);	
		}
		//GetCorrespondingObjectFromSource
		//UnityEditor.PrefabUtility.ReplacePrefab(gameObject, UnityEditor.PrefabUtility.GetPrefabParent(gameObject), UnityEditor.ReplacePrefabOptions.ConnectToPrefab);

		//프리팹 설정 내용
		//UnityEditor.PrefabUtility.ReplacePrefab(gameObject, UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(gameObject), UnityEditor.ReplacePrefabOptions.ConnectToPrefab);

#endif
		*/
		dicParts = new Dictionary<BONE, List<Parts>>();

		for (num = 0; num < listParts.Count; num++)
		{
			if (dicParts.ContainsKey(listParts[num].bone) == false)
			{
				dicParts.Add(listParts[num].bone, new List<Parts>());
			}
			dicParts[listParts[num].bone].Add(listParts[num]);
		}

	}
	
	public void SetPart(BONE bone, ref Mini mini, int value	, bool isPrefab = false)
	{
		prefabbone = bone;
		switch (prefabbone)
		{
			case BONE.RUpperArm: prefabbone = BONE.LUpperArm; break;
			case BONE.RForeArm: prefabbone = BONE.LForeArm; break;
			case BONE.RHand: prefabbone = BONE.LHand; break;
			case BONE.RCalf: prefabbone = BONE.LCalf; break;
			case BONE.RThigh: prefabbone = BONE.LThigh; break;
			case BONE.RFoot: prefabbone = BONE.LFoot; break;
		}
		//Debug.Log(bone+":"+value);
		//tmpPart = ((GameObject)GameObject.Instantiate(dicParts[prefabbone][value-1].obj)).GetComponent<Parts>();
		//tmpPart.obj.name = dicParts[prefabbone][value - 1].obj.name;
		//mini.SetPart(ref tmpPart, bone, ref dicParts[prefabbone][value-1].obj, isPrefab);
		//Debug.Log(dicParts[prefabbone].Count);
		//Debug.Log(value - 1);
		if (dicParts[prefabbone].Count <= value - 1)
		{
			return;
		}
		if (value - 1 < 0)
		{
			mini.RemovePart(bone, string.Empty);
			return;
		}
		mini.SetPart(bone, ref dicParts[prefabbone][value - 1].obj, isPrefab);
	}
}
