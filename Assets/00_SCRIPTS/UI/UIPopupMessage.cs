using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopupMessage : UIPopupBase
{
	public enum MSG_TYPE
	{
		None = 0,
		Network,
		Save,
		Restart,
		MaX
	}

	private MSG_TYPE msgType;

	public ButtonObject btnPlane;
	
	public UnityEngine.UI.InputField inputName;

	public ButtonObject btnConnect;
	public ButtonObject btnExit;

	public ButtonObject btnUpload;
	public ButtonObject btnShop;
	public ButtonObject btnRestart;

	public UnityEngine.UI.Text txtComment;

	public override void Init()
	{
		type = TYPE.Msg;
		base.Init();
		btnLock.obj.SetActive(false);

		btnPlane.btn.onClick.AddListener(ClickClose);
		btnConnect.btn.onClick.AddListener(ClickConnect);
		btnExit.btn.onClick.AddListener(ClickExit);
		btnShop.btn.onClick.AddListener(ClickShop);
		btnUpload.btn.onClick.AddListener(ClickUpload);
		btnRestart.btn.onClick.AddListener(ClickRestart);
	}

	public void Show(MSG_TYPE msgType)
	{
		this.msgType = msgType;

		base.Show();

		btnExit.obj.SetActive(false);
		btnConnect.obj.SetActive(false);
		btnShop.obj.SetActive(false);
		btnUpload.obj.SetActive(false);
		btnRestart.obj.SetActive(false);

		btnClose.obj.SetActive(true);

		switch (msgType)
		{
			case MSG_TYPE.Network:
				if (GameMgr.ins.photon.CheckInRoom())
				{
					txtComment.text = "Do you want to disconnect?";
					btnExit.obj.SetActive(true);
					//objExit.SetActive(true);
					//objConnect.SetActive(false);
				}
				else
				{
					txtComment.text = "Do you want to connect?";
					btnConnect.obj.SetActive(true);
					//objConnect.SetActive(true);
					//objExit.SetActive(false);
				}
				break;
			case MSG_TYPE.Save:
				txtComment.text = string.Empty;
				btnShop.obj.SetActive(true);
				btnUpload.obj.SetActive(true);
				break;
			case MSG_TYPE.Restart:
				txtComment.text = string.Empty;
				btnClose.obj.SetActive(false);
				btnRestart.obj.SetActive(true);
				break;
		}
	}
	
	public void ClickConnect()
	{
		/*
		if (inputName.text == string.Empty)
		{
			GameMgr.ins.mgrUI.popup.Show("Not Empty.");
			return;
		}
		GameMgr.ins.photon.ConnetMatser(inputName.text);
		*/
		//접속
		GameMgr.ins.photon.ConnetMatser(PLAY_DATA.ins.status.userID);
		GameMgr.ins.mgrUI.popup.ShowLock(TxtMgr.TYPE.CONNECTING);
	}

	public void ClickExit()
	{
		GameMgr.ins.photon.ClickExit();
		GameMgr.ins.mgrUI.popup.Close();
		ClickClose();
	}
	//
	/// <summary>
	/// 방 진입
	/// </summary>
	public void OnConnectRoom()
	{
		GameMgr.ins.mgrUI.popup.Close();
		ClickClose();
	}

	public void ClickUpload()
	{
		//GameMgr.ins.talk.interBuild.ani.SetBool("is_active", true);
		ClickClose();

		Debug.Log("plane_idx::" + GameMgr.ins.mgrSave.PLANET_IDX 
				+ ",TileIdx::" + GameMgr.ins.talk.interBuild.TileIdx
				+ ",listIdx::" +GameMgr.ins.talk.interBuild.listIdx 
				+ ",pos::" + GameMgr.ins.talk.interBuild.pos);

		PLAY_DATA.ins.status.savepoint[0] = GameMgr.ins.mgrSave.PLANET_IDX;
		PLAY_DATA.ins.status.savepoint[1] = GameMgr.ins.talk.interBuild.TileIdx;
		PLAY_DATA.ins.status.savepoint[2] = GameMgr.ins.talk.interBuild.pos;

		GameMgr.ins.mgrSave.ShowLock(() =>
		{
			//화면설정
			GameMgr.ins.mgrNpc.Rebirth();
			GameMgr.ins.mgrUI.status.EditHP(Mathf.FloorToInt(GameMgr.ins.PLAYER_MINI.ability.HPmax));
			GameMgr.ins.mgrUI.status.EditMP(Mathf.FloorToInt(GameMgr.ins.PLAYER_MINI.ability.MPmax));
			GameMgr.ins.mgrSave.HideLock();
			GameMgr.ins.talk.interBuild.CheckCurrentPoint();
		});
	}

	public override void ClickClose()
	{
		if (msgType == MSG_TYPE.Restart) return;
		base.ClickClose();
	}

	public void ClickRestart()
	{
		base.ClickClose();

		//GameMgr.ins.mgrSave.ShowLock(() =>
		GameMgr.ins.mgrUI.FogShow(()=>
		{
			GameMgr.ins.mgrEffect.ReturnAll();
			GameMgr.ins.mgrBullet.ReturnAll();
			GameMgr.ins.mgrNpc.Rebirth();
			GameMgr.ins.mgrUI.status.EditHP(Mathf.FloorToInt(GameMgr.ins.PLAYER_MINI.ability.HPmax));
			GameMgr.ins.mgrUI.status.EditMP(Mathf.FloorToInt(GameMgr.ins.PLAYER_MINI.ability.MPmax));
			PLAY_DATA.ins.MOVE_DIRECT = true;
			GameMgr.ins.mgrSave.SetBlockIdx(PLAY_DATA.ins.status.savepoint[1], false);

			//GameMgr.ins.mgrSave.HideLock(false);
			GameMgr.ins.mgrUI.FogHide(false);
			//등장 연출
			GameMgr.ins.PLAYER.ShowIntro();
		});
	}

	public void ClickShop()
	{
		if(PLAY_DATA.ins.status.dicItembox.ContainsKey(PLAY_DATA.ins.C_PLANET))
		{
			PLAY_DATA.ins.status.dicItembox.Remove(PLAY_DATA.ins.C_PLANET);
		}
		ClickRestart();
	}

}
