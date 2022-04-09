using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIObjectManager : MonoBehaviour
{
	public Transform tran;
	public UIObject[] prefab;

	public Color colorNut;

	private Dictionary<string, UIObject> dic_cre;
	private Dictionary<string, List<UIObject>> dic;
	private List<string> list_key;
	
	private string str;
	private int num, num2, lnum, lnum2, gnum;
	[HideInInspector]
	public Vector3 vec3;
	private UIObject tmp;

	public void Init()
	{
		list_key = new List<string>();
		dic = new Dictionary<string, List<UIObject>>();
		dic_cre = new Dictionary<string, UIObject>();

		for (num = 0; num < prefab.Length; num++)
		{
			str = prefab[num].obj.name;
			if (dic.ContainsKey(str)) continue;

			dic.Add(str, new List<UIObject>());
			list_key.Add(str);
			dic_cre.Add(str, prefab[num]);

			prefab[num].obj.SetActive(false);

			if (str == UIObject.UI_KEY_OUTLINE) continue;

			for (num2 = 0; num2 < 5; num2++)
			{
				tmp = CreateObject(str);
				tmp.obj.SetActive(false);
			}
		}
	}
	/// <summary>
	/// 이벤트 말 풍선 표시
	/// </summary>
	public void ShowEvent(string value, ref MiniNPC valueNPC)
	{
		if (valueNPC == null) return;
		dic[UIObject.UI_EVENT][0].obj.SetActive(true);
		//dic[UIObject.UI_EVENT][0].rect.localPosition = Change3D2D(ref valueTran);
		dic[UIObject.UI_EVENT][0].npc = valueNPC;
		dic[UIObject.UI_EVENT][0].tranTarget = valueNPC.mini.trans;
		dic[UIObject.UI_EVENT][0].txt.text = value;
		dic[UIObject.UI_EVENT][0].printText = value;
		dic[UIObject.UI_EVENT][0].degreeY = valueNPC.uihpY;
		dic[UIObject.UI_EVENT][0].UpdateObject();
	}

	public void ShowEvent(string value, Transform valueTran, int gapY = 0)
	{
		if (valueTran == null) return;
		dic[UIObject.UI_EVENT][0].obj.SetActive(true);
		//dic[UIObject.UI_EVENT][0].rect.localPosition = Change3D2D(ref valueTran);
		dic[UIObject.UI_EVENT][0].tranTarget = valueTran;
		dic[UIObject.UI_EVENT][0].txt.text = value;
		dic[UIObject.UI_EVENT][0].printText = value;
		dic[UIObject.UI_EVENT][0].degreeY = gapY;
		dic[UIObject.UI_EVENT][0].UpdateObject();
	}

	public void HideEvent()
	{
		dic[UIObject.UI_EVENT][0].Hide();
	}

	public void ShowText(int value, ref Transform valueTran, bool is_nut = false)
	{
		tmp = GetObject(UIObject.UI_KEY_TXT);
		tmp.rect.localPosition = Change3D2D(ref valueTran);
		tmp.txt.text = value.ToString();
		tmp.is_nut = is_nut;
		tmp.Show();
	}

	public UIObject GetObject(string value)
	{
		if (dic.ContainsKey(value) == false) return null;
		tmp = null;
		for (gnum = 0; gnum < dic[value].Count; gnum++)
		{
			if (dic[value][gnum].obj.activeSelf == true) continue;
			tmp = dic[value][gnum];
			break;
		}
		if (tmp == null) tmp = CreateObject(value);
		return tmp;
	}

	public UIObject CreateObject(string value)
	{
		if (dic_cre.ContainsKey(value) == false) return null;

		tmp = GameObject.Instantiate(dic_cre[value].obj).GetComponent<UIObject>();
		tmp.rect.SetParent(tran);
		tmp.rect.localScale = Vector3.one;
		tmp.rect.localPosition = Vector3.zero;
		tmp.obj.name = value;
		tmp.key = value;
		tmp.index = dic[value].Count;
		tmp.Init();

		dic[value].Add(tmp);
		return tmp;
	}
	
	public void UpdateObject()
	{
		for (lnum = 0; lnum < list_key.Count; lnum++)
		{
			if (dic.ContainsKey(list_key[lnum]) == false) continue;
			for (lnum2 = 0; lnum2 < dic[list_key[lnum]].Count; lnum2++)
			{
				if (dic[list_key[lnum]][lnum2].obj.activeSelf == false) continue;
				dic[list_key[lnum]][lnum2].UpdateObject();
			}
		}
	}

	public void Change3D2D(ref Vector3 value)
	{
		value = GameMgr.ins.mgrCam.cam.WorldToScreenPoint(value);
		ChangeMouse2D(ref value);
	}

	public void ChangeMouse2D(ref Vector3 value)
	{
		value.x -= GameMgr.ins.mgrCam.cam.pixelWidth * 0.5f;
		value.y -= GameMgr.ins.mgrCam.cam.pixelHeight * 0.5f;

		value.x /= GameMgr.ins.mgrUI.canvas.scaleFactor;
		value.y /= GameMgr.ins.mgrUI.canvas.scaleFactor;

		value.z = 0f;
	}

	public Vector3 Change3D2D(ref Transform valueTran, float editY = 0)
	{
		vec3 = valueTran.position;
		vec3.y += editY;

		Change3D2D(ref vec3);

		return vec3;
	}

}
