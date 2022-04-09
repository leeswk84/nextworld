using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct CHECKH
{
	public int block1;
	public int block2;
}

public partial class GroundSideManager : MonoBehaviour
{
	private Dictionary<CHECKH, GroundSide> dicHs;
	[HideInInspector]
	public GroundSide[] SideHs;

	private Dictionary<int, bool> dicHsB;

	private int[] plane_traingles_h;
	private Vector4[] tangents_h;
	private Vector2[] plane_uv_h;

	private CHECKH checkh;

	private void InitH()
	{
		checkh = new CHECKH();

		dicHs = new Dictionary<CHECKH, GroundSide>();		
		SideHs = new GroundSide[30];

		dicHsB = new Dictionary<int, bool>();
		for (num = 0; num < 30; num++) dicHsB.Add(num, true);

		tangents_h = new Vector4[vertexcount];
		plane_traingles_h = new int[6 * GameMgr.WIDTH];
		SetTringles(ref plane_traingles_h, ref tangents_h, 1);

		plane_uv_h = new Vector2[vertexcount];
	}
	
	private void CreateH(int value)
	{
		prefabSide.obj.SetActive(true);
		tmpSide = GameObject.Instantiate(prefabSide.gameObject).GetComponent<GroundSide>();
		tmpSide.Init(GroundSide.TYPE.GROUND_H, vertexcount, value);

		for (num2 = 0; num2 < vertexcount; num2++)
		{
			tmpSide.vertices[num2].x = -(num2 % 2);
			tmpSide.vertices[num2].z = -Mathf.FloorToInt(num2 / 2);
			tmpSide.vertices[num2].y = 0;
		}
		tmpSide.SetVertices(ref plane_traingles_h, ref tangents_h);

		SetSidePos_H(ref tmpSide, value);
		SideHs[value] = tmpSide;
		prefabSide.obj.SetActive(false);
	}

	public void SetSidePos_H(ref GroundSide vSide, int vPos)
	{
		vSide.pos_idx = vPos;
		vec3 = vSide.tran.localPosition;
		vec3.x = ((vPos % (GameMgr.TILE_WIDTH + 1))
				* (GameMgr.WIDTH + 1))
				- ((GameMgr.WIDTH * GameMgr.TILE_WIDTH * 0.5f)) - 2;

		vec3.z = (Mathf.FloorToInt(vPos / (GameMgr.TILE_WIDTH + 1))
				* -(GameMgr.WIDTH + 1))
				+ ((GameMgr.WIDTH * GameMgr.TILE_WIDTH * 0.5f)) + ((GameMgr.TILE_WIDTH - 3));

		vSide.tran.localPosition = vec3;
	}


	private void SetCheckH(int value)
	{
		num2 = value - Mathf.FloorToInt(value / (GameMgr.TILE_WIDTH + 1)) - 1;
		if (value % (GameMgr.TILE_WIDTH + 1) == 0) num2++;
		checkh.block1 = SaveManager.aryIdxs[num2];

		num2 = value - Mathf.FloorToInt(value / (GameMgr.TILE_WIDTH + 1));
		if (value % (GameMgr.TILE_WIDTH + 1) == GameMgr.TILE_WIDTH) num2--;
		checkh.block2 = SaveManager.aryIdxs[num2];
	}

	private void SetSideUV_H(bool vCheck = false)
	{
		for (num = 0; num < SideHs.Length; num++) if (SideHs[num] != null) SideHs[num].filter.sharedMesh.colors = SideHs[num].planeOrgColors;

		for (num = 0; num < SideHs.Length; num++)
		{
			if (SideHs[num] != null) SideHs[num].obj.SetActive(false);
		}

		for (num = 0; num < SideHs.Length; num++)
		{
			if (dicHsB[num] == false)
			{
				SideHs[num].obj.SetActive(false);
				continue;
			}

			SetCheckH(num);

			if (vCheck == true)
			{
				if (dicHs.ContainsKey(checkh) == true)
				{
					SideHs[num] = dicHs[checkh];
					SideHs[num].obj.SetActive(true);
					SetSidePos_H(ref SideHs[num], num);
					continue;
				}
				else
				{
					CreateH(num);
					dicHs.Add(checkh, SideHs[num]);
					SideHs[num].block1 = checkh.block1;
					SideHs[num].block2 = checkh.block2;
				}
			}
			//Debug.Log("H:" + checkh.block1 + "." + checkh.block2);
			SetUV_H(ref SideHs[num]);
		}
	}

	private void SetUV_H(ref GroundSide vSide)
	{
		//0 = 0, 1= 0, 2= 1, 3 = 2, 4 = 3, 5 = 4,
		//6 = 5, 7= 5, 8= 6, 9 = 7, 10 = 8, 11 = 9,
		SetUV(ref plane_uv_h, GameMgr.ins.mgrSave.blockTypes[checkh.block1], 1);
		vSide.filter.sharedMesh.uv2 = plane_uv_h;
		vSide.filter.sharedMesh.uv4 = plane_uv_h;
		//0 = 0, 1= 1, 2= 2, 3 = 3, 4 = 4, 5 = 4,
		//6 = 5, 7= 6, 8= 7, 9 = 8, 10 = 9, 11 = 9,
		SetUV(ref plane_uv_h, GameMgr.ins.mgrSave.blockTypes[checkh.block2], 1);
		vSide.filter.sharedMesh.uv = plane_uv_h;
		vSide.filter.sharedMesh.uv3 = plane_uv_h;
	}

	private void SetSideMoveModeH()
	{
		for (num = 0; num < SideHs.Length; num++)
		{
			if (num % (GameMgr.TILE_WIDTH + 1) == 0
				|| num % (GameMgr.TILE_WIDTH + 1) == GameMgr.TILE_WIDTH)
			{
				dicHsB[num] = GameMgr.ins.mgrUI.PLAY_MODE != UIManager.MODE.MOVE;
				if (dicHsB[num] == true && SideHs[num].obj.activeSelf == false) SideHs[num].obj.SetActive(true);
			}
		}
	}

	private void SetAllH()
	{
		for (num = 0; num < SideHs.Length; num++) SideHs[num].isSet = true;
	}
}
