using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QUEST_DATA
{
	public int id;
	public int before_quest;
	public int stringID;
}

public class QUEST_DATA_LIST
{
	public enum CELL
	{
		id,
		before_quest,
		stringID,
	}

	public Dictionary<int, QUEST_DATA> dic;

	public void Init(string file)
	{
		dic = new Dictionary<int, QUEST_DATA>();

	}	
}
