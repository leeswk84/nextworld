using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSide : MonoBehaviour
{
	public enum TYPE
	{
		NONE,
		GROUND_W,
		GROUND_H,
		GROUND_EDIGE,
		GROUND_OUT_W,
		GROUND_OUT_H,
		GROUND_OUT_EDIGE,
	}

	public GameObject obj;
	public Transform tran;
	public MeshFilter filter;
	public ButtonObject btn;
	public RoadSideManager road;

	private MeshCollider col;

	[HideInInspector]
	public Vector3[] vertices;
	[HideInInspector]
	public Color[] planeColors;
	[HideInInspector]
	public Color[] planeOrgColors;

	[HideInInspector]
	public int pos_idx;
	private TYPE m_type;

	[HideInInspector]
	public int block1;
	[HideInInspector]
	public int block2, block3, block4;
	
	[HideInInspector]
	public bool isSet = false;
	

	public void Init(TYPE valuetype, int maxcount, int valueidx)
	{
		m_type = valuetype;
		pos_idx = valueidx;

		transform.parent = GameMgr.ins.mgrSide.prefabSide.transform.parent;
		gameObject.name = m_type.ToString() + pos_idx.ToString();

		vertices = new Vector3[maxcount];
		planeColors = new Color[maxcount];
		planeOrgColors = new Color[maxcount];
		isSet = false;

		if (valuetype != TYPE.GROUND_EDIGE)
		{
			road.side = this;
			road.objRoad = GameMgr.ins.mgrRoad.objRoad;
			road.roadsMat = GameMgr.ins.mgrRoad.roadsMat;
		}
		road.SIDE_TYPE = valuetype;
		switch (valuetype)
		{
			case TYPE.GROUND_W: road.Init(GameMgr.WIDTH, GameMgr.WIDTH); break;
			case TYPE.GROUND_H: road.Init(GameMgr.WIDTH, 1); break;
		}
		
	}

	private void InitVertices()
	{
		//filter.mesh.Clear();
		filter.sharedMesh = new Mesh();
		filter.sharedMesh.name = m_type.ToString() + pos_idx.ToString();

		filter.sharedMesh.vertices = vertices;
	}

	public void SetVertices(ref int[] value_traingles, ref Vector4[] value_tangents)
	{
		InitVertices();

		filter.sharedMesh.triangles = value_traingles;
		filter.sharedMesh.tangents = value_tangents;

		col = gameObject.AddComponent<MeshCollider>();
		FinishVertices();

		btn.fncPress = GameMgr.ins.mgrBlock.prefabBlock.mgrGround.PressPlane;
	}

	public void FinishVertices()
	{
		filter.sharedMesh.vertices = vertices;

		filter.sharedMesh.RecalculateNormals();
		filter.sharedMesh.RecalculateBounds();

		col.sharedMesh = filter.sharedMesh;
		filter.sharedMesh.colors = planeColors;
	}

	public void SetVertices(int dir, int valueidx, float valuey, ref Color col, ref bool vCheck)
	{
		if (vCheck == true && isSet == true) return;
		
		switch (dir)
		{
			case 0: valueidx = valueidx + GameMgr.WIDTH + 1; break; //상단
			case 1: valueidx = GameMgr.WIDTH - (GameMgr.ins.mgrSide.GROUND_VERTICS_COUNT - valueidx) + 1; break; //하단
			case 2: valueidx = (Mathf.FloorToInt(valueidx / (GameMgr.WIDTH + 1)) * 2); break; //좌측
			case 3: valueidx = (Mathf.FloorToInt(valueidx / (GameMgr.WIDTH + 1)) * 2) + 1; break; //우측
		}

		if (valueidx < 0) valueidx = 0;
		if (valueidx > vertices.Length - 1) valueidx = vertices.Length - 1;

		SetVertices(valueidx, valuey, ref col, ref vCheck);
		
	}

	public void SetOutVartices(int valueidx)
	{
		if ((m_type == TYPE.GROUND_OUT_W && ((pos_idx < 3 && valueidx < GameMgr.WIDTH + 1) || (pos_idx > 2 && valueidx > GameMgr.WIDTH))) || //가로
			(m_type == TYPE.GROUND_OUT_H && ((pos_idx % 2 == 0 && valueidx % 2 == 1) || (pos_idx % 2 == 1 && valueidx % 2 == 0))) || //세로
			(m_type == TYPE.GROUND_OUT_EDIGE &&
			((pos_idx < 4 && valueidx < 2) || (pos_idx > 7 && valueidx > 1) || //모서리 위 아래
			 ((pos_idx == 0 || pos_idx == 4 || pos_idx == 6 || pos_idx == 8) && valueidx % 2 == 1) || //모서리 좌
			   (pos_idx == 3 || pos_idx == 5 || pos_idx == 7 || pos_idx == 11) && valueidx % 2 == 0)) //모서리 우
			)
		{
			planeColors[valueidx] = GameMgr.ins.mgrField.colOutSkybox;
		}
		else planeColors[valueidx] = GameMgr.ins.mgrField.colOutside;
	}

	private bool CheckEdige(int valueidx)
	{
		return (m_type == TYPE.GROUND_EDIGE && pos_idx % (GameMgr.TILE_WIDTH + 1) == GameMgr.TILE_WIDTH && valueidx % 2 == 0) //모서리 우끝
			|| (m_type == TYPE.GROUND_EDIGE && pos_idx % (GameMgr.TILE_WIDTH + 1) == 0 && valueidx % 2 == 1) //모서리 좌끝
			|| (m_type == TYPE.GROUND_EDIGE && Mathf.FloorToInt(pos_idx / (GameMgr.TILE_WIDTH + 1)) == 0 && valueidx < 2) //모서리 상끝
			|| (m_type == TYPE.GROUND_EDIGE && Mathf.FloorToInt(pos_idx / (GameMgr.TILE_WIDTH + 1)) == GameMgr.TILE_WIDTH && valueidx > 1) //모서리 하끝
			|| (m_type == TYPE.GROUND_H && pos_idx % (GameMgr.TILE_WIDTH + 1) == 0 && valueidx % 2 == 1) //세로 좌끝
			|| (m_type == TYPE.GROUND_H && pos_idx % (GameMgr.TILE_WIDTH + 1) == GameMgr.TILE_WIDTH && valueidx % 2 == 0) //세로 우끝
			|| (m_type == TYPE.GROUND_W && Mathf.FloorToInt(pos_idx / GameMgr.TILE_WIDTH) == 0 && valueidx <= GameMgr.WIDTH) //가로 상끝
			|| (m_type == TYPE.GROUND_W && Mathf.FloorToInt(pos_idx / GameMgr.TILE_WIDTH) == GameMgr.TILE_WIDTH && valueidx > GameMgr.WIDTH); //가로 하끝
	}
	private bool CheckCenterLine()
	{
		if (GameMgr.ins.mgrUI.PLAY_MODE == UIManager.MODE.MOVE) return false;

		return (m_type == TYPE.GROUND_EDIGE && (pos_idx == 14 || pos_idx == 15 || pos_idx == 20 || pos_idx == 21))
			|| (m_type == TYPE.GROUND_H && (pos_idx == 14 || pos_idx == 15))//(idx == 2 || idx == 3))
			|| (m_type == TYPE.GROUND_W && (pos_idx == 12 || pos_idx == 17));//(idx == 1 || idx == 4)))
	}
	public void SetVertices(int valueidx, float valuey, ref Color col, ref bool vCheck)
	{
		if (vCheck == true && isSet == true) return;
		
		if (CheckEdige(valueidx))
		{
			vertices[valueidx].y = 0;
			planeOrgColors[valueidx] =
			planeColors[valueidx] = GameMgr.ins.mgrField.colOutSkybox;
			return;
		}

		vertices[valueidx].y = valuey;

		planeOrgColors[valueidx] = 
		planeColors[valueidx] = col;

		if (GameMgr.ins.mgrUI.PLAY_MODE == UIManager.MODE.MOVE) return;

		road.RefreshMeshRoad();

		if(CheckCenterLine())
		{
			planeColors[valueidx].g += 0.2f;
		}
		//FinishVertices();
	}
}