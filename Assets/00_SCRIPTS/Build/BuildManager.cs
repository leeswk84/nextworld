using UnityEngine;
using System.Collections;

public class BuildData
{
	public int idx = 0;
	public float rot = 0;
	public int listIdx;
	public int move_planet = -1;
	public int move_tile = -1;
	public int move_pos = -1;
	public float move_y = 0;
	public float scale = 1;
	public int index_npc = 0;

	public void Copy(BuildData value)
	{
		value.idx = idx;
		value.rot = rot;
		value.listIdx = listIdx;
		value.move_planet = move_planet;
		value.move_tile = move_tile;
		value.move_pos = move_pos;
		value.move_y = move_y;
		value.scale = scale;
		value.index_npc = index_npc;
	}
}

public class BuildManager : BaseBlockManager 
{
	//public Build build1, build2;

	public Vector2[] baseUV;
	private int num, num2;
	private string str;

	public BuildData[] dataBuild;

	public Transform pnlBuild;
	public Build prefabBuild;
	public Material matSelect;

	private System.Collections.Generic.List<Build> listBuild;
	
	private Build build;

	private int createPos = -1;
	private int createIdx = -1;
	private Vector3 vec3;

	private int backMove = -1;
	
	private int viewBuildIndex = -1;
	//private int checkWidth;
	
	public void SetViewBuildIndex(int value) { viewBuildIndex = value; }
	public int GetBackMove() { return backMove; }

	public void Init (int countMax) 
	{
		//baseUV = prefabBuild.filter.mesh.uv;
		listBuild = new System.Collections.Generic.List<Build>();
		dataBuild = new BuildData[countMax];
		createIdx = -1;
		//checkWidth = GameMgr.WIDTH + 1;
	}

	public void Reset() 
	{
		for (num = 0; num < dataBuild.Length; num++)
		{
			dataBuild[num] = new BuildData();
		}
		for (num = 0; num < listBuild.Count; num++) 
		{	listBuild[num].obj.SetActive(false);	}
	}

	public void SetBuild(int value ) 
	{
		Reset();
		for (num = 0; num < GameMgr.ins.mgrSave.dicBuild[value].Count; num++)
		{
			if (dataBuild.Length <= num)
			{
				//dataBuild[num] = 0;
				continue;
			}
			GameMgr.ins.mgrSave.dicBuild[value][num].Copy( dataBuild[num] );
		}
		SetBuildData();
	}

	private void SetBuildData() 
	{
		for (num2 = 0; num2 < dataBuild.Length; num2++)
		{
			if (dataBuild[num2].idx != 0)
			{
				viewBuildIndex = dataBuild[num2].idx;
				CreateBuild(num2);
				CreateComplete();
			}
		}
	}
	//BUILD SAME Y
	/// <summary> 위, 오른쪽 </summary>
	public bool CheckBuild0(int value)
	{
		return true;
		/*
		if (dataBuild == null) return true;

		//Debug.Log("value:" + value);
		//Debug.Log("dataBuild.length:"+dataBuild.Length);

		//첫번째 윗부분이 아니라면 확인
		if (Mathf.FloorToInt(value / checkWidth) != 0
			&& dataBuild[value - 1 - GameMgr.WIDTH].idx != 0) {	return false;	}
		//첫번째 오른쪽 부분이 아니라면 확인
		if (value % checkWidth != 0
			&& dataBuild[value - 1].idx != 0) { return false; }
		//첫번째 윗부분이 아니라면 확인
		//첫번째 오른쪽 부분이 아니라면 확인
		if (Mathf.FloorToInt(value / checkWidth) != 0
			&& value % checkWidth != 0
			&& dataBuild[value - 2 - GameMgr.WIDTH].idx != 0) { return false; }
		return true; 
		*/
	}
	/// <summary> 위, 왼쪽 </summary>
	public bool CheckBuild1(int value)
	{
		return true;
		/*
		if (dataBuild == null) return true;

		//첫번째 윗부분이 아니라면 확인
		if (Mathf.FloorToInt(value / checkWidth) != 0
			&& dataBuild[value - 1 -GameMgr.WIDTH].idx != 0) { return false; }
		//첫번째 왼쪽 부분이 아니라면 확인
		if (value % checkWidth != checkWidth - 2
			&& dataBuild[value + 1].idx != 0) { return false; }
		//첫번째 윗부분이 아니라면 확인
		//첫번째 왼쪽 부분이 아니라면 확인
		if (Mathf.FloorToInt(value / checkWidth) != 0
			&& value % checkWidth != checkWidth - 2
			&& dataBuild[value - GameMgr.WIDTH].idx != 0) { return false; }
		return true;
		*/
	}
	/// <summary> 아래, 오른쪽 </summary>
	public bool CheckBuild2(int value)
	{
		return true;
		/*
		if (dataBuild == null) return true;

		//첫번째 아랫부분이 아니라면 확인
		if (Mathf.FloorToInt(value / checkWidth) != checkWidth - 2
			&& dataBuild[value + 1 + GameMgr.WIDTH].idx != 0) { return false; }
		//첫번째 오른쪽 부분이 아니라면 확인
		if (value % checkWidth != 0
			&& dataBuild[value - 1].idx != 0) { return false; }
		//첫번째 아랫부분이 아니라면 확인
		//첫번째 오른쪽 부분이 아니라면 확인
		if (Mathf.FloorToInt(value / checkWidth) != checkWidth - 2
			&& value % checkWidth != 0
			&& dataBuild[value + GameMgr.WIDTH].idx != 0) { return false; }
		return true;
		*/
	}
	/// <summary> 아래, 오른쪽 </summary>
	public bool CheckBuild3(int value)
	{
		return true;
		/*
		if (dataBuild == null) return true;

		//첫번째 아랫부분이 아니라면 확인
		if (Mathf.FloorToInt(value / checkWidth) != checkWidth - 2
			&& dataBuild[value + 1 + GameMgr.WIDTH].idx != 0) { return false; }
		//첫번째 왼쪽 부분이 아니라면 확인
		if (value % checkWidth != checkWidth - 2
			&& dataBuild[value + 1].idx != 0) { return false; }
		//첫번째 아랫부분이 아니라면 확인
		//첫번째 왼쪽 부분이 아니라면 확인
		if (Mathf.FloorToInt(value / checkWidth) != checkWidth - 2
			&& value % checkWidth != checkWidth - 2
			&& dataBuild[value + 2 +GameMgr.WIDTH].idx != 0) { return false; }
		return true;
		*/
	}
	//Debug.Log(GameMgr.ins.mgrBuild.CheckBuild(idx+1)); //왼쪽
	//Debug.Log(GameMgr.ins.mgrBuild.CheckBuild(idx - 1)); //오른쪽
	//Debug.Log(GameMgr.ins.mgrBuild.CheckBuild(idx - 1 - ground_width)); //위
	//Debug.Log(GameMgr.ins.mgrBuild.CheckBuild(idx - ground_width)); //위, 왼쪽
	//Debug.Log(GameMgr.ins.mgrBuild.CheckBuild(idx - 2 - ground_width)); //위, 오른쪽
	//Debug.Log(GameMgr.ins.mgrBuild.CheckBuild(idx + 1 + ground_width)); //아래
	//Debug.Log(GameMgr.ins.mgrBuild.CheckBuild(idx + 2 + ground_width)); //아래,왼쪽
	//Debug.Log(GameMgr.ins.mgrBuild.CheckBuild(idx + ground_width)); //아래,오른쪽
	
	public void StartMoveBuild(int value, int valuePos) 
	{
		GameMgr.ins.mgrUI.build.isCreate = true;
		createIdx = value;
		build = listBuild[createIdx];
		GameMgr.ins.mgrBuild.SetViewBuildIndex(build.GetIdx());
		dataBuild[valuePos].idx = 0;
		backMove = valuePos;
	}

	/// <summary> 이동 취소 </summary>
	public void MoveCancelBuild() 
	{
		if (build == null) return;
		dataBuild[backMove].idx = build.GetIdx();
		dataBuild[backMove].listIdx = build.listIdx;
		vec3 = GameMgr.ins.mgrGround.vertices[backMove];
		build.SetPosIdx(backMove);
		vec3.x -= 0.5f; vec3.y += 0.5f; vec3.z -= 0.5f;
		build.tran.localPosition = vec3;
		build = null;
		backMove = -1;
	}

	public void MoveBuild(int valueIdx, int valuePos) 
	{
		vec3 = GameMgr.ins.mgrGround.vertices[valuePos];
		vec3.x -= 0.5f; vec3.y += 0.5f; vec3.z -= 0.5f;
		listBuild[valueIdx].tran.localPosition = vec3;
		listBuild[valueIdx].SetPosIdx(valuePos);
		dataBuild[valuePos].idx = listBuild[valueIdx].GetIdx();
		dataBuild[valuePos].listIdx = valueIdx;
	}

	public void SetUpdateBuild(int valuePos)
	{
		//Debug.Log("AAAA::" + valuePos);
		if (dataBuild[valuePos].idx <= 0) return;

		for (num = 0; num < listBuild.Count; num++) 
		{
			if (listBuild[num].pos == valuePos) 
			{
				build = listBuild[num];
				break;
			}
		}
		vec3 = GameMgr.ins.mgrGround.vertices[valuePos];
		vec3.x -= 0.5f; vec3.y += 0.5f; vec3.z -= 0.5f;
		build.tran.localPosition = vec3;
		build = null;
	}

	public bool CreateBuild(int value) 
	{
		createPos = value;
		vec3 = getGround().vertices[value];
		
		if (build != null) getGround().setCurIdx(build.pos);
		
		/*
		//BUILD SAME Y
		if (vec3.y != getGround().vertices[value+1].y
			|| vec3.y != getGround().vertices[value + 1 + GameMgr.WIDTH].y
			|| vec3.y != getGround().vertices[value + 2 + GameMgr.WIDTH].y)
			//|| vec3.y != GameMgr.ins.mgrGround.vertices[value + 1 + GameMgr.WIDTH].y
			//|| vec3.y != GameMgr.ins.mgrGround.vertices[value + 2 + GameMgr.WIDTH].y)
		{
			Debug.Log("NO GOOD GROUND");
			return false;
		}
		*/

		//if (GameMgr.ins.mgrRoad.roadsData[value] != 0) 
		if (getRoad().roadsData[value] != 0) 
		{
			Debug.Log("DON'T BUILD HERE ROAD");
			return false;
		}

		if (build == null)
		{
			createIdx = -1;
			//기존에 생성하려고 했다가 취소한 건물 중에서 같은 내용 찾기..
			for (num = 0; num < listBuild.Count; num++)
			{
				if (listBuild[num].obj.activeSelf == false && viewBuildIndex == listBuild[num].GetIdx())
				{
					createIdx = num;
					break;
				}
			}
			//기존에 생성하다가 취소 하거나.. 삭제한 건물중에 조건에 맞는 내용이 없다면 새로 생성
			//Debug.Log("CreateBuild :: " + value);
			if (createIdx == -1)
			{
				build = GameMgr.ins.mgrBuildList.GetBuild(viewBuildIndex);
				if (build == null) build = GameMgr.ins.mgrBuildList.GetBuild(1);
				listBuild.Add(build);
				build.name = "build" + listBuild.Count;
				createIdx = listBuild.Count - 1;
			}
		}
		build = listBuild[createIdx];
		build.Init(viewBuildIndex, SaveManager.aryIdxs[pos_idx], createIdx);
		dataBuild[createPos].listIdx = createIdx;
		
		build.tran.SetParent(pnlBuild);
		build.obj.SetActive(true);
		
		build.SetPosIdx(createPos);
		
		vec3.x -= 0.5f; vec3.y += 0.5f; vec3.z -= 0.5f;
		build.tran.localPosition = vec3;

		build.tranRot.Rotate(Vector3.up, dataBuild[createPos].rot);
		vec3 = Vector3.zero;
		vec3.y = dataBuild[createPos].move_y;
		build.tranRot.localPosition = vec3;
		
		vec3.x = vec3.y = vec3.z = dataBuild[createPos].scale;
		build.tranRot.localScale = vec3;
		
		if (build.npc != null && !PLAY_DATA.ins.dataNPC.dic.ContainsKey(dataBuild[createPos].index_npc)) Debug.LogWarning("NPC_DATA NOT " + dataBuild[createPos].index_npc);
		if (build.npc != null && PLAY_DATA.ins.dataNPC.dic.ContainsKey(dataBuild[createPos].index_npc))
			build.npc.data = PLAY_DATA.ins.dataNPC.dic[dataBuild[createPos].index_npc];

		return true;
	}

	public void SelectBuildComplete(int value = -1) 
	{
		if (value == -1) value = createPos;

		GameMgr.ins.mgrGround.setCurIdx(value);
		//GameMgr.ins.mgrGround.pnlFocus.SetActive(true);
		GameMgr.ins.mgrField.pnlFocus.SetActive(true);
		GameMgr.ins.mgrGround.PlaneEdit(value, false, true);
		GameMgr.ins.mgrGround.SetGroundColor(true);
		GameMgr.ins.mgrUI.select.Show(UISelect.TYPE.BUILD);
		GameMgr.ins.mgrGround.UpdateEditor();
	}

	public void BuildRotation(int valueIdx, float value)
	{
		//listBuild[valueIdx].tranRot.Rotate(Vector3.up, 15f);
		//dataBuild[listBuild[valueIdx].pos].rot += 15;
		//if (dataBuild[listBuild[valueIdx].pos].rot >= 360) dataBuild[listBuild[valueIdx].pos].rot = 0;
		//Debug.Log( Mathf.RoundToInt(listBuild[value].tran.localRotation.y * 180) );

		dataBuild[listBuild[valueIdx].pos].rot = value;
		vec3 = Vector3.zero;
		vec3.y = value;
		listBuild[valueIdx].tranRot.localEulerAngles = vec3;
	}

	public void BuildMoveY(int valueIdx, float value)
	{
		dataBuild[listBuild[valueIdx].pos].move_y = value;
		vec3 = Vector3.zero;
		vec3.y = value;
		listBuild[valueIdx].tranRot.localPosition = vec3;
	}

	public void BuildScale(int valueIdx, float value)
	{
		dataBuild[listBuild[valueIdx].pos].scale = value;
		vec3.x = vec3.y = vec3.z = value;
		listBuild[valueIdx].tranRot.localScale = vec3;
	}

	public void BuildNpcIndex(int valueIdx, int value)
	{
		dataBuild[listBuild[valueIdx].pos].index_npc = value;
		if(listBuild[valueIdx].buildType == Build.BTYPE.ItemBox) return;

		if (!PLAY_DATA.ins.dataNPC.dic.ContainsKey(value)) Debug.LogWarning("NPC_DATA NOT " + value);
		if (listBuild[valueIdx].npc != null && PLAY_DATA.ins.dataNPC.dic.ContainsKey(value))
			listBuild[valueIdx].npc.data = PLAY_DATA.ins.dataNPC.dic[value];
	}

	public void CreateCancel() 
	{
		if (build != null)
		{
			build.obj.SetActive(false);
			if (build.npc != null) build.npc.is_active = false;
		}
		build = null;
	}

	public void CreateComplete() 
	{
		dataBuild[createPos].idx = build.GetIdx();
		build.CompleteCreate();
		build = null;
	}
	/// <summary> 건물 생성, 이동, 교체 완료.. 수정 완료. </summary>
	public void EditBuildComplete() 
	{
		GameMgr.ins.mgrSave.AddListBackAll();
	}

	public Build GetFocusBuild() { return build; }

	/// <summary>
	/// Build Object 의 listIdx로 Build Object 반환
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public Build GetBuild(int value) 
	{
		if(value >= 0 && value < listBuild.Count) return listBuild[value];
		return null;
	}
	/// <summary>
	/// Build Object 의 listIdx로 BuildData 반환
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public BuildData GetBuildDataListIndex(int value)
	{
		if(value >= 0 
			&& listBuild.Count > value 
			&& listBuild[value] != null 
			&& dataBuild.Length > listBuild[value].pos) return dataBuild[listBuild[value].pos];
		return null;
	}
	/// <summary>
	/// Build Object의 BuildData 반환
	/// </summary>
	public static BuildData GetBuildData(Build value)
	{
		return GameMgr.ins.mgrBlock.GetBlockIdx(value.TileIdx).mgrBuild.GetBuildDataListIndex(value.listIdx);
	}

	/// <summary> 건물 제거 </summary>
	public void DeleteBuild(int value) 
	{
		if (value == -1) return;

		dataBuild[listBuild[value].pos].idx = 0;
		listBuild[value].obj.SetActive(false);
		//npc인 경우 제거
		if (listBuild[value].npc != null) listBuild[value].npc.is_active = false;
		
		GameMgr.ins.mgrSave.AddListBackAll();
	}

	/// <summary>
	/// 선택한 건물 투명도 설정
	/// </summary>
	/// <param name="isSelect"> 선택 여부 </param>
	public void SetFocusBuildMaterial(bool isSelect) 
	{
		if (build == null) return;
		if (isSelect)
		{
			matSelect.mainTexture = build.render.material.mainTexture;
			build.render.material = matSelect;
		}
		else 
		{
			build.render.material = prefabBuild.render.sharedMaterial;
			//build.render.material.mainTexture = matSelect.mainTexture;
		}
	}
}
