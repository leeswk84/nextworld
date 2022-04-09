using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ITEM_DATA
{
	public enum TYPE_1
	{
		NONE,
		EQUIP,
		ITEM,
		CODE,
		MAX,
	}
	public enum TYPE_2
	{
		BODY = 0,
		FOOT,
		ARML,
		ARMR,
		WEAPONR,
		HP_UP,
		CHAT,
		GENIE,
		MAX,
	}
	public int id;

	public TYPE_1 type1;
	public TYPE_2 type2;
	public int value;
	public string icon;
	public int stringID;
}

public class ITEM_DATA_LIST
{
	public enum CELL
	{
		id,
		type1,
		type2,
		value,
		icon,
		stringID,
	}

	public Dictionary<int, ITEM_DATA> dic;
	
	public void Init(string file)
	{
		TextAsset data = Resources.Load(file) as TextAsset;
		LoadData(data.text);
	}
	
	public void LoadData(string value)
	{
		if (dic == null) dic = new Dictionary<int, ITEM_DATA>();
		else dic.Clear();

		string[] lines = value.Split('\n');
		string[] cate = lines[0].Split(',');
		string[] values;
		int j;
		ITEM_DATA tmp;
		for (int i = 1; i < lines.Length; i++)
		{
			tmp = new ITEM_DATA();
			values = lines[i].Split(',');
			for (j = 0; j < values.Length; j++)
			{
				if (cate[j].Contains("#")) continue;

				switch ((CELL)System.Enum.Parse(typeof(CELL), cate[j]))
				{
					case CELL.id: tmp.id = int.Parse(values[j]); break;
					case CELL.type1: tmp.type1 = (ITEM_DATA.TYPE_1)System.Enum.Parse(typeof(ITEM_DATA.TYPE_1), values[j]); break;
					case CELL.type2: tmp.type2 = (ITEM_DATA.TYPE_2)System.Enum.Parse(typeof(ITEM_DATA.TYPE_2), values[j]); break;
					case CELL.value: tmp.value = int.Parse(values[j]); break;
					case CELL.icon: tmp.icon = values[j]; break;
					case CELL.stringID: tmp.stringID = int.Parse(values[j]); break;
				}
			}
			dic.Add(tmp.id, tmp);
		}
	}
	

}
