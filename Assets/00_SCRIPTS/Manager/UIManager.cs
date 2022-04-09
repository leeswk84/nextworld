using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour 
{
	public enum MODE 
	{
		BUILD,
		MOVE
	}

	[HideInInspector]
	public MODE PLAY_MODE = MODE.BUILD;

	public UIBuildList build;
	public UISelect select;
	public UIStatus status;
	public UISkybox skybox;
	public UIPopup popup;
	[Header("")]

	public Canvas canvas;
	public ButtonObject btnBuild, btnSave, btnLoad, btnBack, btnReset, btnChangeMove, btnChangeBuild;
	public ButtonObject btnTop, btnBottom, btnLeft, btnRight;

	public ButtonObject btnCRTtool;

	[Header("")]
	public GameObject panelBuild;
	public GameObject panelMove;
	public UILogin login;
	[Header("")]
	public GameObject objLockPlane;
	public RectTransform rectLockPlane;
	public Image imgLockPlane;
	[HideInInspector]
	public Color colPlaneShow;
	[HideInInspector]
	public Color colPlaneHide;

	[HideInInspector]
	public Color colWhiteShow;
	[HideInInspector]
	public Color colWhiteHide;

	public GameObject objTopGradation;
	public RectTransform rectTopGradation;

	[Header("")]
	public Text lblGrdPos;
	public Text txtTest;

	public int layer_ui;

	private Vector2 vec2;

	public void Init () 
	{
		layer_ui = 1 << LayerMask.NameToLayer(TxtMgr.layer_UI);
		RenderSettings.fog = false;

		colPlaneShow = imgLockPlane.color;
		colPlaneShow.a = 1f;
		colPlaneHide = imgLockPlane.color;
		colPlaneHide.a = 0f;
		objTopGradation.SetActive(false);

		colWhiteHide =
		colWhiteShow = Color.white;
		colWhiteHide.a = 0f;

		btnBuild.btn.onClick.AddListener(ClickBuild);
		
		btnSave.btn.onClick.AddListener(ClickSave);
		btnLoad.btn.onClick.AddListener(ClickLoad);
		//btnBack.btn.onClick.AddListener(ClickBack);
		btnReset.btn.onClick.AddListener(ClickReset);
		btnChangeMove.btn.onClick.AddListener(ClickChangeModeMove);
		btnChangeBuild.btn.onClick.AddListener(ClickChangeModeBuild);
		btnCRTtool.btn.onClick.AddListener(ClickCRTtool);

		btnTop.btn.onClick.AddListener(ClickGroundTop);
		btnBottom.btn.onClick.AddListener(ClickGroundBottom);
		btnLeft.btn.onClick.AddListener(ClickGroundLeft);
		btnRight.btn.onClick.AddListener(ClickGroundRight);

		popup.Init();
		select.Init();
		status.Init();
		RefreshMode();
		build.Init();
		login.Init();

		GameMgr.ins.mgrCam.cam.clearFlags = CameraClearFlags.Skybox;

		panelMove.SetActive(false);
		login.obj.SetActive(true);

#if UNITY_EDITOR
		btnBuild.obj.SetActive(true);
		btnCRTtool.obj.SetActive(true);
#else
		btnBuild.obj.SetActive(false);
		btnCRTtool.obj.SetActive(false);
#endif

	}

	public void SetHideEdit()
	{
		GameMgr.ins.mgrGround.setSelIdx(-1);
		select.pnlFocus.SetActive(false);
		select.ClickCancel();
	}

	/// <summary> 초기화 </summary>
	private void ClickReset()
	{
		for (int i = 0; i < GameMgr.ins.mgrBlock.posBlocks.Length; i++)
		{
			GameMgr.ins.mgrBlock.posBlocks[i].mgrRoad.Reset();
			GameMgr.ins.mgrBlock.posBlocks[i].mgrBuild.Reset();
			GameMgr.ins.mgrBlock.posBlocks[i].mgrGround.ResetGround(true);
		}
	}

	/// <summary> 건물 짖기 리스트  </summary>
	private void ClickBuild() 
	{
		build.Open();
	}
	private void ClickCRTtool()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(1);
	}
	/// <summary> 저장하기 </summary>
	private void ClickSave()
	{
		popup.ShowLock(TxtMgr.TYPE.POPUP_SAVE_DOING);
		GameMgr.ins.mgrSave.OnSave();
	}

	/// <summary> 불러오기 </summary>
	public void ClickLoad()
	{
		GameMgr.ins.mgrSave.OnLoad();
		SetHideEdit();
	}
	/*
	/// <summary> 뒤로 </summary>
	private void ClickBack()
	{
		GameMgr.ins.mgrSave.OnBack();
		SetHideEdit();
	}
    */
	public void ClickChangeModeMove()
	{
		SetChangeModeMove();
		GameMgr.ins.PLAYER.mini.obj.SetActive(true);
	}

	public void SetChangeModeMove()
	{
		GameMgr.ins.mgrSave.OnSave();
		
		PLAY_MODE = MODE.MOVE;
		GameMgr.ins.mgrSide.SetSideMoveMode();
		RenderSettings.fog = true;
		RenderSettings.fogMode = FogMode.Linear;
		RenderSettings.fogStartDistance = 14f;// fogstart;
		RenderSettings.fogEndDistance = 15f;// fogend;

		GameMgr.ins.mgrCam.cam.clearFlags = CameraClearFlags.Depth;

		//Debug.Log("start:" + RenderSettings.fogStartDistance + ",end:"+RenderSettings.fogEndDistance);

		//GameMgr.ins.mgrMoveCam.IntroMoveCam(35f, 40f, -5f);
		//GameMgr.ins.mgrMoveCam.IntroMoveCam(45f, 40f, -3f);
		//GameMgr.ins.mgrMoveCam.IntroMoveCam(0f, 40f, -3f);
		GameMgr.ins.mgrBlock.RefreshGroundColor();
		//GameMgr.ins.mgrMoveCam.IntroMoveCam(10f, 58f, -4.5f);
		GameMgr.ins.mgrMoveCam.IntroMoveCam(0f, 45f, -6.2f);
		//16.5
		GameMgr.ins.mgrMoveCam.playerRay.cam_outline.farClipPlane =
		GameMgr.ins.mgrCam.cam.farClipPlane = 16.5f;
		
		GameMgr.ins.mgrField.backFilter.gameObject.SetActive(false);
		//GameMgr.ins.mgrField.backFilter2.transform.parent = GameMgr.ins.mgrBlocks[1].block.transform;
		//GameMgr.ins.mgrBlocks[1].mgrGround.filter.transform.parent = GameMgr.ins.mgrField.backFilter.transform.parent;
		RefreshMode();
		
		GameMgr.ins.mgrMoveCam.StartMove();
	}

	public void ClickChangeModeBuild()
	{
		PLAY_MODE = MODE.BUILD;
		GameMgr.ins.mgrSide.SetSideMoveMode();
		GameMgr.ins.mgrBlock.RefreshGroundColor();
		RenderSettings.fog = false;
		GameMgr.ins.mgrCam.cam.farClipPlane = 100f;
		GameMgr.ins.mgrMoveCam.IntroMoveCam(47f, 45f, -11f);
		GameMgr.ins.mgrMoveCam.Move(0f, 0f);

		GameMgr.ins.mgrCam.cam.clearFlags = CameraClearFlags.Skybox;

		//GameMgr.ins.mgrField.backFilter2.transform.parent = GameMgr.ins.mgrField.backFilter.transform;

		GameMgr.ins.mgrField.backFilter.gameObject.SetActive(true);
		
		//GameMgr.ins.mgrField.SetBackRoundCol();
		RefreshMode();
		GameMgr.ins.mgrMoveCam.EndMove();
	}

	private void RefreshMode() 
	{
		panelBuild.SetActive(PLAY_MODE == MODE.BUILD);
		panelMove.SetActive(PLAY_MODE == MODE.MOVE);

		if (login.obj.activeSelf) panelMove.SetActive(false);

		GameMgr.ins.PLAYER.mini.obj.SetActive(false);
		GameMgr.ins.mgrGround.SetHideEdit();
	}
	private void ClickGroundTop()
	{ GameMgr.ins.mgrSave.MoveIndex(SaveManager.DIR.TOP); }
	private void ClickGroundBottom()
	{ GameMgr.ins.mgrSave.MoveIndex(SaveManager.DIR.BOTTOM); }
	private void ClickGroundLeft()
	{ GameMgr.ins.mgrSave.MoveIndex(SaveManager.DIR.LEFT); }
	private void ClickGroundRight()
	{ GameMgr.ins.mgrSave.MoveIndex(SaveManager.DIR.RIGHT); }

	public void FogShow(System.Action fnc)
	{
		imgLockPlane.color = colPlaneHide;
		objLockPlane.SetActive(true);
		LeanTween.value(objLockPlane, MoveFogStart, 1f, 0f, 0.5f);
		LeanTween.value(objLockPlane, MoveFogEnd, 1f, 0f, 0.5f).setDelay(0.2f).setOnComplete(fnc);

		vec2.x = 0; vec2.y = -300;
		rectTopGradation.anchoredPosition = vec2;
		objTopGradation.SetActive(true);
		LeanTween.value(objTopGradation, MoveTopGradation, -300f, 100f, 0.5f);
		//-300  시작 100 도착 700 아웃
	}
	
	public void FogHide(bool isactive = true)//, System.Action fnc = null)
	{
		LeanTween.value(objTopGradation, MoveTopGradation, 100f, -300f, 0.5f).setOnComplete(()=> { objTopGradation.SetActive(false); });

		LeanTween.value(objLockPlane, MoveFogEnd, 0f, 1f, 0.5f);
		LeanTween.value(objLockPlane, MoveFogStart, 0f, 1f, 0.5f).setDelay(0.2f).setOnComplete(()=> 
		{
			//if(fnc != null) fnc();
			if(isactive) objLockPlane.SetActive(false);
		});
	}

	private void MoveFogStart(float value) { RenderSettings.fogStartDistance = 14f * value; }
	private void MoveFogEnd(float value) { RenderSettings.fogEndDistance = 15f * value; }
	private void MoveTopGradation(float value) { vec2.x = 0f; vec2.y = value; rectTopGradation.anchoredPosition = vec2; }

	public void FogReset()
	{
		RenderSettings.fogStartDistance = 14f;
		RenderSettings.fogEndDistance = 15f;
		objTopGradation.SetActive(false);
	}

	public void LockHide(bool isactive = true)
	{
		//Debug.Log("UI Hide");
		LeanTween.cancel(rectLockPlane);
		imgLockPlane.color = colWhiteShow;
		objLockPlane.SetActive(true);
		LeanTween.alpha(rectLockPlane, 0f, 0.5f).setEaseInOutQuad().setOnComplete(() => 
		{
			if (isactive) objLockPlane.SetActive(false);
			//Debug.Log("lock hide");
		});
	}

	public void CheckKey()
	{
		if (Input.GetKeyUp(KeyCode.Space))
		{
			GameMgr.ins.PLAYER.SetTouchAtack();
		}
		
		/*
		if (Input.GetKeyUp(KeyCode.S))
		{
			ClickSave();
		}
		if (Input.GetKeyUp(KeyCode.Q))
		{
			status.EditHP(-10);
		}
		if (Input.GetKeyUp(KeyCode.W))
		{
			status.EditHP(10);
		}
		if (Input.GetKeyUp(KeyCode.E))
		{
			status.EditNut(100);
		}
		if (Input.GetKeyUp(KeyCode.R))
		{
			status.EditNut(-50);
		}
		*/
	}
}
