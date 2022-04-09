using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIBuildList : MonoBehaviour 
{
	public Camera camBuild;

	public UIMapType mapType;

	public GameObject pnlBuild;
	public ButtonObject btnBack;
	public ScrollRect scroll;
	public RectTransform rectContent;

	[HideInInspector]
	public List<Build> listBuild;
	[HideInInspector]
	public Build ViewBuild;
	[HideInInspector]
	public bool isCreate = false;
	
	private int num;
	private Vector3 vec3;
	private Vector2 vec2;

	private Build SelectBuild;
	private Build tmpBuild;

	public void Init()
	{
		camBuild.enabled = false;
		listBuild = null;
		vec3 = new Vector3();
		pnlBuild.SetActive(false);
		isCreate = false;

		btnBack.btn.onClick.AddListener(ClickBuildBack);
		btnBack.fncPress = GameMgr.ins.mgrCam.OnGroundPress;

		mapType.Init();

	}

	public int GetCreateIndex() { return ViewBuild.GetIdx(); }

	public void Open() 
	{
		if (pnlBuild.activeSelf)
		{
			ClickBuildBack();
			return;
		}
		GameMgr.ins.mgrGround.SetHideEdit();
		camBuild.enabled = true;
		isCreate = true;
		if (listBuild == null) 
		{	//리스트 건물...
			listBuild = new List<Build>();
			listBuild.Add(null);
			for (num = 1; num < GameMgr.ins.mgrBuildList.listPrefab.Count; num++) 
			{
				tmpBuild = GameMgr.ins.mgrBuildList.GetBuild(num);
				listBuild.Add(tmpBuild);
				InitBuild(ref tmpBuild);
				tmpBuild.obj.SetActive(true);
				tmpBuild.name = "btnBuild" + num;
				tmpBuild.Init(num, -1);
				tmpBuild.col.enabled = true;
				int idx = num;
				tmpBuild.btnObject.btn.onClick.AddListener(() => ClickBuild(listBuild[idx]));
				//tmpBuild.col.center = Vector3.zero;
				//tmpBuild.col.size = Vector3.one;
			}
			vec2 = rectContent.sizeDelta;
			vec2.x = ((num - 1)* 160);
			rectContent.sizeDelta = vec2;
		}
		
		pnlBuild.SetActive(true);
		if(ViewBuild != null) ViewBuild.gameObject.SetActive(false);

		mapType.Show();
	}

	private void InitBuild(ref Build value) 
	{
		//value.transform.parent = transform;

		value.tran.SetParent(rectContent);

		//value.gameObject.layer = gameObject.layer;
		//value.filter.gameObject.layer = gameObject.layer;

		vec3.x = vec3.y = vec3.z = 100;
		value.transform.localScale = vec3;
		vec3.x = ((num - 1) * 160f) + 90;// - 230f;
		vec3.y = 0;
		vec3.z = -100;
		value.transform.localPosition = vec3;
		vec3.x = vec3.z = 0f;// 350f;
		vec3.y = 45f;
		value.transform.localEulerAngles = vec3;
	}

	public void ClickBuildBack()
	{
		camBuild.enabled = false;
		isCreate = false;
		if(ViewBuild != null) ViewBuild.gameObject.SetActive(false);
		pnlBuild.SetActive(false);
		mapType.Hide();
	}

	private void ClickBuild(Build value) 
	{
		//Debug.Log("ClickBuild :: " + obj.name);
		ClickBuildBack();
		SelectBuild = value;
		
		if (ViewBuild != null)
		{   //버튼 위에 보이는 선택한 건물
			GameObject.Destroy(ViewBuild.obj);
		}
		//ViewBuild = GameMgr.ins.mgrBuildList.GetBuild();//((GameObject)Instantiate(GameMgr.ins.mgrBuild.prefabBuild.obj)).GetComponent<Build>();
		ViewBuild = GameMgr.ins.mgrBuildList.GetBuild(value.GetIdx());

		InitBuild(ref ViewBuild);

		ViewBuild.gameObject.layer = gameObject.layer;
		//ViewBuild.filter.gameObject.layer = gameObject.layer;

		ViewBuild.gameObject.SetActive(false);
		ViewBuild.name = "SelectBuild";
		vec3.x = vec3.y = vec3.z = 50;
		ViewBuild.transform.localScale = vec3;
		ViewBuild.transform.parent = GameMgr.ins.mgrUI.btnBuild.transform.parent;
		vec3 = ViewBuild.transform.localPosition;
		vec3.x = -170f;
		vec3.y = 100f;
		ViewBuild.transform.localPosition = vec3;

		ViewBuild.Init(SelectBuild.GetIdx(), -1);
		ViewBuild.gameObject.SetActive(true);
		isCreate = true;
		GameMgr.ins.mgrBuild.SetViewBuildIndex(ViewBuild.GetIdx());
	}
}
