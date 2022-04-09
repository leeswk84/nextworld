using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PLAY_DATA : MonoBehaviour
{
	public static PLAY_DATA ins;

	[HideInInspector]
	public string C_PLANET;

	public int START_PLANET;
	public int START_MAP;

	public STATUS_DATA status;

	public ITEM_DATA_LIST dataItem;
	public EQUIP_DATA_LIST dataEquip;
	public QUEST_DATA_LIST dataQuest;

	public NPC_DATA_LIST dataNPC;
	public REWARD_DATA_LIST dataReward;
	public SKILL_DATA_LIST dataSkill;
	public STRING_DATA_LIST strItem;
	public STRING_DATA_LIST strTalk;

	public bool MOVE_DIRECT;

	private int load_cnt;
	private int total_cnt;

	public void Awake()
	{
		if (PLAY_DATA.ins != null) return;

		PLAY_DATA.ins = this;
		//GameObject.DontDestroyOnLoad(this);
		Init();
	}

	private void Start()
	{
		Load();
	}

	private void Init()
	{
		total_cnt = 0;
		load_cnt = 0;

		status = new STATUS_DATA();
		status.Init();
		
		dataItem = new ITEM_DATA_LIST();
		dataEquip = new EQUIP_DATA_LIST();
		dataQuest = new QUEST_DATA_LIST();
		dataNPC = new NPC_DATA_LIST();
		dataReward = new REWARD_DATA_LIST();
		dataSkill = new SKILL_DATA_LIST();

		//텍스트 내용
		strItem = new STRING_DATA_LIST();
		strTalk = new STRING_DATA_LIST();

		MOVE_DIRECT = false;
	}

	private void Load()
	{
		dataQuest.Init("csv/nextworld - QUEST_DATA");
		//www = UnityWebRequest.Get("https://docs.google.com/spreadsheets/d/1sXlKMiNriLeDi1XR86dRdM2upowCTBepGoVmjAPs1VY/export?format=csv&gid=0");
		StartCoroutine(LoadData("https://docs.google.com/spreadsheets/d/1DSpOqKe-_Ht6NfuZHX-HY5WxUBCzJLGzn4WoI2dqSq4/export?format=csv&gid=0", dataItem.LoadData));
		StartCoroutine(LoadData("https://docs.google.com/spreadsheets/d/1DSpOqKe-_Ht6NfuZHX-HY5WxUBCzJLGzn4WoI2dqSq4/export?format=csv&gid=926494833", dataEquip.LoadData));
		StartCoroutine(LoadData("https://docs.google.com/spreadsheets/d/1DSpOqKe-_Ht6NfuZHX-HY5WxUBCzJLGzn4WoI2dqSq4/export?format=csv&gid=326502822", dataNPC.LoadData));
		StartCoroutine(LoadData("https://docs.google.com/spreadsheets/d/1DSpOqKe-_Ht6NfuZHX-HY5WxUBCzJLGzn4WoI2dqSq4/export?format=csv&gid=1187020091", dataReward.LoadData));
		StartCoroutine(LoadData("https://docs.google.com/spreadsheets/d/1DSpOqKe-_Ht6NfuZHX-HY5WxUBCzJLGzn4WoI2dqSq4/export?format=csv&gid=1646214550", dataSkill.LoadData));

		StartCoroutine(LoadData("https://docs.google.com/spreadsheets/d/1DSpOqKe-_Ht6NfuZHX-HY5WxUBCzJLGzn4WoI2dqSq4/export?format=csv&gid=900272349", strItem.LoadData));
		StartCoroutine(LoadData("https://docs.google.com/spreadsheets/d/1DSpOqKe-_Ht6NfuZHX-HY5WxUBCzJLGzn4WoI2dqSq4/export?format=csv&gid=1252758369", strTalk.LoadData));
		
		/*
		dataItem.Init("csv/nextworld - ITEM_DATA");
		dataEquip.Init("csv/nextworld - EQUIP_DATA");
		//텍스트 내용
		strItem.Init("csv/nextworld - STR_ITEM_KR");
		strTalk.Init("csv/nextworld - STR_NPC_TALK_KR");
		GameMgr.ins.StartInit();
		*/
	}

	private IEnumerator LoadData(string path, System.Action<string> fnc)
	{
		total_cnt++;
		
		UnityWebRequest www = UnityWebRequest.Get(path);

		yield return www.SendWebRequest();

		if ( string.IsNullOrEmpty( www.error) == false)
		{
			Debug.Log("시트 불러오는 과정에서 에러 :: " + www.error);
			yield break;
		}

		Debug.Log(www.downloadHandler.text);
		fnc(www.downloadHandler.text);

		CheckLoad();
		yield break;
	}

	private void CheckLoad()
	{
		load_cnt++;

		if (load_cnt >= total_cnt) GameMgr.ins.StartInit();
	}
	
	public bool CheckItemBox(string tile, string pos)
	{
		if (status == null ) return false;
		if (status.dicItembox == null) return false;
		if (status.dicItembox.ContainsKey(C_PLANET) == false) return false;
		if (status.dicItembox[C_PLANET].ContainsKey(tile) == false) return false;
		if (status.dicItembox[C_PLANET][tile].ContainsKey(pos) == false) return false;

		return true;
	}
}

public class STATUS_DATA
{
	public string userID;
	/// <summary> 장비 데이터 아이템</summary>
	public string[] equip;
	/// <summary> 화폐 </summary>
	public long nut;
	/// <summary> 아이템을 소지할 수 있는 총 갯수 </summary>
	public int item_max;
	/// <summary> 아이템을 데이터 </summary>
	public Dictionary<string, P_ITEM_DATA> item;
	public int item_idx;
	/// <summary> 퀘스트 데이터 </summary>
	public List<P_QUEST_DATA> quest;
	/// <summary> 하단 단축 버튼</summary>
	public string[] Hotkey;
	
	/// <summary> 저장한 위치 </summary>
	public int[] savepoint;
	/// <summary> 파괴된 위치 </summary>
	public int[] destorypoint;

	/// <summary> 획득한 아이템 상자 위치</summary>
	public Dictionary<string,Dictionary<string,Dictionary<string,int>>> dicItembox;

	public void InitSavePoint()
	{
		savepoint = new int[3];
		savepoint[0] = 0;
		savepoint[1] = 0;
		savepoint[2] = 32;
		
		//PLAY_DATA.ins.status.savepoint[1] = 1;
		//PLAY_DATA.ins.status.savepoint[2] = 40; //시작 위치
		//PLAY_DATA.ins.status.savepoint[1] = 17;
		//PLAY_DATA.ins.status.savepoint[2] = 23; //시작 위치
	}

	public void Init(string userID = "")
	{
		this.userID = userID;
		nut = 0;
		item_max = 30;//97;
		equip = new string[4];
		//equip_item_index = new string[4];
		Hotkey = new string[10];

		InitSavePoint();

		item = new Dictionary<string, P_ITEM_DATA>();
		quest = new List<P_QUEST_DATA>();
		
		equip[(int)ITEM_DATA.TYPE_2.BODY] = "7";
		equip[(int)ITEM_DATA.TYPE_2.FOOT] = "8";//"3";
		equip[(int)ITEM_DATA.TYPE_2.ARML] = "14";//"4";
		equip[(int)ITEM_DATA.TYPE_2.ARMR] = "18";//"5";

		//equip_item_index[(int)ITEM_DATA.TYPE_2.BODY] = (equip[(int)ITEM_DATA.TYPE_2.BODY] - 1).ToString();
		//equip_item_index[(int)ITEM_DATA.TYPE_2.FOOT] = (equip[(int)ITEM_DATA.TYPE_2.FOOT] - 1).ToString();
		//equip_item_index[(int)ITEM_DATA.TYPE_2.ARML] = (equip[(int)ITEM_DATA.TYPE_2.ARML] - 1).ToString();
		//equip_item_index[(int)ITEM_DATA.TYPE_2.ARMR] = (equip[(int)ITEM_DATA.TYPE_2.ARMR] - 1).ToString();

		item_idx = 1;
		int i;
		P_ITEM_DATA tmp;
		for (i = 0; i < 20/*70*/; i++)
		{
			tmp = new P_ITEM_DATA();
			tmp.idx = (i + 1);// % 10;
			tmp.level = 1;
			tmp.cnt = 1;
			item.Add(item_idx.ToString(), tmp);
			item_idx++;
		}
		item["1"].cnt = 3;
		//item["2"].level = 2;
		//item["3"].level = 3;

		for (i = 0; i < Hotkey.Length; i++) Hotkey[i] = string.Empty;
	}
	//아이템 획득 정보 저장
	public void AddGetItemBox(string tile, string pos)
	{
		if(dicItembox.ContainsKey(PLAY_DATA.ins.C_PLANET) == false)
		{
			dicItembox.Add(PLAY_DATA.ins.C_PLANET, new Dictionary<string, Dictionary<string, int>>());	
		}
		if(dicItembox[PLAY_DATA.ins.C_PLANET].ContainsKey(tile) == false)
		{
			dicItembox[PLAY_DATA.ins.C_PLANET].Add(tile, new Dictionary<string, int>());
		}
		dicItembox[PLAY_DATA.ins.C_PLANET][tile].Add(pos, 0);
	}
}

public class P_ITEM_DATA
{
	public int idx;
	public int level;
	public int cnt;
}

public class P_QUEST_DATA
{
	public int idx;
	public bool clear;
}




