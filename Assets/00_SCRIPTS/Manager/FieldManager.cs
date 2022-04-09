using UnityEngine;
using System.Collections;

public class FieldManager : MonoBehaviour 
{
	public const string _MainTex = "_MainTex";

	public enum FIELD_TYPE
	{
		DEFAULT,
		MAKEROAD,
		ROADSTART,
	}

	[HideInInspector]
	public FIELD_TYPE typeField = FIELD_TYPE.DEFAULT;

	public Transform tranField;
	public UISelect select;
	public ButtonObject btnGroundType, btnGroundType0, btnGroundType1;

	public Transform tranWater, tranGroundTypePoint;
	public MeshFilter filterWater;
	public MeshRenderer renderWater;

	public MeshFilter backFilter;
	public GameObject pnlFocus;
	
	private MeshFilter hit_filter;
	[HideInInspector]
	public Vector3[] hit_vertices;

	private Vector2 vec2;
	private Vector3 vec3;
	private float fnum;
	private int num;

	private int ground_width;
	private int curIdx;

	private float water_height;
	
	//public Color colBackStart = new Color(1f, 214f / 255f, 100f / 255f);
	//public Color colBackEnd = new Color(51f / 255f, 89f / 255f, 28f / 255f);

	[HideInInspector]
	public Color colRoad;
	[HideInInspector]
	public Color colBase;
	[HideInInspector]
	public Color colOutside;
	[HideInInspector]
	public Color colSkybox;
	[HideInInspector]
	public Color colOutSkybox;

	public void Init() 
	{
		colRoad = new Color(-0.03f, -0.05f, -0.18f); //(0.15f, -0.02f, -0.18f);
		colBase = new Color(0.85f, 0.85f, 0.85f);
		colOutside = new Color(0.65f, 0.65f, 0.65f);
		colSkybox = new Color(36f / 255f, 66f / 255f, 16f / 255f);
		colOutSkybox = new Color(38f / 255f + 0.2f, 68f / 255f + 0.2f, 8f / 255f + 0.2f);

		water_height = tranWater.localPosition.y;

		ground_width = GameMgr.WIDTH;
		select = GameMgr.ins.mgrUI.select;

		btnGroundType.btn.onClick.AddListener(ClickGroundType);
		btnGroundType0.btn.onClick.AddListener(ClickGroundType0);
		btnGroundType1.btn.onClick.AddListener(ClickGroundType1);

		btnGroundType0.obj.SetActive(false);
		btnGroundType1.obj.SetActive(false);

		//선택 영역 표시 plane
		hit_filter = pnlFocus.GetComponent<MeshFilter>();
		//if (pnlFocus.GetComponent<MeshCollider>() != null) Component.Destroy(pnlFocus.GetComponent<MeshCollider>());
		
		hit_vertices = new Vector3[] { new Vector3(-0.5f, 0, -0.5f), new Vector3(-0.5f, 0, 0.5f), new Vector3(0.5f, 0, -0.5f), new Vector3(0.5f, 0, 0.5f) };
		int[] hit_traingles = new int[] { 0, 1, 3, 0, 3, 2 };
		Vector2[] hit_uv = new Vector2[] { new Vector2(-0.1f, -0.1f), new Vector2(-0.1f, 0.1f), new Vector2(0.1f, -0.1f), new Vector2(0.1f, 0.1f) };
		Vector4[] hit_tangents = new Vector4[] { new Vector4(-1f, 0, 0, -1f), new Vector4(-1f, 0, 0, -1f), new Vector4(-1f, 0, 0, -1f), new Vector4(-1f, 0, 0, -1f) };

		hit_filter.mesh = new Mesh();
		//hit_filter.mesh.name = "SpriteMesh";
		hit_filter.mesh.vertices = hit_vertices;
		hit_filter.mesh.uv = hit_uv;
		hit_filter.mesh.triangles = hit_traingles;
		hit_filter.mesh.tangents = hit_tangents;

		hit_filter.mesh.RecalculateNormals();
		hit_filter.mesh.RecalculateBounds();
		hit_vertices = hit_filter.mesh.vertices;

		SetBackGround();

	}

	public void SetMeshFilter() 
	{
		SetMeshFilter(ref hit_filter, hit_vertices);
	}

	public void SetMeshFilter(ref MeshFilter valueFilter, Vector3[] setVecties)
	{
		valueFilter.mesh.vertices.Initialize();
		valueFilter.mesh.vertices = setVecties;
		valueFilter.mesh.RecalculateNormals();
		valueFilter.mesh.RecalculateBounds();
	}

	public void SetMeshFilter(ref MeshFilter valueFilter, Vector3[] setVecties, ref MeshCollider col)
	{
		SetMeshFilter(ref valueFilter, setVecties);
		if (col != null)
		{
			col.sharedMesh = valueFilter.sharedMesh;
			
			//col.inflateMesh = true;
		}
	}

	public void UpdateWaterMove()
	{
		vec2 = GameMgr.ins.mgrField.renderWater.sharedMaterial.GetTextureOffset(_MainTex);
		vec2.x = Time.time * 0.1f % 1;
		vec2.y = Time.time * 0.05f % 1;
		GameMgr.ins.mgrField.renderWater.sharedMaterial.SetTextureOffset(_MainTex, vec2);

		//SetBackGround(); //색지정시에만 테스트용으로 사용..
	}

	private void SetBackGround()
	{
		SetBackRoundCol();
		//SetFilterColor(ref backFilter2, 3, 40f, false);
	}

	public void SetBackRoundCol(bool isCol = true) 
	{
		//SetFilterColor(ref backFilter, 5, 70f, isCol);
		//SetBackFilterColor(ref backFilter, 3, 50f, isCol);
		SetBackFilterColor(ref backFilter, 5, 50f, isCol);
	}

	private void SetBackFilterColor(ref MeshFilter valueMeshFilter, int inSize,  float outSize, bool isCol) 
	{
		//여분 넓이
		fnum = outSize;
		float back_ground_widith = ground_width * inSize;
		back_ground_widith += 6;

		Vector3[] back_vertices = new Vector3[16];
		back_vertices[0] = new Vector3(-fnum, 0, -fnum);
		back_vertices[3] = new Vector3(-fnum, 0, fnum);
		back_vertices[12] = new Vector3(fnum, 0, -fnum);
		back_vertices[15] = new Vector3(fnum, 0, fnum);

		back_vertices[1] = new Vector3(-fnum, 0, -fnum + (back_ground_widith * 0.5f));
		back_vertices[2] = new Vector3(-fnum, 0, fnum - (back_ground_widith * 0.5f));
		back_vertices[13] = new Vector3(fnum, 0, -fnum + (back_ground_widith * 0.5f));
		back_vertices[14] = new Vector3(fnum, 0, fnum - (back_ground_widith * 0.5f));

		back_vertices[4] = new Vector3(-fnum + (back_ground_widith * 0.5f), 0, -fnum);
		back_vertices[8] = new Vector3(fnum - (back_ground_widith * 0.5f), 0, -fnum);
		back_vertices[7] = new Vector3(-fnum + (back_ground_widith * 0.5f), 0, fnum);
		back_vertices[11] = new Vector3(fnum - (back_ground_widith * 0.5f), 0, fnum);

		back_vertices[5] = new Vector3(back_ground_widith * -0.5f, 0, back_ground_widith * -0.5f);
		back_vertices[6] = new Vector3(back_ground_widith * -0.5f, 0, back_ground_widith * 0.5f);
		back_vertices[9] = new Vector3(back_ground_widith * 0.5f, 0, back_ground_widith * -0.5f);
		back_vertices[10] = new Vector3(back_ground_widith * 0.5f, 0, back_ground_widith * 0.5f);

		//int[] back_traingles = new int[] { 0, 1, 3, 0, 3, 2 };
		int[] back_traingles =
			new int[] 
			{ 
				0, 1, 5,  
				0, 5, 4, 
				1, 2, 6,  
				1, 6, 5, 
				//2, 3, 7, //모서리  
				2, 7, 6, 
				4, 5, 9,  
				4, 9, 8, 
				6, 7, 11,  
				6, 11, 10, 
				8, 9, 13,  
				//8, 13, 12, //모서리
				9, 10, 14, 
				//9, 14, 15,
				9, 15, 13,
				15, 13, 10,
				10, 11, 15,  
				//10, 15, 14
			};

		Vector2[] back_uv = new Vector2[back_vertices.Length];
		Vector2 vec2 = Vector2.zero;

		Vector4[] back_tangents = new Vector4[back_vertices.Length];
		Vector4 vec4 = Vector4.zero;
		vec4.x = -1f; vec4.w = -1f;

		Color[] cols = new Color[back_vertices.Length];
		
		for (num = 0; num < back_vertices.Length; num++)
		{
			vec2.x = back_vertices[num].x * 0.7f;
			vec2.y = back_vertices[num].z * 0.7f;
			back_uv[num] = vec2;
			back_tangents[num] = vec4;
			cols[num] = GameMgr.ins.mgrField.colSkybox;
		}

		if (isCol)
		{
			cols[0] = 
			cols[1] = 
			cols[2] = 
			cols[3] = 
			cols[4] = 
			cols[7] = 

			cols[8] = 
			cols[11] = 
			cols[12] = 

			cols[13] = 
			cols[14] = 
			cols[15] = GameMgr.ins.mgrField.colSkybox;
		}
		valueMeshFilter.sharedMesh = new Mesh();
		valueMeshFilter.sharedMesh.name = "SpriteMesh";
		valueMeshFilter.sharedMesh.vertices = back_vertices;
		valueMeshFilter.sharedMesh.uv = back_uv;
		valueMeshFilter.sharedMesh.triangles = back_traingles;
		valueMeshFilter.sharedMesh.tangents = back_tangents;
		
		valueMeshFilter.sharedMesh.RecalculateNormals();
		valueMeshFilter.sharedMesh.RecalculateBounds();

		valueMeshFilter.sharedMesh.colors = cols;
	}
	

	public void EditorSet(int tileidx,  bool isBuild = false)
	{
		if (select.obj.activeSelf == false) return;

		curIdx = GameMgr.ins.mgrBlock.posBlocks[tileidx].mgrGround.curIdx; //GameMgr.ins.mgrGround.curIdx;
		/*
		if (GameMgr.ins.mgrBuild.CheckBuild0(curIdx) && !GameMgr.ins.mgrGround.checkSide(curIdx)) vec3 = GameMgr.ins.mgrGround.vertices[curIdx];
		else if (GameMgr.ins.mgrBuild.CheckBuild1(curIdx) && !GameMgr.ins.mgrGround.checkSide(curIdx + 1)) vec3 = GameMgr.ins.mgrGround.vertices[curIdx + 1];
		else if (GameMgr.ins.mgrBuild.CheckBuild2(curIdx) && !GameMgr.ins.mgrGround.checkSide(curIdx + 1 + ground_width)) vec3 = GameMgr.ins.mgrGround.vertices[curIdx + 1 + ground_width];
		else if (GameMgr.ins.mgrBuild.CheckBuild3(curIdx) && !GameMgr.ins.mgrGround.checkSide(curIdx + 2 + ground_width)) vec3 = GameMgr.ins.mgrGround.vertices[curIdx + 2 + ground_width];

		if (
			(GameMgr.ins.mgrBuild.CheckBuild0(curIdx) && !GameMgr.ins.mgrGround.checkSide(curIdx) && GameMgr.ins.mgrGround.vertices[curIdx].y != vec3.y)
			|| (GameMgr.ins.mgrBuild.CheckBuild1(curIdx) && !GameMgr.ins.mgrGround.checkSide(curIdx + 1) && GameMgr.ins.mgrGround.vertices[curIdx + 1].y != vec3.y)
			|| (GameMgr.ins.mgrBuild.CheckBuild2(curIdx) && !GameMgr.ins.mgrGround.checkSide(curIdx + 1 + ground_width) && GameMgr.ins.mgrGround.vertices[curIdx + 1 + ground_width].y != vec3.y)
			|| (GameMgr.ins.mgrBuild.CheckBuild3(curIdx) && !GameMgr.ins.mgrGround.checkSide(curIdx + 2 + ground_width) && GameMgr.ins.mgrGround.vertices[curIdx + 2 + ground_width].y != vec3.y))
		{
		*/
		
		if (GameMgr.ins.mgrBuild.CheckBuild0(curIdx)) vec3 = GameMgr.ins.mgrGround.vertices[curIdx];
		else if (GameMgr.ins.mgrBuild.CheckBuild1(curIdx)) vec3 = GameMgr.ins.mgrGround.vertices[curIdx + 1];
		else if (GameMgr.ins.mgrBuild.CheckBuild2(curIdx)) vec3 = GameMgr.ins.mgrGround.vertices[curIdx + 1 + ground_width];
		else if (GameMgr.ins.mgrBuild.CheckBuild3(curIdx)) vec3 = GameMgr.ins.mgrGround.vertices[curIdx + 2 + ground_width];

		if (
			(GameMgr.ins.mgrBuild.CheckBuild0(curIdx) && GameMgr.ins.mgrGround.vertices[curIdx].y != vec3.y)
			|| (GameMgr.ins.mgrBuild.CheckBuild1(curIdx) && GameMgr.ins.mgrGround.vertices[curIdx + 1].y != vec3.y)
			|| (GameMgr.ins.mgrBuild.CheckBuild2(curIdx) && GameMgr.ins.mgrGround.vertices[curIdx + 1 + ground_width].y != vec3.y)
			|| (GameMgr.ins.mgrBuild.CheckBuild3(curIdx) && GameMgr.ins.mgrGround.vertices[curIdx + 2 + ground_width].y != vec3.y))
		{
			GameMgr.ins.mgrGround.editUpdown = false;
			select.btnUp.txt.text = TxtMgr.ins.GetTxt(TxtMgr.TYPE.GROUND_UP_SAM);
			select.btnDown.txt.text = TxtMgr.ins.GetTxt(TxtMgr.TYPE.GROUND_DOWN_SAM);
		}
		else
		{
			GameMgr.ins.mgrGround.editUpdown = true;
			select.btnUp.txt.text = TxtMgr.ins.GetTxt(TxtMgr.TYPE.GROUND_UP);
			select.btnDown.txt.text = TxtMgr.ins.GetTxt(TxtMgr.TYPE.GROUND_DOWN);
		}

		if (isBuild)
		{
			select.btnMakeRoad.obj.SetActive(false);
			return;
		}
		if (GameMgr.ins.mgrGround.CheckGroundHeight())
		{	//물이 있는 땅에는 길 만들수 없도록
			select.btnMakeRoad.obj.SetActive(true);
			GameMgr.ins.mgrRoad.UpdateEditor(curIdx);
		}
		else
		{
			select.btnMakeRoad.obj.SetActive(false);
		}
	}

	private void ClickGroundType()
	{
		//vec3 = tranGroundTypePoint.localScale;
		vec3 = GameMgr.ins.mgrField.tranGroundTypePoint.localScale;
		vec3.y *= -1f;
		//tranGroundTypePoint.localScale = vec3;
		GameMgr.ins.mgrField.tranGroundTypePoint.localScale = vec3;

		btnGroundType0.obj.SetActive(vec3.y < 0);
		btnGroundType1.obj.SetActive(vec3.y < 0);

		GameMgr.ins.mgrGround.SetHideEdit();
	}

	private void ClickGroundType0()
	{
		typeField = FIELD_TYPE.DEFAULT;
		btnGroundType.txt.text = TxtMgr.ins.GetTxt(TxtMgr.TYPE.GROUND_TYPE_DEFAULT);
		ClickGroundType();
	}

	private void ClickGroundType1()
	{
		typeField = FIELD_TYPE.MAKEROAD;
		btnGroundType.txt.text = TxtMgr.ins.GetTxt(TxtMgr.TYPE.ROAD_MAKE);
		ClickGroundType();
	}

	public float GetWaterY() { return water_height; }

}