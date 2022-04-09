using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshAtlas))]
public class MeshAtlasEditor: Editor
{
	public override void OnInspectorGUI()
	{
		if (CRT_TOOL.ins == null)
		{
			DrawDefaultInspector();
			return; //플레이 중일때만 보이도록
		}

		MeshAtlas atlas = (MeshAtlas)target;

		if (GUILayout.Button("텍스쳐 이미지 적용", GUILayout.Height(30)))
		{
			atlas.UpdateMapping();
			atlas.UpdatePrefabValue();
		}

		DrawDefaultInspector();
		GUIStyle style = new GUIStyle();
		style.fontSize = 8;
		style.wordWrap = true;
		GUILayout.TextArea(atlas.PRINT_VERTEX, style);//, GUILayout.ExpandHeight(true));
		GUILayout.Space(2);
		GUILayout.TextArea(atlas.VERTEX_INDEX, style);

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("배열추가(Alt+q)", GUILayout.Height(20)))
		{
			AddArray();
			/*
			Color[] tmpCol = new Color[atlas.vertex_col.Length + 1];
			string[] tmpIdx = new string[tmpCol.Length];

			for(int i=0; i<tmpCol.Length -1; i++)
			{
				tmpCol[i] = atlas.vertex_col[i];
				tmpIdx[i] = atlas.vertex_index[i];
			}

			atlas.vertex_col = tmpCol;
			atlas.vertex_index = tmpIdx;
			atlas.UpdatePrefabValue();
			*/
		}

		if (GUILayout.Button("배열삭제(Alt+w)", GUILayout.Height(20)))
		{
			MinusArray();
			/*
			Color[] tmpCol = new Color[atlas.vertex_col.Length - 1];
			string[] tmpIdx = new string[tmpCol.Length];

			for (int i = 0; i < tmpCol.Length; i++)
			{
				tmpCol[i] = atlas.vertex_col[i];
				tmpIdx[i] = atlas.vertex_index[i];
			}

			atlas.vertex_col = tmpCol;
			atlas.vertex_index = tmpIdx;
			atlas.UpdatePrefabValue();
			*/
		}

		GUILayout.EndHorizontal();
		GUILayout.Space(10);
		if (GUILayout.Button(atlas.showVertex == true ? "Vertex 버튼 보임 (Alt+`)" : "Vertex 버튼 안보임 (Alt+`)", GUILayout.Height(30)))
		{
			atlas.SetVertexPoint();
		}
		GUILayout.Space(10);
		if (GUILayout.Button("색상 적용(Alt+s)", GUILayout.Height(30)))
		{
			//atlas.UpdateColor();
			//atlas.UpdatePrefabValue();
			SetColor();
		}
		
		if (GUILayout.Button("프리펩 적용(Alt+a)", GUILayout.Height(30)))
		{
			ApplyPrefab();
			//atlas.UpdatePrefabValue();
		}
		
		GUILayout.BeginHorizontal();
		CRT_TOOL.ins.uiVertex.SET_COLOR = (UIVertexPoint.INDEX_NUMBER)EditorGUILayout.EnumPopup("", CRT_TOOL.ins.uiVertex.SET_COLOR, GUILayout.Width(80), GUILayout.Height(20));
		if (GUILayout.Button("위치 내용 불러오기(Alt+e)", GUILayout.Height(30)))
		{
			LoadIndex();
			//if (CRT_TOOL.ins.uiVertex.curAtlas == null) atlas.SetVertexPoint();
			//CRT_TOOL.ins.uiVertex.ChangeCurrentColorVertex();
		}
		if (GUILayout.Button("위치 내용 적용(Alt+d)", GUILayout.Height(30)))
		{
			SetIndex();
			//if (CRT_TOOL.ins.uiVertex.curAtlas == null) atlas.SetVertexPoint();
			//CRT_TOOL.ins.uiVertex.SetVerIndexColor();
			//atlas.UpdateColor();
			//atlas.UpdatePrefabValue();
		}
		GUILayout.EndHorizontal();
		/*
		GUILayout.BeginHorizontal();
		CRT_TOOL.ins.uiVertex.CHANGE_COLOR = (UIVertexPoint.INDEX_NUMBER)EditorGUILayout.EnumPopup("", CRT_TOOL.ins.uiVertex.CHANGE_COLOR, GUILayout.Width(80), GUILayout.Height(20));
		if (GUILayout.Button("위치 내용 불러오기", GUILayout.Height(30)))
		{
			if (CRT_TOOL.ins.uiVertex.curAtlas == null) atlas.SetVertexPoint();
			CRT_TOOL.ins.uiVertex.ChangeCurrentColorVertex();
		}
		GUILayout.EndHorizontal();
		*/
		if (GUILayout.Button("입력 내용 지우기 (Alt+x)", GUILayout.Height(30)))
		{
			ClearPrint();
			//atlas.PRINT_VERTEX = "";
			//atlas.VERTEX_INDEX = "";
		}
	}

	
	[MenuItem("HotKey/VertexToggle _&`")]
	static public void VertexToggle()
	{
		if (CRT_TOOL.ins == null)
		{
			Debug.Log("CRT_TOOL 이 있고, 플레이 중에만 가능.");
			return;
		}
		if (Selection.activeGameObject == null)
		{
			Debug.Log("MeshAtlas 를 선택하지 않았음.");
			return;
		}
		MeshAtlas atlas = Selection.activeGameObject.GetComponent<MeshAtlas>();
		if (atlas == null)
		{
			Debug.Log("MeshAtlas 를 선택하지 않았음.");
			return;
		}
		atlas.SetVertexPoint();
	}

	[MenuItem("HotKey/ClearPrint _&x")]
	static public void ClearPrint()
	{
		if (CRT_TOOL.ins == null) { Debug.Log("CRT_TOOL 이 있고, 플레이 중에만 가능."); return; }
		MeshAtlas atlas = Selection.activeGameObject.GetComponent<MeshAtlas>();
		if(atlas == null) { Debug.Log("선택한 Atlas가 없습니다."); return; }
		atlas.PRINT_VERTEX = "";
		atlas.VERTEX_INDEX = "";
	}

	[MenuItem("HotKey/FocusCamera _&f")]
	static public void FocusCamera()
	{
		if (CRT_TOOL.ins == null) {	Debug.Log("CRT_TOOL 이 있고, 플레이 중에만 가능."); return; }
		//SceneView.lastActiveSceneView.FrameSelected();
		CRT_TOOL.ins.cam.transform.position = SceneView.lastActiveSceneView.camera.transform.position;
		CRT_TOOL.ins.cam.transform.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
	}
	[MenuItem("HotKey/FocusCameraSet _&_#f")]
	static public void FocusCameraSet()
	{
		if (CRT_TOOL.ins == null) { Debug.Log("CRT_TOOL 이 있고, 플레이 중에만 가능."); return; }
		CRT_TOOL.ins.IS_SET_CAMERA = !CRT_TOOL.ins.IS_SET_CAMERA;
	}
	
	[MenuItem("HotKey/SetColor _&s")]
	static public void SetColor()
	{
		if (CRT_TOOL.ins == null) { Debug.Log("CRT_TOOL 이 있고, 플레이 중에만 가능."); return; }
		MeshAtlas atlas = Selection.activeGameObject.GetComponent<MeshAtlas>();
		if (atlas == null) { Debug.Log("선택한 Atlas가 없습니다."); return; }

		atlas.UpdateColor();
		atlas.UpdatePrefabValue();
	}

	[MenuItem("HotKey/SetIndex _&d")]
	static public void SetIndex()
	{
		if (CRT_TOOL.ins == null) { Debug.Log("CRT_TOOL 이 있고, 플레이 중에만 가능."); return; }
		MeshAtlas atlas = Selection.activeGameObject.GetComponent<MeshAtlas>();
		if (atlas == null) { Debug.Log("선택한 Atlas가 없습니다."); return; }

		if (CRT_TOOL.ins.uiVertex.curAtlas == null) atlas.SetVertexPoint();
		CRT_TOOL.ins.uiVertex.SetVerIndexColor();
		atlas.UpdateColor();
		atlas.UpdatePrefabValue();
	}

	[MenuItem("HotKey/LoadIndex _&e")]
	static public bool LoadIndex()
	{
		if (CRT_TOOL.ins == null) { Debug.Log("CRT_TOOL 이 있고, 플레이 중에만 가능."); return false; }
		MeshAtlas atlas = Selection.activeGameObject.GetComponent<MeshAtlas>();
		if (atlas == null) { Debug.Log("선택한 Atlas가 없습니다."); return false; }

		if (CRT_TOOL.ins.uiVertex.curAtlas == null) atlas.SetVertexPoint();

		return CRT_TOOL.ins.uiVertex.ChangeCurrentColorVertex();
	}

	[MenuItem("HotKey/ApplyPrefab _&a")]
	static public void ApplyPrefab()
	{
		if (CRT_TOOL.ins == null) { Debug.Log("CRT_TOOL 이 있고, 플레이 중에만 가능."); return; }
		MeshAtlas atlas = Selection.activeGameObject.GetComponent<MeshAtlas>();
		if (atlas == null) { Debug.Log("선택한 Atlas가 없습니다."); return; }

		atlas.UpdatePrefabValue();
	}

	[MenuItem("HotKey/AddArray _&q")]
	static public void AddArray()
	{
		if (CRT_TOOL.ins == null) { Debug.Log("CRT_TOOL 이 있고, 플레이 중에만 가능."); return; }
		MeshAtlas atlas = Selection.activeGameObject.GetComponent<MeshAtlas>();
		if (atlas == null) { Debug.Log("선택한 Atlas가 없습니다."); return; }

		Color[] tmpCol = new Color[atlas.vertex_col.Length + 1];
		string[] tmpIdx = new string[tmpCol.Length];

		for (int i = 0; i < tmpCol.Length - 1; i++)
		{
			tmpCol[i] = atlas.vertex_col[i];
			tmpIdx[i] = atlas.vertex_index[i];
		}

		atlas.vertex_col = tmpCol;
		atlas.vertex_index = tmpIdx;
		atlas.UpdatePrefabValue();

	}
	[MenuItem("HotKey/MinusArray _&w")]
	static public void MinusArray()
	{
		if (CRT_TOOL.ins == null) { Debug.Log("CRT_TOOL 이 있고, 플레이 중에만 가능."); return; }
		MeshAtlas atlas = Selection.activeGameObject.GetComponent<MeshAtlas>();
		if (atlas == null) { Debug.Log("선택한 Atlas가 없습니다."); return; }

		Color[] tmpCol = new Color[atlas.vertex_col.Length - 1];
		string[] tmpIdx = new string[tmpCol.Length];

		for (int i = 0; i < tmpCol.Length; i++)
		{
			tmpCol[i] = atlas.vertex_col[i];
			tmpIdx[i] = atlas.vertex_index[i];
		}

		atlas.vertex_col = tmpCol;
		atlas.vertex_index = tmpIdx;
		atlas.UpdatePrefabValue();
	}

	[MenuItem("HotKey/SelectArray0 _&0")]
	static public void SelectArray0() { ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR00); }
	[MenuItem("HotKey/SelectArray1 _&1")]
	static public void SelectArray1() { ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR01); }
	[MenuItem("HotKey/SelectArray2 _&2")]
	static public void SelectArray2() { ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR02); }
	[MenuItem("HotKey/SelectArray3 _&3")]
	static public void SelectArray3() { ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR03); }
	[MenuItem("HotKey/SelectArray4 _&4")]
	static public void SelectArray4() {	ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR04); }
	[MenuItem("HotKey/SelectArray5 _&5")]
	static public void SelectArray5() { ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR05); }
	[MenuItem("HotKey/SelectArray6 _&6")]
	static public void SelectArray6() {	ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR06); }
	[MenuItem("HotKey/SelectArray7 _&7")]
	static public void SelectArray7() { ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR07); }
	[MenuItem("HotKey/SelectArray8 _&8")]
	static public void SelectArray8() {	ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR08); }
	[MenuItem("HotKey/SelectArray9 _&9")]
	static public void SelectArray9() {	ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR09); }

	static private void ChangeSetColor(UIVertexPoint.INDEX_NUMBER value)
	{
		if (CRT_TOOL.ins == null) { Debug.Log("CRT_TOOL 이 있고, 플레이 중에만 가능."); return; }
		MeshAtlas atlas = Selection.activeGameObject.GetComponent<MeshAtlas>();
		if (atlas == null) { Debug.Log("선택한 Atlas가 없습니다."); return; }

		UIVertexPoint.INDEX_NUMBER back = CRT_TOOL.ins.uiVertex.SET_COLOR;
		CRT_TOOL.ins.uiVertex.SET_COLOR = value;
		if( LoadIndex() == false) CRT_TOOL.ins.uiVertex.SET_COLOR = back;
	}

	[MenuItem("HotKey/SelectArrayPlus _&]")]
	static public void SelectArrayPlus()
	{
		if (CRT_TOOL.ins == null) { Debug.Log("CRT_TOOL 이 있고, 플레이 중에만 가능."); return; }
		MeshAtlas atlas = Selection.activeGameObject.GetComponent<MeshAtlas>();
		if (atlas == null) { Debug.Log("선택한 Atlas가 없습니다."); return; }
		switch (CRT_TOOL.ins.uiVertex.SET_COLOR)
		{
			case UIVertexPoint.INDEX_NUMBER.COLOR00: ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR01); break;
			case UIVertexPoint.INDEX_NUMBER.COLOR01: ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR02); break;
			case UIVertexPoint.INDEX_NUMBER.COLOR02: ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR03); break;
			case UIVertexPoint.INDEX_NUMBER.COLOR03: ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR04); break;
			case UIVertexPoint.INDEX_NUMBER.COLOR04: ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR05); break;
			case UIVertexPoint.INDEX_NUMBER.COLOR05: ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR06); break;
			case UIVertexPoint.INDEX_NUMBER.COLOR06: ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR07); break;
			case UIVertexPoint.INDEX_NUMBER.COLOR07: ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR08); break;
			case UIVertexPoint.INDEX_NUMBER.COLOR08: ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR09); break;
		}
	}
	[MenuItem("HotKey/SelectArrayMinus _&[")]
	static public void SelectArrayMinus()
	{
		if (CRT_TOOL.ins == null) { Debug.Log("CRT_TOOL 이 있고, 플레이 중에만 가능."); return; }
		MeshAtlas atlas = Selection.activeGameObject.GetComponent<MeshAtlas>();
		if (atlas == null) { Debug.Log("선택한 Atlas가 없습니다."); return; }
		switch (CRT_TOOL.ins.uiVertex.SET_COLOR)
		{
			case UIVertexPoint.INDEX_NUMBER.COLOR01: ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR00); break;
			case UIVertexPoint.INDEX_NUMBER.COLOR02: ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR01); break;
			case UIVertexPoint.INDEX_NUMBER.COLOR03: ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR02); break;
			case UIVertexPoint.INDEX_NUMBER.COLOR04: ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR03); break;
			case UIVertexPoint.INDEX_NUMBER.COLOR05: ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR04); break;
			case UIVertexPoint.INDEX_NUMBER.COLOR06: ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR05); break;
			case UIVertexPoint.INDEX_NUMBER.COLOR07: ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR06); break;
			case UIVertexPoint.INDEX_NUMBER.COLOR08: ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR07); break;
			case UIVertexPoint.INDEX_NUMBER.COLOR09: ChangeSetColor(UIVertexPoint.INDEX_NUMBER.COLOR08); break;
		}
	}

}