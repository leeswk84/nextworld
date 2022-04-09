using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : ObjectPrefab
{
	public const string EffBullet002 = "EffBullet002";
	public const string EffHit001 = "EffHit001";
	public const string EffHit002 = "EffHit002";
	public const string EffHit003 = "EffHit003";
	public const string EffHit004 = "EffHit004";
	public const string EffShot001 = "EffShot001";
	public const string EffShot002 = "EffShot002";

	public const string EffShotSmall001 = "EffShotSmall001";
	public const string EffShotSmall001Beam = "EffShotSmall002Beam";

	public const string EffSmoke001 = "EffSmoke001";
	public const string EffSmoke002 = "EffSmoke002";
	public const string EffHead001 = "EffHead001";
	public const string EffHead002 = "EffHead002";
	public const string EffNut001 = "EffNut001";
	public const string EffNut002 = "EffNut002";
	public const string EffDust001 = "EffDust001";
	
	public const string EffItem001 = "EffItem001";

	[HideInInspector]
	public string strKey;
	
	public Transform tranBody;
	public GameObject objBody;

	public bool is_gravity;
	public float HideTime;

	[HideInInspector]
	public int index;

	public ParticleSystem[] pars;

	public GameObject objUI;
	public RectTransform rectUI;
	public UnityEngine.UI.Image imgUI;

	[HideInInspector]
	public float hide_time;

	private int num;
	private Vector3 vec3;
	private Vector3 vec_speed;
	private bool move_player;
	private int bounce_count;

	private int reward_id;
	private int item_id;

	private int nut;
	
	public bool is_not_return;

	public void Init()
	{
		hide_time = -1f;
		nut = 0;
		is_not_return = false;
		
		switch(strKey)
		{
		case EffItem001:
			rectUI.SetParent(GameMgr.ins.mgrUIObj.tran);
			rectUI.localScale = Vector3.one;
			objUI.SetActive(false);
			break;
		}
	}
	public Effect SetRewardId(int value) { reward_id = value; return this; }
	public Effect SetItemId(int value) { item_id = value; return this; }

	private void MoveRandom()
	{
		vec_speed.x = Random.Range(-1f, 1f);
		vec_speed.z = Random.Range(-1f, 1f);
		vec_speed.y = Random.Range(3f, 4f);
		hide_time = Time.time + Random.Range(1f, 1.3f);
		vec3 = tran.localPosition;
		vec3.y += 0.5f;
		tran.localPosition = vec3;
	}
	public void Show()
	{
		item_id = 0;
		reward_id = 0;
		bounce_count = 0;
		move_player = false;
		tran.localScale = Vector3.one;
		vec_speed = Vector3.zero;

		if (HideTime != 0f) hide_time = Time.time + HideTime;

		switch (strKey)
		{
			case EffHead002:
			case EffHead001:
			case EffBullet002:
				vec_speed.x = Random.Range(-3f, 3f);
				vec_speed.z = Random.Range(-3f, 3f);
				vec_speed.y = Random.Range(4f, 5f);
				LeanTween.rotateAround(objBody, Vector3.left, -90f, 0.5f);
				hide_time = Time.time + 0.5f;
				break;
			case EffDust001:
			case EffHit001:
				hide_time = Time.time + 0.5f;
				break;

			case EffHit004:
			case EffHit002: hide_time = Time.time + 1f; break;
			
			case EffShot001: hide_time = Time.time + 0.3f; break;
			case EffSmoke001: hide_time = Time.time + 2f; break;

			case EffSmoke002: hide_time = Time.time + 3f; break;
			case EffShot002: hide_time = Time.time + 0.6f; break;
			case EffHit003: hide_time = Time.time + 1f; break;
			case EffItem001:
				MoveRandom();
				LeanTween.rotateAround(objUI, Vector3.forward, 720f, hide_time - Time.time);
				objUI.SetActive(true);
			break;
			case EffNut001:
			case EffNut002:
				MoveRandom();
				LeanTween.rotateAround(objBody, Vector3.left, 720f, hide_time - Time.time);
				//LeanTween.rotateAround(objBody, Vector3.up, Random.Range(-360f, 360f), hide_time - Time.time);
				//LeanTween.rotateAround(objBody, Vector3.forward, Random.Range(-360f, 360f), hide_time - Time.time);
				break;
				/*
				case EffHead001:
					vec_speed.x = Random.Range(-3f, 3f);
					vec_speed.z = Random.Range(-3f, 3f);
					vec_speed.y = Random.Range(4f, 5f);
					LeanTween.rotateAround(objBody, vec_speed, -90f, 0.5f);
					hide_time = Time.time + 0.5f;
					break;
				*/
		}
		obj.SetActive(true);
	}
	public Effect SetNut(int value, bool isPlayer = true)
	{
		nut = value;
		if (nut >= 1000) tranBody.localScale = Vector3.one * 2.5f;
		else if (nut >= 100) tranBody.localScale = Vector3.one * 2f;
		else if (nut >= 10) tranBody.localScale = Vector3.one * 1.5f;
		else tranBody.localScale = Vector3.one;

		if (isPlayer)
		{
			hide_time = -1;
			is_not_return = true;
			tran.SetParent(GameMgr.ins.PLAYER.DestroyPoint.tran);
		}
		return this;
	}

	public void SetIsNotReturn(bool value) { is_not_return = value; }
	public void SetMovePlayer() { move_player = true; }

	public void Hide()
	{
		switch (strKey)
		{
			case EffHead002:
			case EffHead001:
				//GameMgr.ins.mgrEffect.ShowNut(5, ref tran, false);
				//GameMgr.ins.mgrEffect.ShowEffect(Effect.EffItem001, ref tran);					
				GameMgr.ins.mgrEffect.ShowReward(reward_id, ref tran);

				GameMgr.ins.mgrEffect.ShowEffect(Effect.EffHit003, ref tran);
				break;
			case EffItem001:
			case EffNut002:
				if(GameMgr.ins.PLAYER.mini.ability.HP <= 0) break;
				if (move_player == false)
				{
					move_player = true;
					vec_speed.z = 1f;
					return;
				}
				break;
		}
		if (is_not_return) return;
		Return();
	}
	public void Return()
	{
		switch(strKey)
		{
			case EffItem001:
				objUI.SetActive(false);
			break;
		}
		tran.parent = GameMgr.ins.mgrEffect.tran;
		obj.SetActive(false);
	}
	public void UpdateEffect()
	{
		if(strKey == EffItem001 && objUI.activeSelf == true)
		{	
			rectUI.localPosition = GameMgr.ins.mgrUIObj.Change3D2D(ref tran); //3d 위치 2d위치로 전환
			//크기 설정
			rectUI.localScale = Vector3.one * Mathf.Clamp(13f - Vector3.Distance(GameMgr.ins.mgrCam.tranCam.position, tran.position), 2f, 10f) * 0.1f;
			//Debug.Log(Vector3.Distance(GameMgr.ins.mgrCam.tranCam.position, tran.position));
		}

		if (move_player)
		{
			vec3 = GameMgr.ins.PLAYER_MINI.trans.position;
			vec3.y += 0.5f;
			if (Vector3.Distance(tran.position, vec3) < 0.1f)
			{
				switch (strKey)
				{	//nut 증가
					case EffNut002:
						if(GameMgr.ins.PLAYER.mini.ability.HP <= 0) break;
						GameMgr.ins.mgrUI.status.EditNut(nut);
						GameMgr.ins.mgrUIObj.ShowText(nut, ref tran, true);
						break;
					case EffItem001:
						if(GameMgr.ins.PLAYER.mini.ability.HP <= 0) break;
						//GameMgr.ins.mgrUI.status.UpdateStatus
						//if(PLAY_DATA.ins.status.item.ContainsKey())
						foreach (string str in PLAY_DATA.ins.status.item.Keys)
						{
							if (item_id == PLAY_DATA.ins.status.item[str].idx)
							{
								PLAY_DATA.ins.status.item[str].cnt++;
								GameMgr.ins.mgrUI.status.popupBag.RefreshTab();
								GameMgr.ins.mgrUI.status.hotKey.RefreshList();
								Hide();
								return;
							}
						}
						
						PLAY_DATA.ins.status.item.Add(PLAY_DATA.ins.status.item_idx.ToString(), new P_ITEM_DATA());
						PLAY_DATA.ins.status.item[PLAY_DATA.ins.status.item_idx.ToString()].idx = item_id;
						PLAY_DATA.ins.status.item[PLAY_DATA.ins.status.item_idx.ToString()].level = 1;
						PLAY_DATA.ins.status.item[PLAY_DATA.ins.status.item_idx.ToString()].cnt = 1;
						PLAY_DATA.ins.status.item_idx++;
						
						GameMgr.ins.mgrUI.status.popupBag.RefreshTab();
						GameMgr.ins.mgrUI.status.hotKey.RefreshList();

						break;
				}
				Hide();
				return;
			}
			
			tran.LookAt(vec3);
			vec_speed.z += 0.3f;
			if (vec_speed.z > 30) vec_speed.z = 30;
			tran.Translate(Vector3.forward * vec_speed.z * Time.smoothDeltaTime);
			return;
		}

		if (hide_time != -1 && hide_time < Time.time)
		{
			Hide();
			return;
		}

		if (bounce_count > 3) return;
		tran.position += (vec_speed * Time.smoothDeltaTime);
		
		if (is_gravity)
		{   //중력의 영향으로 천천히 내려간다면..
			vec_speed.y -= (17f * Time.smoothDeltaTime);
			GameMgr.ins.mgrNpc.CheckGroundPoint(ref tran);

			if (GameMgr.ins.mgrNpc.hitGround.point.y + 0.1f > tran.position.y)
			{
				bounce_count++;
				
				vec_speed.y *= -0.5f;
				vec3 = tran.position;
				vec3.y = GameMgr.ins.mgrNpc.hitGround.point.y + (vec_speed.y * Time.smoothDeltaTime);//0.5f;
				tran.position = vec3;

				
				if (bounce_count > 3)
				{
					vec_speed.y = 0;
					vec3 = tran.position;
					vec3.y = GameMgr.ins.mgrNpc.hitGround.point.y + 0.1f;
					tran.position = vec3;
					//vec3 = tran.position;
					//vec3.y += 0.1f;
					//tran.position = vec3;
				}
				
			}
		}
		switch (strKey)
		{
			case EffBullet002:
				tran.localScale -= (tran.localScale * 2f * Time.smoothDeltaTime); break;
		}
	}
}
