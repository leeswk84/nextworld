using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockView : MonoBehaviour
{
	private int num;
	//private float VIEW_DEGREE;
	private bool isCheck = false;
	private Vector3 vecPos;

	public void Init()
	{
		//VIEW_DEGREE = GameMgr.WIDTH * 2.3f;
		isCheck = false;
	}

	public void StartCheck()
	{
		isCheck = true;
		StartCoroutine(DoCheck());
	}

	private IEnumerator DoCheck()
	{
		while (isCheck)
		{
			UpdateBlockView();
			yield return new WaitForSeconds(0.1f);
		}
		yield break;
	}

	public void EndCheck()
	{
		isCheck = false;
		for (num = 0; num < GameMgr.ins.mgrBlock.posBlocks.Length; num++)
		{
			GameMgr.ins.mgrBlock.posBlocks[num].block.obj.SetActive(true);
		}
		for (num = 0; num < GameMgr.ins.mgrSide.SideEs.Length; num++)
		{
			GameMgr.ins.mgrSide.SideEs[num].obj.SetActive(true);
		}
		for (num = 0; num < GameMgr.ins.mgrSide.SideWs.Length; num++)
		{
			GameMgr.ins.mgrSide.SideWs[num].obj.SetActive(true);
		}
		for (num = 0; num < GameMgr.ins.mgrSide.SideHs.Length; num++)
		{
			GameMgr.ins.mgrSide.SideHs[num].obj.SetActive(true);
		}
	}
	public void UpdateBlockView()
	{
		/*
		vecPos =GameMgr.ins.mgrCam.rotXCam.position;
		vecPos.y = 0;

		for (num = 0; num < GameMgr.ins.mgrBlocks.Length; num++)
		{
			if (Vector3.Distance(GameMgr.ins.mgrBlocks[num].block.tran.position, vecPos) > VIEW_DEGREE)
			{
				if(GameMgr.ins.mgrBlocks[num].block.obj.activeSelf) GameMgr.ins.mgrBlocks[num].block.obj.SetActive(false);
			}
			else if(GameMgr.ins.mgrBlocks[num].block.obj.activeSelf == false)
			{
				GameMgr.ins.mgrBlocks[num].block.obj.SetActive(true);
			}
		}

		for (num = 0; num < GameMgr.ins.mgrSide.SideEdiges.Length; num++)
		{	
			if (Vector3.Distance(GameMgr.ins.mgrSide.SideEdiges[num].tran.position, vecPos) > VIEW_DEGREE)
			{
				if (GameMgr.ins.mgrSide.SideEdiges[num].obj.activeSelf) GameMgr.ins.mgrSide.SideEdiges[num].obj.SetActive(false);
			}
			else if (GameMgr.ins.mgrSide.SideEdiges[num].obj.activeSelf == false)
			{
				GameMgr.ins.mgrSide.SideEdiges[num].obj.SetActive(true);
			}
		}

		for (num = 0; num < GameMgr.ins.mgrSide.SideHs.Length; num++)
		{
			if (Vector3.Distance(GameMgr.ins.mgrSide.SideHs[num].tran.position, vecPos) > VIEW_DEGREE)
			{
				if (GameMgr.ins.mgrSide.SideHs[num].obj.activeSelf) GameMgr.ins.mgrSide.SideHs[num].obj.SetActive(false);
			}
			else if (GameMgr.ins.mgrSide.SideHs[num].obj.activeSelf == false)
			{
				GameMgr.ins.mgrSide.SideHs[num].obj.SetActive(true);
			}
		}

		for (num = 0; num < GameMgr.ins.mgrSide.SideWs.Length; num++)
		{
			if (Vector3.Distance(GameMgr.ins.mgrSide.SideWs[num].tran.position, vecPos) > VIEW_DEGREE)
			{
				if (GameMgr.ins.mgrSide.SideWs[num].obj.activeSelf) GameMgr.ins.mgrSide.SideWs[num].obj.SetActive(false);
			}
			else if (GameMgr.ins.mgrSide.SideWs[num].obj.activeSelf == false)
			{
				GameMgr.ins.mgrSide.SideWs[num].obj.SetActive(true);
			}
		}
		*/
	}
}
