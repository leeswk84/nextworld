using UnityEngine;
using System.Collections;

/// <summary>
/// 도로..
/// </summary>
public class Road : MonoBehaviour 
{
	/// <summary> 도로 종류 (추후에는 여러 종류 길로 업데이트) </summary>
	//private int roadType = 1;
	private int idx = -1;
	
	[HideInInspector]
	public int matIdx = -1;
	[HideInInspector]
	public Vector3[] vertices;
	
	public MeshFilter filter;

	[HideInInspector]
	public string strMat;

	private const string MESH_NAME = "SpriteMesh";
	private const string STR_ROAD = "road";
	private const string BASE_MAT = "-";

	public bool Init(int index)
	{
		if (idx != -1) return false;
		strMat = BASE_MAT;
		transform.localPosition = Vector3.zero;

		idx = index;
		gameObject.name = STR_ROAD + idx;
		
		vertices = GameMgr.ins.mgrRoad.road_vertices;
		filter.sharedMesh = new Mesh();

		filter.sharedMesh.name = MESH_NAME;
		filter.sharedMesh.vertices = vertices;
		filter.sharedMesh.uv = GameMgr.ins.mgrRoad.road_uv;
		filter.sharedMesh.triangles = GameMgr.ins.mgrRoad.road_traingles;
		filter.sharedMesh.tangents = GameMgr.ins.mgrRoad.road_tangents;

		filter.sharedMesh.RecalculateNormals();
		filter.sharedMesh.RecalculateBounds();

		vertices = filter.sharedMesh.vertices;
		//Refresh();
		return true;
	}

	/// <summary>
	/// 화면에 보여지기
	/// </summary>
	public void View() 
	{
		gameObject.SetActive(true);
		//Refresh();
		//Debug.Log(gameObject.name + "View");
	}
	/// <summary>
	/// 화면에서 가려지기
	/// </summary>
	public void Hide() 
	{
		gameObject.SetActive(false);
		//Debug.Log(gameObject.name + "Hide");
	}
	/*
	public void Refresh() 
	{
		GameMgr.ins.roadMgr.RoadRefresh(idx, true, true);
	}
	*/
}
