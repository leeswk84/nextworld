using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
	public Transform tran;

	public Effect[] prefab;

	private Dictionary<string, Effect> dic_cre;
	private Dictionary<string, List<Effect>> dic;
	private List<string> list_key;

	private string str;
	private int num, num2, lnum, lnum2, gnum;
	private Vector3 vec3;
	private Effect tmp;
	
	public void Init()
	{
		dic = new Dictionary<string, List<Effect>>();
		dic_cre = new Dictionary<string, Effect>();
		list_key = new List<string>();

		for (num = 0; num < prefab.Length; num++)
		{
			str = prefab[num].obj.name;
			if (dic.ContainsKey(str)) continue;

			dic.Add(str, new List<Effect>());
			list_key.Add(str);
			dic_cre.Add(str, prefab[num]);

			prefab[num].obj.SetActive(false);

			for (num2 = 0; num2 < 5; num2++)
			{
				tmp = CreateEffect(str);
				tmp.obj.SetActive(false);
			}
		}
	}

	public Effect GetEffect(string value)
	{
		if (dic.ContainsKey(value) == false) return null;
		tmp = null;
		for (gnum = 0; gnum < dic[value].Count; gnum++)
		{
			if (dic[value][gnum].obj.activeSelf == true) continue;
			tmp = dic[value][gnum];
			break;
		}

		if (tmp == null) tmp = CreateEffect(value);
		return tmp;
	}

	private Effect CreateEffect(string value)
	{
		if (dic_cre.ContainsKey(value) == false) return null;

		tmp = GameObject.Instantiate(dic_cre[value].obj).GetComponent<Effect>();
		tmp.tran.parent = tran;
		tmp.tran.localScale = Vector3.one;
		tmp.tran.localPosition = Vector3.zero;
		tmp.obj.name = value;
		tmp.strKey = value;
		tmp.index = dic[value].Count;
		tmp.Init();
		dic[value].Add(tmp);
		return tmp;
	}

	public Effect ShowEffect(string value, ref Transform valueTran, bool is_rot = true, int item_id = -1, bool init_rot = false)
	{
		tmp = GetEffect(value);
		tmp.tran.position = valueTran.position;
		if(is_rot) tmp.tran.rotation = valueTran.rotation;
		if (is_rot == false)
		{
			tmp.tran.parent = valueTran;
			tmp.tran.localRotation = Quaternion.identity;
			if(init_rot) tmp.tran.localPosition = Vector3.zero;
		}
		tmp.Show();

		if (item_id !=-1 && value == Effect.EffItem001)
		{
			tmp.imgUI.sprite = GameMgr.ins.uiatlas.GetSprite(PLAY_DATA.ins.dataItem.dic[item_id].icon);
			tmp.SetItemId(item_id);
		}

		return tmp;
	}

	public void UpdateEffect()
	{
		for (lnum = 0; lnum < list_key.Count; lnum++)
		{
			if (dic.ContainsKey(list_key[lnum]) == false) continue;
			for (lnum2 = 0; lnum2 < dic[list_key[lnum]].Count; lnum2++)
			{
				if (dic[list_key[lnum]][lnum2].obj.activeSelf == false) continue;
				dic[list_key[lnum]][lnum2].UpdateEffect();
			}
		}
	}

	public void ShowReward(int value, ref Transform tran)
	{
		if(PLAY_DATA.ins.dataReward.dic.ContainsKey(value) == false) return;
		//테이블에 의한 보상 생성
		int j;
		for(int i =0; i< PLAY_DATA.ins.dataReward.dic[value].Count; i++)
		{
			if(PLAY_DATA.ins.dataReward.dic[value][i].is_item)
			{
				for(j =0; j<PLAY_DATA.ins.dataReward.dic[value][i].stack; j++)
				{
					ShowEffect(Effect.EffItem001, ref tran, true, PLAY_DATA.ins.dataReward.dic[value][i].item_id);	
				}
			}
			else
			{
				ShowNut(PLAY_DATA.ins.dataReward.dic[value][i].stack, ref tran);	
			}
		}
				
		/*
		if(BuildManager.GetBuildData(interBuild).index_npc == 2)
		{
			GameMgr.ins.mgrEffect.ShowNut(315, ref interBuild.tran);
			//GameMgr.ins.mgrEffect.ShowEffect(Effect.EffItem001, ref interBuild.tran);					
			//for (i = 0; i < 3; i++)	{ GameMgr.ins.mgrEffect.ShowEffect(Effect.EffNut002, ref interBuild.tran).SetNut(100, false); }
			//for (i = 0; i < 15; i++)	{ GameMgr.ins.mgrEffect.ShowEffect(Effect.EffNut002, ref interBuild.tran).SetNut(1, false); }
		}
		*/
	}

	public void ShowNut(int nut, ref Transform tran, bool isdestroyPlayer = false)
	{
		int i;
		if (nut > 10)
		{
			nut -= 10;
			for (i = 0; i < 10; i++)
			{ 
				tmp = ShowEffect(Effect.EffNut002, ref tran).SetNut(1, isdestroyPlayer);
				if(isdestroyPlayer) GameMgr.ins.PLAYER.effNut.Add(tmp); 
			}
		}
		for (i = 0; i < Mathf.FloorToInt(nut / 1000); i++)
		{ 
			tmp = ShowEffect(Effect.EffNut002, ref tran).SetNut(1000, isdestroyPlayer);
			if(isdestroyPlayer) GameMgr.ins.PLAYER.effNut.Add(tmp); 
		}
		nut = nut % 1000;
		for (i = 0; i < Mathf.FloorToInt(nut / 100); i++)
		{ 
			tmp = ShowEffect(Effect.EffNut002, ref tran).SetNut(100, isdestroyPlayer);
			if(isdestroyPlayer) GameMgr.ins.PLAYER.effNut.Add(tmp);
		}
		nut = nut % 100;
		for (i = 0; i < Mathf.FloorToInt(nut / 10); i++)
		{
			tmp = ShowEffect(Effect.EffNut002, ref tran).SetNut(10, isdestroyPlayer);
			if(isdestroyPlayer) GameMgr.ins.PLAYER.effNut.Add(tmp); 
		}
		nut = nut % 10;
		for (i = 0; i < Mathf.FloorToInt(nut % 10); i++)
		{
			tmp = ShowEffect(Effect.EffNut002, ref tran).SetNut(1, isdestroyPlayer);
			if(isdestroyPlayer) GameMgr.ins.PLAYER.effNut.Add(tmp); 
		}
	}
	public void ReturnAll()
	{
		for (lnum = 0; lnum < list_key.Count; lnum++)
		{
			for (lnum2 = 0; lnum2 < dic[list_key[lnum]].Count; lnum2++)
			{
				if (dic[list_key[lnum]][lnum2].is_not_return) continue;
				dic[list_key[lnum]][lnum2].Return();
			}
		}
	}
}
