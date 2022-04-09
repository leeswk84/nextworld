using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITalkSmall : MonoBehaviour
{
	public RectTransform rect;

	//private List<UITalk> list;

	private Vector3 vec3;
	private int num;
	public ButtonObject btnTalk;
	
	public ButtonObject btnTalkNext;
	public UITalk mainTalk;

	private const string STR_NET = "☎";
	private const string STR_TALK = "···";
	private const string STR_ATTACK = "🜸";
	private const string STR_ACTIVE = "!";

	private UITalk tmp;
	[HideInInspector]
	public int index_talk;
	[HideInInspector]
	public MiniNPC npcTalk;

	public Build interBuild;

	public void Init()
	{
		//list = new List<UITalk>();
		
		mainTalk.obj.SetActive(false);
		btnTalk.objMain.SetActive(false);
		btnTalkNext.obj.SetActive(false);
		
		btnTalk.btn.onClick.AddListener(ClickTalk);
		btnTalkNext.btn.onClick.AddListener(ClickTalkNext);
	}

	public void ClickTalk()
	{
		if (GameMgr.ins.PLAYER.is_intro) return;

		HideBtnTalk();
		mainTalk.obj.SetActive(true);
		btnTalkNext.obj.SetActive(true);

		bool is_talk = false;

		switch (index_talk)
		{
			case -2:  //방 진입
				//Debug.Log("ShowNet");
				GameMgr.ins.mgrUI.status.popupMessage.Show(UIPopupMessage.MSG_TYPE.Network);
				break;
			case -4: //저장 포인트
				//Debug.Log("SAVE POINT");
				GameMgr.ins.mgrUI.status.popupMessage.Show(UIPopupMessage.MSG_TYPE.Save);
				break;
			case -5: //아이템 상자
				//Debug.Log("ITEM GET");
				//Debug.Log("ItemBoxType::" + BuildManager.GetBuildData(interBuild).index_npc);
				
				GameMgr.ins.PLAYER.mini.ani.PlayItemOpen();
				LeanTween.delayedCall(0.25f, () =>
				{	//상자 날아가며 보상 나타나는 연출
					if (interBuild != null && interBuild.ani != null) interBuild.ani.SetBool("is_open", true);
					//테이블에 의한 보상 생성
					GameMgr.ins.mgrEffect.ShowReward(BuildManager.GetBuildData(interBuild).index_npc, ref interBuild.tran);
					GameMgr.ins.mgrEffect.ShowEffect(Effect.EffHit003, ref interBuild.tran);
					PLAY_DATA.ins.status.AddGetItemBox(interBuild.TileIdx.ToString(), interBuild.pos.ToString());
				});
				

				break;
			default:
				is_talk = true;
				if (PLAY_DATA.ins.strTalk.dic.ContainsKey(index_talk))
				{
					mainTalk.txt.text = PLAY_DATA.ins.strTalk.dic[index_talk].comment;
				}
				else
				{
					ClickTalkNext();
				}
				break;
		}

		if (is_talk == false)
		{
			index_talk = -1;
			ClickTalkNext();
			return;
		}

		mainTalk.rect.localPosition = GameMgr.ins.mgrUIObj.Change3D2D(ref npcTalk.mini.trans, npcTalk.uihpY);

	}

	private void ClickTalkNext()
	{
		btnTalkNext.obj.SetActive(false);
		mainTalk.obj.SetActive(false);
	}

	public void UpdateBefore()
	{
		index_talk = -1;
		npcTalk = null;
	}
	
	public void ShowTalkBtn(int value, MiniNPC valueNpc, EventCollider evtcol = null)
	{
		index_talk = value;
		npcTalk = valueNpc;

		interBuild = null;
		if (evtcol == null) return;
		interBuild = evtcol.build;
		if (interBuild == null) return;
		//
		switch (interBuild.buildType)
		{
			case Build.BTYPE.Net: index_talk = -2; break;
			case Build.BTYPE.SavePoint: index_talk = -4; break;
			case Build.BTYPE.DestroyPoint: //gameover point 
				//Debug.Log("DestroyPoint");
				if (GameMgr.ins.PLAYER.mini.ability.HP <= 0) break;
				for (int i = 0; i < GameMgr.ins.PLAYER.effNut.Count; i++)
				{
					GameMgr.ins.PLAYER.effNut[i].SetIsNotReturn(false);
					GameMgr.ins.PLAYER.effNut[i].SetMovePlayer();
					GameMgr.ins.PLAYER.effNut[i].tran.SetParent(GameMgr.ins.mgrEffect.tran);
				}
				GameMgr.ins.PLAYER.effNut.Clear();
				GameMgr.ins.PLAYER.DestroyPoint.obj.SetActive(false);
				PLAY_DATA.ins.status.destorypoint = new int[10];
				break;
			case Build.BTYPE.ItemBox:
				if (interBuild.ani.GetBool("is_open"))
				{
					index_talk = -1;
					interBuild = null;
					break;
				}
				index_talk = -5;
				break;
		}
	}

	private void HideBtnTalk()
	{
		btnTalk.objMain.SetActive(false);
		GameMgr.ins.mgrUIObj.HideEvent();
	}

	public void UpdateTalk()
	{
		//Debug.Log("index_talk :: " + index_talk);

		if (index_talk != -1)
		{
			if (btnTalkNext.obj.activeSelf == true) return;
			if (btnTalk.objMain.activeSelf == false)
			{
				btnTalk.objMain.SetActive(true);
				switch(index_talk)
				{
					case -2:
						btnTalk.txt.text = STR_NET;
						GameMgr.ins.mgrUIObj.ShowEvent(STR_ACTIVE, GameMgr.ins.PLAYER.mini.trans, 1);
						break;
					case -4: //저장 포인트
					case -5: //아이템
						btnTalk.txt.text = STR_ACTIVE;
						GameMgr.ins.mgrUIObj.ShowEvent(STR_ACTIVE, GameMgr.ins.PLAYER.mini.trans, 1);
						break;
					default:
						btnTalk.txt.text = STR_TALK;
						GameMgr.ins.mgrUIObj.ShowEvent(STR_TALK, ref npcTalk);
						break;
				}
				/*
				if (index_talk == -2)
				{	//Debug.Log("Show Net");
					btnTalk.txt.text = STR_NET;
					GameMgr.ins.mgrUIObj.ShowEvent(STR_ACTIVE, GameMgr.ins.PLAYER.mini.trans, 1);
				}
				else
				{
					btnTalk.txt.text = STR_TALK;
					GameMgr.ins.mgrUIObj.ShowEvent(STR_TALK, ref npcTalk);
				}
				*/
			}
			return;
		}

		if (btnTalk.objMain.activeSelf == true) HideBtnTalk();
	}

}
