using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupHotKey : UIPopupBase
{
	public UISlot prefab;
	public RectTransform rectList;
	private List<UISlot> list;

	public override void Init()
	{
		type = TYPE.HotKey;
		list = new List<UISlot>();

		prefab.objBg.SetActive(false);
		prefab.icon.obj.SetActive(false);

		vec3 = Vector3.zero;
		UISlot tmp;
		for (num = 0; num < 10; num++)
		{
			tmp = GameObject.Instantiate(prefab.obj).GetComponent<UISlot>();
			tmp.rect.SetParent(rectList);
			tmp.rect.localScale = Vector3.one;
			vec3.x = (num * + 60) -270;
			tmp.rect.localPosition = vec3;
			tmp.Init();
			tmp.icon.ActiveDrag = true;
			tmp.popup = this;
			list.Add(tmp);
		}
		prefab.obj.SetActive(false);
	}

	public void RefreshList()
	{
		for (num = 0; num < list.Count; num++)
		{
			list[num].RefreshData();
		}
	}

	public void UpdateData()
	{
		PLAY_DATA.ins.status.Hotkey = new string[list.Count];
		for (num = 0; num < list.Count; num++)
		{
			PLAY_DATA.ins.status.Hotkey[num] = list[num].P_idx;
		}
	}

	public void LoadData()
	{
		for (num = 0; num < list.Count; num++)
		{
			//Debug.Log(PLAY_DATA.ins.status.Hotkey[num]);
			if (PLAY_DATA.ins.status.Hotkey[num] == string.Empty) continue;
			list[num].SetData(PLAY_DATA.ins.dataItem.dic[PLAY_DATA.ins.status.item[PLAY_DATA.ins.status.Hotkey[num]].idx], PLAY_DATA.ins.status.Hotkey[num]);
		}
	}

}
