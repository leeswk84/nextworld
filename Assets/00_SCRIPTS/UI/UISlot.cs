using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISlot : MonoBehaviour
{
	public GameObject obj;
	public RectTransform rect;
	public BoxCollider col;
	public UIIcon icon;
	public Text txtE;

	public GameObject objLevel;
	public Text txtLvDesc;
	public Text txtLv;

	public GameObject objBg;
	public RectTransform rectFrame;

	[HideInInspector]
	public UIPopupBase popup;

	//[HideInInspector]
	public string P_idx;

	private Vector3 vec;
	
	public void Init()
	{
		P_idx = string.Empty;
		txtE.text = string.Empty;
		objLevel.SetActive(false);
		icon.slot = this;
		icon.Init();
	}

	public void SetData(ITEM_DATA value, string idx)
	{
		P_idx = idx;
		icon.data = value;
		icon.obj.SetActive(true);

		//Debug.Log(PLAY_DATA.ins.dataItem.dic[PLAY_DATA.ins.status.item[snum].idx].icon);
		//Debug.Log(icon.data.icon);
		icon.img.sprite = GameMgr.ins.uiatlas.GetSprite(icon.data.icon);

		switch(icon.data.type2)
		{
			case ITEM_DATA.TYPE_2.ARMR:
				vec.x = 0; vec.y = 0; vec.z = 280;
				icon.rectImg.eulerAngles = vec;
				vec.x = -1; vec.y = vec.z = 1;
				icon.rectImg.localScale = vec;
				break;
			case ITEM_DATA.TYPE_2.ARML:
				icon.rectImg.localScale = Vector3.one;
				vec.x = 0; vec.y = 0; vec.z = 20;
				icon.rectImg.eulerAngles = vec;
				break;
			default:
				icon.rectImg.localScale = Vector3.one;
				icon.rectImg.eulerAngles = Vector3.zero;
				break;
		}
		RefreshData();
	}

	public void RefreshData()
	{
		//txtE.text = PLAY_DATA.ins.status.item[P_idx].cnt.ToString();
		//icon.txt.text =	PLAY_DATA.ins.status.item[P_idx].idx.ToString();
		//icon.txt.text = PLAY_DATA.ins.strItem.dic[data.id].name;
		if (icon.obj.activeSelf == false || icon.data == null) return;

		icon.txt.text = "";
		objLevel.SetActive(false);
		switch (icon.data.type1)
		{
			case ITEM_DATA.TYPE_1.ITEM:
				icon.txt.text = PLAY_DATA.ins.status.item[P_idx].cnt.ToString();
				break;
			case ITEM_DATA.TYPE_1.EQUIP:
				if (popup != GameMgr.ins.mgrUI.status.popupStatus
					&& PLAY_DATA.ins.status.equip[(int)icon.data.type2] == icon.slot.P_idx) icon.txt.text = "ⓔ";//"◆";//"✔";
					//&& PLAY_DATA.ins.status.equip[(int)icon.data.type2] == icon.data.id) icon.txt.text = "ⓔ";//"◆";//"✔";
				objLevel.SetActive(true);

				txtLvDesc.text = "lv.";
				txtLv.text = PLAY_DATA.ins.status.item[P_idx].level.ToString();

				break;
		}
	}
}
