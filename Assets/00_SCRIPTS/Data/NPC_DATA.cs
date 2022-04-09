using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_DATA
{
	public int id;
	public bool is_attack;
	//public MiniNPC.STATE startState;
	public int index_talk;
	public Vector3[] moves;
	
	public ABILITY abiliy;
	public int reward;
	/*
	public float delayShot;
	public string BulletType = Bullet.Bullet001;
	public float bullet_time;
	public float bullet_speed;
	*/
	public int skill01;
}

public class NPC_DATA_LIST
{
	public enum CELL
	{
		id,
		index_talk,
		reward,
		moves,
		is_attack,
		BulletType,
		HPmax,
		/*
		power,
		delayShot,
		bullet_time,
		bullet_speed,
		*/
		skill01,
	}

	public Dictionary<int, NPC_DATA> dic;

	public void Init()
	{
		//땅속에서 올라와 공격하는 적
		dic = new Dictionary<int, NPC_DATA>();
		dic.Add(1, new NPC_DATA());
		dic[1].id = 1;
		dic[1].is_attack = true;
		//dic[1].delayShot = 1f;
		//dic[1].BulletType = Bullet.Bullet002;
		dic[1].abiliy = new ABILITY();
		dic[1].abiliy.HPmax = 50;
		//dic[1].abiliy.power = 20;
		dic[1].skill01 = 1001;

		//움직이면서 말하는 npc
		dic.Add(2, new NPC_DATA());
		dic[2].id = 2;
		dic[2].is_attack = false;
		dic[2].abiliy = new ABILITY();
		dic[2].index_talk = 1;
		dic[2].moves = new Vector3[3];
		dic[2].moves[0] = new Vector3(0, 0, 2);
		dic[2].moves[1] = new Vector3(1.75f, 0, 1.77f);
		dic[2].moves[2] = new Vector3(0, 0, 0);
		
		//가만히 있는 말하는 npc
		dic.Add(3, new NPC_DATA());
		dic[3].id = 3;
		dic[3].is_attack = false;
		dic[3].abiliy = new ABILITY();
		dic[3].index_talk = 2;
		//이동 point가 1개면 이동하지 않고 해당 위치를 바라보고만 있다.
		dic[3].moves = new Vector3[1];
		dic[3].moves[0] = new Vector3(5f, 0, 5f);

		//움직이며 공격하는 적
		dic.Add(4, new NPC_DATA());
		dic[4].id = 4;

		dic[4].is_attack = true;
		//dic[4].delayShot = 0.5f;
		//dic[4].BulletType = Bullet.Bullet003;
		dic[4].abiliy = new ABILITY();
		dic[4].abiliy.HPmax = 100;
		//dic[4].abiliy.power = 35;
		dic[4].skill01 = 1000;
		dic[4].moves = new Vector3[3];
		dic[4].moves[0] = new Vector3(0, 0, 2);
		dic[4].moves[1] = new Vector3(1.75f, 0, 1.77f);
		dic[4].moves[2] = new Vector3(0, 0, 0);
	}

	public void Init(string file)
	{
		TextAsset data = Resources.Load(file) as TextAsset;
		LoadData(data.text);
	}

	public void LoadData(string value)
	{
		if (dic == null) dic = new Dictionary<int, NPC_DATA>();
		else dic.Clear();

		string[] lines = value.Split('\n');
		string[] cate = lines[0].Split(',');
		string[] values;
		string[] strs, strs2;
		int j, z;
		NPC_DATA tmp;
		for (int i = 1; i < lines.Length; i++)
		{
			tmp = new NPC_DATA();
			tmp.abiliy = new ABILITY();
			values = lines[i].Split(',');
			for (j = 0; j < values.Length; j++)
			{
				if(cate[j].Contains("#")) continue;

				switch ((CELL)System.Enum.Parse(typeof(CELL), cate[j]))
				{
					case CELL.id: tmp.id = int.Parse(values[j]); break;
					case CELL.index_talk: tmp.index_talk = int.Parse(values[j]); break;
					case CELL.moves: 
						strs = values[j].Split('_'); 
						tmp.moves = new Vector3[strs.Length];
						for(z = 0; z< strs.Length; z++)
						{
							strs2 = strs[z].Split('|');
							if(strs2.Length < 2) continue;
							tmp.moves[z].y = 0;
							tmp.moves[z].x =  int.Parse(strs2[0]) * 0.01f;
							tmp.moves[z].z =  int.Parse(strs2[1]) * 0.01f;
						}
						break;
					case CELL.is_attack: tmp.is_attack = values[j] == "1" ? true : false; break;
					case CELL.reward: tmp.reward = int.Parse(values[j]); break;
					case CELL.HPmax: tmp.abiliy.HPmax = int.Parse(values[j]); break;
					case CELL.skill01: tmp.skill01 = int.Parse(values[j]); break;
					/*
					case CELL.power: tmp.abiliy.power = int.Parse(values[j]); break;
					case CELL.delayShot: tmp.delayShot = int.Parse(values[j]) * 0.01f; break;
					case CELL.BulletType: tmp.BulletType = values[j]; break;
					case CELL.bullet_time: tmp.bullet_time = int.Parse(values[j]) * 0.01f; break;
					case CELL.bullet_speed: tmp.bullet_speed = int.Parse(values[j]) * 0.01f; break;
					*/
				}
			}
			dic.Add(tmp.id, tmp);
		}
	}
}

