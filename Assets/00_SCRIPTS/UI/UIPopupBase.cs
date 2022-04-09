using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupBase : MonoBehaviour
{
	public enum TYPE
	{
		HotKey,
		Bag,
		ItemDetail,
		Quest,
		Shop,
		Skill,
		Status,
		Msg,
	}
	[HideInInspector]
	public TYPE type;
	public GameObject obj;
	public RectTransform rect;

	public ButtonObject btnClose;
	public ButtonObject btnLock;
	public ButtonObject btnDragBar;

	public Text txtTitle;
	[Header("")]
	private Vector3 startMouse;
	private Vector3 startPos;
	private Vector3 dragMouse;

	protected Vector3 vec3;

	private bool isDrag;
	protected float fnum;
	protected int num;
	protected string str;

	private Collider[] cols;
	private float distance;
	
	protected UISlot tmpSlot;
	private UISlot hitSlot;

	protected float screen_width;
	protected float screen_height;

	private ITEM_DATA tmpData;
	
	virtual public void Init()
	{
		isDrag = false;
		Hide();
		btnLock.btn.onClick.AddListener(ClickLock);
		btnClose.btn.onClick.AddListener(ClickClose);
		btnDragBar.fncPress = PressBack;
		btnLock.txt.text = "";
		screen_width = GameMgr.ins.mgrCam.cam.pixelWidth / GameMgr.ins.mgrUI.canvas.scaleFactor;
		screen_height = GameMgr.ins.mgrCam.cam.pixelHeight / GameMgr.ins.mgrUI.canvas.scaleFactor;
	}

	public void Toggle()
	{
		if (!obj.activeSelf) Show();
		else Hide();
	}

	public virtual void Show()
	{
		screen_width = GameMgr.ins.mgrCam.cam.pixelWidth / GameMgr.ins.mgrUI.canvas.scaleFactor;
		screen_height = GameMgr.ins.mgrCam.cam.pixelHeight / GameMgr.ins.mgrUI.canvas.scaleFactor;
		PosLimit(ref rect);
		obj.SetActive(true);
		SetDepthUp();
	}

	public virtual void ClickClose()
	{
		btnLock.txt.text = "";
		Hide();
	}

	public virtual void Hide()
	{
		obj.SetActive(false);
	}

	private void ClickLock()
	{
		btnLock.txt.text = btnLock.txt.text == "" ? "✔" : "";
	}

	private void PressBack(bool value)
	{
		isDrag = value;
		//Debug.Log(value);
		if (value)
		{
			SetDepthUp();
			startMouse = Input.mousePosition;
			GameMgr.ins.mgrUIObj.ChangeMouse2D(ref startMouse);
			startPos = rect.localPosition;
			StartCoroutine(DoDrag());
		}
	}
	
	public void SetDepthUp()
	{
		rect.SetParent(GameMgr.ins.mgrUI.rectLockPlane);
		rect.SetParent(GameMgr.ins.mgrUI.status.rectPopup);
		
		if (GameMgr.ins.mgrUI.status.popupStatus.obj.activeSelf)
		{	//* 그냥 카메라 뎁스 수정과 같은 모습.. (Status 창. 뎁스 뒤로 가면 카메라 내용 render texture 로 수정... ugui 뎁스와 연결하기 위해서)
			if (this == GameMgr.ins.mgrUI.status.popupStatus)
			{
				GameMgr.ins.PLAYER.cam_front.depth = 10;
				//GameMgr.ins.PLAYER.objCam_front.SetActive(true);
				//GameMgr.ins.PLAYER.objCam_back.SetActive(false);
				//GameMgr.ins.mgrUI.status.popupStatus.objCharacter.SetActive(false);
			}
			else
			{
				GameMgr.ins.PLAYER.cam_front.depth = 1;
				//GameMgr.ins.PLAYER.objCam_front.SetActive(false);
				//GameMgr.ins.PLAYER.objCam_back.SetActive(true);
				//GameMgr.ins.mgrUI.status.popupStatus.objCharacter.SetActive(true);
			}
		}
		
	}

	private IEnumerator DoDrag()
	{
		while (isDrag)
		{
			dragMouse = Input.mousePosition;
			GameMgr.ins.mgrUIObj.ChangeMouse2D(ref dragMouse);
			rect.localPosition = startPos - (startMouse - dragMouse);
			
			PosLimit(ref rect);
			DoDrag_ing();
			yield return new WaitForSeconds(0.01f);
		}
		yield break;
	}

	protected virtual void DoDrag_ing()
	{

	}

	public void PosLimit(ref RectTransform value)
	{
		vec3 = value.localPosition;

		vec3.x += ((GameMgr.ins.mgrCam.cam.pixelWidth * 0.5f) / GameMgr.ins.mgrUI.canvas.scaleFactor);
		fnum = screen_width - value.sizeDelta.x;
		if (vec3.x < 0) vec3.x = 0;
		if (vec3.x > fnum) vec3.x = fnum;
		vec3.x -= ((GameMgr.ins.mgrCam.cam.pixelWidth * 0.5f) / GameMgr.ins.mgrUI.canvas.scaleFactor);

		vec3.y -= ((GameMgr.ins.mgrCam.cam.pixelHeight * 0.5f) / GameMgr.ins.mgrUI.canvas.scaleFactor);
		fnum = screen_height - value.sizeDelta.y;
		if (vec3.y > 0) vec3.y = 0;
		if (vec3.y < -fnum) vec3.y = -fnum;
		vec3.y += ((GameMgr.ins.mgrCam.cam.pixelHeight * 0.5f) / GameMgr.ins.mgrUI.canvas.scaleFactor);

		value.localPosition = vec3;
	}

	public void CheckIcon(UIIcon value)
	{
		vec3.x = vec3.y = vec3.z = 0.39f;

		hitSlot = null;
		cols = Physics.OverlapBox(value.rect.position, vec3, Quaternion.identity, GameMgr.ins.mgrUI.layer_ui);
		if (cols != null && cols.Length > 0)
		{
			distance = 1000f;
			for (num = 0; num < cols.Length; num++)
			{
				tmpSlot = cols[num].transform.gameObject.GetComponent<UISlot>();
				if (tmpSlot != null)
				{
					if (tmpSlot == value.slot)
					{
						if (value.data.type1 == ITEM_DATA.TYPE_1.CODE)
						{	//코드는 바로 실행
							Debug.Log("CODE DOING :: " + value.data.type2);
							return;
						}

						switch (value.slot.popup.type)
						{
							case TYPE.HotKey:  //단축키 선택
							//Debug.Log("Click Hot Key");
							switch (value.data.type1)
							{
								case ITEM_DATA.TYPE_1.EQUIP: SetEquip(ref value); break; //장비 장착
								case ITEM_DATA.TYPE_1.ITEM: //아이템 사용
										if (UseItem(value.slot.P_idx, value.data) == false) return;
									/*
									if (PLAY_DATA.ins.status.item[value.slot.P_idx].cnt < 1) return;
									PLAY_DATA.ins.status.item[value.slot.P_idx].cnt--;
									GameMgr.ins.mgrUI.status.popupBag.RefreshTab();
									GameMgr.ins.mgrUI.status.hotKey.RefreshList();
									switch (value.data.type2)
									{
										case ITEM_DATA.TYPE_2.HP_UP:
											GameMgr.ins.mgrUI.status.EditHP(value.data.value);
											GameMgr.ins.mgrUIObj.ShowText(value.data.value, ref GameMgr.ins.PLAYER.mini.trans);
											break;
									}
									*/
									break;
							}
							break;
							case TYPE.Bag: //인벤토리 아이템 선택 
								switch (value.data.type1)
								{
									case ITEM_DATA.TYPE_1.EQUIP: 
									case ITEM_DATA.TYPE_1.ITEM://자세히 보기
										GameMgr.ins.mgrUI.status.popupItemDetail.Show(value.data, value.slot.P_idx);
										break;
								}
							break;
						}

						//Debug.Log("Click");
						return;
					}
					fnum = Vector3.Distance(value.rect.position, tmpSlot.rect.position);
					if (fnum < distance)
					{
						distance = fnum;
						hitSlot = tmpSlot;
					}
				}
			}
			
			if (hitSlot.popup == GameMgr.ins.mgrUI.status.popupStatus)
			{	//장착 장비 수정
				//Debug.Log("SET EQUIP");
				if (hitSlot.icon.data.type1 != value.data.type1 || hitSlot.icon.data.type2 != value.data.type2) return;
				SetEquip(ref value);
			}
			
			if (hitSlot.popup == GameMgr.ins.mgrUI.status.hotKey)
			{   //단축키에 설정
				tmpData = null;
				num = -1;
				str = string.Empty;
				if (hitSlot.icon.data != null)
				{
					tmpData = hitSlot.icon.data;
					str = hitSlot.P_idx;
				}
				hitSlot.SetData(value.data, value.slot.P_idx);
				if (value.slot.popup == GameMgr.ins.mgrUI.status.hotKey)
				{   //단축키에서 단축키 드래그의 경우 이전 내용은 없어 지도록
					if (tmpData != null) value.slot.SetData(tmpData, str); //서로 자리가 바뀌는 경우
					else value.SetBlank(); //이동하는 경우
				}

				GameMgr.ins.mgrUI.status.hotKey.UpdateData();
			}
			//hitSlot.icon.obj.SetActive(true);
			//hitSlot.icon.txt.text = value.txt.text;
			//Debug.Log(hitSlot.popup.obj.name);
			//Debug.Log(cols[0].transform.gameObject.name);
		}

		if (value.slot.popup == GameMgr.ins.mgrUI.status.hotKey && hitSlot == null)
		{   //단축키를 빈곳에 드래그 할 경우 사라지도록
			value.SetBlank();
			GameMgr.ins.mgrUI.status.hotKey.UpdateData();
			return;
		}
	}
	/// <summary>
	/// 아이템 사용
	/// </summary>
	/// <param name="P_idx"></param>
	/// <param name="data"></param>
	public bool UseItem(string P_idx, ITEM_DATA data)
	{
		if (PLAY_DATA.ins.status.item[P_idx].cnt < 1) return false;
		PLAY_DATA.ins.status.item[P_idx].cnt--;
		GameMgr.ins.mgrUI.status.popupBag.RefreshTab();
		GameMgr.ins.mgrUI.status.hotKey.RefreshList();
		switch (data.type2)
		{
			case ITEM_DATA.TYPE_2.HP_UP:
				GameMgr.ins.mgrUI.status.EditHP(data.value);
				GameMgr.ins.mgrUIObj.ShowText(data.value, ref GameMgr.ins.PLAYER.mini.trans);
				break;
		}

		return true;
	}
	/// <summary> 장비 설정 </summary>
	public void SetEquip(ref UIIcon value)
	{
		bool before_is_outline = GameMgr.ins.PLAYER_MINI.is_show_outline;

		//PLAY_DATA.ins.status.equip[(int)value.data.type2] = value.data.id;// id;
		//PLAY_DATA.ins.status.equip_item_index[(int)value.data.type2] = value.slot.P_idx;
		PLAY_DATA.ins.status.equip[(int)value.data.type2] = value.slot.P_idx;
		
		GameMgr.ins.mgrUI.status.popupStatus.RefreshEquip();
		GameMgr.ins.PLAYER.RefreshPart(value.data.type2);
		GameMgr.ins.PLAYER.mini.RefreshMaxAbility();
		GameMgr.ins.mgrUI.status.popupBag.RefreshTab();
		GameMgr.ins.mgrUI.status.hotKey.RefreshList();
		GameMgr.ins.PLAYER_MINI.SetOutLine(before_is_outline);
	}

	public void IconDrag(UIIcon value)
	{
		Transform rectParent = value.slot.rect.parent;
		
		//value.slot.rect.SetParent(GameMgr.ins.mgrUI.rectLockPlane);
		//value.slot.rect.SetParent(rectParent);

		value.rect.SetParent(value.slot.popup.rect);
		value.slot.popup.SetDepthUp();

		//StartCoroutine(DoDragWait(value));
		IconDepthSet(value);
		StartCoroutine(DoIconDrag(value));
	}

	private IEnumerator DoDragWait(UIIcon value)
	{
		yield return new WaitForSeconds(0.2f);

		if (value.isDrag == false) yield break;
		IconDepthSet(value);
		yield break;
	}

	private void IconDepthSet(UIIcon value)
	{
		StartCoroutine(DoIconDrag(value));
	}

	private IEnumerator DoIconDrag(UIIcon value)
	{
		Vector3 startMouse = Input.mousePosition;
		GameMgr.ins.mgrUIObj.ChangeMouse2D(ref startMouse);
		Vector3 startPos = value.rect.localPosition;
		Vector3 dragMouse;

		while (value.isDrag)
		{
			dragMouse = Input.mousePosition;
			GameMgr.ins.mgrUIObj.ChangeMouse2D(ref dragMouse);
			value.rect.localPosition = startPos - (startMouse - dragMouse);
			yield return new WaitForSeconds(0.01f);
		}
		yield break;
	}
}
