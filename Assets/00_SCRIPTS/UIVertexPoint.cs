using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VertexPoint
{
	public ButtonObject btn;
	public Vector3 vecCube;
	public string strIdx;
}

public class UIVertexPoint : MonoBehaviour
{
	public enum INDEX_NUMBER
	{
		COLOR00 = 0,
		COLOR01, COLOR02, COLOR03, COLOR04, COLOR05, COLOR06, COLOR07, COLOR08, COLOR09,
	}

	[HideInInspector]
	public INDEX_NUMBER SET_COLOR;
	//[HideInInspector]
	//public INDEX_NUMBER CHANGE_COLOR;

	public GameObject obj;
	public RectTransform rect;

	public ButtonObject prefab;
	
	private Dictionary<string, VertexPoint> dic;
	private Dictionary<int, string> dicBtn;

	[HideInInspector]
	public MeshAtlas curAtlas;

	private Vector3 vec3;
	

	public void Init()
	{
		prefab.obj.SetActive(false);
		dic = new Dictionary<string, VertexPoint>();
		dicBtn = new Dictionary<int, string>();
		obj.SetActive(false);
	}

	public void ShowList(MeshAtlas atlas)
	{
		HideList();
		curAtlas = atlas;
		obj.SetActive(true);
		ButtonObject btn;
		string str;

		for (int i = 0; i < atlas.listVertex.Count; i++)
		{
			atlas.listVertex[i].obj.SetActive(false);
			str = atlas.listVertex[i].tran.position.x.ToString() + atlas.listVertex[i].tran.position.y.ToString() + atlas.listVertex[i].tran.position.z.ToString();
			if (dic.ContainsKey(str) == true)
			{
				dic[str].strIdx += "," + i.ToString();
				continue;
			}
			dic.Add(str, new VertexPoint());
			dic[str].btn = GameObject.Instantiate(prefab.obj).GetComponent<ButtonObject>();
		
			btn = dic[str].btn;
			btn.tran.SetParent(rect);
			btn.tran.localScale = Vector3.one;
			btn.tran.localRotation = Quaternion.identity;

			dic[str].vecCube = atlas.listVertex[i].tran.position;
			dic[str].strIdx = i.ToString();
			dicBtn.Add(i, str);
			
			//btn.tran.localPosition = vec3;
			btn.obj.SetActive(true);
			btn.txt.text = i.ToString();
			btn.obj.name = i.ToString();

			string keystr = str;
			btn.btn.onClick.AddListener(() => ClickBtn(keystr));
		}

		UpdatePosition();
	}

	private void SetVertextList(ref string str, string value)
	{
		int pos = str.IndexOf("," + value + ",");

		if (pos != -1)
		{   //중간 위치값 삭제
			curAtlas.VERTEX_INDEX = curAtlas.VERTEX_INDEX.Substring(0, pos) + curAtlas.VERTEX_INDEX.Substring(pos + value.Length + 1);
			return;
		}
		
		pos = str.IndexOf("," + value);
		if (pos != -1 && pos + value.Length + 1 != str.Length) pos = -1;
		if (pos != -1)
		{   //맨뒤 위치값 삭제
			curAtlas.VERTEX_INDEX = curAtlas.VERTEX_INDEX.Substring(0, pos);
			return;
		}

		pos = str.IndexOf(value + ",");
		if (pos != -1 && str.IndexOf(",") != value.Length) pos = -1;
		if (pos != -1)
		{   //맨앞 위치값 삭제
			curAtlas.VERTEX_INDEX = curAtlas.VERTEX_INDEX.Substring(value.Length + 1);
			return;
		}
		
		//위치값 추가
		str += string.IsNullOrEmpty(str) ? string.Empty : ",";
		str += value;

	}

	private void ClickBtn(string value)
	{
		/*
		string str = "1,10,20";
		string a = "20";
		int pos = str.IndexOf("," + a + ",");
		if (pos == -1)
		{
			pos = str.IndexOf("," + a);
			Debug.Log("pos + a.Length : "+ (pos + a.Length + 1) + " / str.Length:" + str.Length);
			if (pos != -1 && pos + a.Length + 1!= str.Length) pos = -1;
		}
		if (pos == -1)
		{
			pos = str.IndexOf(a + ",");
			Debug.Log("str.IndexOf(',') : " + str.IndexOf(",") + " / a.Length:" + a.Length);
			if (pos != -1 && str.IndexOf(",") != a.Length) pos = -1;
		}
		Debug.Log(pos);
		return;
		*/

		SetVertextList(ref curAtlas.VERTEX_INDEX, dic[value].btn.txt.text);
		SetVertextList(ref curAtlas.PRINT_VERTEX, dic[value].strIdx);

		/*
		int idxcnt = dic[value].btn.txt.text.Length + 1;
		int prtcnt = dic[value].strIdx.Length + 1;

		int idxpos = curAtlas.VERTEX_INDEX.IndexOf("," + dic[value].btn.txt.text + ",");
		int prtpos = curAtlas.PRINT_VERTEX.IndexOf("," + dic[value].strIdx + ",");
		//Debug.Log("curAtlas.VERTEX_INDEX:"+ curAtlas.VERTEX_INDEX);
		//Debug.Log("curAtlas.PRINT_VERTEX:" + curAtlas.PRINT_VERTEX);
		if (idxpos != -1)
		{   //위치 값 삭제
			//Debug.Log("["+dic[value].btn.txt.text + "] 이미 있음.");
			//Debug.Log(curAtlas.VERTEX_INDEX.Substring(0, idxpos) + curAtlas.VERTEX_INDEX.Substring(idxpos + idxcnt));
			//Debug.Log(curAtlas.PRINT_VERTEX.Substring(0, prtpos) + curAtlas.PRINT_VERTEX.Substring(prtpos + prtcnt));

			Debug.Log(curAtlas.VERTEX_INDEX);
			Debug.Log(curAtlas.PRINT_VERTEX);
			Debug.Log("prtpos:" + prtpos + " / prtcnt:" + prtcnt + " / idxpos:" + idxpos);

			curAtlas.VERTEX_INDEX = curAtlas.VERTEX_INDEX.Substring(0, idxpos) + curAtlas.VERTEX_INDEX.Substring(idxpos + idxcnt);
			curAtlas.PRINT_VERTEX = curAtlas.PRINT_VERTEX.Substring(0, prtpos) + curAtlas.PRINT_VERTEX.Substring(prtpos + prtcnt);

			return;
		}
		idxpos = curAtlas.VERTEX_INDEX.IndexOf("," + dic[value].btn.txt.text);
		prtpos = curAtlas.PRINT_VERTEX.IndexOf("," + dic[value].strIdx);
		if (idxpos != -1)
		{   //위치 값 삭제  
			//Debug.Log("[" + dic[value].btn.txt.text + "] 이미 있음.");
			curAtlas.VERTEX_INDEX = curAtlas.VERTEX_INDEX.Substring(0, idxpos) + curAtlas.VERTEX_INDEX.Substring(idxpos + idxcnt);
			curAtlas.PRINT_VERTEX = curAtlas.PRINT_VERTEX.Substring(0, prtpos) + curAtlas.PRINT_VERTEX.Substring(prtpos + prtcnt);
			return;
		}


		//위치값 추가

		curAtlas.PRINT_VERTEX += string.IsNullOrEmpty(curAtlas.PRINT_VERTEX) ? "" : ",";
		curAtlas.PRINT_VERTEX += dic[value].strIdx;

		curAtlas.VERTEX_INDEX += string.IsNullOrEmpty(curAtlas.VERTEX_INDEX) ? "" : ",";
		curAtlas.VERTEX_INDEX += dic[value].btn.txt.text;
		*/
	}

	public void Update()
	{
		if (obj.activeSelf == false) return;
		UpdatePosition();
	}

	public void UpdatePosition()
	{
		if (obj.activeSelf == false) return;
		
		foreach (string key in dic.Keys)
		{
			vec3 = dic[key].vecCube;
			ChangeVec3to2(ref vec3);
			dic[key].btn.tran.localPosition = vec3;
			dic[key].btn.txt.color = Color.white;
			//atlas.VERTEX_INDEX에 있는 내용 색상 값 표시
		}
		if (string.IsNullOrEmpty(curAtlas.VERTEX_INDEX)) return;
		string[] arr = curAtlas.VERTEX_INDEX.Split(',');
		for (int i = 0; i < arr.Length; i++)
		{
			if (string.IsNullOrEmpty(arr[i])) continue;
			if(dicBtn.ContainsKey(int.Parse(arr[i])))
			{
				dic[dicBtn[int.Parse(arr[i])]].btn.txt.color = Color.cyan;
			}
		}
	}

	/// <summary>
	/// 색상 위치 값 적용
	/// </summary>
	public void SetVerIndexColor()
	{
		if (string.IsNullOrEmpty(curAtlas.VERTEX_INDEX))
		{
			Debug.Log("설정할 VERTEX_INDEX 값을 입력해야함.");
			return;
		}
		int idx = (int)SET_COLOR;
		string str = "";

		string[] arr = curAtlas.VERTEX_INDEX.Split(',');
		VertexPoint tmp;
		for (int i = 0; i < arr.Length; i++)
		{
			tmp = dic[dicBtn[int.Parse(arr[i])]];
			str += string.IsNullOrEmpty(str) ? "" : ",";
			str += tmp.strIdx;
		}
		curAtlas.vertex_index[idx] = str;
	}

	/// <summary>
	/// curAtlas.vertex_index[CHANGE_COLOR]의 내용 curAtlas.VERTEX_INDEX로 변환
	/// </summary>
	public bool ChangeCurrentColorVertex()
	{
		if (curAtlas.vertex_index.Length <= (int)SET_COLOR)
		{
			Debug.LogWarning("COLOR 배열 길이가 " +((int)SET_COLOR)+" 보다 적습니다.");
			return false;
		}

		curAtlas.PRINT_VERTEX = curAtlas.vertex_index[(int)SET_COLOR];
		curAtlas.VERTEX_INDEX = "";

		string[] arr = curAtlas.vertex_index[(int)SET_COLOR].Split(',');
		for(int i =0; i<arr.Length; i++)
		{
			if(dicBtn.ContainsKey(int.Parse(arr[i])) == true)
			{
				curAtlas.VERTEX_INDEX += string.IsNullOrEmpty(curAtlas.VERTEX_INDEX) ? "" : ",";
				curAtlas.VERTEX_INDEX += arr[i];
			}
		}
		return true;
	}

	public void ChangeVec3to2(ref Vector3 value)
	{
		value = CRT_TOOL.ins.cam.WorldToScreenPoint(value);

		value.x -= CRT_TOOL.ins.cam.pixelWidth * 0.5f;
		value.y -= CRT_TOOL.ins.cam.pixelHeight * 0.5f;

		value.x /= CRT_TOOL.ins.canvas.scaleFactor;
		value.y /= CRT_TOOL.ins.canvas.scaleFactor;

		value.z = 0f;
	}

	public void HideList()
	{
		foreach (string key in dic.Keys)
		{
			GameObject.Destroy(dic[key].btn.obj);
		}
		dic.Clear();
		dicBtn.Clear();

		obj.SetActive(false);
	}
}
