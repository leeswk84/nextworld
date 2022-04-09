using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct CHECKE
{
	public int block1;
	public int block2;
	public int block3;
	public int block4;
}

public partial class GroundSideManager : MonoBehaviour
{
	private CHECKE checke;
	private Dictionary<CHECKE, GroundSide> dicEs;
	[HideInInspector]
	public GroundSide[] SideEs;

	private Dictionary<int, bool> dicEsB;

	private int[,] sideEdigeIdx;
	private int[,,] sideEdigeMapp;

	private int[] plane_traingles_e;
	private Vector4[] tangents_e;
	private Vector2[] plane_uv_e;

	private void InitE()
	{
		checke = new CHECKE();
		dicEs = new Dictionary<CHECKE, GroundSide>();
		SideEs = new GroundSide[36];

		dicEsB = new Dictionary<int, bool>();
		for (num = 0; num < 36; num++) dicEsB.Add(num, true);

		sideEdigeIdx = new int[,] {
			{ 0,0,0,0 }, //0
			{ 0,1,0,1 }, //1
			{ 1,2,1,2 }, //2
			{ 2,3,2,3 }, //3
			{ 3,4,3,4 }, //4
			{ 4,4,4,4 }, //5

			{ 0,0,5,5 }, //6
			{ 0,1,5,6 }, //7
			{ 1,2,6,7 }, //8
			{ 2,3,7,8 }, //9
			{ 3,4,8,9 }, //10
			{ 4,4,9,9 }, //11

			{ 5,5,10,10 }, //12
			{ 5,6,10,11 }, //13
			{ 6,7,11,12 }, //14
			{ 7,8,12,13 }, //15
			{ 8,9,13,14 }, //16
			{ 9,9,14,14 }, //17

			{ 10,10,15,15 }, //18
			{ 10,11,15,16 }, //19
			{ 11,12,16,17 }, //20
			{ 12,13,17,18 }, //21
			{ 13,14,18,19 }, //22
			{ 14,14,19,19 }, //23

			{ 15,15,20,20 }, //24
			{ 15,16,20,21 }, //25
			{ 16,17,21,22 }, //26
			{ 17,18,22,23 }, //27
			{ 18,19,23,24 }, //28
			{ 19,19,24,24 }, //29

			{ 20,20,20,20 }, //30
			{ 20,21,20,21 }, //31
			{ 21,22,21,22 }, //32
			{ 22,23,22,23 }, //33
			{ 23,24,23,24 }, //34
			{ 24,24,24,24 }, //35
		};

		int check_idx;
		sideEdigeMapp = new int[GameMgr.TILE_MAX, 4, 4];
		for (num = 0; num < GameMgr.TILE_MAX; num++)
		{
			for (check_idx = 0; check_idx < 4; check_idx++)
			{
				sideEdigeMapp[num, check_idx, 0] = -1;
				sideEdigeMapp[num, check_idx, 1] = -1;
				sideEdigeMapp[num, check_idx, 2] = -1;
				sideEdigeMapp[num, check_idx, 3] = -1;
				num_mapp = 0;
				for (num2 = 0; num2 < (sideEdigeIdx.Length / 4); num2++)
				{
					if (sideEdigeIdx[num2, check_idx] == num)
					{
						sideEdigeMapp[num, check_idx, num_mapp] = num2;
						num_mapp++;
					}
				}
			}
		}

		/*
		sideOutEdigeIdx = new int[,] {	{ 0,0,0,0},
										{ 0,1,0,1},
										{ 1,2,1,2},
										{ 2,2,2,2},
										{ 0,0,3,3},
										{ 2,2,5,5},
										{ 3,3,6,6},
										{ 5,5,8,8},
										{ 6,6,6,6},
										{ 6,7,6,7},
										{ 7,8,7,8},
										{ 8,8,8,8},
		};
		*/

		plane_uv_e = new Vector2[4];
		tangents_e = new Vector4[4];
		plane_traingles_e = new int[6];

		SetTringles(ref plane_traingles_e, ref tangents_e, 1);
		SetUV(ref plane_uv_e, 2, 1);
	}

	private void InitCreateE()
	{
		for (num = 0; num < SideEs.Length; num++)
		{   
			
		}
	}

	private void CreateE(int value)
	{
		prefabSide.obj.SetActive(true);

		tmpSide = GameObject.Instantiate(prefabSide.gameObject).GetComponent<GroundSide>();
		tmpSide.Init(GroundSide.TYPE.GROUND_EDIGE, 4, value);

		for (num2 = 0; num2 < tmpSide.vertices.Length; num2++)
		{
			tmpSide.vertices[num2].x = 0.5f - (num2 % 2);
			tmpSide.vertices[num2].z = 0.5f - Mathf.FloorToInt(num2 / 2);
			tmpSide.vertices[num2].y = 0;
		}

		tmpSide.SetVertices(ref plane_traingles_e, ref tangents_e);
		tmpSide.filter.sharedMesh.uv = plane_uv_e;

		SetSidePos_E(ref tmpSide, num);

		SideEs[value] = tmpSide;

		prefabSide.obj.SetActive(false);
	}

	private void SetSidePos_E(ref GroundSide vSide, int vPos)
	{
		vSide.pos_idx = vPos;
		vec3 = vSide.tran.localPosition;

		vec3.x = ((vPos % (GameMgr.TILE_WIDTH + 1)) * (GameMgr.WIDTH + 1)) - ((GameMgr.WIDTH + 1) * (GameMgr.TILE_WIDTH * 0.5f));
		vec3.z = (Mathf.FloorToInt((vPos / -(GameMgr.TILE_WIDTH + 1))) * (GameMgr.WIDTH + 1)) + ((GameMgr.WIDTH + 1) * (GameMgr.TILE_WIDTH * 0.5f));

		vSide.tran.localPosition = vec3;
	}

	private void SetCheckE(int value)
	{
		checke.block1 = SaveManager.aryIdxs[sideEdigeIdx[value, 0]];
		checke.block2 = SaveManager.aryIdxs[sideEdigeIdx[value, 1]];
		checke.block3 = SaveManager.aryIdxs[sideEdigeIdx[value, 2]];
		checke.block4 = SaveManager.aryIdxs[sideEdigeIdx[value, 3]];
	}

	private void SetSideUV_E(bool vCheck = false)
	{
		for (num = 0; num < SideEs.Length; num++) if (SideEs[num] != null) SideEs[num].filter.sharedMesh.colors = SideEs[num].planeOrgColors;

		for (num = 0; num < SideEs.Length; num++)
		{
			if (SideEs[num] != null) SideEs[num].obj.SetActive(false);
		}
		for (num = 0; num < SideEs.Length; num++)
		{
			//if (SideEs[num].obj.activeSelf == false) continue;
			if (dicEsB[num] == false)
			{
				SideEs[num].obj.SetActive(false);
				continue;
			}

			SetCheckE(num);

			if (vCheck == true)
			{
				if (dicEs.ContainsKey(checke) == true)
				{
					SideEs[num] = dicEs[checke];
					SideEs[num].obj.SetActive(true);
					SetSidePos_E(ref SideEs[num], num);
					continue;
				}
				else
				{
					CreateE(num);
					dicEs.Add(checke, SideEs[num]);
					SideEs[num].block1 = checke.block1;
					SideEs[num].block2 = checke.block2;
					SideEs[num].block3 = checke.block3;
					SideEs[num].block4 = checke.block4;
				}
			}
			//Debug.Log("E:" + checke.block1 + "." + checke.block2);
			SetUV_E(ref SideEs[num]);
		}
	}
	private void SetUV_E(ref GroundSide vSide)
	{
		/*
		 모서리
		 0 = 0,1 3,4
		 1 = 1,2 4,5
		 2 = 3,4 6,7
		 3 = 4,5 7,8
		 */
		/*
	   SetUV(ref plane_uv_e, GameMgr.ins.mgrSave.blockTypes[SaveManager.aryIdxs[sideEdigeIdx[num, 0]]], 1);
	   SideEs[num].filter.mesh.uv4 = plane_uv_e;
	   SetUV(ref plane_uv_e, GameMgr.ins.mgrSave.blockTypes[SaveManager.aryIdxs[sideEdigeIdx[num, 1]]], 1);
	   SideEs[num].filter.mesh.uv3 = plane_uv_e;
	   SetUV(ref plane_uv_e, GameMgr.ins.mgrSave.blockTypes[SaveManager.aryIdxs[sideEdigeIdx[num, 2]]], 1);
	   SideEs[num].filter.mesh.uv2 = plane_uv_e;
	   SetUV(ref plane_uv_e, GameMgr.ins.mgrSave.blockTypes[SaveManager.aryIdxs[sideEdigeIdx[num, 3]]], 1);
	   SideEs[num].filter.mesh.uv = plane_uv_e;
	   */
		SetUV(ref plane_uv_e, GameMgr.ins.mgrSave.blockTypes[checke.block1], 1);
		SideEs[num].filter.sharedMesh.uv4 = plane_uv_e;
		SetUV(ref plane_uv_e, GameMgr.ins.mgrSave.blockTypes[checke.block2], 1);
		SideEs[num].filter.sharedMesh.uv3 = plane_uv_e;
		SetUV(ref plane_uv_e, GameMgr.ins.mgrSave.blockTypes[checke.block3], 1);
		SideEs[num].filter.sharedMesh.uv2 = plane_uv_e;
		SetUV(ref plane_uv_e, GameMgr.ins.mgrSave.blockTypes[checke.block4], 1);
		SideEs[num].filter.sharedMesh.uv = plane_uv_e;
	}


	private void SetSideMoveModeE()
	{
		for (num = 0; num < SideEs.Length; num++)
		{
			if (num % (GameMgr.TILE_WIDTH + 1) == 0
				|| num % (GameMgr.TILE_WIDTH + 1) == GameMgr.TILE_WIDTH
				|| num < GameMgr.TILE_WIDTH + 1
				|| num >= (GameMgr.TILE_WIDTH + 1) * (GameMgr.TILE_WIDTH))
			{
				dicEsB[num] = GameMgr.ins.mgrUI.PLAY_MODE != UIManager.MODE.MOVE;
				if (dicEsB[num] == true && SideEs[num].obj.activeSelf == false) SideEs[num].obj.SetActive(true);
				//SideEs[num].obj.SetActive(GameMgr.ins.mgrUI.PLAY_MODE != UIManager.MODE.MOVE);
			}
		}
	}

	private void SetAllE()
	{
		for (num = 0; num < SideEs.Length; num++) SideEs[num].isSet = true;
	}
}
