using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STRING_DATA
{
	public int id;
	public string name;
	public string comment;
}

public class STRING_DATA_LIST
{
	public enum CELL
	{
		id,
		name,
		comment,
	}

	public Dictionary<int, STRING_DATA> dic;

	public void Init(string file)
	{
		TextAsset data = Resources.Load(file) as TextAsset;
		LoadData(data.text);
	}

	public void LoadData(string value)
	{
		if (dic == null) dic = new Dictionary<int, STRING_DATA>();
		else dic.Clear();

		string[] lines = value.Split('\n');
		string[] cate = lines[0].Split(',');

		string[] values;
		int j;
		STRING_DATA tmp;
		for (int i = 1; i < lines.Length; i++)
		{
			tmp = new STRING_DATA();
			values = lines[i].Split(',');
			for (j = 0; j < values.Length; j++)
			{
				switch ((CELL)System.Enum.Parse(typeof(CELL), cate[j]))
				{
					case CELL.id: tmp.id = int.Parse(values[j]); break;
					case CELL.name: tmp.name = values[j]; break;
					case CELL.comment: tmp.comment = values[j]; break;
				}
			}
			dic.Add(tmp.id, tmp);
		}
	}
}
