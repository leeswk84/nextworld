using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniAni : MonoBehaviour
{
	public enum STATE
	{
		IDLE = 0,
		RUN,
		DMG,
		ATTACK,
		ITEMOPEN,
	}


	public Mini mini;
	public Animation[] ani;

	public const string RUN = "Run";
	public const string IDLE = "Idle";
	public const string DMG001 = "Dmg001";
	public const string DMG002 = "Dmg002";
	public const string ATK001 = "Atk001";
	public const string ATK002 = "Atk002";
	public const string ATKLB_FULL = "Atk_LBullet_full"; //왼손 원거리 전체
	public const string ATKLB_SHOT = "Atk_LBullet_shot"; //왼손 원거리 발사
	public const string ATKRB_FULL = "Atk_RBullet_full"; //오른손 전체
	public const string ATKRB_SHOT = "Atk_RBullet_shot"; //오른손 발사
	public const string ATKB_FULL = "Atk_Bullet_full"; //양손 전체
	public const string ATKB_SHOT = "Atk_Bullet_shot"; //양손 발사

	public const string ITEMOPEN = "ItemOpen";

	private float next_time;

	[HideInInspector]
	public STATE c_state;
	private string c_dmg;
	[HideInInspector]
	public string c_atk;

	public ITEM_DATA.TYPE_2 atk_arm;

	private int typebullet;
	private int typesword;
	private int cntatk;

	public float motion_time;
	private int n;
	private float f;

	public Player player;

	public float add_speed;

	public void Init()
	{
		player = null;

		next_time = -1;
		c_state = STATE.IDLE;
		for (n = 0; n < ani.Length; n++)
		{
			ani[n][RUN].speed = 2f;
			ani[n][DMG001].speed = 3.9f;
			ani[n][DMG002].speed = 3.9f;
			//ani[n][ATK001].speed = 1f;
			//ani[n][ATK002].speed = 1f;
			ani[n][ITEMOPEN].speed = 2f;
		}
		c_atk = ATK001;
		cntatk = 0;
	}

	public float GetNextTime() { return next_time; }

	public void UpdateAni(bool isRun)
	{
		/*
		if (c_state == STATE.DMG)
		{
			for (n = 0; n < ani.Length; n++) ani[n].CrossFade(c_dmg, 0.3f);
		}
		if (c_state == STATE.ITEMOPEN)
		{
			ani[1].CrossFade(ITEMOPEN, 0.3f);
		}
		if (c_state == STATE.ATTACK) { for (n = 0; n < ani.Length; n++) ani[n].CrossFade(c_atk, 0.3f); }
		
		if (next_time > Time.time) return;

		if (isRun == false)
		{
			c_state = STATE.IDLE;
			for (n = 0; n < ani.Length; n++) ani[n].CrossFade(IDLE, 0.3f);
		}
		else
		{
			c_state = STATE.RUN;
			for (n = 0; n < ani.Length; n++) ani[n].CrossFade(RUN, 0.3f);
		}
		*/
		//Debug.Log("---" + c_state);
		switch (c_state)
		{
			case STATE.ATTACK: ani[1].CrossFade(c_atk, 0.3f); break;
			case STATE.DMG: ani[1].CrossFade(c_dmg, 0.3f); break;
			case STATE.ITEMOPEN: ani[1].CrossFade(ITEMOPEN, 0.3f); break;
			default:
				if (isRun == false) ani[1].CrossFade(IDLE, 0.2f);
				else ani[1].CrossFade(RUN, 0.3f);
				
				break;
		}

		if (isRun == false) ani[0].CrossFade(IDLE, 0.3f);
		else ani[0].CrossFade(RUN, 0.3f);
		
		if (next_time <= Time.time)
		{
			if (isRun == false) c_state = STATE.IDLE;
			else c_state = STATE.RUN;
		}
		
	}
	public void PlayItemOpen()
	{
		if (c_state == STATE.DMG 
			|| c_state == STATE.ATTACK
			|| c_state == STATE.ITEMOPEN) return;

		c_state = STATE.ITEMOPEN;
		next_time = Time.time + 0.5f;
		
		for (n = 0; n < ani.Length; n++) ani[n][ITEMOPEN].time = 0f;
	}

	public void PlayDamage()
	{
		if (c_state == STATE.DMG || c_state == STATE.ATTACK) return;
		c_state = STATE.DMG;
		next_time = Time.time + 0.3f;
		
		switch (Random.Range(0, 2))
		{
			case 0: c_dmg = DMG001; break;
			default: c_dmg = DMG002; break;
		}
		//ani[MiniAni.DMG001].weight = 0f;
		for (n = 0; n < ani.Length; n++) ani[n][c_dmg].time = 0f;
	}

	public bool PlayAttack()
	{
		if (c_state == STATE.ATTACK)
		{
			if (PlayAttackBuletDoing())
			{
				PlayAttackMotion();
				return true;
			}
			return false;
		}
		c_state = STATE.ATTACK;

		if (SetPlayAttack() == false)
		{
			GameMgr.ins.mgrEffect.ShowEffect(Effect.EffSmoke001, ref mini.tranBones[(int)BONE.Spine1]);
			return false;
		}

		PlayAttackMotion();
		return true;
	}

	private bool SetPlayAttack()
	{
		add_speed = 1f;

		if (mini.GetSkillType(ITEM_DATA.TYPE_2.ARML) == SKILL_DATA.ATYPE.bigsword)
		{   //대검 공격인 경우

			return true;
		}
		if (mini.GetSkillType(ITEM_DATA.TYPE_2.BODY) == SKILL_DATA.ATYPE.bigbullet)
		{   //대포 공격인 경우

			return true;
		}
		if (mini.GetSkillType(ITEM_DATA.TYPE_2.BODY) == SKILL_DATA.ATYPE.nearattack)
		{   //돌진 공격인 경우

			return true;
		}

		//원거리 공격 내용
		typebullet = 0;
		typesword = 0;
		if (mini.GetSkillType(ITEM_DATA.TYPE_2.ARML) == SKILL_DATA.ATYPE.bullet) typebullet = 1;
		if (mini.GetSkillType(ITEM_DATA.TYPE_2.ARMR) == SKILL_DATA.ATYPE.bullet)
		{
			if (typebullet == 0) typebullet = 2;
			if (typebullet == 1) typebullet = 3;
		}
		//근거리 공격 체크
		if (mini.GetSkillType(ITEM_DATA.TYPE_2.ARML) == SKILL_DATA.ATYPE.sword) typesword = 1;
		if (mini.GetSkillType(ITEM_DATA.TYPE_2.ARMR) == SKILL_DATA.ATYPE.sword)
		{
			if (typesword == 0) typesword = 2;
			if (typesword == 1) typesword = 3;
		}
		
		if (typebullet == 3)
		{   //양손 원거리 공격의 경우
			c_atk = ATKB_FULL;
			atk_arm = ITEM_DATA.TYPE_2.ARML;
			return true;
		}

		if (typesword == 3)
		{   //양손 검인 경우
			//랜덤 양손
			/*
			switch (Random.Range(0, 2))
			{
				case 0: //왼손 
					c_atk = MiniAni.ATK002;
					atk_arm = ITEM_DATA.TYPE_2.ARML;
					//f = PLAY_DATA.ins.dataSkill.dic[PLAY_DATA.ins.dataEquip.dic[mini.dicEquip[ITEM_DATA.TYPE_2.ARML]].skill].values[SKILL_DATA.VTYPE.motion];
					break;
				default: //오른손
					c_atk = MiniAni.ATK001;
					atk_arm = ITEM_DATA.TYPE_2.ARMR;
					//f = PLAY_DATA.ins.dataSkill.dic[PLAY_DATA.ins.dataEquip.dic[mini.dicEquip[ITEM_DATA.TYPE_2.ARMR]].skill].values[SKILL_DATA.VTYPE.motion];
					break;
			}
			*/
			//순차 양손
			if (cntatk == 0)
			{
				c_atk = ATK002;
				atk_arm = ITEM_DATA.TYPE_2.ARML;
			}
			if (cntatk == 1)
			{
				c_atk = ATK001;
				atk_arm = ITEM_DATA.TYPE_2.ARMR;
			}
			if (CheckMana() == false) return false;
			cntatk++;
			if (cntatk > 1) cntatk = 0;
			return true;
		}
		
		if (typebullet == 1)
		{   //왼손이 원거리인 경우
			c_atk = ATKLB_FULL;
			atk_arm = ITEM_DATA.TYPE_2.ARML;
		}
		if (typebullet == 2)
		{   //왼손 원거리인 경우
			c_atk = ATKRB_FULL;
			atk_arm = ITEM_DATA.TYPE_2.ARMR;
		}

		if (player != null
			&& player.lookNPC != null
			&& Vector3.Distance(mini.trans.position, player.lookNPC.mini.trans.position) < mini.GetSkillValue(atk_arm, SKILL_DATA.VTYPE.minlength))
		{
			if (typesword == 1)
			{   //근거리 왼손
				c_atk = ATK002;
				atk_arm = ITEM_DATA.TYPE_2.ARML;
				if (CheckMana() == false) return false;
			}
			if (typesword == 2)
			{   //근거리 오른손
				c_atk = ATK001;
				atk_arm = ITEM_DATA.TYPE_2.ARMR;
				if (CheckMana() == false) return false;
			}
			add_speed = mini.GetSkillValue(atk_arm, SKILL_DATA.VTYPE.solospeed);
		}

		return true;
	}

	private bool CheckMana()
	{
		if (player == null) return true;
		if (c_atk == ATKB_FULL
			|| c_atk == ATKB_SHOT)
		{   //양손 원거리의 경우 두 mana 합산해서 계산
			if (GameMgr.ins.mgrUI.status.EditMP(-(mini.GetSkillData(ITEM_DATA.TYPE_2.ARML).mp + mini.GetSkillData(ITEM_DATA.TYPE_2.ARMR).mp)) == false)
			{
				return false;
			}
			return true;
		}

		if (GameMgr.ins.mgrUI.status.EditMP(-mini.GetSkillData(atk_arm).mp) == false) return false;

		return true;
	}

	private bool PlayAttackBuletDoing()
	{
		switch (c_atk)
		{
			case ATKLB_FULL:
				//if (next_time - Time.time >= (4f / 8f) * motion_time) break;
				//if (next_time - Time.time + 0.3f >= (5f / 8f) * motion_time) break;
				//if (next_time - Time.time + 0.3f >= (3f / 8f) * motion_time) break;
				if (next_time - Time.time + 0.3f >= ((3f + mini.GetSkillValue(ITEM_DATA.TYPE_2.ARML, SKILL_DATA.VTYPE.delay)) / 8f) * motion_time) break;
				c_atk = MiniAni.ATKLB_SHOT;
				return true;
			case ATKRB_FULL:
				if (next_time - Time.time + 0.3f >= ((3f + mini.GetSkillValue(ITEM_DATA.TYPE_2.ARMR, SKILL_DATA.VTYPE.delay)) / 8f) * motion_time) break;
				c_atk = MiniAni.ATKRB_SHOT;
				return true;
			case ATKB_FULL:
				if (next_time - Time.time + 0.3f >= ((3f + mini.GetSkillValue(ITEM_DATA.TYPE_2.ARML, SKILL_DATA.VTYPE.delay)) / 8f) * motion_time) break;
				c_atk = MiniAni.ATKB_SHOT;
				return true;
			case ATKRB_SHOT:
				if (next_time - Time.time + 0.3f >= ((3f + mini.GetSkillValue(ITEM_DATA.TYPE_2.ARMR, SKILL_DATA.VTYPE.delay)) / 6f) * motion_time) break;
				return true;
			case ATKLB_SHOT:
			case ATKB_SHOT:
				//if (next_time - Time.time >= (1f / 6f) * motion_time) break;
				//if (next_time - Time.time + 0.3f >= (3f / 6f) * motion_time) break;
				if (next_time - Time.time + 0.3f >= ((3f + mini.GetSkillValue(ITEM_DATA.TYPE_2.ARML, SKILL_DATA.VTYPE.delay)) / 6f) * motion_time) break;
				return true;
		}
		return false;
	}

	private void PlayAttackMotion()
	{
		f = mini.GetSkillValue(atk_arm, SKILL_DATA.VTYPE.motion) * add_speed;
		motion_time = ani[1][c_atk].length * f;
		//next_time = Time.time + 1f;// 0.5f;
		next_time = (Time.time + (motion_time)) - 0.3f;

		//Debug.Log("motion_time:" + motion_time +",next_time:" + next_time + ",Time.time" + Time.time);

		//c_atk = MiniAni.ATK001; //RIGHT
		//c_atk = MiniAni.ATK002; //LEFT

		for (n = 0; n < ani.Length; n++)
		{
			//Debug.Log(ani[n][c_atk].length);
			ani[n][c_atk].speed = 1f / f;
			ani[n][c_atk].time = 0f;
		}

		switch (c_atk)
		{
			case ATKLB_FULL:
			case ATKLB_SHOT:
			case ATKRB_FULL:
			case ATKRB_SHOT:
			case ATKB_FULL:
			case ATKB_SHOT:
				break;
			default:  return;
		}

		BONE boneShot = BONE.None;
		float delay = 0f;
		/*
		switch (c_atk)
		{
			case ATKLB_SHOT:
			case ATKLB_FULL:
			case ATKB_FULL:
			case ATKB_SHOT:
				boneShot = BONE.LHand;
				break;
			case ATKRB_SHOT:
			case ATKRB_FULL:
				boneShot = BONE.RHand;
				break;
		}
		*/

		if (atk_arm == ITEM_DATA.TYPE_2.ARML) boneShot = BONE.LHand;
		else boneShot = BONE.RHand;

		//공격 대상과 일정 거리 이상 가까우면 발사하지 않음
		if (player != null
			&& (CheckMana() == false) || (
				player.lookNPC != null
				&& Vector3.Distance(mini.trans.position, player.lookNPC.mini.trans.position) < mini.GetSkillValue(atk_arm, SKILL_DATA.VTYPE.minlength)
			))
		{
			GameMgr.ins.mgrEffect.ShowEffect(Effect.EffSmoke001, ref mini.tranBones[(int)boneShot]);
			if(c_atk == ATKB_FULL || c_atk == ATKB_SHOT) GameMgr.ins.mgrEffect.ShowEffect(Effect.EffSmoke001, ref mini.tranBones[(int)BONE.RHand]);

			return;
		}
		//Debug.Log("distance::" + Vector3.Distance(mini.trans.position, player.lookNPC.mini.trans.position));
		//Debug.Log("minlength::" + mini.GetSkillValue(atk_arm, SKILL_DATA.VTYPE.minlength));
		
		if (c_atk == ATKLB_FULL
			|| c_atk == ATKRB_FULL
			|| c_atk == ATKB_FULL)
		{ delay = 0.9f * mini.GetSkillValue(atk_arm, SKILL_DATA.VTYPE.motion); }

		LeanTween.delayedCall(delay, () => 
		{
			GameMgr.ins.mgrBullet.ShotBullet(mini, atk_arm, boneShot, player, mini.tranBones[(int)boneShot]);
			//GameMgr.ins.mgrEffect.ShowEffect(Effect.EffShotSmall001, ref mini.tranBones[(int)boneShot], false);
			//if (Random.Range(0, 3) == 0) GameMgr.ins.mgrEffect.ShowEffect(Effect.EffSmoke001, ref mini.tranBones[(int)boneShot]);
		});

		if (c_atk == ATKB_FULL || c_atk == ATKB_SHOT)
		{	//양손 공격인 경우 
			BONE boneShot2 = BONE.RHand;
			ITEM_DATA.TYPE_2 shot_arm2 = ITEM_DATA.TYPE_2.ARMR;
			LeanTween.delayedCall(delay, () =>
			{
				GameMgr.ins.mgrBullet.ShotBullet(mini, shot_arm2, boneShot2, player, mini.tranBones[(int)boneShot2]);
				//GameMgr.ins.mgrEffect.ShowEffect(Effect.EffShotSmall001, ref mini.tranBones[(int)boneShot2], false);
				//if (Random.Range(0, 3) == 0) GameMgr.ins.mgrEffect.ShowEffect(Effect.EffSmoke001, ref mini.tranBones[(int)boneShot2]);
			});
		}

	}

	public void PlayIdle()
	{
		c_state = STATE.IDLE;
		next_time = -1;
	}

}
