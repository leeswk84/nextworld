using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalBlockManager : MonoBehaviour
{
	private const string STR_BLOCK = "Block";

	public BlockManager prefabBlock;

	[HideInInspector]
	public BlockManager[] posBlocks;

	private Dictionary<int, BlockManager> dicBlocks;
	private int col_i;

	private BlockManager tmpBlock;
	private int block_idx;

	public void Init()
	{
		dicBlocks = new Dictionary<int, BlockManager>();

		posBlocks = new BlockManager[ GameMgr.TILE_MAX];
		for (col_i = 0; col_i < posBlocks.Length; col_i++)
		{
			CreateBlock(col_i);
		}
	}

	public void SetBlock(int value)
	{
		block_idx = SaveManager.aryIdxs[value];

		if (dicBlocks.ContainsKey(block_idx) == false)
		{
			CreateBlock(value);
		}

		posBlocks[value] = dicBlocks[block_idx];
		posBlocks[value].SetPosIdx(value);
	}

	private void CreateBlock(int value)
	{
		prefabBlock.block.obj.SetActive(true);
		tmpBlock = ((GameObject)(Instantiate(prefabBlock.gameObject))).GetComponent<BlockManager>();
		tmpBlock.transform.parent = prefabBlock.transform.parent;

		tmpBlock.block = ((GameObject)(Instantiate(prefabBlock.block.gameObject))).GetComponent<Block>();
		tmpBlock.block.transform.parent = prefabBlock.block.transform.parent;

		tmpBlock.block.name = STR_BLOCK + value;
		
		posBlocks[value] = tmpBlock;
		tmpBlock.Init(value);
		dicBlocks.Add(SaveManager.aryIdxs[value], tmpBlock);
		
		tmpBlock.mgrGround.col = tmpBlock.mgrGround.objFilter.AddComponent<MeshCollider>();
		
		//최초 생성 후 게임 오버 위치 생성
		if( PLAY_DATA.ins != null
			&& PLAY_DATA.ins.status != null
			&& PLAY_DATA.ins.status.destorypoint != null
			&& PLAY_DATA.ins.status.destorypoint[5] != 0 
			&& PLAY_DATA.ins.status.destorypoint[1] == GameMgr.ins.mgrSave.PLANET_IDX
			&& tmpBlock.block_idx == PLAY_DATA.ins.status.destorypoint[1])
		{
			GameMgr.ins.PLAYER.CreateDestroyPointData(tmpBlock.block);
		}

		prefabBlock.block.obj.SetActive(false);
	}
	/// <summary>
	/// block의 고유 key 값으로 지금 위치를 반환
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public BlockManager GetBlockIdx(int value)
	{
		if (dicBlocks.ContainsKey(value) == false) return null;
		return dicBlocks[value];
	}

	public void RefreshGroundColor()
	{
		for (col_i = 0; col_i < posBlocks.Length; col_i++)
		{
			posBlocks[col_i].mgrGround.SetBaseColor();
			posBlocks[col_i].mgrGround.SetGroundColor();
		}
		for (col_i = 0; col_i < posBlocks.Length; col_i++)
		{ posBlocks[col_i].mgrGround.SetSideGroundColor(); }
	}
}
