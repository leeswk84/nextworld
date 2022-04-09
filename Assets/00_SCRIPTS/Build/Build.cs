using UnityEngine;
using System.Collections;

public class Build : ObjectPrefab
{
	public enum BTYPE
	{
		None = 0,
		SavePoint,
		ItemBox,
		Net,
		DestroyPoint,
		Max
	}

	public Transform tranRot;
	public GameObject objRot;

	public Animator ani;
	public Renderer render;
	public MiniNPC npc;

	public BTYPE buildType;

	public ButtonObject btnObject;

	public Collider col;

	public bool IsShadow;
	public bool IsShadowLight;
	//public BoxCollider col;
	//public int mapUV;
	
	/// <summary> 건물 종류 고유값 </summary>
	private int idx;
	/// <summary> 건물 위치값 </summary>
	[HideInInspector]
	public int pos = -1;
	/// <summary> 고유값(list의 위치 값) </summary>
	[HideInInspector]
	public int listIdx;

	/// <summary> 건물 종류 반환  </summary>
	public int GetIdx() { return idx; }

	[HideInInspector]
	public int TileIdx = -1;

	private Vector2[] vecs2_base;

	public TrailRenderer[] trail;
	public Transform[] tran_pos;

	public void Init(int value, int valueTileIdx, int valueListIdx = -1) 
	{
		TileIdx = valueTileIdx;
		idx = value;
		listIdx = valueListIdx;
		tran.localRotation = Quaternion.identity;
		
		if (npc != null && valueListIdx != -1) npc.Init();
		
		//GameMgr.ins.mgrAtlas.SetTextureUV(ref filter, render.sharedMaterial.mainTexture.name, mapUV);
	}

	public bool CheckCurrentPoint()
	{
		if (GameMgr.ins.mgrSave.PLANET_IDX != PLAY_DATA.ins.status.savepoint[0]
			|| TileIdx != PLAY_DATA.ins.status.savepoint[1]
			|| pos != PLAY_DATA.ins.status.savepoint[2]) return false;

		if (GameMgr.ins.mgrMoveCam.CurrentSavePoint != this
			&& GameMgr.ins.mgrMoveCam.CurrentSavePoint != null)
		{
			GameMgr.ins.mgrMoveCam.CurrentSavePoint.ani.SetBool("is_active", false);
			GameMgr.ins.mgrMoveCam.CurrentSavePoint.ani.Play("SavePoint_notactive");
		}
		GameMgr.ins.mgrMoveCam.CurrentSavePoint = this;
		ani.SetBool("is_active", true);
		
		//Debug.Log("CURRENT SAVE POINT ACTIVE");

		if (trail != null)
		{
			for(int i =0; i < trail.Length; i++) trail[i].Clear();
		}
		return true;
	}

	public void OnEnable()
	{
		if (PLAY_DATA.ins == null || PLAY_DATA.ins.status == null || PLAY_DATA.ins.status.savepoint == null) return;

		switch (buildType)
		{
			case BTYPE.SavePoint:
				if (CheckCurrentPoint())
				{
					ani.Play("SavePoint_active");

					if (PLAY_DATA.ins.MOVE_DIRECT == true)
					{
						//Debug.Log("MOVE_DIRECT");
						Vector3 vec = tran.position;
						vec.z -= 1f;
						GameMgr.ins.PLAYER.mini.trans.position = vec;
						PLAY_DATA.ins.MOVE_DIRECT = false;
					}
				}
				else
				{
					ani.SetBool("is_active", false);
				}
				break;
			case BTYPE.ItemBox:

				if(PLAY_DATA.ins.CheckItemBox(TileIdx.ToString(), pos.ToString()))
				{ 
					ani.Play("ItemBox_opened");
					ani.SetBool("is_open", true);
				}
				else
				{
					ani.SetBool("is_open", false);
				}
				break;
		}
	}

	public void SetPosIdx(int value)
	{
		pos = value;
	}
	
	/// <summary> 생성 완료 </summary>
	public void CompleteCreate() 
	{
		btnObject.btn.onClick.RemoveAllListeners();
		btnObject.btn.onClick.AddListener(ClickBuild);
		btnObject.fncPress = GameMgr.ins.mgrCam.OnGroundPress;
	}

	public void ClickBuild() 
	{
		if (GameMgr.ins.mgrUI.PLAY_MODE == UIManager.MODE.MOVE) return; //이동 중에는 건물 수정 불가

		//Debug.Log("TileIdx:"+ TileIdx+",");
		//if (TileIdx != GameMgr.TILE_CENTER) return;
		if (TileIdx != SaveManager.aryIdxs[GameMgr.TILE_CENTER])
		{
			GameMgr.ins.mgrBlock.GetBlockIdx(TileIdx).mgrGround.ShowMoveBlock();
			return;
		}
		if (GameMgr.ins.mgrUI.build.isCreate)
		{
			if (GameMgr.ins.mgrUI.select.getType() == UISelect.TYPE.BUILD_MOVE 
				&& GameMgr.ins.mgrBuild.GetFocusBuild().listIdx != listIdx)
			{
				//Debug.Log("TRADE");
				GameMgr.ins.mgrUI.select.SetTradeBuild(listIdx);
			}
			return;
		}

		if (pos == GameMgr.ins.mgrGround.getCurIdx()
			&& pos == GameMgr.ins.mgrGround.getSelIdx())
		{
			GameMgr.ins.mgrGround.setCurIdx(-1);
			GameMgr.ins.mgrGround.setSelIdx(-1);
			GameMgr.ins.mgrUI.select.Hide();
			return;
		}

		GameMgr.ins.mgrGround.setCurIdx(pos);
		GameMgr.ins.mgrGround.setSelIdx(pos);
		//GameMgr.ins.mgrGround.pnlFocus.SetActive(true);
		GameMgr.ins.mgrField.pnlFocus.SetActive(true);
		GameMgr.ins.mgrUI.select.Show(UISelect.TYPE.BUILD_EDIT, listIdx);
		GameMgr.ins.mgrGround.UpdateEditor();
		GameMgr.ins.mgrGround.PlaneEdit(pos, false, true);
	}

	public void HideBuild()
	{
		
	}

}
