using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupItemDetail : UIPopupBase
{
	public Text txtDesc;
	public ButtonObject btnAction;

	private UISlot slot;
	private string cP_idx;
	private ITEM_DATA data;

	public override void Init()
	{
		type = TYPE.ItemDetail;
		base.Init();

		btnLock.obj.SetActive(false);
		btnAction.btn.onClick.AddListener(ClickAction);

		slot = GameObject.Instantiate(GameMgr.ins.mgrUI.status.popupBag.prefab.obj).GetComponent<UISlot>();
		slot.rect.SetParent(rect);
		slot.rect.localScale = Vector3.one;
		slot.rect.localPosition = Vector3.zero;
		slot.rect.anchoredPosition = new Vector2(160,-110);
		slot.obj.SetActive(true);
		slot.Init();
		slot.icon.ActiveDrag = false;
		slot.popup = this;
	}

	/// <summary>
	/// 팝업 띄우기
	/// </summary>
	/// <param name="P_idx">아이템 고유 값</param>
	public void Show(ITEM_DATA vData, string P_idx)
	{
		Show();
		
		cP_idx = P_idx;
		data = vData;

		slot.SetData(data, cP_idx);
		txtTitle.text = PLAY_DATA.ins.strItem.dic[data.stringID].name;
		txtDesc.text = PLAY_DATA.ins.strItem.dic[data.stringID].comment;

		btnAction.obj.SetActive(true);
		switch (data.type1)
		{
			case ITEM_DATA.TYPE_1.EQUIP:
				btnAction.txt.text = "EQUIP";
				if (PLAY_DATA.ins.status.equip[(int)data.type2] == P_idx) btnAction.obj.SetActive(false);
				break;
			case ITEM_DATA.TYPE_1.ITEM: btnAction.txt.text = "USE"; break;

		}

		//파츠와 지니는 사용 불가
		//if(data.type1 == ITEM_DATA.TYPE_1.EQUIP || data.type2 == ITEM_DATA.TYPE_2.GENIE) btnAction.obj.SetActive(false);
	}

	private void ClickAction()
	{
		switch (data.type1)
		{
			case ITEM_DATA.TYPE_1.EQUIP:
				SetEquip(ref slot.icon);
				btnAction.obj.SetActive(false);
				break;
			case ITEM_DATA.TYPE_1.ITEM:
				UseItem(cP_idx, data);
				break;
		}
		slot.SetData(data, cP_idx);
	}
}
