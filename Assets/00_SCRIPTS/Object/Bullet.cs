using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public string EffShot;
	public string EffShotRandom;
	public string EffHit;
	public string EffHit2;

	public Mini mini;

	public const string Bullet001 = "Bullet001";
	public const string Bullet002 = "Bullet002";
	public const string Bullet003 = "Bullet003";
	
	public GameObject obj;
	public Transform tran;
	public ParticleSystem[] par;
	public LineRenderer line;
	public TrailRenderer trail;

	public GameObject objCol;
	public CapsuleCollider col;
	public Transform tranBack;

	public Mini followMini;

	[HideInInspector]
	public int index;
	[HideInInspector]
	public int power;
	[HideInInspector]
	public float show_time;

	private float hide_time;

	private int layer_ground;

	private Vector3 vec3;
	private Quaternion rot_cur, rot_dir;
	private float speed;
	//private Vector3 vecBefore;

	public string Type;
	public bool is_player;
	
	public void Init()
	{
		layer_ground = 1 << LayerMask.NameToLayer(TxtMgr.layer_ground) | 1 << LayerMask.NameToLayer(TxtMgr.layer_build);
	}

	public void Show(Mini valueMini, ref Transform valuetran, int valuePower, float time, float speed)
	{
		followMini = null;
		is_player = false;
		mini = valueMini;
		power = valuePower;
		obj.SetActive(true);
		tran.parent = valuetran;
		tran.localRotation = Quaternion.identity;
		tran.localPosition = Vector3.zero;
		tran.parent = GameMgr.ins.mgrBullet.tran;

		//vecBefore = tran.position;

		if (trail != null) trail.Clear();
		/*
		hide_time = Time.time + 2f; //2초 동안 화면에 보임
		this.speed = 5f;
		
		switch (obj.name)
		{
			case Bullet001:
			case Bullet003:
				hide_time = Time.time + 10f; //10초 동안 보임
				this.speed = 2f;
				break;
		}
		*/
		if (speed != 0f) this.speed = speed;
		if (time != 0f) hide_time = Time.time + time;
		
		InitCol();
	}

	public void InitCol()
	{
		vec3 = Vector3.zero;
		vec3.z = -0.15f;
		tranBack.localPosition = vec3;
		col.height = 0.3f;
		UpdateCol();
	}

	private void UpdateCol()
	{
		col.height = Vector3.Distance(tran.position, tranBack.position);
		vec3 = Vector3.zero;
		vec3.z = col.height * -0.5f;
		col.center = vec3;
	}

	public void UpdateBullet()
	{
		if (Time.time > hide_time)
		{
			//Hide();
			obj.SetActive(false);
			return;
		}

		if (followMini != null)
		{
			if (followMini.ability.HP <= 0)
			{
				Hide();
				return;
			}

			vec3 = followMini.tranTarget.position;// GameMgr.ins.PLAYER.mini.trans.position;
			//vec3.y += 0.2f;

			tran.LookAt(vec3);
			rot_dir = mini.trans.rotation;
			rot_cur = mini.trans.rotation = Quaternion.Lerp(rot_cur, rot_dir, speed * Time.smoothDeltaTime);
		}
		/*
		switch (obj.name)
		{
			case Bullet001:
			case Bullet003:
				vec3 = GameMgr.ins.PLAYER.mini.trans.position;
				vec3.y += 0.2f;

				tran.LookAt(vec3);
				rot_dir = mini.trans.rotation;
				rot_cur = mini.trans.rotation = Quaternion.Lerp(rot_cur, rot_dir, speed * Time.smoothDeltaTime);
				break;
		}
		*/
		vec3 = tran.position;
		tran.Translate(Vector3.forward * Time.smoothDeltaTime * speed);
		tranBack.position = vec3;
		UpdateCol();
		/*
		if (trail != null && Vector3.Distance(vecBefore, tran.position) > 1f)
		{
			obj.SetActive(false);
			trail.Clear();
			obj.SetActive(true);
		}
		vecBefore = tran.position;
		*/
		CheckCollision();
	}

	public void Hide()
	{
		obj.SetActive(false);

		if (string.IsNullOrEmpty(EffHit) == false)
		{
			GameMgr.ins.mgrEffect.ShowEffect(EffHit, ref tran);
			if (string.IsNullOrEmpty(EffHit2) == false)
			{
				GameMgr.ins.mgrEffect.ShowEffect(EffHit2, ref tran);
			}
			return;
		}

		switch (obj.name)
		{
			case Bullet002:
				//GameMgr.ins.mgrEffect.ShowEffect(Effect.EffHit002, ref tran);
				GameMgr.ins.mgrEffect.ShowEffect(Effect.EffHit002, ref tran);
				GameMgr.ins.mgrEffect.ShowEffect(Effect.EffBullet002, ref tran);
				break;
			case Bullet001:
			case Bullet003:
				GameMgr.ins.mgrEffect.ShowEffect(Effect.EffHit004, ref tran);
				break;
		}

	}

	private void CheckCollision()
	{
		if (Physics.CheckCapsule(tran.position, tranBack.position, col.radius, layer_ground ) == true)
		//if (Physics.CheckSphere(tran.position, col.radius, layer_ground) == true)
		{   //땅과 충돌 하면 사라지도록
			Hide();
		}

	}
}
