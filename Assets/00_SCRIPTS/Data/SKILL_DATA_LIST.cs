using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SKILL_DATA
{
    public int id;
    public int power;
    public int mp;
    public string res;
    public ATYPE atype;
    public Dictionary<VTYPE, float> values;

    public enum ATYPE
    {
        none = 0,
        bullet,
        nearattack,
        sword,
        body,
		bigsword,
		bigbullet,
        bodytarget,
		bullet_npc,
		foot,
    }
    public enum VTYPE
    {
        none = 0,
        lengthlook,
        lengthatk,
        userback,
        check,
        targetback,
        delay,
        delay2,
        attackcnt,
        speed,
        time,
        motion,
		minlength,
		solospeed,
		mpmax,
		follow,
		weight,
		addhp,
    }

	public float GetValue(VTYPE key)
	{
		if (values.ContainsKey(key)) return values[key];
		return 0;
	}

}

public class SKILL_DATA_LIST 
{
    private enum CELL
    {
        id,
        atype,
        power,
        mp,
        res,
        type,
        value,
		/*
		type2,
        value2,
        type3,
        value3,
        type4,
        value4,
        type5,
        value5,
		*/
    }

    public Dictionary<int, SKILL_DATA> dic;

    public void Init(string file)
    {
        UnityEngine.TextAsset data = UnityEngine.Resources.Load(file) as UnityEngine.TextAsset;
		LoadData(data.text);
    }   

    public void LoadData(string value)
	{
		if (dic == null) dic = new Dictionary<int, SKILL_DATA>();
		else dic.Clear();

		string[] lines = value.Split('\n');
		string[] cate = lines[0].Split(',');
		string[] values;
		int j;
		SKILL_DATA tmp;
		for (int i = 1; i < lines.Length; i++)
		{
            tmp = new SKILL_DATA();
            tmp.values = new Dictionary<SKILL_DATA.VTYPE, float>();
			values = lines[i].Split(',');
			SKILL_DATA.VTYPE vType = SKILL_DATA.VTYPE.none;
            for (j = 0; j < values.Length; j++)
			{
				if(cate[j].Contains("#")) continue;
				
				switch ((CELL)System.Enum.Parse(typeof(CELL), cate[j]))
				{
					case CELL.id: tmp.id = int.Parse(values[j]); break;
					case CELL.atype: tmp.atype = (SKILL_DATA.ATYPE)System.Enum.Parse(typeof(SKILL_DATA.ATYPE), values[j]);
						if (tmp.atype == SKILL_DATA.ATYPE.bigbullet
							|| tmp.atype == SKILL_DATA.ATYPE.bullet) tmp.values[SKILL_DATA.VTYPE.check] = 2f;
							break;
					case CELL.power: tmp.power = int.Parse(values[j]); break;
					case CELL.mp: tmp.mp = int.Parse(values[j]); break;
                    case CELL.res: tmp.res = values[j]; break;
					case CELL.type:
					/*
					case CELL.type1:
                    case CELL.type2:
                    case CELL.type3:
                    case CELL.type4:
                    case CELL.type5: 
					*/
                        //Debug.Log(values[j]);
                        if(string.IsNullOrEmpty(values[j])) vType = SKILL_DATA.VTYPE.none;
                        else vType = (SKILL_DATA.VTYPE)System.Enum.Parse(typeof(SKILL_DATA.VTYPE), values[j]);
                        break;
					/*
					case CELL.value1:
                    case CELL.value2:
                    case CELL.value3:
                    case CELL.value4:
                    case CELL.value5:
					*/
					case CELL.value:
                        if(vType == SKILL_DATA.VTYPE.none) break;
                        if(string.IsNullOrEmpty(values[j])) tmp.values.Add(vType, 0);
                        else tmp.values.Add(vType, int.Parse(values[j]) * 0.01f);

						switch (vType)
						{
							case SKILL_DATA.VTYPE.mpmax:
							case SKILL_DATA.VTYPE.attackcnt:
								tmp.values[vType] = Mathf.RoundToInt(tmp.values[vType] * 100f);
								break;
						}

                    break;
				}
			}
            dic.Add(tmp.id, tmp);
		}
	}

}
