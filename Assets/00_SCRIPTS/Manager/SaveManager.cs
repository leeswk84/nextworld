using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour 
{
	private const string SAVE_TILE_MAP = "SAVE_TILE_MAP";
	private const string SAVE_DATA_GROUND = "SAVE_DATA_GROUND";
	private const string SAVE_DATA_ROAD = "SAVE_DATA_ROAD";
	private const string SAVE_DATA_BUILD = "SAVE_DATA_BUILD";
	
	private const string STR_R = "r";
	private const string STR_Y = "y";
	private const string STR_S = "s";
	private const string STR_N = "n";
	private const string STR_MOVE = "m";
	private const string STR_COLON = ":";
	private const string STR_SEMICOLON = ";";
	private const string STR_COMMA = ",";
	
	private System.Char COLON = ':';
	private System.Char SEMICOLON = ';';
	private System.Char COMMA = ',';

	public enum DIR 
	{
		NONE = -1,
		TOP = 0,
		BOTTOM,
		LEFT,
		RIGHT,
		LEFT2,
		RIGHT2,
	}

	public enum LOAD_STATE
	{
		NONE = 0,
		BUILD, 
		MOVE,
	}

	private LOAD_STATE loadState = LOAD_STATE.NONE;
	/// <summary> 행성 고유값 </summary>
	[HideInInspector]
	public int PLANET_IDX;
	/// <summary> 행성 넓이 </summary>
	[HideInInspector]
	public static int PLANET_WIDTH = 3;
	[HideInInspector]
	public static int PLANET_HEIGHT = 3;
	/// <summary> 행성 마지막 위치 </summary>
	public static int PLANET_MAX;
	
	private string strSave, str;
	
	//private List<string> strSaves;
	private string[] arystr;
	private string[] arystr2;
	private string[] parsestrs;
	private string[] parsestrs2;
	private string[] parsestrs3;

	private List<string> listBack;

	private int num, num2, num3;
	
	[HideInInspector]
	public static int[] aryIdxs;
	[HideInInspector]
	public int editIndex;

	private string[] strsaves = new string[4];

	//private int widthIndex;
	[HideInInspector]
	public Dictionary<int, List<float>> dicGround;
	[HideInInspector]
	public Dictionary<int, List<int>> dicRoad;
	[HideInInspector]
	public Dictionary<int, List<BuildData>> dicBuild;
	[HideInInspector]
	public Dictionary<int, int> blockTypes;

	private string strLoadTile = "";
	private bool isCamMove;

	private int check_num;
	private int line_first;
	private int line_center;

	private BlockManager tmpBlock;

	private int parse_num;

	public void SetPlanet(ref Planet data)
	{
		PLANET_IDX = data.planet;
		PLANET_MAX = data.count;
		PLANET_WIDTH = data.width;
		PLANET_HEIGHT = data.height;
		
		PLAY_DATA.ins.C_PLANET = PLANET_IDX.ToString();
	}

	public void Init() 
	{
		//WORLD_WIDTH = 3;
		//WORLD_MAX = (WORLD_WIDTH * WORLD_WIDTH) - 1;
		isCamMove = false;
		
		editIndex = PLAY_DATA.ins.START_MAP;	//
		//aryIdxs = new int[9];
		aryIdxs = new int[GameMgr.TILE_MAX];

		blockTypes = new Dictionary<int, int>();
		//strSaves = new List<string>();
		listBack = new List<string>();

		dicGround = new Dictionary<int, List<float>>();
		dicBuild = new Dictionary<int, List<BuildData>>();
		dicRoad = new Dictionary<int, List<int>>();

		strSave = "";
		
		SetIndex();
	}
	
	public void MoveIndex(DIR valueDir, bool isCamCon = true) 
	{
		editIndex = GetDirIndex(valueDir);
		SetIndex();
		loadState = LOAD_STATE.MOVE;
		GameMgr.ins.mgrUI.ClickLoad();
		if (!isCamCon) return;
		isCamMove = true;
	}

	public int GetDirIndex(DIR valueDir)
	{
		switch (valueDir)
		{
			case DIR.TOP: return aryIdxs[7];//1];
			case DIR.BOTTOM: return aryIdxs[17];//7];
			case DIR.LEFT: return aryIdxs[11];//3];
			case DIR.LEFT2: return aryIdxs[10];
			case DIR.RIGHT2: return aryIdxs[14];
			default:
			case DIR.RIGHT: return aryIdxs[13];//5];
		}
	}

	public void SetBlockIdx(int value, bool isBuild = true)
	{
		editIndex = value;
		SetIndex();
		if(isBuild) loadState = LOAD_STATE.BUILD;
		GameMgr.ins.mgrUI.ClickLoad();
	}

	public void SetIndex()
	{
		GameMgr.ins.mgrUI.lblGrdPos.text = editIndex.ToString();

		//Debug.Log("PLANET_WIDTH : " + PLANET_WIDTH);
		//Debug.Log("PLANET_MAX : " + PLANET_MAX);
		//center
		SetLine(editIndex, 12);

		//up
		line_center = editIndex - PLANET_WIDTH;
		if (line_center < 0) line_center = PLANET_MAX + line_center;
		SetLine(line_center, 7);

		//up up
		line_center = editIndex - (PLANET_WIDTH * 2);
		if (line_center < 0) line_center = PLANET_MAX + line_center;
		SetLine(line_center, 2);

		//down
		line_center = editIndex + (PLANET_WIDTH);
		if (line_center > (PLANET_MAX - 1)) line_center = line_center - PLANET_MAX;
		SetLine(line_center, 17);

		//down down
		line_center = editIndex + (PLANET_WIDTH * 2);
		if (line_center > (PLANET_MAX - 1)) line_center = line_center - PLANET_MAX;
		SetLine(line_center, 22);

		//Debug.Log(aryIdxs[0] + " " + aryIdxs[1] + " " + aryIdxs[2] + " " + aryIdxs[3] + " " + aryIdxs[4]);
		//Debug.Log(aryIdxs[5] + " " + aryIdxs[6] + " " + aryIdxs[7] + " " + aryIdxs[8] + " " + aryIdxs[9]);
		//Debug.Log(aryIdxs[10] + " " + aryIdxs[11] + " " + aryIdxs[12] + " " + aryIdxs[13] + " " + aryIdxs[14]);
		//Debug.Log(aryIdxs[15] + " " + aryIdxs[16] + " " + aryIdxs[17] + " " + aryIdxs[18] + " " + aryIdxs[19]);
		//Debug.Log(aryIdxs[20] + " " + aryIdxs[21] + " " + aryIdxs[22] + " " + aryIdxs[23] + " " + aryIdxs[24]);
	}

	private void SetLine(int valueIdx, int valueCur)
	{
		aryIdxs[valueCur] = valueIdx;

		line_first = Mathf.FloorToInt(valueIdx / PLANET_WIDTH) * PLANET_WIDTH; //지금 위치 좌측 처음 수
																			   //left
		check_num = valueIdx - 1;
		if (check_num < line_first) check_num = (line_first + PLANET_WIDTH) - (line_first - check_num);
		aryIdxs[valueCur - 1] = check_num;

		//left left
		check_num = valueIdx - 2;
		if (check_num < line_first) check_num = (line_first + PLANET_WIDTH) - (line_first - check_num);
		aryIdxs[valueCur - 2] = check_num;

		//right
		check_num = valueIdx + 1;
		if (check_num > line_first + PLANET_WIDTH - 1) check_num = line_first + (check_num - (line_first + PLANET_WIDTH));
		aryIdxs[valueCur + 1] = check_num;

		//right right
		check_num = valueIdx + 2;
		if (check_num > line_first + PLANET_WIDTH - 1) check_num = line_first + (check_num - (line_first + PLANET_WIDTH));
		aryIdxs[valueCur + 2] = check_num;
	}

	public void OnSave(bool isFinish = false) 
	{
		WriteStrSaveGround();
		strsaves[0] = strSave;
		//PlayerPrefs.SetString(editIndex + SAVE_DATA_GROUND, strSave);
		WriteStrSaveRoad();
		strsaves[1] = strSave;
		//PlayerPrefs.SetString(editIndex + SAVE_DATA_ROAD, strSave);
		
		WriteStrSaveBuild();
		strsaves[2] = strSave;
		//PlayerPrefs.SetString(editIndex + SAVE_DATA_BUILD, strSave);

		SetDictionary(ref dicGround, editIndex, strsaves[0].Split(SEMICOLON));
		SetDictionary(ref dicRoad, editIndex, strsaves[1].Split(SEMICOLON));
		SetDictionaryBuildData(ref dicBuild, editIndex, strsaves[2].Split(SEMICOLON));
		
		GameMgr.ins.mgrNetwork.SendMapEdit(PLANET_IDX, editIndex, strsaves[0], strsaves[1], strsaves[2], isFinish);
	}
	
	private void WriteStrSaveGround() 
	{
		strSave = "";
		for (num = 0; num < GameMgr.ins.mgrGround.vertices.Length; num++)
		{
			if (num != 0) strSave += SEMICOLON;
			strSave += GameMgr.ins.mgrGround.vertices[num].y.ToString();
		}
	}

	private void WriteStrSaveRoad()
	{
		strSave = "";
		for (num = 0; num < GameMgr.ins.mgrRoad.roadsData.Length; num++)
		{
			if (num != 0) strSave += SEMICOLON;
			strSave += GameMgr.ins.mgrRoad.roadsData[num].ToString();
		}
	}

	private void WriteStrSaveBuild() 
	{
		strSave = "";
		for (num = 0; num < GameMgr.ins.mgrBuild.dataBuild.Length; num++)
		{
			if (num != 0) strSave += SEMICOLON;
			strSave += GameMgr.ins.mgrBuild.dataBuild[num].idx.ToString();

			if (GameMgr.ins.mgrBuild.dataBuild[num].rot != 0)
			{
				strSave += RoadManager.BAR+ STR_R + STR_COLON;
				strSave += GameMgr.ins.mgrBuild.dataBuild[num].rot.ToString();
			}

			if (GameMgr.ins.mgrBuild.dataBuild[num].move_y != 0)
			{
				strSave += RoadManager.BAR + STR_Y + STR_COLON;
				strSave += GameMgr.ins.mgrBuild.dataBuild[num].move_y.ToString();
			}

			if (GameMgr.ins.mgrBuild.dataBuild[num].scale != 1)
			{
				strSave += RoadManager.BAR + STR_S + STR_COLON;
				strSave += GameMgr.ins.mgrBuild.dataBuild[num].scale.ToString();
			}

			if (GameMgr.ins.mgrBuild.dataBuild[num].index_npc != 0)
			{
				strSave += RoadManager.BAR + STR_N + STR_COLON;
				strSave += GameMgr.ins.mgrBuild.dataBuild[num].index_npc.ToString();
			}

			if (GameMgr.ins.mgrBuild.dataBuild[num].move_planet != -1)
			{
				strSave += RoadManager.BAR + STR_MOVE + STR_COLON;
				strSave += GameMgr.ins.mgrBuild.dataBuild[num].move_planet.ToString()
						+ STR_COMMA + GameMgr.ins.mgrBuild.dataBuild[num].move_tile.ToString()
						+ STR_COMMA + GameMgr.ins.mgrBuild.dataBuild[num].move_pos.ToString();
			}
		}
	}
	public void OnLoad()
	{
		isCamMove = false;
		//필요한 데이터 내용 생성..
		strLoadTile = "";
		for (num = 0; num < aryIdxs.Length; num++)
		{
			if (dicGround.ContainsKey(aryIdxs[num]) == false)
			{
				if (strLoadTile != "") strLoadTile += STR_COMMA;
				strLoadTile += aryIdxs[num].ToString();
			}
		}

		//Debug.Log(strLoadTile);
		if (strLoadTile == "")
		{   //불러올 내용이 없는 경우
			SetTiles();
			return;
		}
		GameMgr.ins.mgrNetwork.SendMapList(PLANET_IDX, strLoadTile);
	}

	private void SetDictionary(ref Dictionary<int, List<float>> dic, int value, string[] strs)
	{
		if (dic.ContainsKey(value) == false) dic.Add(value, new List<float>());

		for (parse_num = 0; parse_num < strs.Length; parse_num++)
		{
			dic[value].Add(float.Parse(strs[parse_num]));
		}
	}

	private void SetDictionary(ref Dictionary<int,List<int>> dic, int value, string[] strs)
	{
		if (dic.ContainsKey(value) == false) dic.Add(value, new List<int>());

		for (parse_num = 0; parse_num < strs.Length; parse_num++)
		{
			if (string.IsNullOrEmpty(strs[parse_num])) dic[value].Add(0);
			else dic[value].Add(int.Parse(strs[parse_num]));
		}
	}

	private void SetDictionaryBuildData(ref Dictionary<int, List<BuildData>> dic, int value, string[] strs)
	{
		if (dic.ContainsKey(value) == false) dic.Add(value, new List<BuildData>());

		for (parse_num = 0; parse_num < strs.Length; parse_num++)
		{
			dic[value].Add(new BuildData());
			parsestrs = strs[parse_num].Split(RoadManager.CHR_ROAD_DIR_BAR);

			for (num = 0; num < parsestrs.Length; num++)
			{
				if (num == 0)
				{
					if (string.IsNullOrEmpty(parsestrs[num])) dic[value][parse_num].idx = 0;
					else dic[value][parse_num].idx = int.Parse(parsestrs[num]);
					continue;
				}
				parsestrs2 = parsestrs[num].Split(COLON);
				if (parsestrs2.Length < 2) continue;
				switch (parsestrs2[0])
				{
					case STR_R: dic[value][parse_num].rot = float.Parse(parsestrs2[1]); break;
					case STR_Y: dic[value][parse_num].move_y = float.Parse(parsestrs2[1]); break;
					case STR_S: dic[value][parse_num].scale = float.Parse(parsestrs2[1]); break;
					case STR_N: dic[value][parse_num].index_npc = int.Parse(parsestrs2[1]); break;
					case STR_MOVE:
						parsestrs3 = parsestrs2[1].Split(COMMA);
						if (parsestrs3.Length < 3) break;
						dic[value][parse_num].move_planet = int.Parse(parsestrs3[0] );
						dic[value][parse_num].move_tile = int.Parse(parsestrs3[1]);
						dic[value][parse_num].move_pos = int.Parse(parsestrs3[2]);
						break;
				}
			}
		}
	}

	public void SetTiles(ref MapList data)
	{
		PLANET_MAX = data.count;
		PLANET_WIDTH = data.width;
		//PLANET_HEIGHT = data.height;
		foreach (string key in data.tiles.Keys)
		{
			num = int.Parse(key);
			if (blockTypes.ContainsKey(num)) blockTypes[num] = data.tiles[key].dt;
			else blockTypes.Add(num, data.tiles[key].dt);
			
			if (string.IsNullOrEmpty(data.tiles[key].dg) == true)
			{
				if (dicGround.ContainsKey(num) == false) dicGround.Add(num, null);
				else dicGround[num] = null;
			}
			else SetDictionary(ref dicGround, num, data.tiles[key].dg.Split(SEMICOLON));

			if (string.IsNullOrEmpty(data.tiles[key].dr) == true)
			{
				if (dicRoad.ContainsKey(num) == false) dicRoad.Add(num, null);
				else dicRoad[num] = null;
			}
			else SetDictionary(ref dicRoad, num, data.tiles[key].dr.Split(SEMICOLON));
			
			if (string.IsNullOrEmpty(data.tiles[key].db) == true)
			{
				if (dicBuild.ContainsKey(num) == false) dicBuild.Add(num, null);
				else dicBuild[num] = null;
			}
			else
			{
				//data.tiles[key].db = data.tiles[key].db.Replace(";0", ";");
				SetDictionaryBuildData(ref dicBuild, num, data.tiles[key].db.Split(SEMICOLON));
			}
		}	
		SetTiles();
	}

	public void SetTiles()
	{
		//GameMgr.ins.mgrUI.popup.Show(JsonFx.Json.JsonWriter.Serialize(data));
		GameMgr.ins.mgrSide.is_edit = false;
		//block 내용 재사용, 기존 적용된 내용 중에서 중복되는 내용은 위치 이동해서 새로 적용 하지 않음.
		for (num = 0; num < aryIdxs.Length; num++)
		{
			GameMgr.ins.mgrBlock.posBlocks[num].block.obj.SetActive(false);
		}
		for (num = 0; num < aryIdxs.Length; num++)
		{
			GameMgr.ins.mgrBlock.SetBlock(num);
			GameMgr.ins.mgrBlock.posBlocks[num].block.obj.SetActive(true);
		}
		
		for (num = 0; num < GameMgr.TILE_MAX; num++)
		{
			if (GameMgr.ins.mgrBlock.posBlocks[num].block_idx == aryIdxs[num]) continue;
			GameMgr.ins.mgrBlock.posBlocks[num].block_idx = aryIdxs[num];
			//Debug.Log(num);

			if (dicGround.ContainsKey(aryIdxs[num]) == false) dicGround.Add(aryIdxs[num], null);
			if (dicGround[aryIdxs[num]] == null
				|| dicGround[aryIdxs[num]].Count < 1)
			{
				GameMgr.ins.mgrBlock.posBlocks[num].mgrGround.Reset(true);
				dicGround[aryIdxs[num]] = new List<float>();
				for (parse_num = 0; parse_num < GameMgr.ins.mgrBlock.posBlocks[num].mgrGround.vertices.Length; parse_num++)
					dicGround[aryIdxs[num]].Add(GameMgr.ins.mgrBlock.posBlocks[num].mgrGround.vertices[parse_num].y);
			}
			else
			{
				GameMgr.ins.mgrBlock.posBlocks[num].mgrGround.SetGround(aryIdxs[num]);
			}


			if (dicBuild.ContainsKey(aryIdxs[num]) == false) dicBuild.Add(aryIdxs[num], null);
			if ( dicBuild[aryIdxs[num]] == null 
				|| dicBuild[aryIdxs[num]].Count < 1)
			{
				GameMgr.ins.mgrBlock.posBlocks[num].mgrBuild.Reset();
				dicBuild[aryIdxs[num]] = new List<BuildData>();
				for (parse_num = 0; parse_num < GameMgr.ins.mgrBlock.posBlocks[num].mgrBuild.dataBuild.Length; parse_num++)
				{
					dicBuild[aryIdxs[num]].Add(new BuildData());
					GameMgr.ins.mgrBlock.posBlocks[num].mgrBuild.dataBuild[parse_num].Copy(dicBuild[aryIdxs[num]][dicBuild[aryIdxs[num]].Count-1]);
					//dicBuild[aryIdxs[num]].Add(GameMgr.ins.mgrBlock.posBlocks[num].mgrBuild.dataBuild[parse_num]);
				}
			}
			else
			{
				GameMgr.ins.mgrBlock.posBlocks[num].mgrBuild.SetBuild(aryIdxs[num]);
			}

			if (dicRoad.ContainsKey(aryIdxs[num]) == false) dicRoad.Add(aryIdxs[num], null);
			if (dicRoad[aryIdxs[num]] == null
				|| dicRoad[aryIdxs[num]].Count < 1)
			{
				GameMgr.ins.mgrBlock.posBlocks[num].mgrRoad.Reset();
				dicRoad[aryIdxs[num]] = new List<int>();
				for (parse_num = 0; parse_num < GameMgr.ins.mgrBlock.posBlocks[num].mgrRoad.roadsData.Length; parse_num++)
					dicRoad[aryIdxs[num]].Add(GameMgr.ins.mgrBlock.posBlocks[num].mgrRoad.roadsData[parse_num]);
			}
			else
			{
				GameMgr.ins.mgrBlock.posBlocks[num].mgrRoad.SetRoad(aryIdxs[num]);
			}
		}

		for (num = 0; num < GameMgr.ins.mgrBlock.posBlocks.Length; num++)
		{
			GameMgr.ins.mgrBlock.posBlocks[num].mgrGround.SetBaseColor();
			GameMgr.ins.mgrBlock.posBlocks[num].mgrGround.SetGroundColor(false);
		}
		
		GameMgr.ins.mgrSide.SetSideUV(true);
		GameMgr.ins.mgrSide.SetGroundSideColor(loadState == LOAD_STATE.MOVE);

		GameMgr.ins.mgrSide.UpdateRoadAll();
		GameMgr.ins.mgrSide.is_edit = true;

		listBack.Clear();
		AddListBackAll();
		
		if (loadState == LOAD_STATE.BUILD)
		{
			GameMgr.ins.mgrUI.select.SetBlockMove();
		}
		if (loadState == LOAD_STATE.MOVE)
		{
			GameMgr.ins.mgrCam.MoveIndex();
		}

		System.GC.Collect();
		
		if (isCamMove)
		{
			GameMgr.ins.mgrMoveCam.IntroMoveCam(180f, 73f, -38f);
			GameMgr.ins.mgrMoveCam.Move(0f, 0f);
		}

		if (GameMgr.ins.mgrUI.objLockPlane.activeSelf 
			&& GameMgr.ins.mgrUI.login.obj.activeSelf)
		{
			LeanTween.delayedCall(0.5f, ()=> { HideLock(); });
			/*
#if UNITY_EDITOR
			HideLock();
#else
			LeanTween.delayedCall(0.5f, HideLock);	
#endif
		*/
			GameMgr.ins.GameStart();
		}
	}

	public void ShowLock(System.Action fncShow)
	{
		GameMgr.ins.mgrUI.imgLockPlane.color = GameMgr.ins.mgrUI.colPlaneHide;
		GameMgr.ins.mgrUI.objLockPlane.SetActive(true);
		LeanTween.alpha(GameMgr.ins.mgrUI.rectLockPlane, 1f, 0.35f);
		LeanTween.delayedCall(0.35f, () => { if (fncShow != null) fncShow(); });
	}

	public void HideLock(bool isactive = true)
	{
		//Debug.Log("Save Hide");
		GameMgr.ins.mgrUI.imgLockPlane.color = GameMgr.ins.mgrUI.colPlaneShow;
		LeanTween.alpha(GameMgr.ins.mgrUI.rectLockPlane, 0f, 0.5f).setEaseInOutQuad().setOnComplete(()=>
		{
			if (isactive)
			{
				GameMgr.ins.mgrUI.objLockPlane.SetActive(false);
				//Debug.Log("lock hide");
			}
		});
	}
	


	/*
	/// <summary>
	/// strSave 에 있는 땅 내용 적용
	/// </summary>
	private void SetStrLoadGround(int value) 
	{
		if (strSave == "") return;
		GameMgr.ins.mgrBlocks[value].mgrGround.SetGround(strSave.Split(';'));
	}
	
	private void SetStrLoadRoad(int value) 
	{
		if (strSave == "") return;
		GameMgr.ins.mgrBlocks[value].mgrRoad.SetRoad(strSave.Split(';'));
	}

	private void SetStrLoadBuild(int value)
	{
		if (strSave == "") return;
		GameMgr.ins.mgrBlocks[value].mgrBuild.SetBuild(strSave.Split(';'));
	}
	*/
	public void AddListBackAll()
	{
		return;
		/*
		WriteStrSaveGround();
		str = "G" + strSave;
		WriteStrSaveRoad();
		str = str + ',' + "R" + strSave;
		WriteStrSaveBuild();
		str = str + ',' + "B" + strSave;
		strSave = str;
		AddListBack();
		*/
	}

	private void AddListBack() 
	{
		return;
		/*
		listBack.Add(strSave);
		
		if (listBack.Count > 50) 
		{	//일정 갯수 이상 쌓이지 않도록
			listBack.RemoveAt(0);
		}
		*/
	}
	/*
	/// <summary> 뒤로 </summary>
	public void OnBack() 
	{
		if (listBack.Count < 2) return;
		
		strSave = listBack[listBack.Count - 1];

		arystr2 = strSave.Split(',');

		for (num2 = 0; num2 < arystr2.Length; num2++ )
		{
			strSave = arystr2[num2];
			str = strSave.Substring(0, 1);
			strSave = strSave.Substring(1);
			switch (str) 
			{
				case "G": SetStrLoadGround(4); break;
				case "R": SetStrLoadRoad(4); break;
				case "B": SetStrLoadBuild(4); break;
			}
		}
		GameMgr.ins.mgrGround.SetGroundColor();
		listBack.RemoveAt(listBack.Count - 1);
	}
	*/
}
