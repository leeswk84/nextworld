using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public Mini mini;

	public GameObject objCam_back;
	public Camera cam_back;
	public GameObject objCam_front;
	public Camera cam_front;
	
	public int layer_bullet;
	public int layer_user_bullet;
	public int layer_npc;

	public int set_layer_bullet;
	public int set_layer_user_bullet;

	private Vector3 col_start, col_end;

	private RaycastHit[] hit;
	private int num;
	private Bullet tmpBullet;
	public MiniNPC lookNPC;

	private float radius_bullet;
	private float radius_atk;
	private Transform tranAtkHand;
	
	private Vector3 vec;

	public bool is_intro;

	public Build DestroyPoint;
	public List<Effect> effNut;

	public GameObject objShadow;

	//private bool is_touch_atk;

	private List<string> atkNpc;

	private float targetDistance;
	private float minDistance;
	private MiniNPC tmpNpc;

	private BONE[] bons = new BONE[]
		{
			BONE.RCalf,
			BONE.LCalf,
			BONE.RThigh,
			BONE.LThigh,
			BONE.Spine,
			BONE.Spine1,
			BONE.LUpperArm,
			BONE.LForeArm,
			BONE.RUpperArm,
			BONE.RForeArm,
			BONE.RFoot,
			BONE.LFoot,
			BONE.RHand,
			BONE.LHand,
		};

	public void Init()
	{
		mini.Init();
		mini.ani.player = this;

		atkNpc = new List<string>();

		col_end =
		col_start = mini.cap.center;// + mini.trans.position;

		col_start.y -= (mini.cap.height * 0.5f);
		col_end.y += (mini.cap.height * 0.5f);

		radius_bullet = mini.cap.radius * 0.8f;
		//radius_atk = mini.cap.radius * 2f;

		set_layer_bullet = LayerMask.NameToLayer(TxtMgr.layer_bullet);
		set_layer_user_bullet = LayerMask.NameToLayer(TxtMgr.layer_user_bullet);

		layer_bullet = 1 << LayerMask.NameToLayer(TxtMgr.layer_bullet);
		layer_user_bullet = 1 << LayerMask.NameToLayer(TxtMgr.layer_user_bullet);
		layer_npc = 1 << LayerMask.NameToLayer(TxtMgr.layer_npc);

		mini.ability.HP = mini.ability.HPmax = 100;
		mini.ability.MP = mini.ability.MPmax = 100;

		mini.ability.power = 25;
		
		RefreshParts();

		if(DestroyPoint != null) DestroyPoint.obj.SetActive(false);
		effNut = new List<Effect>();

		//is_touch_atk = false;
	}

	public virtual void RefreshParts()
	{
		RefreshPart(ITEM_DATA.TYPE_2.BODY);
		RefreshPart(ITEM_DATA.TYPE_2.FOOT);
		RefreshPart(ITEM_DATA.TYPE_2.ARML);
		RefreshPart(ITEM_DATA.TYPE_2.ARMR);

		mini.RefreshMaxAbility();
	}

	public void UpdataPlayer()
	{
		if (mini.ability.HP < 1) return;
		CheckCollision();
	}

	//public void SetTouchAtack() { is_touch_atk = true; mini.ani.PlayAttack(); }
	public void SetTouchAtack()
	{
		if(mini.ani.PlayAttack() == true) atkNpc.Clear();
	}

	private void CheckCollision()
	{
		hit = Physics.CapsuleCastAll(col_start + mini.trans.position, col_end + mini.trans.position, radius_bullet, Vector3.up, 0f, layer_bullet);
		if (hit != null && hit.Length > 0)
		{
			for (num = 0; num < hit.Length; num++)
			{
				tmpBullet = hit[num].collider.transform.parent.gameObject.GetComponent<Bullet>();
				if (tmpBullet == null) continue;
				if (tmpBullet.is_player) continue;

				GameMgr.ins.mgrUI.status.EditHP(tmpBullet.power * -1);
				GameMgr.ins.mgrUIObj.ShowText(tmpBullet.power, ref tmpBullet.tran);
				tmpBullet.Hide();
				mini.ani.PlayDamage();
				//Debug.Log(hit[num].collider.gameObject.name);
			}
		}

		CheckCollAttackMode();

		//Debug.Log(mini.ani.ani[1][mini.ani.c_atk].time +"/"+ mini.ani.ani[1][mini.ani.c_atk].length);
		
		if (mini.ani.c_state == MiniAni.STATE.ATTACK
			//&& mini.ani.ani[1][mini.ani.c_atk].time > 0.5f)
			&& mini.ani.ani[1][mini.ani.c_atk].time > (mini.ani.motion_time / (mini.GetSkillValue(mini.ani.atk_arm, SKILL_DATA.VTYPE.motion) * mini.ani.add_speed) ) * mini.GetSkillValue(mini.ani.atk_arm, SKILL_DATA.VTYPE.check))
		{   //공격 데미지
			//hit = Physics.SphereCastAll(mini.trans.position, radius_atk, Vector3.up, 0f, layer_npc);
			switch (mini.ani.c_atk)
			{
				case MiniAni.ATK001: tranAtkHand = mini.tranBones[(int)BONE.RHand]; break;
				default: tranAtkHand = mini.tranBones[(int)BONE.LHand]; break;
			}
			//공격 반경
			//hit = Physics.SphereCastAll(tranAtkHand.position, 0.1f, Vector3.up, 0f, layer_npc);
			//hit = Physics.SphereCastAll(tranAtkHand.position, 0.5f, Vector3.up, 0f, layer_npc);
			hit = Physics.SphereCastAll(tranAtkHand.position, mini.GetSkillValue(mini.ani.atk_arm, SKILL_DATA.VTYPE.lengthatk), Vector3.up, 0f, layer_npc);

			lookNPC = null;
			if (hit != null && hit.Length > 0)
			{
				for (num = 0; num < hit.Length; num++)
				{
					lookNPC = hit[num].collider.gameObject.GetComponent<MiniNPC>();
					if (lookNPC.data.is_attack == false) continue;
					if (atkNpc.Contains(lookNPC.build.obj.name)) continue;
					atkNpc.Add(lookNPC.build.obj.name);
					//Debug.Log(lookNPC.build.obj.name);
					if (lookNPC.Damage(mini.ability.power) == true)
					{
						GameMgr.ins.mgrEffect.ShowEffect(Effect.EffHit002, ref tranAtkHand);
					}
				}
				//밀리는 모습
				/*
				mini.moveForce.x = (mini.trans.position.x - lookNPC.mini.trans.position.x);
				mini.moveForce.z = (mini.trans.position.z - lookNPC.mini.trans.position.z);
				mini.moveForce = Vector3.Normalize(mini.moveForce);
				mini.moveForce *= 0.15f;
				*/
				//mini.moveForce.z /= Mathf.Abs(mini.moveForce.x);
				//mini.moveForce.x /= Mathf.Abs(mini.moveForce.x);
				//Debug.Log(mini.moveForce);
			}
		}
	}
	
	private void CheckCollAttackMode()
	{
		lookNPC = null;
		//공격용 타겟 찾기
		//hit = Physics.SphereCastAll(mini.trans.position, radius_atk, Vector3.up, 0f, layer_npc);
		hit = Physics.SphereCastAll(mini.trans.position, mini.GetSkillValue(ITEM_DATA.TYPE_2.BODY, SKILL_DATA.VTYPE.lengthlook), Vector3.up, 0f, layer_npc);
		if (hit != null && hit.Length > 0)
		{
			minDistance = -1;
			for (num = 0; num < hit.Length; num++)
			{
				tmpNpc = hit[num].collider.gameObject.GetComponent<MiniNPC>();
				if (tmpNpc == null 
					|| tmpNpc.data.is_attack == false 
					|| tmpNpc.is_active == false 
					|| tmpNpc.cur_state == MiniNPC.STATE.HIDE) continue;
				
				targetDistance = Vector3.Distance(tmpNpc.mini.trans.position, mini.trans.position);
				if (targetDistance > mini.GetSkillValue(ITEM_DATA.TYPE_2.BODY, SKILL_DATA.VTYPE.lengthlook)) continue;

				if (minDistance == -1 
					|| minDistance > targetDistance)
				{
					lookNPC = tmpNpc;
					minDistance = targetDistance;
				}
				
				/*
				//일정 거리 접근하면 자동 공격
				if (lookNPC != null && lookNPC.data.is_attack && lookNPC.is_active)
				{	//공격
					mini.ani.PlayAttack();
					return;
				}
				*/
			}
		}
		//대화 npc
		//hit = Physics.SphereCastAll(mini.trans.position, radius_atk, Vector3.up, 0f, layer_npc);
		if (lookNPC != null)
		{
			if (lookNPC.cur_state != MiniNPC.STATE.HIDE
				&& lookNPC.cur_state != MiniNPC.STATE.NONE
				&& lookNPC.cur_state != MiniNPC.STATE.MAX)
			{
				if (lookNPC.uihp.objTarget.activeSelf == false) lookNPC.uihp.ShowTarget();
			}
			else if (lookNPC.uihp.objTarget.activeSelf == true)
			{
				lookNPC.uihp.Hide();
			}
			return;
		}
		hit = Physics.SphereCastAll(mini.trans.position, 0.5f, Vector3.up, 0f, layer_npc);
		if (hit != null && hit.Length > 0)
		{
			for (num = 0; num < hit.Length; num++)
			{
				lookNPC = hit[num].collider.gameObject.GetComponent<MiniNPC>();
				if (lookNPC == null || lookNPC.data.is_attack == true) lookNPC = null;
				if (lookNPC != null) break;
			}
		}

		/*
		if (is_touch_atk)
		{   //일반 터치에 의한 공격 모션
			//atkNpc.Clear();
			//is_touch_atk = false;
			//mini.ani.PlayAttack();
			if (mini.ani.GetNextTime() < Time.time)
			{
				atkNpc.Clear();
				is_touch_atk = false;
			}
			else
			{
				mini.ani.PlayAttack();
				return;
			}
			
		}
		*/
		/*
		if (mini.ani.c_state != MiniAni.STATE.DMG 
			&& mini.ani.c_state != MiniAni.STATE.ITEMOPEN)
		{
			mini.ani.PlayIdle();
		}
		*/

		/*
		if (Physics.CheckSphere(mini.trans.position, radius_atk, layer_npc) == true)
		{   //일정 거리 이상 가까이 적이 있다면 공격 모션
			mini.ani.PlayAttack();
		}
		else if (mini.ani.c_state != MiniAni.STATE.DMG)
		{
			mini.ani.PlayIdle();
		}
		*/
	}
	public virtual void RefreshPart(ITEM_DATA.TYPE_2 value)
	{
		GameMgr.ins.photon.SendMSG_Stat();
		//Debug.Log(PLAY_DATA.ins.status.equip[(int)value]);
		mini.EquipPart(	PLAY_DATA.ins.dataEquip.GetData(PLAY_DATA.ins.status.item[PLAY_DATA.ins.status.equip[(int)value]]),
						PLAY_DATA.ins.dataItem.dic[PLAY_DATA.ins.status.item[PLAY_DATA.ins.status.equip[(int)value]].idx].type2);
	}

	/*
	case ITEM_DATA.TYPE_2.BODY:
		bone = BONE.Spine1;
	case ITEM_DATA.TYPE_2.FOOT:
		case 0: bone = BONE.Spine; break;
		case 1: bone = BONE.RThigh; break;
		case 2: bone = BONE.RCalf; break;
		case 3: bone = BONE.LThigh; break;
		case 4: bone = BONE.LCalf; break;
	case ITEM_DATA.TYPE_2.ARML:
			case 0: bone = BONE.LUpperArm; break;
			case 1: bone = BONE.LForeArm; break;
	case ITEM_DATA.TYPE_2.ARMR:
			case 0: bone = BONE.RUpperArm; break;
			case 1: bone = BONE.RForeArm; break;
	*/

	public void CreateFirstDestroyPoint()
	{
		if(GameMgr.ins.mgrSave.PLANET_IDX != PLAY_DATA.ins.status.destorypoint[0]) return;

		for(int i = 0; i< GameMgr.ins.mgrBlock.posBlocks.Length; i++)
		{
			if(GameMgr.ins.mgrBlock.posBlocks[i].block_idx == PLAY_DATA.ins.status.destorypoint[1])
			{
				CreateDestroyPointData(GameMgr.ins.mgrBlock.posBlocks[i].block);
				break;
			}
		}
	}

	public void CreateDestroyPointData(Block block)
	{
		Vector3 vec = Vector3.zero;
		vec.x = PLAY_DATA.ins.status.destorypoint[2] * 0.01f;
		vec.y = PLAY_DATA.ins.status.destorypoint[3] * 0.01f;
		vec.z = PLAY_DATA.ins.status.destorypoint[4] * 0.01f;

		DestroyPoint.tran.parent = block.tran;
		DestroyPoint.tran.localPosition = vec;
		CreateDestoryPoint(block.tran, PLAY_DATA.ins.status.destorypoint[5]);
	}

	private void CreateDestoryPoint(Transform block, int nut)
	{
		if (block == null) return;
		if (nut <= 0) return;

		DestroyPoint.obj.SetActive(true);
		DestroyPoint.tran.parent = block;

		GameMgr.ins.mgrEffect.ShowNut(nut, ref DestroyPoint.tran, true);
		/*
		int i;
		if (nut > 10)
		{
			nut -= 10;
			for (i = 0; i < 10; i++)
			{ effNut.Add(GameMgr.ins.mgrEffect.ShowEffect(Effect.EffNut002, ref DestroyPoint.tran).SetNut(1, true)); }
		}

		for (i = 0; i < Mathf.FloorToInt(nut / 1000); i++)
		{ effNut.Add(GameMgr.ins.mgrEffect.ShowEffect(Effect.EffNut002, ref DestroyPoint.tran).SetNut(1000, true)); }
		nut = nut % 1000;
		for (i = 0; i < Mathf.FloorToInt(nut / 100); i++)
		{ effNut.Add(GameMgr.ins.mgrEffect.ShowEffect(Effect.EffNut002, ref DestroyPoint.tran).SetNut(100, true)); }
		nut = nut % 100;
		for (i = 0; i < Mathf.FloorToInt(nut / 10); i++)
		{ effNut.Add(GameMgr.ins.mgrEffect.ShowEffect(Effect.EffNut002, ref DestroyPoint.tran).SetNut(10, true)); }
		nut = nut % 10;
		for (i = 0; i < Mathf.FloorToInt(nut % 10); i++)
		{ effNut.Add(GameMgr.ins.mgrEffect.ShowEffect(Effect.EffNut002, ref DestroyPoint.tran).SetNut(1, true)); }
		*/
	}

	//파괴 연출
	public void DestroyIntro()
	{
		if (is_intro) return;
		
		is_intro = true;
		GameMgr.ins.mgrUI.imgLockPlane.color = GameMgr.ins.mgrUI.colPlaneHide;
		GameMgr.ins.mgrUI.objLockPlane.SetActive(true);
		//LeanTween.alpha(GameMgr.ins.mgrUI.rectLockPlane, 0f, 0f);

		Vector3 vec = Vector3.zero;
		
		Ray ray = new Ray();
		ray.direction = Vector3.down;
		RaycastHit rayHit = new RaycastHit();
		
		GameMgr.ins.mgrEffect.ShowEffect(Effect.EffShot002, ref mini.trans);
		GameMgr.ins.mgrEffect.ShowEffect(Effect.EffHit003, ref mini.trans);
		GameMgr.ins.mgrEffect.ShowEffect(Effect.EffDust001, ref mini.trans);

		int i;
		if (DestroyPoint.obj.activeSelf)
		{   //기존 내용 삭제
			for (i = 0; i < effNut.Count; i++)
			{
				Effect eff = effNut[i];
				eff.tran.parent = GameMgr.ins.mgrEffect.tran;
				LeanTween.rotateAroundLocal(eff.objBody, Vector3.left, 720f, 0.5f).setDelay(0.03f * i);
				LeanTween.rotateAroundLocal(eff.objBody, Vector3.up, 720f, 0.5f).setDelay(0.03f * i);
				LeanTween.moveLocalY(eff.obj, eff.tran.localPosition.y + 0.5f, 0.5f).setDelay(0.03f * i).setOnComplete(()=>
				{
					eff.SetIsNotReturn(false);
					eff.Return();	
				});
				
			}
			effNut.Clear();
			DestroyPoint.obj.SetActive(false);
		}
		PLAY_DATA.ins.status.destorypoint = new int[10];

		DestroyPoint.tran.position = mini.trans.position;
		vec = mini.trans.position;
		vec.y += 10f;
		ray.origin = vec;

		Physics.Raycast(ray, out rayHit, 100f, GameMgr.ins.mgrMoveCam.layer_ground);

		Block block = rayHit.collider.transform.parent.GetComponent<Block>();
		GroundSide side = rayHit.collider.GetComponent<GroundSide>();
		Transform tranD = null;
		
		if(side != null)
		{
			PLAY_DATA.ins.status.destorypoint[1] = side.block1;//side.pos_idx;
			for(i = 0; i< GameMgr.ins.mgrBlock.posBlocks.Length; i++)
			{
				if(GameMgr.ins.mgrBlock.posBlocks[i].block_idx == PLAY_DATA.ins.status.destorypoint[1])
				{
					tranD = GameMgr.ins.mgrBlock.posBlocks[i].block.tran;
					break;
				}
			}
		}
		if(block != null)
		{	
			PLAY_DATA.ins.status.destorypoint[1] = block.mgr.block_idx;
			tranD = block.tran;
		}

		if(tranD != null)
		{	
			PLAY_DATA.ins.status.destorypoint[0] = GameMgr.ins.mgrSave.PLANET_IDX;
			PLAY_DATA.ins.status.destorypoint[5] = (int)PLAY_DATA.ins.status.nut;
			/*
			Debug.Log(
				GameMgr.ins.mgrSave.PLANET_IDX + " / "
				//+ block.mgr.block_idx + " / "
				+ DestroyPoint.tran.localPosition
			);
			*/
			CreateDestoryPoint(tranD, (int)PLAY_DATA.ins.status.nut);
			PLAY_DATA.ins.status.destorypoint[2] = Mathf.RoundToInt(DestroyPoint.tran.localPosition.x * 100);
			PLAY_DATA.ins.status.destorypoint[3] = Mathf.RoundToInt(DestroyPoint.tran.localPosition.y * 100);
			PLAY_DATA.ins.status.destorypoint[4] = Mathf.RoundToInt(DestroyPoint.tran.localPosition.z * 100);
			for(i =0; i< PLAY_DATA.ins.status.item["1"].cnt; i++)
			{
				GameMgr.ins.mgrEffect.ShowEffect(Effect.EffItem001, ref DestroyPoint.tran, true, PLAY_DATA.ins.status.item["1"].idx);
			}
			PLAY_DATA.ins.status.item["1"].cnt = 0;
			GameMgr.ins.mgrUI.status.popupBag.RefreshTab();
			GameMgr.ins.mgrUI.status.hotKey.RefreshList();
			PLAY_DATA.ins.status.nut = 0;
			GameMgr.ins.mgrUI.status.EditNut(0);
		}

		objShadow.SetActive(false);

		int cntbone = 0;
		for (i = 0; i < bons.Length; i++)
		{
			if (mini.dicPart.ContainsKey(bons[i]) == false) continue;
			mini.dicPart[bons[i]].tran.SetParent(mini.trans.parent);
			cntbone++;
			vec.x = Random.Range(-1f, 1f) * 0.5f;
			vec.z = Random.Range(-1f, 1f) * 0.5f;
			vec.y = Random.Range(0.1f, 0.5f);

			Vector3 vecf = Vector3.zero;

			vecf.x = (vec.x * 2f) + mini.trans.position.x;
			vecf.z = (vec.z * 2f) + mini.trans.position.z;
			vecf.y = 10f;

			ray.origin = vecf;
			Physics.Raycast(ray, out rayHit, 100f, GameMgr.ins.mgrMoveCam.layer_ground);
			vecf.y = rayHit.point.y + 0.1f;

			vec += mini.trans.position;
			int idx = i;
			int cnt = cntbone;
			LeanTween.rotateAroundLocal(mini.dicPart[bons[i]].obj, Vector3.left, 360f, 0.5f);
			LeanTween.rotateAroundLocal(mini.dicPart[bons[i]].obj, Vector3.up, 360f, 0.5f);
			LeanTween.move(mini.dicPart[bons[i]].obj, vec, 0.25f + Random.Range(0, 0.1f)).setOnComplete(()=>
			{
				int idx2 = cnt;// cntbone;//idx;
				LeanTween.move(mini.dicPart[bons[idx]].obj, vecf, 0.3f).setOnComplete(() =>
				{
					if (idx2 == cntbone - 1) //bons.Length - 1)
					{
						is_intro = false;
						GameMgr.ins.mgrUI.objLockPlane.SetActive(false);
						GameMgr.ins.mgrUI.status.popupMessage.Show(UIPopupMessage.MSG_TYPE.Restart);
					}
				});
			});
		}
			
	}
	
	//등장 연출
	public void ShowIntro()
	{
		is_intro = true;
		mini.obj.SetActive(false);
		GameMgr.ins.mgrUI.imgLockPlane.color = GameMgr.ins.mgrUI.colPlaneHide;
		GameMgr.ins.mgrUI.objLockPlane.SetActive(true);
		//LeanTween.alpha(GameMgr.ins.mgrUI.rectLockPlane, 0f, 0f);
		
		//회복 아이템 2개로 고정
		PLAY_DATA.ins.status.item["1"].cnt = 2;
		GameMgr.ins.mgrUI.status.popupBag.RefreshTab();
		GameMgr.ins.mgrUI.status.hotKey.RefreshList();

		objShadow.SetActive(false);

		Vector3 vec = Vector3.zero;
		int bonecnt = 0;
		for (int i = 0; i < bons.Length; i++)
		{
			if (mini.dicPart.ContainsKey(bons[i]) == false) continue;
			mini.dicPart[bons[i]].tran.SetParent(GameMgr.ins.mgrMoveCam.CurrentSavePoint.tran_pos[i].parent);
			bonecnt++;
			vec.x = Random.Range(-50f, 50f);
			vec.y = Random.Range(0f, 50f);
			vec.z = Random.Range(0f, -50f);
			mini.dicPart[bons[i]].tran.position = vec;
			int idx = i;
			int cnt = bonecnt;
			LeanTween.rotateAroundLocal(mini.dicPart[bons[i]].obj, Vector3.left, 180f, 0.7f).setDelay(i * 0.1f).setEaseOutExpo();
			LeanTween.rotateAroundLocal(mini.dicPart[bons[i]].obj, Vector3.up, 180f, 0.7f).setDelay(i * 0.1f).setEaseOutExpo();
			LeanTween.rotateAroundLocal(mini.dicPart[bons[i]].obj, Vector3.forward, 180f, 0.7f).setDelay(i * 0.1f).setEaseOutExpo();

			LeanTween.moveLocal(mini.dicPart[bons[i]].obj, GameMgr.ins.mgrMoveCam.CurrentSavePoint.tran_pos[i].localPosition, 0.7f).setDelay(i * 0.1f).setOnComplete(()=> 
			{
				GameMgr.ins.mgrEffect.ShowEffect(Effect.EffHit003, ref mini.dicPart[bons[idx]].tran);

				//if (idx == bonecnt - 1)//bons.Length - 1)
				if (cnt == bonecnt - 1)//bons.Length - 1)
				{
					LeanTween.delayedCall(0.3f, () =>
					{
						float delay = 0.3f;

						mini.obj.SetActive(true);
						for (int j = 0; j < bons.Length; j++)
						{
							if (mini.dicPart.ContainsKey(bons[j]) == false) continue;

							mini.dicPart[bons[j]].tran.SetParent(mini.tranBones[(int)bons[j]]);

							LeanTween.move(mini.dicPart[bons[j]].obj, mini.trans.position, delay).setEaseInExpo();
							LeanTween.rotateAroundLocal(mini.dicPart[bons[j]].obj, Vector3.up, 180f, delay);
							LeanTween.rotateAroundLocal(mini.dicPart[bons[j]].obj, Vector3.left, 180f, delay);

							LeanTween.moveLocal(mini.dicPart[bons[j]].obj, Vector3.zero, 0.3f).setDelay(delay).setEaseOutExpo();//.setEaseInQuad();
							LeanTween.rotateLocal(mini.dicPart[bons[j]].obj, Vector3.zero, 0.3f).setDelay(delay);//.setEaseInQuad();
						}
						
						Invoke("ShowPlayer", 0.3f + delay);

						LeanTween.delayedCall(delay, () =>
						{
							objShadow.SetActive(true);
							GameMgr.ins.mgrEffect.ShowEffect(Effect.EffShot002, ref mini.trans);
							GameMgr.ins.mgrEffect.ShowEffect(Effect.EffHit003, ref mini.trans);
						});
					});
				}

				/*
				LeanTween.delayedCall(1f - (idx * 0.1f), () =>
				{
					mini.obj.SetActive(true);
					mini.dicPart[bons[idx]].tran.SetParent(mini.tranBones[(int)bons[idx]]);
					LeanTween.moveLocal(mini.dicPart[bons[idx]].obj, Vector3.zero, 0.38f);//.setEaseOutQuad();
					LeanTween.rotateLocal(mini.dicPart[bons[idx]].obj, Vector3.zero, 0.38f);//.setEaseOutQuad();

					if (idx == bons.Length - 1) Invoke("ShowPlayer", 0.38f);
				});
				*/
			});
		}
	}
	
	private void ShowPlayer()
	{
		mini.obj.SetActive(true);
		GameMgr.ins.mgrUI.objLockPlane.SetActive(false);
		is_intro = false;
	}

}
