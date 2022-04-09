using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcManager : MonoBehaviour
{
	//tile 값안의 npc
	private Dictionary<int, List<MiniNPC>> dicNpc;
	private List<int> loopTile;
	private int loop_i;
	private int loop_t;

	[HideInInspector]
	public Ray rayGround;
	[HideInInspector]
	public RaycastHit hitGround;
	private int layer_ground;

	private Vector3 vec3;

	public void Init()
	{
		layer_ground = 1 << LayerMask.NameToLayer(TxtMgr.layer_ground);
		hitGround = new RaycastHit();
		rayGround = new Ray();
		rayGround.direction = Vector3.down;
		
		dicNpc = new Dictionary<int, List<MiniNPC>>();
		loopTile = new List<int>();
	}

	public void AddNpc(ref MiniNPC value, int tile)
	{
		//Debug.Log(tile);
		if (dicNpc.ContainsKey(tile) == false) dicNpc.Add(tile, new List<MiniNPC>());
		dicNpc[tile].Add(value);
		if (loopTile.Contains(tile) == false) loopTile.Add(tile);
	}
	/// <summary>
	/// npc 다시 부활, 처치한 적 초기화
	/// </summary>
	public void Rebirth()
	{
		for (loop_t = 0; loop_t < loopTile.Count; loop_t++)
		{
			for (loop_i = 0; loop_i < dicNpc[loopTile[loop_t]].Count; loop_i++)
			{
				//if (dicNpc[loopTile[loop_t]][loop_i].mini.obj.activeSelf == false
				//	&& dicNpc[loopTile[loop_t]][loop_i].build.obj.activeSelf == true)
				if( dicNpc[loopTile[loop_t]][loop_i].data.is_attack == true)
				{	//공격 하는 NPC는 모두 부활
					dicNpc[loopTile[loop_t]][loop_i].Rebirth();
				}
			}
		}
	}

	public void UpdateNpc()
	{
		for (loop_t = 0; loop_t < loopTile.Count; loop_t++)
		{
			for (loop_i = 0; loop_i < dicNpc[loopTile[loop_t]].Count; loop_i++)
			{
				dicNpc[loopTile[loop_t]][loop_i].UpdateNPC();
			}
		}
	}

	public void CheckGroundPoint(ref Transform value)
	{
		vec3 = value.position;
		CheckGroundPoint(ref vec3);
	}

	public void CheckGroundPoint(ref Vector3 vec)
	{
		vec.y += 50;
		rayGround.origin = vec;
		Physics.Raycast(rayGround, out hitGround, 200f, layer_ground);
		vec.y -= 50;
	}
}
