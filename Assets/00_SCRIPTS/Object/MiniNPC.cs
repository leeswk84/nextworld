using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniNPC : MonoBehaviour
{
	public enum STATE
	{
		NONE = 0,
		HIDE,
		SHOW,
		DMG,
		MOVE,
		IDLE,
		MAX,
	}

	public enum OBJ_BODY
	{
		None = -1,
		BulletShotBody,
		LeftDoor,
		RightDoor,
		Head,
		Max,
	}

	public Mini mini;
	public Build build;
	
	public Transform[] tranBullet;
	public GameObject[] objBodys;
	
	public STATE cur_state;
	private STATE backup_state;
	private STATE base_state;

	private Quaternion rot_dir;
	private Quaternion rot_cur;

	private Quaternion rot_head_dir;
	private Quaternion rot_head_cur;

	private Vector3 vec3;
	private int layer_player;

	private float next_time;
	private float shot_time;
	private int move_pos;
	///public bool is_base_move;
	[HideInInspector]
	public bool is_active;

	public float hit_area;

	[HideInInspector]
	public UIObject uihp;

	public Transform tranTarget;
	public GameObject objBaseMove;
	public Transform tranShadow;
	public GameObject objShadow;

	private float distance_before;
	private float distance;
	public float uihpY;

	public NPC_DATA data;
	
	public GameObject objActive;
	public GameObject objHide0;
	public GameObject objHide1;
	public GameObject objBroken;

	public bool IS_ROTATION_X = true;

	private int cntatk;

	private RaycastHit[] hit;
	private int num;
	private Bullet tmpBullet;

	public void Init()
	{
		layer_player = 1 << LayerMask.NameToLayer(TxtMgr.layer_player);
		mini.tranTarget = tranTarget;
		//Debug.Log("npc");
		GameMgr.ins.mgrNpc.AddNpc(ref build.npc, build.TileIdx);
		rot_cur = mini.trans.rotation;
		rot_head_cur = mini.tranModelHead.rotation;
		
		if (uihp == null)
		{ 
			uihp = GameMgr.ins.mgrUIObj.GetObject(UIObject.UI_KEY_HP);
			uihp.npc = this;
			uihp.tranTarget = mini.trans;
			uihp.degreeY = uihpY;
		}
		distance = 0f;
		base_state = cur_state;
		Rebirth();
	}

	public void Rebirth()
	{
		is_active = true;
		mini.obj.SetActive(true);
		shot_time = -1f;
		next_time = -1f;
		cntatk = 0;

		cur_state = base_state;
		if (data != null)
		{
			mini.ability.HP = mini.ability.HPmax = data.abiliy.HPmax;
			mini.ability.power = data.abiliy.power;
		}
		if (objShadow != null) objShadow.SetActive(true);

		uihp.obj.SetActive(true);
		uihp.objSlider.SetActive(false);
		
		move_pos = 0;
		if (objBaseMove != null)
		{
			LeanTween.cancel(objBaseMove);
			LeanTween.moveLocalY(objBaseMove, 0.01f, 1f).setLoopPingPong(500);
		}
		mini.trans.localPosition = Vector3.zero;
		UpdatePosY();
		if (base_state == STATE.HIDE)
		{
			mini.trans.localPosition = Vector3.zero;

			LeanTween.cancel(objBodys[(int)OBJ_BODY.Head]);
			LeanTween.cancel(objBodys[(int)OBJ_BODY.LeftDoor]);
			LeanTween.cancel(objBodys[(int)OBJ_BODY.RightDoor]);

			LeanTween.moveLocalY(objBodys[(int)OBJ_BODY.Head], -1f, 0f);
			LeanTween.rotateZ(objBodys[(int)OBJ_BODY.LeftDoor], 0f, 0f);
			LeanTween.rotateZ(objBodys[(int)OBJ_BODY.RightDoor], 0f, 0f);

			if (objBroken != null) objBroken.SetActive(false);

			objActive.SetActive(false);
			objHide0.SetActive(false);
			objHide1.SetActive(false);

			if (Random.Range(0, 3) == 0) objActive.SetActive(true);
			if (Random.Range(0,2) == 0) objHide0.SetActive(true);
			if (Random.Range(0, 2) == 0) objHide1.SetActive(true);
		}
	}

	public void UpdateNPC()
	{
		if (is_active == false) return;
		//distance = Vector3.Distance(mini.trans.position, GameMgr.ins.PLAYER.trans.position);
		//Debug.Log(distance);
		if (next_time != -1f && next_time <= Time.time)
		{
			next_time = -1f;
			cur_state = backup_state;
		}
		if (next_time > Time.time) return;


		if (cur_state != STATE.HIDE && data.is_attack == true)
		{
			CheckCollision();
			if (GameMgr.ins.PLAYER.lookNPC != this) uihp.HideTarget();
		}

		switch (cur_state)
		{
			case STATE.HIDE:

				if (Physics.CheckSphere(mini.trans.position, 3f, layer_player) == true)
				{   //일정거리 이상 유저 가까우면 등장
					objActive.SetActive(true);
					objHide0.SetActive(false);
					objHide1.SetActive(false);

					cur_state = STATE.SHOW;
					LeanTween.cancel(objBodys[(int)OBJ_BODY.Head]);
					LeanTween.cancel(objBodys[(int)OBJ_BODY.LeftDoor]);
					LeanTween.cancel(objBodys[(int)OBJ_BODY.RightDoor]);

					LeanTween.moveLocalY(objBodys[(int)OBJ_BODY.Head], 0.1f, 0.7f).setEaseOutCirc();//.setEaseOutBack();
					LeanTween.rotateZ(objBodys[(int)OBJ_BODY.LeftDoor], 170f, 0.5f).setEaseOutCirc();
					LeanTween.rotateZ(objBodys[(int)OBJ_BODY.RightDoor], -170f, 0.5f).setEaseOutCirc();

					shot_time = Time.time + 2f;
				}
				break;
			case STATE.SHOW:
				//if (distance > 3.5f)
				if (Physics.CheckSphere(mini.trans.position, 3.5f, layer_player) == false)
				{   //일정거리 이상 멀어지면 다시 숨김
					cur_state = STATE.HIDE;
					LeanTween.cancel(objBodys[(int)OBJ_BODY.Head]);
					LeanTween.cancel(objBodys[(int)OBJ_BODY.LeftDoor]);
					LeanTween.cancel(objBodys[(int)OBJ_BODY.RightDoor]);

					LeanTween.moveLocalY(objBodys[(int)OBJ_BODY.Head], -1f, 0.5f).setEaseInCirc();//.setEaseInBack();
					LeanTween.rotateZ(objBodys[(int)OBJ_BODY.LeftDoor], 0f, 0.5f).setEaseInCirc();
					LeanTween.rotateZ(objBodys[(int)OBJ_BODY.RightDoor], 0f, 0.5f).setEaseInCirc();
					break;
				}
				//유저를 바라봄
				/*
				vec3 = GameMgr.ins.PLAYER_MINI.trans.position;
				vec3.y += 0.5f;
				mini.tranModelHead.LookAt(vec3);
				if (IS_ROTATION_X == false)
				{
					vec3 = mini.tranModelHead.localEulerAngles;
					vec3.x = 0;
					mini.tranModelHead.localEulerAngles = vec3;
				}
				rot_head_dir = mini.tranModelHead.rotation;
				rot_head_cur = mini.tranModelHead.rotation = Quaternion.Lerp(rot_head_cur, rot_head_dir, 5f * Time.smoothDeltaTime);
				*/
				LookAtPlayerHead();
				if (Vector3.Distance(build.tran.position, GameMgr.ins.PLAYER_MINI.trans.position) < 1f)
				{
					if (data.is_attack == false && data.index_talk != 0) GameMgr.ins.talk.ShowTalkBtn(data.index_talk, this);
					break;
				}
				ShotBullet();
				break;
			case STATE.DMG:

				break;
			case STATE.MOVE:
				//너무 가까우면 대기
				if (GameMgr.ins.PLAYER.mini.ability.HP > 0 && Vector3.Distance(mini.trans.position, GameMgr.ins.PLAYER_MINI.trans.position) < 1f)
				{
					if (data.is_attack == false && data.index_talk != 0) GameMgr.ins.talk.ShowTalkBtn(data.index_talk, this);
					LookAtPlayer();
					//LookAtPlayerHead();
					return;
				}
				//Debug.Log(build);
				//Debug.Log(mini);
				//Debug.Log(data);
				if (GameMgr.ins.PLAYER.mini.ability.HP > 0 && data.is_attack && Vector3.Distance(mini.trans.position, build.tran.position) < 10f)
				{
					if (Physics.CheckSphere(mini.trans.position, 3f, layer_player) == true)
					{   //일정거리 이상 가까우면 유저 공격
						//LookAtPlayer();
						LookAtPlayerHead();
						ShotBullet();
						UpdatePosY();
						return;
					}

					if (Physics.CheckSphere(mini.trans.position, 5f, layer_player) == true)
					{   //일정거리 이상 가까우면 유저 따라감
						LookAtPlayer();
						mini.trans.Translate(Vector3.forward * 1f * Time.smoothDeltaTime);
						UpdatePosY();
						LookAtPlayerHead(true);
						return;
					}
				}
				if (data.moves != null && data.moves.Length == 1)
				{	//point 1개로 해당 방향 계속 바라보도록
					vec3 = build.tran.position;
					vec3.y = mini.trans.position.y;
					if (Vector3.Distance(vec3 , mini.trans.position) > 0.1f)
					{
						LookAtBody(ref vec3);
						mini.trans.Translate(Vector3.forward * 1f * Time.smoothDeltaTime);
						UpdatePosY();
					}
					else
					{
						vec3 = data.moves[0] + build.tran.position;
						LookAtBody(ref vec3);
					}
					LookAtPlayerHead(true);
				}
				else if (data.moves != null)
				{	//point 이동
					vec3 = data.moves[move_pos] + build.tran.position;
					vec3.y = mini.trans.position.y;
					
					LookAtBody(ref vec3);
					LookAtPlayerHead(true);

					mini.trans.Translate(Vector3.forward * 1f * Time.smoothDeltaTime);
					UpdatePosY();

					vec3 = data.moves[move_pos] + build.tran.position;
					vec3.y = mini.trans.position.y;
					distance = Vector3.Distance(vec3, mini.trans.position);
					if (distance < 0.1f || distance == distance_before)
					{
						move_pos++;
						if (move_pos >= data.moves.Length) move_pos = 0;
					}
					distance_before = distance;
				}
				else
				{
					vec3 = build.tran.position;
					vec3.z += 5;
					LookAtBody(ref vec3);
					LookAtPlayerHead(true);
					//LookAtPlayer();
				}
				break;
		}
	}

	/// <summary>
	/// 충동 체크
	/// </summary>
	private void CheckCollision ()
	{
		//hit = Physics.SphereCastAll(tranTarget.position + mini.trans.position, hit_area, Vector3.up, 0f, GameMgr.ins.PLAYER.layer_user_bullet);
		hit = Physics.SphereCastAll(tranTarget.position, hit_area, Vector3.up, 0f, GameMgr.ins.PLAYER.layer_user_bullet);
		if (hit != null && hit.Length > 0)
		{
			for (num = 0; num < hit.Length; num++)
			{
				tmpBullet = hit[num].collider.transform.parent.gameObject.GetComponent<Bullet>();
				if (tmpBullet == null) continue;
				if (tmpBullet.is_player == false) continue;

				if( Damage(tmpBullet.power, false) == true) GameMgr.ins.mgrEffect.ShowEffect(Effect.EffHit002, ref tmpBullet.tran);
				tmpBullet.Hide();
				//Debug.Log(hit[num].collider.gameObject.name);
			}
		}
		//Debug.Log(mini.obj.name + data.id);
	}

	private void ShotBullet()
	{
		//return; //test 우선 공격 안하는 모습
		if (data.is_attack == false) return;
		if (shot_time >= Time.time) return;
		if (GameMgr.ins.PLAYER.mini.ability.HP <= 0) return; //유저 파괴되면 공격하지 않음

		if (cntatk > PLAY_DATA.ins.dataSkill.dic[data.skill01].values[SKILL_DATA.VTYPE.attackcnt])
		{
			shot_time = Time.time + PLAY_DATA.ins.dataSkill.dic[data.skill01].values[SKILL_DATA.VTYPE.delay2];
			cntatk = 0;
			return;
		}
		shot_time = Time.time + PLAY_DATA.ins.dataSkill.dic[data.skill01].values[SKILL_DATA.VTYPE.delay];// delayShot;//0.3f;//0.3f;
		cntatk++;

		//총알 발사
		LeanTween.cancel(objBodys[(int)OBJ_BODY.BulletShotBody]);
		LeanTween.moveLocalZ(objBodys[(int)OBJ_BODY.BulletShotBody], 0f, 0f);
		LeanTween.moveLocalZ(objBodys[(int)OBJ_BODY.BulletShotBody], -10f, 0.15f).setLoopPingPong(1);
		
		GameMgr.ins.mgrBullet.ShotBullet(mini, 
										PLAY_DATA.ins.dataSkill.dic[data.skill01].res,
										ref tranBullet[0], 
										PLAY_DATA.ins.dataSkill.dic[data.skill01].power,
										PLAY_DATA.ins.dataSkill.dic[data.skill01].values[SKILL_DATA.VTYPE.time],
										PLAY_DATA.ins.dataSkill.dic[data.skill01].values[SKILL_DATA.VTYPE.speed],
										(PLAY_DATA.ins.dataSkill.dic[data.skill01].GetValue(SKILL_DATA.VTYPE.follow) == 0) ? null : GameMgr.ins.PLAYER.mini,
										null, tranBullet[0]);

		//GameMgr.ins.mgrBullet.ShotBullet(mini, data.BulletType.ToString(), ref tranBullet[0], mini.ability.power, data.bullet_time, data.bullet_speed);
		//GameMgr.ins.mgrBullet.ShotBullet(Bullet.Bullet002, ref tranBullet[0], mini.ability.power);
		//GameMgr.ins.mgrBullet.ShotBullet(mini, Bullet.Bullet003, ref tranBullet[0], mini.ability.power);
		
		//GameMgr.ins.mgrEffect.ShowEffect(Effect.EffShot001, ref tranBullet[0]);
		//if (Random.Range(0, 3) == 0) GameMgr.ins.mgrEffect.ShowEffect(Effect.EffSmoke001, ref tranBullet[0]);

	}

	private void LookAtPlayerHead(bool isBase = false)
	{
		vec3 = GameMgr.ins.PLAYER_MINI.trans.position;
		vec3.y += 0.5f;
		mini.tranModelHead.LookAt(vec3);
		if (isBase) mini.tranModelHead.localRotation = Quaternion.identity;
		if (IS_ROTATION_X == false)
		{
			vec3 = mini.tranModelHead.localEulerAngles;
			vec3.x = 0;
			mini.tranModelHead.localEulerAngles = vec3;
		}
		rot_head_dir = mini.tranModelHead.rotation;
		rot_head_cur = mini.tranModelHead.rotation = Quaternion.Lerp(rot_head_cur, rot_head_dir, 5f * Time.smoothDeltaTime);
	}

	private void LookAtPlayer()
	{
		vec3 = GameMgr.ins.PLAYER_MINI.trans.position;
		vec3.y += 0.2f;
		LookAtBody(ref vec3);
	}

	private void LookAtBody(ref Vector3 vec)
	{
		mini.trans.LookAt(vec);
		rot_dir = mini.trans.localRotation;
		rot_cur = mini.trans.localRotation = Quaternion.Lerp(rot_cur, rot_dir, 5f * Time.smoothDeltaTime);
		rot_head_cur = mini.tranModelHead.rotation;
	}

	private void UpdatePosY()
	{
		GameMgr.ins.mgrNpc.CheckGroundPoint(ref mini.trans);
		vec3 = mini.trans.position;
		vec3.y = GameMgr.ins.mgrNpc.hitGround.point.y;
		mini.trans.position = vec3;
		vec3.y += 0.2f;
		if(tranShadow != null) tranShadow.position = vec3;
	}

	public bool Damage(int value, bool isstate = true)
	{
		if (is_active == false) return false;
		if (isstate == true && cur_state == STATE.DMG) return false;
		
		GameMgr.ins.mgrUIObj.ShowText(value, ref mini.trans);

		mini.ability.HP -= value;
		uihp.objSlider.SetActive(true);
		uihp.slider.value = mini.ability.HP / mini.ability.HPmax;

		if (mini.ability.HP <= 0) Destroy();

		if (cur_state == STATE.DMG) return true;
		
		backup_state = cur_state;
		cur_state = STATE.DMG;
		next_time = Time.time + 0.3f;

		LeanTween.cancel(objBodys[(int)OBJ_BODY.BulletShotBody]);
		LeanTween.moveLocalZ(objBodys[(int)OBJ_BODY.BulletShotBody], 0f, 0f);
		LeanTween.moveLocalY(objBodys[(int)OBJ_BODY.BulletShotBody], 0f, 0f);
		LeanTween.moveLocalX(objBodys[(int)OBJ_BODY.BulletShotBody], 0f, 0f);

		LeanTween.moveLocalY(objBodys[(int)OBJ_BODY.BulletShotBody], Random.Range(-7f, -10), Random.Range(0.15f, 0.1f)).setLoopPingPong(1);
		LeanTween.moveLocalX(objBodys[(int)OBJ_BODY.BulletShotBody], Random.Range(-5f, 5f), Random.Range(0.15f, 0.1f)).setLoopPingPong(1);

		

		return true;
	}

	public void Destroy()
	{
		GameMgr.ins.mgrEffect.ShowEffect(Effect.EffShot002, ref mini.trans);
		GameMgr.ins.mgrEffect.ShowEffect(Effect.EffSmoke002, ref mini.trans);
		GameMgr.ins.mgrEffect.ShowEffect(Effect.EffHit003, ref mini.trans);

		//GameMgr.ins.mgrEffect.ShowEffect(Effect.EffHead001, ref mini.tranModelHead);
		GameMgr.ins.mgrEffect.ShowEffect(Effect.EffHead002, ref mini.tranModelHead).SetRewardId(data.reward);

		cur_state = STATE.NONE;

		is_active = false;

		uihp.HideTarget();

		mini.obj.SetActive(false);
		if (objBroken != null) objBroken.SetActive(true);
		
		uihp.objSlider.SetActive(false);
		if(objShadow != null) objShadow.SetActive(false);
	}
}
