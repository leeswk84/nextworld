using System.Collections;
using System.Collections.Generic;

public class REWARD_DATA
{
    public int id;
    public bool is_item;  
    public int item_id;
    public int stack;

}

public class REWARD_DATA_LIST
{
    private enum CELL
    {
        id,
        is_item,
        item_id,
        stack,
    }
  
    public Dictionary<int, List<REWARD_DATA>> dic;

    public void Init(string file)
	{
		UnityEngine.TextAsset data = UnityEngine.Resources.Load(file) as UnityEngine.TextAsset;
		LoadData(data.text);
	}

    public void LoadData(string value)
	{
		if (dic == null) dic = new Dictionary<int, List<REWARD_DATA>>();
		else dic.Clear();

		string[] lines = value.Split('\n');
		string[] cate = lines[0].Split(',');
		string[] values;
		int j;
		REWARD_DATA tmp;
		for (int i = 1; i < lines.Length; i++)
		{
            tmp = new REWARD_DATA();
			values = lines[i].Split(',');
			for (j = 0; j < values.Length; j++)
			{
				if(cate[j].Contains("#")) continue;
				
				switch ((CELL)System.Enum.Parse(typeof(CELL), cate[j]))
				{
					case CELL.id: tmp.id = int.Parse(values[j]); break;
					case CELL.is_item: tmp.is_item = values[j] == "1" ? true : false; break;
					case CELL.item_id: tmp.item_id = int.Parse(values[j]); break;
					case CELL.stack: tmp.stack = int.Parse(values[j]); break;
				}
			}

            if(dic.ContainsKey(tmp.id) == false) dic.Add(tmp.id, new List<REWARD_DATA>());
			dic[tmp.id].Add(tmp);
		}
	}

}
