using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
	public Transform tran;

	public Bullet[] prefab;

	private Dictionary<string, Bullet> dic_cre;
	private Dictionary<string, List<Bullet>> dic;
	private List<string> list_key;

	private string str;
	private int num, num2, lnum, lnum2, gnum, pnum, pnum2;
	private Vector3 vec3;
	private Bullet tmp;
	
	public void Init()
	{
		list_key = new List<string>();
		dic = new Dictionary<string, List<Bullet>>();
		dic_cre = new Dictionary<string, Bullet>();

		for (num = 0; num < prefab.Length; num++)
		{
			str = prefab[num].obj.name;
			if (dic.ContainsKey(str)) continue;

			dic.Add(str, new List<Bullet>());
			list_key.Add(str);
			dic_cre.Add(str, prefab[num]);
			
			prefab[num].obj.SetActive(false);

			for (num2 = 0; num2 < 5; num2++)
			{
				tmp = CreateBullet(str);
				tmp.obj.SetActive(false);
			}
		}
	}

	private Bullet GetBullet(string value)
	{
		if (dic.ContainsKey(value) == false) return null;
		tmp = null;
		for (gnum = 0; gnum < dic[value].Count; gnum++)
		{
			if (dic[value][gnum].obj.activeSelf == true) continue;
			tmp = dic[value][gnum];
			tmp.Type = value;

			break;
		}

		if (tmp == null) tmp = CreateBullet(value);

		return tmp;
	}

	private Bullet CreateBullet(string value)
	{
		if (dic_cre.ContainsKey(value) == false) return null;

		tmp = GameObject.Instantiate(dic_cre[value].obj).GetComponent<Bullet>();
		tmp.tran.parent = tran;
		tmp.tran.localScale = Vector3.one;
		tmp.tran.localPosition = Vector3.zero;
		tmp.obj.name = value;
		tmp.index = dic[value].Count;
		tmp.Init();

		dic[value].Add(tmp);
		return tmp;
	}

	public Bullet ShotBullet(Mini mini, ITEM_DATA.TYPE_2 type2, BONE bone, Player player, Transform tranShot = null)
	{
		tmp = ShotBullet(	mini, 
							mini.GetSkillData(type2).res, 
							ref mini.tranBones[(int)bone],
							mini.GetSkillData(type2).power,
							mini.GetSkillValue(type2, SKILL_DATA.VTYPE.time),
							mini.GetSkillValue(type2, SKILL_DATA.VTYPE.speed),
							mini.GetSkillValue(type2, SKILL_DATA.VTYPE.follow) == 0 ? null : (player.lookNPC != null) ? player.lookNPC.mini : null,
							player, tranShot
							);

		if (player != null)
		{
			tmp.objCol.layer = GameMgr.ins.PLAYER.set_layer_user_bullet;
			tmp.is_player = true;

			//마나 감소
			//GameMgr.ins.PLAYER.mini.ability.
			
			if (player.lookNPC != null) tmp.tran.LookAt(player.lookNPC.tranTarget);
			else
			{
				mini.tranHitCheck.position = mini.tranBones[(int)bone].position;
				vec3 = mini.tranHitCheck.localPosition;
				vec3.z += 10;
				mini.tranHitCheck.localPosition = vec3;
				tmp.tran.LookAt(mini.tranHitCheck);
			}
			tmp.tran.Translate(Vector3.forward * 0.3f);
		}
	
		return tmp;
	}

	public Bullet ShotBullet(Mini valueMini, string value, ref Transform valueTran, int valuePower, float time, float speed, Mini followMini, Player player = null, Transform tranShot = null)
	{	
		tmp = GetBullet(value);
		tmp.objCol.layer = GameMgr.ins.PLAYER.set_layer_bullet;
		tmp.Show(valueMini, ref valueTran, valuePower, time, speed);
		tmp.followMini = followMini;
		if (string.IsNullOrEmpty(tmp.EffShot) == false)
		{
			GameMgr.ins.mgrEffect.ShowEffect(tmp.EffShot, ref tranShot, false, -1, true);
		}
		if (string.IsNullOrEmpty(tmp.EffShotRandom) == false)
		{
			if (Random.Range(0, 3) == 0) GameMgr.ins.mgrEffect.ShowEffect(tmp.EffShotRandom, ref tranShot);
		}
		return tmp;
	}

	public void UpdateBullet()
	{
		for (lnum = 0; lnum < list_key.Count; lnum++)
		{
			if (dic.ContainsKey(list_key[lnum]) == false) continue;
			for (lnum2 = 0; lnum2 < dic[list_key[lnum]].Count; lnum2++)
			{
				if (dic[list_key[lnum]][lnum2].obj.activeSelf == false) continue;

				dic[list_key[lnum]][lnum2].UpdateBullet();
			}
		}	
	}

	/// <summary>
	/// 타일이 이동한 경우
	/// </summary>
	public void OnChangeTile()
	{
		for(pnum2 = 0; pnum2 < list_key.Count; pnum2++)
		{
			for (pnum = 0; pnum < dic[list_key[pnum2]].Count; pnum++)
			{
				if (dic[list_key[pnum2]][pnum].obj.activeSelf == false) continue;

				dic[list_key[pnum2]][pnum].InitCol();

				if (dic[list_key[pnum2]][pnum].trail == null) continue;
				
				dic[list_key[pnum2]][pnum].trail.Clear();
				dic[list_key[pnum2]][pnum].obj.SetActive(false);
				dic[list_key[pnum2]][pnum].obj.SetActive(true);

				
			}
		}
	}

	public void ReturnAll()
	{
		for (lnum = 0; lnum < list_key.Count; lnum++)
		{
			for (lnum2 = 0; lnum2 < dic[list_key[lnum]].Count; lnum2++)
			{
				dic[list_key[lnum]][lnum2].obj.SetActive(false);
				dic[list_key[lnum]][lnum2].tran.parent = tran;
			}
		}
	}

}
