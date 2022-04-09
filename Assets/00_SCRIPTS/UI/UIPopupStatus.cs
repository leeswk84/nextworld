using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupStatus : UIPopupBase
{
	public UISlot[] slots;
	private Rect cam_rect_back;
	private Rect cam_rect_front;
	
	public GameObject objCharacter;
	public RectTransform rectCharacter;

	override public void Init()
	{
		type = TYPE.Status;
		base.Init();
		objCharacter.SetActive(false);

		for (num = 0; num < slots.Length; num++)
		{
			slots[num].Init();
			slots[num].popup = this;
		}
		cam_rect_back = new Rect();
		vec3.x = vec3.y = 1;
		cam_rect_back.size = vec3;
		vec3.x = vec3.y = 0;
		cam_rect_back.position = vec3;
		GameMgr.ins.PLAYER.cam_back.rect = cam_rect_back;

		cam_rect_front = new Rect();
		vec3.x = rectCharacter.sizeDelta.x / screen_width;
		vec3.y = rectCharacter.sizeDelta.y / screen_height;
		cam_rect_front.size = vec3;
		GameMgr.ins.PLAYER.cam_front.rect = cam_rect_front;
	}

	public override void Show()
	{
		base.Show();
		RefreshEquip();

		GameMgr.ins.PLAYER.objCam_front.SetActive(true);
		GameMgr.ins.PLAYER.objCam_back.SetActive(false);
		objCharacter.SetActive(false);

		DoDrag_ing();
	}

	public override void Hide()
	{
		base.Hide();
		GameMgr.ins.PLAYER.objCam_back.SetActive(false);
		GameMgr.ins.PLAYER.objCam_front.SetActive(false);
		objCharacter.SetActive(false);
	}

	protected override void DoDrag_ing()
	{   //드래그 중 캐릭터 카메라 업데이트
		vec3.x = (rect.anchoredPosition.x / screen_width) + 0.09f;
		vec3.y = (1 + (rect.anchoredPosition.y / screen_height)) - 0.28f; //- 0.35f + 0.01f;
		cam_rect_front.position = vec3;
		GameMgr.ins.PLAYER.cam_front.rect = cam_rect_front;
	}
	/// <summary>
	/// 장비 설정 새로 고침
	/// </summary>
	public void RefreshEquip()
	{
		for (num = 0; num < slots.Length; num++)
		{
			slots[num].SetData(PLAY_DATA.ins.dataItem.dic[PLAY_DATA.ins.status.item[PLAY_DATA.ins.status.equip[num]].idx], PLAY_DATA.ins.status.equip[num]);
		}
		SetDepthUp();
	}
}
