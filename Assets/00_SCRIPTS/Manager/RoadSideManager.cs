using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSideManager : RoadManager
{
	private int n;
	private int h_idx;
	private int check_h;

	[HideInInspector]
	public bool isSet = false;

	public void Init()
	{

	}

	public void SetRoadSide(bool valueCheck = true)
	{
		if (valueCheck == true && isSet == true) return;

		switch (SIDE_TYPE)
		{
			case GroundSide.TYPE.GROUND_W:
				//Debug.Log("pos_idx:"+pos_idx+",block_idx:"+block_idx);
				if (side.pos_idx > GameMgr.TILE_WIDTH - 1 && side.pos_idx < GameMgr.TILE_WIDTH * GameMgr.TILE_WIDTH)
				{
					for (n = 0; n < GameMgr.WIDTH; n++)
					{
						//Debug.Log(GameMgr.ins.mgrBlock.GetBlockIdx(side.pos_idx - GameMgr.TILE_WIDTH).mgrRoad.roadsData[n + (GameMgr.TILE_WIDTH * (GameMgr.TILE_WIDTH))]);
						//Debug.Log(GameMgr.ins.mgrBlock.GetBlockIdx(side.pos_idx).mgrRoad.roadsData[n]);
						if (GameMgr.ins.mgrBlock.posBlocks[side.pos_idx - GameMgr.TILE_WIDTH].mgrRoad.roadsData[n - 1 + (GameMgr.WIDTH * (GameMgr.WIDTH))] != 0
							&& GameMgr.ins.mgrBlock.posBlocks[side.pos_idx].mgrRoad.roadsData[n] != 0)
						{
							SetRoad(n, true, 1);
							num_side_idx = n - 1 + (GameMgr.WIDTH * (GameMgr.WIDTH));
							num_side = side.pos_idx - GameMgr.TILE_WIDTH;
							if (GameMgr.ins.mgrBlock.posBlocks[num_side].mgrRoad.roads[num_side_idx] != null
								&& GameMgr.ins.mgrBlock.posBlocks[num_side].mgrRoad.roads[num_side_idx].strMat.Contains(RoadManager.ROAD_DIR4) == false)
							{   //위에 block에 하단 연결 길이 없을 경우.
								SetSideRoad(RoadManager.ROAD_DIR4);
							}
							num_side_idx = n;
							num_side = side.pos_idx;
							if (GameMgr.ins.mgrBlock.posBlocks[num_side].mgrRoad.roads[num_side_idx] != null
								&& GameMgr.ins.mgrBlock.posBlocks[num_side].mgrRoad.roads[num_side_idx].strMat.Contains(RoadManager.ROAD_DIR2) == false)
							{   //위에 block에 하단 연결 길이 없을 경우.
								SetSideRoad(RoadManager.ROAD_DIR2);
							}
						}
						else
						{   //제거
							//SetRoad(n, true, 0);
							HideRoad(n);
						}
					}
				}
				break;
			case GroundSide.TYPE.GROUND_H:
				if (side.pos_idx % (GameMgr.TILE_WIDTH + 1) != 0 && side.pos_idx % (GameMgr.TILE_WIDTH + 1) != GameMgr.TILE_WIDTH)
				{
					//Debug.Log(GameMgr.ins.mgrBlock.posBlocks[side.pos_idx - 1 - Mathf.FloorToInt(side.pos_idx/GameMgr.TILE_WIDTH)]);
					check_h = side.pos_idx - Mathf.FloorToInt(side.pos_idx / (GameMgr.TILE_WIDTH + 1));
					for (n = 0; n < GameMgr.WIDTH; n++)
					{
						if (GameMgr.ins.mgrBlock.posBlocks[check_h].mgrRoad.roadsData[((n + 1) * GameMgr.WIDTH) - 1 + n] != 0
							&& GameMgr.ins.mgrBlock.posBlocks[check_h - 1].mgrRoad.roadsData[n * (GameMgr.WIDTH + 1)] != 0)
						{   //생성
							SetRoad(n, true, 1);
							SideRoadUpdate(n);
							num_side = check_h;
							num_side_idx = ((n + 1) * GameMgr.WIDTH) - 1 + n;
							if (GameMgr.ins.mgrBlock.posBlocks[num_side].mgrRoad.roads[num_side_idx] != null
								&& GameMgr.ins.mgrBlock.posBlocks[num_side].mgrRoad.roads[num_side_idx].strMat.Contains(RoadManager.ROAD_DIR1) == false)
							{   //위에 block에 연결 길이 없을 경우.
								SetSideRoad(RoadManager.ROAD_DIR1);
							}
							num_side = check_h - 1;
							num_side_idx = n * (GameMgr.WIDTH + 1);
							if (GameMgr.ins.mgrBlock.posBlocks[num_side].mgrRoad.roads[num_side_idx] != null
								&& GameMgr.ins.mgrBlock.posBlocks[num_side].mgrRoad.roads[num_side_idx].strMat.Contains(RoadManager.ROAD_DIR3) == false)
							{   //위에 block에 연결 길이 없을 경우.
								SetSideRoad(RoadManager.ROAD_DIR3);
							}
						}
						else
						{   //제거
							//SetRoad(n, true, 0);
							HideRoad(n);
						}
						//if(side.pos_idx == 14) Debug.Log(roadsData[n]);
					}
				}
				break;
		}

		isSet = true;
	}

	public void RefreshMeshRoad()
	{
		for (num = 0; num < GameMgr.WIDTH; num++)
		{
			RefreshMesh(num);
		}
	}

	protected override void RefreshMesh(int idx)
	{
		if (checkRoadVisible(idx)) return;

		if (SIDE_TYPE == GroundSide.TYPE.GROUND_W)
		{
			roads[idx].vertices[0] = side.vertices[idx];
			roads[idx].vertices[1] = side.vertices[idx + 1];
			roads[idx].vertices[2] = side.vertices[idx + 1 + WIDTH];
			roads[idx].vertices[3] = side.vertices[idx + 2 + WIDTH];
		}

		if (SIDE_TYPE == GroundSide.TYPE.GROUND_H)
		{
			h_idx = idx * 2;
			roads[idx].vertices[0] = side.vertices[h_idx];
			roads[idx].vertices[1] = side.vertices[h_idx + 1];
			roads[idx].vertices[2] = side.vertices[h_idx + 1 + WIDTH];
			roads[idx].vertices[3] = side.vertices[h_idx + 2 + WIDTH];
		}

		roads[idx].filter.sharedMesh.vertices = roads[idx].vertices;
		roads[idx].filter.sharedMesh.RecalculateBounds();
		roads[idx].filter.sharedMesh.RecalculateNormals();
	}

	public override void UpdateMaterial(int idx)
	{
		if (checkRoadVisible(idx)) return;

		SetMatType(idx);

		str = "";
		
		if (SIDE_TYPE == GroundSide.TYPE.GROUND_W)
		{
			if (idx < roadsData.Length - 1 && roadsData[idx + 1] != 0)
			{   //첫번째 왼쪽 부분이 아니라면 확인
				if (string.IsNullOrEmpty(str) == false) str += BAR;
				str += ROAD_DIR1;
			}
			if (string.IsNullOrEmpty(str) == false) str += BAR;
			str += ROAD_DIR2;

			if (idx > 0 && roadsData[idx - 1] != 0)
			{   //첫번째 오른쪽 부분이 아니라면 확인
				if (string.IsNullOrEmpty(str) == false) str += BAR;
				str += ROAD_DIR3;
			}
			str += BAR + ROAD_DIR4;

			SetMaterial(idx);
			return;
		}

		if (SIDE_TYPE == GroundSide.TYPE.GROUND_H)
		{
			str += ROAD_DIR1;
			if (idx > 0 && roadsData[idx - 1] != 0)
			{   //첫번째 줄이 아니면 윗부분 확인 
				if (string.IsNullOrEmpty(str) == false) str += BAR;
				str += ROAD_DIR2;
			}
			if (string.IsNullOrEmpty(str) == false) str += BAR;
			str += ROAD_DIR3;

			if (idx < GameMgr.WIDTH - 1 && roadsData[idx + 1] != 0)
			{   //마지막 아랫쪽 아니라면 확인 
				if (string.IsNullOrEmpty(str) == false) str += BAR;
				str += ROAD_DIR4;
			}

			SetMaterial(idx);
			return;
		}
	}

	private void SideRoadUpdate(int value)
	{
		if (value > 0) UpdateMaterial(value - 1);
		if (value < GameMgr.WIDTH - 1) UpdateMaterial(value + 1);
	}

	protected override void HideRoad(int value)
	{
		if (roads[value] != null) roads[value].Hide();
		roadsData[value] = 0;

		SideRoadUpdate(value);
	}
}