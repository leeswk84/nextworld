using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMapType : MonoBehaviour
{
	public Text txtMapTile;
	public InputField inputTiles, inputType;
	public ButtonObject btnEdit;

	private Vector3 backVec;
	private float backZoom;

	private int num;

	public void Init()
	{
		btnEdit.btn.onClick.AddListener(ClickEdit);

	}

	public void Show()
	{
		string str = "";

		for (num = 0; num < 25; num++)
		{
			str += string.Format("{0:00}", GameMgr.ins.mgrBlock.posBlocks[num].block_idx);
			str += ":"+string.Format("{0:00}", GameMgr.ins.mgrSave.blockTypes[GameMgr.ins.mgrBlock.posBlocks[num].block_idx]);
			if ((num + 1) % 5 != 0) str += ", ";
			else str += "\n";
			//GameMgr.ins.mgrBlock.GetBlockIdx(0).mgrGround.block_idx
		}

		txtMapTile.text = str;

		//inputTiles.text = "15";
		//inputType.text = "0";
	}

	private void ClickEdit()
	{
		//Debug.Log("ClickEdit");
		//Debug.Log(GameMgr.ins.mgrBlock.GetBlockIdx(19).pos_idx);
		
		string[] strs = inputTiles.text.Split(',');
		int mtype = int.Parse(inputType.text);
		int idx;
		int pos;
		for (num = 0; num < strs.Length; num++)
		{
			idx = int.Parse(strs[num]);
			pos = GameMgr.ins.mgrBlock.GetBlockIdx(idx).pos_idx;
			GameMgr.ins.mgrSave.blockTypes[idx] = mtype;
			GameMgr.ins.mgrBlock.posBlocks[pos].mgrGround.SetGround(idx);
		}
		GameMgr.ins.mgrSide.is_edit = false;
		GameMgr.ins.mgrSide.SetSideUV(true);
		GameMgr.ins.mgrSide.SetGroundSideColor(false);
		GameMgr.ins.mgrSide.is_edit = true;
		
		GameMgr.ins.mgrNetwork.SendMapEdittype(inputTiles.text, mtype);
	}

	public void Hide()
	{
		
	}
}
