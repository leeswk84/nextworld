using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct CHECKW
{
	public int block1;
	public int block2;
}

public partial class GroundSideManager : MonoBehaviour
{
	private Dictionary<CHECKW, GroundSide> dicWs;
	[HideInInspector]
	public GroundSide[] SideWs;

	private Dictionary<int, bool> dicWsB;

	private int[] plane_traingles_w;
	private Vector4[] tangents_w;
	private Vector2[] plane_uv_w;

	private CHECKW checkw;

	private void InitW()
	{
		checkw = new CHECKW();

		dicWs = new Dictionary<CHECKW, GroundSide>();
		SideWs = new GroundSide[30];

		dicWsB = new Dictionary<int, bool>();
		for (num = 0; num < 30; num++) dicWsB.Add(num, true);
		
		tangents_w = new Vector4[vertexcount];
		plane_traingles_w = new int[6 * GameMgr.WIDTH];
		SetTringles(ref plane_traingles_w, ref tangents_w, GameMgr.WIDTH);

		plane_uv_w = new Vector2[vertexcount];
	}
	
	private void CreateW(int value)
	{
		prefabSide.obj.SetActive(true);
		tmpSide = GameObject.Instantiate(prefabSide.gameObject).GetComponent<GroundSide>();
		tmpSide.Init(GroundSide.TYPE.GROUND_W, vertexcount, value);

		for (num2 = 0; num2 < vertexcount; num2++)
		{
			tmpSide.vertices[num2].x = (GameMgr.WIDTH * 0.5f) - (num2 % (GameMgr.WIDTH + 1));
			tmpSide.vertices[num2].z = -Mathf.FloorToInt(num2 / (GameMgr.WIDTH + 1));
			tmpSide.vertices[num2].y = 0;
		}
		tmpSide.SetVertices(ref plane_traingles_w, ref tangents_w);

		SetSidePos_W(ref tmpSide, value);

		SideWs[value] = tmpSide;

		prefabSide.obj.SetActive(false);
	}

	public void SetSidePos_W(ref GroundSide vSide, int vPos)
	{
		vSide.pos_idx = vPos;

		vec3 = vSide.tran.localPosition;

		vec3.x = ((vPos % (GameMgr.TILE_WIDTH))
				* (GameMgr.WIDTH + 1))
				- ((GameMgr.WIDTH * GameMgr.TILE_WIDTH * 0.5f)) + ((GameMgr.WIDTH - 4) * 0.5f);
		vec3.z = (Mathf.FloorToInt(vPos / (GameMgr.TILE_WIDTH))
				* -(GameMgr.WIDTH + 1))
				+ ((GameMgr.WIDTH * GameMgr.TILE_WIDTH * 0.5f)) + ((GameMgr.TILE_WIDTH - 2));

		vSide.tran.localPosition = vec3;
	}
	private void SetCheckW(int value)
	{
		num2 = value - GameMgr.TILE_WIDTH;
		if (num2 < 0)
		{
			num2 += GameMgr.TILE_WIDTH;
			num2 = SaveManager.aryIdxs[num2];
			//num2 -= SaveManager.PLANET_WIDTH;
			//if (num2 < 0) num2 = SaveManager.PLANET_MAX + num2;
			checkw.block1 = num2;
		}
		else checkw.block1 = SaveManager.aryIdxs[num2];

		num2 = value;
		if (num2 >= SaveManager.aryIdxs.Length)
		{
			num2 -= GameMgr.TILE_WIDTH;
			num2 = SaveManager.aryIdxs[num2];
			//num2 += SaveManager.PLANET_WIDTH;
			//if (num2 > SaveManager.PLANET_MAX) num2 = num2 - SaveManager.PLANET_MAX;
			checkw.block2 = num2;
		}
		else checkw.block2 = SaveManager.aryIdxs[num2];
	}

	private void SetSideUV_W(bool vCheck = false)
	{
		for (num = 0; num < SideWs.Length; num++) if (SideWs[num] != null) SideWs[num].filter.sharedMesh.colors = SideWs[num].planeOrgColors;

		for (num = 0; num < SideWs.Length; num++)
		{
			if(SideWs[num] != null) SideWs[num].obj.SetActive(false);
		}

		for (num = 0; num < SideWs.Length; num++)
		{
			if (dicWsB[num] == false)
			{
				SideWs[num].obj.SetActive(false);
				continue;
			}

			SetCheckW(num);
			
			if (vCheck == true)
			{
				if (dicWs.ContainsKey(checkw) == true)
				{
					SideWs[num] = dicWs[checkw];
					SideWs[num].obj.SetActive(true);
					SetSidePos_W(ref SideWs[num], num);
					continue;
				}
				else
				{
					CreateW(num);
					dicWs.Add(checkw, SideWs[num]);
					SideWs[num].block1 = checkw.block1;
					SideWs[num].block2 = checkw.block2;
				}
			}
			//Debug.Log("W:" + checkw.block1 + "." + checkw.block2);
			SetUV_W(ref SideWs[num]);
		}
	}

	private void SetUV_W(ref GroundSide vSide)
	{
		SetUV(ref plane_uv_w, GameMgr.ins.mgrSave.blockTypes[checkw.block1]);
		vSide.filter.sharedMesh.uv3 = plane_uv_w;
		vSide.filter.sharedMesh.uv4 = plane_uv_w;

		SetUV(ref plane_uv_w, GameMgr.ins.mgrSave.blockTypes[checkw.block2]);
		vSide.filter.sharedMesh.uv = plane_uv_w;
		vSide.filter.sharedMesh.uv2 = plane_uv_w;

	}

	private void SetSideMoveModeW()
	{
		for (num = 0; num < SideWs.Length; num++)
		{
			if (num < GameMgr.TILE_WIDTH
				|| num >= GameMgr.TILE_WIDTH * GameMgr.TILE_WIDTH)
			{
				dicWsB[num] = GameMgr.ins.mgrUI.PLAY_MODE != UIManager.MODE.MOVE;
				if (dicWsB[num] == true && SideWs[num].obj.activeSelf == false) SideWs[num].obj.SetActive(true);
			}
		}
	}

	private void SetAllW()
	{
		for (num = 0; num < SideWs.Length; num++) SideWs[num].isSet = true;
	}
}
