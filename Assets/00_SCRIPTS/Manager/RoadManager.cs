using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadManager : BaseBlockManager 
{
	private Material[] mat;

	public Transform pnlRoad;
	public Road objRoad;
	public Material[] roadsMat;

	[HideInInspector]
	public Road[] roads;
	[HideInInspector]
	public int[] roadsData;

	//private Road road;
	//private Vector3 vec3;
	protected int num, numx, numy;
	private int[] ary;
	//private string[] aryStr;
	[HideInInspector]
	public string str;

	/// <summary> block 넓이 </summary>
	[HideInInspector]
	public int WIDTH;
	/// <summary> block 넓이 + 1 </summary>
	protected int WIDTH_P1;
	/// <summary> block 넓이 - 1 </summary>
	protected int WIDTH_M1;

	[HideInInspector]
	public GroundSide.TYPE SIDE_TYPE = GroundSide.TYPE.NONE;
	[HideInInspector]
	public GroundSide side;
	
	protected int num_side, num_side_idx;
	private Road tmpRoad;
	private RoadManager tmpRoadMgr;
	private List<int> list_side = new List<int>();
	private string[] strs_side;

	private Vector2[] vecs2;
	
	public const float MAT_DEGREE = 0.25f;

	public const char CHR_ROAD_DIR_BAR = '|';

	public const string BAR = "|";
	protected const string ROAD_DIR1 = "1";
	protected const string ROAD_DIR2 = "2";
	protected const string ROAD_DIR3 = "3";
	protected const string ROAD_DIR4 = "4";
	protected const string ROAD_DIR5 = "1|2";
	protected const string ROAD_DIR6 = "1|4";
	protected const string ROAD_DIR7 = "2|3";
	protected const string ROAD_DIR8 = "3|4";
	protected const string ROAD_DIR9 = "1|3";
	protected const string ROAD_DIR10 = "2|4";
	protected const string ROAD_DIR11 = "1|2|3";
	protected const string ROAD_DIR12 = "1|2|4";
	protected const string ROAD_DIR13 = "1|3|4";
	protected const string ROAD_DIR14 = "2|3|4";
	protected const string ROAD_DIR15 = "1|2|3|4";

	[HideInInspector]
	public Vector4[] road_tangents;
	[HideInInspector]
	public Vector3[] road_vertices;
	[HideInInspector]
	public int[] road_traingles;
	[HideInInspector]
	public Vector2[] road_uv;

	public void Init(int maxCount, int valueW = GameMgr.WIDTH) 
	{
		WIDTH = valueW;
		WIDTH_P1 = WIDTH + 1;
		WIDTH_M1 = WIDTH - 1;

		roads = new Road[maxCount + 1];
		roadsData = new int[maxCount];
		
		//roads[0] = objRoad;
		
		ary = new int[4];
		vecs2 = new Vector2[] { Vector2.one, Vector2.one, Vector2.one, Vector2.one };

		road_traingles = new int[] { 0, 2, 3, 0, 3, 1 };
		road_vertices = new Vector3[] { new Vector3(-0.5f, 0, -0.5f), new Vector3(-0.5f, 0, 0.5f), new Vector3(0.5f, 0, -0.5f), new Vector3(0.5f, 0, 0.5f) };
		road_tangents = new Vector4[] { new Vector4(-1f, 0, 0, -1f), new Vector4(-1f, 0, 0, -1f), new Vector4(-1f, 0, 0, -1f), new Vector4(-1f, 0, 0, -1f) };
		road_uv = new Vector2[] {	new Vector2(3 * MAT_DEGREE, 3 * MAT_DEGREE), 
										new Vector2((3 + 1) * MAT_DEGREE, 3 * MAT_DEGREE), 
										new Vector2(3 * MAT_DEGREE, (3 + 1) * MAT_DEGREE), 
										new Vector2((3 + 1) * MAT_DEGREE, (3 + 1) * MAT_DEGREE) };
	}

	public void Reset() 
	{
		for (num = 0; num < roads.Length; num++)
		{
			if (num < roadsData.Length)
			{
				roadsData[num] = 0;
			}
			if(roads[num] == null) continue;
			roads[num].gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// 길 설정
	/// </summary>
	/// <param name="idx"> 위치 값 </param>
	/// <param name="isCheckSide"> 옆쪽 체크 할지 여부 </param>
	/// <param name="valueSet"> 기본 -1 : 토글, 1 : 생성 , 0 : 제거.</param>
	public void SetRoad(int idx, bool isCheckSide = true, int valueSet = -1) 
	{
		if (valueSet == 1 ||(
			(isCheckSide == true && roadsData[idx] == 0) 
			|| (isCheckSide == false && roadsData[idx] != 0)))
		{	//도로가 없는 경우 생기도록
			if (roads[idx] == null)
			{
				roads[idx] = ((GameObject)Instantiate(objRoad.gameObject)).GetComponent<Road>();
				roads[idx].transform.SetParent(pnlRoad);
			}
			//roads[idx].transform.SetParent(pnlRoad); //GameMgr.ins.mgrBlock.posBlocks[pos_idx].block.tranRoad;

			roads[idx].Init(idx);
			roads[idx].View();
			roadsData[idx] = 1;
		}
		else if(valueSet != 1)
		{
			HideRoad(idx);
		}

		if(SIDE_TYPE == GroundSide.TYPE.NONE) GameMgr.ins.mgrSave.dicRoad[block_idx][idx] = roadsData[idx];
		RoadRefresh(idx, false, isCheckSide);
	}
	

	public void SetRoad(int value) 
	{
		if (GameMgr.ins.mgrSave.dicRoad.ContainsKey(value))
		{
			for (num = 0; num < GameMgr.ins.mgrSave.dicRoad[value].Count; num++)
			{
				if (roadsData.Length > num)
				{
					roadsData[num] = GameMgr.ins.mgrSave.dicRoad[value][num];
				}
			}
		}
		RefreshRoad();
	}

	public void SetRoadData(int value)
	{
		roadsData[value] = (roadsData[value] == 0) ? 1 : 0;
		GameMgr.ins.mgrSave.dicRoad[block_idx][value] = roadsData[value];

		RefreshRoad();

		//0 9 18
		//7 16 25
		//Debug.Log(value);
		if (value % (GameMgr.WIDTH + 1) == GameMgr.WIDTH - 1)
			GameMgr.ins.mgrSide.UpdateH(14);
		if (value % (GameMgr.WIDTH + 1)== 0)
			GameMgr.ins.mgrSide.UpdateH(15);
		if (Mathf.CeilToInt(value / (GameMgr.WIDTH + 1)) == 0)
			GameMgr.ins.mgrSide.UpdateW(12);
		//63 70
		if (Mathf.CeilToInt(value / (GameMgr.WIDTH + 1)) == GameMgr.WIDTH - 1)
			GameMgr.ins.mgrSide.UpdateW(17);
			
	}

	private void RefreshRoad()
	{
		for (num = 0; num < roadsData.Length - 1; num++)
		{
			SetRoad(num, false);
		}
	}
	/// <summary>
	/// 해당 위치 길 수정
	/// </summary>
	/// <param name="idx"> 위치 고유 값 </param>
	/// <param name="isCheckPos"> 주위의 고,저 설정 여부 </param>
	/// <param name="isCheckMat"> 주위의 길 연결 상태 설정 여부 </param>
	/// <param name="isCheckSide"> 주위 설정 여부 </param>
	public void RoadRefresh(int idx, bool isCheckPos = false, bool isCheckMat = true) 
	{
		RefreshMesh(idx);
		UpdateMaterial(idx);

		if (isCheckPos == false && isCheckMat == false) return;

		num = WIDTH + 1;

		//첫번째 줄이 아니면 윗부분 확인 
		if (Mathf.FloorToInt(idx / num) != 0)
		{
			if (isCheckMat) UpdateMaterial(idx - num);
			if (isCheckPos)
			{
				RefreshMesh(idx - num);
				//첫번째 오른쪽 부분이 아니라면 확인
				if (idx % num != 0) RefreshMesh(idx - num - 1);
				//첫번째 왼쪽 부분이 아니라면 확인
				if (idx % num != num - 2) RefreshMesh(idx - num + 1);
			}
		}
		//마지막 아랫쪽 아니라면 확인 
		if (Mathf.FloorToInt(idx / num) != num - 2)
		{
			if (isCheckMat) UpdateMaterial(idx + num);
			if (isCheckPos)
			{
				RefreshMesh(idx + num);
				//첫번째 오른쪽 부분이 아니라면 확인
				if (idx % num != 0) RefreshMesh(idx + num - 1);
				//첫번째 왼쪽 부분이 아니라면 확인
				if (idx % num != num - 2) RefreshMesh(idx + num + 1);
			}
		}
		if (idx % num != 0)
		{	//첫번째 오른쪽 부분이 아니라면 확인
			if (isCheckPos) RefreshMesh(idx - 1);
			if (isCheckMat) UpdateMaterial(idx - 1);
		}

		if (idx % num != num - 2)
		{	//첫번째 왼쪽 부분이 아니라면 확인
			if (isCheckPos) RefreshMesh(idx + 1);
			if (isCheckMat) UpdateMaterial(idx + 1);
		}
	}

	protected virtual void RefreshMesh(int idx) 
	{
		if (checkRoadVisible(idx)) return;
		
		roads[idx].vertices[0] = getGround().vertices[idx];
		roads[idx].vertices[1] = getGround().vertices[idx + 1];
		roads[idx].vertices[2] = getGround().vertices[idx + 1 + WIDTH];
		roads[idx].vertices[3] = getGround().vertices[idx + 2 + WIDTH];
		
		roads[idx].filter.sharedMesh.vertices = roads[idx].vertices;
		roads[idx].filter.sharedMesh.RecalculateBounds();
		roads[idx].filter.sharedMesh.RecalculateNormals();
	}

	public void SetMatType(int idx)
	{
		if (mat == null || mat.Length < 1) mat = new Material[1];

		if (roads[idx].matIdx == -1 || roads[idx].matIdx != 0)
		{	//땅 종류 설정..
			roads[idx].matIdx = 0;
			mat[0] = roadsMat[roads[idx].matIdx];
			roads[idx].GetComponent<Renderer>().sharedMaterials = mat;
		}
	}

	protected bool checkRoadVisible(int idx) 
	{
		if (idx < 0
			|| roads == null
			|| roads.Length <= idx
			|| roads[idx] == null
			|| roads[idx].gameObject.activeSelf == false) return true;
		return false;
	}

	public virtual void UpdateMaterial(int idx)
	{
		if (checkRoadVisible(idx)) return;

		SetMatType(idx);

		str = "";
		
		if (idx % WIDTH_P1 != WIDTH_M1 && roadsData[idx + 1] != 0)
		{   //첫번째 왼쪽 부분이 아니라면 확인
			if (string.IsNullOrEmpty(str) == false) str += BAR;
			str += ROAD_DIR1;
		}

		if (str.Contains(ROAD_DIR1) == false && pos_idx % GameMgr.TILE_WIDTH > 0 && pos_idx % GameMgr.TILE_WIDTH < GameMgr.TILE_WIDTH-1)
		{
			num_side = pos_idx - 1;
			num_side_idx = idx - WIDTH_M1;
			//Debug.Log(SaveManager.aryIdxs[num_side]);
			//Debug.Log(num_side_idx);
			if (idx % WIDTH_P1 == WIDTH_M1 && CheckSideVisible())
			{   //첫번째 왼쪽이면 옆 block 확인
				if (string.IsNullOrEmpty(str) == false) str += BAR;
				str += ROAD_DIR1;
				SetSideRoad(ROAD_DIR3);
			}
		}
		
		if (Mathf.FloorToInt(idx / WIDTH_P1) != 0 && roadsData[idx - WIDTH_P1] != 0)
		{   //첫번째 줄이 아니면 윗부분 확인 
			if (string.IsNullOrEmpty(str) == false) str += BAR;
			str += ROAD_DIR2;
		}
		
		if (str.Contains(ROAD_DIR2) == false && pos_idx > 4 && pos_idx < 20)
		{	//첫번째 줄이면 윗부분 block 확인 
			num_side = pos_idx - GameMgr.TILE_WIDTH;
			num_side_idx = idx + (WIDTH_P1 * WIDTH_M1);
			if (Mathf.FloorToInt(idx / WIDTH_P1) == 0 && CheckSideVisible())
			{
				if (string.IsNullOrEmpty(str) == false) str += BAR;
				str += ROAD_DIR2;
				SetSideRoad(ROAD_DIR4);
			}
		}
		
		if (idx % WIDTH_P1 != 0 && roadsData[idx - 1] != 0)
		{   //첫번째 오른쪽 부분이 아니라면 확인
			if (string.IsNullOrEmpty(str) == false) str += BAR;
			str += ROAD_DIR3;
		}
		
		if ( str.Contains(ROAD_DIR3) == false && pos_idx % 5 > 0 && pos_idx % 5 < 4)
		{   //오른쪽 내용 체크
			//Debug.Log("pos_idx:"+pos_idx +",idx:"+ idx);
			//Debug.Log(idx % num2);
			//Debug.Log(GameMgr.ins.mgrSave.dicRoad[SaveManager.aryIdxs[pos_idx + 1]][idx + (GameMgr.WIDTH - 1)]);
			num_side = pos_idx + 1;
			num_side_idx = idx + WIDTH_M1;
			if (idx % WIDTH_P1 == 0 && CheckSideVisible())
			{   //첫번째 오른쪽이면 옆 block 확인
				if (string.IsNullOrEmpty(str) == false) str += BAR;
				str += ROAD_DIR3;
				SetSideRoad(ROAD_DIR1);
			}
		}

		if (Mathf.FloorToInt(idx / WIDTH_P1) != WIDTH_M1 && roadsData[idx + WIDTH_P1] != 0)
		{   //마지막 아랫쪽 아니라면 확인 
			if (string.IsNullOrEmpty(str) == false) str += BAR;
			str += ROAD_DIR4;
		}

		if (str.Contains(ROAD_DIR4) == false && pos_idx > 4 && pos_idx < 20)
		{
			num_side = pos_idx + GameMgr.TILE_WIDTH;
			num_side_idx = idx - (WIDTH_P1 * WIDTH_M1);
			//아랫쪽 체크
			if (Mathf.FloorToInt(idx / WIDTH_P1) == WIDTH_M1 && CheckSideVisible())
			{   //아랫쪽이면 아래 block 확인
				if (string.IsNullOrEmpty(str) == false) str += BAR;
				str += ROAD_DIR4;
				SetSideRoad(ROAD_DIR2);
			}
		}
		//Debug.Log(idx + ":" + str);
		SetMaterial(idx);
	}

	private bool CheckSideVisible()
	{
		return GameMgr.ins.mgrSave.dicRoad.ContainsKey(SaveManager.aryIdxs[num_side])
				&& GameMgr.ins.mgrSave.dicRoad[SaveManager.aryIdxs[num_side]] != null
				&& GameMgr.ins.mgrSave.dicRoad[SaveManager.aryIdxs[num_side]].Count > num_side_idx
				&& GameMgr.ins.mgrSave.dicRoad[SaveManager.aryIdxs[num_side]][num_side_idx] != 0;
	}

	protected void SetSideRoad(string ADD_STR)
	{
		tmpRoadMgr = GameMgr.ins.mgrBlock.posBlocks[num_side].mgrRoad;
		tmpRoad = tmpRoadMgr.roads[num_side_idx];

		if (tmpRoad != null && tmpRoad.strMat.Contains(ADD_STR) == false)
		{
			tmpRoadMgr.str = tmpRoad.strMat;
			if (string.IsNullOrEmpty(tmpRoadMgr.str) == false) tmpRoadMgr.str += BAR;
			tmpRoadMgr.str += ADD_STR;

			list_side.Clear();
			strs_side = tmpRoadMgr.str.Split(CHR_ROAD_DIR_BAR);
			for (num_side = 0; num_side < strs_side.Length; num_side++)
			{
				list_side.Add(int.Parse(strs_side[num_side]));
			}
			list_side.Sort();
			tmpRoadMgr.str = "";
			for (num_side = 0; num_side < list_side.Count; num_side++)
			{
				if (num_side != 0) tmpRoadMgr.str += BAR;
				tmpRoadMgr.str += list_side[num_side];
			}
			tmpRoadMgr.SetMaterial(num_side_idx);
		}
	}

	/*
	 0  1  2  3  4
	 5  6  7  8  9
	10 11 12 13 14
	15 16 17 18 19
	20 21 22 23 24
	*/
	protected virtual void HideRoad(int value)
	{
		if (roads[value] != null) roads[value].Hide();
		roadsData[value] = 0;

		if (GameMgr.ins.mgrSave.dicRoad == null) return;
		
		HideCheckW(value);
		HideCheckH(value);
	}
	/// <summary> 좌우로 다음 블록 길 연결 여부 확인  </summary>
	private void HideCheckW(int value)
	{
		num_side = pos_idx + 1;
		num_side_idx = value + WIDTH_M1;
		if (pos_idx % 5 > 0 && pos_idx % 5 < 4
			&& GameMgr.ins.mgrSave.dicRoad[SaveManager.aryIdxs[num_side]] != null
			&& GameMgr.ins.mgrSave.dicRoad[SaveManager.aryIdxs[num_side]].Count > num_side_idx)
		{   //오른쪽 내용 체크
			if (value % WIDTH_P1 == 0
				&& GameMgr.ins.mgrSave.dicRoad[SaveManager.aryIdxs[num_side]][num_side_idx] != 0)
			{   //첫번째 오른쪽이면 옆 block 확인
				if (GameMgr.ins.mgrBlock.posBlocks[num_side].mgrRoad.roads[num_side_idx] != null)
				{
					GameMgr.ins.mgrBlock.posBlocks[num_side].mgrRoad.UpdateMaterial(num_side_idx);
				}
				return;
			}
		}

		num_side = pos_idx - 1;
		num_side_idx = value - WIDTH_M1;
		if (pos_idx % 5 > 0 && pos_idx % 5 < 4
			&& GameMgr.ins.mgrSave.dicRoad[SaveManager.aryIdxs[num_side]] != null
			&& GameMgr.ins.mgrSave.dicRoad[SaveManager.aryIdxs[num_side]].Count > num_side_idx)
		{   //왼쪽 체크
			if (value % WIDTH_P1 == WIDTH_M1 && GameMgr.ins.mgrSave.dicRoad[SaveManager.aryIdxs[num_side]][num_side_idx] != 0)
			{
				if (GameMgr.ins.mgrBlock.posBlocks[num_side].mgrRoad.roads[num_side_idx] != null)
				{
					GameMgr.ins.mgrBlock.posBlocks[num_side].mgrRoad.UpdateMaterial(num_side_idx);
				}
				return;
			}
		}
	}
	/// <summary> 위아래로 다음 블록 길 연결 여부 확인  </summary>
	private void HideCheckH(int value)
	{
		num_side = pos_idx - GameMgr.TILE_WIDTH;
		num_side_idx = value + (WIDTH_P1 * WIDTH_M1);
		if (pos_idx > 4 && pos_idx < 20
			&& GameMgr.ins.mgrSave.dicRoad[SaveManager.aryIdxs[num_side]] != null
			&& GameMgr.ins.mgrSave.dicRoad[SaveManager.aryIdxs[num_side]].Count > num_side_idx)
		{   //위쪽 체크
			if (Mathf.FloorToInt(value / WIDTH_P1) == 0 && GameMgr.ins.mgrSave.dicRoad[SaveManager.aryIdxs[num_side]][num_side_idx] != 0)
			{
				if (GameMgr.ins.mgrBlock.posBlocks[num_side].mgrRoad.roads[num_side_idx] != null)
				{
					GameMgr.ins.mgrBlock.posBlocks[num_side].mgrRoad.UpdateMaterial(num_side_idx);
				}
			}
			return;
		}

		num_side = pos_idx + GameMgr.TILE_WIDTH;
		num_side_idx = value - (WIDTH_P1 * WIDTH_M1);

		if (pos_idx > 4 && pos_idx < 20
		   && GameMgr.ins.mgrSave.dicRoad[SaveManager.aryIdxs[num_side]] != null
		   && GameMgr.ins.mgrSave.dicRoad[SaveManager.aryIdxs[num_side]].Count > num_side_idx)
		{   //아래쪽 체크
			if (Mathf.FloorToInt(value / WIDTH_P1) == WIDTH_M1 && GameMgr.ins.mgrSave.dicRoad[SaveManager.aryIdxs[num_side]][num_side_idx] != 0)
			{
				if (GameMgr.ins.mgrBlock.posBlocks[num_side].mgrRoad.roads[num_side_idx] != null)
				{
					GameMgr.ins.mgrBlock.posBlocks[num_side].mgrRoad.UpdateMaterial(num_side_idx);
				}
			}
			return;
		}
	}

	public void SetMaterial(int idx)
	{
		if (str == roads[idx].strMat) return;
		roads[idx].strMat = str;

		numx = 3; numy = 3;
		ary[0] = 0; ary[1] = 1; ary[2] = 2; ary[3] = 3;

		switch (str)
		{
			case ROAD_DIR1:
			case ROAD_DIR2:
			case ROAD_DIR3:
			case ROAD_DIR4:
				numx = 3; numy = 2;
				if (str == ROAD_DIR3) { ary[0] = 0; ary[1] = 1; ary[2] = 2; ary[3] = 3; }
				else if (str == ROAD_DIR4) { ary[0] = 2; ary[1] = 0; ary[2] = 3; ary[3] = 1; }
				else if (str == ROAD_DIR1) { ary[0] = 3; ary[1] = 2; ary[2] = 1; ary[3] = 0; }
				else { ary[0] = 1; ary[1] = 3; ary[2] = 0; ary[3] = 2; }
				break;
			case ROAD_DIR5:
			case ROAD_DIR6:
			case ROAD_DIR7:
			case ROAD_DIR8:
				numx = 2; numy = 3;
				if (str == ROAD_DIR8) { ary[0] = 2; ary[1] = 0; ary[2] = 3; ary[3] = 1; }
				else if (str == ROAD_DIR7) { ary[0] = 0; ary[1] = 1; ary[2] = 2; ary[3] = 3; }
				else if (str == ROAD_DIR6) { ary[0] = 3; ary[1] = 1; ary[2] = 2; ary[3] = 0; }
				else { ary[0] = 1; ary[1] = 0; ary[2] = 3; ary[3] = 2; }
				break;
			case ROAD_DIR9:
			case ROAD_DIR10:
				numx = 0; numy = 2;
				if (str == ROAD_DIR9) { ary[0] = 2; ary[1] = 0; ary[2] = 3; ary[3] = 1; }
				break;
			case ROAD_DIR11:
			case ROAD_DIR12:
			case ROAD_DIR13:
			case ROAD_DIR14:
				numx = 2; numy = 2;
				if (str == ROAD_DIR12) { ary[0] = 2; ary[1] = 0; ary[2] = 3; ary[3] = 1; }
				else if (str == ROAD_DIR13) { ary[0] = 0; ary[1] = 1; ary[2] = 2; ary[3] = 3; }
				else if (str == ROAD_DIR14) { ary[0] = 3; ary[1] = 1; ary[2] = 2; ary[3] = 0; }
				else { ary[0] = 3; ary[1] = 2; ary[2] = 1; ary[3] = 0; }
				break;
			case ROAD_DIR15:
				numx = 0; numy = 3;
				break;

		}
		//Debug.Log(idx +":" + str);

		vecs2[ary[0]].x = numx * MAT_DEGREE;
		vecs2[ary[0]].y = numy * MAT_DEGREE;
		vecs2[ary[1]].x = (numx + 1) * MAT_DEGREE;
		vecs2[ary[1]].y = numy * MAT_DEGREE;
		vecs2[ary[2]].x = numx * MAT_DEGREE;
		vecs2[ary[2]].y = (numy + 1) * MAT_DEGREE;
		vecs2[ary[3]].x = (numx + 1) * MAT_DEGREE;
		vecs2[ary[3]].y = (numy + 1) * MAT_DEGREE;

		roads[idx].filter.sharedMesh.uv = vecs2;
	}
	
	/// <summary> 도로 생성 인지 제거인지 </summary>
	public void UpdateEditor(int idx) 
	{
		if (roadsData[idx] == 0)
		{
			GameMgr.ins.mgrUI.select.btnMakeRoad.txt.text = TxtMgr.ins.GetTxt(TxtMgr.TYPE.ROAD_MAKE);
		}
		else GameMgr.ins.mgrUI.select.btnMakeRoad.txt.text = TxtMgr.ins.GetTxt(TxtMgr.TYPE.ROAD_DEL);
	}

}
