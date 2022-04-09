using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshAtlas : MonoBehaviour
{
	public ObjectPrefab parentPrefab;

	public Renderer render;
	public MeshFilter filter;
	//public Material mat;
	//public Mesh mesh;
	public int idx_map;

	public Color col;

	public string[] vertex_index;
	public Color[] vertex_col;

	[Header("")]
	public string PRINT_VERTEX;
	[Header("중복되는 vertex 위치 값")]
	public string VERTEX_INDEX;
	
	[HideInInspector]
	public bool showVertex;

	[HideInInspector]
	public List<VertexCube> listVertex;
	
	public void Start()
	{
		showVertex = true;
		//render.sharedMaterial = mat;
		//mesh = filter.sharedMesh;
		//filter.sharedMesh = mesh;
		//filter.mesh = mesh;
		UpdateMapping();
		//vertex color 설정
		UpdateColor();
	}

	public void UpdateColor()
	{
		int i, j, num;
		string[] strs;
		Dictionary<int, Color> vertex_dic = new Dictionary<int, Color>();
		for (i = 0; i < vertex_index.Length; i++)
		{
			if (i >= vertex_col.Length) break;

			strs = vertex_index[i].Split('-');
			if (strs.Length == 2)
			{
				int s, m;
				if (int.TryParse(strs[0], out s) == false) continue;
				if (int.TryParse(strs[1], out m) == false) continue;
				for (num = s; num <= m; num++)
				{
					if (vertex_dic.ContainsKey(num) == true) continue;
					vertex_dic.Add(num, vertex_col[i]);
				}
				continue;
			}

			strs = vertex_index[i].Split(',');
			for (j = 0; j < strs.Length; j++)
			{
				if (int.TryParse(strs[j], out num) == false) continue;
				if (vertex_dic.ContainsKey(num) == true) continue;
				vertex_dic.Add(num, vertex_col[i]);
			}
		}

		Color[] cols = new Color[filter.mesh.vertexCount];

		for (i = 0; i < cols.Length; i++)
		{
			if (vertex_dic.ContainsKey(i))
			{
				cols[i] = vertex_dic[i];
				continue;
			}
			cols[i] = col;
		}
		filter.mesh.colors = cols;
	}
	/// <summary>
	/// Mapping uv 값 설정
	/// </summary>
	public void UpdateMapping()
	{
		//mesh = filter.sharedMesh;
		//mesh = filter.mesh;
		//filter.sharedMesh = mesh;
		GameMgr.ins.mgrAtlas.SetTextureUV(ref filter, render.sharedMaterial.mainTexture.name, idx_map);

		//UnityEditor.PrefabUtility.
		//UnityEditor.PrefabUtility.ConnectGameObjectToPrefab(obj, prefab);
		//Debug.Log("AAA:" + UnityEditor.PrefabUtility.ReconnectToLastPrefab(obj));
	}
	/// <summary>
	/// Vertex 위치 index값을 알기 위한 gameobject 생성
	/// </summary>
	public void SetVertexPoint()
	{
#if UNITY_EDITOR
		if (showVertex == true)
		{
			listVertex = new List<VertexCube>();
			VertexCube prefabV = ((GameObject)Resources.Load("VertexCube")).GetComponent<VertexCube>();
			VertexCube cube;
			GameObject objVertex = new GameObject();
			objVertex.name = name + "_vertexs";
			objVertex.transform.parent = transform.parent;
			objVertex.transform.position = transform.position;
			objVertex.transform.rotation = transform.rotation;
			objVertex.transform.localScale = transform.localScale;
			Vector3[] vecs = filter.sharedMesh.vertices;
			for (int i = 0; i < vecs.Length; i++)
			{
				cube = GameObject.Instantiate(prefabV);
				cube.tran.parent = objVertex.transform;
				cube.atlas = this;
				cube.tran.localScale = new Vector3(0.05f,0.05f,0.05f);
				cube.tran.localPosition = vecs[i];
				cube.name = "vertex" + i;
				listVertex.Add(cube);
			}

			//UI로 표시
			if (CRT_TOOL.ins != null) CRT_TOOL.ins.uiVertex.ShowList(this);
		}
		else if(listVertex != null && listVertex.Count > 0 && listVertex[0] != null)
		{
			GameObject.DestroyImmediate(listVertex[0].tran.parent.gameObject);
			listVertex.Clear();
			if (CRT_TOOL.ins != null) CRT_TOOL.ins.uiVertex.HideList();
			

		}
		showVertex = !showVertex;
#endif
	}
	/// <summary>
	/// Prefab에 해당 정보 저장
	/// </summary>
	public void UpdatePrefabValue()
	{
		if (parentPrefab == null)
		{
			Debug.LogWarning(gameObject.name + ".parentPrefab is NULL");
			return;
		}

		parentPrefab.prefab = null;
		
		int i;
		
		if (parentPrefab is Parts)
		{
			for (i = 0; i < GameMgr.ins.mgrParts.listParts.Count; i++)
			{
				if (parentPrefab.obj.name == GameMgr.ins.mgrParts.listParts[i].obj.name)
				{
					parentPrefab.prefab = GameMgr.ins.mgrParts.listParts[i].obj;
					break;
				}
			}
		}
		if (parentPrefab is Build)
		{
			for (i = 0; i < GameMgr.ins.mgrBuildList.listPrefab.Count; i++)
			{
				if (parentPrefab.obj.name == GameMgr.ins.mgrBuildList.listPrefab[i].obj.name)
				{
					parentPrefab.prefab = GameMgr.ins.mgrBuildList.listPrefab[i].obj;
					break;
				}
			}
		}

		if (parentPrefab is Effect)
		{
			for (i = 0; i < GameMgr.ins.mgrEffect.prefab.Length; i++)
			{
				if (parentPrefab.obj.name == GameMgr.ins.mgrEffect.prefab[i].obj.name)
				{
					parentPrefab.prefab = GameMgr.ins.mgrEffect.prefab[i].obj;
					break;
				}
			}
		}

		if (parentPrefab.prefab == null)
		{
			Debug.LogWarning(gameObject.name + ".parentPrefab.prefab is NULL");
			return;
		}
		
		MeshAtlas[] atlas;
		atlas = parentPrefab.prefab.GetComponentsInChildren<MeshAtlas>();
		for (i = 0; i < atlas.Length; i++)
		{
			if (atlas[i].gameObject.name == gameObject.name)
			{
				atlas[i].idx_map = idx_map;
				atlas[i].col = col;
				atlas[i].vertex_index = vertex_index;
				atlas[i].vertex_col = vertex_col;

				atlas[i].transform.localPosition = transform.localPosition;
				atlas[i].transform.localRotation = transform.localRotation;
				atlas[i].transform.localScale = transform.localScale;
				break;
			}
		}
		//
		/*
		Debug.Log(UnityEditor.PrefabUtility.IsPartOfPrefabAsset(parentPrefab));
		Debug.Log(UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this));
		Debug.Log("Assets/03_PREFABS/Parts/" + parentPrefab.obj.name + ".prefab");
		UnityEditor.PrefabUtility.ApplyObjectOverride(parentPrefab, "Assets/03_PREFABS/Parts/" + parentPrefab.obj.name + ".prefab", UnityEditor.InteractionMode.AutomatedAction);
		*/
		//프리팹을 적용하면 mesh가 빠짐
		//UnityEditor.PrefabUtility.SaveAsPrefabAssetAndConnect(parentPrefab.obj, "Assets/03_PREFABS/Parts/" + parentPrefab.obj.name + ".prefab", UnityEditor.InteractionMode.AutomatedAction);

		Debug.Log(gameObject.name + ".parentPrefab Update");
	}
}
