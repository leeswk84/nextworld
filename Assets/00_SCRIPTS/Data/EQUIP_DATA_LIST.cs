using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EQUIP_DATA
{
	public int id;
	public int lv;
	public int hp;
	//public int value;
	public int weight;
	public int[] model;
	public int skill;
}

public class EQUIP_DATA_LIST
{
	public enum CELL
	{
		id,
		lv,
		desc,
		hp,
		//value,
		weight,
		model,
		skill,
	}
	public Dictionary<int, List<EQUIP_DATA>> dic;

	public void Init(string file)
	{
		TextAsset data = Resources.Load(file) as TextAsset;
		LoadData(data.text);
	}

	public void LoadData(string value)
	{
		if(dic == null) dic = new Dictionary<int, List<EQUIP_DATA>>();
		else dic.Clear();

		string[] lines = value.Split('\n');
		string[] cate = lines[0].Split(',');
		string[] values;
		int j;
		EQUIP_DATA tmp;
		for (int i = 1; i < lines.Length; i++)
		{
			tmp = new EQUIP_DATA();
			values = lines[i].Split(',');
			for (j = 0; j < values.Length; j++)
			{
				if (cate[j].Contains("#")) continue;

				switch ((CELL)System.Enum.Parse(typeof(CELL), cate[j]))
				{
					case CELL.id: tmp.id = int.Parse(values[j]); break;
					case CELL.lv: tmp.lv = int.Parse(values[j]); break;
					case CELL.hp: tmp.hp = int.Parse(values[j]); break;
					//case CELL.value: tmp.value = int.Parse(values[j]); break;
					case CELL.weight: tmp.weight = int.Parse(values[j]); break;
					case CELL.model: tmp.model = System.Array.ConvertAll(values[j].Split('_'), int.Parse); break;
					case CELL.skill: tmp.skill = int.Parse(values[j]); break;
				}
			}
			if (dic.ContainsKey(tmp.id) == false)
			{
				dic.Add(tmp.id, new List<EQUIP_DATA>());
				dic[tmp.id].Add(new EQUIP_DATA());
			}
			dic[tmp.id].Add(tmp);
		}
	}
	public EQUIP_DATA GetData(int id, int lv)
	{
		return dic[id][lv];
	}

	public EQUIP_DATA GetData(P_ITEM_DATA data)
	{
		return dic[PLAY_DATA.ins.dataItem.dic[data.idx].value][data.level];
	}
}
