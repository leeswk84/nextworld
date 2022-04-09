using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetPlayer : Player
{
	public string playerName;

	private int[] equip;
	private Vector3 vec3;
	private Vector3 vec_dir = Vector3.zero;

	private Quaternion rot_head_dir;
	private Quaternion rot_head_cur;
	public Transform tranShadow;

	private float distance_before;
	private float distance;

	private float speed;

	private int n;
	private int nn;
	private int[] w_check = new[] { 0, 1, 3, 4 };
	private int[] h_check = new[] { 0, 5, 15, 20 };

	private int w_pos;
	private int h_pos;

	//움직임 목표 설정
	public void SetMove(MSG_POS data)
	{
		vec3.x = data.x * 0.01f;
		vec3.y = data.y * 0.01f;
		vec3.z = data.z * 0.01f;
		/*
		 0  1  2  3  4
		 5  6  7  8  9
		10 11 12 13 14
		15 16 17 18 19
		20 21 22 23 24
		 */
		//타일에 의한 위치 수정
		if (GameMgr.ins.mgrSave.editIndex != data.t)
		{
			//Debug.Log(SaveManager.PLANET_WIDTH)
			//GameMgr.ins.mgrSave.editIndex
			w_pos = -1;
			for (n = 0; n < w_check.Length; n++)
			{
				for (nn = 0; nn < 5; nn++)
				{
					if (data.t == GameMgr.ins.mgrBlock.posBlocks[w_check[n] + (nn * 5)].block_idx)
					{
						w_pos = n;
						break;
					}
				}
				if (w_pos != -1) break;
			}
			switch (w_pos)
			{
				case 0: vec3.x -= (GameMgr.WIDTH + 1) * 2 ;  break; //좌 두칸
				case 1: vec3.x -= (GameMgr.WIDTH + 1); break; //좌 한칸
				case 2: vec3.x += (GameMgr.WIDTH + 1); break; //우 힌칸
				case 3: vec3.x += (GameMgr.WIDTH + 1) * 2; break; //우 두칸
			}
			h_pos = -1;
			for (n = 0; n < h_check.Length; n++)
			{
				for (nn = 0; nn < 5; nn++)
				{
					if (data.t == GameMgr.ins.mgrBlock.posBlocks[h_check[n] + nn].block_idx)
					{
						h_pos = n;
						break;
					}
				}
				if (h_pos != -1) break;
			}
			switch (h_pos)
			{
				case 0: vec3.z += (GameMgr.WIDTH + 1) * 2; break; //위 두칸
				case 1: vec3.z += (GameMgr.WIDTH + 1); break; //위 한칸
				case 2: vec3.z -= (GameMgr.WIDTH + 1); break; //밑 힌칸
				case 3: vec3.z -= (GameMgr.WIDTH + 1) * 2; break; //밑 두칸
			}
			/*
			if (data.t == GameMgr.ins.mgrBlock.posBlocks[1].block_idx
				|| data.t == GameMgr.ins.mgrBlock.posBlocks[6].block_idx
				|| data.t == GameMgr.ins.mgrBlock.posBlocks[11].block_idx
				|| data.t == GameMgr.ins.mgrBlock.posBlocks[16].block_idx
				|| data.t == GameMgr.ins.mgrBlock.posBlocks[21].block_idx)
			{}   //한칸 뒤
			else if (data.t == GameMgr.ins.mgrBlock.posBlocks[0].block_idx) {}	//두칸 뒤
			else if (data.t == GameMgr.ins.mgrBlock.posBlocks[0].block_idx) {}   //한칸 앞
			else if (data.t == GameMgr.ins.mgrBlock.posBlocks[0].block_idx) {}   //두칸 앞
			*/
		}
		
		if (vec_dir == Vector3.zero ||  Vector3.Distance(mini.trans.position, vec3) >= (GameMgr.WIDTH + 1))
		{   //처음 생성시 바로 해당 위치로 이동
			mini.trans.position = vec3;
			UpdatePosY();
		}

		vec_dir = vec3;

	}

	public void UpdateTilePos(Vector3 vec)
	{
		vec_dir += vec;
		mini.trans.position += vec;
		UpdatePosY();
	}

	public void UpdateNetPlayer()
	{
		if (mini.obj.activeSelf == false) return;
		UpdateMove();
	}

	private void UpdateMove()
	{
		vec3 = vec_dir;
		distance = Vector3.Distance(vec3, mini.trans.position);
		
		if (vec3 == mini.trans.position || distance < 0.3f || distance == distance_before)
		{
			mini.ani.UpdateAni(false);
			return;
		}
		vec3.y = mini.trans.position.y;
		LookAtBody(ref vec3);

		speed = Mathf.Clamp(distance * 1.5f, 1f, 7f); //거리가 멀수록 빨리 이동하도록
		mini.trans.Translate(Vector3.forward * speed * Time.smoothDeltaTime);

		UpdatePosY();

		mini.ani.UpdateAni(true);
		distance_before = distance;
	}

	private void UpdatePosY()
	{
		GameMgr.ins.mgrNpc.CheckGroundPoint(ref mini.trans);
		vec3 = mini.trans.position;
		vec3.y = GameMgr.ins.mgrNpc.hitGround.point.y;
		mini.trans.position = vec3;
		vec3.y += 0.2f;
		if (tranShadow != null) tranShadow.position = vec3;
	}

	private void LookAtBody(ref Vector3 vec)
	{
		mini.trans.LookAt(vec);
		rot_head_dir = mini.trans.rotation;
		rot_head_cur = mini.trans.rotation = Quaternion.Lerp(rot_head_cur, rot_head_dir, 5f * Time.smoothDeltaTime);
	}

	public void SetParts(string valueName, int[] valueEquip)
	{
		playerName = valueName;
		
		equip = valueEquip;

		RefreshPart(ITEM_DATA.TYPE_2.BODY);
		RefreshPart(ITEM_DATA.TYPE_2.FOOT);
		RefreshPart(ITEM_DATA.TYPE_2.ARML);
		RefreshPart(ITEM_DATA.TYPE_2.ARMR);

		mini.RefreshMaxAbility(false);

		if (mini.obj.activeSelf == false)
		{
			mini.obj.SetActive(true);
			vec_dir = Vector3.zero;
		}
	}

	public override void RefreshParts()
	{
		//base.RefreshParts();
	}

	public override void RefreshPart(ITEM_DATA.TYPE_2 value)
	{   //우선 레벨 1로 설정
		//base.RefreshPart(value);
		//Debug.Log("EQUIP_DATA.id :: " + equip[(int)value]);
		mini.EquipPart(PLAY_DATA.ins.dataEquip.GetData(PLAY_DATA.ins.dataItem.dic[equip[(int)value]].value, 1), value);
		mini.SetOutLine(false);
	}
}
