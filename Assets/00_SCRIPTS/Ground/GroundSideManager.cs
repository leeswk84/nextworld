using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GroundSideManager: MonoBehaviour 
{
	public GroundSide prefabSide;
	
	private int num, num2, num_mapp, num_col;
	private Vector3 vec3;
	private int vertexcount;

	private int side_top, side_bottom, side_left, side_right;
	private int side_lt, side_rt, side_lb, side_rb;

	private int uvcnt;
	private float fuv, fuv2;
	
	[HideInInspector]
	public int GROUND_VERTICS_COUNT;

	private GroundSide tmpSide;

	[HideInInspector]
	public bool isInitCreate;
	[HideInInspector]
	public bool is_edit = false;

	public void Init() 
	{
		isInitCreate = false;
		GROUND_VERTICS_COUNT = (GameMgr.WIDTH + 1) * (GameMgr.WIDTH + 1);
		vertexcount = (GameMgr.WIDTH + 1) * 2;

		InitW();
		InitH();
		InitE();
	}

	public void InitCreate()
	{
		isInitCreate = true;
		InitCreateE();
	}

	public void SetSideUV(bool vCheck = false)
	{
		if (isInitCreate == false) InitCreate();
	
		SetSideUV_W(vCheck);
		SetSideUV_H(vCheck);
		SetSideUV_E(vCheck);

		SetSideMoveMode();
	}

	public void SetGroundSideColor(bool vCheck = false)
	{
		for (num_col = 0; num_col < GameMgr.ins.mgrBlock.posBlocks.Length; num_col++)
		{
			GameMgr.ins.mgrBlock.posBlocks[num_col].mgrGround.SetSideGroundColor(vCheck);
		}
		SetAll();
	}

	/// <summary>
	/// UV 내용 적용 하기
	/// </summary>
	/// <param name="value_uv"> 적용할 UV </param>
	/// <param name="type">0:좌상단, 1:우상단, 2:좌하단, 3:우하단</param>
	public void SetUV(ref Vector2[] value_uv, int type = 0, int degree = -1) 
	{
		if (degree == -1) degree = GameMgr.WIDTH;

		//대각선의 내용만 지정 가능, 그 외는 마지막에 수정하는 식으로 수정
		//우상단 이미지
		if (type == 1)
		{
			fuv =  0.95f; //0.49f;
			fuv2 = 0.55f; //0.26f;
		}
		else
		{
			//좌하단 이미지
			fuv = 0.45f;//0.24f;
			fuv2 = 0.05f;//0.01f;
		}

		for (uvcnt = 0; uvcnt < value_uv.Length; uvcnt++)
		{
			if (Mathf.FloorToInt(uvcnt / (degree + 1)) % 2 == 0)
			{
				value_uv[uvcnt].x = ((uvcnt % (degree + 1)) % 2 == 0) ? fuv2 : fuv;
				value_uv[uvcnt].y = fuv2;
			}
			else
			{
				value_uv[uvcnt].x = ((uvcnt % (degree + 1)) % 2 == 0) ? fuv2 : fuv;
				value_uv[uvcnt].y = fuv;
			}

			if (type == 0) value_uv[uvcnt].y -= 0.5f;//0.25f
			if (type == 3) value_uv[uvcnt].x += 0.5f;//0.25f
		}
	}

	private void SetTringles(ref int[] value_traingles, ref Vector4[] value_tangents, int degree) 
	{
		for (num = 0; num < value_traingles.Length; num++)
		{
			num2 = Mathf.FloorToInt(num / 6) + (Mathf.FloorToInt(Mathf.FloorToInt(num / 6) / degree));
			if (num % 6 == 0) value_traingles[num] = num2;
			if (num % 6 == 1) value_traingles[num] = num2 + degree + 1;
			if (num % 6 == 2) value_traingles[num] = num2 + degree + 2;
			if (num % 6 == 3) value_traingles[num] = num2;
			if (num % 6 == 4) value_traingles[num] = num2 + degree + 2;
			if (num % 6 == 5) value_traingles[num] = num2 + 1;
		}

		for (num = 0; num < value_tangents.Length; num++)
		{
			value_tangents[num].x = value_tangents[num].w = -1;
			value_tangents[num].y = value_tangents[num].z = 0;
		}
	}

	public bool CheckSide(int TileIdx, int idx, float valuey, ref Color col, bool vCheck = false) 
	{
		if (isInitCreate == false) return false;

		side_bottom = side_top = side_left = side_right = -1;
		side_lt = side_rt = side_lb = side_rb = -1;
		
		side_top = TileIdx;
		side_bottom = TileIdx + GameMgr.TILE_WIDTH;
		if (side_bottom >= SideWs.Length) side_bottom = -1;

		//left
		//0 = 0, 1= 0, 2= 1, 3 = 2, 4 = 3, 5 = 4,
		//6 = 5, 7= 5, 8= 6, 9 = 7, 10 = 8, 11 = 9,
		//12 = 10, 13 = 10, 14 = 11
		//right
		//0 = 0, 1= 1, 2= 2, 3 = 3, 4 = 4, 5 = 4,
		//6 = 5, 7= 6, 8= 7, 9 = 8, 10 = 9, 11 = 9,

		side_left = TileIdx + Mathf.FloorToInt(TileIdx / (GameMgr.TILE_WIDTH));
		side_right = TileIdx + Mathf.FloorToInt(TileIdx / (GameMgr.TILE_WIDTH)) + 1;
		
		if (sideEdigeMapp.Length / 4 / 4 > TileIdx)
		{
			side_lt = sideEdigeMapp[TileIdx, 3, 0];
			side_rt = sideEdigeMapp[TileIdx, 2, 0];
			side_lb = sideEdigeMapp[TileIdx, 1, 0];
			side_rb = sideEdigeMapp[TileIdx, 0, 0];
		}
		
		if (SideWs[side_top].obj.activeSelf == false) side_top = -1;
		if (SideWs[side_bottom].obj.activeSelf == false) side_bottom = -1;
		if (SideHs[side_right].obj.activeSelf == false) side_right = -1;
		if (SideHs[side_left].obj.activeSelf == false) side_left = -1;
		
		if (Mathf.FloorToInt(idx / (GameMgr.WIDTH + 1)) == 0)
		{   //상단
			if (side_top != -1)
			{
				SideWs[side_top].SetVertices(0, idx, valuey, ref col, ref vCheck);
				if (Mathf.FloorToInt(side_top / GameMgr.TILE_WIDTH) == GameMgr.TILE_WIDTH - 1)
					SideWs[side_top + GameMgr.TILE_WIDTH].SetVertices(0, idx, 0, ref col, ref vCheck);
			}
			//	SideEdiges[side_lt].SetVertices(2, valuey, ref col);
			//좌상단
			if (side_lt != -1 && idx % (GameMgr.WIDTH + 1) == GameMgr.WIDTH)
			{
				if (SideEs[side_lt].obj.activeSelf) SideEs[side_lt].SetVertices(2, valuey, ref col, ref vCheck);
				for (num_mapp = 1; num_mapp < 4; num_mapp++)
				{
					side_lt = sideEdigeMapp[TileIdx, 3, num_mapp];
					if (side_lt == -1) break;
					if (SideEs[side_lt].obj.activeSelf) SideEs[side_lt].SetVertices(2, valuey, ref col, ref vCheck);
				}
			}
		}
		if (Mathf.FloorToInt(idx / (GameMgr.WIDTH + 1)) == GameMgr.WIDTH)
		{   //하단
			if (side_bottom != -1)
			{
				SideWs[side_bottom].SetVertices(1, idx, valuey, ref col, ref vCheck);
				if (Mathf.FloorToInt(side_bottom / GameMgr.TILE_WIDTH) == 1)
					SideWs[side_bottom - GameMgr.TILE_WIDTH].SetVertices(1, idx, 0, ref col, ref vCheck);
			}
			//	SideEdiges[side_rb].SetVertices(1, valuey, ref col);
			//우하단
			if (side_rb != -1 && idx % (GameMgr.WIDTH + 1) == 0)
			{
				if(SideEs[side_rb].obj.activeSelf) SideEs[side_rb].SetVertices(1, valuey, ref col, ref vCheck);
				for (num_mapp = 1; num_mapp < 4; num_mapp++)
				{
					side_rb = sideEdigeMapp[TileIdx, 0, num_mapp];
					if (side_rb == -1) break;
					if (SideEs[side_rb].obj.activeSelf) SideEs[side_rb].SetVertices(1, valuey, ref col, ref vCheck);
				}
			}
		}
		if (idx % (GameMgr.WIDTH + 1) == GameMgr.WIDTH)
		{   //좌측
			if (side_left != -1)
			{
				SideHs[side_left].SetVertices(2, idx, valuey, ref col, ref vCheck);
				if( (TileIdx % GameMgr.TILE_WIDTH) == GameMgr.TILE_WIDTH - 1) SideHs[side_left+1].SetVertices(2, idx, 0, ref col, ref vCheck);
			}
			//	SideEdiges[side_lb].SetVertices(0, valuey, ref col);
			//좌하단
			if (side_lb != -1 && Mathf.FloorToInt(idx / (GameMgr.WIDTH + 1)) == GameMgr.WIDTH)
			{
				if (SideEs[side_lb].obj.activeSelf) SideEs[side_lb].SetVertices(0, valuey, ref col, ref vCheck);
				for (num_mapp = 1; num_mapp < 4; num_mapp++)
				{
					side_lb = sideEdigeMapp[TileIdx, 1, num_mapp];
					if (side_lb == -1) break;
					if (SideEs[side_lb].obj.activeSelf) SideEs[side_lb].SetVertices(0, valuey, ref col, ref vCheck);
				}
			}
		}
		if (idx % (GameMgr.WIDTH + 1) == 0)
		{   //우측
			if (side_right != -1)
			{
				SideHs[side_right].SetVertices(3, idx, valuey, ref col, ref vCheck);
				if ((TileIdx % GameMgr.TILE_WIDTH) == 0) SideHs[side_right - 1].SetVertices(3, idx, 0, ref col, ref vCheck);
			}
			//우상단
			if (side_rt != -1 && Mathf.FloorToInt(idx / (GameMgr.WIDTH + 1)) == 0)
			{
				if (SideEs[side_rt].obj.activeSelf) SideEs[side_rt].SetVertices(3, valuey, ref col, ref vCheck);
				for (num_mapp = 1; num_mapp < 4; num_mapp++)
				{
					side_rt = sideEdigeMapp[TileIdx, 2, num_mapp];
					if (side_rt == -1) break;
					if (SideEs[side_rt].obj.activeSelf) SideEs[side_rt].SetVertices(3, valuey, ref col, ref vCheck);
				}
			}
		}
		
		
		return false;
	}

	public void FinishSide(bool vCheck = false)
	{
		if (isInitCreate == false) return;
		
		for (num = 0; num < SideHs.Length; num++)
		{
			if (SideHs[num].obj.activeSelf == false) continue;
			if (vCheck == true && SideHs[num].isSet == true) continue;
			//SideHs[num].isSet = true;
			SideHs[num].FinishVertices();
		}
		for (num = 0; num < SideWs.Length; num++)
		{
			if (SideWs[num].obj.activeSelf == false) continue;
			if (vCheck == true && SideWs[num].isSet == true) continue;
			//SideWs[num].isSet = true;
			SideWs[num].FinishVertices();
		}
		for (num = 0; num < SideEs.Length; num++)
		{	
			if (SideEs[num].obj.activeSelf == false) continue;
			if (vCheck == true && SideEs[num].isSet == true) continue;
			SideEs[num].FinishVertices();
		}
	}
	public void UpdateRoadAll()
	{
		for (num = 0; num < SideHs.Length; num++)
		{
			SideHs[num].road.SetRoadSide();
		}
		for (num = 0; num < SideWs.Length; num++)
		{
			SideWs[num].road.SetRoadSide();
		}
	}

	public void UpdateW(int value)
	{
		if (is_edit == false) return;
		SideWs[value].road.isSet = false;
		SideWs[value].road.SetRoadSide();
		SideWs[value].road.isSet = true;
	}
	public void UpdateH(int value)
	{
		if (is_edit == false) return;
		SideHs[value].road.isSet = false;
		SideHs[value].road.SetRoadSide();
		SideHs[value].road.isSet = true;
	}

	/// <summary>
	/// 플레이 모드에 따라 모서리 라인 안보이도록 설정.
	/// </summary>
	public void SetSideMoveMode()
	{
		if (SideEs.Length < 1) return;

		SetSideMoveModeW();
		SetSideMoveModeH();
		SetSideMoveModeE();
	}

	public void SetAll()
	{
		SetAllW();
		SetAllH();
		SetAllE();
	}
}
