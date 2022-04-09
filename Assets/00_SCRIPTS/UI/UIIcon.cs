using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIIcon : MonoBehaviour
{
	public GameObject obj;
	public RectTransform rect;
	public RectTransform rectImg;
	public Text txt;
	public Image img;
	public Image imgBg;
	public ButtonObject btn;
	[HideInInspector]
	public UISlot slot;
	
	[HideInInspector]
	public bool ActiveDrag;
	[HideInInspector]
	public bool isDrag;

	[HideInInspector]
	public ITEM_DATA data;
	
	public void Init()
	{
		ActiveDrag = false;
		obj.SetActive(false);
		isDrag = false;
		txt.text = "";
		btn.fncPress = PressBack;
		//img.sprite = GameMgr.ins.uiatlas.GetSprite("ICON_BODY001");
	}

	private void PressBack(bool value)
	{
		if (ActiveDrag == false) return;
		
		isDrag = value;
		
		if (value)
		{
			/*
			Transform rectParent = slot.rect.parent;
			slot.rect.SetParent(GameMgr.ins.mgrUI.rectLockPlane);
			slot.rect.SetParent(rectParent);

			rect.SetParent(slot.popup.rect);
			slot.popup.SetDepthUp();
			StartCoroutine(DoDragIcon());
			*/
			if (GameMgr.ins.mgrUI.status.popupItemDetail.obj.activeSelf) GameMgr.ins.mgrUI.status.popupItemDetail.Hide();
			if (data.type1 == ITEM_DATA.TYPE_1.EQUIP || data.type2 == ITEM_DATA.TYPE_2.GENIE) return;
			slot.popup.IconDrag(this);
		}
		else
		{
			slot.popup.CheckIcon(this);
			rect.SetParent(slot.rect);
			rect.localPosition = Vector3.zero;

			slot.rectFrame.SetParent(GameMgr.ins.mgrUI.rectLockPlane);
 			slot.rectFrame.SetParent(slot.rect);
		}
	}
	
	private IEnumerator DoDragIcon()
	{
		//yield return new WaitForSeconds(1f);
		
		Vector3 startMouse = Input.mousePosition;
		GameMgr.ins.mgrUIObj.ChangeMouse2D(ref startMouse);
		Vector3 startPos = rect.localPosition;
		Vector3 dragMouse;

		while (isDrag)
		{
			dragMouse = Input.mousePosition;
			GameMgr.ins.mgrUIObj.ChangeMouse2D(ref dragMouse);

			//Debug.Log(dragMouse);
			rect.localPosition = startPos - (startMouse - dragMouse);
			//popup.PosLimit(ref rect);
			yield return new WaitForSeconds(0.01f);
		}
		//Debug.Log("AA");

		yield break;
	}

	/// <summary>
	/// 비어 있는 슬롯으로 설정
	/// </summary>
	public void SetBlank()
	{
		data = null;
		slot.P_idx = string.Empty;
		obj.SetActive(false);
	}
}
