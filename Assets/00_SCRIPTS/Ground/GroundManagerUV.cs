using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GroundManager : BaseBlockManager
{
	private float fuv, fuv2;
	private int uvcnt;

	private int uvtype;

	public void SetGroundUV()
	{
		return;
		//GameMgr.ins.mgrSide.SetUV(ref plane_uv, 3);
		//filter.mesh.uv2 = plane_uv;
		//filter.mesh.uv = plane_uv;
		/*
		GameMgr.ins.mgrSide.SetUV(ref plane_uv, GameMgr.ins.mgrSave.blockTypes[SaveManager.aryIdxs[TileIdx]]);
		filter.mesh.uv = plane_uv;
		filter.mesh.uv3 = plane_uv;
		filter.mesh.uv4 = plane_uv;

		SetUV(ref plane_uv, GameMgr.ins.mgrSave.blockTypes[SaveManager.aryIdxs[TileIdx]]);
		filter.mesh.uv2 = plane_uv;
		*/
		//filter.mesh.uv = plane_uv;
		//filter.mesh.uv3 = plane_uv;
		//filter.mesh.uv4 = plane_uv;
	}

	public void SetUV(ref Vector2[] value_uv, int type = 0, int degree = -1)
	{
		if (degree == -1) degree = GameMgr.WIDTH;

		//대각선의 내용만 지정 가능, 그 외는 마지막에 수정하는 식으로 수정
		//우상단 이미지
		
		for (uvcnt = 0; uvcnt < value_uv.Length; uvcnt++)
		{
			uvtype = type;

			if (uvcnt == 35)
			{
				uvtype = 3;
				if (uvtype == 1)
				{
					fuv = 0.45f;
					fuv2 = 0.05f;
				}
				else
				{
					fuv = 0.95f;
					fuv2 = 0.55f;
				}

				value_uv[uvcnt].x = ((uvcnt % (degree + 1)) % 2 == 0) ? fuv2 : fuv;

				if (Mathf.FloorToInt(uvcnt / (degree + 1)) % 2 == 0)
				{
					value_uv[uvcnt].y = fuv2;
				}
				else value_uv[uvcnt].y = fuv;
				
				if (uvtype == 3) value_uv[uvcnt].y -= 0.5f;
				if (uvtype == 0) value_uv[uvcnt].x += 0.5f;
				continue;
			}
		
			if (uvtype == 1)
			{
				fuv = 0.95f;
				fuv2 = 0.55f;
			}
			else
			{	//좌하단 이미지
				fuv = 0.45f;
				fuv2 = 0.05f;
			}

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

			if (uvtype == 0) value_uv[uvcnt].y -= 0.5f;
			if (uvtype == 3) value_uv[uvcnt].x += 0.5f;
		}
	}
}
