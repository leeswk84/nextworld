using UnityEngine;
using System.Collections;

public class BlockManager : MonoBehaviour 
{
	/// <summary> block 화면에서의 위치 고유값 </summary>
	[HideInInspector]
	public int pos_idx;
	/// <summary> block 데이터 고유값 </summary>
	[HideInInspector]
	public int block_idx;

	public Block block;
	
	public GroundManager mgrGround;
	public RoadManager mgrRoad;
	public BuildManager mgrBuild;
	
	public void Init(int valueIdx) 
	{
		block_idx = -1;

		mgrGround.mgr = this;
		mgrRoad.mgr = this;
		mgrBuild.mgr = this;

		block.mgr = this;
		mgrGround.filter = block.filterGround;
		mgrRoad.pnlRoad = block.tranRoad;
		mgrBuild.pnlBuild = block.tranBuild;

		SetPosIdx(valueIdx);

		mgrGround.Init();
		mgrRoad.Init(mgrGround.vertices.Length - (GameMgr.WIDTH + 1));
		mgrBuild.Init(mgrGround.vertices.Length - (GameMgr.WIDTH + 1));
	}
	public void SetPosIdx(int valueIdx)
	{
		pos_idx = valueIdx;
		//name = "BlockManager" + pos_idx;

		//mgrGround.pos_idx = pos_idx;
		mgrGround.SetPosition(pos_idx);

		mgrRoad.pos_idx = pos_idx;
		mgrRoad.block_idx = SaveManager.aryIdxs[pos_idx];
		mgrBuild.pos_idx = pos_idx;
		mgrBuild.block_idx = SaveManager.aryIdxs[pos_idx];
		
		if (pos_idx == GameMgr.TILE_CENTER)
		{
			GameMgr.ins.mgrGround = mgrGround;
			GameMgr.ins.mgrRoad = mgrRoad;
			GameMgr.ins.mgrBuild = mgrBuild;
			GameMgr.ins.mgrCam.SetMgrGround();
		}
	}
}
