using UnityEngine;
using System.Collections;

public partial class GroundManager : BaseBlockManager 
{
	public const string GrassGround = "GrassGround";

	public GameObject objFilter;
	public MeshFilter filter;
	public ButtonObject btnFilter;

	private Vector2[] vecs2;
	private Vector2 vec2;
	
	[HideInInspector]
	public Vector3[] vertices;
	
	private RaycastHit hit = new RaycastHit();
	private Ray ray;
	private Vector2[] plane_uv;
	private Color[] planeColors;
	
	[HideInInspector]
	public MeshCollider col;

	/// <summary> 높이 수정 위아래 인지 여부 </summary>
	[HideInInspector]
	public bool editUpdown = true;
	/// <summary> 수정 목표 기준 위치 </summary>
	private float dirPos = 0f;

	private int planet_count;

	private int selIdx;
	[HideInInspector]
	public int curIdx =-1;
	
	private Vector3 vec3;
	private bool check, check2;	
	private int num, num2;
	private bool isCheck;

	private const float UP_DEGREE = 0.2f;

	private Vector2 pressPos;

	//private string strSave, str, backGround, backRoad;
	
	private string[] arystr;
	private string[] arystr2;
	
	[HideInInspector]
	public Color colBase;
	
	public int getCurIdx() { return curIdx; }
	public int getSelIdx() { return selIdx; }
	public void setCurIdx(int value) { curIdx = value; }
	public void setSelIdx(int value) { selIdx = value; }

	[HideInInspector]
	public Vector2 posTile = Vector2.zero;

	public void Init() 
	{
		objFilter = filter.gameObject;
		InitParams();
		InitGround();
		SetBtns();
	}

	private void InitParams() 
	{
		selIdx = -1;
		curIdx = -1;
		
		SetBaseColor();
	}

	public void SetBaseColor() 
	{
		colBase = GameMgr.ins.mgrField.colBase; //GameMgr.ins.mgrBlocks[4].mgrGround.colBase;
		/*
		if (pos_idx != GameMgr.TILE_CENTER)
		{
			if (GameMgr.ins.mgrUI.PLAY_MODE != UIManager.MODE.MOVE)
			{
				colBase.r -= 0.2f;//0.05f;
				colBase.g -= 0.2f;//0.05f;
				colBase.b -= 0.2f;//0.05f;
			}
		}
		*/
	}

	private void SetBtns() 
	{
		//UIEventListener.Get(filter.gameObject).onPress = ClickPlane;

		btnFilter = objFilter.GetComponent<ButtonObject>();
		btnFilter.fncPress = OnPressPlane;
	}

	public void SetPosition(int valuePos)
	{
		pos_idx = valuePos;
		posTile.x = (pos_idx % GameMgr.TILE_WIDTH) - 2;
		posTile.y = Mathf.FloorToInt(pos_idx / -GameMgr.TILE_WIDTH) + 2;

		//vec3 = filter.transform.parent.position;
		vec3 = GameMgr.ins.mgrBlock.posBlocks[pos_idx].block.tran.position;
		vec3.x = posTile.x * GameMgr.WIDTH + posTile.x;
		vec3.z = posTile.y * GameMgr.WIDTH + posTile.y;
		//filter.transform.parent.position = vec3;
		GameMgr.ins.mgrBlock.posBlocks[pos_idx].block.tran.position = vec3;
	}

	private void InitGround() 
	{
		/*
		switch (TileIdx) 
		{
			case 0: posTile.x = -1; posTile.y = 1; break;
			case 1: posTile.x = 0; posTile.y = 1; break;
			case 2: posTile.x = 1; posTile.y = 1; break;
			case 3: posTile.x = -1; posTile.y = 0; break;
			case 4: posTile.x = 0; posTile.y = 0; break;
			case 5: posTile.x = 1; posTile.y = 0; break;
			case 6: posTile.x = -1; posTile.y = -1; break;
			case 7: posTile.x = 0; posTile.y = -1; break;
			case 8: posTile.x = 1; posTile.y = -1; break;	
		}
		*/
		
		vertices = new Vector3[(GameMgr.WIDTH + 1) * (GameMgr.WIDTH + 1)];
		planeColors = new Color[vertices.Length];

		ResetGround();
		
		int start = 0;
		int[] plane_traingles = new int[6 * GameMgr.WIDTH * GameMgr.WIDTH];
		for (num = 0; num < plane_traingles.Length; num++)
		{
			start = Mathf.FloorToInt(num / 6) + (Mathf.FloorToInt(Mathf.FloorToInt(num / 6) / GameMgr.WIDTH));
			if (num % 6 == 0) plane_traingles[num] = start;
			if (num % 6 == 1) plane_traingles[num] = start + GameMgr.WIDTH + 1;
			if (num % 6 == 2) plane_traingles[num] = start + GameMgr.WIDTH + 2;
			if (num % 6 == 3) plane_traingles[num] = start;
			if (num % 6 == 4) plane_traingles[num] = start + GameMgr.WIDTH + 2;
			if (num % 6 == 5) plane_traingles[num] = start + 1;
		}

		plane_uv = new Vector2[vertices.Length];
		Vector4[] tangents = new Vector4[vertices.Length];

		if(GameMgr.ins.mgrSave.blockTypes.Count < 1) GameMgr.ins.mgrSide.SetUV(ref plane_uv, 0);
		else GameMgr.ins.mgrSide.SetUV(ref plane_uv, GameMgr.ins.mgrSave.blockTypes[SaveManager.aryIdxs[pos_idx]]);

		for (num = 0; num < vertices.Length; num++)
		{
			tangents[num].x = tangents[num].w = -1;
			tangents[num].y = tangents[num].z = 0;

			planeColors[num] = colBase;
		}
		InitGround(pos_idx, ref planeColors, ref plane_uv, ref tangents, ref plane_traingles);
		/*
		for (num = 0; num < 9; num++) 
		{
			InitGround(num, ref planeColors, ref plane_uv, ref tangents, ref plane_traingles);
		}
		*/
		//vertices = filter.mesh.vertices;
	}
	
	private void InitGround(int value, ref Color[] cols, ref Vector2[] plane_uv, ref Vector4[] tangents, ref int[] plane_traingles) 
	{
		//filter.mesh.Clear();
		filter.sharedMesh = new Mesh();
		filter.sharedMesh.name = GrassGround + value;

		filter.sharedMesh.vertices = vertices;
		filter.sharedMesh.uv = plane_uv;
		filter.sharedMesh.triangles = plane_traingles;
		filter.sharedMesh.tangents = tangents;

		filter.sharedMesh.RecalculateNormals();
		filter.sharedMesh.RecalculateBounds();

		filter.sharedMesh.colors = cols;
	}

	public void SetGround(int value) 
	{
		for (num = 0; num < vertices.Length; num++)
		{
			vertices[num].y = 0f;
			if (GameMgr.ins.mgrSave.dicGround[value].Count > num)
			{
				vertices[num].y = Mathf.RoundToInt(GameMgr.ins.mgrSave.dicGround[value][num] * 10f) * 0.1f;
				//vertices[num].y = Mathf.RoundToInt(vertices[num].y * 10f) * 0.1f;
			}
			SetGroundColor(num);
			checkSide(num);
		}
		GameMgr.ins.mgrSide.FinishSide();
		GameMgr.ins.mgrSide.SetUV(ref plane_uv, GameMgr.ins.mgrSave.blockTypes[SaveManager.aryIdxs[pos_idx]]);
		filter.sharedMesh.uv = plane_uv;

		GameMgr.ins.mgrField.SetMeshFilter(ref filter, vertices, ref col);
		SetGroundColor();
		
	}

	public void Reset(bool isSet = false) 
	{
		planeColors = new Color[vertices.Length];
		
	for (num = 0; num < vertices.Length; num++)
		{
			vertices[num].x = (GameMgr.WIDTH * 0.5f) - (num % (GameMgr.WIDTH + 1));
			vertices[num].z = (GameMgr.WIDTH * 0.5f) - Mathf.FloorToInt(num / (GameMgr.WIDTH + 1));
			vertices[num].y = 0;
			planeColors[num] = colBase;
			checkSide(num);
		}
		GameMgr.ins.mgrSide.FinishSide();

		if (isSet)
		{
			GameMgr.ins.mgrSide.SetUV(ref plane_uv, GameMgr.ins.mgrSave.blockTypes[SaveManager.aryIdxs[pos_idx]]);
			filter.sharedMesh.uv = plane_uv;
			GameMgr.ins.mgrField.SetMeshFilter(ref filter, vertices,ref col);
			vertices = filter.sharedMesh.vertices;
			filter.sharedMesh.colors = planeColors;
		}
	}

	public void ResetGround(bool isSet = false) 
	{
		Reset(isSet);

		if (isSet) GameMgr.ins.mgrSave.AddListBackAll();
		
		SetHideEdit();
	}

	public void SetHideEdit() 
	{
		selIdx = -1;
		//pnlFocus.SetActive(false);
		GameMgr.ins.mgrField.pnlFocus.SetActive(false);
		GameMgr.ins.mgrUI.select.ClickCancel();
	}

	public void UpdateEditor() 
	{
		if (curIdx == -1) return;

		vec3 = vertices[curIdx];
		GameMgr.ins.mgrField.select.UpdatePosition(ref vec3);
	}

	public void PressPlane(bool isPress)
	{
		ReturnPressPlane(isPress);
	}

	public bool ReturnPressPlane(bool isPress)
	{
		if (GameMgr.ins.mgrUI.PLAY_MODE == UIManager.MODE.MOVE)
		{ GameMgr.ins.mgrMoveCam.OnPressGround(null, isPress); return false; }

		GameMgr.ins.mgrCam.OnPress(null, isPress);
		
		return true;
	}

	private void OnPressPlane(bool isPress) 
	{
		if (ReturnPressPlane(isPress) == false) return;
		
		ClickPlane(isPress, pos_idx);
	}

	public void ShowMoveBlock()
	{
		SetCurrentPos();
		curIdx = -1;
		SetHideEdit();
		vec3 = hit.point;
		GameMgr.ins.mgrUI.select.movetile = pos_idx;
		GameMgr.ins.mgrUI.select.movetileedit = num;
		GameMgr.ins.mgrUI.select.Show(UISelect.TYPE.BLOCK_MOVE);
		GameMgr.ins.mgrField.select.UpdatePosition(ref vec3, false);
	}

	private void SetCurrentPos()
	{
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(ray, out hit);
		vec3 = hit.point;
		vec3.x -= (posTile.x * GameMgr.WIDTH) + posTile.x;
		vec3.z -= (posTile.y * GameMgr.WIDTH) + posTile.y;
		num = Mathf.FloorToInt(GameMgr.WIDTH - (vec3.x + (GameMgr.WIDTH * 0.5f))) + (Mathf.FloorToInt(GameMgr.WIDTH - (vec3.z + (GameMgr.WIDTH * 0.5f))) * (GameMgr.WIDTH + 1));
		//Debug.Log("hit.point:"+hit.point);
		//Debug.Log("posTile:" + posTile);
		//Debug.Log("tileIdx:" + TileIdx);
		//Debug.Log("result : "+num);
		//return;
		if (num < 0) num = 0;
		//Debug.Log(Mathf.FloorToInt(10f - (hit.point.z + 5f)) * 11);
		//Debug.Log(hit.collider.name + "::" + hit.point + ":" + num);
		curIdx = num;
	}

	private void ClickPlane(bool isPress, int idx) 
	{
		if (isPress)
		{
			pressPos = CameraControl.POS_MOUSE;
			return;
		}
		if (Vector2.Distance(pressPos, CameraControl.POS_MOUSE) > 10f) return;

		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit) == false) return;
		SetCurrentPos();
		
		if (idx != GameMgr.TILE_CENTER)
		{	//중앙 영역이 아닌 구역 선택
			if (isPress == false) ShowMoveBlock();
			return;
		}
		if (GameMgr.ins.mgrUI.build.isCreate)
		{	//건물 생성
			if (GameMgr.ins.mgrBuild.CreateBuild(curIdx))
				GameMgr.ins.mgrBuild.SelectBuildComplete();
			return;
		}
		if (GameMgr.ins.mgrField.typeField == FieldManager.FIELD_TYPE.MAKEROAD)
		{	//도로 생성
			selIdx = curIdx;
			if(CheckGroundHeight())	MakeRoad();
			SetHideEdit();
			GameMgr.ins.mgrField.pnlFocus.SetActive(true);
			PlaneEdit(curIdx, false, true);
			return;
		}
		if (selIdx == curIdx)
		{	//토글.. 이미 선택한 곳 다시 선택하면 취소
			SetHideEdit();
			return;
		}
		
		if (GameMgr.ins.mgrBuild.dataBuild != null 
			&& GameMgr.ins.mgrBuild.dataBuild.Length > curIdx 
			&& GameMgr.ins.mgrBuild.dataBuild[curIdx] != null
			&& GameMgr.ins.mgrBuild.dataBuild[curIdx].idx != 0)
		{   //건물이 있는 경우 건물 선택
			GameMgr.ins.mgrBuild.GetBuild(GameMgr.ins.mgrBuild.dataBuild[curIdx].listIdx).ClickBuild();
			return;
		}
		
		GameMgr.ins.mgrField.select.Show(UISelect.TYPE.DEFAULT);
		selIdx = curIdx;
		GameMgr.ins.mgrField.pnlFocus.SetActive(true);
		PlaneEdit(curIdx, false, true);
		
		GameMgr.ins.mgrField.EditorSet(pos_idx);
		UpdateEditor();
	}

	public void GroundUp() 
	{
		if (editUpdown == false) 
		{	// 4꼭지점이 높이가 서로 다를 경우.. 목표 높이를 설정
			
			if (GameMgr.ins.mgrBuild.CheckBuild0(curIdx)) dirPos = vertices[curIdx].y;
			else if (GameMgr.ins.mgrBuild.CheckBuild1(curIdx)) dirPos = vertices[curIdx + 1].y;
			else if (GameMgr.ins.mgrBuild.CheckBuild2(curIdx)) dirPos = vertices[curIdx + 1 + GameMgr.WIDTH].y;
			else if (GameMgr.ins.mgrBuild.CheckBuild3(curIdx)) dirPos = vertices[curIdx + 2 + GameMgr.WIDTH].y;

			if (GameMgr.ins.mgrBuild.CheckBuild0(curIdx) && vertices[curIdx].y > dirPos) dirPos = vertices[curIdx].y;
			if (GameMgr.ins.mgrBuild.CheckBuild1(curIdx) && vertices[curIdx + 1].y > dirPos) dirPos = vertices[curIdx + 1].y;
			if (GameMgr.ins.mgrBuild.CheckBuild2(curIdx) && vertices[curIdx + 1 + GameMgr.WIDTH].y > dirPos) dirPos = vertices[curIdx + 1 + GameMgr.WIDTH].y;
			if (GameMgr.ins.mgrBuild.CheckBuild3(curIdx) && vertices[curIdx + 2 + GameMgr.WIDTH].y > dirPos) dirPos = vertices[curIdx + 2 + GameMgr.WIDTH].y;

			/*
			Debug.Log(vertices[curIdx].y );
			Debug.Log(vertices[curIdx+1].y);
			Debug.Log(vertices[curIdx+1+GameMgr.WIDTH].y);
			Debug.Log(vertices[curIdx+2+GameMgr.WIDTH].y);
			Debug.Log(dirPos);
			*/
		}
		PlaneEdit(curIdx); 
		UpdateEditor();

		//EditorSet();
		GameMgr.ins.mgrField.EditorSet(pos_idx);
	}

	public void GroundDown() 
	{
		if (editUpdown == false)
		{
			if (GameMgr.ins.mgrBuild.CheckBuild0(curIdx)) dirPos = vertices[curIdx].y;
			else if (GameMgr.ins.mgrBuild.CheckBuild1(curIdx)) dirPos = vertices[curIdx + 1].y;
			else if (GameMgr.ins.mgrBuild.CheckBuild2(curIdx)) dirPos = vertices[curIdx + 1 + GameMgr.WIDTH].y;
			else if (GameMgr.ins.mgrBuild.CheckBuild3(curIdx)) dirPos = vertices[curIdx + 2 + GameMgr.WIDTH].y;

			if (GameMgr.ins.mgrBuild.CheckBuild0(curIdx) && vertices[curIdx].y < dirPos) dirPos = vertices[curIdx].y;
			if (GameMgr.ins.mgrBuild.CheckBuild1(curIdx) && vertices[curIdx + 1].y < dirPos) dirPos = vertices[curIdx + 1].y;
			if (GameMgr.ins.mgrBuild.CheckBuild2(curIdx) && vertices[curIdx + 1 + GameMgr.WIDTH].y < dirPos) dirPos = vertices[curIdx + 1 + GameMgr.WIDTH].y;
			if (GameMgr.ins.mgrBuild.CheckBuild3(curIdx) && vertices[curIdx + 2 + GameMgr.WIDTH].y < dirPos) dirPos = vertices[curIdx + 2 + GameMgr.WIDTH].y;
		}
		
		PlaneEdit(curIdx, false); 
		UpdateEditor();

		//EditorSet();
		GameMgr.ins.mgrField.EditorSet(pos_idx);
	}
	/// <summary>
	/// 땅 끝 내용은 점 높이 값을 옆쪽 땅도 같도록
	/// </summary>
	public bool checkSide(int idx, bool vCheck = false)
	{
		if (   idx % (GameMgr.WIDTH + 1) == 0 
			|| Mathf.FloorToInt(idx / (GameMgr.WIDTH + 1)) == 0
			|| idx % (GameMgr.WIDTH + 1) == GameMgr.WIDTH
			|| Mathf.FloorToInt(idx / (GameMgr.WIDTH + 1)) == GameMgr.WIDTH)
		{
			check = false;
			SetGroundColor(idx);
			GameMgr.ins.mgrSide.CheckSide(pos_idx, idx, vertices[idx].y, ref planeColors[idx], vCheck);
		}
		return false;
	}

	private void PlaneVertextMove(int idx, bool isUp = true) 
	{
		vec3 = vertices[idx];

		//if (checkSide(idx)) return;

		//Debug.Log(idx % (GameMgr.WIDTH + 1));

		if (isUp) vec3.y += UP_DEGREE;
		else vec3.y -= UP_DEGREE;

		vec3.y = Mathf.RoundToInt(vec3.y*10f)*0.1f;
		
		vertices[idx] = vec3;
		
		checkSide(idx);
		GameMgr.ins.mgrSide.FinishSide();
	}

	private void PlanUpSelect(int idx, int hit_idx = -1, bool isUp = true, bool isSetSelect = false) 
	{
		if (isSetSelect == false && SetVertectUpDown(idx)) PlaneVertextMove(idx, isUp);

		if (hit_idx != -1)
		{
			GameMgr.ins.mgrField.hit_vertices[hit_idx] = vertices[idx];
		}
	}

	private bool SetVertectUpDown(int idx)
	{
		if (editUpdown == false && dirPos == vertices[idx].y) return false;
		return true;
	}

	public void PlaneEdit(int idx, bool isUp = true, bool isSetSelect = false) 
	{
		if (!isSetSelect)  GameMgr.ins.mgrSave.AddListBackAll();

		if (isSetSelect || GameMgr.ins.mgrBuild.CheckBuild0(idx)) PlanUpSelect(idx, 0, isUp, isSetSelect); //위, 오른쪽
		if (isSetSelect || GameMgr.ins.mgrBuild.CheckBuild1(idx)) PlanUpSelect(idx + 1, 2, isUp, isSetSelect); //위, 왼쪽
		if (isSetSelect || GameMgr.ins.mgrBuild.CheckBuild2(idx)) PlanUpSelect(idx + 1 + GameMgr.WIDTH, 1, isUp, isSetSelect); //아래, 오른쪽
		if (isSetSelect || GameMgr.ins.mgrBuild.CheckBuild3(idx)) PlanUpSelect(idx + 2 + GameMgr.WIDTH, 3, isUp, isSetSelect); //아래, 왼쪽
		
		GameMgr.ins.mgrField.SetMeshFilter();
		
		if (isSetSelect) return;

		GameMgr.ins.mgrField.SetMeshFilter(ref filter, vertices,ref col);
		GameMgr.ins.mgrRoad.RoadRefresh(curIdx,true);
		SetGroundColor();
	}

	public void UpdateGroundPlane() 
	{
		GameMgr.ins.mgrField.SetMeshFilter(ref filter, vertices, ref col);
		SetGroundColor();
	}

	public void SetGroundColor(bool isSide = false) 
	{
		//Debug.Log("*"+GameMgr.WIDTH);
		//Debug.Log("**"+GameMgr.ins.mgrRoad.roadsData.Length);
		//Debug.Log(planeColors.Length);
		for (num = 0; num < planeColors.Length; num++) 
		{
			SetGroundColor(num);
			if (isSide) checkSide(num);
		}
		if(isSide) GameMgr.ins.mgrSide.FinishSide();
		filter.sharedMesh.colors = planeColors;

		SetGroundUV();
	}

	public void SetSideGroundColor(bool vCheck = false) 
	{
		for (num = 0; num < planeColors.Length; num++)
		{	checkSide(num, vCheck);	}
		GameMgr.ins.mgrSide.FinishSide(vCheck);
	}

	public void SetGroundColor(int value) 
	{
		planeColors[value] = colBase;// Color.white;

		isCheck = false;
		num2 = value;
		SetGroundRoadColor(ref isCheck, ref value);
		num2 = value - 1;
		SetGroundRoadColor(ref isCheck, ref value);
		num2 = value - GameMgr.WIDTH - 1;
		SetGroundRoadColor(ref isCheck, ref value);
		num2 = value - GameMgr.WIDTH - 2;
		SetGroundRoadColor(ref isCheck, ref value);

		isCheck = false;
		num2 = value;
		SetBuildRoadHeightColor(ref isCheck);
		num2 = value - 1;
		SetBuildRoadHeightColor(ref isCheck);
		num2 = value - GameMgr.WIDTH - 1;
		SetBuildRoadHeightColor(ref isCheck);
		num2 = value - GameMgr.WIDTH - 2;
		SetBuildRoadHeightColor(ref isCheck);

		if (vertices[value].y < GameMgr.ins.mgrField.GetWaterY() + 0.2f)
		{   //물 아래 땅..
			planeColors[value].r -= 0.3f;//0.6f;
			planeColors[value].g -= 0.3f;//0.6f;
			planeColors[value].b -= 0.2f;//0.5f;
		}

		if (vertices[value].y > 2f)
		{	//일정 높이 이상
			planeColors[value].r += 0.42f;//0.8f;
			planeColors[value].g += 0.42f;//0.8f;
			planeColors[value].b += 0.42f;//0.8f;
		}
	}

	private void SetGroundRoadColor(ref bool value, ref int valueidx) 
	{
		if (value == true) return;

		if (num2 < 0 || num2 >= getRoad().roadsData.Length)
		{
			value = false;
			return;
		}
		if ( getRoad().roadsData[num2] != 0)
		{
			planeColors[valueidx] += GameMgr.ins.mgrField.colRoad;
			value = true;
			return;
		}
		value = false;
		return;
	}
	private void SetBuildRoadHeightColor(ref bool value) 
	{
		if (value == true) return;
		
		if (num2 < 0 
			|| getBuild().dataBuild == null 
			|| num2 >= getBuild().dataBuild.Length 
			|| getBuild().dataBuild[num2] == null
			|| (GameMgr.ins.mgrBuildList.listPrefab[getBuild().dataBuild[num2].idx].IsShadow == false
				&& GameMgr.ins.mgrBuildList.listPrefab[getBuild().dataBuild[num2].idx].IsShadowLight == false)
			)
		{
			value = false;
			return;
		}

		if (getBuild().dataBuild[num2].idx != 0 
			&& GameMgr.ins.mgrBuildList.listPrefab[getBuild().dataBuild[num2].idx].IsShadow == true)
		{
			planeColors[num].r -= 0.1f;
			planeColors[num].g -= 0.2f;
			planeColors[num].b -= 0.2f;
			value =  true;
			return;
		}

		if (getBuild().dataBuild[num2].idx != 0
			&& GameMgr.ins.mgrBuildList.listPrefab[getBuild().dataBuild[num2].idx].IsShadowLight == true)
		{
			planeColors[num].r -= 0.05f;
			planeColors[num].g -= 0.1f;
			planeColors[num].b -= 0.1f;
			value = true;
			return;
		}


		value = false;
	}

	/// <summary> 
	/// 도로 만들기 
	/// </summary>
	public void MakeRoad() 
	{
		if (curIdx == -1) return;
		GameMgr.ins.mgrSave.AddListBackAll();
		GameMgr.ins.mgrRoad.SetRoadData(curIdx);
		//EditorSet();
		GameMgr.ins.mgrField.EditorSet(pos_idx);
		SetGroundColor(true);
	}

	/// <summary> 해당 위치 땅 높이 확인, 일정 높이 이하이면 도로 제거  </summary>
	public bool CheckGroundHeight()
	{
		num = GameMgr.WIDTH+ 1;
		check = true;
		//첫번째 줄이 아니면 윗부분 확인 
		if (Mathf.FloorToInt(curIdx / num) != 0)
		{ 
			CheckGroundHeight(curIdx - num);
			if (curIdx % num != 0) CheckGroundHeight(curIdx - num -1);
			if (curIdx % num != num - 2) CheckGroundHeight(curIdx - num + 1);
		}
		if (Mathf.FloorToInt(curIdx / num) != num - 2)
		{
			CheckGroundHeight(curIdx + num);
			if (curIdx % num != 0) CheckGroundHeight(curIdx + num - 1);
			if (curIdx % num != num - 2) CheckGroundHeight(curIdx + num + 1);
		}
		if (curIdx % num != 0) CheckGroundHeight(curIdx - 1);
		if (curIdx % num != num - 2) CheckGroundHeight(curIdx + 1);
		
		check2 = CheckGroundHeight(curIdx);

		if (check == false) 
		{	//도로에 변화가 생겼을 경우
			//listBack.RemoveAt(listBack.Count - 1);
			//AddListBackGroundAndRoad();
		}

		return check2;
	}

	private bool CheckGroundHeight(int idx)
	{
		//Debug.Log("CheckGroundHeight.idx:"+idx );
		//if (idx < 0 || idx > vertices.Length - 1) return true;
		//Debug.Log(vertices[idx]);
		//Debug.Log(curIdx);

		if (vertices[idx].y < GameMgr.ins.mgrField.GetWaterY()
			|| vertices[idx + 1].y < GameMgr.ins.mgrField.GetWaterY()
			|| vertices[idx + 1 + GameMgr.WIDTH].y < GameMgr.ins.mgrField.GetWaterY()
			|| vertices[idx + 2 + GameMgr.WIDTH].y < GameMgr.ins.mgrField.GetWaterY() )
		{
			if (GameMgr.ins.mgrRoad.roads[idx] != null &&
				GameMgr.ins.mgrRoad.roads[idx].gameObject.activeSelf)
			{
				GameMgr.ins.mgrRoad.SetRoadData(idx);
				check = false;
			}
			
			return false;
		}
		
		return true;
	}
}
